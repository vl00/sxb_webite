using PMS.School.Domain.Entities.WechatDemo;
using PMS.School.Domain.IRepositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.School.Repository.Repositories
{
    public class OnlineSchoolFractionRepository : Repository<OnlineSchoolFractionInfo, ISchoolDataDBContext>, IOnlineSchoolFractionRepository
    {
        ISchoolDataDBContext _db;
        public OnlineSchoolFractionRepository(ISchoolDataDBContext schoolDataDBContext) : base(schoolDataDBContext)
        {
            _db = schoolDataDBContext;
        }

        public async Task<bool> Delete2AsyncByEIDYear(Guid eid, int year)
        {
            var str_SQL = $"Delete From [SchoolFractionInfo2] Where [EID] = @eid AND [Year] = @year";
            return (await _db.ExecuteAsync(str_SQL, new { eid, year })) > 0;
        }

        public async Task<bool> DeleteByEID(Guid eid)
        {
            var str_SQL = "Delete From [OnlineSchoolFractionInfo] Where [EID] = @eid";
            return (await _db.ExecuteAsync(str_SQL, new { eid }) > 0);
        }

        public async Task<bool> DeleteByEIDYear(Guid eid, int year)
        {
            var str_SQL = "Delete From [OnlineSchoolFractionInfo] Where [EID] = @eid AND [Year] = @year";
            return (await _db.ExecuteAsync(str_SQL, new { eid, year }) > 0);
        }

        public async Task<IEnumerable<SchoolFractionInfo2>> Get2ByEID(Guid eid, int year = 0, int type = 0)
        {
            var str_SQL = $"Select * From [SchoolFractionInfo2] Where [EID] = @eid";
            if (year < 1)
            {
                var str_SQL_MaxYear = $"Select ISNULL(Max(Year),0) as [Year] From [SchoolFractionInfo2] Where [EID] = @eid";
                var maxYear = _db.QuerySingle<int>(str_SQL_MaxYear, new { eid });
                if (maxYear > 0) year = maxYear;
            }
            if (year > 0)
            {
                str_SQL += " AND [Year] = @year";
            }
            if (type > 0) str_SQL += " AND [Type] = @type";
            return await _db.QueryAsync<SchoolFractionInfo2>(str_SQL, new { eid, year, type });
        }

        public async Task<IEnumerable<(int, int)>> Get2Years(Guid eid)
        {
            var str_SQL = "Select [Type],[Year] from [SchoolFractionInfo2] Where EID = @eid";
            return await _db.QueryAsync<(int, int)>(str_SQL, new { eid });
        }

        public async Task<int> GetRecentYear(Guid eid)
        {
            var str_SQL = @"SELECT
                             ISNULL(MAX([year]), 0)
                            FROM
	                            OnlineSchoolFractionInfo 
                            WHERE
	                            EID = @eid;";
            return await _db.QuerySingleAsync<int>(str_SQL, new { eid });
        }

        public async Task<bool> Insert2(SchoolFractionInfo2 entity)
        {
            return (await _db.InsertAsync(entity)) > 0;
        }
    }
}
