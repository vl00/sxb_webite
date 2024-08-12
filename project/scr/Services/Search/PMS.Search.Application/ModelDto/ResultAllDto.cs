using System;
using System.Collections.Generic;

namespace PMS.Search.Application.ModelDto
{
    public class ResultAllDto
    {
        public long Total { get; set; }
        public List<SearchAllDto> List { get; set; }
    }

    public class ResultIdDto
    {
        public long Total { get; set; }
        public List<SearchIdDto> List { get; set; }
    }
}
