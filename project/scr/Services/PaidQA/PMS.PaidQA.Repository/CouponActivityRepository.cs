using PMS.PaidQA.Domain.Entities;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.PaidQA.Repository
{
    public class CouponActivityRepository: Repository<CouponActivity, PaidQADBContext>, ICouponActivityRepository
    {
        PaidQADBContext _paidQADBContext;
        public CouponActivityRepository(PaidQADBContext dBContext) : base(dBContext)
        {
            _paidQADBContext = dBContext;
        }

        public async Task<IEnumerable<CouponQAExample>> GetCouponQAExample(Guid activityId)
        {
            string sql = @"SELECT * FROM CouponQAExample 
WHERE ActivityId=@ActivityId
ORDER BY Sort";
            return await _paidQADBContext.QueryAsync<CouponQAExample>(sql,new { ActivityId  = activityId });
        }

        public async Task<bool> InsertExampleQA(IEnumerable<CouponQAExample> couponQAExamples)
        {
            using (var tran = _paidQADBContext.BeginTransaction())
            {
                try
                {
                    string delSql = "DELETE CouponQAExample WHERE  ActivityId=@ActivityId";
                    await _paidQADBContext.ExecuteAsync(delSql, new { ActivityId = couponQAExamples.First().ActivityId },tran);
                    var result = (await _paidQADBContext.InsertsAsync(couponQAExamples,tran)) > 0;
                    tran.Commit();
                    return result;
                }
                catch {
                    tran.Rollback();
                    return false;
                }
            }
              
          
        }
        public async Task<bool> InsertSpecialTalent(IEnumerable<CouponSpecialTalent>  couponSpecialTalents)
        {
            using (var tran = _paidQADBContext.BeginTransaction())
            {
                try
                {
                    string delSql = "DELETE CouponSpecialTalent WHERE  ActivityId=@ActivityId";
                    await _paidQADBContext.ExecuteAsync(delSql, new { ActivityId = couponSpecialTalents.First().ActivityId }, tran);

                     var result = (await _paidQADBContext.InsertsAsync(couponSpecialTalents,tran)) > 0;
                    tran.Commit();
                    return result;
                }
                catch
                {
                    tran.Rollback();
                    return false;
                }
            }

          
        }


        public async Task<CouponSpecialTalent> GetRandomSpecialTalent(Guid activityId)
        {
            string randomsql = @"SELECT  * FROM [dbo].[CouponSpecialTalent]
WHERE ActivityId =@ActivityId
order by NEWID()
OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY ";
            return (await _paidQADBContext.QueryAsync<CouponSpecialTalent>(randomsql, new { ActivityId  = activityId})).FirstOrDefault();


        }
    }
}
