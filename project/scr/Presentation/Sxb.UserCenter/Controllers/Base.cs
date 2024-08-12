using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Common;
using Sxb.UserCenter.Response;
using Sxb.UserCenter.Utils;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.UserCenter.Controllers
{
    public class Base : Controller
    {
        private IAccountService _account;

        protected Guid userID { get; set; }
        protected double? latitude { get; set; }
        protected double? longitude { get; set; }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _account = HttpContext.RequestServices.GetService<IAccountService>();

            string controllerName = ControllerContext.RouteData.Values["Controller"].ToString().ToLower();
            string actionName = ControllerContext.RouteData.Values["Action"].ToString().ToLower();
            if (controllerName == "account" && actionName == "logout")
            {
                return;
            }
            if (User.Identity.IsAuthenticated)
            {
                var id = User.Identity as ClaimsIdentity;
                var claim = id.FindFirst(JwtClaimTypes.Id);
                userID = Guid.Parse(claim.Value);
                ViewBag.Nickname = id.FindFirst(JwtClaimTypes.Name)?.Value;
                ViewBag.HeadImgUrl = id.FindFirst(JwtClaimTypes.Picture)?.Value;
                ViewBag.Roles = id.FindFirst(JwtClaimTypes.Role)?.Value;
                var isBlocked = _account.CheckAccountStatus(userID).Result;
                if (isBlocked == 1)
                {
                    HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    HttpContext.Response.SetUserId("");
                    if (IsAjaxRequest())
                    {
                        var result = ResponseResult.Failed("当前账号已失效，如有疑问请致电020-89623079咨询");
                        result.Data = new
                        {
                            RetutnUrl = "/login/",
                            IsBlocked = 1
                        };
                        result.status = ResponseCode.NoLogin;
                        context.Result = Content(Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None, new Newtonsoft.Json.JsonSerializerSettings()
                        {
                            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                        }));
                    }
                    else
                    {
                        context.Result = View("~/Views/Shared/Prompt.cshtml", ("当前账号已失效，如有疑问请致电020-89623079咨询", Url.Action("Logout", "Account")));
                    }
                    return;
                }

                //登录状态下，如果cookie中找不到userid，重新设置一下userid
                if (userID != Guid.Empty && string.IsNullOrWhiteSpace(HttpContext.Request.GetUserId()))
                {
                    HttpContext.Response.SetUserId(userID.ToString());
                }
            }

            var _lat = context.HttpContext.Request.GetLatitude();
            var _lng = context.HttpContext.Request.GetLongitude();
            if (double.TryParse(_lat, out double _latitude))
            {
                latitude = _latitude;
            }
            if (double.TryParse(_lng, out double _longitude))
            {
                longitude = _longitude;
            }
            Task.Run(() =>
            {
                if (!string.IsNullOrEmpty(Request.Cookies["devicetoken"]) && !string.IsNullOrEmpty(Request.Cookies["uuid"]) && Request.Cookies["devicetoken"].Length >= 30)
                {
                    var type = EnumSet.DeviceType.Windows;
                    if (Request.Cookies["devicetoken"].Length == 64)//Request.Headers["User-Agent"].ToString().ToLower().Contains("ios"))
                        type = EnumSet.DeviceType.iOS;
                    else if (Request.Cookies["devicetoken"].Length < 64) //Request.Headers["User-Agent"].ToString().ToLower().Contains("android"))
                        type = EnumSet.DeviceType.Android;
                    int.TryParse(Request.Cookies["localcity"], out int city);
                    _account.SetDeviceToken(Request.Cookies["uuid"], Request.Cookies["devicetoken"], type, userID, city);
                }
            });
        }
        protected new ContentResult Json(object obj)
        {
            Newtonsoft.Json.JsonSerializerSettings js = new Newtonsoft.Json.JsonSerializerSettings();
            //js.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            js.StringEscapeHandling = Newtonsoft.Json.StringEscapeHandling.EscapeNonAscii;
            return new ContentResult() { Content = Newtonsoft.Json.JsonConvert.SerializeObject(obj, js), ContentType = "text/json", StatusCode = 200 };
        }
        protected ContentResult Jsonp(object obj, string callback)
        {
            Newtonsoft.Json.JsonSerializerSettings js = new Newtonsoft.Json.JsonSerializerSettings();
            //js.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            return new ContentResult() { Content = (callback ?? "callback") + "(" + Newtonsoft.Json.JsonConvert.SerializeObject(obj, js) + ");", ContentType = "text/html", StatusCode = 200 };
        }
        protected new ViewResult View()
        {
            return View(null, null);
        }
        protected new ViewResult View(string viewName)
        {
            return View(viewName, null);
        }
        protected new ViewResult View(object model)
        {
            return View(null, model);
        }
        protected new ViewResult View(string viewName, object model)
        {
            string deviceType = IsMobile() ? "Mobile" : "PC";
            if (!string.IsNullOrEmpty(viewName))
            {
                return base.View(viewName.Replace("~/Views/", "~/Views/" + deviceType + "/"), model);
            }
            else
            {
                string controllerName = ControllerContext.RouteData.Values["Controller"].ToString();
                string actionName = ControllerContext.RouteData.Values["Action"].ToString();
                return base.View("~/Views/" + deviceType + "/" + controllerName + "/" + actionName + ".cshtml", model);
            }
        }
        public bool IsAjaxRequest()
        {
            if (HttpContext.Request == null)
                throw new ArgumentNullException("request");

            if (HttpContext.Request.Headers != null)
                return HttpContext.Request.Headers["X-Requested-With"].ToString().ToLower() == "xmlhttprequest";

            return false;
        }
        public bool IsMobile()
        {
            //string ua = Request.Headers["User-Agent"].ToString().ToLower();
            //return ua.Contains("mobile") || ua.Contains("ischool");
            string controllerName = ControllerContext.RouteData.Values["Controller"].ToString().ToLower();
            string actionName = ControllerContext.RouteData.Values["Action"].ToString().ToLower();
            if (controllerName.Contains("login")/* && actionName=="index"*/)
            {
                string ua = Request.Headers["User-Agent"].ToString().ToLower();
                return ua.Contains("mobile") || ua.Contains("ischool");
            }
            return true;
        }
        public IActionResult iSchoolResult(object model)
        {
            if (IsAjaxRequest())
            {
                return Json(model);
            }
            else
            {
                return View(model);
            }
        }
    }

    //public class AuthorizeAttribute : ActionFilterAttribute
    //{
    //    readonly bool throwIfNoQx;

    //    public AuthorizeAttribute() : this(false) { }

    //    public AuthorizeAttribute(bool throwIfNoQx)
    //    {
    //        this.throwIfNoQx = throwIfNoQx;
    //    }

    //    public override void OnActionExecuting(ActionExecutingContext context)
    //    {
    //        // [AllowAnonymous] or IGoThroughMvcFilter
    //        if (context.ActionDescriptor.FilterDescriptors.Any(a =>
    //        {
    //            var fty = a.Filter.GetType();
    //            return fty == typeof(Microsoft.AspNetCore.Mvc.Authorization.AllowAnonymousFilter) ||
    //                typeof(IGoThroughMvcFilter).IsAssignableFrom(fty);
    //        }))
    //        {
    //            //业务逻辑
    //            base.OnActionExecuting(context);
    //            return;
    //        }

    //        // 已登录用户
    //        if (context.HttpContext.User.Identity.IsAuthenticated)
    //        {
    //            //业务逻辑
    //            base.OnActionExecuting(context);
    //            return;
    //        }

    //        var config = context.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
    //        var IsHost = config.GetSection("IsHost").Get<IsHost>();

    //        ///
    //        /// 未登录
    //        /// 
    //        if (IsJsonpRequest(context.HttpContext.Request))
    //        {
    //            var url = $"{IsHost.SiteHost_User}/login/?return={Uri.EscapeDataString(context.HttpContext.Request.Headers["Referer"])}";
    //            var callback = context.HttpContext.Request.Query["callback"][0];
    //            Newtonsoft.Json.JsonSerializerSettings js = new Newtonsoft.Json.JsonSerializerSettings();
    //            js.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
    //            context.Result = new ContentResult()
    //            {
    //                Content = (callback ?? "callback") + "(" + Newtonsoft.Json.JsonConvert.SerializeObject(
    //                new { status = 2, errorDescription = "未登录", url }, js) + ");",
    //                ContentType = "text/html",
    //                StatusCode = 200
    //            };
    //        }
    //        else if (IsAjaxRequest(context.HttpContext.Request))
    //        {
    //            string referer = context.HttpContext.Request.Headers["Referer"];
    //            if (!string.IsNullOrEmpty(referer))
    //            {
    //                referer = Uri.EscapeDataString(referer);
    //            }
    //            var url = $"{IsHost.SiteHost_User}/login/?return={referer}";
    //            context.Result = new JsonResult(new { status = 2, errorDescription = "未登录", url });
    //            //context.Result = new UnauthorizedObjectResult(url);
    //        }
    //        else
    //        {
    //            string url = "//" + context.HttpContext.Request.Host + context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
    //            context.Result = new RedirectResult($"{IsHost.SiteHost_User}/login/?return=" + Uri.EscapeDataString(url));
    //        }
    //    }

    //    /// <summary>
    //    /// from https://stackoverflow.com/questions/29282190/where-is-request-isajaxrequest-in-asp-net-core-mvc
    //    /// </summary>
    //    /// <param name="request"></param>
    //    /// <returns></returns>
    //    public static bool IsAjaxRequest(HttpRequest request)
    //    {
    //        if (request == null)
    //            throw new ArgumentNullException("request");

    //        if (request.Headers != null)
    //            return request.Headers["X-Requested-With"] == "XMLHttpRequest";

    //        return false;
    //    }
    //    public bool IsJsonpRequest(HttpRequest request)
    //    {
    //        return request.Query["callback"].Count > 0;
    //    }
    //}


    //public class AuthorizeResponseResultAttribute : AuthorizeAttribute
    //{
    //    readonly bool throwIfNoQx;

    //    public AuthorizeResponseResultAttribute() : this(false) { }

    //    public AuthorizeResponseResultAttribute(bool throwIfNoQx)
    //    {
    //        this.throwIfNoQx = throwIfNoQx;
    //    }

    //    public override void OnActionExecuting(ActionExecutingContext context)
    //    {
    //        // [AllowAnonymous] or IGoThroughMvcFilter
    //        var uncheck = context.ActionDescriptor.FilterDescriptors.Any(a =>
    //        {
    //            var fty = a.Filter.GetType();
    //            return fty == typeof(Microsoft.AspNetCore.Mvc.Authorization.AllowAnonymousFilter) ||
    //                typeof(IGoThroughMvcFilter).IsAssignableFrom(fty);
    //        });

    //        if (uncheck)
    //        {
    //            //业务逻辑
    //            base.OnActionExecuting(context);
    //            return;
    //        }

    //        // 已登录用户
    //        if (context.HttpContext.User.Identity.IsAuthenticated)
    //        {
    //            //业务逻辑
    //            base.OnActionExecuting(context);
    //            return;
    //        }

    //        UnAuthorizeHandle(context);
    //    }

    //    /// <summary>
    //    /// 处理未登录
    //    /// </summary>
    //    /// <param name="context"></param>
    //    public void UnAuthorizeHandle(ActionExecutingContext context)
    //    {
    //        var config = context.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
    //        var IsHost = config.GetSection("IsHost").Get<IsHost>();

    //        var result = new ResponseResult() { status = ResponseCode.NoLogin, Succeed = false, Msg= "你还未登录，请登录后再来吧" };
    //        if (IsJsonpRequest(context.HttpContext.Request))
    //        {
    //            var url = $"{IsHost.SiteHost_User}/login/?return={Uri.EscapeDataString(context.HttpContext.Request.Headers["Referer"])}";
    //            result.Data = new { loginUrl = url };
    //            var callback = context.HttpContext.Request.Query["callback"][0];
    //            Newtonsoft.Json.JsonSerializerSettings js = new Newtonsoft.Json.JsonSerializerSettings();
    //            js.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
    //            context.Result = new ContentResult()
    //            {
    //                Content = (callback ?? "callback") + "(" + Newtonsoft.Json.JsonConvert.SerializeObject(result, js) + ");",
    //                ContentType = "text/html",
    //                StatusCode = 200
    //            };
    //        }
    //        else if (IsAjaxRequest(context.HttpContext.Request))
    //        {
    //            string referer = context.HttpContext.Request.Headers["Referer"];
    //            if (!string.IsNullOrEmpty(referer))
    //            {
    //                referer = Uri.EscapeDataString(referer);
    //            }
    //            var url = $"{IsHost.SiteHost_User}/login/?return={referer}";
    //            result.Data = new { loginUrl = url };

    //            context.Result = new JsonResult(result);
    //        }
    //        else
    //        {
    //            string url = "//" + context.HttpContext.Request.Host + context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
    //            context.Result = new RedirectResult($"{IsHost.SiteHost_User}/login/?return=" + Uri.EscapeDataString(url));
    //        }
    //    }
    //}

    /// <summary>
    /// 用于跳过mvc filter处理
    /// </summary>
    public interface IGoThroughMvcFilter : IFilterMetadata
    {
    }
}
