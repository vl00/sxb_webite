using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using PMS.CommentsManage.Domain.Common;
using PMS.UserManage.Domain.Common;

namespace PMS.CommentsManage.Domain.Entities
{
    /// <summary>
    /// 问题详情
    /// </summary>
    [Table("QuestionInfos")]
    public class QuestionInfo
    {
        public QuestionInfo()
        {
            Id = Guid.NewGuid();
            State = ExamineStatus.Unread;
            IsTop = false;
            LikeCount = 0;
            ReplyCount = 0;
            IsHaveImagers = false;
            IsAnony = false;
        }

        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// 问题审核状态（0：未阅，1：已阅，2：已加精，4：已屏蔽） 默认值为：0（可以在前端显示）
        /// </summary>
        [Required]
        public ExamineStatus State { get; set; }
        /// <summary>
        /// 学校Id
        /// </summary>
        public Guid SchoolId { get; set; }

        /// <summary>
        /// 学校学部ID
        /// </summary>
        public Guid SchoolSectionId { get; set; }
        /// <summary>
        /// 问题写入者
        /// </summary>
        public Guid UserId { get; set; }

        public UserRole PostUserRole { get; set; }

        /// <summary>
        /// 问题内容
        /// </summary>
        [Required]
        public string Content { get; set; }
        //点赞数
        public int LikeCount { get; set; }
        public int ReplyCount { get; set; }
        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }
        /// <summary>
        /// 是否有上传图片
        /// </summary>
        public bool IsHaveImagers { get; set; }
        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool IsTop { get; set; }

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
        /// 创建日期
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateTime { get; set; }
        public virtual QuestionExamine QuestionExamine { get; set; }

        public virtual List<QuestionsAnswersInfo> QuestionsAnswersInfos { get; set; }
        public virtual List<QuestionsAnswersReport> QuestionsAnswersReports { get; set; }
    }
}
