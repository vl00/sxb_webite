using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.ModelDto
{
    public class HotQuestionSchoolDto
    {
        /// <summary>
        /// 学校id
        /// </summary>
        public Guid SchoolId { get; set; }
        /// <summary>
        /// 热问学校分部id
        /// </summary>
        public Guid SchoolSectionId { get; set; }
        /// <summary>
        /// 热问数量
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// 所在城市
        /// </summary>
        public int City { get; set; }
        /// <summary>
        /// 学校名称
        /// </summary>
        public string SchoolName { get; set; }
        /// <summary>
        /// 学校代号
        /// </summary>
        public int SchoolNo { get; set; }
    }
}
