using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Web.Areas.PartTimeJob.Models.ViewEntity
{
    public class SettlementViewVo
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
        /// 兼职人数（兼职领队结算）
        /// </summary>
        public int JobTotal { get; set; }
        /// <summary>
        /// 结算类型
        /// </summary>
        public string SettlementType { get; set; }
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
        public string SettlementStatus { get; set; }
        /// <summary>
        /// 结算金额
        /// </summary>
        public decimal SettlementAmount { get; set; }
        /// <summary>
        /// 任务周期开始时间
        /// </summary>
        public string BeginTime { get; set; }
        /// <summary>
        /// 任务周期结束
        /// </summary>
        public string EndTime { get; set; }
        /// <summary>
        /// 结算日期
        /// </summary>
        public DateTime PassTime { get; set; }
        /// <summary>
        /// 是否可结算
        /// </summary>
        public bool IsSettlement { get; set; }
    }
}
