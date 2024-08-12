using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.ViewModels.Article
{
    /// <summary>
    /// 文章 ViewModel
    /// </summary>
    public class ArticleViewModel
    {
        /// <summary>
        /// 标识
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 文章标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 发表时间
        /// </summary>
        public string Time { get; set; }
        /// <summary>
        /// 发表时间(中文)
        /// </summary>
        public string CNTime
        {
            get
            {
                if (DateTime.TryParse(Time, out DateTime convertResult)) return convertResult.ToString("yyyy年MM月dd日");
                return Time;
            }
        }

        /// <summary>
        /// 阅读量
        /// </summary>
        public int ViweCount { get; set; }

        /// <summary>
        /// 评论数量
        /// </summary>
        public int CommentCount { get; set; }
    }
}