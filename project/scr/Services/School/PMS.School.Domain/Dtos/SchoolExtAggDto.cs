using PMS.School.Domain.Common;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.School.Domain.Dtos
{
    public class SchoolExtAggDto
    {
        /// <summary>
        /// 学部Id
        /// </summary>
        public Guid ExtId { get; set; }
        /// <summary>
        /// 学校全称
        /// </summary>
        public string SchoolName { get; set; }

        /// <summary>
        /// 学校类型
        /// </summary>
        public SchoolType SchoolType { get; set; }

        /// <summary>
        /// 短链
        /// </summary>
        public long SchoolNo { get; set; }
        public string ShortSchoolNo
        {
            get
            {
                return UrlShortIdUtil.Long2Base32(SchoolNo)?.ToLower();
            }
        }

        /// <summary>
        /// 学校评分
        /// SchoolScoreToStart.GetCurrentSchoolstart(Score)
        /// </summary>
        public int Score { get; set; }
        

        public bool IsInternactioner => SchoolType == SchoolType.ForeignNationality;

        /// <summary>
        /// 是否寄宿
        /// </summary>
        public bool? Lodging { get; set; }
        public bool IsLodging => Lodging == true;

        /// <summary>
        /// 是否走读
        /// </summary>
        public bool? Sdextern { get; set; }
        public bool IsSdextern => Sdextern == true;
        /// <summary>
        /// 寄宿类型
        /// </summary>
        public LodgingEnum LodgingType => LodgingUtil.Reason(Lodging, Sdextern);
        public string LodgingTypeName => LodgingType.GetDescription();
    }
}
