using PMS.OperationPlateform.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.ViewModels.School
{
    public class SchoolCollectionVo
    {
        public Guid Sid { get; set; }
        public Guid ExtId { get; set; }
        public byte Grade { get; set; }
        public byte Type { get; set; }
        public string SchoolName { get; set; }
        public string ExtName { get; set; }
        public double? Tuition { get; set; }
        /// <summary>
        /// 是否普惠
        /// </summary>
        public bool? Discount { get; set; }
        /// <summary>
        /// 是否双语
        /// </summary>
        public bool? Diglossia { get; set; }
        /// <summary>
        /// 是否是中国人学校
        /// </summary>
        public bool? Chinese { get; set; }
        public List<H5SchoolRankInfoDto> Ranks { get; set; }
        /// <summary>
        /// 评分
        /// </summary>
        public double? Score { get; set; }
        /// <summary>
        /// 寄宿类型
        /// </summary>
        public int LodgingType { get; set; }
        /// <summary>
        /// 寄宿类型描述
        /// </summary>
        public string LodgingReason { get; set; }
        public List<string> Tags { get; set; }
        public string CityName { get; set; }
        public string AreaName { get; set; }
        public string TypeName { get; set; }
        public int? City { get; set; }
        public int? Area { get; set; }
        public int? Province { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }

        /// <summary>
        /// 评论数
        /// </summary>
        /// <value>The comment count.</value>
        public int CommentCount { get; set; }

        /// <summary>
        /// 提问数
        /// </summary>
        public int QuestionCount { get; set; }

        /// <summary>
        /// 学校总平均分
        /// </summary>
        public decimal SchoolAvgScore { get; set; }

        /// <summary>
        /// 前端展示星星
        /// </summary>
        public int SchoolStars { get; set; }
    }
}
