using PMS.Infrastructure.Application.IService;
using ProductManagement.API.Http.Interface;
using Sxb.Web.Areas.Common.Models;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers
{
    public class SchoolComparePageHandler : ISubscribeCallBackHandler
    {
        IWeChatService _weChatService;
        public SchoolComparePageHandler(IWeChatService weChatService)
        {
            _weChatService = weChatService;
        }

        public  async Task<ResponseResult> Process(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest)
        {
            await _weChatService.SendNews(frequest.OpenId
                 , "点击使用学校对比功能"
                 , "感谢关注上学帮！点击开始使用学校对比功能吧！"
                 , "https://cdn.sxkid.com/images/logo_share_v4.png"
                 , qrequest.DataUrl);
            return ResponseResult.Success("OK");
        }
    }
}
