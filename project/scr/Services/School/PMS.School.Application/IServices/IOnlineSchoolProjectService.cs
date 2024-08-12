using PMS.School.Domain.Entities.WechatDemo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.School.Application.IServices
{
    public interface IOnlineSchoolProjectService : IApplicationService<OnlineSchoolProjectInfo>
    {
        Task<IEnumerable<OnlineSchoolProjectInfo>> GetByEID(Guid eid);
        Task<bool> RemoveByEID(Guid eid);
    }
}
