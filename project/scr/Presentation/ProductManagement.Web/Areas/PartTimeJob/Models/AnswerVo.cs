using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Web.Areas.PartTimeJob.Models
{
    /// <summary>
    /// 兼职问答列表
    /// </summary>
    public class AnswerVo
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 问答写入者
        /// </summary>
        public string AnswerWrite { get; set; }
        /// <summary>
        /// 问题
        /// </summary>
        public string Question { get; set; }
        /// <summary>
        /// 问答
        /// </summary>
        public string Answer { get; set; }
        /// <summary>
        /// 学校
        /// </summary>
        public string School { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        public string ExamineStatus { get; set; }
        /// <summary>
        /// 审核者
        /// </summary>
        public string ExamineAdmin { get; set; }
        public string AddTime { get; set; }
    }
}
