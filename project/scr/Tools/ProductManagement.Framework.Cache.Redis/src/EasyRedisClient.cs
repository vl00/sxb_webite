using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Framework.Cache.Redis
{
    public class EasyRedisClient : IEasyRedisClient
    {
        public ICacheRedisClient CacheRedisClient { get; }

        private readonly ILogger<EasyRedisClient> _logger;

        public EasyRedisClient(ICacheRedisClient cacheRedisClient, ILogger<EasyRedisClient> logger)
        {
            CacheRedisClient = cacheRedisClient;
            _logger = logger;
        }



        /// <summary>
        /// 设置Key的过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiresAt">过期时间</param>
        /// <returns></returns>
        public async Task<bool> KeyExpireAsync(string key, DateTimeOffset expiresAt, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                return await CacheRedisClient.KeyExpireAsync(key, expiresAt, flags);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "设置Key的过期时间异常");

                return false;
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "设置Key的过期时间异常");

                return false;
            }
        }

        /// <summary>
        /// 设置Key的过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiresIn">时间差</param>
        /// <returns></returns>
        public async Task<bool> KeyExpireAsync(string key, TimeSpan expiresIn, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                return await CacheRedisClient.KeyExpireAsync(key, expiresIn, flags);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "设置Key的过期时间异常");

                return false;
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "设置Key的过期时间异常");

                return false;
            }
        }


        public async Task<bool> AddAsync<T>(string key, T value)
        {
            try
            {
                return await CacheRedisClient.AddAsync(key, value);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "添加缓存值异常");

                return false;
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "添加缓存值异常");

                return false;
            }
        }

        public async Task<bool> AddAsync<T>(string key, T value, DateTimeOffset expiresAt, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                return await CacheRedisClient.AddAsync(key, value, expiresAt,flags);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "添加缓存值异常");

                return false;
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "添加缓存值异常");

                return false;
            }
        }

        public async Task<bool> AddAsync<T>(string key, T value, TimeSpan expiresIn, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                return await CacheRedisClient.AddAsync(key, value, expiresIn, flags);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "添加缓存值异常");

                return false;
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "添加缓存值异常");

                return false;
            }
        }

        public async Task<bool> AddStringAsync(string key, string value, TimeSpan expiresIn)
        {
            try
            {
                return await CacheRedisClient.AddStringAsync(key, value, expiresIn);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "添加缓存值异常");

                return false;
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "添加缓存值异常");

                return false;
            }
        }


        public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> func, TimeSpan expiresIn)
        {
            try
            {
                if (CacheRedisClient.RedisConfig.CloseRedis)
                    return await func.Invoke();

                var valueBytes = await CacheRedisClient.Database.StringGetAsync(key);

                if (valueBytes.HasValue)
                    return await CacheRedisClient.Serializer.DeserializeAsync<T>(valueBytes);

                var value = await func.Invoke();
                if (value != null)
                {
                    var entryBytes = await CacheRedisClient.Serializer.SerializeAsync(value);

                    await CacheRedisClient.Database.StringSetAsync(key, entryBytes, expiresIn);
                }
                return value;
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "GetOrAdd缓存值异常");

                return await func.Invoke();
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "GetOrAdd缓存值异常");

                return await func.Invoke();
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                return await CacheRedisClient.ExistsAsync(key);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "判断缓存值异常");

                return false;
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "添加缓存值异常");

                return false;
            }
        }

        public async Task<IDictionary<string, T>> GetAllAsync<T>(IEnumerable<string> keys)
        {
            try
            {
                return await CacheRedisClient.GetAllAsync<T>(keys);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "获取缓存值集合异常");

                return default(IDictionary<string, T>);
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "添加缓存值异常");

                return default(IDictionary<string, T>);
            }
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> func)
        {
            try
            {
                if (CacheRedisClient.RedisConfig.CloseRedis)
                    return await func.Invoke();

                var valueBytes = await CacheRedisClient.Database.StringGetAsync(key);

                if (valueBytes.HasValue)
                    return await CacheRedisClient.Serializer.DeserializeAsync<T>(valueBytes);

                var value = await func.Invoke();
                if (value != null)
                {
                    var entryBytes = await CacheRedisClient.Serializer.SerializeAsync(value);

                    await CacheRedisClient.Database.StringSetAsync(key, entryBytes);
                }
                return value;
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "GetOrAdd缓存值异常");

                return await func.Invoke();
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "GetOrAdd缓存值异常");

                return await func.Invoke();
            }
        }

        public async Task<T> GetAsync<T>(string key)
        {
            try
            {
                return await CacheRedisClient.GetAsync<T>(key);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "获取缓存值异常");

                return default(T);
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "添加缓存值异常");

                return default(T);
            }
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<T> func)
        {
            try
            {
                if (CacheRedisClient.RedisConfig.CloseRedis)
                    return func.Invoke();

                var valueBytes = await CacheRedisClient.Database.StringGetAsync(key);

                if (valueBytes.HasValue)
                    return await CacheRedisClient.Serializer.DeserializeAsync<T>(valueBytes);

                var value = func.Invoke();
                if (value != null)
                {
                    var entryBytes = await CacheRedisClient.Serializer.SerializeAsync(value);

                    await CacheRedisClient.Database.StringSetAsync(key, entryBytes, flags: CommandFlags.FireAndForget);
                }
                return value;
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "GetOrAdd缓存值异常");

                return func.Invoke();
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "GetOrAdd缓存值异常");

                return func.Invoke();
            }
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> func, DateTimeOffset expiresAt)
        {
            try
            {
                if (CacheRedisClient.RedisConfig.CloseRedis)
                    return await func.Invoke();

                var valueBytes = await CacheRedisClient.Database.StringGetAsync(key);

                if (valueBytes.HasValue)
                    return await CacheRedisClient.Serializer.DeserializeAsync<T>(valueBytes);

                var value = await func.Invoke();
                if (value != null)
                {
                    var entryBytes = await CacheRedisClient.Serializer.SerializeAsync(value);
                    var expiration = expiresAt.Subtract(DateTimeOffset.Now);

                    await CacheRedisClient.Database.StringSetAsync(key, entryBytes, expiration, flags: CommandFlags.FireAndForget);
                }
                return value;

            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "GetOrAdd缓存值异常");

                return await func.Invoke();
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "GetOrAdd缓存值异常");

                return await func.Invoke();
            }
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<T> func, DateTimeOffset expiresAt)
        {
            try
            {
                if (CacheRedisClient.RedisConfig.CloseRedis)
                    return func.Invoke();

                var valueBytes = await CacheRedisClient.Database.StringGetAsync(key);

                if (valueBytes.HasValue)
                    return await CacheRedisClient.Serializer.DeserializeAsync<T>(valueBytes);

                var value = func.Invoke();
                if (value != null)
                {
                    var entryBytes = await CacheRedisClient.Serializer.SerializeAsync(value);
                    var expiration = expiresAt.Subtract(DateTimeOffset.Now);

                    await CacheRedisClient.Database.StringSetAsync(key, entryBytes, expiration);
                }
                return value;

            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "GetOrAdd缓存值异常");

                return func.Invoke();
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "GetOrAdd缓存值异常");

                return func.Invoke();
            }
        }

        public async Task<T> GetOrAddAsync<T>(string key, Func<T> func, TimeSpan expiresIn)
        {
            try
            {
                if (CacheRedisClient.RedisConfig.CloseRedis)
                    return func.Invoke();

                var valueBytes = await CacheRedisClient.Database.StringGetAsync(key);

                if (valueBytes.HasValue)
                    return await CacheRedisClient.Serializer.DeserializeAsync<T>(valueBytes);

                var value = func.Invoke();
                if (value != null)
                {
                    var entryBytes = await CacheRedisClient.Serializer.SerializeAsync(value);

                    await CacheRedisClient.Database.StringSetAsync(key, entryBytes, expiresIn, flags: CommandFlags.FireAndForget);
                }
                return value;
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "GetOrAdd缓存值异常");

                return func.Invoke();
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "GetOrAdd缓存值异常");

                return func.Invoke();
            }
        }


        public async Task<IEnumerable<T>> GetOrAddEnumerableAsync<T>(string key, Func<IEnumerable<T>> func, TimeSpan expiresIn)
        {
            try
            {
                if (CacheRedisClient.RedisConfig.CloseRedis)
                    return func.Invoke();

                var valueBytes = await CacheRedisClient.Database.StringGetAsync(key);

                if (valueBytes.HasValue)
                {
                    var data = await CacheRedisClient.Serializer.DeserializeAsync<IEnumerable<T>>(valueBytes);
                    if (data.Any())
                    {
                        return data;
                    }
                }

                var value = func.Invoke();
                if (value != null)
                {
                    var entryBytes = await CacheRedisClient.Serializer.SerializeAsync(value);

                    await CacheRedisClient.Database.StringSetAsync(key, entryBytes, expiresIn, flags: CommandFlags.FireAndForget);
                }
                return value;
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "GetOrAdd缓存值异常");

                return func.Invoke();
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "GetOrAdd缓存值异常");

                return func.Invoke();
            }
        }

        public async Task RemoveAllAsync(IEnumerable<string> keys, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                await CacheRedisClient.RemoveAllAsync(keys,flags);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "移除缓存值异常");
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "移除缓存值异常");
            }
        }

        public void FuzzyRemove(string key, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                CacheRedisClient.FuzzyRemove(key, flags);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "模糊移除缓存值异常");
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "模糊移除缓存值异常");
            }
        }

        public async Task<bool> RemoveAsync(string key, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                return await CacheRedisClient.RemoveAsync(key,flags);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "移除缓存值异常");

                return false;
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "移除缓存值异常");

                return false;
            }
        }

        public async Task<bool> ReplaceAsync<T>(string key, T value)
        {
            try
            {
                return await CacheRedisClient.ReplaceAsync(key, value);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "替换缓存值异常");

                return false;
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "替换缓存值异常");

                return false;
            }
        }

        public async Task<bool> ReplaceAsync<T>(string key, T value, DateTimeOffset expiresAt)
        {
            try
            {
                return await CacheRedisClient.ReplaceAsync(key, value, expiresAt);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "替换缓存值异常");

                return false;
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "替换缓存值异常");

                return false;
            }
        }

        public async Task<bool> ReplaceAsync<T>(string key, T value, TimeSpan expiresIn)
        {
            try
            {
                return await CacheRedisClient.ReplaceAsync(key, value, expiresIn);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "替换缓存值异常");

                return false;
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "替换缓存值异常");

                return false;
            }
        }


        //HashSet

        /// <summary>
        /// 获取哈希表中字段的数量
        /// </summary>
        /// <param name="hashKey"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task<long> HashLengthAsync(string hashKey, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                return await CacheRedisClient.HashLengthAsync(hashKey, flags);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "获取哈希表中字段的数量异常");
                return default(long);
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "获取哈希表中字段的数量超时");

                return default(long);
            }
        }

        /// <summary>
        /// 查看哈希表 key 中，指定的字段是否存在
        /// </summary>
        /// <param name="hashKey"></param>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task<bool> HashExistsAsync(string hashKey, string key, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                return await CacheRedisClient.HashExistsAsync(hashKey, key, flags);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "查看哈希表 key 中，指定的字段是否存在异常");
                return default(bool);
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "查看哈希表 key 中，指定的字段是否存在超时");

                return default(bool);
            }
        }
        /// <summary>
        /// 获取存储在哈希表中指定字段的值。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashKey"></param>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task<T> HashGetAsync<T>(string hashKey, string key, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                return await CacheRedisClient.HashGetAsync<T>(hashKey, key, flags);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "获取存储在哈希表中指定字段的值异常");
                return default(T);
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "获取存储在哈希表中指定字段的值超时");

                return default(T);
            }
        }
        /// <summary>
        /// 获取在哈希表中指定 key 的所有字段和值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashKey"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, T>> HashGetAllAsync<T>(string hashKey, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                return await CacheRedisClient.HashGetAllAsync<T>(hashKey, flags);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "获取在哈希表中指定 key 的所有字段和值异常");
                return default(Dictionary<string, T>);
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "获取在哈希表中指定 key 的所有字段和值超时");

                return default(Dictionary<string, T>);
            }
        }

        /// <summary>
        /// 删除一个字段
        /// </summary>
        /// <param name="hashKey"></param>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task<bool> HashDeleteAsync(string hashKey, string key, CommandFlags flags = CommandFlags.None)
        {
            string[] keys = new string[] { key };
            return await HashDeleteAsync(hashKey, keys, flags) > 0;
        }

        /// <summary>
        /// 删除一个或多个哈希表字段
        /// </summary>
        /// <param name="hashKey"></param>
        /// <param name="keys"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task<long> HashDeleteAsync(string hashKey, IEnumerable<string> keys, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                return await CacheRedisClient.HashDeleteAsync(hashKey, keys, flags);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "删除哈希表字段异常");
                return default(long);
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "删除哈希表字段超时");

                return default(long);
            }
        }


        /// <summary>
        /// 设置哈希表字段的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashKey"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="nx">是否存在不覆盖</param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task<bool> HashSetAsync<T>(string hashKey, string key, T value, bool nx = false,
            CommandFlags flags = CommandFlags.None)
        {
            try
            {
                return await CacheRedisClient.HashSetAsync(hashKey, key, value, nx, flags);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "设置哈希表字段的值异常");
                return default(bool);
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "设置哈希表字段的值超时");

                return default(bool);
            }
        }
        /// <summary>
        /// 设置多个哈希表字段的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashKey"></param>
        /// <param name="values"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task HashSetAsync<T>(string hashKey, IDictionary<string, T> values, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                await CacheRedisClient.HashSetAsync<T>(hashKey, values, flags);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "设置多个哈希表字段的值异常");
                return;
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "设置多个哈希表字段的值超时");
                return;
            }
        }



        /// <summary>
        /// 设置哈希表字段的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashKey"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="nx">是否存在不覆盖</param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task<bool> HashSetAsync<T>(string hashKey, string key, T value, TimeSpan expiresIn, bool nx = false,
            CommandFlags flags = CommandFlags.None)
        {
            try
            {
                var result = await CacheRedisClient.HashSetAsync(hashKey, key, value, nx, flags);
                await CacheRedisClient.KeyExpireAsync(hashKey, expiresIn, flags);
                return result;
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "设置哈希表字段的值异常");
                return default(bool);
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "设置哈希表字段的值超时");

                return default(bool);
            }
        }
        /// <summary>
        /// 设置多个哈希表字段的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashKey"></param>
        /// <param name="values"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task HashSetAsync<T>(string hashKey, IDictionary<string, T> values, TimeSpan expiresIn, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                await CacheRedisClient.HashSetAsync<T>(hashKey, values, flags);
                await CacheRedisClient.KeyExpireAsync(hashKey, expiresIn, flags);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "设置多个哈希表字段的值异常");
                return;
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "设置多个哈希表字段的值超时");
                return;
            }
        }




        /// <summary>
        /// 获取hashkey所有字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashKey"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> HashValuesAsync<T>(string hashKey, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                return await CacheRedisClient.HashValuesAsync<T>(hashKey, flags);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "获取hashkey所有字段异常");
                return default(IEnumerable<T>);
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "获取hashkey所有字段超时");
                return default(IEnumerable<T>);
            }
        }


        //有序集合


        /// <summary>
        /// 向有序集合添加或更新成员的分数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member"></param>
        /// <param name="score"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task<bool> SortedSetAddAsync<T>(string key, T member, double score, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                Dictionary<T, double> pairs = new Dictionary<T, double>
                {
                    { member, score }
                };
                return await CacheRedisClient.SortedSetAddAsync(key, pairs, flags) > 0;
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "向有序集合添加或更新成员的分数异常");
                return default(bool);
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "向有序集合添加或更新成员的分数超时");

                return default(bool);
            }
        }

        /// <summary>
        /// 向有序集合添加或更新一个或多个成员的分数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pairs"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task<long> SortedSetAddAsync<T>(string key, Dictionary<T, double> pairs, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                return await CacheRedisClient.SortedSetAddAsync(key, pairs, flags);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "向有序集合添加或更新成员的分数异常");
                return default(long);
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "向有序集合添加或更新成员的分数超时");

                return default(long);
            }
        }

        /// <summary>
        /// 有序集合中对指定成员的分数加上增量 increment
        /// </summary>
        public async Task<double> SortedSetIncrementAsync<T>(string key, T member, double value,
             CommandFlags flags = CommandFlags.None)
        {
            try
            {
                return await CacheRedisClient.SortedSetIncrementAsync(key, member, value, flags);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "向有序集合对指定成员的分数加上增量异常");
                return default(double);
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "向有序集合对指定成员的分数加上增量超时");

                return default(double);
            }
        }

        /// <summary>
        /// 通过索引区间返回有序集合成指定区间内的成员
        /// </summary>
        public async Task<long> SortedSetLengthAsync(string key, double min, double max,
            Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                return await CacheRedisClient.SortedSetLengthAsync(key, min, max, exclude, flags);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "区间返回有序集合成指定区间内的成员数异常");
                return default(long);
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "区间返回有序集合成指定区间内的成员数超时");

                return default(long);
            }
        }

        /// <summary>
        /// 计算在有序集合中指定区间分数的成员数
        /// </summary>
        public async Task<long> SortedSetLengthByValueAsync(string key, double min, double max,
            Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                return await CacheRedisClient.SortedSetLengthByValueAsync(key, min, max, exclude, flags);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "区间返回有序集合成指定区间分数的成员数异常");
                return default(long);
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "区间返回有序集合成指定区间分数的成员数超时");

                return default(long);
            }
        }

        /// <summary>
        /// 返回有序集中指定区间内的成员
        /// </summary>
        public async Task<IEnumerable<T>> SortedSetRangeByRankAsync<T>(string key, long start = 0, long stop = 1,
            Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                return await CacheRedisClient.SortedSetRangeByRankAsync<T>(key, start, stop, order, flags);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "返回有序集中指定区间内的成员异常");
                return default(IEnumerable<T>);
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "返回有序集中指定区间内的成员超时");
                return default(IEnumerable<T>);
            }
        }

        /// <summary>
        /// 通过分数返回有序集合指定区间内的成员
        /// </summary>
        public async Task<Dictionary<T, double>> SortedSetRangeByRankWithScoresAsync<T>(string key, long start = 0, long stop = 1,
            Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                return await CacheRedisClient.SortedSetRangeByRankWithScoresAsync<T>(key, start, stop, order, flags);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "通过分数返回有序集中指定区间内的成员异常");
                return default(Dictionary<T, double>);
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "通过分数返回有序集中指定区间内的成员超时");
                return default(Dictionary<T, double>);
            }
        }


        /// <summary>
        /// 获取一个有序集合成员的分数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public async Task<double?> SortedSetScoreAsync<T>(string key, T member)
        {
            try
            {
                return await CacheRedisClient.SortedSetScoreAsync<T>(key, member);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "获取有序集合成员的分数异常");
                return default(double?);
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "获取有序集合成员的分数超时");

                return default(double?);
            }
        }

        /// <summary>
        /// 计算给定的一个或多个有序集的并集
        /// </summary>
        public async Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, string destination,
            string[] keys, double[] weights, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                List<RedisKey> kk = new List<RedisKey>();
                foreach(var k in keys)
                {
                    kk.Add(k);
                }
                return await CacheRedisClient.SortedSetCombineAndStoreAsync(operation, destination, kk.ToArray(), weights, aggregate, flags);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "计算给定的一个或多个有序集的并集异常");
                return default(long);
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "计算给定的一个或多个有序集的并集超时");

                return default(long);
            }
        }

        /// <summary>
        /// 计算给定的两个有序集的并集
        /// </summary>
        public async Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, RedisKey destination,
            string firstkey, string secondkey, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                double[] weights = { 1, 1 };
                List<RedisKey> kk = new List<RedisKey>
                {
                    firstkey,
                    secondkey
                };
                return await CacheRedisClient.SortedSetCombineAndStoreAsync(operation, destination, kk.ToArray(), weights, aggregate, flags);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "计算给定的两个有序集的并集异常");
                return default(long);
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "计算给定的两个有序集的并集超时");

                return default(long);
            }
        }

        /// <summary>
        /// 计算给定的两个有序集的差集
        /// </summary>
        public async Task<bool> SortedSetDiffAndStoreAsync(SetOperation operation, RedisKey destination,
            string firstkey, string secondkey, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                double[] weights = { 1, 0 };
                List<RedisKey> kk = new List<RedisKey>
                {
                    firstkey,
                    secondkey
                };
                var result = await CacheRedisClient.SortedSetCombineAndStoreAsync(operation, destination, kk.ToArray(), weights, aggregate, flags);
                await CacheRedisClient.SortedSetRemoveRangeByScoreAsync(destination, 0, 0,flags:CommandFlags.FireAndForget);
                if (result>0)
                    return true;
                else
                    return false;
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "计算给定的两个有序集的差集异常");
                return default(bool);
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "计算给定的两个有序集的差集超时");

                return default(bool);
            }
        }

        /// <summary>
        /// 移除有序集合中给定的分数区间的所有成员
        /// </summary>
        public async Task<long> SortedSetRemoveRangeByScoreAsync(string key, double start, double stop,
            Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            try
            {
                return await CacheRedisClient.SortedSetRemoveRangeByScoreAsync(key, start, stop, exclude,flags);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "移除有序集合中给定的分数区间的所有成员异常");
                return default(long);
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "移除有序集合中给定的分数区间的所有成员超时");

                return default(long);
            }
        }

        /// <summary>
        /// 移除有序集合中给定的排名区间的所有成员
        /// </summary>
        public async Task<long> SortedSetRemoveRangeByRankAsync(string key, long start, long stop,
            CommandFlags flags = CommandFlags.None)
        {
            try
            {
                return await CacheRedisClient.SortedSetRemoveRangeByRankAsync(key, start, stop, flags);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "移除有序集合中给定的排名区间的所有成员异常");
                return default(long);
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "移除有序集合中给定的排名区间的所有成员超时");

                return default(long);
            }
        }

        /// <summary>
        /// 移除有序集合中给定的排名区间的所有成员
        /// </summary>
        public async Task<long> SortedSetRemoveAsync<T>(string key, IEnumerable<T> numbers,
             CommandFlags flags = CommandFlags.None)
        {
            try
            {
                return await CacheRedisClient.SortedSetRemoveAsync(key, numbers, flags);
            }
            catch (RedisException ex)
            {
                _logger.LogError(ex, "移除有序集合中给定的排名区间的所有成员异常");
                return default(long);
            }
            catch (RedisTimeoutException ex)
            {
                _logger.LogError(ex, "移除有序集合中给定的排名区间的所有成员超时");

                return default(long);
            }
        }

        public async Task<string> GetStringAsync(string key)
        {
            return await CacheRedisClient.GetStringAsync(key);
        }

        public async Task<List<T>> ListPopAll<T>(string key) where T : class
        {
            return await CacheRedisClient.ListPopAll<T>(key);
        }

        public async Task<bool> ListPush(string key, object value, TimeSpan? expiry = null)
        {
            return await CacheRedisClient.ListPush(key, value,expiry);
        }


        public async Task<bool> LockTakeAsync(string key,string value,TimeSpan expiry)
        {
           bool flag = await CacheRedisClient.Database.LockTakeAsync(key, value, expiry);
            return flag;
        }

        public async Task<bool> LockReleaseAsync(string key, string value)
        {

            bool flag = await CacheRedisClient.Database.LockReleaseAsync(key, value);
            return flag;
        }


    }
}
