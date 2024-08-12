using PMS.School.Domain.Entities.WechatDemo;
using System;
using System.Threading.Tasks;

namespace PMS.School.Domain.IRepositories
{
    public interface IAreaRecruitPlanRepository : IRepository<AreaRecruitPlanInfo>
    {
        /// <summary>
        /// 获取最近的年份
        /// </summary>
        /// <returns></returns>
        Task<int> GetRecentYear(string areaCode, string schFType);
        Task<bool> DeleteByAreaYearSchFType(string schFType, string areaCode, int year);
    }
}
