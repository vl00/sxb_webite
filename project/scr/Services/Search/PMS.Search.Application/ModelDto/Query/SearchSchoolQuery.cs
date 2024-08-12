using System;
using System.Collections.Generic;

namespace PMS.Search.Application.ModelDto.Query
{
    public class SearchMapSchoolQuery
    {
        public string KeyWords { get; set; }

        public List<Guid> MetroLineIds { get; set; } = new List<Guid>();

        public List<int> MetroStationIds { get; set; } = new List<int>();


        public int CityCode { get; set; }

        public List<int> Grade { get; set; } = new List<int>();

        public List<string> Type { get; set; } = new List<string>();

        /// <summary>
        /// 是否寄宿
        /// </summary>
        public bool? Lodging { get; set; }

        public bool? Canteen { get; set; }

        public List<Guid> AuthIds { get; set; } = new List<Guid>();

        public List<Guid> CharacIds { get; set; } = new List<Guid>();

        public List<Guid> AbroadIds { get; set; } = new List<Guid>();

        public List<Guid> CourseIds { get; set; } = new List<Guid>();

        public double SWLat { get; set; }
        public double SWLng { get; set; }

        public double NELat { get; set; }
        public double NELng { get; set; }

        public int Level { get; set; }
    }
}
