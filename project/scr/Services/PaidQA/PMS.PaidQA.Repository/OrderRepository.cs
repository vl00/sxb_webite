using Microsoft.Extensions.Logging;
using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.Enums;
using ProductManagement.Framework.Foundation;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using Sxb.GenerateNo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.PaidQA.Repository
{
    public class OrderRepository : Repository<Order, PaidQADBContext>, IOrderRepository
    {

        const int timeoutsecond = 86400;
        ILogger _logger;
        PaidQADBContext _paidQADBContext;
        IOrderOpreateLogRepository _opreateLogRepository;
        ITalentSettingRepository _talentSettingRepository;
        public OrderRepository(PaidQADBContext dBContext
            , IOrderOpreateLogRepository opreateLogRepository
            , ITalentSettingRepository talentSettingRepository
            , ILogger<OrderRepository> logger
           ) : base(dBContext)
        {
            _paidQADBContext = dBContext;
            _opreateLogRepository = opreateLogRepository;
            _talentSettingRepository = talentSettingRepository;
            _logger = logger;
        }



        /// <summary>
        /// 微信支付回调修改订单状态
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="updateStatus"></param>
        /// <param name="userId"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public async Task<bool> WechatPayCallUpdateOrder(Guid orderid, OrderStatus updateStatus, Guid userId, string remark, int paystatus)
        {
            using (var tran = _paidQADBContext.BeginTransaction())
            {
                try
                {
                    Order order = new Order() { ID = orderid, Status = updateStatus, UpdateTime = DateTime.Now };
                    bool updateOrderResult = await this.UpdateAsync(order
                        , userId
                        , tran
                        , new[] { "Status", "UpdateTime" }, remark, paystatus);
                    if (updateOrderResult)
                    {
                        tran.Commit();
                        return true;
                    }
                    else
                    {
                        throw new Exception("WechatPayCallUpdateOrder事务操作保存失败。");
                    }
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    //记录日志
                    return false;
                }

            }
        }
        public async Task<bool> WechatPayCallOrderLog(Guid orderid, int paystatus, string remark)
        {
            var sql = @"insert into [OrderOpreateLog] (ID,OrderId,PayStatus,Remark,CreateTime) values 
(@ID,@OrderId,@PayStatus,@Remark,@CreateTime)";
            return await _paidQADBContext.ExecuteAsync(sql, new
            {
                ID = Guid.NewGuid(),
                OrderId = orderid,
                PayStatus = paystatus,
                Remark = remark,
                CreateTime = DateTime.Now,

            }) > 0;
        }
        /// <summary>
        /// 检查订单状态扭转的合理性
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool UpdateCheckLegal(Guid orderid, int status)
        {
            var old = Get(orderid);
            if (null != old)
            {
                //由未支付转换成待提问
                if (status == (byte)OrderStatus.WaitingAsk && old.Status == (byte)OrderStatus.UnPay)
                    return true;

            }
            return false;

        }

        public async Task<int> Count(string str_Where, object param)
        {
            var str_SQL = $"Select Count(1) From [Order] Where {str_Where}";
            return await _paidQADBContext.QuerySingleAsync<int>(str_SQL, param);
        }

        public async Task<double> GetSixHoursReplyPercentByTalentUserID(System.Guid talentUserID)
        {
            var str_Where = "Status = 4 AND AnswerID = @talentUserID";
            var finishOrderCount = await Count(str_Where, new { talentUserID });
            if (finishOrderCount < 1) return 0;
            var str_SQL = $@"SELECT
	                            COUNT(1) AS [FindCount] 
                            FROM
	                            [Order] 
                            WHERE
	                            FirstReplyTimespan <= 21600 
	                            AND Status = 4
	                            AND AnswerID = @talentUserID";
            var findCount = await _paidQADBContext.QuerySingleAsync<int>(str_SQL, new { talentUserID });
            double result = (double)findCount / (double)finishOrderCount;
            return result.CutDoubleWithN(2);
        }



        /// <summary>
        /// 重写update函数，监听是否修改了状态的字段。
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="transaction"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(Order entity
            , Guid operateUserID
            , IDbTransaction transaction = null
            , string[] fields = null
            , string remark = null, int paystatus = 0)
        {
            bool isOutsideTrans = true;
            try
            {

                if (transaction == null)
                {
                    transaction = this.BeginTransaction();
                    isOutsideTrans = false;
                }
                entity.UpdateTime = DateTime.Now;
                if (fields != null && fields.Any())
                {
                    fields.Append(nameof(entity.UpdateTime));
                }
                var result = await base.UpdateAsync(entity, transaction, fields);
                if (fields != null && fields.Any(f => f.Equals(nameof(Order.Status))))
                {
                    OrderOpreateLog orderOpreateLog = new OrderOpreateLog()
                    {
                        Id = Guid.NewGuid(),
                        CreateTime = DateTime.Now,
                        OrderId = entity.ID,
                        UpdateStatus = (int)entity.Status,
                        Remark = remark,
                        UserId = operateUserID,
                        PayStatus = paystatus

                    };
                    bool addlogResult = await _opreateLogRepository.AddAsync(orderOpreateLog, transaction);
                    if (!addlogResult)
                    {
                        throw new Exception("记录订单流转日志发生异常。");
                    }
                }

                if (result)
                {
                    if (!isOutsideTrans)
                    {
                        transaction.Commit();
                    }
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                if (!isOutsideTrans)
                {
                    transaction.Rollback();
                }
                return false;
            }


        }


        public async Task<bool> TimOutCloseOrder(Order order, Guid operatorID, string remark)
        {
            using (var tran = this.BeginTransaction())
            {
                try
                {

                    //超时关闭订单
                    string sql = @"UPDATE [Order] 
SET Status=6,FinishTime=GETDATE(),UpdateTime=GETDATE()
WHERE  ID=@id AND (Status=1 OR Status=2) AND (DATEDIFF(SECOND, CreateTime, GETDATE())>=@timeoutsecond Or  ExpireTime<GETDATE())";
                    int effectRow = await this.ExecuteAsync(sql, new { id = order.ID, timeoutsecond }, tran);
                    OrderOpreateLog orderOpreateLog = new OrderOpreateLog()
                    {
                        Id = Guid.NewGuid(),
                        CreateTime = DateTime.Now,
                        OrderId = order.ID,
                        UpdateStatus = (int)OrderStatus.TimeOut,
                        Remark = remark,
                        UserId = operatorID
                    };
                    await _opreateLogRepository.AddAsync(orderOpreateLog, tran);
                    tran.Commit();
                    return effectRow > 0;
                }
                catch (Exception)
                {
                    tran.Rollback();
                    return false;
                }
            }



        }

        public async Task<bool> OverOrder(Order order, Guid operatorID, string remark)
        {
            using (var tran = this.BeginTransaction())
            {
                try
                {
                    //正常关闭订单
                    string sql = @"UPDATE [Order] 
SET Status=4,FinishTime=GETDATE(),UpdateTime=GETDATE()
WHERE  ID=@id AND (Status=3)";
                    int effectRow = await this.ExecuteAsync(sql, new { id = order.ID }, tran);
                    OrderOpreateLog orderOpreateLog = new OrderOpreateLog()
                    {
                        Id = Guid.NewGuid(),
                        CreateTime = DateTime.Now,
                        OrderId = order.ID,
                        UpdateStatus = (int)OrderStatus.Finish,
                        Remark = remark,
                        UserId = operatorID
                    };
                    await _opreateLogRepository.AddAsync(orderOpreateLog, tran);
                    tran.Commit();
                    return effectRow > 0;
                }
                catch (Exception)
                {
                    tran.Rollback();
                    return false;
                }
            }
        }


        public async Task<(bool Successed, Order OriginOrder, Order NewOrder)> TransitingOrder(Order newOrder, Guid originOrderID, Guid talentUserID, Guid operatorID, string remark)
        {
            using (var tran = this.BeginTransaction())
            {
                try
                {
                    //订单切换流转状态，必须是未进行、未过期,未转过单的订单才能进行转单
                    string sql = @"UPDATE [Order] 
SET Status=5,FinishTime=GETDATE(),UpdateTime=GETDATE()
WHERE  ID=@id AND (Status=1 or Status=2) AND  DATEDIFF(SECOND, CreateTime, GETDATE())<=@timeoutsecond";
                    int effectRow = await this.ExecuteAsync(sql, new { id = originOrderID, timeoutsecond }, tran);
                    OrderOpreateLog orderOpreateLog = new OrderOpreateLog()
                    {
                        Id = Guid.NewGuid(),
                        CreateTime = DateTime.Now,
                        OrderId = originOrderID,
                        UpdateStatus = (int)OrderStatus.Transfered,
                        Remark = remark,
                        UserId = operatorID
                    };
                    await _opreateLogRepository.AddAsync(orderOpreateLog, tran);
                    if (effectRow <= 0) throw new Exception("旧订单状态流转失败。");
                    //创建一张新订单
                    var originOrder = await this.GetAsync(originOrderID, tran);
                    newOrder.Status = OrderStatus.WaitingAsk;//转单的订单默认待提问
                    newOrder.CreatorID = originOrder.CreatorID;
                    newOrder.OrginAskID = originOrder.ID;
                    newOrder.IsAnonymous = originOrder.IsAnonymous;
                    newOrder.IsPublic = originOrder.IsPublic;
                    bool isNewOrderSuccess = await this.AddAsync(newOrder, tran);
                    if (!isNewOrderSuccess) { throw new Exception("创建新订单时发生异常。"); }
                    tran.Commit();
                    return (true, originOrder, newOrder);
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    _logger.LogError(ex, "转单数据流转发生异常。");
                    return (false, null, null);
                }
            }
        }

        public override Task<bool> UpdateAsync(Order entity, IDbTransaction transaction = null, string[] fields = null)
        {
            throw new Exception("由于订单需要全局监听状态变更，所以关闭该方法。");
        }
        public override bool Update(Order entity)
        {
            throw new Exception("由于订单需要全局监听状态变更，所以关闭该方法。");
        }
        public override bool Update(Order entity, System.Data.IDbTransaction transaction = null, string[] fields = null)
        {
            throw new Exception("由于订单需要全局监听状态变更，所以关闭该方法。");
        }

        public override Task<bool> UpdateAsync(Order entity)
        {
            throw new Exception("由于订单需要全局监听状态变更，所以关闭该方法。");
        }

        public async Task<IEnumerable<Guid>> GetUnReadOrderIDs(Guid userID, OrderStatus? status = null)
        {
            //var str_SQL2 = $@"SELECT DISTINCT
            //                 ( o.ID ) 
            //                FROM
            //                 [Order] AS o
            //                 JOIN Message AS m ON m.SenderID = o.AnswerID 
            //                 AND o.ID = m.OrderID 
            //                WHERE
            //                 m.ReadTime IS NULL 
            //                 AND m.IsValid = 1 
            //                 AND o.CreatorID =@userID";
            var str_Where = " AND o.Status in (2,3,4)";
            if (status != null)
            {
                str_Where = $" AND o.Status = {(int)status}";
            }
            var str_SQL = $@"SELECT 
	                            DISTINCT(OrderID) 
                            FROM
	                            Message as m
                                Left Join [Order] as o on o.id = m.OrderID
                            WHERE
	                            m.ReceiveID = @userID 
	                            AND m.ReadTime IS NULL 
	                            AND m.IsValid = 1
	                            {str_Where}";
            return await _paidQADBContext.QueryAsync<Guid>(str_SQL, new { userID });
        }

        public async Task<string> GetPayOpenID(Guid userId)
        {

            string sql = @"SELECT * FROM Temp_OpendMappers
WHERE UserID=@userId";
            var result = await _paidQADBContext.QueryAsync(sql, new { userId });
            if (result != null && result.Any())
            {
                return result.FirstOrDefault().OpenID;
            }
            else
            {
                return string.Empty;
            }

        }

        public async Task<IEnumerable<Guid>> GetIsAskOrderIDs(IEnumerable<Guid> orderIDs, Guid talentUserID)
        {
            var str_SQL = "Select ID from [Order] WHERE CreatorID = @talentUserID And ID IN @orderIDs";
            return await _paidQADBContext.QueryAsync<Guid>(str_SQL, new { orderIDs, talentUserID });
        }

        public async Task<IEnumerable<Guid>> GetWaitAskToTimeOutOrderIDs(IEnumerable<Guid> orderIDs)
        {
            var str_SQL = $@"SELECT
	                            ID 
                            FROM
	                            [Order] 
                            WHERE
	                            Status = 6 
	                            AND ID IN ( SELECT DISTINCT(OrderID) FROM Message WHERE OrderID IN @orderIDs AND MsgType = 3 )";
            var finds = await _paidQADBContext.QueryAsync<Guid>(str_SQL, new { orderIDs });
            if (finds?.Any() == true)
            {
                return finds;
            }
            else
            {
                return new Guid[0];
            }
        }

        public async Task<Order> GetPayOrderBy(Guid transitingOrderID)
        {
            string sql = @"
WITH _Orders AS (
		SELECT * FROM [Order] WHERE ID = @orderID
		UNION ALL
		SELECT [Order].* FROM [Order],_Orders WHERE [Order].ID = _Orders.OrginAskID
)
SELECT TOP 1 * FROM _Orders
WHERE 
Status  = 5
AND 
OrginAskID IS NULL
ORDER BY CreateTime  ASC
";
            var result = await this._paidQADBContext.QueryAsync<Order>(sql, new { orderID = transitingOrderID });
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<OrderTag>> GetTagsByID(Guid id)
        {
            return await _paidQADBContext.QueryAsync<OrderTag>("Select * From OrderTag Where OrderID = @id", new { id });
        }



        public async Task<bool> IsHasProcessingOrder(Guid answerId)
        {
            string sql = @"SELECT COUNT(1) FROM [Order] WHERE
AnswerID=@answerId
AND  Status=3 ";
            return (await _paidQADBContext.ExecuteScalarAsync<int>(sql, new { answerId })) > 0;
        }

        public async Task<bool> ExistsEffectiveOrder(Guid talentUserId, Guid userId)
        {
            string sql = @"SELECT COUNT(1) FROM [ORDER]
where 
status in (1,2,3)
AND creatorid=@userId
and answerid=@talentUserId";
           return  (await _paidQADBContext.ExecuteScalarAsync<int>(sql,new { talentUserId, userId }))>0;
        }

        public async Task<bool> ExistsOrderAsync(Guid userId)
        {
            string sql = @"SELECT COUNT(1) FROM [ORDER]
where 
status > 0
AND creatorid=@userId";
            return (await _paidQADBContext.ExecuteScalarAsync<int>(sql, new {  userId })) > 0;
        }
    }
}
