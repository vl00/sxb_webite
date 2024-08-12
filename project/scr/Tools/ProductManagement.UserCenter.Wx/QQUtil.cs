using ProductManagement.Framework.Foundation;
using ProductManagement.UserCenter.TencentCommon.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.UserCenter.TencentCommon
{
    public class QQUtil
    {
        protected HttpClient client;

        public QQUtil(
            HttpClient httpClient
            )
        {
            this.client = httpClient;
        }

        public string GetWebLoginURL(string url, string appid = null, string state = null)
        {
            return
                "https://graph.qq.com/oauth2.0/authorize?client_id="
                + (appid ?? "101645642") +
                "&redirect_uri="
                + Uri.EscapeDataString("https://qq.sxkid.com/LoginAuth?redirect_uri="
                + Uri.EscapeDataString(url))
                + "&response_type=code&scope=all&state=" + state;
        }
        /// <summary>
        /// 通过code获取access_token
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<string> GetQQAccessToken(string appId, string appSecret, string code, string redirect_uri)
        {
            string url = "https://graph.qq.com/oauth2.0/token?client_id=" + appId + "&client_secret=" + appSecret +
                "&code=" + code + "&grant_type=authorization_code&redirect_uri=" + redirect_uri;
            string ret = await client.GetAsAsync<string>(url);
            return CommonHelper.GetQueryValue(ret, "access_token");
        }
        /// <summary>
        /// 通过access_token获取openid及unionid
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async Task<QQOpenID> GetQQOpenID(string accessToken)
        {
            string url = "https://graph.qq.com/oauth2.0/me?access_token=" + accessToken + "&unionid=1";
            return await client.GetAsAsync<QQOpenID>(url);
        }
        /// <summary>
        /// 拉取用户信息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public async Task<QQUserInfoResult> GetQQUserInfo(string accessToken, string appSecret, string openId)
        {
            string url = "https://graph.qq.com/user/get_user_info?access_token=" + accessToken + "&oauth_consumer_key=" + appSecret + "&openid=" + openId;
            return await client.GetAsAsync<QQUserInfoResult>(url);
        }

    }
}
