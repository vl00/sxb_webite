using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.Entities.ProcViewEntities
{
    /// <summary>
    /// 兼职数据概况,实体数据
    /// </summary>
    public class ProcFindAllJobEntityList
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 兼职联系方式
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 结算类型
        /// </summary>
        public string SettlementType { get; set; }
        /// <summary>
        /// 兼职昵称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 点评总数
        /// </summary>
        public int TotalSchoolComment { get; set; }
        /// <summary>
        /// 精选点评数
        /// </summary>
        public int TotalSchoolCommentsSelected { get; set; }
        /// <summary>
        /// 答题总数
        /// </summary>
        public int TotalAnswer { get; set; }
        /// <summary>
        /// 精选答题数
        /// </summary>
        public int TotalAnswerSelected { get; set; }
        /// <summary>
        /// 是否被拉黑
        /// </summary>
        public bool Shield { get; set; }
    }
}
