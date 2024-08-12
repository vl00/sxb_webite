using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.CommentsManage.Domain.Common
{
    public enum ReportDataType : int
    {
        [Description("数据源")]
        DataSource = 1,

        [Description("回复")]
        Replay = 2
    }
}
