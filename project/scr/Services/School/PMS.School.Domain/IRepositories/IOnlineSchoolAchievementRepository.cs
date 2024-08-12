using PMS.School.Domain.Entities.WechatDemo;
using System;
using System.Threading.Tasks;

namespace PMS.School.Domain.IRepositories
{
    public interface IOnlineSchoolAchievementRepository : IRepository<OnlineSchoolAchievementInfo>
    {
        /// <summary>
        /// 获取最近的年份
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <returns></returns>
        Task<int> GetRecentYear(Guid eid);
        Task<bool> DeleteByEIDYear(Guid eid, int year);
    }
}
