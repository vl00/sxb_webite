using System;
namespace PMS.CommentsManage.Domain.Entities.ViewEntities
{
    public class SchoolCommentReplyExt
    {
        public Guid Id { get; set; }

        public Guid SchoolCommentId { get; set; }

        public Guid UserId { get; set; }

        public Guid? ReplyId { get; set; }

        public Guid? ReplyUserId { get; set; }

        public bool ReplyUserIdIsAnony { get; set; }

        /// <summary>
        /// 是否学校发布
        /// </summary>
        public bool IsSchoolPublish { get; set; }
        /// <summary>
        /// 是否就读
        /// </summary>
        public bool IsAttend { get; set; }
        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }
        /// <summary>
        /// 是否辟谣
        /// </summary>
        public bool RumorRefuting { get; set; }
        /// <summary>
        /// 回复内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool IsTop { get; set; }
        public int ReplyCount { get; set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        public int LikeCount { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
