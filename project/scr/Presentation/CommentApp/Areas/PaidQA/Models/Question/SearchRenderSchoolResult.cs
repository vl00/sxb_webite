using PMS.School.Domain.Common;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.PaidQA.Models
{
    public class SearchRenderSchoolResult
    {

        public Guid Sid { get; set; }

        public Guid ExtId { get; set; }

        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }

        public string Province { get; set; }
        public string City { get; set; }
        public string Area { get; set; }

        public int ProvinceCode { get; set; }
        public int CityCode { get; set; }
        public int AreaCode { get; set; }

        /// <summary>
        /// 学费
        /// </summary>
        public double? Tuition { get; set; }
        /// <summary>
        /// 格式化学费
        /// </summary>
        public string TuitionPerYear;

        /// <summary>
        /// 距离
        /// </summary>
        public double? Distance { get; set; }
        public string DistanceString => Math.Round((decimal)((Distance ?? 0) / 1000), 2) + "km";

        /// <summary>
        /// 评分
        /// </summary>
        public double? Score { get; set; }
        /// <summary>
        /// 评级
        /// </summary>
        public string Str_Score { get; set; }
        /// <summary>
        /// 评论条数
        /// </summary>
        public int CommentCount { get; set; }
        /// <summary>
        /// taglist
        /// </summary>
        public List<string> Tags { get; set; }

        public int Grade { get; set; }
        public int Type { get; set; }

        public string TypeName { get; set; }

        /// <summary>
        /// 是否寄宿
        /// </summary>
        public bool? Lodging { get; set; }
        /// <summary>
        /// 是否走读
        /// </summary>
        public bool? Sdextern { get; set; }

        /// <summary>
        /// 寄宿类型
        /// </summary>
        public LodgingEnum LodgingType { get; set; }
        public string LodgingTypeName { get; set; }

        /// <summary>
        /// 是否国际学校
        /// </summary>
        public bool International => Type == (int)SchoolType.International;

        /// <summary>
        /// 分部是否有效
        /// </summary>
        public bool ExtValid { get; set; }

        /// <summary>
        /// 学校是否有效
        /// </summary>
        public bool SchoolValid { get; set; }

        /// <summary>
        /// 学校状态
        /// </summary>
        public SchoolStatus Status { get; set; }
        /// <summary>
        /// 学校自编代号
        /// </summary>
        public int SchoolNo { get; set; }

        public string ShortId
        {
            get
            {
                return UrlShortIdUtil.Long2Base32(SchoolNo).ToLower();
            }
        }
    }
}
