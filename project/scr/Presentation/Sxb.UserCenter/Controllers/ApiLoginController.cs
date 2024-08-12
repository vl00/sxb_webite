using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Entities;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model;
using ProductManagement.API.SMS;
using ProductManagement.Framework.AspNetCoreHelper.Utils;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using ProductManagement.UserCenter.TencentCommon;
using ProductManagement.UserCenter.TencentCommon.Model;
using ProductManagement.UserCenter.Wx;
using ProductManagement.UserCenter.Wx.Model;
using Sxb.UserCenter.Authentication.Attributes;
using Sxb.UserCenter.Request.Login;
using Sxb.UserCenter.Response;
using Sxb.UserCenter.Utils;
using Sxb.UserCenter.ViewModel;

namespace Sxb.UserCenter.Controllers
{
    [AllowAnonymous]
    public class ApiLoginController : Base
    {
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly WeixinUtil _weixinUtil;
        private readonly QQUtil _QQUtil;
        private readonly ILoginService _loginService;
        private readonly IAccountService _accountService;
        private readonly ITalentService _talentService;
        private readonly IVerifyService _verifyService;
        private readonly IOAuthWeixinService _OAuthWeixinService;
        private readonly IOAuthQQService _OAuthQQService;
        private readonly IOAuthBaiduService _OAuthBaiduService;
        private readonly AppInfos AppInfos;
        private readonly IsHost IsHost;
        private readonly IWeChatAppClient _weChatAppClient;

        private readonly ILogger<ApiLoginController> _logger;

        public ApiLoginController(IEasyRedisClient easyRedisClient,
            IOptions<AppInfos> _appInfos,
            IOptions<IsHost> _isHost,
            ITalentService talentService,
            ILoginService loginService,
            IAccountService accountService,
            IVerifyService verifyService,
            IOAuthWeixinService OAuthWeixinService,
            IOAuthQQService OAuthQQService,
            IOAuthBaiduService OAuthBaiduService,
            IWeChatAppClient weChatAppClient,
            ILogger<ApiLoginController> logger)
        {
            AppInfos = _appInfos.Value;
            IsHost = _isHost.Value;
            _easyRedisClient = easyRedisClient;

            _loginService = loginService;
            _accountService = accountService;
            _talentService = talentService;
            _verifyService = verifyService;
            _OAuthWeixinService = OAuthWeixinService;
            _OAuthQQService = OAuthQQService;
            _OAuthBaiduService = OAuthBaiduService;
            _weChatAppClient = weChatAppClient;

            _weixinUtil = new WeixinUtil(new System.Net.Http.HttpClient());
            _QQUtil = new QQUtil(new System.Net.Http.HttpClient());

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }



        /// <summary>
        /// 设置登录所需的Cookie
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="talent"></param>
        /// <returns></returns>
        private async Task SetSignInCookie(UserInfo userInfo, TalentEntity talent)
        {
            if (userInfo.Id != Guid.Empty)
            {
                HttpContext.Response.Cookies.Delete("NotAllowAutoLogin", new Microsoft.AspNetCore.Http.CookieOptions()
                {
                    SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax,
                    Domain = ".sxkid.com"
                });
                await JwtCookieHelper.SetSignInCookie(HttpContext, userInfo, talent);
                HttpContext.Response.SetUserId(userInfo.Id.ToString());

            }
        }
        string BUrl = "https://www.shangxuebang.com/user/login";
        //string BUrl = "http://localhost:8006/login-success.html";//test
        string[] WhiteUrls = null; // new string[] { "localhost", "sxkid.com", "shangxuebang.com" };

        /// <summary>
        /// 需要登录凭证
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        private bool IsNeedCode(string returnUrl)
        {
            return !string.IsNullOrWhiteSpace(returnUrl)
                && returnUrl.IsDomain("shangxuebang.com")
                ;
        }



        /// <summary>
        /// 获取跳转的url
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        private async Task<string> GetRedirectUrl(Guid? userId, string returnUrl)
        {
            var defaultRoute = IsMobile() ? "/mine/" : "/";

            //非白名单, 返回默认路径
            if (WhiteUrls != null && !returnUrl.IsDomain(WhiteUrls))
            {
                return defaultRoute;
            }

            if (IsNeedCode(returnUrl) && userId != null)
            {
                var code = await _loginService.GetUserCode(userId.Value);
                //HttpContext.Response.SetUserCode(code);

                var newReturnUrl = $"{BUrl}?code={code}&return_url={HttpUtility.UrlEncode(returnUrl)}";
                return newReturnUrl;
            }
            return !string.IsNullOrWhiteSpace(returnUrl) ? returnUrl : defaultRoute;
        }

        private async Task<IActionResult> LoginRedirect(UserInfo userInfo, string returnUrl)
        {
            var redirectUrl = await GetRedirectUrl(userInfo.Id, returnUrl);
            return Redirect(redirectUrl);
        }
        private async Task<IActionResult> LoginRedirectView(UserInfo userInfo, string returnUrl)
        {
            ViewBag.RedirectUri = await GetRedirectUrl(userInfo.Id, returnUrl);
            return View("~/Views/Login/LoginAuth.cshtml");
        }

        public async Task<PMS.UserManage.Application.ModelDto.Login.Post> UserToPost(UserInfo userInfo, string returnUrl = "")
        {
            var post = new PMS.UserManage.Application.ModelDto.Login.Post()
            {
                ID = userInfo.Id,
                Nickname = userInfo.NickName,
                HeadImgUrl = userInfo.HeadImgUrl,
                ReturnUrl = returnUrl,
                MobileBinded = !string.IsNullOrEmpty(userInfo.Mobile)
            };

            //登录凭证
            //post.Code = await _loginService.GetUserCode(userInfo.Id);
            post.ReturnUrl = await GetRedirectUrl(userInfo.Id, returnUrl);

            return post;
        }

        /// <summary>
        /// 判断是否已经登录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResponseResult IsLogin()
        {
            if (User.Identity.IsAuthenticated)
                return ResponseResult.Success();
            return ResponseResult.Failed("请先登录");
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // + ","+ CookieAuthenticationDefaults.AuthenticationScheme
        public ResponseResult ValidToken()
        {
            if (User.Identity.IsAuthenticated)
                return ResponseResult.Success();
            return ResponseResult.Failed("请先登录");
        }

        [HttpGet]
        public ResponseResult TokenLogin()
        {
            var userModel = _accountService.GetUserInfo(Guid.Parse("899FA737-70E3-4F49-B92F-4C7B766CF689"));

            var talent = _talentService.GetTalentByUserId(userModel.Id.ToString());

            var token = JwtCookieHelper.GetSignInToken(userModel, talent);

            return ResponseResult.Success(new
            {
                access_token = token,
                token_type = "Bearer",
            });
        }


        /// <summary>
        /// 获取登录码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseResult> GetLoginCode(Guid userId)
        {
            if (HttpContext.Request.Headers.TryGetValue("appid", out StringValues appid))
                if (HttpContext.Request.Headers.TryGetValue("appsecret", out StringValues appsecret))
                {
                    var insideAppSecret = await _easyRedisClient.GetStringAsync($"InsideApps:{appid}");
                    if (insideAppSecret.Equals(appsecret) == false)
                        return ResponseResult.Failed(ResponseCode.NoAuth);
                    else {
                        var code = await _loginService.GetUserCode(userId);
                        return ResponseResult.Success(code,"ok");
                    }
                }
            return ResponseResult.Failed(ResponseCode.NoAuth);
        }


        [HttpGet("[Controller]/cookie/{code}")]
        public async Task<ResponseResult> CookieByCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return ResponseResult.Failed("请输入Code");
            }

            var userInfo = await _loginService.LoginByCode(code);
            if (userInfo == null)
                return ResponseResult.Failed("Code失效");
            var talent = _talentService.GetTalentByUserId(userInfo.Id.ToString());

            //PMS.UserManage.Application.ModelDto.Login.Post model = await UserToPost(userInfo);
            await JwtCookieHelper.SetSignInCookie(HttpContext,userInfo, talent);
            return ResponseResult.Success("OK");
        }


        [HttpGet("[Controller]/Token/{code}")]
        public async Task<ResponseResult> TokenByCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return ResponseResult.Failed("请输入Code");
            }

            var userInfo = await _loginService.LoginByCode(code);
            if (userInfo == null)
                return ResponseResult.Failed("Code失效");
            var talent = _talentService.GetTalentByUserId(userInfo.Id.ToString());

            //PMS.UserManage.Application.ModelDto.Login.Post model = await UserToPost(userInfo);
            var token = JwtCookieHelper.GetSignInToken(userInfo, talent);
            return ResponseResult.Success(new
            {
                Token = token
            });
        }

        /// <summary>
        /// 获取登录加密公钥和微信QQ授权登录链接
        /// </summary>
        /// <param name="ReturnUrl"></param>
        /// <returns></returns>
        public ResponseResult GetRSAkeyPair(string ReturnUrl)
        {
            //ViewBag.ReturnUrl = Uri.EscapeDataString(ReturnUrl ?? "");
            string state = Guid.NewGuid().ToString("N");
            //前端endcode url
            string redirect_uri_wx = Request.Scheme + "://" + Request.Host + "/ApiLogin/WXAuth?ReturnUrl=" +
               Uri.EscapeDataString(ReturnUrl ?? "/mine/");
            string redirect_uri_qq = Request.Scheme + "://" + Request.Host + "/ApiLogin/QQAuth?ReturnUrl=" +
               Uri.EscapeDataString(ReturnUrl ?? "/mine/");

            _easyRedisClient.AddAsync("WXAuth:wx_redirect-" + state, redirect_uri_wx, new TimeSpan(0, 0, 0, 600), StackExchange.Redis.CommandFlags.FireAndForget);
            _easyRedisClient.AddAsync("QQAuth:qq_redirect-" + state, redirect_uri_qq, new TimeSpan(0, 0, 0, 600), StackExchange.Redis.CommandFlags.FireAndForget);
            string Login_Uri_WX;
            if (Request.Headers["User-Agent"].ToString().ToLower().Contains("micromessenger"))
            {
                string appid = AppInfos.Weixin.AppID;
                Login_Uri_WX = _weixinUtil.GetLoginURL(redirect_uri_wx, appid, "snsapi_userinfo", state);
            }
            else
            {
                string appid = AppInfos.Weixin_Web.AppID;
                Login_Uri_WX = _weixinUtil.GetWebLoginUrl(redirect_uri_wx, appid, state);
            }
            string Login_Uri_QQ = _QQUtil.GetWebLoginURL(redirect_uri_qq, state: state);

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

            if (model.IsLogin)
            {
                model.RedirectUrl = GetRedirectUrl(User.Identity.GetId(), ReturnUrl).GetAwaiter().GetResult();
            }
            return ResponseResult.Success(model);
        }

        #region 手机号登录
        /// <summary>
        /// 手机号登录（密码登录、验证码登录）
        /// </summary>
        /// <param name="kid"></param>
        /// <param name="nationCode"></param>
        /// <param name="mobile"></param>
        /// <param name="password"></param>
        /// <param name="rnd"></param>
        /// <param name="ReturnUrl"></param>
        /// <param name="city"></param>
        /// <param name="nickname"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> MobileLogin(Guid kid, short nationCode, string mobile, string password, string rnd, string ReturnUrl, int? city, string nickname = null)
        {
            var userModel = _loginService.Login(kid, nationCode, mobile, password, rnd, out string failResult, nickname, city, out bool mobileRepeatUser);
            if (userModel == null || userModel.Id == default)
            {
                return ResponseResult.Failed(failResult);
            }

            //var verifyInfo = _verifyService.GetVerifies(userModel.Id);
            var talent = _talentService.GetTalentByUserId(userModel.Id.ToString());
            await SetSignInCookie(userModel, talent);
            await _easyRedisClient.RemoveAsync($"privateKey:{kid}", StackExchange.Redis.CommandFlags.FireAndForget);
            //PMS.UserManage.Application.ModelDto.Login.Post model = new PMS.UserManage.Application.ModelDto.Login.Post()
            //{
            //    ID = userModel.Id,
            //    Nickname = userModel.NickName,
            //    HeadImgUrl = userModel.HeadImgUrl,
            //    ReturnUrl = ReturnUrl,
            //    MobileBinded = !string.IsNullOrWhiteSpace(userModel.Mobile),
            //    MoblieRepeatUser = mobileRepeatUser
            //};

            PMS.UserManage.Application.ModelDto.Login.Post model = await UserToPost(userModel, ReturnUrl);
            model.MoblieRepeatUser = mobileRepeatUser;

            return ResponseResult.Success(model);
        }
        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="imgrnd"></param>
        /// <param name="mobile"></param>
        /// <param name="nationCode"></param>
        /// <param name="codeType"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult GetRndCode(string imgrnd, string mobile, short nationCode = 86, string codeType = "RegistOrLogin")
        {
            string ipaddress = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            int ipSendCount = _easyRedisClient.GetAsync<int>("RndCode:GetRndCodeCount-" + ipaddress).Result;
            string[] imgVerify = new string[] { "ForgetPSW" };
            bool needImgRnd = imgVerify.Contains(codeType) || ipSendCount >= 100;
            if (needImgRnd && string.IsNullOrEmpty(imgrnd))
            {
                return ResponseResult.Failed("请输入图形验证码", new
                {
                    errorstatus = 2,
                    errorDescription = "请输入图形验证码",
                    verifyCodeImg = Request.Scheme + "://" + Request.Host + "/setting/VerifyCode?t=" + DateTime.Now.Ticks
                });
            }
            _easyRedisClient.AddAsync("RndCode:GetRndCodeCount-" + ipaddress, ++ipSendCount, new TimeSpan(0, 0, 0, 3600));
            if (!User.Identity.IsAuthenticated && string.IsNullOrEmpty(mobile))
            {
                return ResponseResult.Failed("参数错误", new
                {
                    errorstatus = 1,
                    errorDescription = "参数错误"
                });
            }
            if (User.Identity.IsAuthenticated && string.IsNullOrEmpty(mobile))
            {
                var info = _accountService.GetUserInfo(userID, false);
                if (!string.IsNullOrEmpty(info.Mobile))
                {
                    nationCode = info.NationCode.Value;
                    mobile = info.Mobile;
                }
            }
            var _verifyCode = new VerifyCodeHelper(_easyRedisClient);
            if (needImgRnd)
            {
                Guid.TryParse(Request.Cookies["verifycode"], out Guid ImgRndCodeID);
                if (!_verifyCode.Check(ImgRndCodeID, imgrnd, false))
                {
                    return ResponseResult.Failed("图形验证码错误", new
                    {
                        errorstatus = 1,
                        errorDescription = "图形验证码错误"
                    });
                }
            }
            if (!CommonHelper.isMobile(mobile))
            {
                return ResponseResult.Failed("请输入有效的手机号码", new
                {
                    errorstatus = 1,
                    errorDescription = "请输入有效的手机号码"
                });
            }
            var sMSHelper = new SMSHelper(_easyRedisClient);
            bool sendStatus = sMSHelper.SendRndCode(mobile, codeType, nationCode, out string failReason);
            if (!sendStatus)
            {
                return ResponseResult.Failed(failReason, new
                {
                    errorstatus = 1,
                    errorDescription = failReason
                });

            }
            return ResponseResult.Success("发送成功");
        }
        [HttpPost]
        public ResponseResult GetSelfRndCode(string imgrnd, string mobile, short nationCode, string codeType = "RegistOrLogin")
        {
            if (!User.Identity.IsAuthenticated)
            {
                var result = ResponseResult.Failed("Need login");
                result.status = ResponseCode.NoLogin;
                return result;
            }
            var userInfo = _accountService.GetUserInfo(userID, false);
            if (userInfo == null || userInfo.Id == default || nationCode != userInfo.NationCode || mobile != userInfo.Mobile) return ResponseResult.Failed("Invalid phone number");
            return GetRndCode(imgrnd, mobile, nationCode, codeType);
        }
        /// <summary>
        /// 检验验证码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult VerifyRndCode(string mobile, string code, short nationCode = 86, string codeType = "RegistOrLogin")
        {
            string[] imgVerify = new string[] { "ForgetPSW" };
            if (!User.Identity.IsAuthenticated && string.IsNullOrEmpty(mobile))
            {
                return ResponseResult.Failed("参数错误", new
                {
                    errorstatus = 1,
                    errorDescription = "参数错误"
                });
            }
            if (User.Identity.IsAuthenticated && string.IsNullOrEmpty(mobile))
            {
                var info = _accountService.GetUserInfo(userID, false);
                if (!string.IsNullOrEmpty(info.Mobile))
                {
                    nationCode = info.NationCode.Value;
                    mobile = info.Mobile;
                }
            }
            var _verifyCode = new VerifyCodeHelper(_easyRedisClient);
            if (!CommonHelper.isMobile(mobile))
            {
                return ResponseResult.Failed("请输入有效的手机号码", new
                {
                    errorstatus = 1,
                    errorDescription = "请输入有效的手机号码"
                });
            }
            var smsHelper = new SMSHelper(_easyRedisClient);


            if (smsHelper.CheckRndCode(nationCode, mobile, code, codeType, false))
            {
                return ResponseResult.Success("验证成功");
            }
            else
            {
                return ResponseResult.Failed("验证失败", new
                {
                    errorstatus = 1,
                    errorDescription = "请输入正确的验证码"
                });
            }
        }

        /// <summary>
        /// 获取同一手机号的重复账户
        /// </summary>
        /// <param name="nationCode"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ResponseResult GetMoblieUsers([FromQuery] short nationCode, string mobile)
        {
            //判断当前登录账户手机是不是这个手机，禁止非法查询
            if (!_loginService.CheckUserMobile(User.Identity.GetUserInfo().UserId, nationCode, mobile))
            {
                return ResponseResult.Failed("请求有误");
            }
            var users = _loginService.GetSameMobileUsers(nationCode, mobile).OrderByDescending(x => x.LoginTime);
            var r = new List<LoginUserViewModel>();
            foreach (var item in users)
            {
                r.Add(new LoginUserViewModel() { Id = item.Id, NickName = item.NickName, HeadImgUrl = item.HeadImgUrl, LoginTime = item.LoginTime.ToString("yyyy-MM-dd HH:mm:ss") });
            }
            return ResponseResult.Success(r);
        }
        /// <summary>
        /// 选择登录同一手机号的账户
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="ReturnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ResponseResult> SelectUser(Guid userid, string ReturnUrl)
        {
            //判断当前登录账户手机是不是这个手机，禁止非法登录
            var selUser = _loginService.SelectUserLogin(userid);
            if (null == selUser)
                return ResponseResult.Failed("请求有误1");

            if (!_loginService.CheckUserMobile(User.Identity.GetUserInfo().UserId, selUser.NationCode ?? 86, selUser.Mobile))
            {
                return ResponseResult.Failed("请求有误2");
            }
            var talent = _talentService.GetTalentByUserId(selUser.Id.ToString());
            await SetSignInCookie(selUser, talent);
            //更改登陆时间
            //PMS.UserManage.Application.ModelDto.Login.Post model = new PMS.UserManage.Application.ModelDto.Login.Post()
            //{
            //    ID = selUser.Id,
            //    Nickname = selUser.NickName,
            //    HeadImgUrl = selUser.HeadImgUrl,
            //    ReturnUrl = ReturnUrl
            //};
            PMS.UserManage.Application.ModelDto.Login.Post model = await UserToPost(selUser, ReturnUrl);
            return ResponseResult.Success(model);
        }
        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <param name="ReturnUrl"></param>
        /// <returns></returns>
        public ResponseResult ForgetPSW(string ReturnUrl)
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
            return ResponseResult.Success(model);
        }
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="kid"></param>
        /// <param name="nationCode"></param>
        /// <param name="mobile"></param>
        /// <param name="password"></param>
        /// <param name="rnd"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult ResetPSW(Guid kid, short nationCode, string mobile, string password, string rnd)
        {
            if (!_accountService.ChangePSW(kid, nationCode, mobile, password, rnd, out string failResult, true, "ForgetPSW"))
            {
                return ResponseResult.Failed(failResult);
            }
            return ResponseResult.Success();
        }
        #endregion

        #region 第三方登录-微信授权
        //给微信支付使用的获取OpenId专用方法，只有使用正式服服务号的appid才能支付
        public async Task<IActionResult> WXOpenId(string code, string state, string ReturnUrl)
        {
            string weixin_AppID = "wxeefc53a3617746e2";
            string appsecret = "54c29b72683aa64ffbf2309586027ca8";
            var AuthAccessToken = await _weixinUtil.GetWeiXinAccessToken(weixin_AppID, appsecret, code);

            return Redirect(ReturnUrl + (ReturnUrl.Contains("?") ? "&" : "?") + "openId=" + AuthAccessToken?.openid);
        }
        //根据小程序支付时获取openid方法（正式环境）
        public async Task<string> WXMPOpenId(string code, string mp = "sxkid")
        {
            string weixin_AppID = "wx5de9fffde7f54eb0";
            string appsecret = "cb8e3d37f09e7352449f357da0049045";

            switch (mp.ToLower())
            {
                case "org":
                    weixin_AppID = "wx0da8ff0241f39b11";
                    appsecret = "0b82fc3e6826ba30fe7fa02f4b3824b4";
                    break;
                case "shop":
                    weixin_AppID = "wx53bc8d0d70a0ddf3";
                    appsecret = "029a0e4cac540d5531b530331842c3d6";
                    break;
                case "pay":
                    weixin_AppID = "wx53bc8d0d70a0ddf3";
                    appsecret = "029a0e4cac540d5531b530331842c3d6";
                    break;
            }

            WXMiniProgramAuth AuthInfo = await _weixinUtil.GetWeiXinMiniProgramAuth(weixin_AppID, appsecret, code);

            if (AuthInfo != null && !string.IsNullOrEmpty(AuthInfo.OpenID))
            {
                _OAuthWeixinService.UpdateWeiXinSessionKey(AuthInfo.OpenID, AuthInfo.Session_Key);
            }

            return AuthInfo?.OpenID;
        }

        //获取OpenId通用方法
        public async Task<IActionResult> WXGetOpenId(string code, string state, string ReturnUrl)
        {
            string weixin_AppID = AppInfos.Weixin.AppID;
            string appsecret = AppInfos.Weixin.AppSecret;
            var AuthAccessToken = await _weixinUtil.GetWeiXinAccessToken(weixin_AppID, appsecret, code);

            return Redirect(ReturnUrl + (ReturnUrl.Contains("?") ? "&" : "?") + "openId=" + AuthAccessToken?.openid);
        }
        public async Task<IActionResult> WXAuth(string code, string state, string ReturnUrl)
        {
            if (string.IsNullOrEmpty(code) || code.Length != 32)
            {
                return Content("第三方登录授权失败");
            }
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

            bool isReg = false;

            var AuthAccessToken = await _weixinUtil.GetWeiXinAccessToken(weixin_AppID, appsecret, code);
            if (!string.IsNullOrEmpty(AuthAccessToken.openid))
            {
                var WXuserinfo = await _weixinUtil.GetWeiXinUserInfo(AuthAccessToken.access_token, AuthAccessToken.openid);

                if (!User.Identity.IsAuthenticated)
                {
                    //未登录，微信授权登录
                    UserInfo userInfo = new UserInfo();

                    if (string.IsNullOrEmpty(WXuserinfo.unionid))
                    {
                        //获取不到unionid，需要用户进行授权，授权后再返回当前页面
                        string redirect_uri_wx = Request.Scheme + "://" + Request.Host + "/ApiLogin/WXAuth?ReturnUrl=" +
                            Uri.EscapeDataString(ReturnUrl ?? "/mine/");
                        var a = _easyRedisClient.AddAsync("WXAuth:wx_redirect-" + state, state).Result;
                        return Redirect(_weixinUtil.GetLoginURL(redirect_uri_wx, weixin_AppID, "snsapi_userinfo", state));
                    }
                    userInfo.NickName = WXuserinfo.nickname;
                    userInfo.HeadImgUrl = WXuserinfo.headimgurl;
                    userInfo.Sex = WXuserinfo.sex == "1" ? 1 : WXuserinfo.sex == "2" ? 0 : (byte?)null;

                    try
                    {
                        //获取用户关注状态
                        WXuserinfo.OpenIdSubscribeStatus = await _weChatAppClient.GetSubscribeStatus(AuthAccessToken.openid);
                    }
                    catch (Exception)
                    {
                    }

                    _OAuthWeixinService.WXLogin(WXuserinfo.unionid, AuthAccessToken.openid, null, weixin_AppName, weixin_AppID, ref userInfo, ref isReg, WXuserinfo.OpenIdSubscribeStatus);

                    if (userInfo.Id != Guid.Empty)
                    {
                        var talent = _talentService.GetTalentDetail(userInfo.Id.ToString());
                        await SetSignInCookie(userInfo, talent);
                    }

                    if (IsAjaxRequest())
                    {
                        PMS.UserManage.Application.ModelDto.Login.Post model = await UserToPost(userInfo, ReturnUrl);
                        if (userInfo.Id == default) model.status = (int)ResponseCode.Failed;
                        return Json(model);
                        //app登录
                        //return Json(new PMS.UserManage.Application.ModelDto.Login.Post()
                        //{
                        //    ID = userInfo.Id,
                        //    Nickname = userInfo.NickName,
                        //    HeadImgUrl = userInfo.HeadImgUrl,
                        //    ReturnUrl = ReturnUrl,
                        //    MobileBinded = !string.IsNullOrEmpty(userInfo.Mobile)
                        //});
                    }
                    else
                    {
                        //sign in fail
                        if (userInfo.Id == default)
                        {
                            if (IsMobile())
                            {
                                if (!string.IsNullOrWhiteSpace(ReturnUrl) && ReturnUrl.ToLower() != "/mine/")
                                {
                                    return Redirect($"/mine/account-ban/?ReturnUrl={ReturnUrl}");
                                }
                                else
                                {
                                    return Redirect($"/mine/account-ban/");
                                }
                            }
                            else
                            {
                                return Redirect($"/login/login-pc.html?returnUrl={ReturnUrl}&isBlocked=1");
                            }
                        }
                        return await LoginRedirect(userInfo, ReturnUrl);
                    }
                }
                else
                {
                    //已登录，微信授权绑定微信
                    if (WXuserinfo.openid != default)
                    {
                        WXuserinfo.AppName = weixin_AppName;
                        WXuserinfo.AppId = weixin_AppID;
                        try
                        {
                            //获取用户关注状态
                            WXuserinfo.OpenIdSubscribeStatus = await _weChatAppClient.GetSubscribeStatus(AuthAccessToken.openid);
                            if (!string.IsNullOrWhiteSpace(ReturnUrl) && ReturnUrl.ToLower().Contains("confirm=true")) WXuserinfo.ForceBind = true;
                        }
                        catch (Exception)
                        {
                        }
                    }
                    await _easyRedisClient.AddAsync("WXAuth:WXBindInfo-" + userID, WXuserinfo, DateTime.Now.AddSeconds(300), StackExchange.Redis.CommandFlags.FireAndForget);
                    return Redirect(ReturnUrl ?? Url.Action("BindWX", "ApiAccount"));
                }
            }
            return Redirect(ReturnUrl ?? "/login/");
        }
        /// <summary>
        /// 微信小程序登录
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
                case "org":
                    weixin_AppID = AppInfos.Weixin_MiniProgram_Org.AppID;
                    weixin_AppName = AppInfos.Weixin_MiniProgram_Org.AppName;
                    appsecret = AppInfos.Weixin_MiniProgram_Org.AppSecret;
                    break;
                case "shop":
                    weixin_AppID = AppInfos.Weixin_MiniProgram_Shop.AppID;
                    appsecret = AppInfos.Weixin_MiniProgram_Shop.AppSecret;
                    break;
                case "pay":
                    weixin_AppID = AppInfos.Weixin_MiniProgram_Pay.AppID;
                    appsecret = AppInfos.Weixin_MiniProgram_Pay.AppSecret;
                    break;
            }
            var AuthInfo = await _weixinUtil.GetWeiXinMiniProgramAuth(weixin_AppID, appsecret, data.Code);
            UserInfo userInfo = new UserInfo();
            bool isReg = false;
            if (AuthInfo.Errcode == 0)
            {
                if (!string.IsNullOrEmpty(data.IV))
                {
                    string infoJSON = _weixinUtil.AES_decrypt(data.EncryptedData, data.IV, AuthInfo.Session_Key);
                    var wxUserInfo = JsonConvert.DeserializeObject<WXMiniProgramUserInfo>(infoJSON);

                    //性别 0：未知、1：男、2：女
                    int? sex = wxUserInfo.Gender == 0 ? (int?)null : wxUserInfo.Gender == 2 ? 0 : 1;
                    userInfo.NickName = wxUserInfo.NickName;
                    userInfo.HeadImgUrl = wxUserInfo.AvatarUrl;
                    userInfo.Sex = sex;
                }
                else if (!string.IsNullOrWhiteSpace(data.NickName))
                {
                    //新版微信小程序2021.04.15后不再由后端解密用户数据
                    //性别 0：未知、1：男、2：女
                    int? sex = data.Gender == 0 ? (int?)null : data.Gender == 2 ? 0 : 1;
                    userInfo.NickName = data.NickName;
                    userInfo.HeadImgUrl = data.AvatarUrl;
                    userInfo.Sex = sex;
                }

                _OAuthWeixinService.WXLogin(AuthInfo.Unionid, AuthInfo.OpenID, AuthInfo.Session_Key, weixin_AppName, weixin_AppID, ref userInfo, ref isReg);
                if (userInfo.Id != Guid.Empty)
                {
                    var talent = _talentService.GetTalentDetail(userInfo.Id.ToString());
                    await SetSignInCookie(userInfo, talent);
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
            }
            return Json(ResponseResult.Failed("登录失败"), null);
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
            //var userId = Guid.Parse("73f232b5-086e-4db7-a1c0-38451ec5bf9a");
            string weixin_AppName = AppInfos.Weixin_MiniProgram_Sxkid.AppName;

            switch (data.Mp.ToLower())
            {
                case "org":
                    weixin_AppName = AppInfos.Weixin_MiniProgram_Org.AppName;
                    break;
                case "shop":
                    weixin_AppName = AppInfos.Weixin_MiniProgram_Shop.AppName;
                    break;
                case "pay":
                    weixin_AppName = AppInfos.Weixin_MiniProgram_Pay.AppName;
                    break;
            }

            var AuthInfo = _OAuthWeixinService.GetWXAuthInfo(userId, weixin_AppName);

            WXMiniProgramMobile mobileInfo = new WXMiniProgramMobile();
            string infoJSON = null; 
            try
            {
                infoJSON = _weixinUtil.AES_decrypt(data.EncryptedData, data.IV, AuthInfo.SessionKey);
                mobileInfo = JsonConvert.DeserializeObject<WXMiniProgramMobile>(infoJSON);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "微信小程序绑定手机号失败，infoJSON：{0}，userId:{1},EncryptedData：{2},IV:{3}", infoJSON, userId, data?.EncryptedData, data?.IV);
                return ResponseResult.Failed(ResponseCode.NoLogin, "解密失败，绑定手机失败");
            }

            if (!string.IsNullOrEmpty(mobileInfo?.purePhoneNumber))
            {
                if (_accountService.ListMobileConflict(86, mobileInfo.purePhoneNumber).Any())
                {
                    return ResponseResult.Failed("绑定失败，此号码已绑定其他账号", mobileInfo);
                }
                _accountService.BindMobile(userId, Convert.ToInt16(mobileInfo.countryCode), mobileInfo.purePhoneNumber);
                return ResponseResult.Success(mobileInfo);
            }
            return ResponseResult.Failed(mobileInfo);
        }

        /// <summary>
        /// 微信公众号获取用户信息
        /// </summary>
        /// <param name="openID"></param>
        /// <param name="unionid"></param>
        /// <param name="nickname"></param>
        /// <param name="headImgUrl"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult FwhWxAccount(string openID, string unionid, string nickname, string headImgUrl)
        {
            //todo:待做
            return ResponseResult.Success();
        }
        #endregion

        #region 第三方登录-QQ授权
        public async Task<IActionResult> QQAuth(string code, string state, string ReturnUrl)
        {
            if (string.IsNullOrEmpty(state) || string.IsNullOrEmpty(code) || code.Length != 32)
            {
                return Content("第三方登录授权失败");
            }
            string stateData = _easyRedisClient.GetAsync<string>("QQAuth:qq_redirect-" + state).Result;
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
                catch (Exception)
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
                    _OAuthQQService.QQLogin(OpenID.unionid, OpenID.openid, qq_AppName, ref userInfo);

                    //sign in fail
                    if (userInfo.Id == default)
                    {
                        if (IsMobile())
                        {
                            if (!string.IsNullOrWhiteSpace(ReturnUrl) && ReturnUrl.ToLower() != "/mine/")
                            {
                                return Redirect($"/mine/account-ban/?ReturnUrl={ReturnUrl}");
                            }
                            else
                            {
                                return Redirect($"/mine/account-ban/");
                            }
                        }
                        else
                        {
                            return Redirect($"/login/login-pc.html?returnUrl={ReturnUrl}&isBlocked=1");
                        }
                    }

                    var talent = _talentService.GetTalentDetail(userInfo.Id.ToString());
                    await SetSignInCookie(userInfo, talent);
                    if (!IsAjaxRequest())
                    {
                        //ViewBag.RedirectUri = ReturnUrl ?? "/mine/";
                        //return View("~/Views/Login/LoginAuth.cshtml");
                        return await LoginRedirectView(userInfo, ReturnUrl);
                    }
                    else
                    {
                        //return Json(new PMS.UserManage.Application.ModelDto.Login.Post()
                        //{
                        //    ID = userInfo.Id,
                        //    Nickname = userInfo.NickName,
                        //    HeadImgUrl = userInfo.HeadImgUrl,
                        //});
                        PMS.UserManage.Application.ModelDto.Login.Post model = await UserToPost(userInfo, ReturnUrl);
                        return Json(model);
                    }
                }
                else
                {
                    await _easyRedisClient.AddAsync("QQAuth:QQBindInfo-" + userID, OpenID, new TimeSpan(0, 0, 0, 300), StackExchange.Redis.CommandFlags.FireAndForget);
                    await _easyRedisClient.AddAsync("QQAuth:QQBindUserInfo-" + userID, QQuserinfo, new TimeSpan(0, 0, 0, 300), StackExchange.Redis.CommandFlags.FireAndForget);
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
        #endregion

        #region 第三方登录-百度授权
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
            var loginSucess = _OAuthBaiduService.Login(data.Code, ref userInfo, ref openId, ref isReg);

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
            else if (userInfo.Blockage)
            {
                var result = ResponseResult.Failed("当前账号已失效，如有疑问请致电020-89623079咨询");
                result.status = ResponseCode.NoLogin;
                result.Data = new
                {
                    IsBlocked = 1
                };
                return result;
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

            var bdAuthInfo = _OAuthBaiduService.GetAuthInfo(userId);

            //通过openId获取accessKey
            var userinfo = _OAuthBaiduService.GetUserInfo(data.EncryptedData, data.IV, bdAuthInfo.AccessKey);
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

            var bdAuthInfo = _OAuthBaiduService.GetAuthInfo(userId);

            //通过openId获取accessKey
            var info = _OAuthBaiduService.GetMobile(data.EncryptedData, data.IV, bdAuthInfo.AccessKey);

            if (!string.IsNullOrEmpty(info.Mobile))
            {
                _accountService.BindMobile(userId, 86, info.Mobile);
            }

            return ResponseResult.Success(info);
        }
        #endregion



        /// <summary>
        /// Token
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult GetToken([FromBody] TokenReqVM tokenReqVM)
        {
            if (tokenReqVM.GrantType != GrantType.Token)
            {
                return ResponseResult.Failed("GrantType类型错误, 请输入Token");
            }

            var isLogin = _loginService.Login(tokenReqVM.AppId, tokenReqVM.Secret);
            if (!isLogin)
            {
                return ResponseResult.Failed("AppId与Secret不匹配");
            }

            var claims = new[] {
                new Claim(ClaimTypes.Name, tokenReqVM.AppId),
            };
            var tokenString = TokenHelper.GetToken(claims);
            return ResponseResult.Success(tokenString);
        }

        [HttpGet]
        public ResponseResult ValidateTokenTest(string tokenString)
        {
            var principal = TokenHelper.ValidateTokenTest(tokenString);
            return ResponseResult.Success(principal?.Identity.IsAuthenticated);
        }
    }
}
