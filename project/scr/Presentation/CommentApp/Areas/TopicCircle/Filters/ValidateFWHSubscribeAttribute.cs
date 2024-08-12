using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PMS.TopicCircle.Application.Services;
using PMS.UserManage.Application.IServices;
using ProductManagement.API.Http.Model;
using ProductManagement.API.Http.Service;
using Sxb.Web.Areas.TopicCircle.Models;
using Sxb.Web.Response;
using Sxb.Web.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeChat;
using WeChat.Interface;
using WeChat.Model;
using IUserService = PMS.UserManage.Application.IServices.IUserService;

namespace Sxb.Web.Areas.TopicCircle.Filters
{

    /// <summary>
    /// 验证用户是否关注微信服务号
    /// </summary>
    public class ValidateFWHSubscribeAttribute : ActionFilterAttribute
    {

        public enum FWHQRCodeType { 

            JoinCircle=1,

            CreateCircle = 2
        }

        public string DataParamName { get; set; }

        public FWHQRCodeType QRCodeType { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            IUserService userService =  context.HttpContext.RequestServices.GetService(typeof(IUserService)) as IUserService;
            WeChatAppClient weChatAppClient = context.HttpContext.RequestServices.GetService(typeof(WeChatAppClient)) as WeChatAppClient;
            IWeChatQRCodeService  weChatQRCodeService = context.HttpContext.RequestServices.GetService(typeof(IWeChatQRCodeService)) as IWeChatQRCodeService;
            ControllerBase controller = context.Controller as ControllerBase;
            Guid userId = controller.GetUserInfo().UserId;
            if (!userService.CheckIsSubscribeFwh(userId, ConfigHelper.GetFwhSuffix().Replace("_","")))
            {
                 string QRCode = "";
                switch (QRCodeType)
                {
                    case FWHQRCodeType.JoinCircle:
                        Guid.TryParse(context.HttpContext.Request.Param(DataParamName), out Guid circleID);
                        string subscribeHandleUrl = $"{ConfigHelper.GetHost()}/tpc/Share/SubscribeCallBack?DataId={circleID}&type={(int)SubscribeCallBackType.joincircle}";
                        ICircleService circleService = context.HttpContext.RequestServices.GetService(typeof(ICircleService)) as ICircleService;
                        QRCode = circleService.GenerateWeChatCode(circleID, subscribeHandleUrl).GetAwaiter().GetResult();
                        break;
                    default:
                        int expire_second = (int)TimeSpan.FromDays(2).TotalSeconds; //30天有效期的二维码
                        var accessToken = weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" }).GetAwaiter().GetResult();
                        var Scene = new CreateTempQRStrSceneCodeRequest(expire_second);
                        var qrcodeResponse = weChatQRCodeService.GenerateTempQRCode(accessToken.token, Scene).GetAwaiter().GetResult();
                        QRCode = qrcodeResponse.ImgUrl;
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
    }
}
