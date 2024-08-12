using PMS.School.Domain.Entities.WechatDemo;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PMS.School.Application.IServices
{
    public interface ISchoolOverViewService : IApplicationService<SchoolOverViewInfo>
    {
        Task<SchoolOverViewInfo> GetByEID(Guid eid);
        /// <summary>
        /// 存在则删除
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        Task<bool> DeleteIfExisted(Guid eid);
        Task<bool> ModifySchoolCertifications(Guid eid, string certifications);
    }
}
