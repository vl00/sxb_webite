using PMS.School.Domain.Entities.WechatDemo;
using PMS.School.Domain.IRepositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Threading.Tasks;

namespace PMS.School.Repository.Repositories
{
    public class OnlineSchoolProjectRepository : Repository<OnlineSchoolProjectInfo, ISchoolDataDBContext>, IOnlineSchoolProjectRepository
    {
        ISchoolDataDBContext _db;
        public OnlineSchoolProjectRepository(ISchoolDataDBContext schoolDataDBContext) : base(schoolDataDBContext)
        {
            _db = schoolDataDBContext;
        }

        public async Task<bool> DeleteByEID(Guid eid)
        {
            var str_SQL = "Delete From [OnlineSchoolProjectInfo] Where [EID] = @eid";
            return (await _db.ExecuteAsync(str_SQL, new { eid })) > 0;
        }
    }
}
