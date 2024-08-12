using PMS.CommentsManage.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.Entities
{
    /// <summary>
    /// 结算详情
    /// </summary>
    public class SettlementView
    {
        /// <summary>
        /// 管理员id
        /// </summary>
        public Guid AdminId { get; set; }
        /// <summary>
        /// 结算id
        /// </summary>
        public Guid SettlementId { get; set; }
        /// <summary>
        /// 管理员昵称
        /// </summary>
        public string AdminNmae { get; set; }
        /// <summary>
        /// 管理员联系方式
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 管理员类型
        /// </summary>
        public int Role { get; set; }
        /// <summary>
        /// 兼职人数（兼职领队结算）
        /// </summary>
        public int JobTotal { get; set; }
        /// <summary>
        /// 结算类型
        /// </summary>
        public SettlementType SettlementType { get; set; }
        /// <summary>
        /// 该任务期间问答精选数
        /// </summary>
        public int TotalAnswerSelected { get; set; }
        /// <summary>
        /// 该任务期间点评精选数
        /// </summary>
        public int TotalSchoolCommentsSelected { get; set; }
        /// <summary>
        /// 结算状态
        /// </summary>
        public SettlementStatus SettlementStatus { get; set; }
        /// <summary>
        /// 结算金额
        /// </summary>
        public float SettlementAmount { get; set; }
        /// <summary>
        /// 任务周期开始时间
        /// </summary>
        public DateTime BeginTime { get; set; }
        /// <summary>
        /// 任务周期结束
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 结算日期
        /// </summary>
        public DateTime PassTime { get; set; }
    }
}
