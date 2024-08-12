using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.ModelDto
{
    public class HotCommentDto
    {
        /// <summary>
        /// 点评id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 学校分部id
        /// </summary>
        public Guid SchoolSectionId { get; set; }
        /// <summary>
        /// 学校id
        /// </summary>
        public Guid SchoolId { get; set; }
        /// <summary>
        /// 学校名称
        /// </summary>
        public string SchoolName { get; set; }
        /// <summary>
        /// 城市code
        /// </summary>
        public int City { get; set; }
        /// <summary>
        /// 该时间内容的热度
        /// </summary>
        public int Total { get; set; }
    }
}
