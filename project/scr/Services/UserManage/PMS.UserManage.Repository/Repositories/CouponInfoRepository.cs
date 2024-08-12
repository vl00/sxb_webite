using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.UserManage.Repository.Repositories
{
    public class CouponInfoRepository : ICouponInfoRepository
    {
        private readonly UserDbContext _dbcontext;
        public CouponInfoRepository(UserDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<bool> Add(CouponInfo couponInfo)
        {
            return await _dbcontext.InsertAsync(couponInfo)>0;
        }

        public async Task<CouponInfo> Get(Guid id)
        {
            return await _dbcontext.GetAsync<CouponInfo,Guid>(id);
        
        }

        public async Task<IEnumerable<CouponRule>> GetCouponRule(Guid couponId, bool direct)
        {
            string sql = @"SELECT [CouponRule].* FROM [dbo].[CouponRule]
JOIN CouponRuleType ON CouponRuleType.Id = CouponRule.RuleType
WHERE CouponRuleType.direct = @direct and CouponId =@couponId
";
           return  await _dbcontext.QueryAsync<CouponRule>(sql, new { direct = direct, couponId = couponId });
        }
    }
}
