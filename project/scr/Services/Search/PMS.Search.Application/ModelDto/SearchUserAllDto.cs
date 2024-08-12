using System;
using PMS.Search.Domain.Common;

namespace PMS.Search.Application.ModelDto
{
    public class SearchUserAllDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public SearchTypeEnum SearchType { get; set; }
    }
}
