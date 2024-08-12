using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.UserCenter.Models.QuestionViewModel
{
    public class AnswerWriterViewModel
    {
        /// <summary>
        /// 问题id
        /// </summary>
        public Guid QuestionId { get; set; }
        /// <summary>
        /// 是否入读
        /// </summary>
        public bool IsAttend { get; set; }
        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnony { get; set; }
        /// <summary>
        /// 回答内容
        /// </summary>
        public string Content { get; set; }
    }
}
