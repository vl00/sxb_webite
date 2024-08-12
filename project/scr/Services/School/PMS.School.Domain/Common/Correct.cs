using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.School.Domain.Common
{
    /// <summary>
    /// 纠错类型
    /// </summary>
    public enum SchCorrectErrType
    {
        /// <summary>
        /// 位置信息错误
        /// </summary>
        [Description("位置信息错误")]
        Location = 1,
        /// <summary>
        /// 学校名称错误
        /// </summary>
        [Description("学校名称错误")]
        Name = 2,
        /// <summary>
        /// 学校类型错误
        /// </summary>
        [Description("学校类型错误")]
        SchType = 3,
        /// <summary>
        /// 其它
        /// </summary>
        [Description("其它")]
        Other = 4,
    }

    /// <summary>
    /// 纠错状态
    /// </summary>
    public enum SchCorrectStatus
    {
        /// <summary>
        /// 未处理
        /// </summary>
        [Description("未处理")]
        UnHandled = 1,
        /// <summary>
        /// 已处理
        /// </summary>
        [Description("已处理")]
        Handled = 2,
        /// <summary>
        /// 不受理
        /// </summary>
        [Description("不受理")]
        Reject = 3,
    }
}
