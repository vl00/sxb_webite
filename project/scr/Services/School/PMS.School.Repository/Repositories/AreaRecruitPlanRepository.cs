using PMS.School.Domain.Entities.WechatDemo;
using PMS.School.Domain.IRepositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Threading.Tasks;

namespace PMS.School.Repository.Repositories
{
    public class AreaRecruitPlanRepository : Repository<AreaRecruitPlanInfo, ISchoolDataDBContext>,
        IAreaRecruitPlanRepository
    {
        ISchoolDataDBContext _db;
        public AreaRecruitPlanRepository(ISchoolDataDBContext schoolDataDBContext) : base(schoolDataDBContext)
        {
            _db = schoolDataDBContext;
        }

        public async Task<bool> DeleteByAreaYearSchFType(string schFType, string areaCode, int year)
        {
            var str_SQL = "Delete From [AreaRecruitPlanInfo] Where [AreaCode] = @areaCode AND [SchFType] = @schFType AND [Year] = @year";
            return (await _db.ExecuteAsync(str_SQL, new { schFType, areaCode, year }) > 0);
        }

        public async Task<int> GetRecentYear(string areaCode, string schFType)
        {
            var str_SQL = @"SELECT
                             ISNULL(MAX([year]), 0)
                            FROM
	                            AreaRecruitPlanInfo 
                            WHERE
	                            [AreaCode] = @areaCode AND [SchFType] = @schFType;";
            return await _db.QuerySingleAsync<int>(str_SQL, new { areaCode, schFType });
        }
    }
}
