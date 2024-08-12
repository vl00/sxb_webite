using PMS.School.Application.IServices;
using PMS.School.Domain.Entities.WechatDemo;
using PMS.School.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.School.Application.Services
{
    public class OnlineSchoolQuotaService : ApplicationService<OnlineSchoolQuotaInfo>, IOnlineSchoolQuotaService
    {
        IOnlineSchoolQuotaRepository _onlineSchoolQuotaRepository;
        public OnlineSchoolQuotaService(IOnlineSchoolQuotaRepository onlineSchoolQuotaRepository) : base(onlineSchoolQuotaRepository)
        {
            _onlineSchoolQuotaRepository = onlineSchoolQuotaRepository;
        }

        public async Task<IEnumerable<OnlineSchoolQuotaInfo>> GetByEID(Guid eid, int year = 0, int type = 0)
        {
            if (eid == Guid.Empty) return null;

            var str_Where = "EID = @eid";
            if (year < 1)
            {
                var find = await _onlineSchoolQuotaRepository.GetRecentYear(eid);
                if (find > 0) year = find;
            }
            if (year > 0)
            {
                str_Where += " AND Year = @year";
            }
            if (type > 0) str_Where += " AND [Type] = @type";
            return await _onlineSchoolQuotaRepository.GetByAsync(str_Where, new { eid, year, type }, "Year Desc");
        }

        public async Task<IEnumerable<KeyValuePair<int, IEnumerable<int>>>> GetYears(Guid eid)
        {
            if (eid == Guid.Empty) return null;
            var finds = await _onlineSchoolQuotaRepository.GetByAsync("eid = @eid", new { eid }, fileds: new string[] { "[Type]", "[Year]" });
            if (finds?.Any() == true)
            {
                return finds.Select(p => p.Type).Distinct().OrderBy(p => p).
                    Select(p => new KeyValuePair<int, IEnumerable<int>>(p, finds.Where(x => x.Type == p).Select(x => x.Year).Distinct().OrderByDescending(x => x)));
            }
            return null;
        }

        public async Task<bool> RemoveByEID(Guid eid, int? type = null)
        {
            if (eid == Guid.Empty) return false;
            return await _onlineSchoolQuotaRepository.DeleteByEID(eid, type);
        }
    }
}
