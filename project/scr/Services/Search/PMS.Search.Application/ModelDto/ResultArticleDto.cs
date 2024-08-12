using System;
using System.Collections.Generic;

namespace PMS.Search.Application.ModelDto
{
    public class ResultArticleDto
    {
        public long Total { get; set; }
        public List<SearchArticleDto> Articles { get; set; }
    }
}
