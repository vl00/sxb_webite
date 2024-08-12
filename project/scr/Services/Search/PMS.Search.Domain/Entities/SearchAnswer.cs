using System;
namespace PMS.Search.Domain.Entities
{
    public class SearchAnswer
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid QuestionId { get; set; }
        public string Context { get; set; }
        public DateTime? PublishTime { get; set; }

        public DateTime? UpdateTime { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
