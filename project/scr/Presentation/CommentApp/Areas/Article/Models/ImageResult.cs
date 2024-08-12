using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Article.Models
{
    public class ImageResult
    {
        /// <summary>
        /// 原图
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 缩略图
        /// </summary>
        public string Thunmbnail { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

    }
}
