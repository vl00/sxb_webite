using System;
namespace PMS.Search.Domain.Entities
{
    public class SearchQuestion : SearchPUV
    {
        public Guid Id { get; set; }
        public Guid? Eid { get; set; }
        public Guid UserId { get; set; }
        public string Context { get; set; }
        public DateTime? PublishTime { get; set; }
        public int? ReplyCount { get; set; }
        public int? CityCode { get; set; }
        public int? AreaCode { get; set; }
        public int? Grade { get; set; }
        public int? Type { get; set; }
        public bool? Lodging { get; set; }

        public DateTime? UpdateTime { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
