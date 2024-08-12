using Microsoft.Extensions.Options;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model;
using ProductManagement.Infrastructure.Configs;
using Sxb.Web.Areas.Common.Models;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeChat;
using WeChat.Interface;

namespace Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers.Topic
{

    /// <summary>
    /// 处理绑定账户场景回调
    /// </summary>
    public class BindAccountHandler : ISubscribeCallBackHandler
    {
        IWeChatAppClient _weChatAppClient;
        TopicOption _topicOption;
        ICustomMsgService _customMsgService;
        public BindAccountHandler(IWeChatAppClient weChatAppClient
            , IOptions<TopicOption> options
            , ICustomMsgService customMsgService)
        {
            _weChatAppClient = weChatAppClient;
            _topicOption = options.Value;
            _customMsgService = customMsgService;
        }

        public async Task<ResponseResult> Process(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest)
        {
            NewsCustomMsg msg = new NewsCustomMsg()
            {
                ToUser  = frequest.OpenId,
                Url = _topicOption.BindAccountKFMsg.RedirectUrl.Replace("{userId}", qrequest.DataId.ToString()),
                Description = _topicOption.BindAccountKFMsg.Description,
                PicUrl = _topicOption.BindAccountKFMsg.ImgUrl,
                Title = _topicOption.BindAccountKFMsg.Title
            };
            var accessToken = await _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" }) ;
            var result = await _customMsgService.Send(accessToken.token, msg);

            return new ResponseResult() { Succeed = result.Success };
        }
    }
}
