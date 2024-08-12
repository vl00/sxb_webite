using PMS.School.Domain.Entities.WechatDemo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.School.Application.IServices
{
    public interface IOnlineSchoolQuotaService : IApplicationService<OnlineSchoolQuotaInfo>
    {
        Task<IEnumerable<OnlineSchoolQuotaInfo>> GetByEID(Guid eid, int year = 0, int type = 0);
        Task<IEnumerable<KeyValuePair<int, IEnumerable<int>>>> GetYears(Guid eid);
        Task<bool> RemoveByEID(Guid eid, int? type = null);
    }
}
