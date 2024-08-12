using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace ProductManagement.Framework.Cache.Redis
{
    public interface IEasyRedisClient
    {
        /// <summary>
        /// 返回一个CacheRedisClient
        /// </summary>
        ICacheRedisClient CacheRedisClient { get; }

        /// <summary>
        /// 异步检查键值是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> ExistsAsync(string key);

        /// <summary>
        /// 根据前缀批量删除键值
        /// </summary>
        /// <param name="key"></param>
        void FuzzyRemove(string key, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 异步移除一个键值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> RemoveAsync(string key, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 异步移除一组键值
        /// </summary>
        /// <param name="keys"></param>
        Task RemoveAllAsync(IEnumerable<string> keys, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 获取一个值，如果值不存在即设置一个缓存并返回该值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        Task<T> GetOrAddAsync<T>(string key, Func<T> func);

        /// <summary>
        /// 获取一个值，如果值不存在即设置一个缓存并返回该值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> func);

        /// <summary>
        /// 根据一个键异步获取一个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> GetAsync<T>(string key);

        Task<string> GetStringAsync(string key);
        /// <summary>
        /// 异步添加一对键值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> AddAsync<T>(string key, T value);

        /// <summary>
        /// 异步根据一个键替换一个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> ReplaceAsync<T>(string key, T value);

        /// <summary>
        /// 获取一个值，如果值不存在即设置一个缓存并返回该值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <param name="expiresAt">失效期</param>
        /// <returns></returns>
        Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> func, DateTimeOffset expiresAt);

        /// <summary>
        /// 获取一个值，如果值不存在即设置一个缓存并返回该值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <param name="expiresAt">失效期</param>
        /// <returns></returns>
        Task<T> GetOrAddAsync<T>(string key, Func<T> func, DateTimeOffset expiresAt);


        /// <summary>
        /// 异步添加一对键值，并设置失效期
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresAt">失效期</param>
        /// <returns></returns>
        Task<bool> AddAsync<T>(string key, T value, DateTimeOffset expiresAt, CommandFlags flags = CommandFlags.None);


        /// <summary>
        /// 异步替换一个键值并设置失效期
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresAt"></param>
        /// <returns></returns>
        Task<bool> ReplaceAsync<T>(string key, T value, DateTimeOffset expiresAt);

        /// <summary>
        /// 添加一对键值，并设置有效期
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresIn"></param>
        /// <returns></returns>
        Task<bool> AddAsync<T>(string key, T value, TimeSpan expiresIn, CommandFlags flags = CommandFlags.None);

        Task<bool> AddStringAsync(string key, string value, TimeSpan expiresIn);




        /// <summary>
        /// 获取一个值，如果值不存在即设置一个缓存并返回该值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <param name="expiresIn"></param>
        /// <returns></returns>
        Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> func, TimeSpan expiresIn);

        /// <summary>
        /// 获取一个值，如果值不存在即设置一个缓存并返回该值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <param name="expiresIn"></param>
        /// <returns></returns>
        Task<T> GetOrAddAsync<T>(string key, Func<T> func, TimeSpan expiresIn);

        /// <summary>
        /// 替换一个键值，并设置有效期
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresIn"></param>
        /// <returns></returns>
        Task<bool> ReplaceAsync<T>(string key, T value, TimeSpan expiresIn);

        /// <summary>
        /// 获取一组键的值的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <returns></returns>
        Task<IDictionary<string, T>> GetAllAsync<T>(IEnumerable<string> keys);




        //HashSet

        /// <summary>
        /// 获取哈希表中字段的数量
        /// </summary>
        /// <param name="hashKey"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        Task<long> HashLengthAsync(string hashKey, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 查看哈希表 key 中，指定的字段是否存在
        /// </summary>
        /// <param name="hashKey"></param>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        Task<bool> HashExistsAsync(string hashKey, string key, CommandFlags flags = CommandFlags.None);
        /// <summary>
        /// 获取存储在哈希表中指定字段的值。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashKey"></param>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        Task<T> HashGetAsync<T>(string hashKey, string key, CommandFlags flags = CommandFlags.None);
        /// <summary>
        /// 获取在哈希表中指定 key 的所有字段和值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashKey"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        Task<Dictionary<string, T>> HashGetAllAsync<T>(string hashKey, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 删除一个字段
        /// </summary>
        /// <param name="hashKey"></param>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        Task<bool> HashDeleteAsync(string hashKey, string key, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 删除一个或多个哈希表字段
        /// </summary>
        /// <param name="hashKey"></param>
        /// <param name="keys"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        Task<long> HashDeleteAsync(string hashKey, IEnumerable<string> keys, CommandFlags flags = CommandFlags.None);


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
        Task<bool> HashSetAsync<T>(string hashKey, string key, T value, bool nx = false,
            CommandFlags flags = CommandFlags.None);
        /// <summary>
        /// 设置多个哈希表字段的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashKey"></param>
        /// <param name="values"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        Task HashSetAsync<T>(string hashKey, IDictionary<string, T> values, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 获取hashkey所有字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashKey"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> HashValuesAsync<T>(string hashKey, CommandFlags flags = CommandFlags.None);


        /// <summary>
        /// 设置哈希表字段的值,设置过期时间
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashKey"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="nx">是否存在不覆盖</param>
        /// <param name="flags"></param>
        /// <returns></returns>
        Task<bool> HashSetAsync<T>(string hashKey, string key, T value, TimeSpan expiresIn, bool nx = false,
            CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 设置多个哈希表字段的值,设置过期时间
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashKey"></param>
        /// <param name="values"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        Task HashSetAsync<T>(string hashKey, IDictionary<string, T> values, TimeSpan expiresIn, CommandFlags flags = CommandFlags.None);

        //有序集合
        /// <summary>
        /// 向有序集合添加或更新成员的分数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member"></param>
        /// <param name="score"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        Task<bool> SortedSetAddAsync<T>(string key, T member, double score, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 向有序集合添加或更新一个或多个成员的分数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pairs"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        Task<long> SortedSetAddAsync<T>(string key, Dictionary<T, double> pairs, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 有序集合中对指定成员的分数加上增量 increment
        /// </summary>
        Task<double> SortedSetIncrementAsync<T>(string key, T member, double value,
              CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 通过索引区间返回有序集合成指定区间内的成员
        /// </summary>
        Task<long> SortedSetLengthAsync(string key, double min, double max,
            Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 计算在有序集合中指定区间分数的成员数
        /// </summary>
        Task<long> SortedSetLengthByValueAsync(string key, double min, double max,
            Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 返回有序集中指定区间内的成员
        /// </summary>
        Task<IEnumerable<T>> SortedSetRangeByRankAsync<T>(string key, long start = 0, long stop = 1,
            Order order = Order.Ascending, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 通过分数返回有序集合指定区间内的成员
        /// </summary>
        Task<Dictionary<T, double>> SortedSetRangeByRankWithScoresAsync<T>(string key, long start = 0, long stop = 1,
            Order order = Order.Ascending, CommandFlags flags = CommandFlags.None);


        /// <summary>
        /// 获取一个有序集合成员的分数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        Task<double?> SortedSetScoreAsync<T>(string key, T member);

        /// <summary>
        /// 计算给定的一个或多个有序集的并集
        /// </summary>
        Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, string destination,
            string[] keys, double[] weights, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 计算给定的两个有序集的并集
        /// </summary>
        Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, RedisKey destination,
            string firstkey, string secondkey, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 计算给定的两个有序集的差集
        /// </summary>
        Task<bool> SortedSetDiffAndStoreAsync(SetOperation operation, RedisKey destination,
            string firstkey, string secondkey, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 移除有序集合中给定的分数区间的所有成员
        /// </summary>
        Task<long> SortedSetRemoveRangeByScoreAsync(string key, double start, double stop,
            Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 移除有序集合中给定的排名区间的所有成员
        /// </summary>
        Task<long> SortedSetRemoveRangeByRankAsync(string key, long start, long stop,
            CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// 移除有序集合中给定的排名区间的所有成员
        /// </summary>
        Task<long> SortedSetRemoveAsync<T>(string key, IEnumerable<T> numbers,
             CommandFlags flags = CommandFlags.None);

        Task<List<T>> ListPopAll<T>(string key) where T : class;
        Task<bool> ListPush(string key, object value, TimeSpan? expiry = null);

        Task<bool> LockTakeAsync(string key, string value, TimeSpan expiry);

        Task<bool> LockReleaseAsync(string key, string value);
        Task<IEnumerable<T>> GetOrAddEnumerableAsync<T>(string key, Func<IEnumerable<T>> func, TimeSpan expiresIn);
    }
}
