using PMS.School.Domain.Entities.WechatDemo;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PMS.School.Domain.IRepositories
{
    public interface IOnlineSchoolFractionRepository : IRepository<OnlineSchoolFractionInfo>
    {
        /// <summary>
        /// 获取最近的年份
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <returns></returns>
        Task<int> GetRecentYear(Guid eid);
        Task<IEnumerable<SchoolFractionInfo2>> Get2ByEID(Guid eid, int year = 0, int type = 0);
        Task<IEnumerable<(int, int)>> Get2Years(Guid eid);
        Task<bool> DeleteByEID(Guid eid);
        Task<bool> DeleteByEIDYear(Guid eid,int year);
        Task<bool> Insert2(SchoolFractionInfo2 entity);
        Task<bool> Delete2AsyncByEIDYear(Guid eid, int year);
    }
}
