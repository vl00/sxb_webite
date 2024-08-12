using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using PMS.UserManage.Domain.Common;
using PMS.UserManage.Application.IServices;
using Sxb.UserCenter.Response;
using Microsoft.Extensions.Configuration;

namespace Sxb.UserCenter.Controllers
{
    /// <summary>
    /// 首页控制器
    /// </summary>
    [AllowAnonymous]
    public class HomeController : Base
    {
        public IsHost IsHost { get; }
        private IHomeService _homeService { get; set; }
        public HomeController(IOptions<IsHost> _isHost, IHomeService homeService)
        {
            IsHost = _isHost.Value;
            _homeService = homeService;
        }
        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            //ViewBag.Files = System.IO.Directory.GetFiles(@"\\10.1.0.16\shared-auth-ticket-keys");
            if (User.Identity.IsAuthenticated)
            {
                string cookieStr = Request.Headers["Cookie"];
                string uuid = Request.Cookies["uuid"];
                PMS.UserManage.Application.ModelDto.Home.Index model = _homeService.GetUserCenterHomeInfo(userID, cookieStr, uuid);
                return iSchoolResult(model);
            }
            else
            {
                if (IsMobile())
                {
                    PMS.UserManage.Application.ModelDto.Home.Index model = new PMS.UserManage.Application.ModelDto.Home.Index();
                    return iSchoolResult(model);
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }
        }
        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// 检测是否登录
        /// </summary>
        /// <returns></returns>
        public ResponseResult CheckLogin()
        {
            var config = HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
            var userServiceUrl = config.GetSection("UserSystemConfig").GetValue<string>("ServerUrl");

            bool IsLogin = User.Identity.IsAuthenticated;
            return ResponseResult.Success(new { IsLogin, LoginUrl = userServiceUrl + "/login?ReturnUrl=" });
        }

    }
}
