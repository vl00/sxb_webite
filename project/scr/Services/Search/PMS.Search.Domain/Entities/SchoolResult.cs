using System;
namespace PMS.Search.Domain.Entities
{
    public class SchoolResult
    {
        public Guid Id { get; set; }
        public Guid? SchoolId { get; set; }
        public string Name { get; set; }
        public double Distance { get; set; }
        public double? Score { get; set; }
    }
}
