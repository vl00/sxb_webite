﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Api.ViewDto.QuestionVo
{
    /// <summary>
    /// 问答实体
    /// </summary>
    public class AnswerModel
    {
        /// <summary>
        /// 问答id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 问答详情
        /// </summary>
        public string AnswerContent { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }
    }
}
