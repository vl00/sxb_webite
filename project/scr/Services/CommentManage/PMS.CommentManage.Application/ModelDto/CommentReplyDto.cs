using System;
namespace PMS.CommentsManage.Application.ModelDto
{
    public class CommentReplyDto
    {
        public Guid ReplyId { get; set; }

        public string ReplyContent { get; set; }

        public DateTime ReplyTime { get; set; }

        public Guid ReplayUserId { get; set; }

        public Guid? ParentId { get; set; }

        public Guid? ParentUserId { get; set; }
    }
}
