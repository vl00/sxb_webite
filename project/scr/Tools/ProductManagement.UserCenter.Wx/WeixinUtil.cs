using ProductManagement.UserCenter.Wx.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ProductManagement.Framework.Foundation;

namespace ProductManagement.UserCenter.Wx
{
    public class WeixinUtil
    {
        protected HttpClient client;

        public WeixinUtil(
            HttpClient httpClient
            )
        {
            this.client = httpClient;
        }

        public string GetLoginURL(string url, string appid = null, string scope = "snsapi_base", string state = null)
        {
            return
                "https://open.weixin.qq.com/connect/oauth2/authorize?" +
                "appid=" + (appid ?? "wxeefc53a3617746e2") +
                "&redirect_uri="
                + Uri.EscapeDataString("https://weixin.sxkid.com/LoginAuth.aspx?redirect_uri="
                + Uri.EscapeDataString(url))
                + "&response_type=code&scope=" + scope + "&state=" + state + "#wechat_redirect";
        }
        public string GetWebLoginUrl(string url, string appid = null, string state = null)
        {
            return @"https://open.weixin.qq.com/connect/qrconnect?appid=" + (appid ?? "wxd3064b3b16a7768f") + "&scope=snsapi_login&redirect_uri="
                + Uri.EscapeDataString("https://weixin.sxkid.com/LoginAuth.aspx?redirect_uri="
                + Uri.EscapeDataString(url))
                + "&state=" + state + "&login_type=jssdk&self_redirect=true&styletype=&sizetype=&bgcolor=&rst=&style=black";
        }
        /// <summary>
        /// 通过code获取access_token
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public  async Task<WXAccessToken> GetWeiXinAccessToken(string appId, string appSecret, string code)
        {
            string url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid=" + appId + "&secret=" + appSecret +
                "&code=" + code + "&grant_type=authorization_code";
            return await client.GetAsAsync<WXAccessToken>(url);
        }
        /// <summary>
        /// 拉取用户信息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public  async Task<WXUserInfoResult> GetWeiXinUserInfo(string accessToken, string openId)
        {
            string url = "https://api.weixin.qq.com/sns/userinfo?access_token=" + accessToken + "&openid=" + openId + "&lang=zh_CN";
            return await client.GetAsAsync<WXUserInfoResult>(url);
        }

        /// <summary>
        /// 拉取用户信息
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public async Task<WXUserInfoResult> GetUserInfo(string accessToken, string openId)
        {
            var url = $"https://api.weixin.qq.com/cgi-bin/user/info?access_token={accessToken}&openid={openId}";

            //string url = "https://api.weixin.qq.com/sns/userinfo?access_token=" + accessToken + "&openid=" + openId + "&lang=zh_CN";
            return await client.GetAsAsync<WXUserInfoResult>(url);
        }

        ///////////////////////小程序部分/////////////////////
        public  async Task<WXMiniProgramAuth> GetWeiXinMiniProgramAuth(string appId, string appSecret, string code)
        {
            string url = "https://api.weixin.qq.com/sns/jscode2session?appid=" + appId + "&secret=" + appSecret + "&js_code=" + code + "&grant_type=authorization_code";
            return await client.GetAsAsync<WXMiniProgramAuth>(url);
        }
        public  string AES_decrypt(string Input, string Iv, string Key)
        {

            System.Security.Cryptography.RijndaelManaged aes = new System.Security.Cryptography.RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = System.Security.Cryptography.CipherMode.CBC;
            aes.Padding = System.Security.Cryptography.PaddingMode.None;
            aes.Key = Convert.FromBase64String(Key);
            aes.IV = Convert.FromBase64String(Iv);
            var decrypt = aes.CreateDecryptor(aes.Key, aes.IV);
            byte[] xBuff = null;
            using (var ms = new System.IO.MemoryStream())
            {
                using (var cs = new System.Security.Cryptography.CryptoStream(ms, decrypt, System.Security.Cryptography.CryptoStreamMode.Write))
                {
                    byte[] xXml = Convert.FromBase64String(Input);
                    byte[] msg = new byte[xXml.Length + 32 - xXml.Length % 32];
                    Array.Copy(xXml, msg, xXml.Length);
                    cs.Write(xXml, 0, xXml.Length);
                }
                xBuff = decode2(ms.ToArray());
            }
            return System.Text.Encoding.UTF8.GetString(xBuff);
        }
        private  byte[] decode2(byte[] decrypted)
        {
            int pad = (int)decrypted[decrypted.Length - 1];
            if (pad < 1 || pad > 32)
            {
                pad = 0;
            }
            byte[] res = new byte[decrypted.Length - pad];
            Array.Copy(decrypted, 0, res, 0, decrypted.Length - pad);
            return res;
        }
    }
}
