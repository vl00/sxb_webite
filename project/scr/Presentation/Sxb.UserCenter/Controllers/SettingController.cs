using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMS.Infrastructure.Application.IService;
using PMS.UserManage.Application.IServices;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;

namespace Sxb.UserCenter.Controllers
{
    [AllowAnonymous]
    public class SettingController : Base
    {

        private readonly IEasyRedisClient _easyRedisClient;
        VerifyCodeHelper verifyCodeHelper;
        ICityInfoService _cityInfoService;
        public SettingController(IEasyRedisClient easyRedisClient, ICityInfoService cityInfoService)
        {
            _cityInfoService = cityInfoService;
            _easyRedisClient = easyRedisClient;
            verifyCodeHelper = new VerifyCodeHelper(_easyRedisClient);
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult NationCodeSelect()
        {
            return View();
        }
        public IActionResult VerifyCode()
        {
            if(Guid.TryParse(Request.Cookies["verifycode"], out Guid OcodeID))
            {
                _easyRedisClient.RemoveAsync("verifyCode-" + OcodeID);
            }
            System.IO.MemoryStream ms = verifyCodeHelper.Create(out Guid codeID, out string code, VerifyCodeHelper.VerifyCodeType.Number);
            Response.Body.Dispose();
            string domain = ".sxkid.com";
            Response.Cookies.Append("verifycode", codeID.ToString(), new CookieOptions() { Expires = DateTime.Now.AddMinutes(5), Path = "/", Domain = domain, SameSite = SameSiteMode.Lax });
            if (!IsAjaxRequest())
            {
                return File(ms.ToArray(), @"image/png");
            }
            else
            {
                return Json(new {
                    img = Convert.ToBase64String(ms.ToArray()),
                    verifycode = codeID
                });
            }
        }
        public IActionResult CheckVerifyCode(string code)
        {
            Guid.TryParse(Request.Cookies["verifycode"], out Guid codeID);
            return Json(new { status = !codeID.Equals(Guid.Empty) && verifyCodeHelper.Check(codeID, code, false) ? 0 : 1 });
        }
        public IActionResult GetLocalList(int parent=0)
        {
            PMS.UserManage.Domain.Common.RootModel json = new PMS.UserManage.Domain.Common.RootModel();
            json.data = _cityInfoService.GetLocalList(parent);
            return Json(json);
        }
    }
}