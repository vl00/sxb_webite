using System;
using System.Collections.Generic;

namespace PMS.Search.Domain.Entities
{
    public class SearchSchool : SearchPUV
    {
        public Guid Id { get; set; }
        public Guid? SchoolId { get; set; }

        public string Name { get; set; }
        public SearchGeo Location { get; set; }

        public string City { get; set; }
        public int? CityCode { get; set; }

        public string Area { get; set; }
        public int? AreaCode { get; set; }

        public string Cityarea { get; set; }

        public int? Grade { get; set; }

        public string SchooltypeCode { get; set; }
        public string SchooltypeNewCode { get; set; }
        public string Schooltype { get; set; }

        public List<Guid> MetroLineId { get; set; }
        public List<int> MetroStationId { get; set; }

        public bool? Canteen { get; set; }
        public bool? Lodging { get; set; }
        public bool? Sdextern { get; set; }
        
        public int? Studentcount { get; set; }
        public int? Teachercount { get; set; }

        public List<Guid> Authentication { get; set; }
        public List<Guid> Characteristic { get; set; }
        public List<Guid> Abroad { get; set; }
        public List<Guid> Courses { get; set; }

        public List<string> Tags { get; set; }

        //点评评分
        public double? Star { get; set; }

        //点评数量
        public int? Commentcount { get; set; }
        //问题数量
        public int? Questioncount { get; set; }

        //学校评分
        public SearchSchoolScore Score { get; set; }

        /// <summary>
        /// 学费
        /// </summary>
        public double? Tuition { get; set; }

        public DateTime? UpdateTime { get; set; }
        public bool? IsDeleted { get; set; }
        public int No { get; set; }

        /// <summary>
        /// 浏览数
        /// </summary>
        public long? ViewCount { get; set; }

        /// <summary>
        /// 浏览数计算出的热度值
        /// </summary>
        public long? HotValue { get; set; }

    }


    public class SearchUV
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 浏览数
        /// </summary>
        public long Uv { get; set; }
    }

    public class SearchSchoolViewCount
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 浏览数
        /// </summary>
        public long? ViewCount { get; set; }

        /// <summary>
        /// 浏览数计算出的热度值
        /// </summary>
        public long? HotValue { get; set; }
    }
}
