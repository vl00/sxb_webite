using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Web.Areas.PartTimeJob.Models
{
    /// <summary>
    /// 兼职点评列表
    /// </summary>
    public class CommentVo
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 点评内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 学校名称
        /// </summary>
        public string SchoolName { get; set; }
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
