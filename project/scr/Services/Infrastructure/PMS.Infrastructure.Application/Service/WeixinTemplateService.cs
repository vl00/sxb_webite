using PMS.Infrastructure.Domain.IRepositories;
using ProductManagement.Framework.Cache.Redis;
using System;
using System.Threading.Tasks;

namespace PMS.Infrastructure.Application.IService
{
    public class WeixinTemplateService : IWeixinTemplateService
    {
        IWeixinTemplateRepository _weixinTemplateRepository;
        IEasyRedisClient _easyRedisClient;

        public WeixinTemplateService(IWeixinTemplateRepository weixinTemplateRepository, IEasyRedisClient easyRedisClient)
        {
            _weixinTemplateRepository = weixinTemplateRepository;
            _easyRedisClient = easyRedisClient;
        }

        public async Task<string> GetTemplateText(Guid id)
        {
            return await _weixinTemplateRepository.GetTemplateText(id);
        }

        public async Task<string> GetTemplateTextFromCache(Guid id)
        {
            string cacheKey = $"ischool:weixin:template:id:{id}";
            return await _easyRedisClient.GetOrAddAsync(cacheKey, async () =>
             {
                 return await _weixinTemplateRepository.GetTemplateText(id);
             },TimeSpan.FromHours(6));

        }

    }
}
