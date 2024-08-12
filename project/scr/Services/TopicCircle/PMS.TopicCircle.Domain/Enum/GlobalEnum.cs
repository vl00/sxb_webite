using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace PMS.TopicCircle.Domain.Enum
{
    public class GlobalEnum
    {


        /// <summary>
        /// 话题类型 类型  0 无 1 文本 2 图片 4 直播 8 文章 16 院校 32 点评 64 回答 128 外链 256 问题
        /// </summary>
        [Flags]
        public enum TopicType
        {
            /// <summary>
            /// 无
            /// </summary>
            None,
            /// <summary>
            /// 文本
            /// </summary>
            Text,
            /// <summary>
            /// 图片
            /// </summary>
            Image,
            /// <summary>
            /// 直播
            /// </summary>
            Live = 4,
            /// <summary>
            /// 文章
            /// </summary>
            Article = 8,
            /// <summary>
            /// 院校
            /// </summary>
            School = 16,
            /// <summary>
            /// 点评
            /// </summary>
            Comment = 32,
            /// <summary>
            /// 问题
            /// </summary>
            Answer = 64,
            /// <summary>
            /// 外链
            /// </summary>
            OuterUrl = 128,
            /// <summary>
            /// 问题
            /// </summary>
            Question = 256,
        }


        /// <summary>
        /// 话题置顶类型
        /// </summary>
        [Flags]
        public enum TopicTopType
        {
            /// <summary>
            /// 无
            /// </summary>
            None,
            /// <summary>
            /// 圈主置顶
            /// </summary>
            Circle,
            /// <summary>
            /// 管理员置顶
            /// </summary>
            Admin,
            All
        }

        /// <summary>
        /// 话题排序
        /// </summary>
        public enum TopicSort
        {
            [Description("T.LastReplyTime  DESC")]
            None,
            /// <summary>
            /// 按置顶时间排序
            /// </summary>
            [Description("T.TopTime  DESC, T.CreateTime  DESC")]
            Top,
            /// <summary>
            /// 按加精时间排序
            /// </summary>
            [Description("T.GoodTime DESC, T.CreateTime DESC")]
            Good,
            /// <summary>
            /// 按回复时间排序
            /// </summary>
            [Description("T.LastReplyTime DESC, T.CreateTime  DESC")]
            Reply,
            /// <summary>
            /// 按动态时间排序
            /// </summary>
            [Description("T.Time DESC, T.CreateTime  DESC")]
            Time
        }
    }
}
