using System;
using System.Collections.Generic;

namespace PMS.Search.Application.ModelDto
{
    public class ResultCommentDto
    {
        public long Total { get; set; }
        public List<SearchCommentDto> Comments { get; set; }
    }
}
