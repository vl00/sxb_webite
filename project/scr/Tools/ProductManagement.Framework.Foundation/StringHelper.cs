using Microsoft.Extensions.Primitives;
using System;
using System.Text.RegularExpressions;

namespace ProductManagement.Framework.Foundation
{
    public static class StringHelper
    {
        public static long ToLong(this string input, long def = 0L)
        {
            if (string.IsNullOrWhiteSpace(input)) return def;

            if (long.TryParse(input, out long result))
                return result;

            return def;
        }

        public static string ToHeadImgUrl(this string input)
        {
            string def = "https://cos.sxkid.com/images/school_v3/head.png";
            return !string.IsNullOrWhiteSpace(input) ? input : def;
        }

        public static bool IsSxkidDomain(this string url)
        {
            return IsDomain(url, "sxkid.com");
        }

        public static bool IsDomain(this string url, params string[] domains)
        {
            if (domains == null)
            {
                throw new ArgumentNullException();
            }

            try
            {
                var uri = new Uri(url);
                //return uri.Host.Contains(domain);
                foreach (var domain in domains)
                {
                    if (string.IsNullOrWhiteSpace(domain))
                    {
                        throw new ArgumentNullException();
                    }

                    if (uri.Host.ToLower().Contains(domain))
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        /// <summary>
        /// 将字符串根据长度从头截取 , 加上...返回
        /// </summary>
        /// <param name="input">输入的字符串</param>
        /// <param name="length">截取的长度`</param>
        /// <returns></returns>
        public static string GetShortString(this string input, int length)
        {
            var result = string.Empty;
            if (string.IsNullOrWhiteSpace(input) || length < 1) return input;
            if (input.Length > length)
            {
                result = input.Substring(0, length) + "...";
            }
            else
            {
                result = input;
            }
            return result;
        }

        /// <summary>
        /// 截取HTML页面的HEAD相关
        /// <para>过滤 空格，回车，双引号，尖括号</para>
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="length">截取长度</param>
        /// <returns></returns>
        public static string GetHtmlHeaderString(this string input, int length)
        {
            var result = string.Empty;
            if (string.IsNullOrWhiteSpace(input) || length < 1) return input;
            //result = Regex.Replace(input, @"<\/?[p|(span)|b|img].*?\/?>", "", RegexOptions.IgnoreCase);
            result = input.ReplaceHtmlTag();
            result = result.Replace("\r", "").Replace("\n", "").Replace("\"", "").Replace("<", "")
                .Replace(">", "").Replace("“", "").Replace("”", "").Replace("*", "").Replace(" ", "").Replace("【", "").Replace("】", "");
            return result.GetShortString(length);
        }

        /// <summary>
        /// 字符串的首字母大写转换
        /// </summary>
        /// <returns></returns>
        public static string ToTitleCase(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }
            return input.Substring(0, 1).ToUpper() + input.Substring(1);
        }

        /// <summary>
        /// 转换为首字母小写的副本
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToFirstLower(this string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                if (input.Length > 1)
                {
                    return char.ToLower(input[0]) + input.Substring(1);
                }
                return char.ToLower(input[0]).ToString();
            }
            return null;
        }

        /// <summary>
        /// 去除HTML标签
        /// </summary>
        /// <param name="html">带有html标签的文本</param>
        /// <returns></returns>
        public static string ReplaceHtmlTag(this string html)
        {
            var a = System.Web.HttpUtility.UrlDecode(html);
            var b = System.Web.HttpUtility.UrlEncode(html);
            string strText = Regex.Replace(html, "<[^>]+>", "");
            strText = Regex.Replace(strText, "&[^;]+;", "");
            return strText;
        }

        /// <summary>
        /// 按照文章列表定义的日期格式规则生成时间格式化
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string ToArticleFormattString(this DateTime time)
        {
            var passBy_H = (int)(DateTime.Now - time).TotalHours;

            if (passBy_H > 24 || passBy_H < 0)
            {
                //超过24小时,显示日期
                return time.ToString("yyyy年MM月dd日");
            }
            if (passBy_H == 0)
            {
                return time.ToString("刚刚");
            }
            else
            {
                return $"{passBy_H}小时前";
            }
        }

        public static string ToWebUrl(this string url, string domain, string def = "")
        {
            if (string.IsNullOrWhiteSpace(url))
                return def;

            if (url.StartsWith("https://") || url.StartsWith("http://"))
                return url;

            return $"{domain.TrimEnd('/')}/{url.TrimStart('/')}";
        }
    }
}
