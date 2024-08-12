using System;
namespace PMS.Search.Domain.Entities
{
    public class SchoolHistogram
    {
        public string Key { get; set; }
        public double? From { get; set; }
        public double? To { get; set; }
        public long Count { get; set; }
    }
}
