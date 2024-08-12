using System;
using System.Collections.Generic;

namespace PMS.School.Application.ModelDto.Query
{
    public class ListExtSchoolQuery
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public List<int> ProvinceCode { get; set; } = new List<int>();

        public List<int> CityCode { get; set; } = new List<int>();

        public List<int> AreaCodes { get; set; } = new List<int>();

        public List<Guid> MetroLineIds { get; set; } = new List<Guid>();

        public List<int> MetroStationIds { get; set; } = new List<int>();

        public decimal Distance { get; set; }

        public List<int> Grade { get; set; } = new List<int>();

        public List<string> Type { get; set; } = new List<string>();

        public int MinCost { get; set; }

        public int MaxCost { get; set; }

        /// <summary>
        /// 是否寄宿
        /// </summary>
        public bool? Lodging { get; set; }

        public List<Guid> AuthIds { get; set; } = new List<Guid>();

        public List<Guid> CharacIds { get; set; } = new List<Guid>();

        public List<Guid> AbroadIds { get; set; } = new List<Guid>();

        public List<Guid> CourseIds { get; set; } = new List<Guid>();

        public int Orderby { get; set; }

        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
