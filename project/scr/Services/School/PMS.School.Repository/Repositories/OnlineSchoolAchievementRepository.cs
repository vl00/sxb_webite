using PMS.School.Domain.Entities.WechatDemo;
using PMS.School.Domain.IRepositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Threading.Tasks;

namespace PMS.School.Repository.Repositories
{
    public class OnlineSchoolAchievementRepository : Repository<OnlineSchoolAchievementInfo, ISchoolDataDBContext>, IOnlineSchoolAchievementRepository
    {
        ISchoolDataDBContext _db;
        public OnlineSchoolAchievementRepository(ISchoolDataDBContext schoolDataDBContext) : base(schoolDataDBContext)
        {
            _db = schoolDataDBContext;
        }

        public async Task<bool> DeleteByEIDYear(Guid eid, int year)
        {
            var str_SQL = "Delete From [OnlineSchoolAchievementInfo] Where [EID] = @eid AND [Year] = @year";
            return (await _db.ExecuteAsync(str_SQL, new { eid, year })) > 0;
        }

        public async Task<int> GetRecentYear(Guid eid)
        {
            var str_SQL = @"SELECT
                             ISNULL(MAX([year]), 0)
                            FROM
	                            OnlineSchoolAchievementInfo 
                            WHERE
	                            EID = @eid;";
            return await _db.QuerySingleAsync<int>(str_SQL, new { eid });
        }
    }
}
