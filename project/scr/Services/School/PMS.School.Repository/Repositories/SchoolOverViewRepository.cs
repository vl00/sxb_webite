using PMS.School.Domain.Entities.WechatDemo;
using PMS.School.Domain.IRepositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.School.Repository.Repositories
{
    public class SchoolOverViewRepository : Repository<SchoolOverViewInfo, ISchoolDataDBContext>, ISchoolOverViewRepository
    {
        ISchoolDataDBContext _db;
        public SchoolOverViewRepository(ISchoolDataDBContext schoolDataDBContext) : base(schoolDataDBContext)
        {
            _db = schoolDataDBContext;
        }

        public async Task<bool> DeleteIFExist(Guid eid)
        {
            var str_SQL = "Select Count(1) From [SchoolOverViewInfo] Where [EID] = @eid;";
            var count = await _db.QuerySingleAsync<int>(str_SQL, new { eid });
            if (count > 0)
            {
                str_SQL = "Delete From [SchoolOverViewInfo] Where [EID] = @eid;";
                return (await _db.ExecuteAsync(str_SQL, new { eid })) > 0;
            }
            return false;
        }
    }
}
