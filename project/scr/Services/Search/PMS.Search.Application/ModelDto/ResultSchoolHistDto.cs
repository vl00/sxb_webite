using System;
using System.Collections.Generic;

namespace PMS.Search.Application.ModelDto
{
    public class ResultSchoolStatsDto
    {
        public string Name { get; set; }
        public List<ResultSchoolHistDto> Hist { get; set; }
    }

    public class ResultSchoolHistDto
    {
        public string Key { get; set; }
        public long Count { get; set; }
    }
}
