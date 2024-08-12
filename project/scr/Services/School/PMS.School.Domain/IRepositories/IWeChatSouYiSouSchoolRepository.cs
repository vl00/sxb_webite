using PMS.School.Domain.Entities;
using ProductManagement.Framework.EntityFramework;
using ProductManagement.Framework.MSSQLAccessor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.School.Domain.IRepositories
{
    public interface IWeChatSouYiSouSchoolRepository: IRepository<WeChatSouYiSouSchool>
    {
        Task<IEnumerable<WeChatSouYiSouSchool>> GetByEIds(List<Guid> eids);
    }
}
