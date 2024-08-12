using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.ViewModels.Article
{
    /// <summary>
    /// 文章列表展示项的ViewModel
    /// </summary>
    public class ArticleListItemViewModel:ArticleViewModel
    {
        /// <summary>
        /// 点击跳转的链接
        /// </summary>
        public string ActionUrl { get; set; }
        /// <summary>
        /// 背景图片
        /// </summary>
        public List<string> Covers { get; set; }


        /// <summary>
        /// 列表布局方式
        /// </summary>
        public int Layout { get; set; }


        /// <summary>
        /// 文章摘要
        /// </summary>
        public string Digest { get; set; }

        public string No { get; set; }


    }
}
