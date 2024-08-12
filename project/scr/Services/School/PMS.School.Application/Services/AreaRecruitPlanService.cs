using PMS.School.Application.IServices;
using PMS.School.Domain.Entities.WechatDemo;
using PMS.School.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.School.Application.Services
{
    public class AreaRecruitPlanService : ApplicationService<AreaRecruitPlanInfo>, IAreaRecruitPlanService
    {
        IAreaRecruitPlanRepository _areaRecruitPlanRepository;
        public AreaRecruitPlanService(IAreaRecruitPlanRepository areaRecruitPlanRepository) : base(areaRecruitPlanRepository)
        {
            _areaRecruitPlanRepository = areaRecruitPlanRepository;
        }

        public async Task<IEnumerable<AreaRecruitPlanInfo>> GetByAreaCodeAndSchFType(string areaCode, string schFType, int year = 0)
        {
            if (string.IsNullOrWhiteSpace(areaCode) || string.IsNullOrWhiteSpace(schFType)) return null;
            var str_Where = "[AreaCode] = @areaCode AND [SchFType] = @schFType";
            if (year < 1)
            {
                var find = await _areaRecruitPlanRepository.GetRecentYear(areaCode, schFType);
                if (find > 0) year = find;
            }
            if (year > 0)
            {
                str_Where += " AND Year = @year";
            }
            return await _areaRecruitPlanRepository.GetByAsync(str_Where, new { areaCode, schFType, year }, "Year Desc");
        }

        public async Task<IEnumerable<int>> GetYears(string areaCode, string schFType)
        {
            if (string.IsNullOrWhiteSpace(areaCode) || string.IsNullOrWhiteSpace(schFType)) return null;
            var finds = await _areaRecruitPlanRepository.GetByAsync("[AreaCode] = @areaCode AND [SchFType] = @schFType",
                new { areaCode, schFType }, fileds: new string[] { "Year" });
            if (finds?.Any() == true)
            {
                return finds.Select(p => p.Year).Distinct().OrderByDescending(p => p);
            }
            return null;
        }

        public async Task<bool> RemoveByAreaYearSchFType(string schFType, string areaCode, int year)
        {
            if (string.IsNullOrWhiteSpace(schFType) || string.IsNullOrWhiteSpace(areaCode) || year < 1) return false;
            return await _areaRecruitPlanRepository.DeleteByAreaYearSchFType(schFType, areaCode, year);
        }
    }
}
