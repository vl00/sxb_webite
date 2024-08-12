using System;
using System.Collections.Generic;

namespace PMS.Search.Domain.Entities
{
    public class SchoolStatsResult
    {
        public String Name { get; set; }
        public List<SchoolHistogram> Hist { get; set; }
    }
}
