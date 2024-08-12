using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.PaidQA.Domain.Enums
{
    /// <summary>
    /// 流水类型枚举
    /// </summary>
    public enum StatementTypeEnum
    {
        /// <summary>
        /// 充值
        /// </summary>
        Recharge = 1,

        /// <summary>
        /// 支出
        /// </summary>
        Outgoings = 2,

        /// <summary>
        /// 收入
        /// </summary>
        Incomings = 3,

        /// <summary>
        /// 结算
        /// </summary>
        Settlement = 4,

        /// <summary>
        /// 服务费
        /// </summary>
        ServiceFee = 5,
    }
}
