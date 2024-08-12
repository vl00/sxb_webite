using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;

namespace ProductManagement.Framework.Foundation
{
    [Flags]
    public enum ClientType
    {
        UnKown = 0,
        /// <summary>
        /// 移动端
        /// </summary>
        [DefaultValue("H5")]
        Mobile = 1,
        /// <summary>
        /// PC端
        /// </summary>
        [DefaultValue("PC")]
        PC = 2,
        /// <summary>
        /// 来自小程序
        /// </summary>
        WxMinApp = 4,
        /// <summary>
        /// 来自APP
        /// </summary>
        App = 8,
        /// <summary>
        /// 来自微信浏览器
        /// </summary>
        WX = 16,
    }

    public static class UserAgentUtils
    {
        public static ClientType GetClientType(this IHeaderDictionary header)
        {
            if (header.ContainsKey("User-Agent") == false) {
                return ClientType.UnKown;
            }
            string usagent =   header["User-Agent"].ToString().ToLower();
            Regex mobileRegex = new Regex("Mobile.*", RegexOptions.IgnoreCase);
            if (mobileRegex.IsMatch(usagent))
            {
                if (header.ContainsKey("ua"))
                {
                    //ua是APP端特写的header。
                    string ua = header["ua"].ToString().ToLower();
                    if (usagent.Contains("ischool"))
                    {
                        return ClientType.App | ClientType.Mobile;
                    }
                }
                if (usagent.Contains("miniprogram"))
                { 
                    return ClientType.WxMinApp;
                }
                if (usagent.Contains("micromessenger"))
                {
                    return ClientType.WX | ClientType.Mobile;
                }

               return ClientType.Mobile;
            }
            else {
                if (usagent.Contains("micromessenger"))
                {
                    return ClientType.WX | ClientType.PC;
                }
                return ClientType.PC;
            }
   
        }


        public static int GetPlatformMode(IDictionary<string, string> headers)
        {
            //private int platform;
            // 0x111 第一位1表示pc 2表示移动端,第二位 1表示android 2表示ios, 0表示其他 第三位 1表示app 2表示微信 3表示小程序 0表示浏览器
            bool isMobile = false;
            String ua = headers.ContainsKey("ua") ? headers["ua"] : null;
            String usagent = headers.ContainsKey("User-Agent") ? headers["User-Agent"] : null;
            if (ua != null)
                isMobile = true;
            else if (IsMobileByUserAgent(usagent))
            {
                isMobile = true;
            }
            if (!isMobile)
            {
                // System.out.println("pc端");
                return 0x100;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("移动端");
            int mode = 0x200;
            if (ua != null)
            {
                if (ua.ToLower().IndexOf("ios") != -1)
                {
                    mode |= 0x020;
                    sb.Append("iphone");
                }
                else if (ua.ToLower().IndexOf("android") != -1)
                {
                    mode |= 0x010;
                    sb.Append("android");
                }
            }
            else if (usagent != null)
            {
                if (usagent.ToLower().IndexOf("iphone") != -1)
                {
                    mode |= 0x020;
                    sb.Append("iphone");
                }
                else if (usagent.ToLower().IndexOf("android") != -1 || usagent.ToLower().IndexOf("linux") != -1)
                {
                    mode |= 0x010;
                    sb.Append("android");
                }
            }
            if (ua != null)
            {
                if (ua.ToLower().IndexOf("miniprogram") != -1)
                {
                    mode |= 0x003;
                    sb.Append("小程序");
                }
                else
                {
                    mode |= 0x001;
                    sb.Append("app");
                }
            }
            else if (usagent != null && usagent.ToLower().IndexOf("micromessenger") != -1)
            {
                mode |= 0x002;
                sb.Append("微信");
            }
            else if (usagent != null)
            {
                if (usagent.ToLower().IndexOf("micromessenger") != -1)
                {
                    mode |= 0x002;
                    sb.Append("微信");
                }
                else if (usagent.ToLower().IndexOf("miniprogram") != -1)
                {
                    mode |= 0x003;
                    sb.Append("小程序");
                }
                else if (usagent.ToLower().IndexOf("ischool") != -1)
                {
                    mode |= 0x001;
                    sb.Append("app");
                }
                else
                {
                    sb.Append("浏览器");
                }
            }
            else
            {
                sb.Append("浏览器");
            }
            // System.out.println(sb.toString());
            return mode;
        }


        public static bool IsMobileByUserAgent(String useragent)
        {
            bool isMoblie = false;
            String[] mobileAgents = { "iphone", "android", "phone", "mobile", "wap", "netfront", "java", "opera mobi",
    "opera mini", "ucweb", "windows ce", "symbian", "series", "webos", "sony", "blackberry", "dopod",
    "nokia", "samsung", "palmsource", "xda", "pieplus", "meizu", "midp", "cldc", "motorola", "foma",
    "docomo", "up.browser", "up.link", "blazer", "helio", "hosin", "huawei", "novarra", "coolpad", "webos",
    "techfaith", "palmsource", "alcatel", "amoi", "ktouch", "nexian", "ericsson", "philips", "sagem",
    "wellcom", "bunjalloo", "maui", "smartphone", "iemobile", "spice", "bird", "zte-", "longcos", "pantech",
    "gionee", "portalmmm", "jig browser", "hiptop", "benq", "haier", "^lct", "320x320", "240x320",
    "176x220", "w3c ", "acs-", "alav", "alca", "amoi", "audi", "avan", "benq", "bird", "blac", "blaz",
    "brew", "cell", "cldc", "cmd-", "dang", "doco", "eric", "hipt", "inno", "ipaq", "java", "jigs", "kddi",
    "keji", "leno", "lg-c", "lg-d", "lg-g", "lge-", "maui", "maxo", "midp", "mits", "mmef", "mobi", "mot-",
    "moto", "mwbp", "nec-", "newt", "noki", "oper", "palm", "pana", "pant", "phil", "play", "port", "prox",
    "qwap", "sage", "sams", "sany", "sch-", "sec-", "send", "seri", "sgh-", "shar", "sie-", "siem", "smal",
    "smar", "sony", "sph-", "symb", "t-mo", "teli", "tim-", /* "tosh", */ "tsm-", "upg1", "upsi", "vk-v",
    "voda", "wap-", "wapa", "wapi", "wapp", "wapr", "webc", "winw", "winw", "xda", "xda-","Googlebot-Mobile" };
            if (useragent != null)
            {
                foreach (String mobileAgent in mobileAgents)
                {
                    if (useragent.ToLower().IndexOf(mobileAgent, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        isMoblie = true;
                        break;
                    }
                }
            }
            if (isMoblie)
            {
                return true;
            }
            return false;
        }
    }
}
