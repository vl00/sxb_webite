using PMS.School.Domain.Entities.WechatDemo;
using PMS.School.Domain.IRepositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.School.Repository.Repositories
{
    public class OnlineSchoolExtLevelRepository : Repository<OnlineSchoolExtLevelInfo, ISchoolDataDBContext>, IOnlineSchoolExtLevelRepository
    {
        ISchoolDataDBContext _db;
        public OnlineSchoolExtLevelRepository(ISchoolDataDBContext schoolDataDBContext) : base(schoolDataDBContext)
        {
            _db = schoolDataDBContext;
        }

        public async Task<bool> DeleteByCityCodeSchFType(int cityCode, string schFType)
        {
            var str_SQL = "Delete From [OnlineSchoolExtLevelInfo] Where [CityCode] = @cityCode AND [SchFType] = @schFType;";
            return (await ExecuteAsync(str_SQL, new { cityCode, schFType })) > 0;
        }
    }
}
