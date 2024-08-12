using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ProductManagement.API.Http.RequestCommon
{
    [Obsolete("See PMS.UserManage.Domain.Common.EnumSet.MessageType")]
    public enum MessageType : int
    {
        [Description("通知")]
        Notice = 0,

        [Description("回复")]
        Reply = 1,

        [Description("邀请点评")]
        InviteComment = 2,

        [Description("邀请提问")]
        InviteQuesion = 3,

        [Description("邀请回答")]
        InviteAnswer = 4,

        [Description("新的攻略")]
        NewStrategy = 5,

        [Description("点评关注的学校")]
        CommentFollowSchool = 6,

        [Description("邀请回复")]
        InviteReply = 7,

        [Description("邀请回复点评的回复 | 邀请回复回答的回复")]
        ReplyAnswer = 8,

        [Description("点赞点评")]
        LikeComment = 9,

        [Description("点赞回复")]
        LikeReply = 10,

        [Description("点赞回答")]
        LikeAnswer = 11
    }
    [Obsolete("See PMS.UserManage.Domain.Common.EnumSet.MessageDataType")]
    public enum MessageDataType : int
    {
        [Description("文章")]
        Article = 0,

        [Description("学校学部")]
        SchoolSection = 1,

        [Description("问答")]
        Question = 2,

        [Description("点评")]
        Comment = 3,

        [Description("点赞")]
        Like = 4
    }
}
