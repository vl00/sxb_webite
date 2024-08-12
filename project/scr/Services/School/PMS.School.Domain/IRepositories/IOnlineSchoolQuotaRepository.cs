using PMS.School.Domain.Entities.WechatDemo;
using System;
using System.Threading.Tasks;

namespace PMS.School.Domain.IRepositories
{
    public interface IOnlineSchoolQuotaRepository : IRepository<OnlineSchoolQuotaInfo>
    {
        /// <summary>
        /// 获取最近的年份
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <returns></returns>
        Task<int> GetRecentYear(Guid eid);
        Task<bool> DeleteByEID(Guid eid, int? type = null);
    }
}
