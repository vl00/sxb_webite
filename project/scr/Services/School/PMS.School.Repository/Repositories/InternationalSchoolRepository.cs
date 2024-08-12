using PMS.School.Domain.Entities.SEO_International;
using PMS.School.Domain.IRepositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.School.Repository.Repositories
{
    public class InternationalSchoolRepository : IInternationalSchoolRepository
    {
        readonly ISchoolDataDBContext _db;

        public InternationalSchoolRepository(ISchoolDataDBContext dbc)
        {
            this._db = dbc;
        }

        public async Task<IEnumerable<InternationalSchoolInfo>> GetSchools(Guid pageID)
        {
            var str_SQL = $@"Select * From [SEO_InternationalSchoolInfo] Where PageID = @pageID";
            return await _db.QueryAsync<InternationalSchoolInfo>(str_SQL, new { pageID });
        }

        public async Task<InternationalPage> GetSelectedReading(int id)
        {
            var str_SQL = $@"Select * From [SEO_InternationalPage] Where ID = @id";
            return await _db.QuerySingleAsync<InternationalPage>(str_SQL, new { id });
        }
    }
}