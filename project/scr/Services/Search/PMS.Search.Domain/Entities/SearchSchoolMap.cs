using System;
using System.Collections.Generic;

namespace PMS.Search.Domain.Entities
{
    public class SearchSchoolMap
    {
        public long Count { get; set; }
        public long ViewCount { get; set; }
        public List<SearchMap> List { get; set; }
    }

    public class SearchMap
    {
        public Guid Id { get; set; }
        public Guid? SchoolId { get; set; }

        public string Name { get; set; }

        public SearchGeo Location { get; set; }

        public long Count { get; set; }
    }
}
