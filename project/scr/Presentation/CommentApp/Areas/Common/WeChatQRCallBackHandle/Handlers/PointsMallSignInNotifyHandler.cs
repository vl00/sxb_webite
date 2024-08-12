using PMS.Infrastructure.Application.IService;
using ProductManagement.API.Http.Interface;
using Sxb.Web.Areas.Common.Models;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using Newtonsoft.Json;
using WeChat.Interface;
using Sxb.Web.Utils;
using ProductManagement.Framework.Foundation;
using Sxb.Web.Areas.Common.WeChatQRCallBackHandle.DataJson;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using DotNetCore.CAP;
using Sxb.Web.Application;
using Microsoft.Extensions.Logging;

namespace Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers
{
    public class PointsMallSignInNotifyHandler : ISubscribeCallBackHandler
    {
        IWeChatService _weChatService;
        IWebHostEnvironment _webHostEnvironment;

        public PointsMallSignInNotifyHandler(IWeChatService weChatService, IWebHostEnvironment webHostEnvironment)
        {
            _weChatService = weChatService;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<ResponseResult> Process(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest)
        {
            if (qrequest.DataUrl == null)
            {
                return ResponseResult.Failed("DataUrl不能为空");
            }
            if (_webHostEnvironment.IsProduction())
            {
                await SendMp(qrequest, frequest);
            }
            else
            {
                await SendMpTest(qrequest, frequest);
            }
            return ResponseResult.Success("OK");

        }

        private async Task SendMpTest(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest)
        {
            var pagePath = qrequest.DataUrl;

            await _weChatService.SendText(frequest.OpenId, "发送了小程序页面");
            await _weChatService.SendText(frequest.OpenId, pagePath);
        }

        private async Task SendMp(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest)
        {
            var appid = ConfigHelper.GetWxMpOrgAppId();
            var pagePath = qrequest.DataUrl;

            var ret = await _weChatService.SendMiniProgramCard(frequest.OpenId,
                appid,
                pagePath,
                "ih5M-3iKDsME3MNG3h5Q3lTqw8mNMydo8P4GqUx3t8Y",
                title: $"点击返回");
        }
    }
}
