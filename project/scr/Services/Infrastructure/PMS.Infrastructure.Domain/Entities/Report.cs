using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Infrastructure.Domain.Entities
{
    public class Report
    {
        public Report() 
        {
            Id = Guid.NewGuid();
            Time = DateTime.Now;
        }

        public Guid Id { get; set; }
        public Guid UserID { get; set; }
        public Guid DataID { get; set; }
        public byte DataType { get; set; }
        public string Reason { get; set; }
        public string Description { get; set; }
        public string EvidenceURL { get; set; }
        public DateTime Time { get; set; }
        public byte Status { get; set; }
        public List<Report_Img> Report_Imgs { get; set; }
    }

    public class Report_Img
    {
        public Report_Img() 
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public Guid Report_Id { get; set; }
        public string url { get; set; }
        public byte[] bytes { get; set; }
    }
}
