using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Web.Areas.PartTimeJob.Models
{
    public class QuestionInfoVo
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 问题Id
        /// </summary>
        public Guid QuestionExamineId { get; set; }
        /// <summary>
        /// 学校Id
        /// </summary>
        public Guid SchoolId { get; set; }
        /// <summary>
        /// 问题写入者
        /// </summary>
        public Guid PartTimeJobAdminId { get; set; }
        /// <summary>
        /// 问题内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateTime { get; set; }

        public QuestionExamineVo QuestionExamineVo { get; set; }
        public PartTimeJobAdminVo PartTimeJobAdminVo { get; set; }
    }
}
