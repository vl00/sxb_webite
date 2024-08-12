using PMS.Infrastructure.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Infrastructure.Application.IService
{
    public interface ISystemNotifyService
    {

        Task<bool> AddAsync(SystemNotify systemNotify);

        Task<IEnumerable<SystemNotify>> GetAsync(List<int> infoTypes,Guid toUserId);

        Task<bool> ConfirmAsync(Guid notifyId);
    }
}
