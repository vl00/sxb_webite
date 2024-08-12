using PMS.School.Application.IServices;
using PMS.School.Domain.Entities.WechatDemo;
using PMS.School.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.School.Application.Services
{
    public class OnlineSchoolAchievementService : ApplicationService<OnlineSchoolAchievementInfo>, IOnlineSchoolAchievementService
    {
        IOnlineSchoolAchievementRepository _onlineSchoolAchievementRepository;
        public OnlineSchoolAchievementService(IOnlineSchoolAchievementRepository onlineSchoolAchievementRepository) : base(onlineSchoolAchievementRepository)
        {
            _onlineSchoolAchievementRepository = onlineSchoolAchievementRepository;
        }

        public async Task<IEnumerable<OnlineSchoolAchievementInfo>> GetByEID(Guid eid, int year = 0)
        {
            if (eid == Guid.Empty) return null;
            var str_Where = "EID = @eid";
            if (year < 1)
            {
                var find = await _onlineSchoolAchievementRepository.GetRecentYear(eid);
                if (find > 0) year = find;
            }
            if (year > 0)
            {
                str_Where += " AND Year = @year";
            }
            return await _onlineSchoolAchievementRepository.GetByAsync(str_Where, new { eid, year }, "Year Desc");
        }

        public async Task<IEnumerable<int>> GetYears(Guid eid)
        {
            if (eid == Guid.Empty) return null;
            var finds = await _onlineSchoolAchievementRepository.GetByAsync("[EID] = @eid", new { eid }, fileds: new string[] { "Year" });
            if (finds?.Any() == true)
            {
                return finds.Select(p => p.Year).Distinct().OrderByDescending(p => p);
            }
            return null;
        }

        public async Task<bool> RemoveByEIDYear(Guid eid, int year)
        {
            if (eid == Guid.Empty || year <= 1980) return false;
            return await _onlineSchoolAchievementRepository.DeleteByEIDYear(eid, year);
        }
    }
}
