using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Web.Areas.PartTimeJob.Models
{
    /// <summary>
    /// 审核点评
    /// </summary>
    public class ExaminerCommentVo
    {
        /// <summary>
        /// 点评Id
        /// </summary>
        public Guid id { get; set; }
        /// <summary>
        /// 点评内容
        /// </summary>
        public string CommentCotent { get; set; }
        /// <summary>
        /// 学校名称
        /// </summary>
        public string School { get; set; }
        /// <summary>
        /// 写入点评者联系方式
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 点评状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 点评写入者昵称
        /// </summary>
        public string CommentWriteAdmin { get; set; }
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
