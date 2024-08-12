using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.School.Domain.Common
{
    public enum QuestionAndCommentOrder
    {
        [Description("暂无排序规则")]
        None = -1,

        [Description("录入时间")]
        CreateTime = 1,

        [Description("评论数")]
        Reply = 3
    }
}
