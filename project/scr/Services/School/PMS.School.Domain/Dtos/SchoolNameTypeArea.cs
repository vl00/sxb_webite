using iSchool;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.School.Domain.Dtos
{
    public class SchoolNameTypeArea
    {
        public Guid ExtId { get; set; }
        public string SchoolName { get; set; }

        public byte Grade { get; set; }
        public byte Type { get; set; }
        public bool? Discount { get; set; }
        public bool? Diglossia { get; set; }
        public bool? Chinese { get; set; }
        public SchFType0 SchFType0 => new SchFType0(Grade, Type, Discount, Diglossia, Chinese);

        public string CityName { get; set; }
        public string AreaName { get; set; }
    }
}
