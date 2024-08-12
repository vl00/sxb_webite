using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProductManagement.Framework.Cache.Redis.Configuration;
using StackExchange.Redis;

namespace ProductManagement.Framework.Cache.Redis
{
    public interface ICacheRedisClient : IDisposable
    {
        /// <summary>
        /// 配置选项
        /// </summary>
        RedisConfig RedisConfig { get; }

        /// <summary>
        /// redis的连接管理实例
        /// </summary>
        IConnectionMultiplexer Connection { get; }

        /// <summary>
        /// 返回一个序列化的实例
        /// </summary>
        ISerializer Serializer { get; }

        /// <summary>
        /// 返回一个缓存数据库的实例
        /// </summary>
        IDatabase Database { get; }

        /// <summary>
        /// 设置Key的过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiresAt"></param>
        /// <returns></returns>
        Task<bool> KeyExpireAsync(string key, DateTimeOffset expiresAt, CommandFlags flags = CommandFlags.None);
        Task<bool> KeyExpireAsync(string key, TimeSpan expiresIn, CommandFlags flags = CommandFlags.None);

        ///// <summary>
        ///// 检查键值是否存在
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //bool Exists(string key);

        /// <summary>
        /// 异步检查键值是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> ExistsAsync(string key);

        ///// <summary>
        ///// 移除一个键值
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //bool Remove(string key, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 异步移除一个键值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> RemoveAsync(string key, CommandFlags flags = CommandFlags.None);

        ///// <summary>
        ///// 移除一组键值
        ///// </summary>
        ///// <param name="keys"></param>
        //void RemoveAll(IEnumerable<string> keys, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 异步移除一组键值
        /// </summary>
        /// <param name="keys"></param>
        Task RemoveAllAsync(IEnumerable<string> keys, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// key的前缀批量删除
        /// </summary>
        /// <param name="key"></param>
        long FuzzyRemove(string key, CommandFlags flags = CommandFlags.None);

        ///// <summary>
        ///// 根据一个键获取一个值
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //T Get<T>(string key);

        /// <summary>
        /// 根据一个键异步获取一个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> GetAsync<T>(string key);


        Task<string> GetStringAsync(string key);

        ///// <summary>
        ///// 添加一对键值
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="key"></param>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //bool Add<T>(string key, T value);

        /// <summary>
        /// 异步添加一对键值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> AddAsync<T>(string key, T value);

        ///// <summary>
        ///// 根据一个键替换一个值
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="key"></param>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //bool Replace<T>(string key, T value);

        /// <summary>
        /// 异步根据一个键替换一个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> ReplaceAsync<T>(string key, T value);
        Task<bool> AddStringAsync(string key, string value, TimeSpan expiresIn);

        ///// <summary>
        ///// 添加一对键值，并设置失效期
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="key"></param>
        ///// <param name="value"></param>
        ///// <param name="expiresAt">失效期</param>
        ///// <returns></returns>
        //bool Add<T>(string key, T value, DateTimeOffset expiresAt);

        /// <summary>
        /// 异步添加一对键值，并设置失效期
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresAt">失效期</param>
        /// <returns></returns>
        Task<bool> AddAsync<T>(string key, T value, DateTimeOffset expiresAt, CommandFlags flags = CommandFlags.None);

        ///// <summary>
        ///// 替换一个键值并设置失效期
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="key"></param>
        ///// <param name="value"></param>
        ///// <param name="expiresAt"></param>
        ///// <returns></returns>
        //bool Replace<T>(string key, T value, DateTimeOffset expiresAt);

        /// <summary>
        /// 异步替换一个键值并设置失效期
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresAt"></param>
        /// <returns></returns>
        Task<bool> ReplaceAsync<T>(string key, T value, DateTimeOffset expiresAt);

        ///// <summary>
        ///// 添加一对键值，并设置有效期
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="key"></param>
        ///// <param name="value"></param>
        ///// <param name="expiresIn"></param>
        ///// <returns></returns>
        //bool Add<T>(string key, T value, TimeSpan expiresIn);

        /// <summary>
        /// 添加一对键值，并设置有效期
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresIn"></param>
        /// <returns></returns>
        Task<bool> AddAsync<T>(string key, T value, TimeSpan expiresIn, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 替换一对键值，并设置有效期
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresIn"></param>
        /// <returns></returns>
        //bool Replace<T>(string key, T value, TimeSpan expiresIn);

        Task<bool> ReplaceAsync<T>(string key, T value, TimeSpan expiresIn);

        //IDictionary<string, T> GetAll<T>(IEnumerable<string> keys);

        Task<IDictionary<string, T>> GetAllAsync<T>(IEnumerable<string> keys);

        //bool AddAll<T>(IList<Tuple<string, T>> items);

        Task<bool> AddAllAsync<T>(IList<Tuple<string, T>> items);

        //bool SetAdd<T>(string key, T item) where T : class;

        Task<bool> SetAddAsync<T>(string key, T item) where T : class;

        //long SetAddAll<T>(string key, params T[] items) where T : class;

        Task<long> SetAddAllAsync<T>(string key, params T[] items) where T : class;

        //bool SetRemove<T>(string key, T item) where T : class;

        Task<bool> SetRemoveAsync<T>(string key, T item) where T : class;

        //long SetRemoveAll<T>(string key, params T[] items) where T : class;

        Task<long> SetRemoveAllAsync<T>(string key, params T[] items) where T : class;

        //string[] SetMember(string memberName);

        Task<string[]> SetMemberAsync(string memberName);

        //IEnumerable<T> SetMembers<T>(string key);

        Task<IEnumerable<T>> SetMembersAsync<T>(string key);

        //IEnumerable<string> SearchKeys(string pattern);

        Task<IEnumerable<string>> SearchKeysAsync(string pattern);

        //void FlushDb();

        Task FlushDbAsync();

        //void Save(SaveType saveType);

        Task SaveAsync(SaveType saveType);

        //Dictionary<string, string> GetInfo();

        Task<Dictionary<string, string>> GetInfoAsync();

        //long Publish<T>(RedisChannel channel, T message, CommandFlags flags = CommandFlags.None);

        Task<long> PublishAsync<T>(RedisChannel channel, T message, CommandFlags flags = CommandFlags.None);

        //void Subscribe<T>(RedisChannel channel, Action<T> handler, CommandFlags flags = CommandFlags.None);

        Task SubscribeAsync<T>(RedisChannel channel, Func<T, Task> handler, CommandFlags flags = CommandFlags.None);

        //void Unsubscribe<T>(RedisChannel channel, Action<T> handler, CommandFlags flags = CommandFlags.None);

        Task UnsubscribeAsync<T>(RedisChannel channel, Func<T, Task> handler, CommandFlags flags = CommandFlags.None);

        //void UnsubscribeAll(CommandFlags flags = CommandFlags.None);

        Task UnsubscribeAllAsync(CommandFlags flags = CommandFlags.None);

        //long ListLeftPush<T>(string key, T item) where T : class;

        Task<long> ListLeftPushAsync<T>(string key, T item) where T : class;

        //T ListRightPop<T>(string key) where T : class;

        Task<T> ListRightPopAsync<T>(string key) where T : class;

        //bool HashDelete(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None);

        //long HashDelete(string hashKey, IEnumerable<string> keys, CommandFlags commandFlags = CommandFlags.None);

        //bool HashExists(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None);

        //T HashGet<T>(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None);

        //Dictionary<string, T> HashGet<T>(string hashKey, IEnumerable<string> keys, CommandFlags commandFlags = CommandFlags.None);

        //Dictionary<string, T> HashGetAll<T>(string hashKey, CommandFlags commandFlags = CommandFlags.None);

        //long HashIncerementBy(string hashKey, string key, long value, CommandFlags commandFlags = CommandFlags.None);

        //double HashIncerementBy(string hashKey, string key, double value, CommandFlags commandFlags = CommandFlags.None);

        //IEnumerable<string> HashKeys(string hashKey, CommandFlags commandFlags = CommandFlags.None);

        //long HashLength(string hashKey, CommandFlags commandFlags = CommandFlags.None);

        //bool HashSet<T>(string hashKey, string key, T value, bool nx = false, CommandFlags commandFlags = CommandFlags.None);

        //void HashSet<T>(string hashKey, Dictionary<string, T> values, CommandFlags commandFlags = CommandFlags.None);

        //IEnumerable<T> HashValues<T>(string hashKey, CommandFlags commandFlags = CommandFlags.None);

        //Dictionary<string, T> HashScan<T>(string hashKey, string pattern, int pageSize = 10, CommandFlags commandFlags = CommandFlags.None);

        Task<bool> HashDeleteAsync(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None);

        Task<long> HashDeleteAsync(string hashKey, IEnumerable<string> keys, CommandFlags commandFlags = CommandFlags.None);

        Task<bool> HashExistsAsync(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None);

        Task<T> HashGetAsync<T>(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None);

        Task<Dictionary<string, T>> HashGetAsync<T>(string hashKey, IEnumerable<string> keys, CommandFlags commandFlags = CommandFlags.None);

        Task<Dictionary<string, T>> HashGetAllAsync<T>(string hashKey, CommandFlags commandFlags = CommandFlags.None);

        Task<long> HashIncerementByAsync(string hashKey, string key, long value, CommandFlags commandFlags = CommandFlags.None);

        Task<double> HashIncerementByAsync(string hashKey, string key, double value, CommandFlags commandFlags = CommandFlags.None);

        Task<IEnumerable<string>> HashKeysAsync(string hashKey, CommandFlags commandFlags = CommandFlags.None);

        Task<long> HashLengthAsync(string hashKey, CommandFlags commandFlags = CommandFlags.None);

        Task<bool> HashSetAsync<T>(string hashKey, string key, T value, bool nx = false, CommandFlags commandFlags = CommandFlags.None);

        Task HashSetAsync<T>(string hashKey, IDictionary<string, T> values, CommandFlags commandFlags = CommandFlags.None);

        Task<IEnumerable<T>> HashValuesAsync<T>(string hashKey, CommandFlags commandFlags = CommandFlags.None);

        Task<Dictionary<string, T>> HashScanAsync<T>(string hashKey, string pattern, int pageSize = 10, CommandFlags commandFlags = CommandFlags.None);



        /// <summary>
        /// 向有序集合添加一个或多个成员，或者更新已存在成员的分数
        /// </summary>
        Task<long> SortedSetAddAsync<T>(RedisKey key, IDictionary<T, double> pairs, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 有序集合中对指定成员的分数加上增量 increment
        /// </summary>
        Task<double> SortedSetIncrementAsync<T>(RedisKey key, T member, double value,
             CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 通过索引区间返回有序集合成指定区间内的成员
        /// </summary>
        Task<long> SortedSetLengthAsync(RedisKey key, double min, double max,
            Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 计算在有序集合中指定区间分数的成员数
        /// </summary>
        Task<long> SortedSetLengthByValueAsync(RedisKey key, double min, double max,
            Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 返回有序集中指定区间内的成员
        /// </summary>
        Task<IEnumerable<T>> SortedSetRangeByRankAsync<T>(RedisKey key, long start = 0, long stop = 1,
            Order order = Order.Ascending, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 通过分数返回有序集合指定区间内的成员
        /// </summary>
        Task<Dictionary<T, double>> SortedSetRangeByRankWithScoresAsync<T>(RedisKey key, long start = 0, long stop = 1,
            Order order = Order.Ascending, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 返回有序集中，成员的分数值
        /// </summary>
        Task<double?> SortedSetScoreAsync<T>(RedisKey key, T member);

        /// <summary>
        /// 计算给定的一个或多个有序集的并集
        /// </summary>
        Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, RedisKey destination,
            RedisKey[] keys, double[] weights, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 移除有序集合中给定的分数区间的所有成员
        /// </summary>
        Task<long> SortedSetRemoveRangeByScoreAsync(RedisKey key, double start, double stop,
            Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 移除有序集合中给定的排名区间的所有成员
        /// </summary>
        Task<long> SortedSetRemoveRangeByRankAsync(RedisKey key, long start, long stop,
             CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 移除有序集合中的一个或多个成员
        /// </summary>
        Task<long> SortedSetRemoveAsync<T>(RedisKey key, IEnumerable<T> numbers,
             CommandFlags flags = CommandFlags.None);


        Task<List<T>> ListPopAll<T>(string key) where T : class;
        Task<bool> ListPush(string key, object value, TimeSpan? expiry = null);
    }
}
