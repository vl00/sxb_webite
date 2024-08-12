using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver.Core.Operations;
using Newtonsoft.Json;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Entities;
using ProductManagement.API.Http.Interface;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using ProductManagement.UserCenter.TencentCommon;
using ProductManagement.UserCenter.TencentCommon.Model;
using ProductManagement.UserCenter.Wx;
using ProductManagement.UserCenter.Wx.Model;
using Sxb.UserCenter.Models.AccountViewModel;
using Sxb.UserCenter.Response;
using Sxb.UserCenter.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sxb.UserCenter.Controllers
{
    public class ApiAccountController : Base
    {
        private readonly WeixinUtil _weixinUtil;
        private readonly QQUtil _qqUtil;
        private readonly IAccountService _accountService;
        private readonly IOAuthWeixinService _OAuthWeixinService;
        private readonly IOAuthQQService _OAuthQQService;
        private readonly AppInfos _AppInfos;
        private readonly IsHost _isHost;
        IUserService _userService;

        private readonly IEasyRedisClient _easyRedisClient;
        private readonly ILogger<ApiAccountController> _logger;

        public ApiAccountController(IOptions<AppInfos> _appInfos,
            IAccountService account,
            IOAuthWeixinService OAuthWeixinService,
            IOAuthQQService OAuthQQService,
            IEasyRedisClient easyRedisClient,
            IOptions<IsHost> _isHost, ILogger<ApiAccountController> logger,
            IUserService userService)
        {
            _userService = userService;
            _AppInfos = _appInfos.Value;
            _accountService = account;
            _OAuthWeixinService = OAuthWeixinService;
            _OAuthQQService = OAuthQQService;
            _easyRedisClient = easyRedisClient;
            this._isHost = _isHost.Value;

            _qqUtil = new QQUtil(new HttpClient());
            _weixinUtil = new WeixinUtil(new HttpClient());
            _logger = logger;
        }

        /// <summary>
        /// 账号绑定详情
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ResponseResult Index()
        {
            AccountBind accountBind = new AccountBind()
            {
                UserID = userID
            };
            var bind = _accountService.GetBindInfo(userID);

            if (!string.IsNullOrWhiteSpace(bind.Mobile))
            {
                accountBind.IsBindPhone = true;
            }

            if (!string.IsNullOrWhiteSpace(bind.UnionID_Weixin))
            {
                accountBind.IsBindWX = true;
            }

            if (bind.UnionID_QQ != null)
            {
                accountBind.IsBindQQ = true;
            }
            bind.UnionID_QQ = null;
            bind.UnionID_Weixin = null;
            accountBind.BindInfo = bind;

            #region CheckWXBind
            var wxUserinfo = _easyRedisClient.GetAsync<WXUserInfoResult>("WXAuth:WXBindInfo-" + userID).Result;
            if (wxUserinfo != null)
            {
                accountBind.IsBindSuccess = false;
                if (_OAuthWeixinService.CheckWXCanBind(wxUserinfo.unionid, userID))
                {
                    bool success = _OAuthWeixinService.BindWX(wxUserinfo.unionid, wxUserinfo.openid, "", wxUserinfo.AppName, wxUserinfo.AppId, userID, wxUserinfo.nickname, wxUserinfo.OpenIdSubscribeStatus);
                    if (success) { _ = _easyRedisClient.AddAsync($"WXUserInfo:UserID_{userID}", wxUserinfo, TimeSpan.FromDays(1)).Result; }
                    accountBind.IsBindSuccess = success;
                }
                else
                {
                    _easyRedisClient.AddAsync($"BindConflict:{userID}", new KeyValue()
                    {
                        Key = "WX",
                        Value = $"{wxUserinfo.unionid}|{wxUserinfo.nickname}"
                    }, TimeSpan.FromMinutes(1));
                }
                if (!wxUserinfo.ForceBind) _easyRedisClient.RemoveAsync("WXAuth:WXBindInfo-" + userID);
            }
            else
            {
                var isBindSuccess = _easyRedisClient.GetStringAsync("WXAuth:WXIsBindSuccess-" + userID);
                if (!string.IsNullOrWhiteSpace(isBindSuccess.Result))
                {
                    _easyRedisClient.RemoveAsync("WXAuth:WXIsBindSuccess-" + userID);
                    accountBind.IsBindSuccess = isBindSuccess.Result == "true";
                }
            }
            #endregion

            return ResponseResult.Success(accountBind);
        }


        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="kid"></param>
        /// <param name="password"></param>
        /// <param name="rnd"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult ChangePSW(Guid kid, string password, string rnd, string mobile, short nationCode = 86)
        {
            if (User.Identity.IsAuthenticated)
            {
                var info = _accountService.GetUserInfo(userID, false);
                if (info == null || info.Id == default) return ResponseResult.Failed("invalid request");
                if (string.IsNullOrEmpty(info.Mobile)) return ResponseResult.Failed("请先绑定手机号");
                if (info.NationCode != nationCode || info.Mobile != mobile) return ResponseResult.Failed("手机号或验证码错误");
            }

            //PMS.UserManage.Domain.Common.RootModel json = new PMS.UserManage.Domain.Common.RootModel();
            if (!_accountService.ChangePSW(kid, nationCode, mobile, password, rnd, out string failResult))
            {
                return ResponseResult.Failed(failResult);
            }
            return ResponseResult.Success();
        }

        /// <summary>
        /// 获取公钥
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResponseResult GetPublicKey_Kid()
        {
            string keyId = "", publicKey = "";
            RSAKeyPair keyPair = RSAHelper.GenerateRSAKeyPair();
            publicKey = keyPair.PublicKey;
            string privateKey = keyPair.PrivateKey;
            Guid _kid = Guid.NewGuid();
            if (_easyRedisClient.AddAsync($"privateKey:{_kid}", privateKey, new TimeSpan(0, 0, 0, 300)).Result)
            {
                keyId = _kid.ToString();
            }

            return ResponseResult.Success(new { keyId, publicKey });
        }

        /// <summary>
        /// 手机号绑定
        /// </summary>
        /// <param name="kid"></param>
        /// <param name="mobile_o"></param>
        /// <param name="mobile_n"></param>
        /// <param name="rnd"></param>
        /// <param name="nationCode_o"></param>
        /// <param name="nationCode_n"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ResponseResult BindMobile(Guid kid, string mobile_o, string mobile_n, string rnd, string rnd_o, short nationCode_o = 86, short nationCode_n = 86)
        {
            PMS.UserManage.Application.ModelDto.Account.Conflict conflict = new PMS.UserManage.Application.ModelDto.Account.Conflict();

            if (!_accountService.BindMobile(userID, kid, nationCode_o, mobile_o, nationCode_n, mobile_n, rnd, rnd_o, out string failResult, out int status))
            {
                var privateResult = _easyRedisClient.GetAsync<string>($"privateKey:{kid.ToString()}");
                if (string.IsNullOrWhiteSpace(privateResult?.Result) == false)
                {
                    var mobile = RSAHelper.RSADecrypt(privateResult.Result, mobile_n);
                    _easyRedisClient.AddAsync($"BindConflict:{userID}", new KeyValue()
                    {
                        Key = "Phone",
                        Value = $"{nationCode_n}|{mobile}"
                    }, TimeSpan.FromMinutes(1));
                }
                conflict.status = status;
                conflict.errorDescription = failResult;
                conflict.dataType = 0;
                return ResponseResult.Failed(conflict);
            }
            return ResponseResult.Success(conflict);
        }

        /// <summary>
        /// 账号冲突详情
        /// </summary>
        /// <param name="kid"></param>
        /// <param name="data"></param>
        /// <param name="rnd"></param>
        /// <param name="dataType">0:手机号/1:微信/2:QQ</param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult Conflict(string data, string rnd, byte dataType, Guid kid = default, short nationCode = 86, bool removeRndCache = true, string errorMessage = default)
        {
            var a = _accountService.ListConflict(userID, kid, nationCode, ref data, rnd, dataType, removeRndCache);
            if (a.status == 2001)
            {
                a.myInfo = _accountService.GetUserInfo(userID);
            }
            if (string.IsNullOrWhiteSpace(a.errorDescription)) a.errorDescription = errorMessage;
            return a.status == 0 ? ResponseResult.Success(a, errorMessage) :
                new ResponseResult()
                {
                    Data = a,
                    Msg = errorMessage,
                    status = ResponseCode.Failed,
                    Succeed = false
                };
        }

        /// <summary>
        /// 微信绑定
        /// </summary>
        /// <param name="confirm"></param>
        /// <param name="ReturnUrl"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> BindWX(bool confirm = false, string ReturnUrl = "/mine/account-setting/")
        {
            PMS.UserManage.Application.ModelDto.Account.Conflict conflict = new PMS.UserManage.Application.ModelDto.Account.Conflict();

            string appName = "web";
            if (Request.Headers["User-Agent"].ToString().ToLower().Contains("micromessenger"))
            {
                appName = "fwh";
            }
            else if (Request.Headers["User-Agent"].ToString().ToLower().Contains("ischool"))
            {
                appName = "app";
            }
            var WXuserinfo = _easyRedisClient.GetAsync<WXUserInfoResult>("WXAuth:WXBindInfo-" + userID).Result;
            if (WXuserinfo == null)
            {
                //缓存中取不到微信用户数据，调整至/ApiLogin/WXAuth获取，并存入缓存
                string state = Guid.NewGuid().ToString("N");
                string redirect_uri = _isHost.SiteHost_User + "/ApiLogin/WXAuth?ReturnUrl=";
                if (Request.Headers["User-Agent"].ToString().ToLower().Contains("micromessenger"))
                {
                    redirect_uri += Uri.EscapeDataString(Request.Path + Request.QueryString);
                    //redirect_uri += Uri.EscapeDataString(ReturnUrl);
                    conflict.data = _weixinUtil.GetLoginURL(redirect_uri, _AppInfos.Weixin.AppID, "snsapi_userinfo", state);
                }
                else
                {
                    if (confirm) ReturnUrl = "/ApiAccount/BindWX?confirm=true";
                    redirect_uri += Uri.EscapeDataString(ReturnUrl);
                    conflict.data = _weixinUtil.GetWebLoginUrl(redirect_uri, _AppInfos.Weixin_Web.AppID, state);
                }
                await _easyRedisClient.AddAsync("WXAuth:wx_redirect-" + state, redirect_uri, new TimeSpan(0, 0, 0, 600));

                //链接跳转
                conflict.status = 99;
                return Json(ResponseResult.Success(conflict), null);
            }
            else
            {
                var unionIDCanBind = _OAuthWeixinService.CheckWXCanBind(WXuserinfo.unionid, userID);
                var oldUserIDs = new List<Guid>();
                if (confirm || unionIDCanBind)
                {
                    #region 如无其他绑定和手机号则注销该帐号
                    if (!unionIDCanBind)
                    {
                        var conflictUserInfos = _OAuthWeixinService.ListWXConflict(WXuserinfo.unionid, null);
                        if (conflictUserInfos?.Any() == true)
                        {
                            oldUserIDs.AddRange(conflictUserInfos.Select(p => p.Id).Distinct());
                            var conflictUserInfo = conflictUserInfos.First();
                            if (conflictUserInfo.Id != default)
                            {
                                var bindInfo = _accountService.GetBindInfo(conflictUserInfo.Id);
                                if (bindInfo != null)
                                {
                                    if (string.IsNullOrWhiteSpace(bindInfo.Mobile) && !bindInfo.UnionID_QQ.HasValue)
                                    {
                                        await _userService.RemoveUser(conflictUserInfo.Id);
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    bool success = _OAuthWeixinService.BindWX(WXuserinfo.unionid, WXuserinfo.openid, "", WXuserinfo.AppName, WXuserinfo.AppId, userID, WXuserinfo.nickname, WXuserinfo.OpenIdSubscribeStatus);
                    if (success)
                    {
                        await _easyRedisClient.AddAsync($"WXUserInfo:UserID_{userID}", WXuserinfo, TimeSpan.FromDays(1));
                        //替换openID原有userID
                        if (oldUserIDs?.Any() == true)
                        {
                            foreach (var oldUserID in oldUserIDs)
                            {
                                await _OAuthWeixinService.ChangeOpenIDUser(oldUserID, userID);
                            }
                        }
                    }
                    //绑定成功
                    await _easyRedisClient.RemoveAsync("WXAuth:WXBindInfo-" + userID);

                    string isBindSuccess = success ? "true" : "false";
                    await _easyRedisClient.AddStringAsync("WXAuth:WXIsBindSuccess-" + userID, isBindSuccess, TimeSpan.FromMinutes(1));
                    return Redirect(ReturnUrl);
                }
            }
            await _easyRedisClient.AddAsync($"BindConflict:{userID}", new KeyValue()
            {
                Key = "WX",
                Value = $"{WXuserinfo.unionid}|{WXuserinfo.nickname}"
            }, TimeSpan.FromMinutes(1));
            if (appName == "fwh")
            {
                return Redirect(ReturnUrl);
            }
            else
            {
                return Json(Conflict(null, null, 1), null);
            }
        }

        /// <summary>
        /// 微信绑定（小程序）
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<ResponseResult> BindWXProgram(string code, string mp, string nickName, bool confirm)
        {
            mp ??= "sxkid";
            string weixin_AppID = _AppInfos.Weixin_MiniProgram_Sxkid.AppID;
            string weixin_AppName = _AppInfos.Weixin_MiniProgram_Sxkid.AppName;
            string appsecret = _AppInfos.Weixin_MiniProgram_Sxkid.AppSecret;
            switch (mp.ToLower())
            {
                case "lecture":
                    weixin_AppID = _AppInfos.Weixin_MiniProgram_Lecture.AppID;
                    weixin_AppName = _AppInfos.Weixin_MiniProgram_Lecture.AppName;
                    appsecret = _AppInfos.Weixin_MiniProgram_Lecture.AppSecret;
                    break;
                case "org":
                    weixin_AppID = _AppInfos.Weixin_MiniProgram_Org.AppID;
                    weixin_AppName = _AppInfos.Weixin_MiniProgram_Org.AppName;
                    appsecret = _AppInfos.Weixin_MiniProgram_Org.AppSecret;
                    break;
            }
            var AuthInfo = await _weixinUtil.GetWeiXinMiniProgramAuth(weixin_AppID, appsecret, code);
            _logger.LogInformation("参数:" + code + "," + mp + "," + nickName + "," + confirm);
            _logger.LogInformation("AuthInfo:" + weixin_AppID + "," + weixin_AppName + "," + appsecret + "," + JsonConvert.SerializeObject(AuthInfo));
            if (AuthInfo?.Errcode == 0)
            {
                var unionIDCanBind = _OAuthWeixinService.CheckWXCanBind(AuthInfo.Unionid, userID);
                if (unionIDCanBind || confirm)
                {
                    #region 如无其他绑定和手机号则注销该帐号
                    if (!unionIDCanBind)
                    {
                        var conflictUserInfos = _OAuthWeixinService.ListWXConflict(AuthInfo.Unionid, null);
                        if (conflictUserInfos?.Any() == true)
                        {
                            var conflictUserInfo = conflictUserInfos.First();
                            if (conflictUserInfo.Id != default)
                            {
                                var bindInfo = _accountService.GetBindInfo(conflictUserInfo.Id);
                                if (bindInfo != null)
                                {
                                    if (string.IsNullOrWhiteSpace(bindInfo.Mobile) && !bindInfo.UnionID_QQ.HasValue)
                                    {
                                        await _userService.RemoveUser(conflictUserInfo.Id);
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    bool success = _OAuthWeixinService.BindWX(AuthInfo.Unionid, AuthInfo.OpenID, AuthInfo.Session_Key, weixin_AppName, weixin_AppID, userID, nickName);

                    if (success)
                    {
                        return ResponseResult.Success("绑定成功");
                    }
                }
                else
                {
                    await _easyRedisClient.AddAsync($"BindConflict:{userID}", new KeyValue()
                    {
                        Key = "WX",
                        Value = $"{AuthInfo.Unionid}|{nickName}"
                    }, TimeSpan.FromMinutes(1));
                    return ResponseResult.Failed("绑定冲突");
                }
            }
            return ResponseResult.Failed("绑定失败");
        }

        /// <summary>
        /// qq绑定
        /// </summary>
        /// <param name="confirm"></param>
        /// <returns></returns>
        [Authorize]
        public IActionResult BindQQ(bool confirm = true, string ReturnUrl = "/mine/account-setting/")
        {
            string appName = "web";
            if (Request.Headers["User-Agent"].ToString().ToLower().Contains("ischool"))
            {
                appName = "app";
            }
            var OpenIDInfo = _easyRedisClient.GetAsync<QQOpenID>("QQAuth:QQBindInfo-" + userID).Result;
            if (OpenIDInfo == null)
            {
                string redirect_uri = _isHost.SiteHost_User + "/ApiLogin/QQAuth?redirect_uri=" +
                    Uri.EscapeDataString(Request.Path + Request.QueryString);

                string state = Guid.NewGuid().ToString("N");
                _easyRedisClient.AddAsync("QQAuth:qq_redirect-" + state, redirect_uri, new TimeSpan(0, 0, 0, 600));
                string url = _qqUtil.GetWebLoginURL(redirect_uri, state: state);

                return Json(ResponseResult.Success(new PMS.UserManage.Domain.Common.RootModel()
                {
                    status = 99,
                    data = url
                }), null);
            }
            else if (_OAuthQQService.BindQQ(userID, Guid.Parse(OpenIDInfo.unionid), Guid.Parse(OpenIDInfo.openid), confirm, appName))
            {
                if (IsAjaxRequest())
                    return Json(ResponseResult.Success(new PMS.UserManage.Domain.Common.RootModel()), null);
                _easyRedisClient.AddStringAsync("QQAuth:QQIsBindSuccess-" + userID, "true", TimeSpan.FromMinutes(1));
                return Redirect(ReturnUrl);
            }
            return Redirect(ReturnUrl);
            //return Json(Conflict(null, null, 2));
        }



        [Description("退出登录")]
        [Authorize]
        public async Task<ResponseResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Response.SetUserId("");
            return ResponseResult.Success();
        }


        /// <summary>
        /// 当前账号进行解绑
        /// </summary>
        /// <returns></returns>
        [Description("解绑微信")]
        [Authorize]
        public ResponseResult UnboundWx(bool confirm = false)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return ResponseResult.Failed("未登录");
            }
            Guid userID = User.Identity.GetUserInfo().UserId;


            #region 检测帐号绑定
            var bindInfo = _accountService.GetBindInfo(userID);
            if (bindInfo != null)
            {
                if (string.IsNullOrWhiteSpace(bindInfo.Mobile) && !bindInfo.UnionID_QQ.HasValue)
                {
                    return ResponseResult.Failed("检测到该账号已无其他帐号关联凭证，故无法继续解绑操作!");
                }
            }
            #endregion
            if (confirm)
            {
                bool IsSuccess = _OAuthWeixinService.UnboundWx(userID);
                return ResponseResult.Success(new { IsSuccess });
            }
            return ResponseResult.Success("检测到该账号存在其他帐号关联凭证，可以进行解绑操作!");
        }

        [Description("解绑QQ")]
        [Authorize]
        public ResponseResult UnboundQQ(bool confirm = false)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return ResponseResult.Failed("未登录");
            }
            Guid userID = User.Identity.GetUserInfo().UserId;

            #region 检测帐号绑定
            var bindInfo = _accountService.GetBindInfo(userID);
            if (bindInfo != null)
            {
                if (string.IsNullOrWhiteSpace(bindInfo.Mobile) && string.IsNullOrWhiteSpace(bindInfo.UnionID_Weixin))
                {
                    return ResponseResult.Failed("检测到该账号已无其他帐号关联凭证，故无法继续解绑操作!");
                }
                else
                {
                    if (confirm)
                    {
                        bool IsSuccess = _OAuthQQService.UnboundQQ(userID);
                        return ResponseResult.Success(new { IsSuccess });
                    }
                }
            }
            #endregion

            return ResponseResult.Success("检测到该账号存在其他帐号关联凭证，可以进行解绑操作!");
        }

        /// <summary>
        /// 检查当前登录用户是否已经绑定手机
        /// </summary>
        /// <returns></returns>
        public ResponseResult CheckIsBoundPhone()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return ResponseResult.Failed("未登录");
            }
            var isBindPhone = _accountService.IsBindPhone(userID);

            return ResponseResult.Success(new { Result = isBindPhone });
        }

        /// <summary>
        /// 绑定冲突详情
        /// </summary>
        /// <returns></returns>
        public ResponseResult GetConflictDetail()
        {
            if (userID == default)
            {
                return ResponseResult.Failed("未登录");
            }

            var key = $"BindConflict:{userID}";

            var conflictTask = _easyRedisClient.GetAsync<KeyValue>(key);

            if (conflictTask?.Result == null)
            {
                return ResponseResult.Failed("请求错误");
            }

            var conflictInfo = conflictTask.Result;

            var userInfo = new List<UserInfo>();

            switch (conflictInfo.Key)
            {
                case "Phone":
                    var splitPhone = conflictInfo.Value.Split('|');
                    if (splitPhone?.Length > 1)
                    {
                        if (short.TryParse(splitPhone.First(), out short nationCode))
                        {
                            userInfo = _accountService.ListMobileConflict(nationCode, splitPhone.Last());
                        }
                    }
                    break;
                case "WX":
                    var splitWX = conflictInfo.Value.Split('|');
                    if (splitWX?.Length > 1)
                    {
                        userInfo = _OAuthWeixinService.ListWXConflict(splitWX.First(), null);
                        conflictInfo.Value = splitWX.Last();
                    }
                    break;
            }

            if (userInfo?.Any() == true)
            {
                var targetUserInfo = userInfo.First();
                var selfUserInfo = _accountService.GetUserInfo(userID);



                return ResponseResult.Success(new
                {
                    targetName = targetUserInfo.NickName,
                    targetHeadImg = targetUserInfo.HeadImgUrl,
                    selfUserName = selfUserInfo.NickName,
                    selfHeadImg = selfUserInfo.HeadImgUrl,
                    conflictType = conflictInfo.Key
                    //conflictValue = conflictInfo.Value
                });
            }
            return ResponseResult.Failed("非法请求");
        }
    }
}