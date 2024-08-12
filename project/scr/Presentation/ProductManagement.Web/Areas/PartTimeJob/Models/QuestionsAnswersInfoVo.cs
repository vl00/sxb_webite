using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Web.Areas.PartTimeJob.Models
{
    public class QuestionsAnswersInfoVo
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 问答Id
        /// </summary>
        public Guid QuestionsAnswerId { get; set; }
        /// <summary>
        /// 问答写入者
        /// </summary>
        public Guid PartTimeJobAdminId { get; set; }
        /// <summary>
        /// 问答内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 问答写入日期
        /// </summary>
        public DateTime CreateTime { get; set; }
        public QuestionsAnswerExamineVo QuestionsAnswerExamineVo { get; set; }
        public PartTimeJobAdminVo PartTimeJobAdminVo { get; set; }
    }
}
