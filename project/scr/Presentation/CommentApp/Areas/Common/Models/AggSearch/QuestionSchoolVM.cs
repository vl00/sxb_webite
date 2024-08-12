using NPOIHelper.Enums;
using PMS.School.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Common.Models
{
    public class QuestionSchoolVM
    {
        /// <summary>
        /// 短链
        /// </summary>
        public string ShortNo { get; set; }

        /// <summary>
        /// 学校名-学部名
        /// </summary>
        public string SchoolName { get; set; }

        /// <summary>
        /// 分部所在学校的问题数
        /// </summary>
        public int QuestionCount { get; set; }

        /// <summary>
        /// 学校类型
        /// </summary>
        public SchoolType SchoolType { get; set; }

        /// <summary>
        /// 学校类型
        /// </summary>
        public string SchoolTypeName => SchoolType.GetDescription();

        /// <summary>
        /// 寄宿类型
        /// </summary>
        public LodgingEnum LodgingType { get; set; }

        /// <summary>
        /// 寄宿类型
        /// </summary>
        public string LodgingTypeName => LodgingType.GetDescription();
    }
}
