using PMS.School.Domain.Common;
using ProductManagement.Framework.Foundation;
using System;

namespace PMS.School.Application.ModelDto
{
    public class SchoolDto
    {
        /// <summary>
        /// 学校分部id，每个学校必定对应一间分部
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 学校id
        /// </summary>
        public Guid SchoolId { get; set; }
        /// <summary>
        /// 学校名称
        /// </summary>
        public string SchoolName { get; set; }
        /// <summary>
        /// 学校年级
        /// </summary>
        public SchoolGrade SchoolGrade { get; set; }
        /// <summary>
        /// 学校类型
        /// </summary>
        public SchoolType SchoolType { get; set; }
        /// <summary>
        /// 寄宿类型
        /// </summary>
        public LodgingEnum LodgingType { get; set; }


        /// <summary>
        /// 学费
        /// </summary>
        public decimal Tuition { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// 城市code
        /// </summary>
        public int City { get; set; }
        /// <summary>
        /// 是否上学帮认证
        /// </summary>
        public bool IsAuth { get; set; }

        public string FullSchoolName => SchoolName + "-" + SchoolType.Description() + SchoolGrade.Description();

        public int SchoolNo { get; set; }
        public string ShortSchoolNo => UrlShortIdUtil.Long2Base32(SchoolNo).ToLower();
    }
}
