using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PMS.MediatR.Events.PaidQA;
using PMS.MediatR.Request.PaidQA;
using PMS.PaidQA.Application.Services;
using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.Enums;
using PMS.PaidQA.Domain.Message;
using PMS.UserManage.Application.IServices;
using ProductManagement.API.SMS;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Infrastructure.Configs;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IMessageService = PMS.PaidQA.Application.Services.IMessageService;

namespace Sxb.Web.Areas.PaidQA.Controllers
{

    /// <summary>
    /// 供给外部定时器触发
    /// </summary>
    [Route("PaidQA/[controller]/[action]")]
    [ApiController]
    public class TimerController : ControllerBase
    {

        IOrderService _orderService;
        IUserService _userService;
        IMediator _mediator;
        IEasyRedisClient _redis;
        ITencentSmsService _tencentSmsService;
        IMessageService _messageService;
        PaidQAOption _paidQAOption;
        ILogger _logger;
        INotificationService _notificationService;
        ICouponTakeService _couponTakeService;
        public TimerController(
            ILogger<TimerController> logger,
            IOptions<PaidQAOption> paidQAOption,
            IMediator mediator, IOrderService orderService, IUserService userService, IEasyRedisClient redis, ITencentSmsService tencentSmsService, IMessageService messageService, INotificationService notificationService, ICouponTakeService couponTakeService)
        {
            _paidQAOption = paidQAOption.Value;
            _mediator = mediator;
            _orderService = orderService;
            _userService = userService;
            _redis = redis;
            _tencentSmsService = tencentSmsService;
            _logger = logger;
            _messageService = messageService;
            _notificationService = notificationService;
            _couponTakeService = couponTakeService;
        }

        /// <summary>
        /// 触发自动评价处理
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseResult> TriggerAutoEvaluationTalentEvent()
        {
            //问题：目前如果异步后台执行，直接返回结果，Sqlconnection生命周期会提前结束。所以目前解决方案还是要使用await
            await _mediator.Publish(new AutoEvaluationTalentEvent());
            return ResponseResult.Success("触发成功");
        }

        /// <summary>
        /// 触发发送推荐专家处理
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseResult> TriggerSendRecommandCardEvent()
        {
            //问题：目前如果异步后台执行，直接返回结果，Sqlconnection生命周期会提前结束。所以目前解决方案还是要使用await

            var orders = _orderService.GetWaitTransitingOrders();
            foreach (var order in orders)
            {
                try
                {
                    string cacheKey = $"SendRecommandCardEvent_Lock:{order.ID}";
                    bool notexists = !(await _redis.ExistsAsync(cacheKey));
                    if (notexists)
                    {
                        TimeSpan lock_ts = TimeSpan.FromHours(24); //默认锁24小时
                        if (order.ExpireTime != null)
                        {
                            //expireTime是新加属性，所以要做一下空判断兼容。
                            lock_ts = order.ExpireTime.Value - DateTime.Now;
                        }
                        await _mediator.Publish(new SendRecommandCardEvent(order));

                        await _redis.AddStringAsync(cacheKey, "1", lock_ts); //发过后锁住该订单。
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"SendRecommandCardEvent_Lock,OrderId:{order?.ID}");
                    continue;
                }
            }

            return ResponseResult.Success("触发成功");
        }

        /// <summary>
        /// 触发扫描超时订单处理
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseResult> TriggerOverTimeOrderEvent()
        {

            try
            {
                await Task.Run(async () =>
                {
                    //获取超时订单
                    var orders = _orderService.GetOverTimeOrders();
                    foreach (var order in orders)
                    {
                        string cacheKey = $"TimeOut_24H_OrderEvent_Lock:{order.ID}";
                        try
                        {

                            bool notexists = !(await _redis.ExistsAsync(cacheKey));
                            if (notexists)
                            {
                                TimeSpan lock_ts = TimeSpan.FromMinutes(5); //5分钟内只能执行一次。
                                //问题：目前如果异步后台执行，直接返回结果，Sqlconnection生命周期会提前结束。所以目前解决方案还是要使用await
                                await _redis.AddStringAsync(cacheKey, "1", lock_ts); //发过后锁住该订单。

                                bool res = true;
                                if (order.Status == OrderStatus.Processing)
                                {
                                    res = await _orderService.TimeOutOverOrder(order, default(Guid), "系统检测订单超时，自动结束订单");

                                }
                                else
                                {
                                    res = await _orderService.TimeOutCloseOrder(order, default(Guid), "系统检测订单超时,自动关闭该订单。");
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"TimeOut_24H_OrderEvent_Lock,OrderId:{order?.ID}");
                            await _redis.RemoveAsync(cacheKey);
                            continue;
                        }
                    }
                });
                return ResponseResult.Success("处理成功。");
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed($"处理失败，{ex.Message}。");
            }
        }
        /// <summary>
        /// 扫描超11小时未回答订单
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseResult> TriggerOverTimeAnswerOrderEvent()
        {

            try
            {
                await Task.Run(async () =>
                {
                    var orders = _orderService.GetOverTimeAnswerOrders();
                    foreach (var order in orders)
                    {
                        //防止重复提醒
                        var done = await _redis.ExistsAsync($"TriggerOverTimeAnswerOrderEvent_orderid{order.ID}");
                        if (!done)
                        {
                            if (!_userService.TryGetOpenId(order.AnswerID, "fwh", out string reciverOpenID)) continue;
                            var senderInfo = _userService.GetUserInfo(order.CreatorID);
                            var daren = _userService.GetUserInfo(order.AnswerID);
                            if (null != senderInfo && null != daren)
                            {
                                if (order.IsAnonymous == true) { senderInfo.NickName = order.AnonyName; }
                                var cmd = new AskWechatTemplateSendRequest()
                                {
                                    user_nickname = senderInfo.NickName,
                                    daren_nickname = daren?.NickName,
                                    keyword1 = $"[{senderInfo.NickName}]的上学问",
                                    keyword2 = $"超过11小时未回复",
                                    openid = reciverOpenID,
                                    remark = "点击【查看详情】马上回复",
                                    msgtype = WechatMessageType.问答超过时未回复,
                                    OrderID = order.ID
                                };
                                await _mediator.Send(cmd);
                            }

                            await _redis.AddAsync($"TriggerOverTimeAnswerOrderEvent_orderid{order.ID}", 1, TimeSpan.FromHours(24));
                        }

                        //问题：目前如果异步后台执行，直接返回结果，Sqlconnection生命周期会提前结束。所以目前解决方案还是要使用await

                    }
                });
                return ResponseResult.Success("处理成功。");
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed($"处理失败，{ex.Message}。");
            }
        }



        public async Task<ResponseResult> TriggerAskerNewsPushMsg()
        {
            int timeout_h = 5; //5小时内只发一次。
            var orders = await _orderService.GetCreatorHasNewMsgOrders(timeout_h);
            foreach (var order in orders)
            {

                try
                {
                    string cacheKey = $"TriggerAskerNewsPushMsg_Lock:{order.ID}";
                    bool notexists = !(await _redis.ExistsAsync(cacheKey));
                    if (notexists)
                    {
                        if (!_userService.CheckIsSubscribeFwh(order.CreatorID, "fwh"))
                        {
                            var creatorInfo = _userService.GetUserInfo(order.CreatorID);
                            var answerInfo = _userService.GetUserInfo(order.AnswerID);
                            var tplparams = _paidQAOption.MobileMsgTplSetting.SendCreatorHasNewsNotify.tplParams.Select(p => p.Replace("{nickName}", answerInfo.NickName)).ToArray();
                            await _tencentSmsService.SendSmsAsync($"+{creatorInfo.NationCode ?? 86}{creatorInfo.Mobile}"
                               , _paidQAOption.MobileMsgTplSetting.SendCreatorHasNewsNotify.tplid
                               , tplparams
                               , App.Push);
                        }
                        await _redis.AddStringAsync(cacheKey, "1", TimeSpan.FromHours(timeout_h)); //发过后锁住该订单。
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"TriggerAskerNewsPushMsg,OrderId:{order?.ID}");
                    continue;
                }
            }
            return ResponseResult.Success("OK");
        }


        public async Task<ResponseResult> TriggerScanRemainOneHourOrders()
        {

            var orders = await _orderService.GetRemainHourOrders(1);
            foreach (var order in orders)
            {
                try
                {
                    string cacheKey = $"ScanRemainOneHourOrder_Lock:{order.ID}";
                    bool notexists = !(await _redis.ExistsAsync(cacheKey));
                    bool notSubscribe = !_userService.CheckIsSubscribeFwh(order.CreatorID, "fwh");
                    bool hasRemainCount = order.AskRemainCount > 0;
                    if (notexists && notSubscribe && hasRemainCount)
                    {
                        var creatorInfo = _userService.GetUserInfo(order.CreatorID);
                        var answerInfo = _userService.GetUserInfo(order.AnswerID);
                        var tplparams = _paidQAOption.MobileMsgTplSetting.RemainOnHoursNotify.tplParams
                            .Select(p =>
                                p
                                .Replace("{nickName}", answerInfo.NickName)
                                .Replace("{overTime}", order.OverTime.ToString("yyyy年MM月dd日 HH:mm:ss"))
                                .Replace("{remainCount}", order.AskRemainCount.ToString())
                                )
                            .ToArray();
                        await _tencentSmsService.SendSmsAsync($"+{creatorInfo.NationCode ?? 86}{creatorInfo.Mobile}"
                           , _paidQAOption.MobileMsgTplSetting.RemainOnHoursNotify.tplid
                           , tplparams
                           , App.Push);

                        await _redis.AddStringAsync(cacheKey, "1", TimeSpan.FromHours(24)); //发过后锁住该订单。
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"TriggerScanRemainOneHourOrders,OrderId:{order?.ID}");
                    continue;
                }
            }
            return ResponseResult.Success("OK");
        }

        public async Task<ResponseResult> NotifiOrderWaitAskAndTimeOut()
        {
            var orders = await _orderService.GetWaitAskAndTimeOutOrders();
            foreach (var order in orders)
            {
                DateTime now = DateTime.Now;
                string lockKey = $"NotifiOrderWaitAskAndTimeOut:{order.ID}";
                string lockVal = order.ID.ToString();
                if (now < order.ExpireTime)
                {
                    try
                    {
                        if (await _redis.LockTakeAsync(lockKey, lockVal, order.ExpireTime.Value - now))
                        {

                            await _notificationService.NotifiUserAsk(order.ID);
                        }
                    }
                    catch (Exception ex)
                    {
                        await _redis.LockReleaseAsync(lockKey, lockVal);
                    }
                }

            }
            return ResponseResult.Success(orders);
        }

        public async Task<ResponseResult> NotifiReply()
        {
            var orders = await _orderService.GetWaitReplyAndTimeOutOrders();
            foreach (var order in orders)
            {
                DateTime now = DateTime.Now;
                string lockKey = $"NotifiReply:{order.ID}";
                string lockVal = order.ID.ToString();
                if (now < order.ExpireTime)
                {
                    try
                    {
                        if (await _redis.LockTakeAsync(lockKey, lockVal, order.ExpireTime.Value - now))
                        {

                            await _notificationService.NotifiReply(order.ID);
                        }
                    }
                    catch (Exception ex)
                    {
                        await _redis.LockReleaseAsync(lockKey, lockVal);
                    }
                }

            }
            return ResponseResult.Success(orders);
        }


        public async Task<ResponseResult> ExpireCouponNotifiy()
        {
            var couponTakes = await _couponTakeService.GetWillExpireCoupons(new List<Guid>() { Guid.Parse("9208898D-8DF0-45E5-834E-667B072F3EF9") });

            foreach (var couponTake in couponTakes)
            {

                string lockKey = $"ExpireCouponNotifiy:{couponTake.Id}";
                string lockVal = couponTake.Id.ToString();
                try
                {
                    if (await _redis.LockTakeAsync(lockKey, lockVal, TimeSpan.FromHours(24)))
                    {

                        await _notificationService.NotifiExpireCoupon(couponTake.Id);
                        //后续更新通知状态
                    }
                }
                catch (Exception ex)
                {
                    await _redis.LockReleaseAsync(lockKey, lockVal);
                }
            }
            return ResponseResult.Success(couponTakes);
        }

    }
}
