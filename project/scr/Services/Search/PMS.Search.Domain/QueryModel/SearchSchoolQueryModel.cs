using System;
using System.Collections.Generic;
using PMS.Search.Domain.Entities;

namespace PMS.Search.Domain.QueryModel
{
    public class SearchSchoolQueryModel
    {
        public string Keyword { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        /// <summary>
        /// should
        /// </summary>
        public int? CurrentCity { get; set; }
        /// <summary>
        /// must
        /// </summary>
        public int? MustCurrentCity { get; set; }
        public List<int> Provinces { get; set; }
        public List<int> Citys { get; set; }
        public List<int> Areas { get; set; }
        public List<SearchMetro> MetroIds { get; set; }
        public List<int> Grades { get; set; }
        public List<string> SchoolTypeCodes { get; set; }
        public int OrderBy { get; set; }
        public bool IsDefault { get; set; }
        public decimal Distance { get; set; }
        public int? MinCost { get; set; }
        public int? MaxCost { get; set; }
        public int? MinStudentCount { get; set; }
        public int? MaxStudentCount { get; set; }
        public int? MinTeacherCount { get; set; }
        public int? MaxTeacherCount { get; set; }
        public bool? Lodging { get; set; }
        public bool? Sdextern { get; set; }
        public bool? Canteen { get; set; }
        public List<Guid> AuthIds { get; set; }
        public List<Guid> CharacIds { get; set; }
        public List<Guid> AbroadIds { get; set; }
        public List<Guid> CourseIds { get; set; }
        public int? MinComposite { get; set; }
        public int? MaxComposite { get; set; }
        public int? MinTeach { get; set; }
        public int? MaxTeach { get; set; }
        public int? MinHard { get; set; }
        public int? MaxHard { get; set; }
        public int? MinCourse { get; set; }
        public int? MaxCourse { get; set; }
        public int? MinLearn { get; set; }
        public int? MaxLearn { get; set; }
        public int? MinCostScore { get; set; }
        public int? MaxCostScore { get; set; }
        public int? MinTotal { get; set; }
        public int? MaxTotal { get; set; }

        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
