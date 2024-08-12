
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using PMS.Infrastructure.Application.IService;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model;
using Sxb.Web.Areas.Common.Models;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeChat;

namespace Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers
{
    public class DefaultHandler : ISubscribeCallBackHandler
    {
        IWeChatService _weChatService;
        IWebHostEnvironment _webHostEnvironment;
        public DefaultHandler(IWeChatService weChatService, IWebHostEnvironment webHostEnvironment)
        {
            _weChatService = weChatService;
            _webHostEnvironment = webHostEnvironment;
        }

        public virtual async Task<ResponseResult> Process(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest)
        {
            if (!_webHostEnvironment.IsProduction())
            {
                await _weChatService.SendText(frequest.OpenId, $"默认场景，测试心跳。\n 我收到的参数：{qrequest} | {frequest},use new method");

                await _weChatService.SendNews(frequest.OpenId, "图文消息", "来自心跳测试", "http://thirdwx.qlogo.cn/mmopen/ajiaJtCRiavibJePI9wIajJ6uCqRhfRr6zRzZI9ASMo7ibDAtXLGib4GygxjicZZEfGswVwMHjiby5GG1MxjmkxzjKVRURaZrYMQDw2/132", qrequest.DataUrl);

            }
            return ResponseResult.Success("OK");
        }
    }
}
