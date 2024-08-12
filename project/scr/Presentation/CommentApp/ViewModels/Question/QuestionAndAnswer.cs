using Sxb.Web.Models.Answer;
using Sxb.Web.Models.Question;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.ViewModels.Question
{
    /// <summary>
    /// 问题与回答
    /// </summary>
    public class QuestionAndAnswer
    {
        /// <summary>
        /// 问题列表
        /// </summary>
        public List<QuestionVo> questionModels { get; set; }
        /// <summary>
        /// 回答列表
        /// </summary>
        public List<AnswerInfoVo> answerModels { get; set; }
    }
}
