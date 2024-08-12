using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Article.Models
{
    public class ArticleCardViewModel
    {
        /// <summary>
        /// 文章标题
        /// </summary>
        public string Title { get; set; }

        ///// <summary>
        ///// 文章摘要
        ///// </summary>
        //public string Summary { get; set; }

        /// <summary>
        /// 文章短链接
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// 封面图 默认值 https://cos.sxkid.com/v4source/pc/imgs/home/sxb.png
        /// </summary>
        public string Cover { get; set; }

        /// <summary>
        /// 文章作者
        /// </summary>
        public string Author { get; set; }
    }
}
