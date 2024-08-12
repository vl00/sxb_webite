using PMS.Infrastructure.Domain.IRepositories;
using ProductManagement.Framework.Cache.Redis;
using System;
using System.Threading.Tasks;

namespace PMS.Infrastructure.Application.IService
{
    public class KeyValueService : IKeyValueService
    {
        IKeyValyeRepository _keyValyeRepository;
        IEasyRedisClient _easyRedisClient;
        public KeyValueService(IKeyValyeRepository keyValyeRepository, IEasyRedisClient easyRedisClient)
        {
            _keyValyeRepository = keyValyeRepository;
            _easyRedisClient = easyRedisClient;
        }

        public async Task<string> GetValue(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }
            return await _keyValyeRepository.GetValue(key);
        }

        public async Task<string> GetValueFromCache(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }
            string cacheKey = $"dbo_kv_{key}";
            return await _easyRedisClient.GetOrAddAsync(cacheKey, async () =>
             {
                 return await _keyValyeRepository.GetValue(key);
             },TimeSpan.FromHours(6));

        }

    }
}
