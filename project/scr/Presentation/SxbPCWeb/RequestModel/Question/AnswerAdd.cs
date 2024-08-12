using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.RequestModel.Question
{
    public class AnswerAdd
    {
        /// <summary>
        /// 问答Id
        /// </summary>
        public Guid QuestionInfoId { get; set; }
        /// <summary>
        /// 父级Id
        /// </summary>
        public Guid? ParentId { get; set; }
        /// <summary>
        /// 问答写入者
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 是否学校发布
        /// </summary>
        public bool IsSchoolPublish { get; set; }

        /// <summary>
        /// 是否就读
        /// </summary>
        public bool IsAttend { get; set; }

        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }

        /// <summary>
        /// 问答内容
        /// </summary>
        public string Content { get; set; }
    }
}
