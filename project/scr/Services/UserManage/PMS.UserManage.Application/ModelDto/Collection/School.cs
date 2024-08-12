using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace PMS.UserManage.Application.ModelDto.Collection
{
    public class School
    {
        public Guid Sid { get; set; }
        public Guid ExtId { get; set; }
        public byte Grade { get; set; }
        public SchoolType Type { get; set; }
        public string SchoolName { get; set; }
        public string ExtName { get; set; }
        public double? Tuition { get; set; }
        public bool? Lodging { get; set; }
        public int? Score { get; set; }
        public List<string> Tags { get; set; }
        public int? City { get; set; }
        public int? Area { get; set; }
        public int? Province { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }

        ////扩展字段
        public string CityName { get; set; }
        public string AreaName { get; set; }
        public string Distance { get; set; }
    }
}
