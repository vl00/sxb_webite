using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.UserCenter.Models.SchoolViewModel
{
    /// <summary>
    /// 问答学校卡片
    /// </summary>
    public class SchoolQuestionCardViewModel
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
        /// 提问总数
        /// </summary>
        public int QuestionTotal { get; set; }
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
        /// 学校类型
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 是否国际学校
        /// </summary>
        public bool International { get; set; }
    }
}
