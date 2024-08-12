using Sxb.PCWeb.ViewModels.Article;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PCWeb.ViewModels.Common
{
    public class ReportModel
    {
        /// <summary>
        ///举报类型【1：点评、问答，2：文章】
        /// </summary>
        public int ReportType { get; set; }
        /// <summary>
        /// 文章举报
        /// </summary>
        public ArticleListItemViewModel ArticleListItemView { get; set; }
    }
}
