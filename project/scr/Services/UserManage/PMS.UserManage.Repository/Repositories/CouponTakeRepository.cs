using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.IRepositories;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PMS.UserManage.Domain.Dtos;
using Dapper;

namespace PMS.UserManage.Repository.Repositories
{
    public class CouponTakeRepository : ICouponTakeRepository
    {
        private readonly UserDbContext _dbcontext;
        ILogger _logger;
        public CouponTakeRepository(UserDbContext dbcontext, ILogger<CouponTakeRepository> logger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
        }

        public async Task<bool> Add(Guid Id, Guid userId, string originType, Guid couponId)
        {
            using (var tran = _dbcontext.BeginTransaction())
            {
                try
                {
                    string insertSql = @"INSERT INTO CouponTake(ID,UserId,GetTime,VaildStartTime,VaildEndTime,Status,OriginType,CouponId)
SELECT 
@Id,
@userId,
GETDATE(),
(CASE VaildDateType
WHEN 1 THEN VaildStartDate
WHEN 2 THEN GETDATE()
END
),
(CASE VaildDateType
WHEN 1 THEN VaildEndDate
WHEN 2 THEN DATEADD(HOUR, VaildTime, GETDATE())
END
),
1,
@originType,
Id
FROM CouponInfo  WHERE 
ID=@couponId
AND Status=1
AND Stock>0
AND MaxTake > (SELECT COUNT(1) FROM CouponTake WHERE  CouponTake.UserId=@userId AND CouponId=@CouponId )
AND GETDATE() BETWEEN CouponInfo.GetStartTime AND CouponInfo.GetEndTime;
UPDATE CouponInfo SET Stock= Stock - 1 WHERE  ID = @couponId  AND Stock>0;
";
                    var res = await _dbcontext.ExecuteAsync(insertSql, new { Id, userId, originType, couponId }, tran) > 0;
                    if (res)
                    {
                        CouponRecord couponRecord = new CouponRecord()
                        {
                            Id = Guid.NewGuid(),
                            CouponTakeID = Id,
                            Creator = userId,
                            CreateTime = DateTime.Now,
                            Remark = "领取优惠券"
                        };
                        await _dbcontext.InsertAsync(couponRecord, tran);
                    }
                    tran.Commit();
                    return res;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    _logger.LogError(ex, null);
                    return false;
                }
            }

        }


        public async Task<int> GetTakeCount(Guid couponID, Guid userID)
        {
            string sql = @"SELECT COUNT(1) FROM CouponTake 
WHERE UserId=@userID AND COUPONID=@couponID";
            return await _dbcontext.ExecuteScalarAsync<int>(sql, new { couponID, userID });

        }

        public async Task<CouponTakeDto> GetAsync(Guid id)
        {
            string sql = @"SELECT * FROM CouponTake
JOIN CouponInfo ON CouponTake.CouponId = CouponInfo.Id
WHERE CouponTake.Id = @coupontakeId";
            return (await _dbcontext.QueryAsync<CouponTakeDto, CouponInfo, CouponTakeDto>(sql, (ct, ci) =>
            {
                ct.CouponInfo = ci;
                return ct;
            }, new { coupontakeId = id })).FirstOrDefault();
        }

        public async Task<IEnumerable<CouponTakeDto>> GetCoupons(Guid userID)
        {
            string sql = @"SELECT * FROM CouponTake
JOIN CouponInfo ON CouponTake.CouponId = CouponInfo.Id
WHERE CouponTake.UserId = @userID";
            return await _dbcontext.QueryAsync<CouponTakeDto, CouponInfo, CouponTakeDto>(sql, (ct, ci) =>
            {
                ct.CouponInfo = ci;
                return ct;
            }, new { userID });
        }

        public async Task<bool> CouponRuleJudge(Guid couponID
        , int plateform
        , List<CouponRule> rules)
        {
            //暂定不能不配置规则
            if (rules != null && rules.Any())
            {

                StringBuilder sql = new StringBuilder(@"SELECT COUNT(1) FROM [dbo].[CouponRule] JOIN CouponRuleType ON CouponRuleType.Id = CouponRule.RuleType");
                List<string> rulesqls = rules.Select(r =>
                {
                    if (r.RuleValue == null)
                    {
                        return $"(Platform = {r.Platform} and  ruleType = {r.RuleType} and ruleValue is null )";
                    }
                    else
                    { 
                        return $"(Platform = {r.Platform} and  ruleType = {r.RuleType} and ruleValue ='{r.RuleValue}')";
                    }

                }).ToList();
                sql.AppendFormat(" WHERE   CouponRuleType.direct = 0 AND ({0})", string.Join(" OR ", rulesqls));
                int res = await _dbcontext.ExecuteScalarAsync<int>(sql.ToString(), new { couponID });
                return res > 0;
            }
            else
            {
                return false;
            }


        }


        public async Task<List<Guid>> GetCouponIdsByRules(List<CouponRule> rules)
        {
            //暂定不能不配置规则
            if (rules != null && rules.Any())
            {
                StringBuilder sql = new StringBuilder(@"SELECT CouponId FROM CouponRule");
                List<string> rulesqls = rules.Select(r =>
                {
                    if (r.RuleValue == null)
                    {
                        return $"(Platform = {r.Platform} and  ruleType = {r.RuleType} and ruleValue is null )";
                    }
                    else
                    {
                        return $"(Platform = {r.Platform} and  ruleType = {r.RuleType} and ruleValue ='{r.RuleValue}')";
                    }

                }).ToList();
                sql.AppendFormat(" WHERE {0}", string.Join(" OR ", rulesqls));
                var res = await _dbcontext.QueryAsync(sql.ToString());
                if (res.Any())
                {
                    return res.Select(d => (Guid)(d.CouponId)).ToList();
                }
                else
                {
                    return new List<Guid>();
                }
            }
            else
            {
                return new List<Guid>();
            }

        }




        public async Task<bool> CancelCouponTake(Guid couponTakeID, Guid userID, Guid orderID)
        {
            using (var tran = _dbcontext.BeginTransaction())
            {
                try
                {
                    string updateCouponTakeSql = @"
UPDATE CouponTake SET UsedTime=GETDATE(),STATUS=2,OrderId=@orderID
where Id=@couponTakeID and Status = 1 and GETDATE() BETWEEN vaildStartTime AND vaildEndTime
";
                    var res = await _dbcontext.ExecuteAsync(updateCouponTakeSql, new { orderID, couponTakeID }, tran) > 0;
                    if (res)
                    {
                        //记录券的操作
                        CouponRecord couponRecord = new CouponRecord()
                        {
                            Id = Guid.NewGuid(),
                            CouponTakeID = couponTakeID,
                            Creator = userID,
                            CreateTime = DateTime.Now,
                            Remark = "消费优惠券"
                        };
                        await _dbcontext.InsertAsync(couponRecord, tran);
                    }
                    tran.Commit();
                    return res;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    _logger.LogError(ex, null);
                    return false;
                }

            }

        }

        public async Task<bool> UpdateReadTime(IEnumerable<Guid> couponTakeIds, DateTime readTime)
        {
            string sql = @"update CouponTake set ReadTime=@readTime WHERE Id IN @couponTakeIds AND ReadTime is NULL";
            return (await _dbcontext.ExecuteAsync(sql, new { couponTakeIds, readTime })) > 0;
        }

        public async Task<IEnumerable<CouponTakeDto>> GetWaitUseCoupons(Guid userID)
        {
            string sql = @"SELECT * FROM CouponTake
JOIN CouponInfo ON CouponTake.CouponId = CouponInfo.Id
WHERE CouponTake.UserId = @userID AND CouponTake.Status = 1 AND GETDATE() < VaildEndTime ";
            return await _dbcontext.QueryAsync<CouponTakeDto, CouponInfo, CouponTakeDto>(sql, (ct, ci) =>
            {
                ct.CouponInfo = ci;
                return ct;
            }, new { userID });
        }


        public async Task<bool> BackCoupon(Guid orderId)
        {

            using (var tran = _dbcontext.BeginTransaction())
            {
                try
                {
                    string queryCouponTakesSql = @"SELECT * FROM CouponTake
WHERE OrderId = @orderId";
                    var couponTakes = await _dbcontext.QueryAsync<CouponTake>(queryCouponTakesSql, new { orderId }, tran);
                    if (couponTakes != null && couponTakes.Any())
                    {
                        string sql = @"UPDATE [dbo].[CouponTake]
SET UsedTime=NULL,Status = 1,OrderId = NULL
WHERE OrderId=@orderId AND Status = 2";
                        bool res = (await _dbcontext.ExecuteAsync(sql, new { orderId }, tran)) > 0;
                        if (res)
                        {
                            bool recordRes = (await _dbcontext.InsertsAsync(couponTakes.Select(ct =>
                            {
                                return new CouponRecord()
                                {
                                    Id = Guid.NewGuid(),
                                    CouponTakeID = ct.Id,
                                    CreateTime = DateTime.Now,
                                    Creator = default(Guid),
                                    Remark = $"退回优惠券，原有订单ID：{orderId}"
                                };
                            }), tran)) > 0;
                            if (recordRes)
                            {
                                tran.Commit();
                                return true;
                            }
                            else
                            {
                                tran.Rollback();
                                return false;
                            }
                        }
                        else
                        {
                            tran.Rollback();
                            return false;
                        }

                    }
                    else
                    {
                        tran.Rollback();
                        return false;
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, null);
                    tran.Rollback();
                    return false;
                }
            }
        }

        public async Task<bool> InsertOrderPreUseRecord(Guid orderId, Guid couponTakeId)
        {
            return (await _dbcontext.InsertAsync(new CouponOrderPreUseRecord()
            {
                Id = Guid.NewGuid(),
                CouponTakeId = couponTakeId,
                OrderId = orderId
            })) > 0;
        }

        public async Task<IEnumerable<CouponOrderPreUseRecord>> GetPreUseRecordBy(Guid orderId)
        {
            string sql = @"
SELECT * FROM CouponOrderPreUseRecord
WHERE OrderId=@orderId";

            return await _dbcontext.QueryAsync<CouponOrderPreUseRecord>(sql, new { orderId });
        }
        public async Task<bool> ClearPreUseRecord(Guid orderId)
        {
            string delPreUseRecord = @"
DELETE CouponOrderPreUseRecord WHERE OrderId =@orderId
";
            return (await _dbcontext.ExecuteAsync(delPreUseRecord, new { orderId })) > 0;
        }

        public async Task<bool> OrderHasUseCoupon(Guid orderId)
        {
            string sql = @"SELECT COUNT(1) FROM CouponTake
WHERE OrderId=@orderId";

            return (await _dbcontext.ExecuteScalarAsync<int>(sql, new { orderId })) > 0;
        }

        public async Task<int> GetUnUseCouponCount(Guid userId, Guid couponId)
        {
            string sql = @"SELECT Count(1) FROM CouponTake
WHERE UserId=@userId AND CouponId=@couponId AND GETDATE() BETWEEN VaildStartTime AND VaildEndTime AND Status=1 ";
            return (await _dbcontext.ExecuteScalarAsync<int>(sql, new { userId, couponId }));
        }

        public async Task<CouponTakeDto> GetLatestUnUseCoupon(Guid userId, Guid couponId)
        {
            string sql = @"SELECT top 1 * FROM CouponTake
WHERE UserId=@userId AND CouponId=@couponId AND GETDATE() BETWEEN VaildStartTime AND VaildEndTime AND Status=1 
ORDER BY GetTime DESC";
            var result = await _dbcontext.QueryAsync<CouponTakeDto>(sql, new { userId, couponId });
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<CouponTake>> GetWillExpireCoupons(List<Guid> specialCoupons = null,int beforeHour=24)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            StringBuilder sql = new StringBuilder(@"SELECT * FROM CouponTake where 
([Status]=1 or UsedTime is null)
and GETDATE() BETWEEN DATEADD(HOUR,@beforeHour *-1,VaildEndTime) AND VaildEndTime
and DATEADD(HOUR,@beforeHour,VaildStartTime) < VaildEndTime
");
            dynamicParameters.Add("beforeHour", beforeHour);
            if (specialCoupons?.Any() == true)
            {
                sql.Append(" and CouponId  in @couponIds ");
                dynamicParameters.Add("couponIds", specialCoupons);
            }
            sql.Append(" order by GetTime desc ");
           return await  _dbcontext.QueryAsync<CouponTake>(sql.ToString(), dynamicParameters);

        }
    }
}