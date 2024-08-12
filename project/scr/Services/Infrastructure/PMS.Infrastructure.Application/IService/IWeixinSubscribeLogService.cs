using PMS.Infrastructure.Domain.Dtos;
using PMS.Infrastructure.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeChat.Model;

namespace PMS.Infrastructure.Application.IService
{
    public interface IWeixinSubscribeLogService
    {
        Task<bool> AddAsync(WeixinSubscribeLog weixinSubscribeLog);
        Task<IEnumerable<WeixinSubscribePvDto>> GetSubscribePvAsync(string[] types, IEnumerable<WeChatEventEnum> events, bool? isFirstSubscribe, DateTime startTime, DateTime endTime);
    }
}