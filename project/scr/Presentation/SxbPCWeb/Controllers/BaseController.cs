using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.Common;
using ProductManagement.Framework.WeChat;
using Microsoft.Extensions.Options;
using ProductManagement.Framework.Foundation;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Sxb.PCWeb.Controllers
{
    public class WxAppId
    {
        public string AppId { get; set; }
    }

    public class BaseController : Controller
    {
        protected Guid? userId
        {
            get
            {
                if (User.Identity.IsAuthenticated)
                {
                    return this.User.Identity.GetUserInfo().UserId;
                }
                else
                {
                    return null;
                }

            }

        }


        //public UserInfo _user = new UserInfo() { Id = Guid.Parse("257DA955-620D-4379-A5B2-44A98DB6574E"),NickName = "Job001", Phone  = "13203056512" , Role = PMS.UserManage.Domain.Common.UserRole.Visitor};

        private string _wxAppId;


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _wxAppId = "wxeefc53a3617746e2";

            string weixin_AppID = _wxAppId;
            long timestamp = DateTime.Now.D2I();
            string nonceStr = Guid.NewGuid().ToString("N");

            string weixin_api_access_token = GetAccessToken_JsApi.GetValue("weixin_access_token");
            string weixin_jsapi_ticket = GetAccessToken_JsApi.GetValue("weixin_jsapi_ticket");

            ViewBag.weixin_AppID = weixin_AppID;
            ViewBag.timestamp = timestamp;
            ViewBag.nonceStr = nonceStr;

            ViewBag.signature = WXAPIHelper.GetSign(weixin_jsapi_ticket, nonceStr, timestamp, Request.GetAbsoluteUri());
            ViewBag.ShareImgLink = "https://cdn.sxkid.com/images/logo_share_v4.png";

            base.OnActionExecuting(context);
        }
        public BaseController()
        {

        }

        protected IActionResult AjaxSuccess(string msg = "OK", object data = null)
        {
            return Json(new
            {
                status = 1,
                msg = msg,
                data = data

            });

        }

        protected IActionResult AjaxFail(string msg = "Fail")
        {
            return Json(new
            {
                status = -1,
                msg = msg

            });

        }

        protected IActionResult AjaxNoLogin(string msg = "Need login")
        {
            return Json(new
            {
                status = 402,
                msg = msg
            });
        }

    }

}