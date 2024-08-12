using System;
using System.Linq.Expressions;
using iSchool;
using PMS.School.Domain.Common;
using PMS.School.Domain.Entities;
using ProductManagement.Framework.Foundation;

namespace PMS.School.Domain.Dtos
{
    /// <summary>
    /// 热门(访问) dto
    /// </summary>
    public class SimpleHotSchoolDto
    {
        public Guid Eid { get; set; }
        public Guid Sid { get; set; }
        /// <summary>
        /// 学校名-学部名
        /// </summary>
        public string Name { get; set; }
        public SchFType0 SchFType { get; set; }
        public SchoolType SchoolType { get; set; }

        public string SchoolTypeName => SchoolType.GetDescription();

        /// <summary>
        /// 是否国际学校
        /// </summary>
        public bool IsInternactioner => SchoolType == SchoolType.International;
        /// <summary>
        /// 学校总评分数
        /// </summary>
        public double? TotalScore { get; set; }
        public string Str_Score => TotalScore.HasValue ? Codstring.GetScoreString(TotalScore.Value) : "暂未评分";
        /// <summary>
        /// 是否寄宿
        /// </summary>
        public bool? Lodging { get; set; }

        /// <summary>
        /// 寄宿类型
        /// </summary>
        public LodgingEnum LodgingType => LodgingUtil.Reason(Lodging, Sdextern);
        public string LodgingTypeName => LodgingType.GetDescription();
        /// <summary>
        /// 是否走读
        /// </summary>
        public bool? Sdextern { get; set; }
        public bool? IsAuthedByOpen { get; set; }

        public int Usercount { get; set; }
        public int SchoolNo { private get; set; }
        public string ShortSchoolNo
        {
            get
            {
                return UrlShortIdUtil.Long2Base32(SchoolNo)?.ToLower();
            }
        }
    }

    /// <summary>
    /// 周边学校 dto
    /// </summary>
    public class SmpNearestSchoolDto
    {
        public Guid Eid { get; set; }
        public Guid Sid { get; set; }
        /// <summary>
        /// 学校名-学部名
        /// </summary>
        public string Name { get; set; }
        public SchFType0 SchFType { get; set; }
        /// <summary>
        /// 学校总评分数
        /// </summary>
        public double? TotalScore { get; set; }
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
        public LodgingEnum LodgingType => LodgingUtil.Reason(Lodging, Sdextern);
        public bool? IsAuthedByOpen { get; set; }

        public int _order { get; set; }
        public double Distance { get; set; }
        public int SchoolNo { get; set; }
    }
}
