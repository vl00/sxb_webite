using System;
namespace PMS.CommentsManage.Domain.Entities.Total
{
    public class LikeTotal
    {
        public Guid SourceId { get; set; }
        public int LikeType { get; set; }
        public int Count { get; set; }
    }
}
