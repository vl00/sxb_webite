using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.School.Domain.Entities
{
    public class SchoolSearchImport
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid Creator { get; set; }
        public string ExtDetail { get; set; }
        public string AuditDetail { get; set; }
    }
}
