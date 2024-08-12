using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.School.Domain.Dtos
{
    public class SchSourceInfoDto
    {
        public Guid Sid { get; set; }
        public Guid Eid { get; set; }
        public string Sname { get; set; }
        public string Ename { get; set; }
        public string SchType { get; set; }
        public string Address { get; set; }
        public double Lng { get; set; }
        public double Lat { get; set; }
    }
}
