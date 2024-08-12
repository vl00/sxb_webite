using System;
using System.Collections.Generic;

namespace PMS.Search.Application.ModelDto
{
    public class ResultMapSchoolDto
    {
        public long Count { get; set; }
        public long ViewCount { get; set; }
        public List<SearchMapDto> List { get; set; }
    }

    public class SearchMapDto
    {
        public Guid Id { get; set; }
        public Guid SchoolId { get; set; }

        public string Name { get; set; }
        /// <summary>
        /// 维度
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude { get; set; }

        public long Count { get; set; }
    }
}
