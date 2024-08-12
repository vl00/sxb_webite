using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.Entities
{
    public class CommentAndReply
    {
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
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
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
        /// 点评写入时间
        /// </summary>
        public DateTime CommmentAddTime { get; set; }
        /// <summary>
        /// 回复写入时间
        /// </summary>
        public DateTime ReplyAddTime { get; set; }
        /// <summary>
        /// 点评写入时间、回复写入时间排序
        /// </summary>
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 0：点评，1：回复
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 点评图片数量
        /// </summary>
        public int ImageTotal { get; set; }
    }
}
