using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.CommentsManage.Domain.Common
{
    public enum LikeStatus : int
    {
        [Description("点赞")]
        Like = 1, 

        [Description("取消点赞")]
        UnLike = -1
    }
}
