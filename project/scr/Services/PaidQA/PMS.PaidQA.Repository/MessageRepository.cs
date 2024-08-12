using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.Enums;
using PMS.PaidQA.Domain.Message;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.PaidQA.Repository
{
    public class MessageRepository : Repository<Message, PaidQADBContext>, IMessageRepository
    {
        PaidQADBContext _dBContext;
        ILogger _logger;
        IOrderRepository _orderRepository;
        IOrderOpreateLogRepository _orderOpreateLogRepository;
        public MessageRepository(PaidQADBContext dBContext
            , ILogger<MessageRepository> logger
            , IOrderRepository orderRepository, IOrderOpreateLogRepository orderOpreateLogRepository) : base(dBContext)
        {
            _logger = logger;
            _dBContext = dBContext;
            _orderRepository = orderRepository;
            _orderOpreateLogRepository = orderOpreateLogRepository;
        }



        public async Task<bool> CreateQuetion(List<Message> messages, Order order)
        {
            var firstMessage = messages.First();
            using (var tran = this.BeginTransaction())
            {
                try
                {
                    //插入一条消息
                    bool result = await this.AddsAsync(messages, tran);
                    //订单表提问次数-1
                    string updateOrderSql = @"UPDATE [Order] SET AskRemainCount= AskRemainCount-1,UpdateTime = GETDATE()
WHERE ID=@id AND AskRemainCount>0
";
                    int updateAskRemainCountEffectRows = await this.ExecuteAsync(updateOrderSql, new { id = firstMessage.OrderID }, tran);
                    if (updateAskRemainCountEffectRows <= 0)
                    {
                        throw new Exception("更新提问次数失败。订单不存在或者没有提问机会。");
                    }
                    //更新状态和是否匿名字段
                    string queryOrderSql = @"SELECT * FROM [Order] WHERE ID=@id";
                    var orderNow = (await this._dBContext.QueryAsync<Order>(queryOrderSql, new { id = firstMessage.OrderID }, tran)).FirstOrDefault();
                    if (orderNow == null)
                    {
                        throw new Exception("订单不存在");
                    }
                    if (orderNow.Status == OrderStatus.WaitingAsk)
                    {
                        //如果这时订单是待提问的话（第一次发消息），那么需要更新订单状态为待回答
                        orderNow.Status = OrderStatus.WaitingReply;
                        orderNow.AnonyName = GenerateAnonyName();
                        orderNow.IsAnonymous = order.IsAnonymous;
                        orderNow.UpdateTime = DateTime.Now;
                        orderNow.IsPublic = order.IsPublic;
                        orderNow.QuestionID = firstMessage.ID;
                        string[] fileds = new[] { "Status", "AnonyName", "IsAnonymous", "IsPublic", "UpdateTime", "QuestionID" };
                        string remark = "用户提问。";
                        bool isOrderUpdateSuccess = await _orderRepository.UpdateAsync(orderNow
                         , orderNow.CreatorID
                         , tran
                         , fileds
                         , remark);
                        if (!isOrderUpdateSuccess)
                        {
                            throw new Exception("更新订单状态失败。");
                        }
                        else
                        {
                            //对外更新一下订单对象
                            order = orderNow;
                        }
                    }
                    else
                    {
                        throw new Exception("当前订单状态不是待提问状态，不能进行提问创建。");
                    }
                    tran.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "用户创建提问数据操作异常。");
                    tran.Rollback();
                    return false;
                }

            }

        }

        public async Task<(bool Success, Order Order)> UserSendMessage(Message message)
        {
            using (var tran = this.BeginTransaction())
            {
                try
                {
                    //插入一条消息
                    bool result = await this.AddAsync(message, tran);
                    //订单表提问次数-1
                    string updateOrderSql = @"UPDATE [Order] SET AskRemainCount= AskRemainCount-1,UpdateTime = GETDATE()
WHERE ID=@id AND AskRemainCount>0
";
                    int updateAskRemainCountEffectRows = await this.ExecuteAsync(updateOrderSql, new { id = message.OrderID }, tran);
                    if (updateAskRemainCountEffectRows <= 0)
                    {
                        throw new Exception("更新提问次数失败。订单不存在或者没有提问机会。");
                    }
                    Order order = await this._orderRepository.GetAsync(message.OrderID, tran);
                    tran.Commit();
                    return (true, order);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "用户发送消息数据操作异常。");
                    tran.Rollback();
                    return (false, null);
                }

            }

        }

        public async Task<(bool Success, bool IsFirstReply)> TalentSendMessage(Message message)
        {
            using (var tran = this.BeginTransaction())
            {
                try
                {
                    bool isFirstReply = false;
                    //插入一条消息
                    bool result = await this.AddAsync(message, tran);
                    //如果订单是在等待回复的话，那么此回复就是第一次回复，修改订单状态为进行中，第一次回复时间
                    string sql = @"UPDATE[Order] SET Status = @processingStatu, IsReply = 1, UpdateTime = GETDATE(),FirstReplyTimespan = DATEDIFF(SECOND, (SELECT CreateTime FROM Message WHERE ID=[Order].QuestionID), GETDATE())
WHERE ID=@id AND Status = @status AND QuestionID IS NOT NULL";
                    int updateOrderResult = await this.ExecuteAsync(sql, new { id = message.OrderID, processingStatu = OrderStatus.Processing, status = OrderStatus.WaitingReply }, tran);
                    if (updateOrderResult > 0)
                    {
                        isFirstReply = true;
                        //添加一条订单扭转记录
                        OrderOpreateLog orderOpreateLog = new OrderOpreateLog()
                        {
                            Id = Guid.NewGuid(),
                            CreateTime = DateTime.Now,
                            OrderId = message.OrderID,
                            UpdateStatus = (int)OrderStatus.Processing,
                            Remark = "达人第一次回复，修改订单状态为进行中。",
                            UserId = message.SenderID,
                        };
                        await _orderOpreateLogRepository.AddAsync(orderOpreateLog, tran);
                    }
                    tran.Commit();
                    return (result, isFirstReply);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "用户发送消息数据操作异常。");
                    tran.Rollback();
                    return (false, false);
                }

            }

        }

        public IEnumerable<Message> GetChartRecord(Order order, Guid userID)
        {
            string where = @"
OrderID = @orderId
AND
IsValid = 1
AND
(SenderID = @userId OR ReceiveID = @userId)
";
            var list = this.GetBy(where
              , new { orderId = order.ID, userId = userID }
              , "CreateTime asc,Id asc ");
            return list;
        }

        public List<Message> GetUnReadMessage(Order order, Guid userID)
        {
            //没读过，读一下
            string where = @"
OrderID = @orderId
AND
IsValid = 1
AND
ReadTime is  null
AND
ReceiveID = @userId
";
            List<Message> unreadList = this.GetBy(where
              , new { orderId = order.ID, userId = userID }
              , "CreateTime asc,Id asc ").ToList();
            return unreadList;
        }

        public async Task<bool> UpdateMessageReadTime(IEnumerable<Message> unreadlist, IDbTransaction transaction = null)
        {
            bool isoutsidetran = transaction != null;
            if (!isoutsidetran)
            {
                transaction = this.BeginTransaction();
            }
            try
            {
                foreach (var message in unreadlist)
                {
                    message.ReadTime = DateTime.Now;
                    bool res = await this.UpdateAsync(message, transaction, new[] {
                            "ReadTime"
                        });
                    if (!res)
                    {
                        throw new Exception("批量更新消息阅读时间时发生异常。");
                    }
                }
                if (!isoutsidetran)
                {
                    transaction.Commit();
                }
                return true;
            }
            catch
            {
                if (!isoutsidetran)
                {
                    transaction.Rollback();
                }
                return false;
            }
        }

        public async Task<Message> GetOrderQuetion(Guid orderID)
        {
            string sql = @"SELECT * FROM Message 
WHERE
OrderID=@orderID
AND  MsgType=@msgType
ORDER BY CreateTime ASC
OFFSET 0 ROWS
FETCH NEXT 1 ROWS ONLY";
            return (await this._dBContext.QueryAsync<Message>(sql, new { orderID = orderID, msgType = MsgType.Question })).FirstOrDefault();

        }

        public async Task<IEnumerable<Message>> GetOrdersQuetion(IEnumerable<Guid> orderIDs)
        {
            string sql = @"SELECT * FROM Message 
                            WHERE
                            OrderID in @orderIDs
                            AND  MsgType=@msgType
                            ORDER BY CreateTime ASC";
            return await _dBContext.QueryAsync<Message>(sql, new { orderIDs, msgType = MsgType.Question });
        }

        public async Task<int> GetAskCount(Guid OrderID)
        {
            var sql = $"SELECT COUNT(1) from Message where OrderID=@OrderId and (MsgType={(int)MsgType.Question} Or MsgType={(int)MsgType.GoAsk})";
            var param = new { OrderID = OrderID };
            return await _dBContext.QuerySingleAsync<int>(sql, param);




        }

        /// <summary>
        /// 生成AnonyName
        /// </summary>
        /// <returns></returns>
        string GenerateAnonyName()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("匿名用户");
            char[] alphabet = new char[] {
            'a','b','c','d','e','f','g','h','i','j','k','l','n','m','o','p','q','x','w','u','v'
            };
            Random random = new Random();
            for (int i = 0; i < 2; i++)
            {
                sb.Append(alphabet[random.Next(0, alphabet.Length)]);
            }
            for (int i = 0; i < 5; i++)
            {
                sb.Append(random.Next(0, 9));
            }
            return sb.ToString();
        }
    }
}
