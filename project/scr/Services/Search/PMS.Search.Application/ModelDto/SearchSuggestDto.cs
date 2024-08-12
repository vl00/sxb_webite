using System;
using System.Collections.Generic;

namespace PMS.Search.Application.ModelDto
{
    public class SearchSuggestDto
    {
        public int Type { get; set; }
        public Guid SourceId { get; set; }
        public string Name { get; set; }
    }

}
