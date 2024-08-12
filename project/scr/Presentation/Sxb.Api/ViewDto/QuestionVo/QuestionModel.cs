﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Api.ViewDto.QuestionVo
{
    /// <summary>
    /// 问题实体
    /// </summary>
    public class QuestionModel
    {
        /// <summary>
        /// 问题id
        /// </summary>
        public Guid QuestionId { get; set; }
        /// <summary>
        /// 学校分部id
        /// </summary>
        public Guid SchoolSectionId { get; set; }
        /// <summary>
        /// 问题内容
        /// </summary>
        public string QuestionContent { get; set; }
        /// <summary>
        /// 问题回答总数
        /// </summary>
        public int CurrentQuestionAnswerTotal { get; set; }
        /// <summary>
        /// 分部下总数量
        /// </summary>
        public int SectionQuestionTotal { get; set; }
        /// <summary>
        /// 问题写入时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 回答列表
        /// </summary>
        public List<AnswerModel> AnswerModels { get; set; }
    }
}
