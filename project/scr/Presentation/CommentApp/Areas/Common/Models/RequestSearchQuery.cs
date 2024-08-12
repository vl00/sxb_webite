using PMS.School.Domain.Enum;
using PMS.Search.Application.ModelDto.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Common.Models
{
    public class RequestSearchQuery : SearchSchoolQuery
    {
        public RequestSearchQuery()
        {
            MetroLineIds = new List<Guid>();
            MetroStationIds = new List<int>();
            GradeIds = new List<int>();
            TypeIds = new List<int>();

            Type = new List<string>();
            Grade = new List<int>();
            AreaCodes = new List<int>();

            AuthIds = new List<Guid>();
            CharacIds = new List<Guid>();
            AbroadIds = new List<Guid>();
            CourseIds = new List<Guid>();
        }

        /// <summary>
        /// 地铁线路ID
        /// </summary>
        public List<Guid> MetroLineIds { get; set; } = new List<Guid>();
        /// <summary>
        /// 地铁站点ID
        /// </summary>
        public List<int> MetroStationIds { get; set; } = new List<int>();

        /// <summary>
        /// 1幼儿园 2小学 3初中 4高中
        /// </summary>
        public List<int> GradeIds { get; set; } = new List<int>();
        /// <summary>
        /// 学校类型
        /// </summary>
        public List<int> TypeIds { get; set; } = new List<int>();

        /// <summary>
        /// 学校评分  A+ A B C D
        /// </summary>
        public string ScoreLevel { get; set; }

        /// <summary>
        /// top10学校的逻辑：按照当前城市指定分数维度倒序取前10的学校
        /// </summary>
        public string Top10 { get; set; }

    }
}
