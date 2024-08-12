using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.CommentsManage.Domain.Common
{
    public enum ImageType : int
    {
        [Description("点评")]
        Comment = 1,

        [Description("问答")]
        Answer = 2,

        [Description("问题")]
        Question = 3,

        [Description("举报")]
        Report = 4
    }
}
