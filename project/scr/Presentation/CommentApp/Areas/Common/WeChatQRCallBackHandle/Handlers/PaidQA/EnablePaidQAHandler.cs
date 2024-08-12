using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model;
using Sxb.Web.Areas.Common.Models;
using Sxb.Web.Response;
using System.Threading.Tasks;
using WeChat;
using WeChat.Interface;

namespace Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers.PaidQA
{

    public class EnablePaidQAHandler : ISubscribeCallBackHandler
    {
        IWeChatAppClient _weChatAppClient;
        ICustomMsgService _customMsgService;
        public EnablePaidQAHandler(IWeChatAppClient weChatAppClient, ICustomMsgService customMsgService)
        {
            _weChatAppClient = weChatAppClient;
            _customMsgService = customMsgService;
        }

        public async Task<ResponseResult> Process(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest)
        {
            NewsCustomMsg msg = new NewsCustomMsg()
            {
                ToUser = frequest.OpenId,
                Url = qrequest.DataUrl,
                Description = "马上开启达人专属功能，彰显您的知识技能价值！",
                PicUrl = "https://cos.sxkid.com/images/navigationicon/cf90a312-3385-41d2-83c7-ea64d1d02ad5/da0a23eb-971b-41a0-98d8-90af75519f54.png",
                Title = "点这开通您的专属上学问功能"
            };
            var accessToken = await _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" });
            var result = await _customMsgService.Send(accessToken.token, msg);

            return new ResponseResult() { Succeed = result.Success };
        }

    }

}