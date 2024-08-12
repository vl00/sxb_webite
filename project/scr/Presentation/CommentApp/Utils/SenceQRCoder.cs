using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model;
using ProductManagement.API.Http.Service;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using Sxb.Web.Areas.Common.Models;
using Sxb.Web.Areas.Common.WeChatQRCallBackHandle;
using Sxb.Web.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WeChat;
using WeChat.Interface;
using WeChat.Model;

namespace Sxb.Web.Utils
{
    public class SenceQRCoder
    {

        /// <summary>
        /// 生成一个场景二维码，在/Common/WeChat/SubscribeCallBack中接收微信回调的参数。
        /// 主要参数有关注者的OpenID
        /// </summary>
        /// <param name="request">可以通过该参数保存你的业务ID，业务链接，业务场景</param>
        /// <param name="expireDays"></param>
        /// <returns></returns>
        public static async Task<string> GenerateSenceQRCode(
            IWeChatAppClient weChatAppClient,
            IEasyRedisClient easyRedisClient,
            IWeChatQRCodeService weChatQRCodeService,
            SubscribeCallBackQueryRequest request, double expireDays = 1)
        {
            string subscribeHandleUrl = $"{ConfigHelper.GetHost()}/Common/WeChat/SubscribeCallBack?{request.ToQueryString()}";
            string paramMd5 =$"wxqrcode_{DesTool.Md5(subscribeHandleUrl)}";
            string code = await easyRedisClient.GetStringAsync(paramMd5);
            if (string.IsNullOrEmpty(code))
            {
                string sceneKey = Guid.NewGuid().ToString("N");
                int expire_second = (int)TimeSpan.FromDays(expireDays).TotalSeconds;
                var accessToken = await weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" });
                             _ = easyRedisClient.AddStringAsync(sceneKey, subscribeHandleUrl, TimeSpan.FromSeconds(expire_second));
                var Scene = new CreateTempQRStrSceneCodeRequest(expire_second);
                Scene.SetScene(sceneKey);
                var qrcodeResponse = await weChatQRCodeService.GenerateTempQRCode(accessToken.token, Scene);
                if (!string.IsNullOrEmpty(qrcodeResponse.ticket))
                {
                     await easyRedisClient.AddStringAsync(paramMd5, qrcodeResponse?.ImgUrl, TimeSpan.FromDays(expireDays));

                }
                return qrcodeResponse?.ImgUrl;
            }
            else
            {
                return code;
            }

        }



    }

}
