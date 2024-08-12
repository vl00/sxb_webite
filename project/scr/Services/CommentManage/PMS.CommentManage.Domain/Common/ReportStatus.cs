using System;
using System.ComponentModel;

namespace PMS.CommentsManage.Domain.Common
{
    public enum ReportStatus : int
    {
        [Description("未回复")]
        Unanswered = 0,


        [Description("已回复")]
        Answered = 1
    }
}
