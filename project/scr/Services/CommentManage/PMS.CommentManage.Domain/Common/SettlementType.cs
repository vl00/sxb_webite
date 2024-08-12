using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.CommentsManage.Domain.Common
{
    /// <summary>
    /// 结算类型
    /// </summary>
    public enum SettlementType : int
    {
        [Description("未知")]
        Unknown = 0,

        [Description("微信")]
        SettlementWeChat = 1,

        [Description("合同")]
        SettlementContract = 2
    }
}
