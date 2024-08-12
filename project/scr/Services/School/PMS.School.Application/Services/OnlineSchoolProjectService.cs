using PMS.School.Application.IServices;
using PMS.School.Domain.Entities.WechatDemo;
using PMS.School.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.School.Application.Services
{
    public class OnlineSchoolProjectService : ApplicationService<OnlineSchoolProjectInfo>, IOnlineSchoolProjectService
    {
        IOnlineSchoolProjectRepository _onlineSchoolProjectRepository;
        public OnlineSchoolProjectService(IOnlineSchoolProjectRepository onlineSchoolProjectRepository) : base(onlineSchoolProjectRepository)
        {
            _onlineSchoolProjectRepository = onlineSchoolProjectRepository;
        }

        public async Task<IEnumerable<OnlineSchoolProjectInfo>> GetByEID(Guid eid)
        {
            if (eid == Guid.Empty) return null;
            var str_Where = "EID = @eid";
            return await _onlineSchoolProjectRepository.GetByAsync(str_Where, new { eid });
        }

        public async Task<bool> RemoveByEID(Guid eid)
        {
            if (eid == Guid.Empty) return false;
            return await _onlineSchoolProjectRepository.DeleteByEID(eid);
        }
    }
}
