using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.CommentsManage.Domain.Common
{
    public enum LabelType : int
    {
        [Description("点评")]
        Comment = 1,

        [Description("提问")]
        Question = 2
    }
}
