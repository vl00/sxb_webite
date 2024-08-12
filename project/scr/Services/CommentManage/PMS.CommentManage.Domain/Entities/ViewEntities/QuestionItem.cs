using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.Entities.ViewEntities
{
    public class QuestionItem
    {
        /// <summary>
        /// 问题Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 问题内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 学校名
        /// </summary>
        public string SchoolName { get; set; }
    }
}
