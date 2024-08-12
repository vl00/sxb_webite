using PMS.School.Domain.Entities.WechatDemo;
using System;
using System.Threading.Tasks;

namespace PMS.School.Domain.IRepositories
{
    public interface ISchoolOverViewRepository : IRepository<SchoolOverViewInfo>
    {
        Task<bool> DeleteIFExist(Guid eid);
    }
}
