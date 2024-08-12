using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.CommentsManage.Domain.Common
{
    /// <summary>
    /// 点评列表排序
    /// </summary>
    public enum CommentListOrder : int
    {
        [Description("无排序")]
        None = 0,

        [Description("智能")]
        Intelligence = 1,

        [Description("时间")]
        AddTime = 2
    }
}
