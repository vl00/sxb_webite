using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Domain.Common
{
    /// <summary>
    /// 个人动态数据类型
    /// </summary>
    public enum UserDynamicType
    {
        /// <summary>
        /// 点评
        /// </summary>
        Comment = 1,
        /// <summary>
        /// 回答
        /// </summary>
        Answer = 2,
        /// <summary>
        /// 提问
        /// </summary>
        Question = 3,
        /// <summary>
        /// 文章
        /// </summary>
        Article = 4,
        /// <summary>
        /// 直播
        /// </summary>
        Live = 5,
        /// <summary>
        /// 话题
        /// </summary>
        Topic = 6,
        /// <summary>
        /// 点评回复
        /// </summary>
        CommentReply = 7
    }
}
