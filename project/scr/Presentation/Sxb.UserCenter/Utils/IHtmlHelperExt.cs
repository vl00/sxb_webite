using Microsoft.AspNetCore.Html;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    public static class IHtmlHelperExt
    {
        public static HtmlString HtmlString(this IHtmlHelper htmlHelper, string html)
        {
            return new HtmlString(html);
        }

        /// <summary>
        /// 文章列表项的时间格式化
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ArticleListItemTimeFormat(this IHtmlHelper htmlHelper, DateTime dateTime)
        {
            var passBy_H = (int)(DateTime.Now - dateTime).TotalHours;

            if (passBy_H > 24 || passBy_H < 0)
            {
                //超过24小时,显示日期
                return dateTime.ToString("yyyy-MM-dd");
            }
            if (passBy_H == 0)
            {
                return dateTime.ToString("刚刚");
            }
            else
            {
                return $"{passBy_H}小时前";
            }
        }
    }
}