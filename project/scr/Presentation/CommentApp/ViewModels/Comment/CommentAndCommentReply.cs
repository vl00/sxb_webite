using Sxb.Web.Models.Comment;
using Sxb.Web.Models.Replay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.ViewModels.Comment
{
    /// <summary>
    /// 点评和回复集合
    /// </summary>
    public class CommentAndCommentReply
    {
        public List<CommentList> CommentModels { get; set; }
        public List<ReplayExhibition> CommentReplyModels { get; set; }
    }
}
