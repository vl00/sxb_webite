using PMS.PaidQA.Domain.Entities;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System.Threading.Tasks;

namespace PMS.PaidQA.Repository
{
    public class CommonSettingRepository : Repository<CommonSetting, PaidQADBContext>, ICommonSettingRepository
    {
        PaidQADBContext _PaidQADBContext;
        public CommonSettingRepository(PaidQADBContext paidQADBContext) : base(paidQADBContext)
        {
            _PaidQADBContext = paidQADBContext;
        }

        public async Task<int> Count(string str_Where, object param)
        {
            var str_SQL = $"Select Count(1) From [Order] Where {str_Where}";
            return await _PaidQADBContext.QuerySingleAsync<int>(str_SQL, param);
        }
    }
}
