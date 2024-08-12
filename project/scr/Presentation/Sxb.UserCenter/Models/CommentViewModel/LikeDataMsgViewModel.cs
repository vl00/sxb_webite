using Sxb.UserCenter.Models.UserInfoViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.UserCenter.Models.CommentViewModel
{
    public class DataMsgViewModel
    {
        public int Type { get; set; }
        public string Time { get; set; }
        public string SName { get; set; }
        public int LikeCount { get; set; }
        public Comment Comment { get; set; }
        public CommentReply CommentReply { get; set; }
        public AnswerReply AnswerReply { get; set; }
        /// <summary>
        /// 点赞用户信息
        /// </summary>
        public UserViewModel User { get; set; }
    }

    /// <summary>
    /// 点评
    /// </summary>
    public class Comment
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        /// <summary>
        /// 写点评者
        /// </summary>
        public UserViewModel CommentUser { get; set; }
    }

    /// <summary>
    /// 点评回复
    /// </summary>
    public class CommentReply 
    {
        public Guid CommentId { get; set; }
        public Guid ReplyId { get; set; }
        public string CommentContent { get; set; }
        public string ReplyConent { get; set; }
        public int ReplyCount { get; set; }
        public Guid ReplyParentId { get; set; }
        public string ReplyParentContent { get; set; }
        public int ReplyType { get; set; }
        /// <summary>
        /// 点评
        /// </summary>
        public UserViewModel CommentUser { get; set; }
        /// <summary>
        /// 回复
        /// </summary>
        public UserViewModel ReplyUser { get; set; }
        /// <summary>
        /// 回复回复
        /// </summary>
        public UserViewModel ReplyParentUser { get; set; }
    }   

    public class AnswerReply 
    {
        public Guid QuestionId { get; set; }
        public Guid AnswerId { get; set; }
        public Guid AnswerReplyId { get; set; }
        public string QuestionContent { get; set; }
        public string AnswerContent { get; set; }
        public string ReplyContent { get; set; }
        public int ReplyCount { get; set; }
        public int AnswerType { get; set; }
        /// <summary>
        /// 提问者
        /// </summary>
        public UserViewModel QuestionUser { get; set; }
        /// <summary>
        /// 回答者
        /// </summary>
        public UserViewModel AnswerUser { get; set; }
        /// <summary>
        /// 回答回复者
        /// </summary>
        public UserViewModel AnswerReplyUser { get; set; }
    }
}
