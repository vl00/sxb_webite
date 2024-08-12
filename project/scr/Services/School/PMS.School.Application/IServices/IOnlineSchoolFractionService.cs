using PMS.School.Domain.Entities.WechatDemo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.School.Application.IServices
{
    public interface IOnlineSchoolFractionService : IApplicationService<OnlineSchoolFractionInfo>
    {
        Task<IEnumerable<OnlineSchoolFractionInfo>> GetByEID(Guid eid, int year = 0);
        Task<IEnumerable<SchoolFractionInfo2>> Get2ByEID(Guid eid, int year = 0, int type = 0);
        Task<IEnumerable<int>> GetYears(Guid eid);
        Task<IEnumerable<KeyValuePair<int, IEnumerable<int>>>> Get2Years(Guid eid);
        Task<bool> RemoveByEID(Guid eid);
        /// <summary>
        /// 根据EID与年份删除
        /// </summary>
        /// <param name="eid"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        Task<bool> RemoveByEIDYear(Guid eid, int year);
        Task<bool> Add2Async(SchoolFractionInfo2 entity);
        Task<bool> Remove2AsyncByEIDYear(Guid eid, int year);
        Task<IEnumerable<OnlineSchoolFractionInfo>> GetAll();
    }
}
