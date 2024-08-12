using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.Entities.Total
{
    public class SupplierTotal
    {
        /// <summary>
        /// 兼职总人数
        /// </summary>
        public int PartTimeJobAdminTotal { get; set; }
        /// <summary>
        /// 总提交点评数
        /// </summary>
        public int CommitCommentTotal { get; set; }
        /// <summary>
        /// 总提交回答数
        /// </summary>
        public int CommitAnswerTotal { get; set; }
        /// <summary>
        /// 已审核点评数
        /// </summary>
        public int ExaminerCommentTotal { get; set; }
        /// <summary>
        /// 已审核问答
        /// </summary>
        public int ExaminerAnswerTotal { get; set; }
        /// <summary>
        /// 已审核加精
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
