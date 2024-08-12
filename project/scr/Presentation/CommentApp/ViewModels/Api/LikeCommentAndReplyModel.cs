using Sxb.Web.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.ViewModels.Api
{
    /// <summary>
    /// 点赞-点评：当前用户点赞的“点评”+当前用户点赞的“点评的回复”+当前用户点赞的“点评的回复”的回复
    /// </summary>
    public class LikeCommentAndReplyModel
    {
        /// <summary>
        /// 发表者用户信息
        /// </summary>
        public UserInfoVo userInfoVo { get; set; }

        /// <summary>
        /// 回复者信息用户信息
        /// </summary>
        public UserInfoVo replyUserInfoVo { get; set; }

        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 点评id
        /// </summary>
        public Guid CommentId { get; set; }
        /// <summary>
        /// 点评回复id
        /// </summary>
        public Guid ParentReplyId { get; set; }
        /// <summary>
        /// 学校id
        /// </summary>
        public Guid SchoolId { get; set; }
        /// <summary>
        /// 学校分部id
        /// </summary>
        public Guid SchoolSectionId { get; set; }
        /// <summary>
        /// 学校名称
        /// </summary>
        public string SchoolName { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 回复内容
        /// </summary>
        public string ReplyContent { get; set; }
        /// <summary>
        /// 点评回复数
        /// </summary>
        public int ReplyCount { get; set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        public int LikeCount { get; set; }
        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }
        /// <summary>
        /// 点评回复的回复id（回复那条的id）
        /// </summary>
        public Guid ReplyId { get; set; }
        /// <summary>
        /// 写入时间
        /// </summary>
        public string AddTime { get; set; }
        /// <summary>
        /// 回复写入时间
        /// </summary>
        public string ReplyAddTime { get; set; }
        /// <summary>
        /// 0：点评，1：回复
        /// </summary>
        public int Type { get; set; }
    }
}
