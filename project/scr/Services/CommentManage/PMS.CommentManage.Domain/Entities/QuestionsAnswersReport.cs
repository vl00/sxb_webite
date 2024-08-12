using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PMS.CommentsManage.Domain.Common;

namespace PMS.CommentsManage.Domain.Entities
{
    /// <summary>
    /// 问答举报
    /// </summary>
    [Table("QuestionsAnswersReports")]
    public class QuestionsAnswersReport
    {
        public QuestionsAnswersReport()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }
        
        /// <summary>
        /// 举报原因
        /// </summary>
        /// <value>The type of the report reason.</value>
        [Required]
        public int ReportReasonType { get; set; }
        
        /// <summary>
        /// 举报人ID
        /// </summary>
        /// <value>The report user identifier.</value>
        public Guid ReportUserId { get; set; }

        /// <summary>
        /// 问题ID
        /// </summary>
        public Guid QuestionId { get; set; }

        /// <summary>
        /// 回答ID
        /// </summary>
        public Guid? QuestionsAnswersInfoId { get; set; }

        /// <summary>
        /// 回复ID
        /// </summary>
        public Guid? AnswersReplyId { get; set; }

        /// <summary>
        /// 举报内容
        /// </summary>
        [Required]
        public string ReportDetail { get; set; }

        /// <summary>
        /// 举报类型 
        /// </summary>
        public ReportDataType ReportDataType { get; set; }

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
        
        [ForeignKey("QuestionId")]
        public virtual QuestionInfo QuestionInfos { get; set; }

        //[ForeignKey("QuestionsAnswersInfoId")]
        //public virtual QuestionsAnswersInfo QuestionsAnswersInfo { get; set; }

        [ForeignKey("ReportReasonType")]
        public virtual ReportType ReportType { get; set; }


        public virtual QuestionsAnswersReportReply QuestionsAnswersReportReply { get; set; }
    }
}
