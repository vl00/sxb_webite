using AutoMapper;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PMS.MediatR.Events.PaidQA;
using PMS.PaidQA.Application.Services;
using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.EntityExtend;
using PMS.PaidQA.Domain.Enums;
using PMS.PaidQA.Domain.Message;
using PMS.PaidQA.Repository;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Entities;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model.FinanceCenter;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using ProductManagement.Tool.Email;
using Sxb.Web.Areas.Common.Models;
using Sxb.Web.Areas.Coupon.Models.Coupon;
using Sxb.Web.Areas.PaidQA.Models.Evaluate;
using Sxb.Web.Areas.PaidQA.Models.Order;
using Sxb.Web.Controllers;
using Sxb.Web.Filters;
using Sxb.Web.Response;
using Sxb.Web.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IMessageService = PMS.PaidQA.Application.Services.IMessageService;
using Message = PMS.PaidQA.Domain.Entities.Message;

namespace Sxb.Web.Areas.PaidQA.Controllers
{

    [Route("PaidQA/[controller]/[action]")]
    [ApiController]
    public class OrderController : ApiBaseController
    {


        public static decimal minPayAmount = 0.01m;

        IOrderService _orderService;
        IMessageService _messageService;
        ITalentSettingService _talentSettingService;
        IFinanceCenterServiceClient _financeCenterServiceClient;
        IMapper _mapper;
        IUserService _userService;
        IEvaluateService _evaluateService;
        IWebHostEnvironment _webHostEnvironment;
        IMediator _mediator;
        IEvaluateTagsService _evaluateTagsService;
        ISxbDarenEmailRepository _darenEmailRepo;
        ICouponTakeService _couponTakeService;
        IWeChatAppClient _weChatAppClient;
        ILogger _logger;

        private readonly IEasyRedisClient _easyRedisClient;
        IEmailClient _emailClient;
        HttpClient _asRebackHttpClient;

        public OrderController(IUserService userService, IOrderService orderService
            , ITalentSettingService talentSettingService
            , IMapper mapper
            , IFinanceCenterServiceClient financeCenterServiceClient
            , IEvaluateService evaluateService
            , IMessageService messageService, IEasyRedisClient easyRedisClient
            , IWebHostEnvironment webHostEnvironment
            , IMediator mediator
            , IEvaluateTagsService evaluateTagsService
            , ISxbDarenEmailRepository darenEmailRepo
            , IEmailClient emailClient
            , ICouponTakeService couponTakeService
            , IHttpClientFactory httpClientFactory
            , IWeChatAppClient weChatAppClient
            , ILogger<OrderController> logger)
        {
            _darenEmailRepo = darenEmailRepo;
            _orderService = orderService;
            _talentSettingService = talentSettingService;
            _userService = userService;
            _mapper = mapper;
            _financeCenterServiceClient = financeCenterServiceClient;
            _evaluateService = evaluateService;
            _messageService = messageService;
            _webHostEnvironment = webHostEnvironment;
            _mediator = mediator;
            _evaluateTagsService = evaluateTagsService;

            _easyRedisClient = easyRedisClient;
            _emailClient = emailClient;
            _couponTakeService = couponTakeService;
            _asRebackHttpClient = httpClientFactory.CreateClient("ADReBack");
            _weChatAppClient = weChatAppClient;
            _logger = logger;
        }



        [Description("获取订单详情信息")]
        [HttpGet]
        public ResponseResult<GetOrderResult> GetOrder([FromQuery] GetOrderRequest request)
        {
            Order order = _orderService.Get(request.OrderID);
            if (order.CreatorID != UserIdOrDefault && order.AnswerID != UserIdOrDefault)
            {
                return ResponseResult<GetOrderResult>.Failed("只有订单相关者才能查看订单详情。");
            }
            GetOrderResult result = new GetOrderResult();
            result.OrderInfo = _mapper.Map<OrderInfoResult>(order);
            return ResponseResult<GetOrderResult>.Success(result, "");
        }

        [HttpGet]
        [Description("轮询订单状态")]
        public async Task<ResponseResult<GetOrderResult>> GetOrderStatu([FromQuery] GetOrderRequest request)
        {
            return await PollingTool<ResponseResult<GetOrderResult>>.StartPolling(async (pool) =>
            {
                if (pool.Count > 1)
                {
                    Order order = await _orderService.GetFromCache(request.OrderID);
                    if (_orderService.CheckOrderIsOverTime(order))
                    {
                        //触发订单超时事件
                        await _mediator.Publish(new OverTimeOrderEvent(order));
                        //更新一下当前订单
                        order = await _orderService.GetFromCache(request.OrderID);
                    }
                    GetOrderResult result = new GetOrderResult();
                    result.OrderInfo = _mapper.Map<OrderInfoResult>(order);
                    if (order.IsEvaluate)
                    {
                        //获取评价信息
                        var evalute = (await _evaluateService.GetByOrderIDs(new List<Guid>() { order.ID })).FirstOrDefault();
                        result.EvaluateInfo = _mapper.Map<EvaluateResult>(evalute);
                        var evaluteTags = await _evaluateTagsService.GetByEvaluateIDs(new List<Guid> { evalute.ID });
                        if (evaluteTags != null && evaluteTags.Any())
                        {
                            result.EvaluateInfo.Tags = evaluteTags.First().Value?.Select(t => t.Name).ToList();
                        }

                    }
                    pool.Data = ResponseResult<GetOrderResult>.Success(result, "");
                    pool.Stop = true;
                }
                return pool;
            }, 2);
        }


        [HttpGet]
        [Description("结束订单")]
        [Authorize]
        public async Task<ResponseResult<OverOrderResult>> OverOrder([FromQuery] OverOrderRequest request)
        {
            Order order = _orderService.Get(request.OrderID);
            if (UserIdOrDefault != order.CreatorID && UserIdOrDefault != order.AnswerID)
            {
                return ResponseResult<OverOrderResult>.Failed("你不是该订单的相关者。");
            }
            bool updateOrderResult = await _orderService.OverOrder(order, UserIdOrDefault, $"订单相关人选择结束订单。");
            if (updateOrderResult)
            {
                OverOrderResult result = new OverOrderResult()
                {
                    OrderInfo = _mapper.Map<OrderInfoResult>(order)
                };
                return ResponseResult<OverOrderResult>.Success(result, "");
            }
            else
            {
                return ResponseResult<OverOrderResult>.Failed("操作失败。");
            }



        }

        [HttpGet]
        [Description("拒绝订单")]
        [Authorize]
        public async Task<ResponseResult<RefusOrderResult>> RefusOrder([FromQuery] RefusOrderRequest request)
        {
            Order order = _orderService.Get(request.OrderID);
            bool updateOrderResult = await _orderService.RefusOrder(order, UserIdOrDefault);
            if (updateOrderResult)
            {
                RefusOrderResult result = new RefusOrderResult()
                {
                    OrderInfo = _mapper.Map<OrderInfoResult>(order)
                };
                return ResponseResult<RefusOrderResult>.Success(result, "");
            }
            else
            {
                return ResponseResult<RefusOrderResult>.Failed("操作失败。");
            }


        }

        [HttpGet]
        [Description("拒绝转单")]
        [Authorize]
        public async Task<ResponseResult<RefusTransitingResult>> RefusTransiting([FromQuery] RefusTransitingRequest request)
        {
            Message message = _messageService.Get(request.MsgID);
            bool updateOrderResult = await _orderService.RefusTransiting(message, UserIdOrDefault);
            if (updateOrderResult)
            {
                //修改信息卡片里的Status为拒绝转单。
                var recommandCardMsg = PaidQAMessage.Create<RecommandCardMessage>(message);
                recommandCardMsg.Status = RecommandCardMessageStatu.Refus; //true为拒绝转单，默认是false
                message.Content = recommandCardMsg.Serialize();
                bool isSuccess = await _messageService.UpdateAsync(message, new[] { nameof(message.Content) });
                if (isSuccess)
                {
                    RefusTransitingResult result = new RefusTransitingResult()
                    {
                        Message = message
                    };
                    //触发用户拒绝转单事件
                    await _mediator.Publish(new RefusTransitingOrderEvent(message.OrderID.GetValueOrDefault()));
                    return ResponseResult<RefusTransitingResult>.Success(result, "");
                }

            }
            return ResponseResult<RefusTransitingResult>.Failed("操作失败。");


        }

        [HttpPost]
        [Description("流转订单")]
        [Authorize]
        public async Task<ResponseResult<TransitingOrderResult>> TransitingOrder([FromBody] TransitingOrderRequest request)
        {
            Order validOrder = (await _orderService.GetValidOrders(UserIdOrDefault, request.TargetAnwserID)).FirstOrDefault();
            if (validOrder != null)
            {
                return ResponseResult<TransitingOrderResult>.Failed("您与该专家已存在一张有效订单。");
            }
            Order order = _orderService.Get(request.OriginOrderID);
            if (!order.EnableTransiting)
            {
                return ResponseResult<TransitingOrderResult>.Failed("该订单已关闭转单功能。");
            }
            if (_orderService.CheckOrderIsOverTime(order) || (order.Status == PMS.PaidQA.Domain.Enums.OrderStatus.Processing))
            {
                return ResponseResult<TransitingOrderResult>.Failed("由于专家已进行回复或该订单已结束，此转单操作已失效。");
            }

            var talentSetting = await _talentSettingService.GetByTalentUserID(request.TargetAnwserID);
            var newOrder = await _orderService.TransitingOrder(order, UserIdOrDefault, talentSetting);
            if (newOrder != null)
            {
                TransitingOrderResult transitingOrderResult = new TransitingOrderResult();
                transitingOrderResult.OrderInfo = _mapper.Map<OrderInfoResult>(order);
                transitingOrderResult.NewOrderInfo = _mapper.Map<OrderInfoResult>(newOrder);
                return ResponseResult<TransitingOrderResult>.Success(transitingOrderResult, "转单成功。");
            }
            else
            {
                return ResponseResult<TransitingOrderResult>.Failed("转单失败。");
            }

        }



        [HttpGet]
        [Description("获取当前用户与专家的有效订单")]
        [Authorize]
        public async Task<ResponseResult<OrderInfoResult>> GetValidOrder(Guid talentUserID)
        {
            var orderInfo = (await _orderService.GetValidOrders(UserIdOrDefault, talentUserID)).FirstOrDefault();
            if (orderInfo != null)
            {
                if (orderInfo.Status == PMS.PaidQA.Domain.Enums.OrderStatus.UnPay)
                {
                    //查询支付订单接口
                    var payResult = await _financeCenterServiceClient.CheckPayStatus(new CheckPayStatusRequest()
                    {
                        orderId = orderInfo.ID,
                        orderType = OrderTypeEnum.Ask
                    });
                    if (payResult != null
                        && payResult.succeed
                        && payResult.data.OrderStatus == ProductManagement.API.Http.Model.FinanceCenter.OrderStatus.PaySucess)
                    {
                        //如果支付成功，触发一下paySuccess方法  
                        //修改订单状态为待提问，
                        var updateSucess = await _orderService.PayCallBack(orderInfo.ID, Guid.Empty, (int)PayStatusEnum.Success, "上学问支付支付成功");
                        if (updateSucess)
                        {
                            orderInfo = await this._orderService.GetAsync(orderInfo.ID);
                        }
                    }
                }
                OrderInfoResult orderInfoResult = _mapper.Map<OrderInfoResult>(orderInfo);
                return ResponseResult<OrderInfoResult>.Success(orderInfoResult, "有效订单列表。");
            }
            else
            {
                return ResponseResult<OrderInfoResult>.Success(null, "无有效订单。");
            }

        }

        [HttpGet]
        [Description("获取优惠券列表")]
        [Authorize]
        public async Task<ResponseResult<GetCouponsResult>> GetCoupons(Guid talentUserId)
        {
            GetCouponsResult result = new GetCouponsResult();
            var detail = await _talentSettingService.GetDetail(talentUserId);
            var couponRules =  GetCouponRules(detail);
            var coupons = await _couponTakeService.GetWaitUseCoupons(UserIdOrDefault, detail.Price, couponRules);
            if (coupons.CanUse.Any())
            {
                var bestCoupons = await _couponTakeService.GetBestCoupons(UserIdOrDefault, detail.Price, minPayAmount, couponRules);
                result.BestCoupons = _mapper.Map<List<CouponTakeResult>>(bestCoupons.BestCoupons);
                result.BestCouponAmmount = bestCoupons.CouponAmmount;
                coupons.CanUse = coupons.CanUse
                    .OrderByDescending(ctd => _couponTakeService.ComputeCouponAmmount(ctd.CouponInfo, detail.Price, minPayAmount))
                    .ThenBy(ctd => ctd.VaildEndTime)
                    .ToList();
            }
            result.CanUseCoupons = _mapper.Map<List<CouponTakeResult>>(coupons.CanUse);
            result.CantUseCoupons = _mapper.Map<List<CouponTakeResult>>(coupons.CantUse);
            return ResponseResult<GetCouponsResult>.Success(result, null);
        }

        [HttpGet]
        [Description("计算某个优惠券组合的优惠金额")]
        [Authorize]
        public async Task<ResponseResult> ComputeCouponAmmount([FromQuery] Guid coupontakeId, [FromQuery] Guid talentUserId)
        {
            var detail = await _talentSettingService.GetDetail(talentUserId);
            decimal couponAmmount = await _couponTakeService.ComputeCouponAmmount(coupontakeId, detail.Price, minPayAmount);
            return ResponseResult.Success(couponAmmount, "优惠金额");
        }





        [HttpPost]
        [Description("支付")]
        [Authorize]
        [ValidateAccoutBind]
        [SafeAction(SafeActionRequestParamName = "request", WillExcuteSecond = 5)]
        public async Task<ResponseResult<PayResult>> Pay(PayRequest request)
        {

            if (UserIdOrDefault == request.TalentID)
            {
                return ResponseResult<PayResult>.Failed("您不能提问你自己！");
            }
            var setting = await _talentSettingService.GetDetail(request.TalentID);
            if (setting == null)
            {
                return ResponseResult<PayResult>.Failed("该专家不存在。");
            }
            if (!setting.IsEnable)
            {
                return ResponseResult<PayResult>.Failed("此专家已关闭上学问服务，您可前往专家列表咨询其他专家。");
            }
            if (await _orderService.ExistsEffectiveOrder(request.TalentID, UserIdOrDefault))
            {
                return ResponseResult<PayResult>.Failed("您与该专家已存在一张有效订单。");
            }
            decimal payAmmount = setting.Price;//支付金额
            bool isUseCoupon = false;
            if (request.CouponTakeID != null)
            {
                var couponrules =  GetCouponRules(setting);
                isUseCoupon = await _couponTakeService.CouponVerification(request.CouponTakeID.Value, UserIdOrDefault, couponrules);
                if (isUseCoupon)
                {
                    //计算优惠券金额
                    decimal couponAmmount = await _couponTakeService.ComputeCouponAmmount(request.CouponTakeID.Value, setting.Price, minPayAmount);
                    decimal priceAfterUseCoupon = setting.Price - couponAmmount;
                    payAmmount = priceAfterUseCoupon < minPayAmount ? minPayAmount : priceAfterUseCoupon;//最少也要一分钱。
                }
                else
                {
                    return ResponseResult<PayResult>.Failed("优惠券无效。");
                }
            }

            //创建新订单
            DateTime now = DateTime.Now;
            var (order, isReCreate) = await _orderService.CreateAsync(new Order()
            {
                ID = Guid.NewGuid(),
                Status = PMS.PaidQA.Domain.Enums.OrderStatus.UnPay,
                Amount = setting.Price,
                AnswerID = setting.TalentUserID,
                CreatorID = UserIdOrDefault,
                PayAmount = payAmmount,
                OriginType = request.Fw,
                CreateTime = now,
                UpdateTime = now,
                ExpireTime = now.AddHours(24), //现有订单24小时过期
                EnableTransiting = isUseCoupon ? false : true
            }); ;
            if (order == null)
            {
                return ResponseResult<PayResult>.Failed("创建订单失败。");
            }


            if (setting.Price == 0)
            {
                //免费
                var updateSucess = await _orderService.PayCallBack(order.ID, UserIdOrDefault, (int)PayStatusEnum.Success, "免费专家支付0元");
                if (updateSucess)
                {
                    PayResult result = new PayResult()
                    {
                        NotNeedPay = true,
                        OrderInfo = _mapper.Map<OrderInfoResult>(order)
                    };
                    return ResponseResult<PayResult>.Success(result, "支付成功。");
                }
                else
                {
                    return ResponseResult<PayResult>.Failed("操作失败。");
                }
            }
            else
            {
                //非免费
                var pay_attch = ConfigHelper.GetHostEnviroment() == 1 ? "from=releaseask" : "from=ask";
                //2.调用支付平台支付接口
                var addPayOrderResult = await _financeCenterServiceClient.AddPayOrder(new AddPayOrderRequest()
                {
                    OrderType = ProductManagement.API.Http.Model.FinanceCenter.OrderTypeEnum.Ask,
                    OpenId = request.OpenId,
                    OrderNo = order.NO,
                    TotalAmount = order.Amount,
                    UserId = order.CreatorID,
                    OrderId = order.ID,
                    PayAmount = payAmmount,
                    DiscountAmount = order.Amount - payAmmount,
                    OrderByProducts = new Orderbyproduct[] {
                           new Orderbyproduct(){
                            ProductId = order.ID,
                            ProductType=(int)ProductType.PaidQA,
                            Remark = "上学问订单",
                            Status = (int)order.Status,
                            Amount = order.Amount
                           }
                           },
                    Remark = "上学问支付",
                    IsRepay = isReCreate ? 1 : 0,
                    Attach = pay_attch,//支付回根据这个回调对应的系统
                    IsWechatMiniProgram = request.ClientType ?? GetPayClientTYpe(),

                }); ;

                if (addPayOrderResult.succeed)
                {
                    if (isUseCoupon)
                    {
                        //清理当前订单对旧优惠券的预占用
                        await _couponTakeService.ClearPreUseRecord(order.ID);
                        //创建订单对新使用优惠券的预占用
                        await _couponTakeService.InsertOrderPreUseRecord(order.ID, request.CouponTakeID.Value);
                    }

                    //3.返回支付凭据和订单信息
                    PayResult result = new PayResult()
                    {
                        NotNeedPay = false,
                        AppId = addPayOrderResult.data.AppId,
                        NonceStr = addPayOrderResult.data.NonceStr,
                        Package = addPayOrderResult.data.Package,
                        PaySign = addPayOrderResult.data.PaySign,
                        SignType = addPayOrderResult.data.SignType,
                        TimeStamp = addPayOrderResult.data.TimeStamp,
                        OrderInfo = _mapper.Map<OrderInfoResult>(order)
                    };
                    return ResponseResult<PayResult>.Success(result, "");
                }
                else
                {
                    return ResponseResult<PayResult>.Failed(addPayOrderResult.msg);
                }
            }


        }



        [HttpPost]
        [Description("支付成功回调")]
        public async Task<ResponseResult> PayCallBack(PayCallBack param)
        {
            //修改订单状态为待提问，
            var updateSucess = await _orderService.PayCallBack(param.orderid, Guid.Empty, param.paystatus, "上学问支付支付成功");
            if (updateSucess)
            {
                return ResponseResult.Success("OK");
            }
            return ResponseResult.Failed("回调业务处理出现问题");
        }

        /// <summary>
        /// 分页获取问题
        /// </summary>
        /// <param name="request"></param>
        /// <modify author="qzy" time="2021-01-11 16:53:57"></modify>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Description("分页获取问题")]
        public async Task<ResponseResult> Page([FromQuery] Models.Order.PageRequest request)
        {
            var result = ResponseResult.Success();
            var talentSetting = await _talentSettingService.GetByTalentUserID(UserId.Value);
            if (!request.IsAsk)
            {
                if (talentSetting == null || !talentSetting.IsEnable)
                {
                    result.Data = new { IsEnable = false };
                    return result;
                }
            }

            var finds = await _orderService.Page(request.PageIndex, request.PageSize, UserId.Value, request._Status, request.IsAsk);
            if (finds?.Items?.Any() == true)
            {
                result.Data = new
                {
                    IsEnable = true,
                    finds.Total,
                    finds.PageCount,
                    rows = finds.Items.Select(p => new ResultItem()
                    {
                        CreateTime = p.CreateTime,
                        FirstReplyTimespan = p.FirstReplyTimespan ?? 0,
                        HasEvaluate = p.IsEvaluate,
                        HasNew = p.HasNew,
                        IsAnony = p.IsAnonymous ?? false,
                        NO = p.NO,
                        Status = p.Status,
                        EvaluateID = p.Comment?.ID ?? Guid.Empty,
                        EvaluateScore = p.Comment?.Score ?? 0,
                        IsAutoEvaluate = p.Comment?.IsAuto ?? false,
                        HeadImageUrl = p.HeadImgUrl,
                        NickName = p.NickName,
                        QuestionContent = p.TextContent,
                        OrderID = p.ID,
                        AnwserUserID = p.AnswerID,
                        NewOrderID = p.NewOrderID.HasValue ? p.NewOrderID.Value : Guid.Empty,
                        IsTimeOutFromWaitAsk = p.IsTimeOutFromWaitAsk,
                        UpdateTime = p.UpdateTime
                    })
                };
            }
            else
            {
                result.Data = new
                {
                    IsEnable = talentSetting?.IsEnable,
                    rows = new object[0]
                };
            }


            return result;
        }

        /// <summary>
        /// 获取是否有未读
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Description("获取是否有未读")]
        public async Task<ResponseResult> HasUnRead(PMS.PaidQA.Domain.Enums.OrderStatus? orderStatus)
        {
            var result = ResponseResult.Success();
            result.Data = false;
            var orderIDs = await _orderService.GetUnReadOrderIDs(UserId.Value, orderStatus);
            if (orderIDs?.Any() == true)
            {
                var askOrderIDs = await _orderService.CheckIsAskOrderIDs(orderIDs, UserId.Value);
                result.Data = new HasUnReadResponse()
                {
                    Total = orderIDs.Count(),
                    AskCount = askOrderIDs?.Count() ?? 0,
                    AnswerCount = orderIDs.Count() - askOrderIDs?.Count() ?? 0
                };
            }
            else
            {
                result.Data = new HasUnReadResponse()
                {
                    Total = 0,
                    AskCount = 0,
                    AnswerCount = 0
                };
            }
            return result;
        }



        [HttpGet]
        [Description("供前端下单成功回传第三方接口。")]
        public async Task<ResponseResult> ReBackOtherAdvPalteforms(string pageUrl, Guid orderId)
        {
            var order = _orderService.Get(orderId);
            var now = DateTime.Now;
            if (order == null)
            {
                return ResponseResult.Failed("找不到该订单");
            }
            if (order.Status != PMS.PaidQA.Domain.Enums.OrderStatus.WaitingAsk)
            {
                return ResponseResult.Failed("当前订单状态非待提问。");
            }
            if (now > order.ExpireTime)
            {
                return ResponseResult.Failed("订单已过期。");
            }
            TimeSpan lockTimspan = order.ExpireTime.GetValueOrDefault() - now;
            var wechatresult = await ReBackWeChat(pageUrl, order, lockTimspan);
            var baiduresult = await ReBackBaidu(pageUrl, order, lockTimspan);
            return ResponseResult.Success(new
            {
                WeChatResult = wechatresult,
                BaiduResult = baiduresult
            });
        }

        async Task<string> ReBackWeChat(string pageUrl, Order order, TimeSpan lockTimeSpan)
        {
            string lockKey = $"PaidQAReBackWeChat:{order.ID}";
            string lockValue = order.ID.ToString();
            try
            {
                Uri uri = new Uri(pageUrl);
                var values = uri.QueryValues("gdt_vid");
                var queryIndex = uri.AbsoluteUri.IndexOf('?');
                string url = string.Empty; ;
                if (queryIndex > 0)
                {
                    url = uri.AbsoluteUri.Substring(0, queryIndex).ToLower();
                }
                if (!values.Any())
                {
                    _logger.LogDebug("找不到[gdt_vid]");
                    return "找不到[gdt_vid]";
                }
                if (await _easyRedisClient.LockTakeAsync(lockKey, lockValue, lockTimeSpan))
                {
                    var tokenResult = await _weChatAppClient.GetAccessToken();
                    string requestUrl = $"https://api.weixin.qq.com/marketing/user_actions/add?version=v1.0&access_token={tokenResult.token}";
                    var param = new
                    {
                        user_action_set_id = "1112003302",
                        actions = new[] {
                            new{
                                url = url,
                                action_time = DateTime.Now.D2ISecond(),
                                action_type="COMPLETE_ORDER",
                                trace = new{
                                click_id = values.LastOrDefault()
                                },
                                action_param = new{
                                value = order.PayAmount,

                                }
                         }
                      }

                    };
                    var response = await _asRebackHttpClient.PostAsJsonAsync(requestUrl, param);
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    if (JObject.Parse(jsonResponse)["errcode"].Value<int>() != 0)
                    {
                        await _easyRedisClient.LockReleaseAsync(lockKey, lockValue);
                    }
                    return jsonResponse;
                }
                else
                {
                    return "当前订单已回传。";
                }

            }
            catch (Exception ex)
            {
                await _easyRedisClient.LockReleaseAsync(lockKey, lockValue);
                _logger.LogError(ex, "微信广告回传异常。");
                return "微信广告回传异常";
            }

        }

        async Task<string> ReBackBaidu(string pageUrl, Order order, TimeSpan lockTimeSpan)
        {
            string lockKey = $"PaidQAReBackBaidu:{order.ID}";
            string lockValue = order.ID.ToString();
            try
            {
                Uri uri = new Uri(pageUrl);
                var values = uri.QueryValues("bd_vid");
                if (!values.Any())
                {
                    _logger.LogDebug("找不到[bd_vid]");
                    return "找不到[bd_vid]";
                }
                if (await _easyRedisClient.LockTakeAsync(lockKey, lockValue, lockTimeSpan))
                {

                    string requestUrl = "https://ocpc.baidu.com/ocpcapi/api/uploadConvertData";
                    var param = new
                    {
                        token = "YfP1HWqaGjhvhHF0oHCCb0j6XYSWWL2A@7q5vhc4i9tU80HwDwimFHGiT01Y4veC1",
                        conversionTypes = new[]
                                {
                                new {
                                    logidUrl=uri.AbsoluteUri,
                                    newType= 14
                                },

                            }
                    };
                    var response = await _asRebackHttpClient.PostAsJsonAsync(requestUrl, param);
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    if (JObject.Parse(jsonResponse)["header"]["status"].Value<int>() != 0)
                    {
                        await _easyRedisClient.LockReleaseAsync(lockKey, lockValue);
                    }
                    return jsonResponse;
                }
                else
                {
                    return "当前订单已回传。";
                }
            }
            catch (Exception ex)
            {
                await _easyRedisClient.LockReleaseAsync(lockKey, lockValue);
                _logger.LogError(ex, "百度广告回传异常。");
                return "百度广告回传异常";
            }

        }


        /// <summary>
        /// 生成上学问所用的规则
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
       public static  List<CouponRule> GetCouponRules(TalentDetailExtend detail)
        {
            //规则
            //1   上学问 - 默认
            //2   上学问 - 擅长领域
            //3   上学问 - 指定专家
            //4   上学问 - 学段
            //5   上学问 - 仅限未有订单用户领取
            //平台类型:1
            List<CouponRule> couponRules = new List<CouponRule>();
            couponRules.Add(new CouponRule() { Platform = 1, RuleType = 1, RuleValue = "0" });
            couponRules.Add(new CouponRule() { Platform = 1, RuleType = 3, RuleValue = detail.TalentUserID.ToString() });
            if (detail.TalentGrades.Any())
            {
                foreach (var grade in detail.TalentGrades)
                {
                    couponRules.Add(new CouponRule() { Platform = 1, RuleType = 4, RuleValue = grade.ID.ToString() });
                }
            }
            if (detail.TalentRegions.Any())
            {
                foreach (var region in detail.TalentRegions)
                {
                    couponRules.Add(new CouponRule() { Platform = 1, RuleType = 2, RuleValue = region.ID.ToString() });
                }
            }
           
            return couponRules;
        }

        int GetPayClientTYpe()
        {
            var ct = Request.Headers.GetClientType();
            if (ct.HasFlag(ClientType.WX))
            {
                return 0;
            }
            if (ct.HasFlag(ClientType.WxMinApp))
            {
                return 1;
            }
            if (ct.HasFlag(ClientType.Mobile))
            {
                return 2;
            }
            return 0;
        }

    }
}
