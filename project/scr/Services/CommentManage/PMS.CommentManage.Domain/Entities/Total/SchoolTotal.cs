using System;
namespace PMS.CommentsManage.Domain.Entities.Total
{
    public class SchoolTotal
    {
        /// <summary>
        /// 统计总数
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// id
        /// </summary>
        public Guid Id { get; set; }
    }

    // <summary>
    /// 评论数据统计
    /// </summary>
    public class SchCommentData
    {
        // <summary>
        /// 点赞总数
        /// </summary>
        public int LikeCount { get; set; } = 0;
        // <summary>
        /// 回复总数
        /// </summary>
        public int ReplyCount { get; set; } = 0;
        // <summary>
        /// 评论浏览数
        /// </summary>
        public int CommentViewCount { get; set; } = 0;
        // <summary>
        /// 评论回复浏览数
        /// </summary>
        public int CommentRepliyViewCount { get; set; } = 0;
    }

    // <summary>
    /// 问答数据统计
    /// </summary>
    public class SchQuestionData
    {
        // <summary>
        /// 问题总数
        /// </summary>
        public int QuestionCount { get; set; } = 0;
        // <summary>
        /// 点赞总数
        /// </summary>
        public int LikeCount { get; set; } = 0;
        // <summary>
        /// 回复总数
        /// </summary>
        public int ReplyCount { get; set; } = 0;
        // <summary>
        /// 问题浏览数
        /// </summary>
        public int QuestionViewCount { get; set; } = 0;
        // <summary>
        /// 回答浏览数
        /// </summary>
        public int AnswerRepliyViewCount { get; set; } = 0;
    }

    public class SchQuestionDataEx : SchQuestionData
    {
        public Guid SID { get; set; }
    }
}
