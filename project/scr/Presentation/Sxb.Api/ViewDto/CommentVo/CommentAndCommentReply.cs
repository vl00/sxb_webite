using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Api.ViewDto.CommentVo
{
    /// <summary>
    /// 点评和回复
    /// </summary>
    public class CommentAndCommentReply
    {
        /// <summary>
        /// 点评实体集合
        /// </summary>
        public List<CommentModel> commentModels { get; set; }
        /// <summary>
        /// 点评回复集合
        /// </summary>
        public List<CommentReplyModel> commentReplyModels { get; set; }
    }
}
