using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ProductManagement.Framework.Foundation
{
    public static class SignatureHelper
    {
        public static string Sort(Dictionary<string, string> parameters)
        {
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters);

            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (var item in sortedParams)
            {
                if (!string.IsNullOrWhiteSpace(item.Key) && !string.IsNullOrWhiteSpace(item.Value))
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        sb.Append("&");
                    }
                    sb.Append(item.Key).Append("=").Append(item.Value);
                }
            }
            return sb.ToString();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramsStr"></param>
        /// <param name="token"></param>
        /// <param name="appId"></param>
        /// <param name="secret"></param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="nonce">随机串</param>
        /// <returns></returns>
        public static string Signature(string paramsStr, string appId, string secret, string timestamp, string nonce)
        {
            var signStr = paramsStr + appId + secret + timestamp + nonce;
            //var sortStr = string.Concat(signStr.OrderBy(c => c));

            return MD5Helper.GetSimpleMD5(signStr);
        }
    }
}
