using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ProductManagement.API.Http.Common
{
    public class HttpEncodingHelper
    {
        /// <summary>
        /// 获取流中的html
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static string GetEncodingHtml(Stream stream, string charset = "")
        {
            var utf8 = Encoding.UTF8;
            var destinationEncoding = Encoding.UTF8;

            // Response Content-Type 中无charset
            if (string.IsNullOrWhiteSpace(charset))
            {
                //从html meta Content-Type中读取charset
                //utf8编码的html内容
                var dynamicHtml = GetStreamString(stream, utf8);
                //读取charset
                charset = GetHtmlContentTypeMeta(dynamicHtml);
            }

            // charset转换为编码, 如果无, 则默认为utf8
            Encoding sourceEncoding = ConvertToEncoding(charset, def: utf8);

            //实际编码的html内容
            stream.Position = 0;
            var sourceHtml = GetStreamString(stream, sourceEncoding);

            //转换为目标编码
            var html = EncodingTo(sourceEncoding, destinationEncoding, sourceHtml);
            return html;
        }

        public static string GetStreamString(Stream stream, Encoding encoding)
        {
            var reader = new StreamReader(stream, encoding);
            return reader.ReadToEndAsync().Result;
        }

        /// <summary>
        /// 转换编码
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="sourceStr"></param>
        /// <returns></returns>
        public static string EncodingTo(Encoding source, Encoding destination, string sourceStr)
        {
            if (source == destination)
                return sourceStr;

            var sourceBytes = source.GetBytes(sourceStr);
            var destinationBytes = Encoding.Convert(source, destination, sourceBytes);
            return destination.GetString(destinationBytes);
        }

        /// <summary>
        /// 获取html meta中的编码
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string GetHtmlContentTypeMeta(string html)
        {
            //var match = Regex.Match(html, "<meta http-equiv=\"content-type\"(.*)/>");
            var match = Regex.Match(html, "<meta(?:[^>]*)charset=([^>]*)(?:[^>]*)(?:/?)>", RegexOptions.IgnoreCase);

            var encodingStr = string.Empty;
            if (match.Groups.Count == 2)
                encodingStr = match.Groups[1].Value;

            return encodingStr;
        }

        /// <summary>
        /// string to Encoding
        /// </summary>
        /// <param name="encodingStr">含有编码的字符串</param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static Encoding ConvertToEncoding(string encodingStr, Encoding def = null)
        {
            if (string.IsNullOrWhiteSpace(encodingStr))
                return def;

            var contains = encodingStr.ToLower();
            if (contains.Contains("gb2312") || contains.Contains("gbk"))
            {
                return Encoding.GetEncoding("gb2312");
            }
            else if (contains.Contains("xxx"))
            {

            }

            return def;
        }
    }
}
