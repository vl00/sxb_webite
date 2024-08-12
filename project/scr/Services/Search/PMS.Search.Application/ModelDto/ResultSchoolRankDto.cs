using System;
using System.Collections.Generic;

namespace PMS.Search.Application.ModelDto
{
    public class ResultSchoolRankDto
    {
        public long Total { get; set; }
        public List<SearchSchoolRankDto> Ranks { get; set; }
    }
}
