using Newtonsoft.Json;
using PMS.Infrastructure.Application.IService;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model.Org;
using ProductManagement.API.Http.Request.Org;
using ProductManagement.API.Http.Result.Org;
using Sxb.Web.Areas.Common.Models;
using Sxb.Web.Response;
using Sxb.Web.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Web;
using WeChat.Interface;
using ProductManagement.Framework.Foundation;
using Sxb.Web.Areas.Common.WeChatQRCallBackHandle.DataJson;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers
{
    public class OrgCommonPageHandler : ISubscribeCallBackHandler
    {
        IWeChatService _weChatService;
        IOrgServiceClient _orgServiceClient;
        IWebHostEnvironment _webHostEnvironment;
        ILogger<CourseOrderDetailPageHandler> _logger;
        public OrgCommonPageHandler(ILogger<CourseOrderDetailPageHandler> logger, IWeChatService weChatService, IOrgServiceClient orgServiceClient, IWebHostEnvironment webHostEnvironment)
        {
            _weChatService = weChatService;
            _orgServiceClient = orgServiceClient;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public async Task<ResponseResult> Process(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest)
        {

            try
            {
              
                _logger.LogInformation("机构通用页面进来了");

                if (_webHostEnvironment.IsProduction())
                {
                    await SendMp(qrequest, frequest);
                }
                else
                {
                    await SendMpTest(qrequest, frequest);
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "发机构通用页面卡片异常");
                // _logger.LogInformation("发订单详情卡片异常" +ex.Message);
            }
            return ResponseResult.Success("OK");

        }

        private async Task SendMpTest(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest)
        {


            await _weChatService.SendText(frequest.OpenId, $"发机构通用页面卡片");
            await _weChatService.SendText(frequest.OpenId, qrequest.DataUrl);
        }

        private async Task SendMp(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest)
        {
            string mediaId = "ih5M-3iKDsME3MNG3h5Q3lTqw8mNMydo8P4GqUx3t8Y";
            //上传二维码到微信的临时素材
            mediaId = await _weChatService.AddTempMedia(MediaType.image, "https://cos.sxkid.com/images/miniprogram/miniprogram/share-imageUrl.png", "tempqrcode.jpeg");
            var appid = ConfigHelper.GetWxMpOrgAppId();

            if (qrequest.DataUrl== "/pagesPlant/pages/plant-publish")//兼容。前端发布麻烦
            {
                qrequest.DataUrl = "pagesPlant/pages/plant-publish/index";
            }
            var ret = await _weChatService.SendMiniProgramCard(frequest.OpenId,
                appid,
                qrequest.DataUrl,
                mediaId ?? "ih5M-3iKDsME3MNG3h5Q3lTqw8mNMydo8P4GqUx3t8Y",
                title: "继续发布种草");



        }
    }
}
