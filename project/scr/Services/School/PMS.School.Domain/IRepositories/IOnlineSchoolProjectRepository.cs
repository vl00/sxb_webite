using PMS.School.Domain.Entities.WechatDemo;
using System;
using System.Threading.Tasks;

namespace PMS.School.Domain.IRepositories
{
    public interface IOnlineSchoolProjectRepository : IRepository<OnlineSchoolProjectInfo>
    {
        Task<bool> DeleteByEID(Guid eid);
    }
}
