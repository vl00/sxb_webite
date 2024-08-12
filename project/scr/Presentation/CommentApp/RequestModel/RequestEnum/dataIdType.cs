using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Api.RequestModel.RequestEnum
{
    /// <summary>
    /// 请求id类型标识
    /// </summary>
    public enum dataIdType : int
    {
        /// <summary>
        /// 点评
        /// </summary>
        [Description("点评")]
        SchooComment = 1,

        /// <summary>
        /// 问题
        /// </summary>
        [Description("问题")]
        Question = 2,

        /// <summary>
        /// 点评回复
        /// </summary>
        [Description("点评回复")]
        CommentReply = 3,

        /// <summary>
        /// 回答
        /// </summary>
        [Description("回答")]
        Answer = 4,

        /// <summary>
        /// 回答回复
        /// </summary>
        [Description("回答回复")]
        AnswerReply = 5
    }
}
