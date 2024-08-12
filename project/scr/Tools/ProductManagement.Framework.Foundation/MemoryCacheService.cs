using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProductManagement.Infrastructure.Toolibrary
{
    public class MemoryCacheService
    {
        private static MemoryCache cache = new MemoryCache(new MemoryCacheOptions());
        public static object GetCacheValue(string key)
        {
            object obj = null;
            bool flag = key != null && MemoryCacheService.cache.TryGetValue(key, out obj);
            object result;
            if (flag)
            {
                result = obj;
            }
            else
            {
                result = null;
            }
            return result;
        }
        public static T Get<T>(string key) where T : class
        {
            bool flag = key == null;
            if (flag)
            {
                throw new ArgumentNullException("key");
            }
            return CacheExtensions.Get(MemoryCacheService.cache, key) as T;
        }
        public static List<T> GetList<T>(string key) where T : class
        {
            bool flag = key == null;
            if (flag)
            {
                throw new ArgumentNullException("key");
            }
            return CacheExtensions.Get(MemoryCacheService.cache, key) as List<T>;
        }
        public static IDictionary<string, object> GetAll(List<string> keys)
        {
            bool flag = keys == null;
            if (flag)
            {
                throw new ArgumentNullException("keys");
            }
            Dictionary<string, object> dict = new Dictionary<string, object>();
            Enumerable.ToList<string>(keys).ForEach(delegate (string item)
            {
                dict.Add(item, CacheExtensions.Get(MemoryCacheService.cache, item));
            });
            return dict;
        }
        public static void SetChacheValue(string key, object value)
        {
            bool flag = key != null;
            if (flag)
            {
                IMemoryCache arg_30_0 = MemoryCacheService.cache;
                MemoryCacheEntryOptions expr_16 = new MemoryCacheEntryOptions();
                //expr_16.set_SlidingExpiration(new TimeSpan?(TimeSpan.FromHours(1.0)));
                CacheExtensions.Set<object>(arg_30_0, key, value, expr_16);
            }
        }
        public static void Add(string key, object value, TimeSpan expiresSliding, TimeSpan expiressAbsoulte)
        {
            bool flag = key == null;
            if (flag)
            {
                throw new ArgumentNullException("key");
            }
            bool flag2 = value == null;
            if (flag2)
            {
                throw new ArgumentNullException("value");
            }
            CacheExtensions.Set<object>(MemoryCacheService.cache, key, value, MemoryCacheEntryExtensions.SetAbsoluteExpiration(MemoryCacheEntryExtensions.SetSlidingExpiration(new MemoryCacheEntryOptions(), expiresSliding), expiressAbsoulte));
        }
        public static void Remove(string key)
        {
            bool flag = key == null;
            if (flag)
            {
                throw new ArgumentNullException("key");
            }
            MemoryCacheService.cache.Remove(key);
        }
    }
}
