using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Web.Areas.PartTimeJob.Models
{
    /// <summary>
    /// 审核人员审核信息
    /// </summary>
    public class AllExaminerAdminInfoVo
    {
        /// <summary>
        /// 审核人员id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 审核人员名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 审核人员手机号
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 结算方式
        /// </summary>
        public string SettlementType { get; set; }
        /// <summary>
        /// 总点评数
        /// </summary>
        public int ExaminerCommentTotal { get; set; }
        /// <summary>
        /// 总问答数
        /// </summary>
        public int ExaminerAnswerTotal { get; set; }
    }
}
