using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.CommentsManage.Domain.Common
{
    public enum OperationType : int
    {
        [Description("点评")]
        Comment = 1,

        [Description("提问")]
        Question = 2,

        [Description("点评回复")]
        Reply = 3,

        [Description("提问回答")]
        Answer = 4
    }
}
