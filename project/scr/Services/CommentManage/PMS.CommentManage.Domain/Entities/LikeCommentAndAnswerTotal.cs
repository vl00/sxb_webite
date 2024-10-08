﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.Entities
{
    public class LikeCommentAndAnswerTotal
    {
        /// <summary>
        /// 点赞的点评总数
        /// </summary>
        public int LikeCommentTotal { get; set; }
        /// <summary>
        /// 点赞的点评回复总数
        /// </summary>
        public int LikeCommentReplyTotal { get; set; }
        /// <summary>
        /// 点赞的点回复的回复总数
        /// </summary>
        public int LikeReplyTotal { get; set; }
        /// <summary>
        /// 点赞的回答总数
        /// </summary>
        public int LikeAnswerTotal { get; set; }
        /// <summary>
        /// 点赞的回答的回复总数
        /// </summary>
        public int LikeAnswerReplyTotal { get; set; }

        /// <summary>
        /// 发布的点评、问题
        /// </summary>
        public int PublishTotal { get; set; }
        /// <summary>
        /// 所有的回答、回复统计
        /// </summary>
        public int AnswerAndReplyTotal { get; set; }
        /// <summary>
        /// 点赞汇总
        /// </summary>
        public int LikeTotal { get; set; }
    }
}
