using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.ModelDto
{
    public class SysAdminQuerySupplierTotalDto
    {
        /// <summary>
        /// 已审核点评数
        /// </summary>
        public int ExaminerCommentTotal { get; set; }
        /// <summary>
        /// 已审核问答
        /// </summary>
        public int ExaminerAnswerTotal { get; set; }
        /// <summary>
        /// 已审核加精点评
        /// </summary>
        public int SelectedCommentTotal { get; set; }
        /// <summary>
        /// 已审核加精回答
        /// </summary>
        public int SelectedAnswerTotal { get; set; }
        /// <summary>
        /// 屏蔽用户数
        /// </summary>
        public int ShieldJobTotal { get; set; }
        /// <summary>
        /// 屏蔽用户提交点评
        /// </summary>
        public int ShieldJobCommentTotal { get; set; }
        /// <summary>
        /// 屏蔽用户提交回答
        /// </summary>
        public int ShieldJobAnswerTotal { get; set; }
        /// <summary>
        /// 屏蔽失效点评
        /// </summary>
        public int ShieldJobSelectedCommentTotal { get; set; }
        /// <summary>
        /// 屏蔽问答数据
        /// </summary>
        public int ShieldJobSelectedAnswerTotal { get; set; }
    }
}
