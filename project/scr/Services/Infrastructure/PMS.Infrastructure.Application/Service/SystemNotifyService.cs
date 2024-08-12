using PMS.Infrastructure.Application.IService;
using PMS.Infrastructure.Domain.Entities;
using PMS.Infrastructure.Domain.IRepositories;
using ProductManagement.Framework.Cache.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Infrastructure.Application.Service
{
    public class SystemNotifyService : ISystemNotifyService
    {
        ISystemNotifyRepository _repository;
        IEasyRedisClient _easyRedisClient;
        //const string unReadMsgCachePrefix = "SystemNotify:";
        public SystemNotifyService(ISystemNotifyRepository repository, IEasyRedisClient easyRedisClient)
        {
            _repository = repository;
            _easyRedisClient = easyRedisClient;
        }

        public async Task<bool> AddAsync(SystemNotify systemNotify)
        {
            return await _repository.AddAsync(systemNotify);
        }

        public async Task<bool> ConfirmAsync(Guid notifyId)
        {
            SystemNotify systemNotify = new SystemNotify() {
                Id = notifyId,
                ReadTime = DateTime.Now
            };
           return await _repository.UpdateAsync(systemNotify, null, new[] { "ReadTime" });
        }

        public async Task<IEnumerable<SystemNotify>> GetAsync(List<int> infoTypes, Guid toUserId)
        {
            if (infoTypes.Any(t => t.Equals(0)) == false)
            {
                infoTypes.Add(0);   //定义0是通用场景。
            }
            string where = @"ReadTime IS NULL
AND InfoType IN @infoTypes
AND ToUser=@toUserId";
            return await _repository.GetByAsync(where, new { infoTypes, toUserId }, "CreateTime DESC");
        }
    }
}
