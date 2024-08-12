using System;
using System.Collections.Generic;

namespace PMS.Search.Application.ModelDto
{
    public class ResultLiveDto
    {
        public long Total { get; set; }
        public List<SearchLiveDto> Lives { get; set; }
    }
}
