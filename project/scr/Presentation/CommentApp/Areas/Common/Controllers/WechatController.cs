using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PMS.Infrastructure.Application.IService;
using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.UserManage.Application.IServices;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using ProductManagement.Tool.QRCoder;
using Sxb.Web.Areas.Common.Models;
using Sxb.Web.Areas.Common.WeChatQRCallBackHandle;
using Sxb.Web.Areas.TopicCircle.Models;
using Sxb.Web.Controllers;
using Sxb.Web.Filters;
using Sxb.Web.Response;
using Sxb.Web.Utils;
using WeChat;
using WeChat.Interface;
using WeChat.Model;
using IUserService = PMS.UserManage.Application.IServices.IUserService;
using SubscribeCallBackQueryRequest = Sxb.Web.Areas.Common.WeChatQRCallBackHandle.SubscribeCallBackQueryRequest;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.Web.Areas.Common.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class WechatController : ApiBaseController
    {

        internal IHtmlServiceClient _htmlServiceClient;
        internal IWeChatAppClient _weChatAppClient;
        internal IEasyRedisClient _easyRedisClient;

        internal IUserService _userService;
        internal IAccountService _accountService;
        IWeChatQRCodeService _weChatQRCodeService;
        ILogger<WechatController> _logger;
        private readonly IDataPacketService _dataPacketService;
        IHostEnvironment _hostEnvironment;
        IWeChatService _weChatService;
        public WechatController(IWeChatAppClient weChatAppClient
            , IEasyRedisClient easyRedisClient
            , IUserService userService
            , IAccountService accountService
            , IWeChatQRCodeService weChatQRCodeService
            , ILogger<WechatController> logger
            , IDataPacketService dataPacketService
            , IHtmlServiceClient htmlServiceClient
            , IHostEnvironment hostEnvironment
            , IWeChatService weChatService)
        {
            _weChatAppClient = weChatAppClient;
            _easyRedisClient = easyRedisClient;
            _userService = userService;
            _accountService = accountService;
            _weChatQRCodeService = weChatQRCodeService;
            _logger = logger;
            _dataPacketService = dataPacketService;
            _htmlServiceClient = htmlServiceClient;
            _hostEnvironment = hostEnvironment;
            _weChatService = weChatService;
        }

        [HttpGet]
        [Description("获取服务号二维码")]
        public async Task<ResponseResult> GetFwhQrCode(int day = 30)
        {
            day = day > 30 || day <= 0 ? 30 : day;

            //最大30天
            int expire_second = (int)TimeSpan.FromDays(day).TotalSeconds;
            var Scene = new CreateTempQRStrSceneCodeRequest(expire_second);

            var accessToken = await _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" });
            var qrcodeResponse = await _weChatQRCodeService.GenerateTempQRCode(accessToken.token, Scene);

            var isBindFwh = UserId != null && _userService.CheckIsSubscribeFwh(UserId.Value, ConfigHelper.GetFwh());
            return ResponseResult.Success(new { QrCode = qrcodeResponse?.ImgUrl, IsBindFwh = isBindFwh });
        }

        /// <summary>
        /// 获取文章的关注二维码
        /// </summary>
        /// <param name="circleId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Description("获取文章的关注二维码")]
        public async Task<ResponseResult> GetSubscribeArticleQrCode(Guid articleId)
        {
            string subscribeHandleUrl = $"{ConfigHelper.GetHost()}/tpc/Share/SubscribeCallBack?DataId={articleId}&type={(int)SubscribeCallBackType.articleview}";

            string sceneKey = $"articleview:{articleId:N}";
            int expire_second = (int)TimeSpan.FromDays(30).TotalSeconds; //30天有效期的二维码

            var Scene = new CreateTempQRStrSceneCodeRequest(expire_second);
            Scene.SetScene(sceneKey);
            _ = _easyRedisClient.AddStringAsync(sceneKey, subscribeHandleUrl, TimeSpan.FromSeconds(expire_second));

            var accessToken = await _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" });
            var qrcodeResponse = await _weChatQRCodeService.GenerateTempQRCode(accessToken.token, Scene);

            var isBindFwh = UserId != null && _userService.CheckIsSubscribeFwh(UserId.Value, ConfigHelper.GetFwh());
            return ResponseResult.Success(new { QrCode = qrcodeResponse?.ImgUrl, IsBindFwh = isBindFwh });
        }


        /// <summary>
        /// 获取资料包中间页, 扫码关注
        /// </summary>
        /// <param name="dataId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> AdWxData(Guid id, long ticks)
        {
            //log dataId
            _logger.LogInformation("获取资料包中间页,id={0} ticks={2}", id, ticks);

            //背景图片
            var basImageUrl = "https://cos.sxkid.com/images/mkt/d0cb18c68f3f40a8a1e448613617b86c.jpg";
            var baseImage = await _htmlServiceClient.GetImage(basImageUrl);

            string dataUrl = "";
            //回调二维码图片
            string sceneKey = $"ad:wxdata:{id:N}";
            string subscribeHandleUrl = $"{ConfigHelper.GetHost()}/common/wechat/SubscribeCallBack?DataId={id}&dataUrl={dataUrl}&type={(int)SubscribeQRCodeType.AdWxData}";
            var qrcodeImageUrl = await GetQrCode(sceneKey, subscribeHandleUrl);
            var qrcodeImage = await _htmlServiceClient.GetImage(qrcodeImageUrl);

            //指定位置画二维码
            using (Graphics graphics = Graphics.FromImage(baseImage))
            {
                graphics.DrawImage(qrcodeImage, 267, 960, 216, 216);
            }

            var ms = new MemoryStream();
            baseImage.Save(ms, ImageFormat.Png);

            baseImage.Dispose();
            qrcodeImage.Dispose();

            //上传到cdn, 并缓存cdn url

            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, MimeTypeMap.GetMimeType(ImageFormat.Png.ToString()), "上学帮让升学更简单.png");
        }


        /// <summary>
        /// 生成网课通拉新推广顾问海报
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GenerateWangKeTongGuWenHaiBao(string city = "cd")
        {
            SubscribeCallBackQueryRequest queryRequest = new SubscribeCallBackQueryRequest() { 
              DataId = UserIdOrDefault,
              type = SubscribeQRCodeType.WangKeTongGuWenHaiBao,
              DataJson = Newtonsoft.Json.JsonConvert.SerializeObject(new { city = city })
            };
            string qrcodeUrl = await _weChatService.GenerateSenceQRCode(UtilsHelper.GetSubscribeCallBackHandlerUrl(queryRequest, Request), 30);
           var userInfo =  _userService.GetUserInfo(UserIdOrDefault);
            Image headImage = null;
            if (userInfo != null && !string.IsNullOrEmpty(userInfo.HeadImgUrl))
            {
                headImage = await GraphicsHelper.ImageFromUrl(userInfo.HeadImgUrl);
            }
            string rootPath = Path.Combine(_hostEnvironment.ContentRootPath, $"Resources/WangKeTong/guwen_{city}.png");
            Rectangle headImageRectangle = Rectangle.Empty, qrCodeImageRectangle = Rectangle.Empty;
            if (city == "cd")
            {
                headImageRectangle = new Rectangle(85,1255, 53, 53);
                qrCodeImageRectangle = new Rectangle(501, 1144, 165, 165);

            }
            if (city == "gz")
            {
                headImageRectangle = new Rectangle(39, 1181, 55, 55);
                qrCodeImageRectangle = new Rectangle(512, 1089, 180, 180);

            }
            var haibaoms = GraphicsHelper.CreateWangKeTongGuWenHaiBao(
                Image.FromFile(rootPath)
                , headImage
                ,headImageRectangle
                , await GraphicsHelper.ImageFromUrl(qrcodeUrl)
                ,qrCodeImageRectangle);

            return File(haibaoms,"image/png");
        }

        [NonAction]
        public async Task<string> GetQrCode(string sceneKey, string subscribeHandleUrl)
        {
            int expire_second = (int)TimeSpan.FromDays(30).TotalSeconds; //30天有效期的二维码

            var Scene = new CreateTempQRStrSceneCodeRequest(expire_second);
            Scene.SetScene(sceneKey);
            _ = _easyRedisClient.AddStringAsync(sceneKey, subscribeHandleUrl, TimeSpan.FromSeconds(expire_second));

            var accessToken = await _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" });
            var qrcodeResponse = await _weChatQRCodeService.GenerateTempQRCode(accessToken.token, Scene);

            return qrcodeResponse?.ImgUrl;
        }
    }
}
