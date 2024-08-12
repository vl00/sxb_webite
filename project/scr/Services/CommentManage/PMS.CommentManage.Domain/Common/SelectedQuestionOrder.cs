using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.CommentsManage.Domain.Common
{
    public enum SelectedQuestionOrder : int
    {
        [Description("无排序")]
        None = -1,

        [Description("智能")]
        Intelligence = 0,

        [Description("发布时间")]
        CreateTime = 1,

        [Description("口碑评级")]
        QuestionTotal = 2,

        [Description("回复数")]
        Answer = 3
    }
}
