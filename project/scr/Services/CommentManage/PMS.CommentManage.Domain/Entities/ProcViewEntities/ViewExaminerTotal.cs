using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.Entities.ProcViewEntities
{
    /// <summary>
    /// 视图：ViewExaminerTotal，获取审核人数、总审核点评、总审核问答
    /// </summary>
    public class ViewExaminerTotal
    {
        /// <summary>
        /// 总审核人员数
        /// </summary>
        public int TotalExaminer { get; set; }
        /// <summary>
        /// 总审核点评数
        /// </summary>
        public int ExaminerCommentTotal { get; set; }
        /// <summary>
        /// 总审核问答数
        /// </summary>
        public int ExaminerAnswerTotal { get; set; }
    }
}
