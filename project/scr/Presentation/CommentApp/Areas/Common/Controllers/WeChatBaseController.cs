using DotNetCore.CAP;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PMS.Infrastructure.Application.IService;
using PMS.MediatR.Events.WeChat;
using PMS.PaidQA.Application.Services;
using PMS.TopicCircle.Application.Services;
using PMS.TopicCircle.Domain.Entities;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Application.ModelDto;
using PMS.UserManage.Domain.Entities;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using ProductManagement.Infrastructure.Configs;
using Sxb.Web.Application.Event;
using Sxb.Web.Areas.Common.Models;
using Sxb.Web.Areas.Common.WeChatQRCallBackHandle;
using Sxb.Web.Controllers;
using Sxb.Web.Filters;
using Sxb.Web.Response;
using Sxb.Web.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WeChat;
using WeChat.Interface;
using WeChat.Model;
using IKeyValueService = PMS.TopicCircle.Application.Services.IKeyValueService;
using IUserService = PMS.UserManage.Application.IServices.IUserService;

namespace Sxb.Web.Areas.Common.Controllers
{
    [Route("Common/wechat/[action]")]
    [ApiController]
    public class WeChatBaseController : ApiBaseController
    {
        internal IUserService _userService;
        internal IWeChatAppClient _weChatAppClient;
        internal IAccountService _accountService;
        IConfiguration config;
        internal ICircleService _circleService;
        internal IKeyValueService _keyValueService;
        internal ICircleFollowerService _circleFollowerService;
        internal ITalentService _talentService;
        internal Microsoft.Extensions.Logging.ILogger _logger;
        internal ITopicService _topicService;
        internal TopicOption _topicOption;
        internal IEasyRedisClient _easyRedisClient;
        IServiceProvider _serviceProvider;
        IWeChatQRCodeService _weChatQRCodeService;
        ITemplateMessageService _templateMessageService;
        IAssessService _assessService;
        IMediator _mediator;
        IWeChatService _weChatService;
        ICapPublisher _capPublisher;
        public WeChatBaseController(IUserService userService,
            ICircleService circleService,
            IKeyValueService keyValueService,
            ICircleFollowerService circleFollowerService,
            ITalentService talentService,
            ILogger<WeChatBaseController> logger,
            ITopicService topicService,
            IConfiguration configuration
            , IAccountService accountService,
            IWeChatAppClient weChatAppClient,
            IOptions<TopicOption> topicOption,
             IEasyRedisClient easyRedisClient,
             IServiceProvider serviceProvider,
             IWeChatQRCodeService weChatQRCodeService,
             IAssessService assessService,
             IMediator mediator
            , ITemplateMessageService templateMessageService
            , IWeChatService weChatService, ICapPublisher capPublisher)
        {
            _assessService = assessService;
            _mediator = mediator;
            this._circleService = circleService;
            this._keyValueService = keyValueService;
            this._circleFollowerService = circleFollowerService;
            this._talentService = talentService;
            this._logger = logger;
            this._topicService = topicService;
            this.config = configuration;
            _topicOption = topicOption.Value;
            _easyRedisClient = easyRedisClient;
            _userService = userService;
            _weChatAppClient = weChatAppClient;
            _accountService = accountService;
            _serviceProvider = serviceProvider;
            _weChatQRCodeService = weChatQRCodeService;
            _templateMessageService = templateMessageService;
            _weChatService = weChatService;
            _capPublisher = capPublisher;
        }



        [HttpGet]
        [Authorize]
        [Description("检查是否关注了服务号")]
        public async Task<ResponseResult> CheckSubscribeFwh([FromQuery] int repeatTime = 1)
        {
            return await PollingTool<ResponseResult>.StartPolling(async (pt) =>
            {
                var userId = base.UserId.Value;
                var isBindFwh = _userService.CheckIsSubscribeFwh(userId, ConfigHelper.GetFwh());
                if (isBindFwh)
                {
                    pt.Stop = true;
                    pt.Data = ResponseResult.Success("已关注");
                }
                else if (pt.Count >= repeatTime)
                {
                    int expire_second = (int)TimeSpan.FromDays(2).TotalSeconds;
                    var accessToken = await _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" });
                    var Scene = new CreateTempQRStrSceneCodeRequest(expire_second);
                    var qrcodeResponse = _weChatQRCodeService.GenerateTempQRCode(accessToken.token, Scene).GetAwaiter().GetResult();
                    pt.Stop = true;
                    pt.Data = new ResponseResult()
                    {
                        Msg = "未关注上学帮微信服务号",
                        status = ResponseCode.UnSubScribeFWH,
                        Succeed = false,
                        Data = new
                        {
                            isBindFwh,
                            qrcode = qrcodeResponse.ImgUrl
                        }

                    };
                }
                return pt;
            }, 1);
        }



        /// <summary>
        /// 已关注的情况下获取不了，未关注能获取。
        /// </summary>
        /// <param name="request"></param>
        /// <param name="repeatTime"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Description("尝试获取获取关注服务号二维码")]
        public async Task<ResponseResult> TryGetWeChatSenceQRCode([FromBody] SubscribeCallBackQueryRequest request, [FromQuery] int repeatTime = 1)
        {

            return await PollingTool<ResponseResult>.StartPolling(async (pt) =>
            {
                var userId = base.UserId.Value;
                var isBindFwh = _userService.CheckIsSubscribeFwh(userId, ConfigHelper.GetFwh());
                if (isBindFwh)
                {
                    pt.Stop = true;
                    pt.Data = ResponseResult.Success("已关注");
                }
                else if (pt.Count >= repeatTime)
                {

                    var qrcodeUrl = await SenceQRCoder.GenerateSenceQRCode(
                     _weChatAppClient,
                     _easyRedisClient,
                     _weChatQRCodeService,
                     request
                    );
                    pt.Stop = true;
                    pt.Data = new ResponseResult()
                    {
                        Msg = "未关注上学帮微信服务号",
                        status = ResponseCode.UnSubScribeFWH,
                        Succeed = true,
                        Data = new
                        {
                            qrcode = qrcodeUrl
                        }
                    };
                }
                return pt;
            }, 1);
        }




        /// <summary>
        /// 已关注的情况下获取不了，未关注能获取。
        /// </summary>
        /// <param name="request"></param>
        /// <param name="repeatTime"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Description("获取关注服务号二维码")]
        public async Task<ResponseResult> GetWeChatSenceQRCode([FromBody] SubscribeCallBackQueryRequest request)
        {
            string qrcodeUrl = await _weChatService.GenerateSenceQRCode(UtilsHelper.GetSubscribeCallBackHandlerUrl(request, Request), 1);
            return ResponseResult.Success(new
            {
                qrcode = qrcodeUrl
            }, "OK");


        }


        /// <summary>
        /// 获取小程序客服消息场景值，类似于获取微信服务号二维码的功能。
        /// 这是自定义的一个功能，最初目的是为了从小程序引导关注服务号。
        /// </summary>
        /// <returns></returns>
        [Description("获取小程序卡片客服消息场景值")]
        [HttpPost]
        public async Task<ResponseResult> GetWXMPCardSencesCode([FromBody] SubscribeCallBackQueryRequest request)
        {
            string subscribeHandleUrl = UtilsHelper.GetSubscribeCallBackHandlerUrl(request, Request);
            string sceneKey = Guid.NewGuid().ToString();
            int expire_second = (int)TimeSpan.FromDays(30).TotalSeconds; //30天有效期
            await _easyRedisClient.AddStringAsync(sceneKey, subscribeHandleUrl, TimeSpan.FromSeconds(expire_second));
            return ResponseResult.Success(new { sceneKey });
        }


        /// <summary>
        /// 接收【服务号】|【小程序】消息回调（通过内部的一个项目进行的二次转发）
        /// </summary>
        /// <param name="qrequest"></param>
        /// <param name="frequest"></param>
        /// <returns></returns>
        [Description("开放一个接口供二维码扫码关注时触发的回调")]
        [HttpPost]
        public async Task<ResponseResult> SubscribeCallBack([FromQuery] SubscribeCallBackQueryRequest qrequest, [FromBody] SubscribeCallBackFormRequest frequest)
        {
            //_logger.LogInformation("接收到上学帮微信业务应用回调，type:{type},openid:{openid},dataId:{dataId}", qrequest.type, frequest.OpenId, qrequest.DataId);
            this._logger.LogDebug("接收到上学帮微信业务应用回调，type:{type},openid:{openid},dataId:{dataId}", qrequest.type, frequest.OpenId, qrequest.DataId);
            Type handlerType = qrequest.type.GetHandler();
            if (handlerType == null)
            {
                return ResponseResult.Failed("找不到对应的处理类型。");
            }
            var handler = _serviceProvider.GetService(handlerType) as ISubscribeCallBackHandler;
            if (handler == null)
            {
                return ResponseResult.Failed("找不到回调Handler");
            }

            if (frequest.Event != null)
            {
                await WxScanAfter(qrequest, frequest).ConfigureAwait(false);
            }

            return await handler.Process(qrequest, frequest);
        }

        private async Task WxScanAfter(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest)
        {
            try
            {
                //扫码回调, 记录扫码日志
                await _mediator.Publish(new ScanSubscribeEvent()
                {
                    AppId = frequest.AppId,
                    AppName = frequest.AppName,
                    OpenId = frequest.OpenId,
                    DataId = qrequest.DataId,
                    DataUrl = qrequest.DataUrl,
                    DataJson = qrequest.DataJson,
                    Type = qrequest.type.ToString(),
                    WeChatEvent = frequest.Event.Value
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            try
            {
                //扫码关注回调, 添加积分
                if (frequest.Event == WeChatEventEnum.subscribe)
                {
                    var openwx = _userService.GetOpenWeixin(frequest.OpenId);
                    if (openwx != null)
                        await _capPublisher.PublishAsync(nameof(AddWeChatSubscribeIntegrationEvent), new AddWeChatSubscribeIntegrationEvent()
                        {
                            UserId = openwx.UserId,
                            CreateTime = DateTime.Now,
                            OpenId = openwx.OpenId,
                        });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
