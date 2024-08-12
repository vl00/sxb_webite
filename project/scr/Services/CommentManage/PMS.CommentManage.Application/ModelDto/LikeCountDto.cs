using System;
namespace PMS.CommentsManage.Application.ModelDto
{
    public class LikeCountDto
    {
        public Guid SourceId { get; set; }
        public int LikeType { get; set; }
        public int Count { get; set; }
    }
}
