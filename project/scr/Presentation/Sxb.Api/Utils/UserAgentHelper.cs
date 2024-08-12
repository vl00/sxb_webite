using Microsoft.AspNetCore.Http;

namespace Sxb.Api.Utils
{
    public static class UserAgentHelper
    {
        public static UA Check(HttpRequest Request)
        {
            if (Request.Headers.Keys.Contains("User-Agent"))
            {
                if (Request.Headers["User-Agent"].ToString().ToLower().Contains("ischool"))
                    return UA.Mobile;
                if (Request.Headers["User-Agent"].ToString().ToLower().Contains("micromessenger"))
                    return UA.WeChat;
            }
            return UA.PC;
        }

        public static UA GetClientType(this HttpRequest Request)
        {
            if (Request.Headers.Keys.Contains("User-Agent"))
            {
                if (Request.Headers["User-Agent"].ToString().ToLower().Contains("ischool"))
                    return UA.Mobile;
                if (Request.Headers["User-Agent"].ToString().ToLower().Contains("micromessenger"))
                    return UA.WeChat;
            }
            return UA.PC;
        }
    }

    public enum UA
    {
        /// <summary>
        /// 电脑网页
        /// </summary>
        PC = 1,

        /// <summary>
        /// 手机网页
        /// </summary>
        Mobile = 2,

        /// <summary>
        /// 微信
        /// </summary>
        WeChat = 3
    }
}