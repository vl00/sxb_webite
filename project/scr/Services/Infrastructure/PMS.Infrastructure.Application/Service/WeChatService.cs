using Newtonsoft.Json.Linq;
using PMS.Infrastructure.Application.Enums;
using PMS.Infrastructure.Application.IService;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WeChat;
using WeChat.Interface;
using WeChat.Model;

namespace PMS.Infrastructure.Application.Service
{
    public class WeChatService : IWeChatService
    {
        IEasyRedisClient _easyRedisClient;
        IWeChatAppClient _weChatAppClient;
        IWeChatQRCodeService _weChatQRCodeService;
        IMediaService _mediaService;
        ITemplateMessageService _templateMessageService;
        IWeChatSearchService _weChatSearchService;
        IHttpClientFactory _httpClientFactory;
        ICustomMsgService _customMsgService;
        public WeChatService(IWeChatAppClient weChatAppClient
            , IEasyRedisClient easyRedisClient
            , IWeChatQRCodeService weChatQRCodeServices
            , IMediaService mediaService
            , ITemplateMessageService templateMessageService
            , IWeChatSearchService weChatSearchService
            , IHttpClientFactory httpClientFactory
            , ICustomMsgService customMsgService)
        {
            _weChatAppClient = weChatAppClient;
            _easyRedisClient = easyRedisClient;
            _weChatQRCodeService = weChatQRCodeServices;
            _mediaService = mediaService;
            _templateMessageService = templateMessageService;
            _weChatSearchService = weChatSearchService;
            _httpClientFactory = httpClientFactory;
            _customMsgService = customMsgService;
        }

        /// <summary>
        /// 生成一个场景二维码，在/Common/WeChat/SubscribeCallBack中接收微信回调的参数。
        /// 主要参数有关注者的OpenID
        /// </summary>
        /// <param name="request">可以通过该参数保存你的业务ID，业务链接，业务场景</param>
        /// <param name="expireDays"></param>
        /// <returns></returns>
        public async Task<string> GenerateSenceQRCode(string callBackUrl, double expireDays = 1)
        {
            string paramMd5 = $"wxqrcode_{DesTool.Md5(callBackUrl)}";
            string code = await _easyRedisClient.GetStringAsync(paramMd5);
            if (string.IsNullOrEmpty(code))
            {
                string sceneKey = Guid.NewGuid().ToString("N");
                int expire_second = (int)TimeSpan.FromDays(expireDays).TotalSeconds;
                var accessToken = await _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" });
                _ = _easyRedisClient.AddStringAsync(sceneKey, callBackUrl, TimeSpan.FromSeconds(expire_second));
                var Scene = new CreateTempQRStrSceneCodeRequest(expire_second);
                Scene.SetScene(sceneKey);
                var qrcodeResponse = await _weChatQRCodeService.GenerateTempQRCode(accessToken.token, Scene);
                if (!string.IsNullOrEmpty(qrcodeResponse.ticket))
                {
                    await _easyRedisClient.AddStringAsync(paramMd5, qrcodeResponse?.ImgUrl, TimeSpan.FromDays(expireDays));

                }
                return qrcodeResponse?.ImgUrl;
            }
            else
            {
                return code;
            }

        }




        public async Task<bool> SendImage(string toUser, string mediaId, string channel = "fwh")
        {
            var accessToken = await _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = channel });
            //发送图片客服消息
            ImageCustomMsg msg = new ImageCustomMsg()
            {
                MediaId = mediaId,
                ToUser = toUser
            };

            var result = await _customMsgService.Send(accessToken.token, msg);
            return result.Success;
        }

        public async Task<bool> SendNews(string toUser, string title, string desc, string picUrl, string url, string channel = "fwh")
        {
            var accessToken = await _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = channel });
            NewsCustomMsg msg = new NewsCustomMsg()
            {
                ToUser = toUser,
                Title = title,
                Description = desc,
                PicUrl = picUrl,
                Url = url
            };
            var result = await _customMsgService.Send(accessToken.token, msg);
            return result.Success;

        }

        public async Task<bool> SendText(string toUser, string content, string channel = "fwh")
        {
            var accessToken = await _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = channel });
            TextCustomMsg msg = new TextCustomMsg()
            {
                ToUser = toUser,
                content = content
            };
            var result = await _customMsgService.Send(accessToken.token, msg);
            return result.Success;

        }
        public async Task<bool> SendMiniProgramCard(string toUser, string appId, string pagePath, string thumbMediaId, string title, string channel = "fwh")
        {
            var accessToken = await _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = channel });
            MiniProgramCustomMsg msg = new MiniProgramCustomMsg()
            {
                ToUser = toUser,
                Appid = appId,
                Pagepath = pagePath,
                ThumbMediaId = thumbMediaId,
                Title = title
            };
            var result = await _customMsgService.Send(accessToken.token, msg);
            return result.Success;

        }

        public async Task SendTemplateMessage(SendTemplateRequest templateParam, string channel = "fwh")
        {
            var accessToken = await _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = channel });
            var result = await _templateMessageService.SendAsync(accessToken.token, templateParam);

        }


        public async Task<string> AddTempMedia(MediaType mediaType, byte[] data, string filename, string channel = "fwh")
        {
            var accessToken = await _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = channel });
            return await _mediaService.AddTempMedia(accessToken.token, mediaType, data, filename);
        }

        public async Task<string> AddTempMedia(MediaType mediaType, string pageUrl, string filename, string channel = "fwh")
        {
            using (HttpClient client = new HttpClient())
            {
                byte[] data = await client.GetByteArrayAsync(pageUrl);
                var accessToken = await _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = channel });
                return await _mediaService.AddTempMedia(accessToken.token, mediaType, data, filename);
            }
        }

        public async Task<string> UpsertPreUniversityBasic(UpsertPreUniversityBasicRequest request, WeChatAppChannel channel = WeChatAppChannel.fwh)
        {
            var accessToken = await _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = channel.ToString() });
            return await _weChatSearchService.UpsertPreUniversityBasic(accessToken.token, request);

        }

        public async Task<string> GetUnionId(string openId, WeChatAppChannel channel = WeChatAppChannel.fwh)
        {
            using (var client = _httpClientFactory.CreateClient("api.weixin.qq.com"))
            {
                var accessToken = await _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = channel.ToString() });
                var wxUserInfoJsonRes = await client.GetStringAsync($"https://api.weixin.qq.com/cgi-bin/user/info?access_token={accessToken.token}&openid={openId}&lang=zh_CN");
                string unionid = string.Empty;
                if (JObject.Parse(wxUserInfoJsonRes).TryGetValue("unionid", out JToken unionidToken))
                {
                    unionid = unionidToken.Value<string>();
                }
                return unionid;
            }

        }



    }
}
