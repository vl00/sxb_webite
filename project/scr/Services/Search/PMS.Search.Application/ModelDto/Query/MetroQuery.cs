using System;
using System.Collections.Generic;

namespace PMS.Search.Application.ModelDto.Query
{
    public class MetroQuery
    {
        public Guid LineId { get; set; }
        public List<int> StationIds { get; set; }
    }
}
