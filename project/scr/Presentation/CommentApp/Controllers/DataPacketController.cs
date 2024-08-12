using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PMS.OperationPlateform.Application.IServices;
using PMS.TopicCircle.Application.Services;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using Sxb.Web.Areas.Common.Models;
using Sxb.Web.Areas.Common.WeChatQRCallBackHandle;
using Sxb.Web.Utils;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using WeChat.Interface;
using WeChat.Model;

namespace Sxb.Web.Controllers
{
    public class DataPacketController : BaseController
    {
        private readonly ILogger<DataPacketController> _logger;
        private readonly IWeChatAppClient _weChatAppClient;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly IHtmlServiceClient _htmlServiceClient;
        private readonly IWeChatQRCodeService _weChatQRCodeService;
        private readonly IFileServiceClient _fileServiceClient;
        private readonly IDataPacketService _dataPacketService;

        public DataPacketController(ILogger<DataPacketController> logger, IWeChatAppClient weChatAppClient, IEasyRedisClient easyRedisClient, IHtmlServiceClient htmlServiceClient, IWeChatQRCodeService weChatQRCodeService, IDataPacketService dataPacketService, IFileServiceClient fileServiceClient)
        {
            _logger = logger;
            _weChatAppClient = weChatAppClient;
            _easyRedisClient = easyRedisClient;
            _htmlServiceClient = htmlServiceClient;
            _weChatQRCodeService = weChatQRCodeService;
            _dataPacketService = dataPacketService;
            _fileServiceClient = fileServiceClient;
        }

        public async Task<IActionResult> AdWxData(Guid id, long ticks)
        {
            //log dataId
            _logger.LogInformation("获取资料包中间页,id={0} ticks={2}", id, ticks);

            ViewBag.ImageUrl = await GetWxDataImage(id);
            return View();
        }

        [HttpGet]
        public async Task<string> GetWxDataImage(Guid id)
        {
            var key = string.Format(RedisKeys.AdDataPacketMiddlePageKey, id);
            var imageUrl = await _easyRedisClient.GetAsync<string>(key);
            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                return imageUrl;
            }

            //背景图片
            var baseImageUrl = "https://cos.sxkid.com/images/mkt/d0cb18c68f3f40a8a1e448613617b86c.jpg";

            string dataUrl = "";
            //回调二维码图片
            string sceneKey = $"ad:wxdata:{id:N}";
            string subscribeHandleUrl = $"{ConfigHelper.GetHost()}/common/wechat/SubscribeCallBack?DataId={id}&dataUrl={dataUrl}&type={(int)SubscribeQRCodeType.AdWxData}";
            var qrcodeImageUrl = await GetQrCode(sceneKey, subscribeHandleUrl);

            using (var ms = await ComposeImages(baseImageUrl, qrcodeImageUrl, 267, 960, 216, 216))
            {
                //上传到cdn, 并缓存cdn url
                var uploadResp = await _fileServiceClient.UploadDataPacketImage($"{Guid.NewGuid()}.png", ms);//上学帮让升学更简单.png

                if (uploadResp == null)
                {
                    return string.Empty;
                }

                imageUrl = uploadResp.cdnUrl;

                await _easyRedisClient.AddAsync(key, imageUrl, TimeSpan.FromDays(25));
                return imageUrl;
            }
        }


        [NonAction]
        public async Task<MemoryStream> ComposeImages(string baseImageUrl, string qrcodeImageUrl, int x, int y, int width, int height)
        {
            //背景图片
            using (var baseImage = await _htmlServiceClient.GetImage(baseImageUrl))
            {
                //二维码图片
                using (var qrcodeImage = await _htmlServiceClient.GetImage(qrcodeImageUrl))
                {
                    return await GraphicsHelper.ComposeImages(baseImage, qrcodeImage, x, y, width, height);
                }
            }
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


        [HttpGet]
        public async Task<object> Summary(DateTime startDate, DateTime endDate)
        {
            return await _dataPacketService.Summary(startDate, endDate);
        }

        [HttpGet]
        public async Task<IActionResult> ExportExcel(DateTime startDate, DateTime endDate)
        {
            var data = await _dataPacketService.Summary(startDate, endDate);
            var userdata =  await _dataPacketService.UserSummary(startDate, endDate);
            var helper = NPOIHelper.NPOIHelperBuild.GetHelper();
            helper.Add("数据总表", data);
            helper.Add("用户行为表", userdata);

            helper.FileName = "资料包报表";
            return File(helper.ToArray(), helper.ContentType, helper.FullName);
        }

        [HttpGet]
        public async Task<object> UserSummary(DateTime startDate, DateTime endDate)
        {
            return await _dataPacketService.UserSummary(startDate, endDate);
        }
    }
}