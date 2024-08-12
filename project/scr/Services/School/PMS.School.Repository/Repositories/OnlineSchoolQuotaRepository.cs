using PMS.School.Domain.Entities.WechatDemo;
using PMS.School.Domain.IRepositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Threading.Tasks;

namespace PMS.School.Repository.Repositories
{
    public class OnlineSchoolQuotaRepository : Repository<OnlineSchoolQuotaInfo, ISchoolDataDBContext>, IOnlineSchoolQuotaRepository
    {
        ISchoolDataDBContext _db;
        public OnlineSchoolQuotaRepository(ISchoolDataDBContext schoolDataDBContext) : base(schoolDataDBContext)
        {
            _db = schoolDataDBContext;
        }

        public async Task<bool> DeleteByEID(Guid eid, int? type = null)
        {
            var str_SQL = "Delete From [OnlineSchoolQuotaInfo] Where [EID] = @eid";
            if (type.HasValue) str_SQL += " AND [Type] = @type";
            return (await _db.ExecuteAsync(str_SQL, new { eid, type }) > 0);
        }

        public async Task<int> GetRecentYear(Guid eid)
        {
            var str_SQL = @"SELECT
                             ISNULL(MAX([year]), 0)
                            FROM
	                            OnlineSchoolQuotaInfo 
                            WHERE
	                            EID = @eid;";
            return await _db.QuerySingleAsync<int>(str_SQL, new { eid });
        }
    }
}
