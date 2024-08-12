using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.Search.Domain.Common
{
    public enum LectureStatus : int
    {
        // 0待审核 1审核中  2未开课   3待开课  4开课中   5已结束  6 审核失败

        /// <summary>
        /// 待审核
        /// </summary>
        [Description("待审核")]
        Unaudit = 0,
        /// <summary>
        /// 审核中
        /// </summary>
        [Description("审核中")]
        Auditing = 1,
        /// <summary>
        /// 未直播
        /// </summary>
        [Description("未直播")]
        Unstart = 2,
        /// <summary>
        /// 待直播
        /// </summary>
        [Description("待直播")]
        Waiting = 3,
        /// <summary>
        /// 直播中
        /// </summary>
        [Description("直播中")]
        Begin = 4,
        /// <summary>
        /// 已结束
        /// </summary>
        [Description("已结束")]
        End = 5,
        /// <summary>
        /// 审核失败
        /// </summary>
        [Description("审核失败")]
        Notpass = 6,
    }
}
