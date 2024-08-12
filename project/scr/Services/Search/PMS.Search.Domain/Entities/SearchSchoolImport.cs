using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Search.Domain.Entities
{
    public class BTSearchSchool
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public DateTime UpdateTime { get; set; }
        public Guid Creator { get; set; }

        public Guid Modifier { get; set; }
        public int Status { get; set; }

        public List<SchoolExtDetail> SchoolExtDetails { get; set; }
    }

    public class SchoolExtDetail 
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Grade { get; set; }
        public int Type { get; set; }
        public int province { get; set; }
        public int City { get; set; }
        public int Area { get; set; }
    }

    public class AuditDetail 
    {
        public Guid Modifier { get; set; }
        public int Status { get; set; }
    }

}
