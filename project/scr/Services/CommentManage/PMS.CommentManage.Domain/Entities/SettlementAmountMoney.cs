using PMS.CommentsManage.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PMS.CommentsManage.Domain.Entities
{
    /// <summary>
    /// 兼职领队：当旗下每有一名兼职的点评与问答设置为精选则入账 【2元】 
    /// 兼职：需在14天内必须有 （点评 + 问答） = 5（精选） 则入账【35元】，在14天内提交但还未通过审核则任务
    /// 还算继续，但金额需要在审核通过5条的下个任务期结算，若不存在还未审核且未通过5条精选则表示任务失败，不予结算
    /// 
    /// 
    /// 启用定时任务（启用时间每天23：59，检测当日是否存在需要结算管理员）
    /// 写入规则：
    /// 1：该兼职领队 | 兼职在次表中未存在记录则检测该管理员账号入炉系统日期
    ///     与当前日期判断是已存在14天，存在则进行录入数据，检测是否可为结算或失败（兼职：未完成）
    /// 2：该管理员在该表中已存在记录，则根据该管理员数据EndTime进行desc检测与当前日期是否存在14天 
    /// 
    /// 
    /// 兼职逻辑修改，精选3元一条
    /// </summary>
    [Table("SettlementAmountMoneys")]
    public class SettlementAmountMoney
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 结算者
        /// </summary>
        [ForeignKey("PartTimeJobAdmin")]
        public Guid PartTimeJobAdminId { get; set; }
        /// <summary>
        /// 该阶段已完成的点评总数
        /// </summary>
        public int TotalSchoolCommentsSelected { get; set; }
        /// <summary>
        /// 该阶段已完成的问答总数
        /// </summary>
        public int TotalAnswerSelected { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime BeginTime { get; set; }
        /// <summary>
        /// 截止日期
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 结算日期
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 结算金额
        /// </summary>
        public float SettlementAmount { get; set; }
        /// <summary>
        /// 结算状态
        /// </summary>
        public SettlementStatus SettlementStatus { get; set; }

        /// <summary>
        /// 所属角色
        /// </summary>
        public int PartJobRole { get; set; }

        public virtual PartTimeJobAdmin PartTimeJobAdmin { get; set; }
    }
}
