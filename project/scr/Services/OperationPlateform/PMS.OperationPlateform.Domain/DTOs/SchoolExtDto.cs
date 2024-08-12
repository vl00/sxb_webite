using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;


namespace PMS.OperationPlateform.Domain.DTOs
{
    /// <summary>
    /// 学校分部
    /// </summary>
    public class SchoolExtDto
    {

        /// <summary>
        /// 学校分部id
        /// </summary>
        public Guid BranchId { get; set; }
        /// <summary>
        /// 学校id
        /// </summary>
        public Guid SchoolId { get; set; }

        /// <summary>
        /// 学校名称
        /// </summary>
        public string SchoolName { get; set; }

        /// <summary>
        /// 是否上学帮认证
        /// </summary>
        public bool Auth { get; set; }

        /// <summary>
        /// 寄宿类型
        /// </summary>
        public int LodgingType { get; set; }
        /// <summary>
        /// 寄宿类型描述
        /// </summary>
        public string LodgingReason { get; set; }

        /// <summary>
        /// 是否国际学校
        /// </summary>
        public bool International { get; set; }

        /// <summary>
        /// 费用
        /// </summary>
        public double? Cost { get; set; }

        /// <summary>
        /// 距离
        /// </summary>
        public double? Distance { get; set; }

        /// <summary>
        /// 城市名称
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// 城区名称
        /// </summary>
        public string AreaName { get; set; }


        public List<string> Tags { get; set; }

        /// <summary>
        /// 口碑评级分
        /// </summary>
        public double? Score { get; set; }
        /// <summary>
        /// 星星
        /// </summary>
        public int Stars { get; set; }
        /// <summary>
        /// 学校点评总条数
        /// </summary>
        public int CommentTotal { get; set; }

        public int Type { get; set; }

        public int SchoolNo { get; set; }
        public string ShortSchoolNo => UrlShortIdUtil.Long2Base32(SchoolNo).ToLower();
    }

}
