using System;
using System.Collections.Generic;

namespace PMS.Search.Application.ModelDto
{
    public class SearchSchoolRankDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public List<string> Schools { get; set; }
    }
}
