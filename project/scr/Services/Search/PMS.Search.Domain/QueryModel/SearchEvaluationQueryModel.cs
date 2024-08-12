using PMS.Search.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Search.Domain.QueryModel
{
    public class SearchEvaluationQueryModel : SearchBaseQueryModel
    {
        public SearchEvaluationQueryModel() : base()
        {
        }

        public SearchEvaluationQueryModel(string keyword, int pageIndex, int pageSize) : base(keyword, pageIndex, pageSize)
        {
        }

        public bool SearchTitleOnly { get; set; }
    }
}
