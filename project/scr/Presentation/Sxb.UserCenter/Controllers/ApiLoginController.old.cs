using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Dtos;
using PMS.UserManage.Domain.Entities;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.SMS;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using ProductManagement.UserCenter.TencentCommon;
using ProductManagement.UserCenter.TencentCommon.Model;
using ProductManagement.UserCenter.Wx;
using ProductManagement.UserCenter.Wx.Model;
using Sxb.UserCenter.Request.Login;
using Sxb.UserCenter.Response;
using Sxb.UserCenter.Utils;
using Sxb.UserCenter.ViewModel;

namespace Sxb.UserCenter.Controllers
{
    [AllowAnonymous]
    public class ApiLoginControllerBackup : Base
    {
        private ITalentService _talentService;

        private ILoginService _loginService;
        private VerifyCodeHelper _verifyCode;
        private IVerifyService _verifyService;
        private IAccountService _account;
        private WeixinUtil weixinUtil;
        private QQUtil _QQUtil;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly SMSHelper sMSHelper;
        private readonly IWeChatAppClient _WeChatAppClient;
        private readonly IOAuthBaiduService _baiduOAuthService;

        private readonly IHttpContextAccessor httpContextAccessor;
        public AppInfos AppInfos { get; }
        public IsHost IsHost { get; }
        public ApiLoginOldController(
            ITalentService talentService,
            IOptions<AppInfos> _appInfos,
            IOptions<IsHost> _isHost,
            IHttpContextAccessor _httpContextAccessor,
            IEasyRedisClient easyRedisClient,
            IVerifyService verifyService,
            ILoginService loginService,
            IAccountService account,
            IWeChatAppClient weChatAppClient,
            IOAuthBaiduService baiduOAuthService)
        {

            _talentService = talentService;
            _loginService = loginService;
            _verifyService = verifyService;
            sMSHelper = new SMSHelper(easyRedisClient);
            _account = account;
            _easyRedisClient = easyRedisClient;
            AppInfos = _appInfos.Value;
            IsHost = _isHost.Value;
            httpContextAccessor = _httpContextAccessor;
            weixinUtil = new WeixinUtil(new System.Net.Http.HttpClient());
            _QQUtil = new QQUtil(new System.Net.Http.HttpClient());
            _verifyCode = new VerifyCodeHelper(easyRedisClient);
            _WeChatAppClient = weChatAppClient;
            _baiduOAuthService = baiduOAuthService;
        }

        private async Task SetSignInCookie(UserInfo userInfo, TalentEntity talent)
        {
            if (userInfo.Id != Guid.Empty)
            {
                await JwtCookieHelper.SetSignInCookie(HttpContext, userInfo, talent);
                HttpContext.Response.SetUserId(userInfo.Id.ToString());
            }
            //if (!string.IsNullOrEmpty(userInfo.Mobile))
            //{
            //    userInfo.Mobile = CommonHelper.HideNumber(userInfo.Mobile);
            //}

            //bool isAuth = false;
            //if (talent != null) 
            //{
            //    if (talent.certification_status != null)
            //    {
            //        if (talent.certification_status == 1)
            //        {
            //            isAuth = true;
            //        }
            //    }
            //}


            //var claims = new List<Claim>() {
            //    new Claim(JwtClaimTypes.Name, userInfo.NickName),
            //    new Claim(JwtClaimTypes.Id, userInfo.Id.ToString()),
            //    new Claim(JwtClaimTypes.PhoneNumber, userInfo.Mobile??""),
            //    new Claim(JwtClaimTypes.Picture, userInfo.HeadImgUrl),
            //    new Claim(JwtClaimTypes.AccessTokenHash, Guid.NewGuid().ToString()),
            //    new Claim("isAuth",isAuth.ToString())
            //};
            ////List<byte> role = new List<byte>();
            ////foreach(var verify in verifies)
            ////{
            ////    if (verify.valid)
            ////    {
            ////        role.Add(verify.verifyType);
            ////    }
            ////}
            //if (talent != null)
            //{
            //    claims.Add(new Claim(JwtClaimTypes.Role, talent.type.ToString()));
            //}

            //var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, JwtClaimTypes.Name, JwtClaimTypes.Id);
            //identity.AddClaims(claims);
            //var princial = new ClaimsPrincipal(identity);
            //var properties = new AuthenticationProperties();
            //var ticket = new AuthenticationTicket(princial, properties, "CookieScheme");
            //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, princial);
        }
        public IActionResult Index(string ReturnUrl)
        {
            ViewBag.ReturnUrl = Uri.EscapeDataString(ReturnUrl ?? "");
            string state = Guid.NewGuid().ToString("N");
            string redirect_uri_wx = Request.Scheme + "://" + Request.Host + "/ApiLogin/WXAuth?ReturnUrl=" +
                Uri.EscapeDataString(ReturnUrl ?? "/mine/");
            string redirect_uri_qq = Request.Scheme + "://" + Request.Host + "/ApiLogin/QQAuth?ReturnUrl=" +
                Uri.EscapeDataString(ReturnUrl ?? "/mine/");

            _easyRedisClient.AddAsync("wx_redirect-" + state, redirect_uri_wx, new TimeSpan(0, 0, 0, 600));
            _easyRedisClient.AddAsync("qq_redirect-" + state, redirect_uri_qq, new TimeSpan(0, 0, 0, 600));
            if (!IsAjaxRequest())
            {
                if (User.Identity.IsAuthenticated)
                {
                    return Redirect(ReturnUrl ?? "/");
                }
                if (Request.Headers["User-Agent"].ToString().ToLower().Contains("micromessenger"))
                {
                    string appid = AppInfos.Weixin.AppID;
                    ViewBag.Login_Uri_WX = weixinUtil.GetLoginURL(redirect_uri_wx, appid, "snsapi_userinfo", state);
                }
                else
                {
                    string appid = AppInfos.Weixin_Web.AppID;
                    ViewBag.Login_Uri_WX = weixinUtil.GetWebLoginUrl(redirect_uri_wx, appid, state);
                }
                ViewBag.Login_Uri_QQ = _QQUtil.GetWebLoginURL(redirect_uri_qq, state: state);
            }
            RSAKeyPair keyPair = RSAHelper.GenerateRSAKeyPair();
            string privateKey = keyPair.PrivateKey;
            Guid _kid = Guid.NewGuid();
            _easyRedisClient.AddAsync($"privateKey:{_kid}", privateKey, new TimeSpan(0, 0, 0, 300));
            PMS.UserManage.Application.ModelDto.Login.Get model = new PMS.UserManage.Application.ModelDto.Login.Get()
            {
                PublicKey = keyPair.PublicKey,
                Kid = _kid.ToString(),
                State = state
            };
            return iSchoolResult(model);
        }

        public ResponseResult GetRSAkeyPair(string ReturnUrl)
        {
            //ViewBag.ReturnUrl = Uri.EscapeDataString(ReturnUrl ?? "");
            string state = Guid.NewGuid().ToString("N");
            //前端endcode url
            string redirect_uri_wx = Request.Scheme + "://" + Request.Host + "/ApiLogin/WXAuth?ReturnUrl=" +
               Uri.EscapeDataString(ReturnUrl ?? "/mine/");
            string redirect_uri_qq = Request.Scheme + "://" + Request.Host + "/ApiLogin/QQAuth?ReturnUrl=" +
               Uri.EscapeDataString(ReturnUrl ?? "/mine/");

            _easyRedisClient.AddAsync("wx_redirect-" + state, redirect_uri_wx, new TimeSpan(0, 0, 0, 600));
            _easyRedisClient.AddAsync("qq_redirect-" + state, redirect_uri_qq, new TimeSpan(0, 0, 0, 600));

            string Login_Uri_WX = "", Login_Uri_QQ = "";

            if (Request.Headers["User-Agent"].ToString().ToLower().Contains("micromessenger"))
            {
                string appid = AppInfos.Weixin.AppID;
                Login_Uri_WX = weixinUtil.GetLoginURL(redirect_uri_wx, appid, "snsapi_userinfo", state);
            }
            else
            {
                string appid = AppInfos.Weixin_Web.AppID;
                Login_Uri_WX = weixinUtil.GetWebLoginUrl(redirect_uri_wx, appid, state);
            }
            Login_Uri_QQ = _QQUtil.GetWebLoginURL(redirect_uri_qq, state: state);

            RSAKeyPair keyPair = RSAHelper.GenerateRSAKeyPair();
            string privateKey = keyPair.PrivateKey;
            Guid _kid = Guid.NewGuid();
            _easyRedisClient.AddAsync($"privateKey:{_kid}", privateKey, new TimeSpan(0, 0, 0, 300));
            PMS.UserManage.Application.ModelDto.Login.Get model = new PMS.UserManage.Application.ModelDto.Login.Get()
            {
                PublicKey = keyPair.PublicKey,
                Kid = _kid.ToString(),
                State = state,
                Login_Uri_QQ = Login_Uri_QQ,
                Login_Uri_WX = Login_Uri_WX,
                IsLogin = User.Identity.IsAuthenticated,
                RedirectUrl = ReturnUrl ?? "/mine/"
            };
            return ResponseResult.Success(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(Guid kid, short nationCode, string mobile, string password, string rnd, string ReturnUrl, int? city, string nickname = null)
        {
            var userModel = _loginService.Login(kid, nationCode, mobile, password, rnd, out string failResult, nickname, city, out bool mobileRepeatUser);
            if (userModel == null)
            {
                return Json(new PMS.UserManage.Domain.Common.RootModel() { status = 1, errorDescription = failResult });
            }



            //var verifyInfo = _verifyService.GetVerifies(userModel.Id);
            var talent = _talentService.GetTalentByUserId(userModel.Id.ToString());
            await SetSignInCookie(userModel, talent);
            _easyRedisClient.RemoveAsync($"privateKey:{kid}");
            PMS.UserManage.Application.ModelDto.Login.Post model = new PMS.UserManage.Application.ModelDto.Login.Post()
            {
                ID = userModel.Id,
                Nickname = userModel.NickName,
                HeadImgUrl = userModel.HeadImgUrl,
                ReturnUrl = ReturnUrl,
                MobileBinded = !string.IsNullOrWhiteSpace(userModel.Mobile),
                MoblieRepeatUser = mobileRepeatUser
            };
            return Json(model);
        }

        [HttpPost]
        public async Task<ResponseResult> MobileLogin(Guid kid, short nationCode, string mobile, string password, string rnd, string ReturnUrl, int? city, string nickname = null)
        {
            var userModel = _loginService.Login(kid, nationCode, mobile, password, rnd, out string failResult, nickname, city, out bool mobileRepeatUser);
            if (userModel == null)
            {
                return ResponseResult.Failed(failResult);
            }
            //var verifyInfo = _verifyService.GetVerifies(userModel.Id);
            var talent = _talentService.GetTalentByUserId(userModel.Id.ToString());
            await SetSignInCookie(userModel, talent);
            await _easyRedisClient.RemoveAsync($"privateKey:{kid}",StackExchange.Redis.CommandFlags.FireAndForget);
            PMS.UserManage.Application.ModelDto.Login.Post model = new PMS.UserManage.Application.ModelDto.Login.Post()
            {
                ID = userModel.Id,
                Nickname = userModel.NickName,
                HeadImgUrl = userModel.HeadImgUrl,
                ReturnUrl = ReturnUrl,
                MobileBinded = !string.IsNullOrWhiteSpace(userModel.Mobile),
                MoblieRepeatUser = mobileRepeatUser
            };
            return ResponseResult.Success(model);
        }

        public async Task<IActionResult> QQAuth(string code, string state, string ReturnUrl)
        {
            if (string.IsNullOrEmpty(state) || string.IsNullOrEmpty(code) || code.Length != 32)
            {
                return Content("第三方登录授权失败");
            }
            string stateData = _easyRedisClient.GetAsync<string>("qq_redirect-" + state).Result;
            //RedisHelper.Remove("qq_redirect-" + state);
            if (string.IsNullOrEmpty(stateData))
            {
                return Content("第三方登录授权失败");
            }
            string qq_AppID = AppInfos.QQ_Web.AppID;
            string appsecret = AppInfos.QQ_Web.AppSecret;
            string AuthAccessToken = await _QQUtil.GetQQAccessToken(qq_AppID, appsecret, code, stateData);
            if (!string.IsNullOrEmpty(AuthAccessToken))
            {
                return await QQAuthToken(AuthAccessToken, ReturnUrl: ReturnUrl);
            }
            ViewBag.RedirectUri = ReturnUrl ?? "/";
            return View("~/Views/Login/LoginAuth.cshtml");
        }
        public async Task<IActionResult> QQAuthToken(string accessToken, Guid? kid = null, bool encrypt = false, string ReturnUrl = null)
        {
            PMS.UserManage.Domain.Common.RootModel json = new PMS.UserManage.Domain.Common.RootModel();
            if (encrypt && kid != null)
            {
                string privateKey = _easyRedisClient.GetAsync<string>($"privateKey:{kid}")?.Result.ToString();
                if (string.IsNullOrEmpty(privateKey))
                {
                    json.status = 1;
                    json.errorDescription = "呆太久了，再试一下吧";
                    return Json(json);
                }
                try
                {
                    accessToken = RSAHelper.RSADecrypt(privateKey, accessToken);
                }
                catch (Exception ex)
                {
                    json.status = 1;
                    json.errorDescription = "数据解密失败，请再试一下吧";
                    return Json(json);
                }
            }
            string qq_AppID = AppInfos.QQ_Web.AppID;
            string qq_AppName = AppInfos.QQ_Web.AppName;
            if (Request.Headers["User-Agent"].ToString().ToLower().Contains("ischool"))
            {
                qq_AppID = AppInfos.QQ_App.AppID;
                qq_AppName = AppInfos.QQ_App.AppName;
            }
            QQOpenID OpenID = await _QQUtil.GetQQOpenID(accessToken);
            if (!string.IsNullOrEmpty(OpenID.openid))
            {
                OpenID.unionid = OpenID.unionid.Replace("UID_", "");
                QQUserInfoResult QQuserinfo = await _QQUtil.GetQQUserInfo(accessToken, qq_AppID, OpenID.openid);
                if (!User.Identity.IsAuthenticated)
                {
                    UserInfo userInfo = new UserInfo();
                    if (!string.IsNullOrEmpty(OpenID.unionid))
                    {
                        userInfo.NickName = QQuserinfo.nickname;
                        userInfo.HeadImgUrl = QQuserinfo.figureurl_qq ?? QQuserinfo.figureurl_qq_2 ?? QQuserinfo.figureurl_qq_1;
                        userInfo.Sex = QQuserinfo.gender == "男" ? 1 : QQuserinfo.gender == "女" ? 0 : (byte?)null;
                    }
                    _loginService.QQLogin(OpenID.unionid, OpenID.openid, qq_AppName, ref userInfo);
                    var talent = _talentService.GetTalentDetail(userInfo.Id.ToString());
                    await SetSignInCookie(userInfo, talent);
                    if (!IsAjaxRequest())
                    {
                        ViewBag.RedirectUri = ReturnUrl ?? "/mine/";
                        return View("~/Views/Login/LoginAuth.cshtml");
                    }
                    else
                    {
                        return Json(new PMS.UserManage.Application.ModelDto.Login.Post()
                        {
                            ID = userInfo.Id,
                            Nickname = userInfo.NickName,
                            HeadImgUrl = userInfo.HeadImgUrl,
                        });
                    }
                }
                else
                {
                    _easyRedisClient.AddAsync("QQBindInfo-" + userID, OpenID, new TimeSpan(0, 0, 0, 300));
                    _easyRedisClient.AddAsync("QQBindUserInfo-" + userID, QQuserinfo, new TimeSpan(0, 0, 0, 300));
                    return RedirectToAction("BindQQ", "ApiAccount");
                }
            }
            else
            {
                json.status = 1;
                json.errorDescription = "AccessToken无效";
                return Json(json);
            }
        }

        //给微信支付使用的获取OpenId专用方法，只有使用正式服服务号的appid才能支付
        public async Task<IActionResult> WXOpenId(string code, string state, string ReturnUrl)
        {
            string weixin_AppID = "wxeefc53a3617746e2";
            string appsecret = "54c29b72683aa64ffbf2309586027ca8";
            WXAccessToken AuthAccessToken = await weixinUtil.GetWeiXinAccessToken(weixin_AppID, appsecret, code);

            return Redirect(ReturnUrl + (ReturnUrl.Contains("?") ? "&" : "?") + "openId=" + AuthAccessToken?.openid);
        }
        //根据小程序支付时获取openid方法（正式环境）
        public async Task<string> WXMPOpenId(string code, string appId = "wx5de9fffde7f54eb0")
        {
            string weixin_AppID = appId;
            string appsecret = "cb8e3d37f09e7352449f357da0049045";
            WXAccessToken AuthAccessToken = await weixinUtil.GetWeiXinAccessToken(weixin_AppID, appsecret, code);
            return AuthAccessToken?.openid;
        }

        //获取OpenId通用方法
        public async Task<IActionResult> WXGetOpenId(string code, string state, string ReturnUrl)
        {
            string weixin_AppID = AppInfos.Weixin.AppID;
            string appsecret = AppInfos.Weixin.AppSecret;
            WXAccessToken AuthAccessToken = await weixinUtil.GetWeiXinAccessToken(weixin_AppID, appsecret, code);

            return Redirect(ReturnUrl + (ReturnUrl.Contains("?") ? "&" : "?") + "openId=" + AuthAccessToken?.openid);
        }

        public async Task<IActionResult> WXAuth(string code, string state, string ReturnUrl)
        {
            if (string.IsNullOrEmpty(code) || code.Length != 32)
            {
                return Content("第三方登录授权失败");
            }
            //if (!state.StartsWith("wxapp"))
            //{
            //    string stateData = _easyRedisClient.GetAsync<string>("wx_redirect-" + state).Result;
            //    if (string.IsNullOrEmpty(stateData))
            //    {
            //        return Content("第三方登录授权失败");
            //    }
            //}
            state ??= "";
            string weixin_AppID = AppInfos.Weixin_Web.AppID;
            string weixin_AppName = AppInfos.Weixin_Web.AppName;
            string appsecret = AppInfos.Weixin_Web.AppSecret;
            string userAgent = Request.Headers["User-Agent"].ToString().ToLower();
            if (userAgent.Contains("micromessenger"))
            {
                if (state.StartsWith("wxapp"))
                {
                    weixin_AppID = AppInfos.Weixin_MiniProgram_Sxkid.AppID;
                    weixin_AppName = AppInfos.Weixin_MiniProgram_Sxkid.AppName;
                    appsecret = AppInfos.Weixin_MiniProgram_Sxkid.AppSecret;
                }
                else
                {
                    weixin_AppID = AppInfos.Weixin.AppID;
                    weixin_AppName = AppInfos.Weixin.AppName;
                    appsecret = AppInfos.Weixin.AppSecret;
                }
            }
            else if (userAgent.Contains("ischool"))
            {
                weixin_AppID = AppInfos.Weixin_App.AppID;
                weixin_AppName = AppInfos.Weixin_App.AppName;
                appsecret = AppInfos.Weixin_App.AppSecret;
            }
            if (state.StartsWith("wxapp"))
            {
                WXMiniProgramAuth AuthInfo = await weixinUtil.GetWeiXinMiniProgramAuth(weixin_AppID, appsecret, code);
                if (AuthInfo.Errcode == 0)
                {
                    UserInfo userInfo = new UserInfo();
                    _loginService.WXLogin(AuthInfo.Unionid, AuthInfo.OpenID, null, weixin_AppName, ref userInfo, null);
                    var talent = _talentService.GetTalentDetail(userInfo.Id.ToString());
                    await SetSignInCookie(userInfo, talent);
                    for (int i = 0; i < Response.Headers["Set-Cookie"].Count; i++)
                    {
                        string c = Response.Headers["Set-Cookie"][i];
                        if (c.StartsWith("iSchoolAuth="))
                        {
                            c = c.Substring("iSchoolAuth=".Length).Split(';')[0];
                            string token = Guid.NewGuid().ToString("N").ToLower();
                            _easyRedisClient.AddAsync("Weixin_MiniProgram_Auth_token-" + token, c, new TimeSpan(0, 0, 0, 300));
                            return Json(new PMS.UserManage.Application.ModelDto.Login.Post()
                            {
                                ID = userInfo.Id,
                                Nickname = userInfo.NickName,
                                HeadImgUrl = userInfo.HeadImgUrl,
                                ReturnUrl = $"{IsHost.SiteHost_M}/ApiLogin/WXMiniProgramLogin?token={token}",
                                MobileBinded = !string.IsNullOrEmpty(userInfo.Mobile)
                            });
                        }
                    }
                }
            }
            else
            {
                WXAccessToken AuthAccessToken = await weixinUtil.GetWeiXinAccessToken(weixin_AppID, appsecret, code);
                if (!string.IsNullOrEmpty(AuthAccessToken.openid))
                {
                    WXUserInfoResult WXuserinfo = await weixinUtil.GetWeiXinUserInfo(AuthAccessToken.access_token, AuthAccessToken.openid);

                    if (!User.Identity.IsAuthenticated)
                    {
                        //未登录，微信授权登录
                        UserInfo userInfo = new UserInfo();
                        if (!string.IsNullOrEmpty(WXuserinfo.unionid))
                        {
                            userInfo.NickName = WXuserinfo.nickname;
                            userInfo.HeadImgUrl = WXuserinfo.headimgurl;
                            userInfo.Sex = WXuserinfo.sex == "1" ? 1 : WXuserinfo.sex == "2" ? 0 : (byte?)null;
                        }
                        else
                        {
                            string redirect_uri_wx = Request.Scheme + "://" + Request.Host + "/ApiLogin/WXAuth?ReturnUrl=" +
               Uri.EscapeDataString(ReturnUrl ?? "/mine/mine");
                            var a = _easyRedisClient.AddAsync("WXAuth:wx_redirect-" + state, state).Result;
                            return Redirect(weixinUtil.GetLoginURL(redirect_uri_wx, weixin_AppID, "snsapi_userinfo", state));
                        }
                        _loginService.WXLogin(WXuserinfo.unionid, AuthAccessToken.openid, null, weixin_AppName, ref userInfo, WXuserinfo.nickname);
                        try
                        {
                            var subscribeStatus = await _WeChatAppClient.GetSubscribeStatus(AuthAccessToken.openid);
                            if (subscribeStatus)
                            {
                                _account.BindWX(userInfo.Id, WXuserinfo.unionid, AuthAccessToken.openid, false, userInfo.NickName, weixin_AppName, subscribeStatus);
                            }

                        }
                        catch
                        {
                        }
                        var talent = _talentService.GetTalentDetail(userInfo.Id.ToString());
                        await SetSignInCookie(userInfo, talent);
                        if (!IsAjaxRequest())
                        {
                            ViewBag.RedirectUri = ReturnUrl ?? "/mine/";
                            return View("~/Views/Login/LoginAuth.cshtml");
                        }
                        else
                        {
                            return Json(new PMS.UserManage.Application.ModelDto.Login.Post()
                            {
                                ID = userInfo.Id,
                                Nickname = userInfo.NickName,
                                HeadImgUrl = userInfo.HeadImgUrl,
                                ReturnUrl = ReturnUrl,
                                MobileBinded = !string.IsNullOrEmpty(userInfo.Mobile)
                            });
                        }
                    }
                    else
                    {
                        //已登录，微信授权绑定微信
                        if (WXuserinfo.openid != default)
                        {
                            try
                            {
                                var subscribeStatus = await _WeChatAppClient.GetSubscribeStatus(WXuserinfo.openid);
                                if (subscribeStatus)
                                {
                                    WXuserinfo.OpenIdSubscribeStatus = true;
                                }

                            }
                            catch
                            {
                            }
                        }
                        await _easyRedisClient.AddAsync("WXAuth:WXBindInfo-" + userID, WXuserinfo, DateTime.Now.AddSeconds(300), StackExchange.Redis.CommandFlags.FireAndForget);
                        return Redirect(ReturnUrl ?? Url.Action("BindWX", "ApiAccount"));
                    }
                }
            }
            return Json(new PMS.UserManage.Domain.Common.RootModel() { status = 1 });
        }
        /// <summary>
        /// 微信小程序登录（旧版）
        /// </summary>
        /// <param name="code"></param>
        /// <param name="encryptedData"></param>
        /// <param name="iv"></param>
        /// <param name="ReturnUrl"></param>
        /// <param name="mp"></param>
        /// <returns></returns>
        public async Task<IActionResult> WXMiniProgramLogin(string code, string encryptedData, string iv, string ReturnUrl = "", string mp = "sxkid")
        {
            ReturnUrl = string.IsNullOrEmpty(ReturnUrl) ? IsHost.SiteHost_M : ReturnUrl;
            string weixin_AppID = AppInfos.Weixin_MiniProgram_Sxkid.AppID;
            string weixin_AppName = AppInfos.Weixin_MiniProgram_Sxkid.AppName;
            string appsecret = AppInfos.Weixin_MiniProgram_Sxkid.AppSecret;
            switch (mp.ToLower())
            {
                case "lecture":
                    weixin_AppID = AppInfos.Weixin_MiniProgram_Lecture.AppID;
                    weixin_AppName = AppInfos.Weixin_MiniProgram_Lecture.AppName;
                    appsecret = AppInfos.Weixin_MiniProgram_Lecture.AppSecret;
                    break;
            }
            WXMiniProgramAuth AuthInfo = await weixinUtil.GetWeiXinMiniProgramAuth(weixin_AppID, appsecret, code);
            UserInfo userInfo = new UserInfo();
            bool isReg = false;
            if (AuthInfo.Errcode == 0)
            {
                if (!string.IsNullOrEmpty(iv))
                {
                    string infoJSON = weixinUtil.AES_decrypt(encryptedData, iv, AuthInfo.Session_Key);
                    var mobileInfo = JsonConvert.DeserializeObject<WXMiniProgramMobile>(infoJSON);
                    _loginService.WXMiniProgramLogin(Convert.ToInt16(mobileInfo.countryCode), mobileInfo.purePhoneNumber, AuthInfo.Unionid, AuthInfo.OpenID, weixin_AppName, ref userInfo, out bool mobileRepeatUser);
                }
                else
                {
                    _loginService.WXLogin(AuthInfo.Unionid, AuthInfo.OpenID, AuthInfo.Session_Key, weixin_AppName, ref userInfo, null);
                }
                var talent = _talentService.GetTalentDetail(userInfo.Id.ToString());
                await SetSignInCookie(userInfo, talent);
            }

            if (IsAjaxRequest())
            {
                if (AuthInfo.Errcode == 0)
                {
                    return Json(ResponseResult.Success(new
                    {
                        isReg,
                        isBindMobile = !string.IsNullOrWhiteSpace(userInfo.Mobile),
                        userInfo = new
                        {
                            userInfo.Id,
                            userInfo.NickName,
                            userInfo.HeadImgUrl,
                            userInfo.Mobile,
                            userInfo.Introduction,
                            userInfo.VerifyTypes
                        },
                        ReturnUrl
                    }));
                }
                else
                {
                    return Json(ResponseResult.Failed("登录失败"));
                }
            }
            return Redirect(ReturnUrl);
        }

        /// <summary>
        /// 微信小程序登录（新版）
        /// </summary>
        /// <param name="code"></param>
        /// <param name="encryptedData"></param>
        /// <param name="iv"></param>
        /// <param name="ReturnUrl"></param>
        /// <param name="mp"></param>
        /// <returns></returns>
        public async Task<IActionResult> WXProgramLogin([FromBody] WXMiniProgramLoginData data)
        {
            data.ReturnUrl = string.IsNullOrEmpty(data.ReturnUrl) ? IsHost.SiteHost_M : data.ReturnUrl;
            string weixin_AppID = AppInfos.Weixin_MiniProgram_Sxkid.AppID;
            string weixin_AppName = AppInfos.Weixin_MiniProgram_Sxkid.AppName;
            string appsecret = AppInfos.Weixin_MiniProgram_Sxkid.AppSecret;
            switch (data.Mp.ToLower())
            {
                case "lecture":
                    weixin_AppID = AppInfos.Weixin_MiniProgram_Lecture.AppID;
                    weixin_AppName = AppInfos.Weixin_MiniProgram_Lecture.AppName;
                    appsecret = AppInfos.Weixin_MiniProgram_Lecture.AppSecret;
                    break;
            }
            WXMiniProgramAuth AuthInfo = await weixinUtil.GetWeiXinMiniProgramAuth(weixin_AppID, appsecret, data.Code);
            UserInfo userInfo = new UserInfo();
            bool isReg = false;
            if (AuthInfo.Errcode == 0)
            {
                if (!string.IsNullOrEmpty(data.IV))
                {
                    string infoJSON = weixinUtil.AES_decrypt(data.EncryptedData, data.IV, AuthInfo.Session_Key);
                    var wxUserInfo = JsonConvert.DeserializeObject<WXMiniProgramUserInfo>(infoJSON);

                    //性别 0：未知、1：男、2：女
                    int? sex = wxUserInfo.Gender == 0 ? (int?)null : wxUserInfo.Gender == 2 ? 0 : 1;

                    _loginService.WXLoginIncludeUserInfo(wxUserInfo.NickName, sex, wxUserInfo.AvatarUrl, AuthInfo.Unionid, AuthInfo.OpenID, AuthInfo.Session_Key, weixin_AppName, ref userInfo, ref isReg);
                }
                else if (!string.IsNullOrWhiteSpace(data.NickName))
                {
                    //新版微信小程序2021.04.15后不再由后端解密用户数据
                    //性别 0：未知、1：男、2：女
                    int? sex = data.Gender == 0 ? (int?)null : data.Gender == 2 ? 0 : 1;
                    _loginService.WXLoginIncludeUserInfo(data.NickName, sex, data.AvatarUrl, AuthInfo.Unionid, AuthInfo.OpenID, AuthInfo.Session_Key, weixin_AppName, ref userInfo, ref isReg);
                }
                else
                {
                    _loginService.WXLogin(AuthInfo.Unionid, AuthInfo.OpenID, AuthInfo.Session_Key, weixin_AppName, ref userInfo, null);
                }
                var talent = _talentService.GetTalentDetail(userInfo.Id.ToString());
                await SetSignInCookie(userInfo, talent);
            }

            if (IsAjaxRequest())
            {
                if (AuthInfo.Errcode == 0)
                {
                    return Json(ResponseResult.Success(new
                    {
                        isReg,
                        isBindMobile = !string.IsNullOrWhiteSpace(userInfo.Mobile),
                        userInfo = new
                        {
                            userInfo.Id,
                            userInfo.NickName,
                            userInfo.HeadImgUrl,
                            userInfo.Mobile,
                            userInfo.Introduction,
                            userInfo.VerifyTypes
                        },
                        data.ReturnUrl
                    }));
                }
                else
                {
                    return Json(ResponseResult.Failed("登录失败"));
                }
            }
            return Redirect(data.ReturnUrl);
        }


        /// <summary>
        /// 解密获取微信小程序的用户手机,同时保存用户手机到数据库
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="encryptedData"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ResponseResult GetWXProgramMobile([FromBody] WXEncryptedData data)
        {
            var userId = User.Identity.GetId();
            string weixin_AppName = AppInfos.Weixin_MiniProgram_Sxkid.AppName;

            var AuthInfo = _loginService.GetWXAuthInfo(userId, weixin_AppName);

            string infoJSON = weixinUtil.AES_decrypt(data.EncryptedData, data.IV, AuthInfo.SessionKey);
            var mobileInfo = JsonConvert.DeserializeObject<WXMiniProgramMobile>(infoJSON);


            if (!string.IsNullOrEmpty(mobileInfo.purePhoneNumber))
            {
                _account.BindMobile(userId, Convert.ToInt16(mobileInfo.countryCode), mobileInfo.purePhoneNumber);
            }

            return ResponseResult.Success(mobileInfo);
        }


        /// <summary>
        /// 百度小程序登录
        /// </summary>
        /// <param name="code"></param>
        /// <param name="ReturnUrl"></param>
        /// <param name="mp"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> BDMiniProgramLogin([FromBody] BDMiniProgramLoginData data)
        {
            string returnUrl = string.IsNullOrEmpty(data.ReturnUrl) ? IsHost.SiteHost_M : data.ReturnUrl;

            UserInfo userInfo = new UserInfo();
            bool isReg = false;
            string openId = null;
            var loginSucess = _baiduOAuthService.Login(data.Code, ref userInfo, ref openId, ref isReg);

            if (loginSucess)
            {
                await SetSignInCookie(userInfo, null);
                return ResponseResult.Success(new
                {
                    isReg,
                    isBindMobile = !string.IsNullOrWhiteSpace(userInfo.Mobile),
                    openId,
                    userInfo = new
                    {
                        userInfo.Id,
                        userInfo.NickName,
                        userInfo.HeadImgUrl,
                        userInfo.Mobile,
                        userInfo.Introduction,
                        userInfo.VerifyTypes
                    },
                    returnUrl
                });
            }
            else
            {
                return ResponseResult.Failed("登录失败");
            }
        }

        /// <summary>
        /// 解密获取百度的用户信息,同时保存用户信息到数据库
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="encryptedData"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ResponseResult GetBDUserInfo([FromBody] BDEncryptedData data)
        {
            var userId = User.Identity.GetId();

            var bdAuthInfo = _baiduOAuthService.GetAuthInfo(userId);

            //通过openId获取accessKey
            var userinfo = _baiduOAuthService.GetUserInfo(data.EncryptedData, data.IV, bdAuthInfo.AccessKey);
            return ResponseResult.Success(userinfo);
        }
        /// <summary>
        /// 解密获取百度的用户手机,同时保存用户手机到数据库
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="encryptedData"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ResponseResult GetBDMobile([FromBody] BDEncryptedData data)
        {
            var userId = User.Identity.GetId();

            var bdAuthInfo = _baiduOAuthService.GetAuthInfo(userId);

            //通过openId获取accessKey
            var info = _baiduOAuthService.GetMobile(data.EncryptedData, data.IV, bdAuthInfo.AccessKey);

            if (!string.IsNullOrEmpty(info.Mobile))
            {
                _account.BindMobile(userId, 86, info.Mobile);
            }

            return ResponseResult.Success(info);
        }



        public IActionResult ForgetPSW(string ReturnUrl)
        {
            ViewBag.ReturnUrl = Uri.EscapeDataString(ReturnUrl ?? "");
            RSAKeyPair keyPair = RSAHelper.GenerateRSAKeyPair();
            string privateKey = keyPair.PrivateKey;
            Guid _kid = Guid.NewGuid();
            _easyRedisClient.AddAsync($"privateKey:{_kid}", privateKey, new TimeSpan(0, 0, 0, 300));
            PMS.UserManage.Application.ModelDto.Login.Get model = new PMS.UserManage.Application.ModelDto.Login.Get()
            {
                PublicKey = keyPair.PublicKey,
                Kid = _kid.ToString()
            };
            return iSchoolResult(model);
        }
        [HttpPost]
        public IActionResult ResetPSW(Guid kid, short nationCode, string mobile, string password, string rnd)
        {
            PMS.UserManage.Domain.Common.RootModel json = new PMS.UserManage.Domain.Common.RootModel();
            if (!_account.ChangePSW(kid, nationCode, mobile, password, rnd, out string failResult, true, "ForgetPSW"))
            {
                json.status = 1;
                json.errorDescription = failResult;
            }
            return Json(json);
        }
        [HttpPost]
        public ResponseResult GetRndCode(string imgrnd, string mobile, short nationCode = 86, string codeType = "RegistOrLogin")
        {
            PMS.UserManage.Domain.Common.RootModel json = new PMS.UserManage.Domain.Common.RootModel();
            string ipaddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            int ipSendCount = _easyRedisClient.GetAsync<int>("GetRndCodeCount-" + ipaddress).Result;
            string[] imgVerify = new string[] { "ForgetPSW" };
            bool needImgRnd = imgVerify.Contains(codeType) || ipSendCount >= 100;
            if (needImgRnd && string.IsNullOrEmpty(imgrnd))
            {
                json.status = 1001;
                json.errorDescription = "请输入图形验证码";
                json.ReturnUrl = "/setting/VerifyCode?t=" + DateTime.Now.Ticks;
                return ResponseResult.Success(json);
            }
            _easyRedisClient.AddAsync("GetRndCodeCount-" + ipaddress, ++ipSendCount, new TimeSpan(0, 0, 0, 3600));
            if (!User.Identity.IsAuthenticated && string.IsNullOrEmpty(mobile))
            {
                json.status = 1;
                json.errorDescription = "参数错误";
                return ResponseResult.Success(json);
            }
            if (User.Identity.IsAuthenticated && string.IsNullOrEmpty(mobile))
            {
                var info = _account.GetUserInfo(userID, false);
                if (!string.IsNullOrEmpty(info.Mobile))
                {
                    nationCode = info.NationCode.Value;
                    mobile = info.Mobile;
                }
            }
            if (needImgRnd)
            {
                Guid.TryParse(Request.Cookies["verifycode"], out Guid ImgRndCodeID);
                if (!_verifyCode.Check(ImgRndCodeID, imgrnd, false))
                {
                    json.status = 1;
                    json.errorDescription = "图形验证码错误";
                    return ResponseResult.Success(json);
                }
            }
            if (!CommonHelper.isMobile(mobile))
            {
                json.status = 1;
                json.errorDescription = "请输入有效的手机号码";
                return ResponseResult.Success(json);
            }

            bool sendStatus = sMSHelper.SendRndCode(mobile, codeType, nationCode, out string failReason);
            if (!sendStatus)
            {
                json.status = 1;
                json.errorDescription = failReason;
            }
            return ResponseResult.Success(json);
        }

        [HttpGet]
        public ResponseResult IsLogin()
        {
            if (User.Identity.IsAuthenticated)
                return ResponseResult.Success();
            return ResponseResult.Failed("请先登录");
        }
        [HttpGet]
        public ResponseResult GetMoblieUsers([FromQuery] short nationCode, string mobile)
        {
            //判断当前登录账户手机是不是这个手机，禁止非法查询
            if (!User.Identity.IsAuthenticated)
                return ResponseResult.Failed("请先登录");
            if (!_loginService.CheckPermission(User.Identity.GetUserInfo().UserId, mobile))
            {
                return ResponseResult.Failed("请求有误");
            }
            var users = _loginService.SameMobileUsers(nationCode, mobile).OrderByDescending(x => x.LoginTime);
            var r = new List<LoginUserViewModel>();
            foreach (var item in users)
            {
                r.Add(new LoginUserViewModel() { Id = item.Id, NickName = item.NickName, HeadImgUrl = item.HeadImgUrl, LoginTime = item.LoginTime.ToString("yyyy-MM-dd HH:mm:ss") });
            }
            return ResponseResult.Success(r);
        }
        [HttpPost]
        public async Task<IActionResult> SelectUser(Guid userid, string ReturnUrl)
        {
            //判断当前登录账户手机是不是这个手机，禁止非法登录
            if (!User.Identity.IsAuthenticated)
                return Content("请先登陆");
            var selUser = _loginService.SelectUserLogin(userid);
            if (null == selUser)
                return Content("请求有误1");

            if (!_loginService.CheckPermission(User.Identity.GetUserInfo().UserId, selUser.Mobile))
            {
                return Content("请求有误2");
            }
            var talent = _talentService.GetTalentByUserId(selUser.Id.ToString());
            await SetSignInCookie(selUser, talent);
            //更改登陆时间
            PMS.UserManage.Application.ModelDto.Login.Post model = new PMS.UserManage.Application.ModelDto.Login.Post()
            {
                ID = selUser.Id,
                Nickname = selUser.NickName,
                HeadImgUrl = selUser.HeadImgUrl,
                ReturnUrl = ReturnUrl,

            };
            return Json(model);
        }
    }
}