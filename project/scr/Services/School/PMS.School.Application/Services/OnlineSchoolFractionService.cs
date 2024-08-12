using PMS.School.Application.IServices;
using PMS.School.Domain.Entities.WechatDemo;
using PMS.School.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.School.Application.Services
{
    public class OnlineSchoolFractionService : ApplicationService<OnlineSchoolFractionInfo>, IOnlineSchoolFractionService
    {
        IOnlineSchoolFractionRepository _onlineSchoolFractionRepository;
        public OnlineSchoolFractionService(IOnlineSchoolFractionRepository onlineSchoolFractionRepository) : base(onlineSchoolFractionRepository)
        {
            _onlineSchoolFractionRepository = onlineSchoolFractionRepository;
        }

        public async Task<IEnumerable<SchoolFractionInfo2>> Get2ByEID(Guid eid, int year = 0, int type = 0)
        {
            if (eid == Guid.Empty) return null;
            var finds = await _onlineSchoolFractionRepository.Get2ByEID(eid, year, type);
            if (finds?.Any() == true) return finds;
            return null;
        }

        public async Task<IEnumerable<OnlineSchoolFractionInfo>> GetByEID(Guid eid, int year = 0)
        {
            if (eid == Guid.Empty) return null;
            var str_Where = "EID = @eid";
            if (year < 1)
            {
                var find = await _onlineSchoolFractionRepository.GetRecentYear(eid);
                if (find > 0) year = find;
            }
            if (year > 0)
            {
                str_Where += " AND Year = @year";
            }
            var finds = await _onlineSchoolFractionRepository.GetByAsync(str_Where, new { eid, year }, "Year Desc");
            if (finds?.Any() == true) return finds;
            return null;
        }

        public async Task<IEnumerable<int>> GetYears(Guid eid)
        {
            if (eid == Guid.Empty) return null;
            var finds = await _onlineSchoolFractionRepository.GetByAsync("[EID] = @eid", new { eid }, fileds: new string[] { "Year" });
            if (finds?.Any() == true) return finds.Select(p => p.Year).Distinct().OrderByDescending(p => p);
            return null;
        }
        public async Task<IEnumerable<KeyValuePair<int, IEnumerable<int>>>> Get2Years(Guid eid)
        {
            if (eid == Guid.Empty) return null;
            var finds = await _onlineSchoolFractionRepository.Get2Years(eid);
            if (finds?.Any() == true) return finds.Select(p => p.Item1).Distinct()
                    .OrderBy(p => p).Select(p => new KeyValuePair<int, IEnumerable<int>>(p, finds.Where(x => x.Item1 == p).Select(x => x.Item2).OrderByDescending(x => x)));
            return null;
        }

        public async Task<bool> RemoveByEID(Guid eid)
        {
            if (eid == Guid.Empty) return false;
            return await _onlineSchoolFractionRepository.DeleteByEID(eid);
        }

        public async Task<bool> Add2Async(SchoolFractionInfo2 entity)
        {
            if (entity.ID == Guid.Empty || entity.EID == Guid.Empty) return false;
            return await _onlineSchoolFractionRepository.Insert2(entity);
        }

        public async Task<bool> Remove2AsyncByEIDYear(Guid eid, int year)
        {
            if (eid == Guid.Empty || year <= 1980) return false;
            return await _onlineSchoolFractionRepository.Delete2AsyncByEIDYear(eid, year);
        }

        public async Task<bool> RemoveByEIDYear(Guid eid, int year)
        {
            if (eid == default || year < 1900) return false;
            return await _onlineSchoolFractionRepository.DeleteByEIDYear(eid, year);
        }

        public async Task<IEnumerable<OnlineSchoolFractionInfo>> GetAll()
        {
            return _onlineSchoolFractionRepository.GetBy("1=1");
        }
    }
}
