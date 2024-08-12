using System;
namespace PMS.CommentsManage.Application.ModelDto
{
    public class LikeDto
    {
        public Guid SourceId { get; set; }

        public bool IsLike { get; set; }
    }
}
