using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.Dtos
{
    public class ArticleGetChoicenessResultDto
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
        /// 阅读量
        /// </summary>
        public int ViweCount { get; set; }

        /// <summary>
        /// 评论数量
        /// </summary>
        public int CommentCount { get; set; }

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
