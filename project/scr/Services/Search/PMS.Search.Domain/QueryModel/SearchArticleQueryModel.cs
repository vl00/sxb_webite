﻿using PMS.Search.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Search.Domain.QueryModel
{
    public class SearchArticleQueryModel : SearchBaseQueryModel
    {
        public SearchArticleQueryModel() : base()
        {
        }
        public SearchArticleQueryModel(string keyword, int pageIndex, int pageSize) : base(keyword, pageIndex, pageSize)
        {
        }

        public Guid? UserId { get; set; }
    }
}
