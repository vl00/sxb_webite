using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using PMS.CommentsManage.Domain.Common;
using PMS.UserManage.Domain.Common;

namespace PMS.CommentsManage.Domain.Entities
{
    /// <summary>
    /// 点评详情
    /// </summary>
    [Table("SchoolComments")]
    public class SchoolComment
    {
        public SchoolComment()
        {
            SchoolCommentTags = new List<SchoolTag>();
            SchoolCommentReplys = new List<SchoolCommentReply>();
            Id = Guid.NewGuid();
            LikeCount = 0;
            IsTop = false;
            ReplyCount = 0;
            State= ExamineStatus.Unread;
            IsSettlement = false;
        }

        public Guid Id { get; set; }
        /// <summary>
        /// 学校ID
        /// </summary>
        public Guid SchoolId { get; set; }
        /// <summary>
        /// 学校学部ID
        /// </summary>
        public Guid SchoolSectionId { get; set; }
        /// <summary>
        /// 评论者ID
        /// </summary>
        public Guid CommentUserId { get; set; }
        /// <summary>
        /// 点评内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 点评审核状态（0：未阅，1：已阅，2：已加精，4：已屏蔽） 默认值为：0（可以在前端显示）
        /// </summary>
        public ExamineStatus State { get; set; }

        public UserRole PostUserRole { get; set; }

        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool IsTop { get; set; }
        /// <summary>
        /// 回复数
        /// </summary>
        public int ReplyCount { get; set; }
        //点赞数
        public int LikeCount { get; set; }
        /// <summary>
        /// 是否已结算
        /// </summary>
        public bool IsSettlement { get; set; }
        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }
        /// <summary>
        /// 是否辟谣
        /// </summary>
        public bool RumorRefuting { get; set; }
        /// <summary>
        /// 是否有上传图片
        /// </summary>
        public bool IsHaveImagers { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long No { get; set; }

        /// <summary>
        /// 浏览量
        /// </summary>
        public int ViewCount { get; set; }

        /// <summary>
        /// 写入日期
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime AddTime { get; set; }

        public virtual SchoolCommentScore SchoolCommentScore { get; set; }

        public virtual SchoolCommentExamine SchoolCommentExamine { get; set; }

        public virtual List<SchoolTag> SchoolCommentTags { get; set; }
        
        public virtual List<SchoolCommentReply> SchoolCommentReplys { get; set; }
        public virtual List<SchoolCommentReport> SchoolCommentReports { get; set; }
    }
}
