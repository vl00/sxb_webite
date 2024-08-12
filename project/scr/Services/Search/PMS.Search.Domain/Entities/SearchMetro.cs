using System;
using System.Collections.Generic;

namespace PMS.Search.Domain.Entities
{
    public class SearchMetro
    {
        public Guid LineId { get; set; }
        public List<int> StationIds { get; set; }
    }
}
