using PMS.Infrastructure.Domain.Dtos;
using PMS.Infrastructure.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.Infrastructure.Domain.IRepositories
{
    public interface IWeixinSubscribeLogRepository
    {
        Task<bool> AddAsync(WeixinSubscribeLog weixinSubscribeLog);
        Task<IEnumerable<WeixinSubscribePvDto>> GetSubscribePvAsync(string[] types, string[] weChatEvents, bool? isFirstSubscribe, DateTime startTime, DateTime endTime);
    }
}
