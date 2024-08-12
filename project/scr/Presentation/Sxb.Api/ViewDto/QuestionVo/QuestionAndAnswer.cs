using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Api.ViewDto.QuestionVo
{
    /// <summary>
    /// 问题与回答
    /// </summary>
    public class QuestionAndAnswer
    {
        /// <summary>
        /// 问题列表
        /// </summary>
        public List<QuestionModel> questionModels { get; set; }
        /// <summary>
        /// 回答列表
        /// </summary>
        public List<AnswerModel> answerModels { get; set; }
    }
}
