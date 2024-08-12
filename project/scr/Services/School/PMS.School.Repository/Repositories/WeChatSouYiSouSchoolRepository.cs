using PMS.School.Domain.Entities;
using PMS.School.Domain.IRepositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.School.Repository.Repositories
{
    public class WeChatSouYiSouSchoolRepository : Repository<WeChatSouYiSouSchool, ISchoolDataDBContext>, IWeChatSouYiSouSchoolRepository
    {
        ISchoolDataDBContext _db;
        public WeChatSouYiSouSchoolRepository(ISchoolDataDBContext schoolDataDBContext) : base(schoolDataDBContext)
        {
            _db = schoolDataDBContext;
        }

        public async Task<IEnumerable<WeChatSouYiSouSchool>> GetByEIds(List<Guid> eids)
        {
            string sql = @"SELECT * FROM WeChatSouYiSouSchool
JOIN (SELECT EId,MAX(ModifiTime) lastTime FROM WeChatSouYiSouSchool where EId in @eids and WXSchoolId is not null  group by EId ) LASTEID 
ON LASTEID.EId = WeChatSouYiSouSchool.EId AND LASTEID.lastTime = WeChatSouYiSouSchool.ModifiTime";
            return await _db.QueryAsync<WeChatSouYiSouSchool>(sql, new { eids });
        }
    }
}
