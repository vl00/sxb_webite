using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PMS.CommentsManage.Domain.Entities
{
    /// <summary>
    /// 答题审核
    /// </summary>
    [Table("QuestionsAnswerExamines")]
    public class QuestionsAnswerExamine
    {
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// 问题id
        /// </summary>
        [ForeignKey("QuestionsAnswersInfos")]
        public Guid QuestionsAnswersInfoId { get; set; }
        /// <summary>
        /// 审核者
        /// </summary>
        public Guid AdminId { get; set; }
        public bool IsPartTimeJob { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? ExamineTime { get; set; }

        public virtual QuestionsAnswersInfo QuestionsAnswersInfo { get; set; }

    }
}
