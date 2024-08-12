using System;
using System.Collections.Generic;

namespace PMS.Search.Application.ModelDto
{
    public class ResultQuestionDto
    {
        public long Total { get; set; }
        public List<SearchQuestionDto> Questions { get; set; }
    }
}
