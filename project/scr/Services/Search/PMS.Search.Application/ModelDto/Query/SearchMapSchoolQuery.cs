using PMS.Search.Domain.Entities;
using PMS.Search.Domain.QueryModel;
using ProductManagement.Infrastructure.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PMS.Search.Application.ModelDto.Query
{
    public class SearchSchoolQuery : SearchSuggestionBase, IConvert<SearchSchoolQueryModel>
    {
        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string Keyword { get; set; }

        public double Longitude { get; set; }
        public double Latitude { get; set; }
        /// <summary>
        /// should 城市ID（选择城市用）
        /// </summary>
        public int? CurrentCity { get; set; }
        /// <summary>
        /// must 城市ID（选择城市用）
        /// </summary>
        public int? MustCurrentCity { get; set; }

        public List<int> ProvinceCode { get; set; } = new List<int>();
        /// <summary>
        /// 城市ID（筛选城市用）
        /// </summary>
        public List<int> CityCode { get; set; } = new List<int>();
        /// <summary>
        /// 市区ID
        /// </summary>
        public List<int> AreaCodes { get; set; } = new List<int>();
        /// <summary>
        /// 地铁站点ID
        /// </summary>
        public List<MetroQuery> MetroIds { get; set; } = new List<MetroQuery>();
        /// <summary>
        /// 距离
        /// </summary>
        public decimal Distance { get; set; }
        /// <summary>
        /// 学校类型 lx210 lx310
        /// </summary>
        public List<string> Type { get; set; } = new List<string>();
        /// <summary>
        /// 学段
        /// </summary>
        public List<int> Grade { get; set; } = new List<int>();
        /// <summary>
        /// 最低学费
        /// </summary>
        public int? MinCost { get; set; }
        /// <summary>
        /// 最高学费
        /// </summary>
        public int? MaxCost { get; set; }

        /// <summary>
        /// 寄宿类型 0 暂未收录 1 走读 2 寄宿 3 寄宿&走读
        /// </summary>
        public int? Lodging { get; set; }

        public (bool? lodging, bool? sdextern) GetLodgingSdextern()
        {
            //是否寄宿
            bool? lodging = null;
            //是否走读
            bool? sdextern = null;
            if (Lodging == 3)
            {
                lodging = true;
                sdextern = true;
            }
            else if (Lodging == 2)
            {
                lodging = true;
                sdextern = false;
            }
            else if (Lodging == 1)
            {
                lodging = false;
                sdextern = true;
            }
            return (lodging, sdextern);
        }

        /// <summary>
        /// 是否有饭堂
        /// </summary>
        public bool? Canteen { get; set; }
        /// <summary>
        /// 最低学生人数
        /// </summary>
        public int? MinStudentCount { get; set; }
        /// <summary>
        /// 最高学生人数
        /// </summary>
        public int? MaxStudentCount { get; set; }
        /// <summary>
        /// 最低教师人数
        /// </summary>
        public int? MinTeacherCount { get; set; }
        /// <summary>
        /// 最高教师人数
        /// </summary>
        public int? MaxTeacherCount { get; set; }

        //最低综合评分
        public int? MinComposite { get; set; }
        //最高综合评分
        public int? MaxComposite { get; set; }

        //最低师资评分
        public int? MinTeach { get; set; }
        //最高师资评分
        public int? MaxTeach { get; set; }

        //最低硬件评分
        public int? MinHard { get; set; }
        //最高硬件评分
        public int? MaxHard { get; set; }

        //最低课程评分
        public int? MinCourse { get; set; }
        //最高课程评分
        public int? MaxCourse { get; set; }

        //最低学术评分
        public int? MinLearn { get; set; }
        //最高学术评分
        public int? MaxLearn { get; set; }

        //最低费用评分
        public int? MinCostScore { get; set; }
        //最高费用评分
        public int? MaxCostScore { get; set; }

        //最低总分评分
        public int? MinTotal { get; set; }
        //最高总分评分
        public int? MaxTotal { get; set; }

        /// <summary>
        /// 学校认证
        /// </summary>
        public List<Guid> AuthIds { get; set; } = new List<Guid>();
        /// <summary>
        /// 特色课程
        /// </summary>
        public List<Guid> CharacIds { get; set; } = new List<Guid>();
        /// <summary>
        /// 出国方向
        /// </summary>
        public List<Guid> AbroadIds { get; set; } = new List<Guid>();
        /// <summary>
        /// 课程设置
        /// </summary>
        public List<Guid> CourseIds { get; set; } = new List<Guid>();

        /// <summary>
        /// 排序  0 默认排序 1 距离从远到近  2 距离从近到远  3 学费从高到低 4 学费从低到高
        /// </summary>
        public int Orderby { get; set; }

        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public SearchSchoolQueryModel ConvertTo()
        {

            var query = this;
            var grades = new List<int>();
            var schoolTypeCodes = query.Type;

            //如果同时筛选了一级和二级, 则排除grade
            if (query.Grade.Any())
            {
                //去掉学校类型所属的学段
                var g = query.Type.Where(q => q.Count() > 2)
                    .Select(q => q[2].ToString()).GroupBy(q => q).Select(q => Convert.ToInt32(q.Key)).ToList();

                grades = query.Grade.Where(q => !g.Contains(q)).ToList();
            }

            //todo:后面重新导入学校数据，lodging字段改为int，解决搜索“寄宿&走读”的学校问题
            //bool? lodging = (query.Lodging == 3 || query.Lodging == 2) ? true : (bool?)null;
            //bool? sdextern = (query.Lodging == 3 || query.Lodging == 1) ? true : (bool?)null;
            var (lodging, sdextern) = query.GetLodgingSdextern();

            var queryData = new SearchSchoolQueryModel
            {
                Keyword = query.Keyword,
                Latitude = query.Latitude,
                Longitude = query.Longitude,
                CurrentCity = query.CurrentCity,
                MustCurrentCity = query.MustCurrentCity,
                Provinces = query.ProvinceCode,
                Citys = query.CityCode,
                Areas = query.AreaCodes,
                MetroIds = query.MetroIds.Select(q => new SearchMetro { LineId = q.LineId, StationIds = q.StationIds.Select(p => p).ToList() }).ToList(),
                Grades = grades,
                SchoolTypeCodes = schoolTypeCodes,
                Distance = query.Distance,
                MinCost = query.MinCost,
                MaxCost = query.MaxCost,
                MinStudentCount = query.MinStudentCount,
                MaxStudentCount = query.MaxStudentCount,
                MinTeacherCount = query.MinTeacherCount,
                MaxTeacherCount = query.MaxTeacherCount,
                Lodging = lodging,
                Sdextern = sdextern,
                Canteen = query.Canteen,
                AuthIds = query.AuthIds,
                CharacIds = query.CharacIds,
                AbroadIds = query.AbroadIds,
                CourseIds = query.CourseIds,
                MinComposite = query.MinComposite,
                MaxComposite = query.MaxComposite,
                MinTeach = query.MinTeach,
                MaxTeach = query.MaxTeach,
                MinHard = query.MinHard,
                MaxHard = query.MaxHard,
                MinCourse = query.MinCourse,
                MaxCourse = query.MaxCourse,
                MinLearn = query.MinLearn,
                MaxLearn = query.MaxLearn,
                MinCostScore = query.MinCostScore,
                MaxCostScore = query.MaxCostScore,
                MinTotal = query.MinTotal,
                MaxTotal = query.MaxTotal,
                PageSize = query.PageSize,
                PageNo = query.PageNo,
                OrderBy = query.Orderby
            };
            return queryData;
        }
    }
}
