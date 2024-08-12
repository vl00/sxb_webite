using PMS.School.Domain.Entities.WechatDemo;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PMS.School.Domain.IRepositories
{
    public interface IOnlineSchoolRecruitRepository : IRepository<OnlineSchoolRecruitInfo>
    {
        /// <summary>
        /// 获取最近的年份
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <returns></returns>
        Task<int> GetRecentYear(Guid eid);
        Task<IEnumerable<RecruitScheduleInfo>> GetRecruitScheduleByRecruitIDs(IEnumerable<Guid> recruitIDs);
        Task<IEnumerable<RecruitScheduleInfo>> GetRecruitSchedule(int cityCode, IEnumerable<int> recruitTypes, string schFType, int? areaCode = null);
        Task<bool> DeleteIFExist(Guid eid);
        Task<bool> InsertRecruitSchedule(RecruitScheduleInfo entity);
        Task<bool> DeleteRecruitSchedules(string cityCode, string areaCode, string schFType, int recruitType);
    }
}
