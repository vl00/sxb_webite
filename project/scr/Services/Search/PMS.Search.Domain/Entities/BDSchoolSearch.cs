using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Search.Domain.Entities
{
    public class BDSchoolSearch
    {
        public BDSchoolSearch() 
        {
            PageIndex = 1;
            PageSize = 10;
        }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Name { get; set; }
        public string Sid { get; set; }
        public int? Grade { get; set; }
        public int? Type { get; set; }
        public int? Province { get; set; }
        public int? City { get; set; }
        public int? Area { get; set; }
        public List<int> AuditStatus { get; set; }
        public DateTime? CreateTime1 { get; set; }
        public DateTime? CreateTime2 { get; set; }
        public List<string> EidtorIds { get; set; }
        public List<string> AuditorIds { get; set; }
    }

    public class BTSearchResult 
    {
        public long Total { get; set; }
        public List<Guid> Sid { get; set; }
    }
}
