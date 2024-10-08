﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.Entities
{
    public class QuestionAnswerAndReply
    {
        public Guid Id { get; set; }
        public Guid QuestionInfoId { get; set; }
        public Guid ParentId { get; set; }
        public Guid UserId { get; set; }
        public bool IsAnony { get; set; }
        public string Content { get; set; }
        public int LikeCount { get; set; }
        public int ReplyCount { get; set; }
        public DateTime CreateTime { get; set; }
        public int Type { get; set; }
    }
}
