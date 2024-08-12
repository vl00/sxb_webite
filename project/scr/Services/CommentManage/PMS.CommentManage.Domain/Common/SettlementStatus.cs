using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.CommentsManage.Domain.Common
{
    /// <summary>
    /// 结算状态
    /// </summary>
    public enum SettlementStatus : int
    {
        //第二次需求变更结算状态整前端展示对应数据库存储字段及标识码：
        //【7：未完成【进行中，已延期，未完成】】
        //【8：已结算【已结算】】 
        //【9：待结算【未结算,结算中】】

        /// <summary>
        /// 延期任务已完成，在定时任务中进行检测是否可结算
        /// </summary>
        [Description("延期完成")]
        DelaySettlement = 5,

        /// <summary>
        /// 已提交还未通过审核，但任务期限已过
        /// </summary>
        [Description("已延期")]
        Delay = 4,

        /// <summary>
        /// 已结算
        /// </summary>
        [Description("已结算")]
        Settled = 3,
        
        /// <summary>
        /// 结算中
        /// </summary>
        [Description("结算中")]
        Settlement = 2,

        /// <summary>
        /// 未结算
        /// </summary>
        [Description("未结算")]
        Unsettled = 1,

        /// <summary>
        /// 进行中
        /// </summary>
        [Description("进行中")]
        Ongoing = 0,
        
        /// <summary>
        /// 未完成
        /// </summary>
        [Description("未完成")]
        Fail = -1
    }
}
