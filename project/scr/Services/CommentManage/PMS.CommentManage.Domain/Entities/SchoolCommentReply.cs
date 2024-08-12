using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PMS.UserManage.Domain.Common;

namespace PMS.CommentsManage.Domain.Entities
{
    /// <summary>
    /// 回复
    /// </summary>
    public class SchoolCommentReply
    {
        public SchoolCommentReply()
        {
            Id = Guid.NewGuid();
            CreateTime = DateTime.Now;
        }

        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid SchoolCommentId { get; set; }
        public Guid? ReplyId { get; set; }
        public Guid UserId { get; set; }

        public UserRole PostUserRole { get; set; }
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

        /// <summary>
        /// 浏览量
        /// </summary>
        public int ViewCount { get; set; }

        /// <summary>
        /// 顶级父级id
        /// </summary>
        public Guid? ParentId { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateTime { get; set; }

        [ForeignKey("SchoolCommentId")]
        public virtual SchoolComment SchoolComment { get; set; }

        [ForeignKey("ReplyId")]
        public virtual SchoolCommentReply ParentReplyInfo { get; set; }
    }
}
