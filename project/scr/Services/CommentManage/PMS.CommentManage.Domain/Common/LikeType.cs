using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.CommentsManage.Domain.Common
{
    public enum LikeType : int
    {
        [Description("点评")]
        Comment = 1,

        [Description("问题")]
        Quesiton = 2,

        [Description("回复")]
        Reply = 3,

        [Description("答题")]
        Answer = 4
    }
}
