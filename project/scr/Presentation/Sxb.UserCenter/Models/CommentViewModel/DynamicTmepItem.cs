using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.UserCenter.Models.CommentViewModel
{
    public class DynamicTmepItem
    {
        public Guid Eid { get; set; }
        /// <summary>
        /// 数据id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        public int LikeCount { get; set; }
        /// <summary>
        /// 回复数
        /// </summary>
        public int ReplyCount { get; set; }
        /// <summary>
        /// 阅读数
        /// </summary>
        public int ViewCount { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public string AddTime { get; set; }
        /// <summary>
        /// 学校名
        /// </summary>
        public string SchoolName { get; set; }
        /// <summary>
        /// 封面图
        /// </summary>
        public string FrontCover { get; set; }
        /// <summary>
        /// 是否点赞
        /// </summary>
        public bool IsLike { get; set; }

        public string No { get; set; }
    }

    /// <summary>
    /// 正在直播
    /// </summary>
    public class Live 
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int ViewCount { get; set; }
    }

}
