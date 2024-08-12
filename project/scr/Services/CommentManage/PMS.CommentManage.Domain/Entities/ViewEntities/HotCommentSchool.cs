using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.Entities.ViewEntities
{
    /// <summary>
    /// 热评学校
    /// </summary>
    public class HotCommentSchool
    {
        /// <summary>
        /// 热评学校分部id
        /// </summary>
        public Guid SchoolSectionId { get; set; }
        /// <summary>
        /// 学校id
        /// </summary>
        public Guid SchoolId { get; set; }
        /// <summary>
        /// 热评数量
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// 学校名称
        /// </summary>
        public string SchoolName { get; set; }
    }
}
