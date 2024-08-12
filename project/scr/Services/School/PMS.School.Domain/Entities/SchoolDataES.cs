using System;
namespace PMS.School.Domain.Entities
{
    public class SchoolDataES
    {
        public Guid Id { get; set; }
        public Guid SchoolId { get; set; }

        public string Name { get; set; }

        public string SchoolName { get; set; }
        public string ExtName { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public double Latitude { get; set; }

        public string City { get; set; }
        public int CityCode { get; set; }

        public string Area { get; set; }
        public int AreaCode { get; set; }

        public string Cityarea { get; set; }

        public int Grade { get; set; }
        public int Type { get; set; }
        public bool Discount { get; set; }
        public bool Diglossia { get; set; }
        public bool Chinese { get; set; }

        public string SchFtype { get; set; }

        public string MetroLineId { get; set; }
        public string MetroStationId { get; set; }

        public bool? Canteen { get; set; }
        public bool? Lodging { get; set; }
        /// <summary>
        /// 是否走读
        /// </summary>
        public bool? Sdextern { get; set; }

        public int? Studentcount { get; set; }
        public int? Teachercount { get; set; }

        public string Authentication { get; set; }
        public string Characteristic { get; set; }
        public string Abroad { get; set; }
        public string Courses { get; set; }

        public string TagName { get; set; }
        /// <summary>
        /// 学费
        /// </summary>
        public double? Tuition { get; set; }

        public DateTime UpdateTime { get; set; }

        public bool IsValid { get; set; }
        public int Status { get; set; }

        public double? Score15 { get; set; }
        public double? Score16 { get; set; }
        public double? Score17 { get; set; }
        public double? Score18 { get; set; }
        public double? Score19 { get; set; }
        public double? Score20 { get; set; }
        public double? Score21 { get; set; }
        public double? Score22 { get; set; }
    }
}
