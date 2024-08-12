using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.Http.Result.Org
{
    /// <summary>
    /// 种草列表item
    /// </summary>
    public class EvaluationDto
    {
        /// <summary>评测id</summary>
        public Guid Id { get; set; }
        /// <summary>评测短id</summary>
        public string Id_s { get; set; }
        /// <summary>标题</summary>
        public string Title { get; set; }
        /// <summary>是否精华</summary>
        public bool Stick { get; set; }
        /// <summary>创建时间</summary>
        public DateTime CreateTime { get; set; } = DateTime.Parse("1986-06-01");

        ///// <summary>是否是纯文字(没图)</summary>
        //public bool IsPlaintext { get; set; }
        /// <summary>内容</summary>
        public string Content { get; set; }

        /// <summary>评测图(原图)</summary>
        public IEnumerable<string> Imgs { get; set; }
        /// <summary>评测图(缩略图)</summary>
        public IEnumerable<string> Imgs_s { get; set; }
        /// <summary>视频地址</summary>
        public string VideoUrl { get; set; }
        /// <summary>视频封面图</summary>
        public string VideoCoverUrl { get; set; }

        /// <summary>作者id</summary>
        public Guid AuthorId { get; set; }
        /// <summary>作者名</summary>
        public string AuthorName { get; set; }
        /// <summary>作者头像</summary>
        public string AuthorHeadImg { get; set; }

        /// <summary>分享数</summary>
        public int SharedCount { get; set; }
        /// <summary>点赞数</summary>
        public int LikeCount { get; set; }
        /// <summary>是否是我曾经点赞过的</summary>
        public bool IsLikeByMe { get; set; }

    }
}
