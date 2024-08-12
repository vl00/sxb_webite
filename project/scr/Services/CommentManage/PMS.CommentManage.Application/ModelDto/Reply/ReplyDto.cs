using System;

namespace PMS.CommentsManage.Application.ModelDto.Reply
{
    public class ReplyDto
    {
        public Guid Id { get; set; }
        public Guid ReplyUserId { get; set; }
        public Guid SchoolCommentId { get; set; }
        public bool isLike { get; set; }
        public string Content { get; set; }
        public int LikeCount { get; set; }
        public int ReplayCount { get; set; }
        public string AddTime { get; set; }
        public bool isStudent { get; set; }
        public bool isAnonymou { get; set; }
        public bool isAttend { get; set; }
        public Guid? ParentID { get; set; }
    }
}
