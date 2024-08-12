using System;
using System.Collections.Generic;
using PMS.School.Domain.Common;
using PMS.School.Domain.Entities;

namespace PMS.School.Application.ModelDto
{
    public class SchoolESDataDto
    {
        public Guid Id { get; set; }
        public Guid SchoolId { get; set; }

        public string Name { get; set; }
        public string SchoolName { get; set; }
        public string ExtName { get; set; }

        public GisPoint Location { get; set; }

        public string City { get; set; }
        public int CityCode { get; set; }

        public string Area { get; set; }
        public int AreaCode { get; set; }

        public string Cityarea { get; set; }
        public int? Grade { get; set; }
        public string SchooltypeNewCode { get; set; }
        public string SchooltypeCode { get; set; }
        public string Schooltype { get; set; }

        public List<Guid> MetroLineId { get; set; }
        public List<int> MetroStationId { get; set; }

        public bool? Canteen { get; set; }
        public bool? Lodging { get; set; }

        public int? Studentcount { get; set; }
        public int? Teachercount { get; set; }

        public List<Guid> Authentication { get; set; }
        public List<Guid> Characteristic { get; set; }
        public List<Guid> Abroad { get; set; }
        public List<Guid> Courses { get; set; }

        public List<string> Tags { get; set; }

        public SchoolScoreData Score { get; set; }

        /// <summary>
        /// 学费
        /// </summary>
        public double? Tuition { get; set; }

        public DateTime UpdateTime { get; set; }

        public bool IsValid { get; set; }
        public int Status { get; set; }
    }
}
