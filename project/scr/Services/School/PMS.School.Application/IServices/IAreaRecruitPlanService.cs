using PMS.School.Domain.Entities.WechatDemo;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PMS.School.Application.IServices
{
    public interface IAreaRecruitPlanService : IApplicationService<AreaRecruitPlanInfo>
    {
        /// <summary>
        /// 根据区域号码与学校类型获取
        /// </summary>
        /// <param name="areaCode">区域号码</param>
        /// <param name="schFType">学校类型</param>
        /// <returns></returns>
        Task<IEnumerable<AreaRecruitPlanInfo>> GetByAreaCodeAndSchFType(string areaCode, string schFType, int year = 0);
        Task<IEnumerable<int>> GetYears(string areaCode, string schFType);
        Task<bool> RemoveByAreaYearSchFType(string schFType, string areaCode, int year);
    }
}
