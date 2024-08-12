using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.Entities.Total
{
    public class ExaminerTotal
    {
        /// <summary>
        /// 待审核点评数
        /// </summary>
        public int WaitSchoolCommentTotal { get; set; }
        /// <summary>
        /// 待审核问答数
        /// </summary>
        public int WaitSchoolAnswerTotal { get; set; }
        /// <summary>
        /// 已审核点评总数
        /// </summary>
        public int SuccessCommentTotal { get; set; }
        /// <summary>
        /// 已审核问答总数
        /// </summary>
        public int SuccessSchoolAnswerTotal { get; set; }
        /// <summary>
        /// 精选点评总数
        /// </summary>
        public int ExaminerSelectedCommentTotal { get; set; }
        /// <summary>
        /// 精选回答总数
        /// </summary>
        public int ExaminerSelectedAnswerTotal { get; set; }
        /// <summary>
        /// 精选通过率
        /// </summary>
        public decimal ExaminerSelectedTotal { get; set; }
    }
}
