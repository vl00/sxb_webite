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
    public class CourseOrderDetailPageHandler : ISubscribeCallBackHandler
    {
        IWeChatService _weChatService;
        IOrgServiceClient _orgServiceClient;
        IWebHostEnvironment _webHostEnvironment;
        ILogger<CourseOrderDetailPageHandler> _logger;
        public CourseOrderDetailPageHandler(ILogger<CourseOrderDetailPageHandler> logger, IWeChatService weChatService, IOrgServiceClient orgServiceClient, IWebHostEnvironment webHostEnvironment)
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
               List<string> banners = await _orgServiceClient.OrgsOrderBanners(new OrgOrderBannerRequest()
                {
                    AdvanceOrderId = qrequest.DataId.Value
                });
                _logger.LogInformation("订单详情进来了");

                if (_webHostEnvironment.IsProduction())
                {
                    await SendMp(qrequest, frequest, banners.FirstOrDefault());
                }
                else
                {
                    await SendMpTest(qrequest, frequest, banners.FirstOrDefault());
                }

                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "发订单详情卡片异常" );
               // _logger.LogInformation("发订单详情卡片异常" +ex.Message);
            }
            return ResponseResult.Success("OK");

        }

        private async Task SendMpTest(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest, string banner)
        {


            await _weChatService.SendText(frequest.OpenId, $"发送了网课通小程序订单页面,banner:{banner.FirstOrDefault()}");
            await _weChatService.SendText(frequest.OpenId, qrequest.DataUrl);
        }

        private async Task SendMp(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest, string banner)
        {
            string mediaId = "ih5M-3iKDsME3MNG3h5Q3lTqw8mNMydo8P4GqUx3t8Y";
            //上传二维码到微信的临时素材
            if (!string.IsNullOrEmpty(banner)) 
             mediaId = await _weChatService.AddTempMedia(MediaType.image, banner, "tempqrcode.jpeg");
            var appid = ConfigHelper.GetWxMpOrgAppId();


            var ret = await _weChatService.SendMiniProgramCard(frequest.OpenId,
                appid,
                qrequest.DataUrl,
                mediaId ?? "ih5M-3iKDsME3MNG3h5Q3lTqw8mNMydo8P4GqUx3t8Y",
                title: "查看订单详细信息");

           

        }
    }
}
