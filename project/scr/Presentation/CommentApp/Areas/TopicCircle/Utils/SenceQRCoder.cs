using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model;
using ProductManagement.API.Http.Service;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using Sxb.Web.Areas.TopicCircle.Models;
using Sxb.Web.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeChat;
using WeChat.Interface;
using WeChat.Model;

namespace Sxb.Web.Areas.TopicCircle.Utils
{
    public class SenceQRCoder:IDisposable
    {

        IWeChatAppClient _weChatAppClient;
        IEasyRedisClient _easyRedisClient;
        IWeChatQRCodeService _weChatQRCodeService;
        public SenceQRCoder(IWeChatAppClient weChatAppClient, IEasyRedisClient easyRedisClient, IWeChatQRCodeService weChatQRCodeService)
        {
            _weChatAppClient = weChatAppClient;
            _easyRedisClient = easyRedisClient;
            _weChatQRCodeService = weChatQRCodeService;
        }


        public async Task<string> GetSenceQRCode(SubscribeCallBackQueryRequest request, double expireDays = 1, bool isMiniApp = false)
        {
            string paramMd5 = DesTool.Md5($"{request.DataId}{request.DataUrl}{request.Type}{isMiniApp}");
            string code = await _easyRedisClient.GetStringAsync(paramMd5);
            if (string.IsNullOrEmpty(code))
            {


                string sceneKey = Guid.NewGuid().ToString("N");
                int expire_second = (int)TimeSpan.FromDays(expireDays).TotalSeconds;
                var accessToken = await this._weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" });
                string subscribeHandleUrl = $"{ConfigHelper.GetHost()}/tpc/Share/SubscribeCallBack?DataId={request.DataId}&DataUrl={request.DataUrl}&type={(int)request.Type}";

                if (isMiniApp)
                {
                    subscribeHandleUrl += "&isMiniApp=true";
                }

                _ = this._easyRedisClient.AddStringAsync(sceneKey, subscribeHandleUrl, TimeSpan.FromSeconds(expire_second));
                var Scene = new CreateTempQRStrSceneCodeRequest(expire_second);
                Scene.SetScene(sceneKey);
                var qrcodeResponse = await _weChatQRCodeService.GenerateTempQRCode(accessToken.token, Scene);
                bool result = await _easyRedisClient.AddStringAsync(paramMd5, qrcodeResponse?.ImgUrl, TimeSpan.FromDays(expireDays));
                return qrcodeResponse?.ImgUrl;
            }
            else {
                return code;
            }

        }
        public void Dispose()
        {

        }


    }

}
