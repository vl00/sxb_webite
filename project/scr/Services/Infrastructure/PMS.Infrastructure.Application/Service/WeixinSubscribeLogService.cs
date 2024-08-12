using PMS.Infrastructure.Domain.Dtos;
using PMS.Infrastructure.Domain.Entities;
using PMS.Infrastructure.Domain.IRepositories;
using ProductManagement.Framework.Cache.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeChat.Model;

namespace PMS.Infrastructure.Application.IService
{
    public class WeixinSubscribeLogService : IWeixinSubscribeLogService
    {
        IWeixinSubscribeLogRepository _subscribeLogRepositoryweixin;

        public WeixinSubscribeLogService(IWeixinSubscribeLogRepository subscribeLogRepositoryweixin)
        {
            _subscribeLogRepositoryweixin = subscribeLogRepositoryweixin;
        }

        public async Task<bool> AddAsync(WeixinSubscribeLog weixinSubscribeLog)
        {
            return await _subscribeLogRepositoryweixin.AddAsync(weixinSubscribeLog);
        }

        public async Task<IEnumerable<WeixinSubscribePvDto>> GetSubscribePvAsync(string[] types, IEnumerable<WeChatEventEnum> events, bool? isFirstSubscribe, DateTime startTime, DateTime endTime)
        {
            //var weChatEvents = new string[] { WeChatEventEnum.subscribe.ToString(), WeChatEventEnum.SCAN.ToString() };
            var weChatEvents = events?.Select(s => s.ToString());
            return await _subscribeLogRepositoryweixin.GetSubscribePvAsync(types, weChatEvents.ToArray(), isFirstSubscribe, startTime, endTime);
        }
    }
}
