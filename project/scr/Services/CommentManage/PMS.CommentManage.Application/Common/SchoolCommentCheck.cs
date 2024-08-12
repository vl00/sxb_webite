using PMS.CommentsManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.Common
{
    /// <summary>
    /// 点评状态检测
    /// </summary>
    public static class SchoolCommentCheck
    {
        /// <summary>
        /// 该点评是否为精选点评
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public static bool CommentIsSelected(SchoolComment comment)
        {
            return comment.ReplyCount > 10;
        }
    }
}
