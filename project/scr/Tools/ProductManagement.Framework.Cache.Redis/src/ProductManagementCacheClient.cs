using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProductManagement.Framework.Cache.Redis.Configuration;
using ProductManagement.Framework.Cache.Redis.ServerIteration;
using StackExchange.Redis;

namespace ProductManagement.Framework.Cache.Redis
{
    public class ProductManagementCacheClient : ICacheRedisClient
    {
        private static Lazy<ConnectionMultiplexer> _lazyConnection;
        private readonly RedisConfig _config;

        private readonly ServerEnumerationStrategy _serverEnumerationStrategy;

        public ProductManagementCacheClient(IRedisCachingConfiguration redisCachingConfiguration) :
            this(redisCachingConfiguration.Serializer,
                redisCachingConfiguration.RedisConfig,
                redisCachingConfiguration.LogWriter)
        {
        }

        public ProductManagementCacheClient(ISerializer serializer, RedisConfig redisConfig, TextWriter logWriter)
        {
            _config = redisConfig ?? throw new ArgumentNullException(nameof(redisConfig));

            _serverEnumerationStrategy = redisConfig.ServerEnumerationStrategy ?? new ServerEnumerationStrategy();

            Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

            _lazyConnection =
                new Lazy<ConnectionMultiplexer>(() =>
                {
                    ThreadPool.SetMinThreads(200, 200);
                    var connection = ConnectionMultiplexer.Connect(_config.RedisConnect, logWriter);
                    //connection.PreserveAsyncOrder = false;
                    return connection;
                });
        }

        private static ConnectionMultiplexer ConnectionMultiplexer => _lazyConnection.Value;

        public RedisConfig RedisConfig => _config;

        public IConnectionMultiplexer Connection => ConnectionMultiplexer;

        public ISerializer Serializer { get; }

        public IDatabase Database => ConnectionMultiplexer.GetDatabase(_config.Database);


        public Task<bool> KeyExpireAsync(string key, DateTimeOffset expiresAt, CommandFlags flags = CommandFlags.None)
        {
            if (_config.CloseRedis)
                return Task.FromResult(false);

            var expiration = expiresAt.Subtract(DateTimeOffset.Now);

            return Database.KeyExpireAsync(key, expiration);
        }
        public Task<bool> KeyExpireAsync(string key, TimeSpan expiresIn, CommandFlags flags = CommandFlags.None)
        {
            if (_config.CloseRedis)
                return Task.FromResult(false);
            return Database.KeyExpireAsync(key, expiresIn);
        }


        public bool Exists(string key)
        {
            return Database.KeyExists(key);
        }

        public Task<bool> ExistsAsync(string key)
        {
            if (_config.CloseRedis)
                return Task.FromResult(false);
            return Database.KeyExistsAsync(key);
        }

        public bool Remove(string key, CommandFlags flags = CommandFlags.None)
        {
            if (_config.CloseRedis)
                return false;

            return Database.KeyDelete(key, flags);
        }

        public Task<bool> RemoveAsync(string key, CommandFlags flags = CommandFlags.None)
        {
            if (_config.CloseRedis)
                return Task.FromResult(false);

            return Database.KeyDeleteAsync(key, flags);
        }

        public void RemoveAll(IEnumerable<string> keys, CommandFlags flags = CommandFlags.None)
        {
            if (_config.CloseRedis)
                return;
            var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
            Database.KeyDelete(redisKeys, flags);
        }

        public Task RemoveAllAsync(IEnumerable<string> keys, CommandFlags flags = CommandFlags.None)
        {
            if (_config.CloseRedis)
                return Task.FromResult(0);

            var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
            return Database.KeyDeleteAsync(redisKeys, flags);
        }

        /// <summary>
        /// key的前缀批量删除
        /// </summary>
        /// <param name="key"></param>
        public long FuzzyRemove(string key, CommandFlags flags = CommandFlags.None)
        {
            var redisResult = Database.ScriptEvaluate(LuaScript.Prepare(
                //Redis的keys模糊查询：
                " local res = redis.call('KEYS', @keypattern) " +
                " return res "), new { @keypattern = key + "*" });

            if (!redisResult.IsNull)
            {
                var keys = ((string[])redisResult).Select(p => (RedisKey)p).ToArray();
                return Database.KeyDelete(keys, flags);  //删除一组key
            }
            return 0;
        }

        public T Get<T>(string key)
        {
            if (_config.CloseRedis)
                return default(T);

            var valueBytes = Database.StringGet(key);

            if (!valueBytes.HasValue)
                return default(T);

            return Serializer.Deserialize<T>(valueBytes);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            if (_config.CloseRedis)
                return default(T);

            var valueBytes = await Database.StringGetAsync(key);

            if (!valueBytes.HasValue)
                return default(T);

            return await Serializer.DeserializeAsync<T>(valueBytes);
        }

        public bool Add<T>(string key, T value)
        {
            if (_config.CloseRedis)
                return false;

            var entryBytes = Serializer.Serialize(value);

            return Database.StringSet(key, entryBytes);
        }

        public async Task<bool> AddAsync<T>(string key, T value)
        {
            if (_config.CloseRedis)
                return false;

            var entryBytes = await Serializer.SerializeAsync(value);

            return await Database.StringSetAsync(key, entryBytes);
        }

        public bool Replace<T>(string key, T value)
        {
            if (_config.CloseRedis)
                return false;

            return Add(key, value);
        }

        public Task<bool> ReplaceAsync<T>(string key, T value)
        {
            if (_config.CloseRedis)
                return Task.FromResult(false);

            return AddAsync(key, value);
        }

        public bool Add<T>(string key, T value, DateTimeOffset expiresAt)
        {
            if (_config.CloseRedis)
                return false;

            var entryBytes = Serializer.Serialize(value);
            var expiration = expiresAt.Subtract(DateTimeOffset.Now);

            return Database.StringSet(key, entryBytes, expiration);
        }

        public async Task<bool> AddAsync<T>(string key, T value, DateTimeOffset expiresAt, CommandFlags flags = CommandFlags.None)
        {
            if (_config.CloseRedis)
                return false;

            var entryBytes = await Serializer.SerializeAsync(value);
            var expiration = expiresAt.Subtract(DateTimeOffset.Now);

            return await Database.StringSetAsync(key, entryBytes, expiration, flags: flags);
        }

        public bool Replace<T>(string key, T value, DateTimeOffset expiresAt)
        {
            if (_config.CloseRedis)
                return false;

            return Add(key, value, expiresAt);
        }

        public Task<bool> ReplaceAsync<T>(string key, T value, DateTimeOffset expiresAt)
        {
            if (_config.CloseRedis)
                return Task.FromResult(false);

            return AddAsync(key, value, expiresAt);
        }

        public bool Add<T>(string key, T value, TimeSpan expiresIn, CommandFlags flags = CommandFlags.None)
        {
            if (_config.CloseRedis)
                return false;

            var entryBytes = Serializer.Serialize(value);

            return Database.StringSet(key, entryBytes, expiresIn);
        }

        public async Task<bool> AddAsync<T>(string key, T value, TimeSpan expiresIn, CommandFlags flags = CommandFlags.None)
        {
            if (_config.CloseRedis)
                return false;

            var entryBytes = await Serializer.SerializeAsync(value);

            return await Database.StringSetAsync(key, entryBytes, expiresIn, flags: flags);
        }

        public async Task<bool> AddStringAsync(string key, string value, TimeSpan expiresIn)
        {
            if (_config.CloseRedis)
                return false;

            return await Database.StringSetAsync(key, value, expiresIn);
        }

        public bool Replace<T>(string key, T value, TimeSpan expiresIn)
        {
            if (_config.CloseRedis)
                return false;

            return Add(key, value, expiresIn);
        }

        public Task<bool> ReplaceAsync<T>(string key, T value, TimeSpan expiresIn)
        {
            if (_config.CloseRedis)
                return Task.FromResult(false);

            return AddAsync(key, value, expiresIn);
        }

        public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
        {
            if (_config.CloseRedis)
                return default(IDictionary<string, T>);

            var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
            var result = Database.StringGet(redisKeys);

            var dict = new Dictionary<string, T>(StringComparer.Ordinal);
            for (var index = 0; index < redisKeys.Length; index++)
            {
                var value = result[index];
                dict.Add(redisKeys[index], value == RedisValue.Null ? default(T) : Serializer.Deserialize<T>(value));
            }

            return dict;
        }

        public async Task<IDictionary<string, T>> GetAllAsync<T>(IEnumerable<string> keys)
        {
            if (_config.CloseRedis)
                return default(IDictionary<string, T>);

            var redisKeys = keys.Select(x => (RedisKey)x).ToArray();
            var result = await Database.StringGetAsync(redisKeys);
            var dict = new Dictionary<string, T>(StringComparer.Ordinal);
            for (var index = 0; index < redisKeys.Length; index++)
            {
                var value = result[index];
                dict.Add(redisKeys[index], value == RedisValue.Null ? default(T) : Serializer.Deserialize<T>(value));
            }
            return dict;
        }

        public bool AddAll<T>(IList<Tuple<string, T>> items)
        {
            if (_config.CloseRedis)
                return false;

            var values = items
                .Select(item => new KeyValuePair<RedisKey, RedisValue>(item.Item1, Serializer.Serialize(item.Item2)))
                .ToArray();

            return Database.StringSet(values);
        }

        public async Task<bool> AddAllAsync<T>(IList<Tuple<string, T>> items)
        {
            if (_config.CloseRedis)
                return false;

            var values = items
                .Select(item => new KeyValuePair<RedisKey, RedisValue>(item.Item1, Serializer.Serialize(item.Item2)))
                .ToArray();

            return await Database.StringSetAsync(values);
        }

        public bool SetAdd<T>(string key, T item) where T : class
        {
            if (_config.CloseRedis)
                return false;

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (item == null)
                throw new ArgumentNullException(nameof(item), "item cannot be null.");

            var serializedObject = Serializer.Serialize(item);

            return Database.SetAdd(key, serializedObject);
        }

        public async Task<bool> SetAddAsync<T>(string key, T item) where T : class
        {
            if (_config.CloseRedis)
                return false;

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (item == null)
                throw new ArgumentNullException(nameof(item), "item cannot be null.");

            var serializedObject = await Serializer.SerializeAsync(item);

            return await Database.SetAddAsync(key, serializedObject);
        }

        public long SetAddAll<T>(string key, params T[] items) where T : class
        {
            if (_config.CloseRedis)
                return 0;

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (items == null)
                throw new ArgumentNullException(nameof(items), "items cannot be null.");

            if (items.Any(item => item == null))
                throw new ArgumentException("items cannot contains any null item.", nameof(items));

            return Database.SetAdd(key,
                items.Select(item => Serializer.Serialize(item)).Select(x => (RedisValue)x).ToArray());
        }

        public async Task<long> SetAddAllAsync<T>(string key, params T[] items) where T : class
        {
            if (_config.CloseRedis)
                return 0;

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (items == null)
                throw new ArgumentNullException(nameof(items), "items cannot be null.");

            if (items.Any(item => item == null))
                throw new ArgumentException("items cannot contains any null item.", nameof(items));

            return await Database.SetAddAsync(key,
                items.Select(item => Serializer.Serialize(item)).Select(x => (RedisValue)x).ToArray());
        }

        public bool SetRemove<T>(string key, T item) where T : class
        {
            if (_config.CloseRedis)
                return false;

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (item == null)
                throw new ArgumentNullException(nameof(item), "item cannot be null.");

            var serializedObject = Serializer.Serialize(item);

            return Database.SetRemove(key, serializedObject);
        }

        public async Task<bool> SetRemoveAsync<T>(string key, T item) where T : class
        {
            if (_config.CloseRedis)
                return false;

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (item == null)
                throw new ArgumentNullException(nameof(item), "item cannot be null.");

            var serializedObject = await Serializer.SerializeAsync(item);

            return await Database.SetRemoveAsync(key, serializedObject);
        }

        public long SetRemoveAll<T>(string key, params T[] items) where T : class
        {
            if (_config.CloseRedis)
                return 0;

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (items == null)
                throw new ArgumentNullException(nameof(items), "items cannot be null.");

            if (items.Any(item => item == null))
                throw new ArgumentException("items cannot contains any null item.", nameof(items));

            return Database.SetRemove(key,
                items.Select(item => Serializer.Serialize(item)).Select(x => (RedisValue)x).ToArray());
        }

        public async Task<long> SetRemoveAllAsync<T>(string key, params T[] items) where T : class
        {
            if (_config.CloseRedis)
                return 0;

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (items == null)
                throw new ArgumentNullException(nameof(items), "items cannot be null.");

            if (items.Any(item => item == null))
                throw new ArgumentException("items cannot contains any null item.", nameof(items));

            return await Database.SetRemoveAsync(key,
                items.Select(item => Serializer.Serialize(item)).Select(x => (RedisValue)x).ToArray());
        }

        public string[] SetMember(string memberName)
        {
            if (_config.CloseRedis)
                return default(string[]);

            return Database.SetMembers(memberName).Select(x => x.ToString()).ToArray();
        }

        public async Task<string[]> SetMemberAsync(string memberName)
        {
            if (_config.CloseRedis)
                return default(string[]);

            return (await Database.SetMembersAsync(memberName)).Select(x => x.ToString()).ToArray();
        }

        public IEnumerable<T> SetMembers<T>(string key)
        {
            if (_config.CloseRedis)
                return default(IEnumerable<T>);

            var members = Database.SetMembers(key);
            return members.Select(m => m == RedisValue.Null ? default(T) : Serializer.Deserialize<T>(m));
        }

        public async Task<IEnumerable<T>> SetMembersAsync<T>(string key)
        {
            if (_config.CloseRedis)
                return default(IEnumerable<T>);

            var members = await Database.SetMembersAsync(key);

            return members.Select(m => m == RedisValue.Null ? default(T) : Serializer.Deserialize<T>(m));
        }

        public IEnumerable<string> SearchKeys(string pattern)
        {
            var keys = new HashSet<RedisKey>();

            var multiplexer = Database.Multiplexer as ConnectionMultiplexer;
            var servers = ServerIteratorFactory.GetServers(multiplexer, _serverEnumerationStrategy).ToArray();
            if (!servers.Any())
                throw new Exception("No server found to serve the KEYS command.");

            foreach (var server in servers)
            {
                var dbKeys = server.Keys(Database.Database, pattern);
                foreach (var dbKey in dbKeys)
                    if (!keys.Contains(dbKey))
                        keys.Add(dbKey);
            }

            return keys.Select(x => (string)x);
        }

        public Task<IEnumerable<string>> SearchKeysAsync(string pattern)
        {
            return Task.Factory.StartNew(() => SearchKeys(pattern));
        }

        public void FlushDb()
        {
            var endPoints = Database.Multiplexer.GetEndPoints();

            foreach (var endpoint in endPoints)
                Database.Multiplexer.GetServer(endpoint).FlushDatabase(Database.Database);
        }

        public async Task FlushDbAsync()
        {
            var endPoints = Database.Multiplexer.GetEndPoints();

            foreach (var endpoint in endPoints)
                await Database.Multiplexer.GetServer(endpoint).FlushDatabaseAsync(Database.Database);
        }

        public void Save(SaveType saveType)
        {
            var endPoints = Database.Multiplexer.GetEndPoints();

            foreach (var endpoint in endPoints)
                Database.Multiplexer.GetServer(endpoint).Save(saveType);
        }

        public async Task SaveAsync(SaveType saveType)
        {
            var endPoints = Database.Multiplexer.GetEndPoints();

            foreach (var endpoint in endPoints)
                await Database.Multiplexer.GetServer(endpoint).SaveAsync(saveType);
        }

        public Dictionary<string, string> GetInfo()
        {
            var info = Database.ScriptEvaluate("return redis.call('INFO')").ToString();

            return ParseInfo(info);
        }

        public async Task<Dictionary<string, string>> GetInfoAsync()
        {
            var info = (await Database.ScriptEvaluateAsync("return redis.call('INFO')")).ToString();

            return ParseInfo(info);
        }

        public long Publish<T>(RedisChannel channel, T message, CommandFlags flags = CommandFlags.None)
        {
            var sub = ConnectionMultiplexer.GetSubscriber();
            return sub.Publish(channel, Serializer.Serialize(message), flags);
        }

        public async Task<long> PublishAsync<T>(RedisChannel channel, T message, CommandFlags flags = CommandFlags.None)
        {
            var sub = ConnectionMultiplexer.GetSubscriber();
            return await sub.PublishAsync(channel, await Serializer.SerializeAsync(message), flags);
        }

        public void Subscribe<T>(RedisChannel channel, Action<T> handler, CommandFlags flags = CommandFlags.None)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var sub = ConnectionMultiplexer.GetSubscriber();
            sub.Subscribe(channel, (redisChannel, value) => handler(Serializer.Deserialize<T>(value)), flags);
        }

        public async Task SubscribeAsync<T>(RedisChannel channel, Func<T, Task> handler,
            CommandFlags flags = CommandFlags.None)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var sub = ConnectionMultiplexer.GetSubscriber();
            await sub.SubscribeAsync(channel,
                async (redisChannel, value) => await handler(Serializer.Deserialize<T>(value)), flags);
        }

        public void Unsubscribe<T>(RedisChannel channel, Action<T> handler, CommandFlags flags = CommandFlags.None)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var sub = ConnectionMultiplexer.GetSubscriber();
            sub.Unsubscribe(channel, (redisChannel, value) => handler(Serializer.Deserialize<T>(value)), flags);
        }

        public async Task UnsubscribeAsync<T>(RedisChannel channel, Func<T, Task> handler,
            CommandFlags flags = CommandFlags.None)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var sub = ConnectionMultiplexer.GetSubscriber();
            await sub.UnsubscribeAsync(channel, (redisChannel, value) => handler(Serializer.Deserialize<T>(value)),
                flags);
        }

        public void UnsubscribeAll(CommandFlags flags = CommandFlags.None)
        {
            var sub = ConnectionMultiplexer.GetSubscriber();
            sub.UnsubscribeAll(flags);
        }

        public async Task UnsubscribeAllAsync(CommandFlags flags = CommandFlags.None)
        {
            var sub = ConnectionMultiplexer.GetSubscriber();
            await sub.UnsubscribeAllAsync(flags);
        }

        public long ListLeftPush<T>(string key, T item) where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (item == null)
                throw new ArgumentNullException(nameof(item), "item cannot be null.");

            var serializedItem = Serializer.Serialize(item);

            return Database.ListLeftPush(key, serializedItem);
        }

        public async Task<long> ListLeftPushAsync<T>(string key, T item) where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            if (item == null)
                throw new ArgumentNullException(nameof(item), "item cannot be null.");

            var serializedItem = await Serializer.SerializeAsync(item);

            return await Database.ListLeftPushAsync(key, serializedItem);
        }

        public T ListRightPop<T>(string key) where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            var item = Database.ListRightPop(key);

            return item == RedisValue.Null ? null : Serializer.Deserialize<T>(item);
        }

        public async Task<T> ListRightPopAsync<T>(string key) where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            var item = await Database.ListRightPopAsync(key);

            if (item == RedisValue.Null) return null;

            return item == RedisValue.Null ? null : await Serializer.DeserializeAsync<T>(item);
        }

        public bool HashDelete(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashDelete(hashKey, key, commandFlags);
        }

        public long HashDelete(string hashKey, IEnumerable<string> keys, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashDelete(hashKey, keys.Select(x => (RedisValue)x).ToArray(), commandFlags);
        }

        public bool HashExists(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashExists(hashKey, key, commandFlags);
        }

        public T HashGet<T>(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisValue = Database.HashGet(hashKey, key, commandFlags);
            return redisValue.HasValue ? Serializer.Deserialize<T>(redisValue) : default(T);
        }

        public Dictionary<string, T> HashGet<T>(string hashKey, IEnumerable<string> keys,
            CommandFlags commandFlags = CommandFlags.None)
        {
            return keys.Select(x => new { key = x, value = HashGet<T>(hashKey, x, commandFlags) })
                .ToDictionary(kv => kv.key, kv => kv.value, StringComparer.Ordinal);
        }

        public Dictionary<string, T> HashGetAll<T>(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database
                .HashGetAll(hashKey, commandFlags)
                .ToDictionary(
                    x => x.Name.ToString(),
                    x => Serializer.Deserialize<T>(x.Value),
                    StringComparer.Ordinal);
        }

        public long HashIncerementBy(string hashKey, string key, long value,
            CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashIncrement(hashKey, key, value, commandFlags);
        }

        public double HashIncerementBy(string hashKey, string key, double value,
            CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashIncrement(hashKey, key, value, commandFlags);
        }

        public IEnumerable<string> HashKeys(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashKeys(hashKey, commandFlags).Select(x => x.ToString());
        }

        public long HashLength(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashLength(hashKey, commandFlags);
        }

        public bool HashSet<T>(string hashKey, string key, T value, bool nx = false,
            CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashSet(hashKey, key, Serializer.Serialize(value), nx ? When.NotExists : When.Always,
                commandFlags);
        }

        public void HashSet<T>(string hashKey, Dictionary<string, T> values,
            CommandFlags commandFlags = CommandFlags.None)
        {
            var entries = values.Select(kv => new HashEntry(kv.Key, Serializer.Serialize(kv.Value)));
            Database.HashSet(hashKey, entries.ToArray(), commandFlags);
        }

        public IEnumerable<T> HashValues<T>(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            return Database.HashValues(hashKey, commandFlags).Select(x => Serializer.Deserialize<T>(x));
        }

        public Dictionary<string, T> HashScan<T>(string hashKey, string pattern, int pageSize = 10,
            CommandFlags commandFlags = CommandFlags.None)
        {
            return Database
                .HashScan(hashKey, pattern, pageSize, commandFlags)
                .ToDictionary(x => x.Name.ToString(),
                    x => Serializer.Deserialize<T>(x.Value),
                    StringComparer.Ordinal);
        }

        public async Task<bool> HashDeleteAsync(string hashKey, string key,
            CommandFlags commandFlags = CommandFlags.None)
        {
            return await Database.HashDeleteAsync(hashKey, key, commandFlags);
        }

        public async Task<long> HashDeleteAsync(string hashKey, IEnumerable<string> keys,
            CommandFlags commandFlags = CommandFlags.None)
        {
            return await Database.HashDeleteAsync(hashKey, keys.Select(x => (RedisValue)x).ToArray(), commandFlags);
        }

        public async Task<bool> HashExistsAsync(string hashKey, string key,
            CommandFlags commandFlags = CommandFlags.None)
        {
            return await Database.HashExistsAsync(hashKey, key, commandFlags);
        }

        public async Task<T> HashGetAsync<T>(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisValue = await Database.HashGetAsync(hashKey, key, commandFlags);
            return redisValue.HasValue ? Serializer.Deserialize<T>(redisValue) : default(T);
        }

        public async Task<Dictionary<string, T>> HashGetAsync<T>(string hashKey, IEnumerable<string> keys,
            CommandFlags commandFlags = CommandFlags.None)
        {
            var result = new Dictionary<string, T>();
            foreach (var key in keys)
            {
                var value = await HashGetAsync<T>(hashKey, key, commandFlags);

                result.Add(key, value);
            }

            return result;
        }

        public async Task<Dictionary<string, T>> HashGetAllAsync<T>(string hashKey,
            CommandFlags commandFlags = CommandFlags.None)
        {
            return (await Database
                    .HashGetAllAsync(hashKey, commandFlags))
                .ToDictionary(
                    x => x.Name.ToString(),
                    x => Serializer.Deserialize<T>(x.Value),
                    StringComparer.Ordinal);
        }

        public async Task<long> HashIncerementByAsync(string hashKey, string key, long value,
            CommandFlags commandFlags = CommandFlags.None)
        {
            return await Database.HashIncrementAsync(hashKey, key, value, commandFlags);
        }

        public async Task<double> HashIncerementByAsync(string hashKey, string key, double value,
            CommandFlags commandFlags = CommandFlags.None)
        {
            return await Database.HashIncrementAsync(hashKey, key, value, commandFlags);
        }

        public async Task<IEnumerable<string>> HashKeysAsync(string hashKey,
            CommandFlags commandFlags = CommandFlags.None)
        {
            return (await Database.HashKeysAsync(hashKey, commandFlags)).Select(x => x.ToString());
        }

        public async Task<long> HashLengthAsync(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            return await Database.HashLengthAsync(hashKey, commandFlags);
        }

        public async Task<bool> HashSetAsync<T>(string hashKey, string key, T value, bool nx = false,
            CommandFlags commandFlags = CommandFlags.None)
        {
            return await Database.HashSetAsync(hashKey, key, Serializer.Serialize(value),
                nx ? When.NotExists : When.Always, commandFlags);
        }

        public async Task HashSetAsync<T>(string hashKey, IDictionary<string, T> values,
            CommandFlags commandFlags = CommandFlags.None)
        {
            var entries = values.Select(kv => new HashEntry(kv.Key, Serializer.Serialize(kv.Value)));
            await Database.HashSetAsync(hashKey, entries.ToArray(), commandFlags);
        }

        public async Task<IEnumerable<T>> HashValuesAsync<T>(string hashKey,
            CommandFlags commandFlags = CommandFlags.None)
        {
            return (await Database.HashValuesAsync(hashKey, commandFlags)).Select(x => Serializer.Deserialize<T>(x));
        }

        public async Task<Dictionary<string, T>> HashScanAsync<T>(string hashKey, string pattern, int pageSize = 10,
            CommandFlags commandFlags = CommandFlags.None)
        {
            return (await Task.Run(() => Database.HashScan(hashKey, pattern, pageSize, commandFlags)))
                .ToDictionary(x => x.Name.ToString(), x => Serializer.Deserialize<T>(x.Value), StringComparer.Ordinal);
        }


        /// <summary>
        /// 向有序集合添加一个或多个成员，或者更新已存在成员的分数
        /// </summary>
        public async Task<long> SortedSetAddAsync<T>(RedisKey key, IDictionary<T, double> pairs, CommandFlags flags = CommandFlags.None)
        {
            if (_config.CloseRedis)
                return default(long);
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            List<SortedSetEntry> Entry = new List<SortedSetEntry>();
            foreach (var pair in pairs)
            {
                Entry.Add(new SortedSetEntry(Serializer.Serialize(pair.Key), pair.Value));
            }
            return await Database.SortedSetAddAsync(key, Entry.ToArray(), flags);
        }


        /// <summary>
        /// 有序集合中对指定成员的分数加上增量 increment
        /// </summary>
        public async Task<double> SortedSetIncrementAsync<T>(RedisKey key, T member, double value,
             CommandFlags flags = CommandFlags.None)
        {
            if (_config.CloseRedis)
                return default(double);
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            return await Database.SortedSetIncrementAsync(key, Serializer.Serialize(member), value, flags);
        }

        /// <summary>
        /// 通过索引区间返回有序集合成指定区间内的成员
        /// </summary>
        public async Task<long> SortedSetLengthAsync(RedisKey key, double min, double max,
            Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            if (_config.CloseRedis)
                return default(long);
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            return await Database.SortedSetLengthAsync(key, min, max, exclude, flags);
        }
        /// <summary>
        /// 计算在有序集合中指定区间分数的成员数
        /// </summary>
        public async Task<long> SortedSetLengthByValueAsync(RedisKey key, double min, double max,
            Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            if (_config.CloseRedis)
                return default(long);
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            return await Database.SortedSetLengthByValueAsync(key, min, max, exclude, flags);
        }

        /// <summary>
        /// 返回有序集中指定区间内的成员
        /// </summary>
        public async Task<IEnumerable<T>> SortedSetRangeByRankAsync<T>(RedisKey key, long start = 0, long stop = 1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            if (_config.CloseRedis)
                return default(IEnumerable<T>);
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            return (await Database.SortedSetRangeByRankAsync(key, start, stop, order, flags)).Select(x => Serializer.Deserialize<T>(x));
        }


        /// <summary>
        /// 通过分数返回有序集合指定区间内的成员
        /// </summary>
        public async Task<Dictionary<T, double>> SortedSetRangeByRankWithScoresAsync<T>(RedisKey key, long start = 0, long stop = 1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            if (_config.CloseRedis)
                return default(Dictionary<T, double>);
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            return (await Database.SortedSetRangeByRankWithScoresAsync(key, start, stop, order, flags))
                    .ToDictionary(
                        x => Serializer.Deserialize<T>(x.Element),
                        x => x.Score);
        }


        /// <summary>
        /// 返回有序集中，成员的分数值
        /// </summary>
        public async Task<double?> SortedSetScoreAsync<T>(RedisKey key, T member)
        {
            if (_config.CloseRedis)
                return default(double?);
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            return await Database.SortedSetScoreAsync(key, Serializer.Serialize(member));
        }

        /// <summary>
        /// 计算给定的一个或多个有序集的并集
        /// </summary>
        public async Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, RedisKey destination,
            RedisKey[] keys, double[] weights, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            if (_config.CloseRedis)
                return default(long);
            if (string.IsNullOrEmpty(destination))
                throw new ArgumentException("key cannot be empty.", nameof(destination));
            return await Database.SortedSetCombineAndStoreAsync(operation, destination, keys, weights, aggregate, flags);
        }

        /// <summary>
        /// 移除有序集合中给定的分数区间的所有成员
        /// </summary>
        public async Task<long> SortedSetRemoveRangeByScoreAsync(RedisKey key, double start, double stop,
            Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            if (_config.CloseRedis)
                return default(long);
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            return await Database.SortedSetRemoveRangeByScoreAsync(key, start, stop, exclude, flags);
        }


        /// <summary>
        /// 移除有序集合中给定的排名区间的所有成员
        /// </summary>
        public async Task<long> SortedSetRemoveRangeByRankAsync(RedisKey key, long start, long stop,
             CommandFlags flags = CommandFlags.None)
        {
            if (_config.CloseRedis)
                return default(long);
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            return await Database.SortedSetRemoveRangeByRankAsync(key, start, stop, flags);
        }


        /// <summary>
        /// 移除有序集合中的一个或多个成员
        /// </summary>
        public async Task<long> SortedSetRemoveAsync<T>(RedisKey key, IEnumerable<T> numbers,
             CommandFlags flags = CommandFlags.None)
        {
            if (_config.CloseRedis)
                return default(long);
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.", nameof(key));

            var index = await Database.SortedSetRemoveAsync(key,
                numbers.Select(x => (RedisValue)Serializer.Serialize(x)).ToArray(), flags);
            return index;
        }

        public void Dispose()
        {
            ConnectionMultiplexer.Dispose();
        }

        private Dictionary<string, string> ParseInfo(string info)
        {
            var lines = info.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var data = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line) || line[0] == '#')
                    continue;

                var idx = line.IndexOf(':');
                if (idx > 0) // double check this line looks about right
                {
                    var key = line.Substring(0, idx);
                    var infoValue = line.Substring(idx + 1).Trim();

                    data.Add(key, infoValue);
                }
            }

            return data;
        }

        public async Task<string> GetStringAsync(string key)
        {
            if (_config.CloseRedis)
                return null;

            var valueBytes = await Database.StringGetAsync(key);

            return valueBytes.ToString();

        }

        public async Task<List<T>> ListPopAll<T>(string key) where T : class
        {
            if (_config.CloseRedis)
                return null;
            List<RedisValue> values = new List<RedisValue>();
            var length = await Database.ListLengthAsync(key);
            for (int i = 0; i < length; i++)
            {
                values.Add(await Database.ListLeftPopAsync(key));
            }
            if (values != null && values.Any())
            {
                var listResult = values.Select(v =>
                {
                    return Serializer.Deserialize<T>(v);
                }).ToList();
                return listResult;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> ListPush(string key, object value, TimeSpan? expiry = null)
        {
            if (_config.CloseRedis)
                return false;
            var json = await Serializer.SerializeAsync(value);
            var result = await Database.ListRightPushAsync(key, json);
            var setExpireResult = await Database.KeyExpireAsync(key, expiry);
            return result > 0;
        }
    }
}