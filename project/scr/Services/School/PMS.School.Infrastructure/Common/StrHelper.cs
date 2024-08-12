using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PMS.School.Infrastructure.Common
{
    public class StrHelper
    {
        /// <summary>
        /// 获取Img的路径
        /// </summary>
        /// <param name="htmlText">Html字符串文本</param>
        /// <returns>以数组形式返回图片路径</returns>
        public static string[] GetHtmlImageUrlList(string htmlText)
        {
            if (string.IsNullOrEmpty(htmlText))
            {
                return new string[] { };
            }
            Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);
            //新建一个matches的MatchCollection对象 保存 匹配对象个数(img标签)
            MatchCollection matches = regImg.Matches(htmlText);
            int i = 0;
            string[] sUrlList = new string[matches.Count];
            //遍历所有的img标签对象
            foreach (Match match in matches)
            {
                //获取所有Img的路径src,并保存到数组中
                sUrlList[i++] = match.Groups["imgUrl"].Value;
            }
            return sUrlList;
        }


        /// <summary>
        /// 提取html中的文本
        /// </summary>
        /// <param name="htmlText"></param>
        /// <returns></returns>
        public static string GetHtmlText(string htmlText,bool img=true)
        {
            if (string.IsNullOrWhiteSpace(htmlText)) return htmlText;

            string noStyle = htmlText.Replace("&quot;", "\"").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"").Replace("&nbsp;", "");
            if (img)
            {
                noStyle = Regex.Replace(noStyle, @"<\/?[p|(span)].*?\/?>", "", RegexOptions.IgnoreCase);
            }
            else
            {
                noStyle = Regex.Replace(noStyle, @"<\/?[p|b|(span)|img].*?\/?>", "", RegexOptions.IgnoreCase);
            }
            //noStyle = Regex.Replace(noStyle, @"\s", "", RegexOptions.IgnoreCase);
            return noStyle;
        }
    }
}
