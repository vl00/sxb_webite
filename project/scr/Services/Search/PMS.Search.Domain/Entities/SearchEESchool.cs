using System;
namespace PMS.Search.Domain.Entities
{
    public class SearchEESchool
    {
        public Guid Id { get; set; }

        public int Grade { get; set; }

        public int Type { get; set; }

        public string Name { get; set; }

        public string Province { get; set; }

        public string City { get; set; }

        public string Area { get; set; }

        public string Cityarea { get; set; }

        public string Status { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}
