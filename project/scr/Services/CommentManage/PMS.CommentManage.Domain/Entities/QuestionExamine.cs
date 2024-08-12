using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PMS.CommentsManage.Domain.Entities
{
    /// <summary>
    /// 问题审核
    /// </summary>
    [Table("QuestionExamines")]
    public class QuestionExamine
    {
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// 审核者
        /// </summary>
        public Guid AdminId { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime ExamineTime { get; set; }

        /// <summary>
        /// 问题
        /// </summary>
        [ForeignKey("QuestionInfos")]
        public Guid QuestionInfoId { get; set; }

        public virtual QuestionInfo QuestionInfo { get; set; }
    }
}
