using PMS.CommentsManage.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.Entities.Total
{
    /// <summary>
    /// 学校点评统计
    /// </summary>
    public class SchoolCommentTotal
    {
        /// <summary>
        /// 统计类型
        /// </summary>
        public QueryCondition TotalType { get; set; }
        /// <summary>
        /// 统计总数
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// 分部id
        /// </summary>
        public Guid SchoolSectionId { get; set; }
        /// <summary>
        /// 学校名称
        /// </summary>
        public string Name { get; set; }
    }
}
