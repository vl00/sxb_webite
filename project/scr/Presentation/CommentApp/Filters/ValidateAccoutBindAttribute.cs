using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PMS.UserManage.Application.IServices;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model;
using ProductManagement.Framework.Cache.Redis;
using Sxb.Web.Areas.TopicCircle.Models;
using Sxb.Web.Response;
using Sxb.Web.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeChat;
using WeChat.Model;

namespace Sxb.Web.Filters
{
    /// <summary>
    /// 校验账号绑定
    /// </summary>
    public class ValidateAccoutBindAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            IAccountService _accountService = context.HttpContext.RequestServices.GetService(typeof(IAccountService)) as IAccountService;
            IUserService userService = context.HttpContext.RequestServices.GetService(typeof(IUserService)) as IUserService;
            ControllerBase controller = context.Controller as ControllerBase;
            Guid userId = controller.User.Identity.GetUserInfo().UserId;
            var isBindPhone = _accountService.IsBindPhone(userId);
            var isBindWx = _accountService.IsBindWx(userId);
            if (!isBindPhone || !isBindWx)
            {
               
               string imgUrl =  _accountService.GenerateBindAccountQRCode($"{ConfigHelper.GetHost()}/tpc/Share/SubscribeCallBack?DataId={userId}&type={(int)SubscribeCallBackType.bindAccount}",1);
                string tipsWord = isBindPhone ? "微信号" : "手机号";
                context.Result = new JsonResult(new ResponseResult()
                {
                    Msg = $"检测到您当前账号<br/>尚未绑定{tipsWord}<br/>因此无法进行下一步操作哦！",
                    status = ResponseCode.UnBindAcount,
                    Succeed = false,
                    Data = new
                    {
                        isBindPhone,
                        isBindWx,
                        qrcode = imgUrl
                    }
                }) ;
            }
        }


    
    }
}
