using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.UserManage.Domain.Common
{
    public enum Tips
    {
        [Description("邀请我的")]
        InviteMe = 1,

        [Description("获赞")]
        LikeTotal = 2,

        [Description("新增粉丝")]
        NewFans = 3,

        [Description("回复我的")]
        ReplyMe = 4
    }
}
