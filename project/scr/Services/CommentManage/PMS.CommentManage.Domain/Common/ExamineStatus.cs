using System;
using System.ComponentModel;

namespace PMS.CommentsManage.Domain.Common
{
    /// <summary>
    /// 点评状态 0 未知 1 未阅 2 已阅 3 已加精 4 已屏蔽 
    /// </summary>
    public enum ExamineStatus : int
    {
        /// <summary>
        /// 无状态
        /// </summary>
        [Description("未知")]
        Unknown = 0,

        /// <summary>
        /// 已屏蔽
        /// </summary>
        [Description("已屏蔽")]
        Block = 4,

        /// <summary>
        /// 未阅
        /// </summary>
        [Description("未阅")]
        Unread = 1,

        /// <summary>
        /// 已阅
        /// </summary>
        [Description("已阅")]
        Readed = 2,

        /// <summary>
        /// 精选
        /// </summary>
        [Description("已加精")]
        Highlight = 3,
    }
}
