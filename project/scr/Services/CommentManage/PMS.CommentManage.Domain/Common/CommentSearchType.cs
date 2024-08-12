using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.CommentsManage.Domain.Common
{
    /// <summary>
    /// 点评查询
    /// </summary>
    public enum CommentSearchType : int
    {
        [Description("全部")]
        All = 1,

        [Description("精华")]
        Selected = 2,

        [Description("过来人")]
        Past = 3,

        [Description("辟谣")]
        Rumor = 4,

        [Description("好评")]
        Praise = 5,

        [Description("差评")]
        Bad = 6,

        [Description("有图")]
        Imagers = 7,

        [Description("高中部")]
        HighSchool = 8,

        [Description("国际部")]
        International = 9
    }
}
