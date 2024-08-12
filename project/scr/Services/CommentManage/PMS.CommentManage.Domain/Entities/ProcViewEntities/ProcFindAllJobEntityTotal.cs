using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.Entities.ProcViewEntities
{
    /// <summary>
    /// 兼职数据概况 条数，存储过程实体：proc_FindAllJob @LeaderId（兼职领队id）
    /// </summary>
    public class ProcFindAllJobEntityTotal
    {
        /// <summary>
        /// 该兼职领队下的兼职人员总数
        /// </summary>
        public int JobTotal { get; set; }
        /// <summary>
        /// 回答总条数
        /// </summary>
        public int TotalAnswer { get; set; }
        /// <summary>
        /// 回答精选总数
        /// </summary>
        public int TotalAnswerSelected { get; set; }
        /// <summary>
        /// 点评总条数
        /// </summary>
        public int TotalSchoolComment { get; set; }
        /// <summary>
        /// 点评精选总数
        /// </summary>
        public int TotalSchoolCommentsSelected { get; set; }
    }
}
