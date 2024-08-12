using System;
namespace PMS.Search.Application.ModelDto
{
    public class SearchCommentDto
    {
        public Guid Id { get; set; }
        public string Context { get; set; }
        public DateTime PublishTime { get; set; }
    }
}
