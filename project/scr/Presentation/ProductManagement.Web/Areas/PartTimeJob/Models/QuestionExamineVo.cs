using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Web.Areas.PartTimeJob.Models
{
    public class QuestionExamineVo
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 问题状态（1：已提交，2：驳回，3：通过，4：精选）
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// 审核者
        /// </summary>
        public Guid PartTimeJobAdminId { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime ExamineTime { get; set; }

        public QuestionInfoVo QuestionInfoDto { get; set; }
        public List<QuestionsAnswerExamineVo> QuestionsAnswerExamineVo { get; set; }
        public PartTimeJobAdminVo PartTimeJobAdminVo { get; set; }
    }
}
