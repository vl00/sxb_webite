using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PMS.Infrastructure.Application.IService;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Entities;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;

namespace Sxb.UserCenter.Controllers
{
    public class VerifyController : Base
    {
        ICityInfoService _cityInfoService;
        private readonly IEasyRedisClient _easyRedisClient;
        private IAccountService _account;
        public IsHost IsHost { get; }
        private IVerifyService _verifyService { get; set; }
        public VerifyController(IAccountService account,
            IOptions<IsHost> _isHost,
            IVerifyService verifyService,
            IEasyRedisClient easyRedisClient,
            ICityInfoService cityInfoService)
        {
            _account = account;
            IsHost = _isHost.Value;
            _verifyService = verifyService;
            _easyRedisClient = easyRedisClient;
            _cityInfoService = cityInfoService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Senior()
        {
            return View(_verifyService.CheckSeniorCondition(userID, Request.Headers["Cookie"]));
        }
        public IActionResult Official()
        {
            return View(_verifyService.CheckOfficialCondition(userID, Request.Headers["Cookie"]));
        }
        public IActionResult Form()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Form(  Verify model, bool isInvite=false)
        {
            PMS.UserManage.Domain.Common.RootModel json = new PMS.UserManage.Domain.Common.RootModel();
            model.UserID = userID;
            if (model.verifyType==1 && _verifyService.GetVerifies(userID).Select(a=>a.verifyType==1).Count()>0)
            {
                json.status = 1;
                json.errorDescription = "您已提交过此类认证";
                return Json(json);
            }

            if(model.verifyType==1)
            {
                var info = _verifyService.CheckSeniorCondition(userID, Request.Headers["Cookie"]);
                if(isInvite || (info.isPublishReach && info.isPublishReach && info.isBindMobile))
                {
                    if (isInvite)
                    {
                        model.IdType = 99;
                        model.IdNumber = "0";
                    }
                    if (!_verifyService.SubmitVerify(model))
                    {
                        json.status = 1;
                        json.errorDescription = "提交失败";
                    }
                }
                else
                {
                    json.status = 1;
                    json.errorDescription = "未满足必要条件";
                }
            }
            else
            {
                json.status = 1;
                json.errorDescription = "未满足必要条件";
            }
            return Json(json);
        }

        [AllowAnonymous]
        public IActionResult Invite()
        {
            ViewBag.ProvinceList = _cityInfoService.GetLocalList(0);
            RSAKeyPair keyPair = RSAHelper.GenerateRSAKeyPair();
            string privateKey = keyPair.PrivateKey;
            Guid _kid = Guid.NewGuid();
            _easyRedisClient.AddAsync(_kid.ToString(), privateKey, new TimeSpan(0,0,0, 300));
            PMS.UserManage.Application.ModelDto.Login.Get model = new PMS.UserManage.Application.ModelDto.Login.Get()
            {
                PublicKey = keyPair.PublicKey,
                Kid = _kid.ToString(),
            };
            return View(model);
        }
        public IActionResult InviteSubmit(string authType)
        {
            if (_verifyService.GetVerifies(userID).Select(a => a.verifyType == 1).Count() > 0)
            {
                return View("~/Views/Shared/Prompt.cshtml", ("您已提交过此类认证", IsHost.SiteHost_M));
            }
            UserInfo model = _account.GetUserInfo(userID);
            if (authType == "company")
                return View("~/Views/Verify/InviteSubmit.cshtml", ("Company", model));
            else if (authType == "personal")
                return View("~/Views/Verify/InviteSubmit.cshtml", ("Personal", model));
            else
                return RedirectToAction("Invite");
        }
        [HttpPost]
        public IActionResult InviteSubmit(string authType, Verify model, string companyName)
        {
            if(authType== "company")
            {
                model.RealName = companyName + "-" + model.RealName;
            }
            model.verifyType = 1;
            model.intro2 = null;
            return Form(model, true);
        }
    }
}