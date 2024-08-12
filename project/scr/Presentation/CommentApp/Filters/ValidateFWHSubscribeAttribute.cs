using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PMS.TopicCircle.Application.Services;
using PMS.UserManage.Application.IServices;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model;
using ProductManagement.API.Http.Service;
using ProductManagement.Framework.Cache.Redis;
using Sxb.Web.Areas.Common.Models;
using Sxb.Web.Areas.Common.WeChatQRCallBackHandle;
using Sxb.Web.Areas.PaidQA.Models.Order;
using Sxb.Web.Areas.TopicCircle.Models;
using Sxb.Web.Response;
using Sxb.Web.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using WeChat;
using WeChat.Interface;
using WeChat.Model;
using IUserService = PMS.UserManage.Application.IServices.IUserService;
using SubscribeCallBackQueryRequest = Sxb.Web.Areas.Common.WeChatQRCallBackHandle.SubscribeCallBackQueryRequest;

namespace Sxb.Web.Filters
{

    /// <summary>
    /// 验证用户是否关注微信服务号
    /// </summary>
    public class ValidateFWHSubscribeAttribute : ActionFilterAttribute
    {

        public SubscribeQRCodeType QRType { get; set; } = SubscribeQRCodeType.Default;

        public ValidateFWHSubscribeAttribute()
        {

        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            IUserService userService = context.HttpContext.RequestServices.GetService(typeof(IUserService)) as IUserService;
            IWeChatAppClient weChatAppClient = context.HttpContext.RequestServices.GetService(typeof(IWeChatAppClient)) as IWeChatAppClient;
            IWeChatQRCodeService weChatQRCodeService = context.HttpContext.RequestServices.GetService(typeof(IWeChatQRCodeService)) as IWeChatQRCodeService;
            ControllerBase controller = context.Controller as ControllerBase;
            Guid userId = controller.User.Identity.GetUserInfo().UserId;
            if (!userService.CheckIsSubscribeFwh(userId, ConfigHelper.GetFwhSuffix().Replace("_", "")))
            {
                string QRCode = string.Empty;
                switch (QRType)
                {
                    case SubscribeQRCodeType.EnablePaidQA:
                        QRCode = GenerateRedirectQRCode(context
                            , weChatAppClient
                            , weChatQRCodeService
                            , QRType
                            , ConfigHelper.GetHost() + "/ask/order/setting/"
                            ).GetAwaiter().GetResult();
                        break;
                    case SubscribeQRCodeType.PaidQAAskAtOnce:
                         QRCode = GeneratePaidQAAskAtOnceQRCode(context
                            , weChatAppClient
                            , weChatQRCodeService
                            ).GetAwaiter().GetResult();
                        break;
                    case SubscribeQRCodeType.Default:
                    default:
                        QRCode = GenerateDefaultQRCode(weChatAppClient, weChatQRCodeService).GetAwaiter().GetResult();
                        break;
                }

                context.Result = new JsonResult(new ResponseResult()
                {
                    Msg = "未关注上学帮微信服务号",
                    status = ResponseCode.UnSubScribeFWH,
                    Succeed = false,
                    Data = new { QRCode }
                });
            }
        }

        async Task<string> GenerateDefaultQRCode(IWeChatAppClient weChatAppClient, IWeChatQRCodeService weChatQRCodeService)
        {
            int expire_second = (int)TimeSpan.FromDays(2).TotalSeconds; //30天有效期的二维码
            var accessToken = await weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" });
            var Scene = new CreateTempQRStrSceneCodeRequest(expire_second);
            var qrcodeResponse = await weChatQRCodeService.GenerateTempQRCode(accessToken.token, Scene);
            return qrcodeResponse.ImgUrl;
        }


        /// <summary>
        /// 生成立即咨询场景的二维码。
        /// </summary>
        /// <param name="context"></param>
        /// <param name="weChatAppClient"></param>
        /// <param name="weChatQRCodeService"></param>
        /// <returns></returns>
        async Task<string> GeneratePaidQAAskAtOnceQRCode(ActionExecutingContext context,
           IWeChatAppClient weChatAppClient,
           IWeChatQRCodeService weChatQRCodeService
           )
        {

            PayRequest payRequest = context.ActionArguments["request"] as PayRequest;
            IEasyRedisClient easyRedisClient = context.HttpContext.RequestServices.GetService(typeof(IEasyRedisClient)) as IEasyRedisClient;
            var code = await SenceQRCoder.GenerateSenceQRCode(
                  weChatAppClient,
                  easyRedisClient,
                  weChatQRCodeService,
                  new SubscribeCallBackQueryRequest()
                  {
                      DataId = payRequest.TalentID,
                      type = SubscribeQRCodeType.PaidQAAskAtOnce
                  }
                 ); ;
            return code;
        }


        async Task<string> GenerateRedirectRefererRCode(ActionExecutingContext context,
            IWeChatAppClient weChatAppClient,
            IWeChatQRCodeService weChatQRCodeService,
            SubscribeQRCodeType subscribeQRCodeType
            )
        {
            var referer = context.HttpContext.Request.Headers["Referer"];
            IEasyRedisClient easyRedisClient = context.HttpContext.RequestServices.GetService(typeof(IEasyRedisClient)) as IEasyRedisClient;
            var code = await SenceQRCoder.GenerateSenceQRCode(
                  weChatAppClient,
                  easyRedisClient,
                  weChatQRCodeService,
                  new SubscribeCallBackQueryRequest()
                  {
                      DataUrl = referer,
                      type = subscribeQRCodeType
                  }
                 ); ;
            return code;
        }
        async Task<string> GenerateRedirectQRCode(ActionExecutingContext context,
            IWeChatAppClient weChatAppClient,
            IWeChatQRCodeService weChatQRCodeService,
            SubscribeQRCodeType subscribeQRCodeType,
            string url
            )
        {
            IEasyRedisClient easyRedisClient = context.HttpContext.RequestServices.GetService(typeof(IEasyRedisClient)) as IEasyRedisClient;
            var code = await SenceQRCoder.GenerateSenceQRCode(
                  weChatAppClient,
                  easyRedisClient,
                  weChatQRCodeService,
                  new SubscribeCallBackQueryRequest()
                  { 
                      DataUrl = url,
                      type = subscribeQRCodeType
                  });
            return code;
        }
    }
}
