using System;
namespace PMS.School.Domain.Entities
{
    public class EESchool
    {
        public Guid Id { get; set; }

        public Guid Sid { get; set; }

        public int Grade { get; set; }

        public int Type { get; set; }

        public string Name { get; set; }

        public string Province { get; set; }

        public string City { get; set; }

        public string Area { get; set; }

        public string Status { get; set; }

        public DateTime Time { get; set; }
    }
}
