using iSchool;
using PMS.School.Domain.Common;
using ProductManagement.Framework.Foundation;
using System;

namespace PMS.School.Domain.Dtos
{
    public class SchExtDto0
    {
        public Guid Eid { get; set; }
        public Guid Sid { get; set; }
        public string SchName { get; set; }
        public string ExtName { get; set; }
        public string SchFtype { get; set; }
        public SchFType0 SchFType0 { get; set; }
        public byte Grade { get; set; }
        public byte Type { get; set; }
        public SchoolType SchoolType => (SchoolType)Type;
        public string SchoolTypeName => SchoolType.GetDescription();

        /// <summary>
        /// 是否国际学校
        /// </summary>
        public bool IsInternactioner => SchoolType == SchoolType.International;

        public bool? Discount { get; set; }
        public bool? Diglossia { get; set; }
        public bool? Chinese { get; set; }

        public int Province { get; set; }
        public int City { get; set; }
        public int Area { get; set; }

        public byte Status { get; set; } = (byte)SchoolStatus.Success;
        public bool IsValid { get; set; } = true;
        /// <summary>
        /// 是否寄宿
        /// </summary>
        public bool? Lodging { get; set; }
        public bool IsLodging => Lodging == true;
        /// <summary>
        /// 是否走读
        /// </summary>
        public bool? Sdextern { get; set; }
        /// <summary>
        /// 寄宿类型
        /// </summary>
        public LodgingEnum LodgingType => LodgingUtil.Reason(Lodging, Sdextern);
        /// <summary>
        /// 寄宿类型
        /// </summary>
        public string LodgingTypeName => LodgingType.GetDescription();
        public int SchoolNo { get; set; }
        public double? TotalScore { get; set; }
        public string Str_Score => TotalScore.HasValue ? Codstring.GetScoreString(TotalScore.Value) : "暂未评分";

        public string ShortSchoolNo
        {
            get
            {
                return UrlShortIdUtil.Long2Base32(SchoolNo)?.ToLower();
            }
        }
    }
}
