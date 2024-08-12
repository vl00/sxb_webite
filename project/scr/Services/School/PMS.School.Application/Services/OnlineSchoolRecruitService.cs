using PMS.School.Application.IServices;
using PMS.School.Domain.Entities.WechatDemo;
using PMS.School.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.School.Application.Services
{
    public class OnlineSchoolRecruitService : ApplicationService<OnlineSchoolRecruitInfo>, IOnlineSchoolRecruitService
    {
        IOnlineSchoolRecruitRepository _onlineSchoolRecruitRepository;
        public OnlineSchoolRecruitService(IOnlineSchoolRecruitRepository onlineSchoolRecruitRepository) : base(onlineSchoolRecruitRepository)
        {
            _onlineSchoolRecruitRepository = onlineSchoolRecruitRepository;
        }

        public async Task<bool> DeleteIfExisted(Guid eid)
        {
            if (eid == Guid.Empty) return false;
            return await _onlineSchoolRecruitRepository.DeleteIFExist(eid);
        }

        public async Task<IEnumerable<OnlineSchoolRecruitInfo>> GetByEID(Guid eid, int year = 0, int type = 0)
        {
            if (eid == Guid.Empty) return null;
            var str_Where = "EID = @eid";
            if (year < 1)
            {
                var find = await _onlineSchoolRecruitRepository.GetRecentYear(eid);
                if (find > 0) year = find;
            }
            if (year > 0)
            {
                str_Where += " AND Year = @year";
            }

            if (type > -1) str_Where += " AND [Type] = @type";
            return await _onlineSchoolRecruitRepository.GetByAsync(str_Where, new { eid, year, type }, "Year Desc");
        }

        public async Task<IEnumerable<OnlineSchoolRecruitInfo>> GetCostByYear(Guid eid, int year)
        {
            if (eid == Guid.Empty || year < 1900) return null;
            var finds = await _onlineSchoolRecruitRepository.GetByAsync("[EID] = @eid AND [Year] = @year AND ([Tuition] is not null or [ApplyCost] is not null or [OtherCost] is not null)",
                new { eid, year }, fileds: new string[] { "[Year]", "[ID]", "[TuiTion]", "[ApplyCost]", "[OtherCost]" });
            if (finds?.Any() == true) return finds;
            return null;
        }

        public async Task<IEnumerable<int>> GetCostYears(Guid eid)
        {
            if (eid == Guid.Empty) return null;
            var finds = await _onlineSchoolRecruitRepository.GetByAsync("[EID] = @eid AND ([Tuition] is not null or [ApplyCost] is not null or [OtherCost] is not null)", new { eid }, fileds: new string[] { "[Year]", "[ID]" });
            if (finds?.Any() == true) return finds.Select(p => p.Year).Distinct().OrderByDescending(p => p);
            return null;
        }

        public async Task<IEnumerable<RecruitScheduleInfo>> GetRecruitSchedules(IEnumerable<Guid> recruitIDs)
        {
            if (recruitIDs?.Any() == true)
            {
                return await _onlineSchoolRecruitRepository.GetRecruitScheduleByRecruitIDs(recruitIDs);
            }
            return null;
        }

        public async Task<IEnumerable<RecruitScheduleInfo>> GetRecruitSchedules(int cityCode, IEnumerable<int> recruitTypes, string schFType, int? areaCode = null)
        {
            if (cityCode < 1 || recruitTypes == null || !recruitTypes.Any() || string.IsNullOrWhiteSpace(schFType)) return null;
            return await _onlineSchoolRecruitRepository.GetRecruitSchedule(cityCode, recruitTypes, schFType, areaCode);
        }

        public async Task<IEnumerable<KeyValuePair<int, IEnumerable<int>>>> GetYears(Guid eid)
        {
            if (eid == Guid.Empty) return null;
            var finds = await _onlineSchoolRecruitRepository.GetByAsync("eid = @eid", new { eid }, fileds: new string[] { "[Type]", "[Year]" });
            if (finds?.Any() == true)
            {
                return finds.Select(p => p.Type).Distinct().OrderBy(p => p).
                    Select(p => new KeyValuePair<int, IEnumerable<int>>(p, finds.Where(x => x.Type == p).Select(x => x.Year).Distinct().OrderByDescending(x => x)));
            }
            return null;
        }

        public async Task<bool> InsertRecruitSchedule(RecruitScheduleInfo entity)
        {
            if (entity == null || entity.ID == Guid.Empty) return false;
            return await _onlineSchoolRecruitRepository.InsertRecruitSchedule(entity);
        }

        public async Task<bool> RemoveRecruitSchedules(string cityCode, string areaCode, string schFType, int recruitType)
        {
            if (string.IsNullOrWhiteSpace(cityCode) || string.IsNullOrWhiteSpace(schFType) || recruitType < 0) return false;
            return await _onlineSchoolRecruitRepository.DeleteRecruitSchedules(cityCode, areaCode, schFType, recruitType);
        }
    }
}
