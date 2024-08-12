using PMS.School.Domain.Entities.WechatDemo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.School.Application.IServices
{
    public interface IOnlineSchoolAchievementService : IApplicationService<OnlineSchoolAchievementInfo>
    {
        Task<IEnumerable<OnlineSchoolAchievementInfo>> GetByEID(Guid eid, int year = 0);
        Task<IEnumerable<int>> GetYears(Guid eid);
        Task<bool> RemoveByEIDYear(Guid eid, int year);
    }
}
