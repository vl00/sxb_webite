using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Web.Areas.PartTimeJob.Models
{
    /// <summary>
    /// 问答审核详情
    /// </summary>
    public class ExaminerAnswerVo
    {
        /// <summary>
        /// 问答Id
        /// </summary>
        public Guid id { get; set; }
        /// <summary>
        /// 问题
        /// </summary>
        public string Question { get; set; }
        /// <summary>
        /// 回答内容
        /// </summary>
        public string AnswerCotent { get; set; }
        /// <summary>
        /// 学校名称
        /// </summary>
        public string School { get; set; }
        /// <summary>
        /// 写入答联系方式
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 问答状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 问答写入者昵称
        /// </summary>
        public string AnswerWriteName { get; set; }
        /// <summary>
        /// 审核者
        /// </summary>
        public string ExaminerAdminNaem { get; set; }
        /// <summary>
        /// 审核日期
        /// </summary>
        public string ExaminerTime { get; set; }
    }
}
