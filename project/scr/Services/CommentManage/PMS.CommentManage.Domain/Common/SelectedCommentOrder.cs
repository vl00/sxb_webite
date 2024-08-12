using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.CommentsManage.Domain.Common
{
    public enum SelectedCommentOrder : int
    {
        None = -1,

        [Description("点评时间")]
        AddTimeOrder = 1,

        [Description("口碑评级")]
        Grade = 2,

        [Description("点评数")]
        CommentTotal = 3,

        [Description("智能")]
        Intelligence = 0
    }
}
