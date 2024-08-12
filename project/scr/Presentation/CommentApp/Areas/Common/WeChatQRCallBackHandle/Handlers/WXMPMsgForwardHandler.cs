using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PMS.TopicCircle.Application.Services;
using PMS.UserManage.Application.ModelDto;
using PMS.UserManage.Application.IServices;
using ProductManagement.API.Http.Interface;
using Sxb.Web.Areas.Common.Models;
using Sxb.Web.Response;
using WeChat;
using Sxb.Web.Utils;
using System.Web;
using ProductManagement.API.Http.Model;
using ProductManagement.Framework.Cache.Redis;
using WeChat.Interface;
using Sxb.Web.Common;
using System.Net.Http;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using PMS.Infrastructure.Application.IService;
using Sxb.Web.Areas.Common.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;

namespace Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers
{

    /// <summary>
    /// 对小程序客服消息进行转发至服务号扫码关注处理。
    /// </summary>
    public class WXMPMsgForwardHandler : ISubscribeCallBackHandler
    {
        IWeChatService _weChatService;
        HttpContext _httpContext;
        IWebHostEnvironment _webHostEnvironment;

        public WXMPMsgForwardHandler(IWeChatService weChatService
            , IHttpContextAccessor httpContext, IWebHostEnvironment webHostEnvironment)

        {
            _weChatService = weChatService;
            _httpContext = httpContext.HttpContext;
            _webHostEnvironment = webHostEnvironment;
        }


        public async Task<ResponseResult> Process(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest)
        {
            //发送小程序客服消息，一张服务号的二维码图片。关注后推送欢迎达人。
            string channel = frequest.AppName;
            //获取服务号的参数二维码
            string qrcodeUrl = await _weChatService.GenerateSenceQRCode(
                UtilsHelper.GetSubscribeCallBackHandlerUrl(new SubscribeCallBackQueryRequest()
                {
                    DataId = qrequest.DataId,
                    DataUrl = qrequest.DataUrl,
                    type = qrequest.ForwardType,
                    IsForwardByWXMP = true,
                    ForwardData = Newtonsoft.Json.JsonConvert.SerializeObject(frequest.MiniProgramCardInfo),
                }, _httpContext.Request)
                ,1);
            var qrImgData = await DownloadImg(qrcodeUrl);

            //上传二维码到微信的临时素材
            string mediaId = await _weChatService.AddTempMedia(MediaType.image, qrImgData, "tempqrcode.jpeg", channel);
            if (!_webHostEnvironment.IsProduction())
            {
                //调式信息
                await _weChatService.SendText(frequest.OpenId, $"调式信息。\n 我收到的参数：{qrequest} | {frequest}",channel);
            }
            //发送图片客服消息
            await _weChatService.SendImage(frequest.OpenId, mediaId, channel);

            return ResponseResult.Success("OK");
        }


        private async Task<byte[]> DownloadImg(string link)
        {
            using (HttpClient client = new HttpClient())
            {
                byte[] data = await client.GetByteArrayAsync(link);
                return data;
            }

        }
    }
}
