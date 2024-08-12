using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.School.Domain.Dtos
{
    /// <summary>
    /// 学校点评、提问卡片
    /// </summary>
    public class SchoolCQDto
    {
        /// <summary>
        /// 学校分部id
        /// </summary>
        public Guid SchoolSectionId { get; set; }
        /// <summary>
        /// 学校id
        /// </summary>
        public Guid SchoolId { get; set; }
        /// <summary>
        /// 学校总评分数
        /// </summary>
        public decimal AggScore { get; set; }
        /// <summary>
        /// 总点评数
        /// </summary>
        public int CommentCount { get; set; }
        /// <summary>
        /// 提问数
        /// </summary>
        public int QuestionCount { get; set; }
        /// <summary>
        /// 学校名称
        /// </summary>
        public string SchoolName { get; set; }
        /// <summary>
        /// 是否住校
        /// </summary>
        public bool Lodging { get; set; }

        /// <summary>
        /// 是否走读
        /// </summary>
        public bool Sdextern { get; set; }
        /// <summary>
        /// 学校类型
        /// </summary>
        public int Type { get; set; }
    }
}
