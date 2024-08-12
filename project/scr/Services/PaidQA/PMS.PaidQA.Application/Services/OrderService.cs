using MediatR;
using Microsoft.Extensions.Logging;
using PMS.MediatR.Events.PaidQA;
using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.EntityExtend;
using PMS.PaidQA.Domain.Enums;
using PMS.PaidQA.Domain.Message;
using PMS.PaidQA.Repository;
using PMS.UserManage.Application.IServices;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model.FinanceCenter;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using ProductManagement.Infrastructure.Toolibrary;
using Sxb.GenerateNo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using OrderStatus = PMS.PaidQA.Domain.Enums.OrderStatus;

namespace PMS.PaidQA.Application.Services
{

    /// <summary>
    /// 订单的功能枚举
    /// </summary>
    enum OrderFunc
    {
        TransitingOrder = 1,
        OverOrder = 2,
        /// <summary>
        /// 拒绝订单功能
        /// </summary>
        RefusOrder = 3,
        /// <summary>
        /// 拒绝转单功能
        /// </summary>
        RefusTransiting = 4,
    }
    public class OrderService : ApplicationService<Order>, IOrderService
    {
        //订单的缓存时间


        int orderCacheExpireMinute = 60;
        IOrderRepository _orderRepository;
        IMessageService _messageService;
        IUserService _userService;
        IEvaluateService _evaluateService;
        IFinanceCenterServiceClient _financeCenterServiceClient;
        IEasyRedisClient _easyRedisClient;
        IRegionTypeService _regionTypeService;
        IGradeService _gradeService;
        IMediator _mediator;
        ILogger _logger;
        ISxbGenerateNo _sxbGenerateNo;
        ICouponTakeService _couponTakeService;



        public int TotalAskCount { get; } = 10; //总提问机会
        public int ExpireTime { get; } = 24; //订单有效时间


        public OrderService(IOrderRepository orderRepository
            , IMessageService messageService
            , IUserService userService
            , IEvaluateService evaluateService
            , IFinanceCenterServiceClient financeCenterServiceClient
            , IEasyRedisClient easyRedisClient
            , IRegionTypeService regionTypeService
            , IGradeService gradeService
            , IMediator mediator
            , ILogger<OrderService> logger
            , ISxbGenerateNo sxbGenerateNo, ICouponTakeService couponTakeService) : base(orderRepository)
        {
            _gradeService = gradeService;
            _regionTypeService = regionTypeService;
            _evaluateService = evaluateService;
            _orderRepository = orderRepository;
            _messageService = messageService;
            _userService = userService;
            _financeCenterServiceClient = financeCenterServiceClient;
            _easyRedisClient = easyRedisClient;
            _mediator = mediator;
            _logger = logger;
            _sxbGenerateNo = sxbGenerateNo;
            _couponTakeService = couponTakeService;
        }
        /// <summary>
        /// 支付回调
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        public async Task<bool> PayCallBack(Guid orderid, Guid userid, int paystatus, string remark)
        {
            if ((byte)PayStatusEnum.Success == paystatus)
            {
                //支付成功的状态
                //检查该订单是否使用了优惠券
                var couponPreUseRecords = await _couponTakeService.GetPreUseRecordBy(orderid);
                if (couponPreUseRecords != null && couponPreUseRecords.Any())
                {
                    //销毁优惠券
                    bool cancelCoupon = await _couponTakeService.CancelCouponTake(couponPreUseRecords.First().CouponTakeId, userid, couponPreUseRecords.First().OrderId);
                    if (!cancelCoupon)
                    {
                        //销毁优惠券失败，认定为支付失败
                        return await PayFailureHandle(orderid, "销毁优惠券失败", paystatus);
                    }
                }
                return await PaySuccessHandle(orderid, userid, remark, paystatus);

            }
            else
            {
                return await _orderRepository.WechatPayCallUpdateOrder(orderid, OrderStatus.PayFaile, userid, "支付失败", paystatus);
            }


        }


        async Task<bool> PaySuccessHandle(Guid orderid, Guid userid, string remark, int paystatus)
        {
            //验证订单的合法性
            var canUpdate = _orderRepository.UpdateCheckLegal(orderid, (byte)OrderStatus.WaitingAsk);
            if (canUpdate)
            {
                bool paySuccess = await _orderRepository.WechatPayCallUpdateOrder(orderid, OrderStatus.WaitingAsk, userid, remark, paystatus);
                if (paySuccess)
                {
                    //await this.SetToCache(orderid);
                    await _mediator.Publish(new OrderChangeEvent(orderid));
                    //抛出订单支付成功事件
                    await _mediator.Publish(new PaySuccessEvent(orderid));
                }
                return paySuccess;
            }
            return false;
        }
        async Task<bool> PayFailureHandle(Guid orderId, string failRemark, int paystatus)
        {
            //todo：
            //更新订单状态为支付失败。
            Order order = await this.GetFromCache(orderId);
            order.Status = OrderStatus.PayFaile;
            order.UpdateTime = DateTime.Now;
            order.Remark = failRemark;
            bool res = await _orderRepository.UpdateAsync(order, default(Guid), null, new[] { "Status", "UpdateTime", "Remark" }, failRemark, paystatus);
            //退款。
            if (res)
            {
                //await this.SetToCache(orderId);
                await _mediator.Publish(new OrderChangeEvent(order.ID));
                await this.Refund(order, "支付处理失败退款。");
            }
            return res;
        }
        public async Task<(Order Order, bool isReCreate)> CreateAsync(Order order)
        {
            string lockKey = $"PaidCreateOrderLock_{order.AnswerID}_{order.CreatorID}";
            string lockVal = lockKey;
            TimeSpan maxHandleTimeSpan = TimeSpan.FromSeconds(10);
            (Order Order, bool IsReCreate) = (null, false);
            //加锁的目的是为了避免并发创建多张未支付订单。
            if ((await _easyRedisClient.LockTakeAsync(lockKey, lockVal, maxHandleTimeSpan)))
            {
                try
                {
                    Order unpayOrder = await this.GetUnPayOrder(order.CreatorID, order.AnswerID);
                    if (unpayOrder == null)
                    {
                        //创建新订单
                        if (await CreateAsync(order, null))
                        {
                            Order = order;
                            IsReCreate = false;
                        }
                    }
                    else
                    {
                        //用一张未支付的订单作为新订单，初衷是为了减少订单的新增量。
                        unpayOrder.Amount = order.Amount;
                        unpayOrder.UpdateTime = order.UpdateTime;
                        unpayOrder.CreateTime = order.CreateTime;
                        unpayOrder.PayAmount = order.PayAmount;
                        unpayOrder.OriginType = order.OriginType;
                        unpayOrder.ExpireTime = order.ExpireTime;
                        unpayOrder.EnableTransiting = order.EnableTransiting;
                        bool updateflag = await _orderRepository.UpdateAsync(unpayOrder, unpayOrder.CreatorID
                            , null
                            , new[] {
                         nameof(unpayOrder.Amount),
                         nameof(unpayOrder.UpdateTime),
                         nameof(unpayOrder.CreateTime),
                         nameof(unpayOrder.PayAmount),
                         nameof(unpayOrder.OriginType),
                         nameof(unpayOrder.ExpireTime),
                         nameof(unpayOrder.EnableTransiting)},
                         "重置未支付订单");
                        if (updateflag)
                        {
                            Order = unpayOrder;
                            IsReCreate = true;
                        }

                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "上学问-创建订单。");
                }
                await _easyRedisClient.LockReleaseAsync(lockKey, lockVal); //订单创建完时可以释放锁
            }
            return (Order, IsReCreate);

        }

        public async Task<IEnumerable<(Guid, Guid, string, string)>> GetQuestionerInfoByOrderIDs(IEnumerable<Guid> ids)
        {
            var str_Where = "ID in @ids";
            var creatorIDs = _orderRepository.GetBy(str_Where, new { ids }, fileds: new string[2] { "ID", "CreatorID" });
            if (creatorIDs?.Any() == true)
            {
                var finds = await _userService.GetUserInfos(creatorIDs.Select(p => p.CreatorID).Distinct());
                if (finds?.Any() == true)
                {
                    return await Task.Run(() =>
                    {
                        return finds.Select(p => (creatorIDs.FirstOrDefault(c => c.CreatorID == p.Id)?.ID ?? Guid.Empty, p.Id, p.NickName, p.HeadImgUrl));
                    });
                }
            }
            return new (Guid, Guid, string, string)[0];
        }

        public async Task<double> GetTalentInSixHourReplyPercent(Guid talentUserID)
        {
            if (talentUserID == Guid.Empty) return default;
            return await _orderRepository.GetSixHoursReplyPercentByTalentUserID(talentUserID);
        }

        public async Task<IEnumerable<Order>> ListByIDs(IEnumerable<Guid> ids)
        {
            var str_Where = "ID in @ids";
            return await Task.Run(() =>
            {
                return _orderRepository.GetBy(str_Where, new { ids }, fileds: new string[1] { "*" });
            });
        }

        public async Task<PageResultEx<OrderPageExtend>> Page(int pageIndex, int pageSize, Guid userID, IEnumerable<int> status, bool isAsk)
        {
            var offset = --pageIndex * pageSize;
            var str_OrderBy = $"UpdateTime desc OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY";
            var str_Where = $"status in @status AND IsBlocked != 1";
            if (isAsk)
            {
                str_Where += " AND CreatorID = @userID";
            }
            else
            {
                str_Where += " AND AnswerID = @userID";
            }
            var orderCount = await _orderRepository.Count(str_Where, new { userID, status });
            if (orderCount > 0)
            {
                var orders = _orderRepository.GetBy(str_Where, new { userID, status }, str_OrderBy, new string[1] { "*" });
                var transferNewOrderIDs = new Dictionary<Guid, Guid>();
                var transferOrderIDs = orders.Where(p => p.Status == OrderStatus.Transfered).Select(p => p.ID);
                if (transferOrderIDs?.Any() == true)
                {
                    await Task.Run(() =>
                    {
                        var finds = _orderRepository.GetBy("OrginAskID in @ids", new { ids = transferOrderIDs }, fileds: new string[2] { "OrginAskID", "ID" });
                        if (finds?.Any() == true)
                        {
                            foreach (var item in finds.Where(p => p.OrginAskID.HasValue))
                            {
                                if (!transferNewOrderIDs.ContainsKey(item.OrginAskID.Value))
                                {
                                    transferNewOrderIDs.Add(item.OrginAskID.Value, item.ID);
                                }
                            }
                        }
                    });
                }

                var questionContents = await _messageService.GetByOrders(orders.Select(p => p.ID).Distinct());
                var userInfos = await Task.Run(() =>
                {
                    if (isAsk)
                    {
                        return _userService.GetUserInfos(orders.Select(p => p.AnswerID).Distinct());
                    }
                    else
                    {
                        return _userService.GetUserInfos(orders.Select(p => p.CreatorID).Distinct());
                    }
                });
                //var unReadOrderIDs = await GetUnReadOrderIDs(userID);
                var comments = await _evaluateService.GetByOrderIDs(orders.Where(p => p.IsEvaluate).Select(p => p.ID));
                var result = new PageResultEx<OrderPageExtend>(pageIndex, pageSize, orderCount);
                //IEnumerable<Guid> timeOutFromWaitAskOrderIDs = new List<Guid>();
                //if (orders.Any(p => p.Status == OrderStatus.TimeOut))
                //{
                //    timeOutFromWaitAskOrderIDs = await _orderRepository.GetWaitAskToTimeOutOrderIDs(orders.Where(p => p.Status == OrderStatus.TimeOut).Select(p => p.ID));
                //}
                foreach (var item in orders)
                {
                    var order = CommonHelper.MapperProperty<Order, OrderPageExtend>(item);
                    if (questionContents.Any(p => p.OrderID == order.ID))
                    {
                        var content = questionContents.Where(p => p.OrderID == order.ID && p.MsgType == MsgType.Question)?.OrderBy(p => p.CreateTime)?.FirstOrDefault();
                        if (content != null)
                        {
                            order.QuestionContent = content.Content;
                            order.HasNew = questionContents.Any(p => p.OrderID == order.ID && p.ReceiveID == userID && !p.ReadTime.HasValue);
                        }
                    }

                    if (item.Status == OrderStatus.TimeOut && string.IsNullOrWhiteSpace(order.QuestionContent))
                    {
                        order.IsTimeOutFromWaitAsk = true;
                        if (isAsk)
                        {
                            order.QuestionContent = "您未提交问题。";
                        }
                        else
                        {
                            order.QuestionContent = "用户未提问。";
                        }
                    }

                    UserManage.Domain.Entities.UserInfo userInfo = null;
                    if (isAsk)
                    {
                        userInfo = userInfos.FirstOrDefault(p => p.Id == order.AnswerID);
                    }
                    else
                    {
                        userInfo = userInfos.FirstOrDefault(p => p.Id == order.CreatorID);
                    }
                    if (userInfo != null)
                    {
                        order.HeadImgUrl = userInfo.HeadImgUrl;
                        order.NickName = userInfo.NickName;
                    }

                    if (order.IsAnonymous.HasValue && order.IsAnonymous.Value && !isAsk)
                    {
                        order.HeadImgUrl = "https://cos.sxkid.com/images/head.png";
                        order.NickName = order.AnonyName;
                    }

                    if (!isAsk && (order.IsTimeOutFromWaitAsk || order.Status == OrderStatus.WaitingAsk))
                    {
                        order.HeadImgUrl = "https://cos.sxkid.com/images/head.png";
                        order.NickName = "上学帮用户";
                    }


                    if (order.IsEvaluate)
                    {
                        var comment = comments.FirstOrDefault(p => p.OrderID == order.ID);
                        if (comment != null)
                        {
                            order.Comment = comment;
                        }
                    }
                    //order.HasNew = unReadOrderIDs?.Any(p => p == order.ID) == true;

                    if (order.Status == OrderStatus.Transfered && transferNewOrderIDs.Any(p => p.Key == order.ID))
                    {
                        order.NewOrderID = transferNewOrderIDs[order.ID];
                    }

                    result.Items.Add(order);
                }
                return result;
            }
            else
            {
                return new PageResultEx<OrderPageExtend>();
            }
        }




        /// <summary>
        /// 从缓存中读取订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Order> GetFromCache(Guid id)
        {
            return await _easyRedisClient.GetOrAddAsync(GenerateOrderKey(id), async () =>
             {
                 Order order = await _orderRepository.GetAsync(id);
                 return order;
             }, TimeSpan.FromMinutes(orderCacheExpireMinute));
        }

        /// <summary>
        /// 将更新到订单缓存中
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public async Task<bool> SetToCache(Guid id)
        {
            try
            {
                Order order = await _orderRepository.GetAsync(id);
                if (order != null)
                {
                    return await _easyRedisClient.AddAsync(GenerateOrderKey(order.ID), order, TimeSpan.FromMinutes(orderCacheExpireMinute));

                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<Order> TransitingOrder(Order originOrder, Guid operatorID, TalentSetting targetTalent)
        {
            if (!CheckOperateOrderValidity(originOrder, operatorID, OrderFunc.TransitingOrder))
            {
                return null;
            }
            try
            {
                Order newOrder = new Order();
                newOrder.ID = Guid.NewGuid();
                newOrder.CreateTime = DateTime.Now;
                newOrder.UpdateTime = DateTime.Now;
                newOrder.NO = $"PAIDQA{_sxbGenerateNo.GetNumber()}"; //订单编号
                newOrder.AskRemainCount = TotalAskCount; //默认都是10次
                newOrder.Amount = targetTalent.Price;
                newOrder.AnswerID = targetTalent.TalentUserID;
                /*多退少不补说明:
                 * 如果转单后的订单金额多于或等于上一张订单的支付金额，那么属于少不补的情况，这时应该认为拿上一张订单支付金额来完全支付当前新订单。
                 * 否则，属于多退情况，这时应该退回上张订单支付金额比当前新订单所需金额多于出来的金额。
                */
                newOrder.PayAmount = Math.Min((decimal)originOrder.PayAmount, newOrder.Amount);
                newOrder.IsBlocked = false;
                var result = await _orderRepository.TransitingOrder(newOrder
                    , originOrder.ID
                    , targetTalent.TalentUserID
                    , operatorID
                    , "用户转单。");
                if (result.Successed)
                {
                    originOrder = result.OriginOrder;
                    newOrder = result.NewOrder;
                    decimal refundAmount = newOrder.PayAmount >= originOrder.PayAmount ? 0 : ((decimal)originOrder.PayAmount - (decimal)newOrder.PayAmount);
                    //更新订单缓存
                    //await SetToCache(originOrder.ID);
                    await _mediator.Publish(new OrderChangeEvent(originOrder.ID));
                    //更新订单缓存
                    //await SetToCache(newOrder.ID);
                    await _mediator.Publish(new OrderChangeEvent(newOrder.ID));
                    //抛出订单支付成功事件
                    await _mediator.Publish(new PaySuccessEvent(newOrder.ID));
                    //触发转单事件
                    await _mediator.Publish(new TransitingOrderEvent(originOrder, newOrder, refundAmount));
                    return newOrder;
                }
                else
                {

                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "转单发生异常。");
                return null;
            }


        }


        public IEnumerable<Order> GetWaitTransitingOrders()
        {
            int hour = 12;
            string strWhere = @"
Status = 2
AND DATEADD(HOUR,@hour,CreateTime) < GETDATE()
AND EnableTransiting = 1
";
            return this._orderRepository.GetBy(strWhere, new { hour }, "CreateTime ASC");
        }



        public async Task<IEnumerable<Guid>> GetSimilarTalentIDs(Guid talentID)
        {
            var regions = await _regionTypeService.GetByTalentUserID(talentID);
            var grades = await _gradeService.GetByTalentUserID(talentID);
            return null;
        }

        public async Task<bool> OverOrder(Order order, Guid operatorID, string remark)
        {
            order.Status = OrderStatus.Finish;
            order.UpdateTime = DateTime.Now;
            order.FinishTime = DateTime.Now;
            bool updateOrderResult = await _orderRepository.OverOrder(order, operatorID, remark);
            if (updateOrderResult)
            {
                //await this.SetToCache(order.ID);
                await _mediator.Publish(new OrderOverEvent(order));
                await _mediator.Publish(new OrderChangeEvent(order.ID));
            }
            return updateOrderResult;
        }

        public async Task<bool> RefusOrder(Order order, Guid operatorID)
        {
            if (!CheckOperateOrderValidity(order, operatorID, OrderFunc.RefusOrder))
            {
                return false;
            }
            try
            {
                order.Status = OrderStatus.Refused;
                order.UpdateTime = DateTime.Now;
                order.FinishTime = DateTime.Now;
                bool updateOrderResult = await _orderRepository.UpdateAsync(order
                    , operatorID
                    , null
                    , new[] { "Status", "UpdateTime", "FinishTime" }
                    , "拒绝订单。");
                if (updateOrderResult)
                {
                    //await this.SetToCache(order.ID);
                    await _mediator.Publish(new OrderChangeEvent(order.ID));
                    await _mediator.Publish(new RefusOrderEvent(order));
                }
                return updateOrderResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "拒绝订单操作异常。");
                return false;
            }
        }

        public async Task<bool> RefusTransiting(Message message, Guid operatorID)
        {
            Order order = _orderRepository.Get(message.OrderID);
            if (!CheckOperateOrderValidity(order, operatorID, OrderFunc.RefusTransiting))
            {
                return false;
            }

            if (order.CreatorID != operatorID)
            {
                return false;
            }
            order.UpdateTime = DateTime.Now;
            order.IsRefusTransiting = true;
            try
            {
                bool updateOrderResult = await _orderRepository.UpdateAsync(order
                , operatorID
                , null
                , new[] { "UpdateTime", "IsRefusTransiting" }
                , "拒绝转单");
                if (updateOrderResult)
                {
                    //await this.SetToCache(order.ID);
                    await _mediator.Publish(new OrderChangeEvent(order.ID));
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        public async Task<bool> UpdateAsync(Order entity, Guid operatorID, string[] fileds, string remark)
        {
            return await _orderRepository.UpdateAsync(entity, operatorID, null, fileds, remark);
        }

        public override async Task<bool> UpdateAsync(Order entity, string[] fileds)
        {
            return await _orderRepository.UpdateAsync(entity, entity.CreatorID, null, fileds, "普通更新订单");
        }

        public async Task<IEnumerable<Order>> GetValidOrders(Guid operatorID, Guid talentUserID)
        {
            string where = @"(Status=0 OR Status=1 OR Status=2 OR Status = 3) 
AND IsBlocked=0
AND CreatorID=@CreatorID
AND AnswerID=@AnswerID
";
            var orders = _orderRepository.GetBy(where, new { AnswerID = talentUserID, CreatorID = operatorID });
            List<Order> normalOrders = new List<Order>();
            foreach (var order in orders)
            {
                if (CheckOrderIsOverTime(order))
                {
                    //超时订单超时处理
                    await _mediator.Publish(new OverTimeOrderEvent(order));
                }
                else
                {
                    normalOrders.Add(order);
                }
            }

            return normalOrders;
        }


        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="order"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        async Task<bool> CreateAsync(Order order, IDbTransaction transaction)
        {
            order.ID = Guid.NewGuid();
            order.Status = OrderStatus.UnPay;//创建订单默认状态都是0
            order.CreateTime = DateTime.Now;
            order.UpdateTime = DateTime.Now;
            order.NO = $"PAIDQA{_sxbGenerateNo.GetNumber()}"; //订单编号
            order.AskRemainCount = TotalAskCount; //默认都是10次
            order.IsBlocked = false;
            bool result = await _orderRepository.AddAsync(order, transaction);
            if (result)
            {
                //await this.SetToCache(order.ID);
                await _mediator.Publish(new OrderChangeEvent(order.ID));

            }
            return result;
        }



        string GenerateOrderKey(Guid orderID)
        {
            string md5 = DesTool.Md5($"{orderID}");
            return string.Format("{0}-{1}", "order", md5);
        }

        string OrderCacheKey(Guid orderID)
        {
            return string.Format("orderstatu_{0}", orderID);
        }
        string OrderUpdateTimeCacheKey(Guid orderID)
        {
            return string.Format("orderupdateTime_{0}", orderID);
        }

        /// <summary>
        /// 检测操作人是否为订单的相关者
        /// </summary>
        /// <param name="order"></param>
        /// <param name="operatorID"></param>
        /// <returns></returns>
        bool IsOrderStakeholders(Order order, Guid operatorID)
        {
            if (operatorID == default(Guid))
            {
                //默认认为是系统
                return true;
            }
            if (order.CreatorID != operatorID && order.AnswerID != operatorID)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public IEnumerable<Order> GetOverTimeOrders()
        {
            string where = @"(Status=1 OR Status=2 OR Status = 3) 
AND IsBlocked=0
AND (DATEADD(HOUR,24,CreateTime)<GETDATE() or ExpireTime<GETDATE())";
            return _orderRepository.GetBy(where);

        }

        public IEnumerable<Order> GetOverTimeAnswerOrders()
        {
            int timeoutsecond = 39600;//11小时
            string where = @$"Status={(int)OrderStatus.WaitingReply} 
AND IsBlocked=0
AND DATEDIFF(SECOND, CreateTime,GETDATE()) > @timeoutsecond";
            return _orderRepository.GetBy(where, new { timeoutsecond });

        }


        public bool CheckOrderIsOverTime(Order order)
        {
            if (
                (order.Status == OrderStatus.WaitingAsk
                || order.Status == OrderStatus.WaitingReply
                || order.Status == OrderStatus.Processing)
                && !order.IsBlocked.GetValueOrDefault()
                && (DateTime.Now - order.CreateTime).TotalHours >= 24
               )
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public bool CheckOrderCanOperate(Order order)
        {

            if (
                (order.Status != OrderStatus.WaitingAsk
                && order.Status != OrderStatus.WaitingReply
                && order.Status != OrderStatus.Processing)
               )
            {
                //不是这些订单的状态是不能操作的
                return false;
            }
            if (order.IsBlocked.GetValueOrDefault())
            {
                //系统屏蔽的订单也是不能操作的
                return false;
            }
            if ((DateTime.Now - order.CreateTime).TotalHours >= 24)
            {
                //订单创建时间超过24小时也是不能操作的
                return false;
            }
            return true;
        }



        public Task<IEnumerable<Guid>> GetUnReadOrderIDs(Guid userID, OrderStatus? status = null)
        {
            if (userID == Guid.Empty) return null;
            return _orderRepository.GetUnReadOrderIDs(userID, status);
        }




        /// <summary>
        /// 校验订单合法性
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        bool CheckOperateOrderValidity(Order order, Guid operatorID, OrderFunc orderFunc)
        {
            //1.订单一定是不能超时的
            if (CheckOrderIsOverTime(order))
            {
                return false;
            }
            //2.拥有操作权限
            if (!CheckCanOperate(order, operatorID, orderFunc))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检查操作人对该订单对应的功能能否进行操作。
        /// </summary>
        /// <param name="order"></param>
        /// <param name="operatorID"></param>
        /// <param name="orderFunc"></param>
        /// <returns></returns>
        bool CheckCanOperate(Order order, Guid operatorID, OrderFunc orderFunc)
        {
            if (operatorID == default(Guid))
            {
                //默认GUID认为是系统调用，无需校验权限
                return true;
            }
            switch (orderFunc)
            {
                case OrderFunc.TransitingOrder:
                    if (order.CreatorID != operatorID)
                    {
                        ///不是创建者不能操作
                        return false;
                    }
                    if (order.Status != OrderStatus.WaitingReply)
                    {
                        //不是等待回复的订单不能操作
                        return false;
                    }
                    break;
                case OrderFunc.OverOrder:
                    if (order.AnswerID != operatorID && order.CreatorID != operatorID)
                    {
                        return false;
                    }
                    if (order.Status != PMS.PaidQA.Domain.Enums.OrderStatus.Processing)
                    {
                        return false;
                    }
                    if (order.AnswerID == operatorID && order.AskRemainCount > 0)
                    {
                        return false;
                    }
                    break;
                case OrderFunc.RefusOrder:
                    if (order.AnswerID != operatorID)
                    {
                        return false;
                    }
                    if (order.Status != OrderStatus.WaitingReply)
                    {
                        return false;
                    }
                    break;
                case OrderFunc.RefusTransiting:
                    if (order.CreatorID != operatorID)
                    {
                        return false;
                    }
                    if (order.Status != OrderStatus.WaitingReply)
                    {
                        return false;
                    }
                    break;
                default:
                    break;
            }
            return true;
        }

        public async Task<string> GetPayOpenID(Guid userId)
        {
            return await _orderRepository.GetPayOpenID(userId);
        }

        public async Task<IEnumerable<Guid>> CheckIsAskOrderIDs(IEnumerable<Guid> orderIDs, Guid talentUserID)
        {
            if (orderIDs?.Any() == true && talentUserID != Guid.Empty)
            {
                return await _orderRepository.GetIsAskOrderIDs(orderIDs, talentUserID);
            }
            return null;
        }


        /// <summary>
        /// 部分退款
        /// </summary>
        /// <param name="order"></param>
        /// <param name="ammount"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public async Task<bool> RefundPart(Order order,decimal ammount, string reason)
        {
            if (order.PayAmount < ammount) {
                return false;
            }
            if (ammount <= 0) {
                return false;
            }
            try
            {

                Order payOrder = order;
                if (order.OrginAskID != null)
                {
                    payOrder = await _orderRepository.GetPayOrderBy(order.ID);
                }
                //发起退款操作（支付中心）
                var refundResult = await _financeCenterServiceClient.Refund(new RefundRequest()
                {
                    OrderId = payOrder.ID,
                    RefundAmount = ammount, 
                    Remark = reason
                });
                if (refundResult.succeed)
                {
                    //触发退款成功事件
                    await _mediator.Publish(new OrderRefundEvent(order));
                }
                else
                {
                    //记录日志
                    _logger.LogWarning($"调用支付中心退款接口返回失败状态：msg={refundResult.msg}");
                }
                return refundResult.succeed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "订单退款异常。");
                return false;
            }

        }


        /// <summary>
        /// 全额退款 
        /// </summary>
        /// <param name="order"></param>
        /// <param name="reason"></param>
        /// <returns></returns>

        public async Task<bool> Refund(Order order, string reason)
        {

            try
            {

                Order payOrder = order;
                //转单的订单需要特别处理,找出真正支付那张订单退款。
                if (order.OrginAskID != null)
                {
                    payOrder = await _orderRepository.GetPayOrderBy(order.ID);
                }
                //发起退款操作（支付中心）
                var refundResult = await _financeCenterServiceClient.Refund(new RefundRequest()
                {
                    OrderId = payOrder.ID,//订单ID需要使用真正支付的那张订单ID
                    RefundAmount =  payOrder.PayAmount.GetValueOrDefault(), //退款金额要使用实际支付金额。
                    Remark = reason
                });
                if (refundResult.succeed)
                {
                    //触发退款成功事件
                    await _mediator.Publish(new OrderRefundEvent(order));
                }
                else
                {
                    //记录日志
                    _logger.LogWarning($"调用支付中心退款接口返回失败状态：msg={refundResult.msg}");
                }
                return refundResult.succeed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "订单退款异常。");
                return false;
            }

        }

        public async Task<bool> TimeOutCloseOrder(Order order, Guid operatorID, string remark)
        {
            bool result = await _orderRepository.TimOutCloseOrder(order, operatorID, remark);
            if (result)
            {
                //发送客服消息给专家
                CustomMessage customMessageTalent = new CustomMessage() { Content = "您未在规定时间内对用户的问题进行答复，系统已自动取消这笔订单并将金额退还给用户。" };
                await _messageService.SendMessage(customMessageTalent.CreateMessage(default(Guid), order.AnswerID, order.ID, MsgType.System)
                    , customMessageTalent
                    );
                //发送客服消息给用户
                CustomMessage customMessageUser = new CustomMessage() { Content = "专家未在规定时间内对您的问题进行答复，我们将为您推荐其他专家解决您的问题。" };
                await _messageService.SendMessage(customMessageUser.CreateMessage(default(Guid), order.CreatorID, order.ID, MsgType.System)
                    , customMessageUser
                    );
                //结束消息
                SystemStatuMessage overMsg = new SystemStatuMessage()
                {
                    Content = "本次咨询已结束"
                };
                Message sendToUser = overMsg.CreateMessage(default(Guid), order.CreatorID, order.ID, MsgType.System);
                Message sendToTalent = overMsg.CreateMessage(default(Guid), order.AnswerID, order.ID, MsgType.System);
                await _messageService.SendMessage(sendToUser, overMsg);
                await _messageService.SendMessage(sendToTalent, overMsg);
                //退款
                await this.Refund(order, "上学问订单超时发起退款。");
                //退券
                await _couponTakeService.BackCoupon(order.ID);

                await _mediator.Publish(new OverTimeOrderEvent(order));
                await _mediator.Publish(new OrderChangeEvent(order.ID));
            }

            return result;
        }

        public int GetHasAskCount(Order order)
        {
            return TotalAskCount - order.AskRemainCount;
        }

        public async Task<IEnumerable<Order>> GetBy(Guid userID)
        {
            return await _orderRepository.GetByAsync(" CreatorID=@CreatorID ", new { CreatorID = userID });
        }

        public bool IsFirstAsk(Order order)
        {
            return order.AskRemainCount == (TotalAskCount - 1);
        }

        public bool IsLastAsk(Order order)
        {
            return order.AskRemainCount == 0;
        }

        public async Task<Order> GetPayOrderBy(Guid transitingOrderID)
        {
            return await _orderRepository.GetPayOrderBy(transitingOrderID);
        }

        public async Task<Order> GetUnPayOrder(Guid userID, Guid talentUserID)
        {
            var result = await _orderRepository.GetByAsync("CreatorID=@CreatorID AND AnswerID=@AnswerID AND IsBlocked=0 AND  Status=0", new
            {
                CreatorID = userID,
                AnswerID = talentUserID
            });

            return result.FirstOrDefault();

        }



        public async Task<bool> IsHasProcessingOrder(Guid answerId)
        {
            var res = await _orderRepository.IsHasProcessingOrder(answerId);
            return res;
        }

        public async Task<IEnumerable<OrderTag>> GetOrderTags(Guid orderID)
        {
            if (orderID == Guid.Empty) return null;
            return await _orderRepository.GetTagsByID(orderID);
        }

        public async Task<IEnumerable<Order>> GetCreatorHasNewMsgOrders(int timeout_h = 5)
        {
            string where = @" Status IN (2,3) --有效订单
AND EXISTS (SELECT 1 FROM Message WHERE
Message.OrderID = [ORDER].ID
AND Message.SenderID = [Order].AnswerId
AND  IsValid = 1 
AND ReadTime IS  NULL 
AND DATEADD(HOUR,@timeout_h,CreateTime) <=GETDATE()) --超过5小时的未读消息
";
            return await _orderRepository.GetByAsync(where, new { timeout_h }, "CreateTime asc");
        }

        public async Task<IEnumerable<Order>> GetRemainHourOrders(int hours)
        {
            string where = @"Status in (2,3)
AND DATEADD(HOUR, @hours, CreateTime) <=GETDATE()";
            return await _orderRepository.GetByAsync(where, new { hours = ExpireTime - hours }, "CreateTime asc");
        }

        public async Task<IEnumerable<Order>> GetByTime(DateTime startTime, DateTime endTime)
        {
            return await _orderRepository.GetByAsync("CreateTime >= @startTime AND CreateTime < @endTime", new { startTime, endTime });
        }

        public async Task<bool> ExistsEffectiveOrder(Guid talentUserId, Guid userId)
        {
            return await _orderRepository.ExistsEffectiveOrder(talentUserId, userId);
        }

        public async Task<bool> TimeOutOverOrder(Order order, Guid operatorID, string remark)
        {
            SystemStatuMessage timOutOverMsg = new SystemStatuMessage()
            {
                Content = "咨询时间已到"
            };
            Message sendToUser = timOutOverMsg.CreateMessage(default(Guid), order.CreatorID, order.ID, MsgType.System);
            Message sendToTalent = timOutOverMsg.CreateMessage(default(Guid), order.AnswerID, order.ID, MsgType.System);
            await _messageService.SendMessage(sendToUser, timOutOverMsg);
            await _messageService.SendMessage(sendToTalent, timOutOverMsg);
            await _mediator.Publish(new OrderChangeEvent(order.ID));
            bool isOK = await this.OverOrder(order, operatorID, remark);
            if (isOK)
            {

            }
            return isOK;


        }

        public async Task<bool> ExistsOrderAsync(Guid userId)
        {
            return await _orderRepository.ExistsOrderAsync(userId);
        }

        public async Task<IEnumerable<Order>> GetWaitAskAndTimeOutOrders(int timeOutHour = 12)
        {
            var orders = await _orderRepository.GetByAsync("[Status] =1 AND GETDATE()>DATEADD(HOUR,@timeOutHour,CreateTime)", new { timeOutHour }, "CreateTime asc");
            return orders;
        }

        public async Task<IEnumerable<Order>> GetWaitReplyAndTimeOutOrders(int timeOutHour = 12)
        {
            var orders = await _orderRepository.GetByAsync("[Status] =2 AND GETDATE()>DATEADD(HOUR,@timeOutHour,CreateTime)", new { timeOutHour }, "CreateTime asc");
            return orders;

        }
    }
}
