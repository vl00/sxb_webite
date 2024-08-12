using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Search.Domain.Entities
{
    public class SearchSignSchool
    {
        public Guid Id { get; set; }
        public string SchName { get; set; }
        public int Type { get; set; }
        public bool IsDel { get; set; }
    }
}
