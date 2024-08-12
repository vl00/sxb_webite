using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PMS.School.Domain.Common;

namespace Sxb.PCWeb.ViewModels.School
{
    /// <summary>
    /// 学校点评卡片
    /// </summary>
    public class SchoolCommentCardViewModel
    {
        /// <summary>
        /// 学校Id
        /// </summary>
        public Guid Sid { get; set; }
        /// <summary>
        /// 学校分部Id
        /// </summary>
        public Guid ExtId { get; set; }
        /// <summary>
        /// 学校名称
        /// </summary>
        public string SchoolName { get; set; }
        /// <summary>
        /// 点评条数
        /// </summary>
        public int CommentTotal { get; set; }
        /// <summary>
        /// 星
        /// </summary>
        public int Stars { get; set; }
        /// <summary>
        /// 学校类型
        /// </summary>
        public SchoolType SchoolType { get; set; }
        /// <summary>
        /// 寄宿类型
        /// </summary>
        public int LodgingType { get; set; }
        /// <summary>
        /// 寄宿类型描述
        /// </summary>
        public string LodgingReason { get; set; }
        /// <summary>
        /// 是否上学帮认证
        /// </summary>
        public bool IsAuth { get; set; }

        public bool International { get; set; }
        /// <summary>
        /// 学校代号
        /// </summary>
        public string ShortSchoolNo { get; set; }
    }
}
