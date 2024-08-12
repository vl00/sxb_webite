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
    /// 欢迎达人关注场景回调
    /// </summary>
    public class WelcomTalentHandler : ISubscribeCallBackHandler
    {
        IWeChatAppClient _weChatAppClient;
        TopicOption _topicOption;
        ICustomMsgService _customMsgService;
        public WelcomTalentHandler(IWeChatAppClient weChatAppClient
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
                ToUser = frequest.OpenId,
                Url = qrequest.DataUrl,
                Description = _topicOption.WelcomTalentCreateKFMsg.Description,
                PicUrl = _topicOption.WelcomTalentCreateKFMsg.ImgUrl,
                Title = _topicOption.WelcomTalentCreateKFMsg.Title
            };
            var accessToken = await _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" });
            var result = await _customMsgService.Send(accessToken.token,msg);

            return new ResponseResult() { Succeed = result.Success };
        }
    }

}
