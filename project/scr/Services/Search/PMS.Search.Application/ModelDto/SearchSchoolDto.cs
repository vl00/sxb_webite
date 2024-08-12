using System;
namespace PMS.Search.Application.ModelDto
{
    public class SearchSchoolDto
    {
        public Guid Id { get; set; }
        public Guid SchoolId { get; set; }

        public string Name { get; set; }
        public double Distance { get; set; }

        public double? SearchScore { get; set; }
    }
}
