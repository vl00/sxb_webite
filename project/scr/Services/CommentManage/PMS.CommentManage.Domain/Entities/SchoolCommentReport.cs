using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PMS.CommentsManage.Domain.Common;

namespace PMS.CommentsManage.Domain.Entities
{
    /// <summary>
    /// 评论举报
    /// </summary>
    [Table("SchoolCommentReports")]
    public class SchoolCommentReport
    {
        public SchoolCommentReport()
        {
            //Id = Guid.NewGuid();
            Status = 0;
        }

        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 举报原因
        /// </summary>
        [Required]
        public int ReportReasonType { get; set; }

        /// <summary>
        /// 举报人ID
        /// </summary>
        public Guid ReportUserId { get; set; }

        /// <summary>
        /// 被举报的点评ID
        /// </summary>
        public Guid CommentId { get; set; }

        /// <summary>
        /// 被举报的点评回复ID
        /// </summary>
        public Guid? ReplayId { get; set; }

        /// <summary>
        /// 举报类型
        /// </summary>
        public ReportDataType ReportDataType { get; set; }

        /// <summary>
        /// 举报内容
        /// </summary>
        [Required]
        public string ReportDetail { get; set; }

        /// <summary>
        /// 举报内容 0、未回复 1、已回复
        /// </summary>
        [Required]
        public ReportStatus Status { get; set; }
        
        /// <summary>
        /// 举报时间
        /// </summary>
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime ReportTime { get; set; }

        [ForeignKey("CommentId")]
        public virtual SchoolComment SchoolComments { get; set; }

        [ForeignKey("ReportReasonType")]
        public virtual ReportType ReportType { get; set; }

        public virtual SchoolCommentReportReply SchoolCommentReportReply { get; set; }
    }

}
