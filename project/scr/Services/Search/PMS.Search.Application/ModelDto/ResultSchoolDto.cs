using System;
using System.Collections.Generic;

namespace PMS.Search.Application.ModelDto
{
    public class ResultSchoolDto
    {
        public long Total { get; set; }
        public List<SearchSchoolDto> Schools { get; set; }
    }
}
