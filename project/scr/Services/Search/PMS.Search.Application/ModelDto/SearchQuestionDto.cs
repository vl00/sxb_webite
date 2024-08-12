using System;
namespace PMS.Search.Application.ModelDto
{
    public class SearchQuestionDto
    {
        public Guid Id { get; set; }
        public string Context { get; set; }
        public DateTime PublishTime { get; set; }
    }
}
