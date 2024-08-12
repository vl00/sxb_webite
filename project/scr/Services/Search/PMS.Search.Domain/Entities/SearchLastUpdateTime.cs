using System;
namespace PMS.Search.Domain.Entities
{
    public class SearchLastUpdateTime
    {
        public string Name { get; set; }
        public DateTime UpdateTime { get; set; }
    }

    public class SearchBDlastCreateTime
    {
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }

    }
}
