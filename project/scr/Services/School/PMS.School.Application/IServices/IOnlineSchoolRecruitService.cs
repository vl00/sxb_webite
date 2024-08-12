using PMS.School.Domain.Entities.WechatDemo;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PMS.School.Application.IServices
{
    public interface IOnlineSchoolRecruitService : IApplicationService<OnlineSchoolRecruitInfo>
    {
        Task<IEnumerable<OnlineSchoolRecruitInfo>> GetByEID(Guid eid, int year = 0, int type = 0);

        Task<IEnumerable<KeyValuePair<int, IEnumerable<int>>>> GetYears(Guid eid);
        /// <summary>
        /// 获取招生日程
        /// </summary>
        /// <param name="RecruitID"></param>
        /// <returns></returns>
        Task<IEnumerable<RecruitScheduleInfo>> GetRecruitSchedules(IEnumerable<Guid> recruitIDs);
        Task<IEnumerable<RecruitScheduleInfo>> GetRecruitSchedules(int cityCode, IEnumerable<int> recruitTypes, string schFType, int? areaCode = null);

        /// <summary>
        /// 存在则删除
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        Task<bool> DeleteIfExisted(Guid eid);
        /// <summary>
        /// 插入招生日程
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> InsertRecruitSchedule(RecruitScheduleInfo entity);
        Task<bool> RemoveRecruitSchedules(string cityCode, string areaCode, string schFType, int recruitType);
        /// <summary>
        /// 获取费用年份
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        Task<IEnumerable<int>> GetCostYears(Guid eid);
        Task<IEnumerable<OnlineSchoolRecruitInfo>> GetCostByYear(Guid eid, int year);
    }
}
