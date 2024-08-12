using System;
using System.Collections.Generic;
using PMS.CommentsManage.Domain.Common;

namespace PMS.CommentsManage.Application.ModelDto
{
    public class AnswerDto
    {
        public Guid QuestionId { get; set; }
        public string QuestionContent { get; set; }

        public Guid? AnswerId { get; set; }
        public string AnswerContent { get; set; }
        public Guid? AnswerUserId { get; set; }

        public Guid? ReplyId { get; set; }
        public string ReplyContent { get; set; }
        public Guid? ReplyUserId { get; set; }

        public ExamineStatus State { get; set; }

        public bool IsTop { get; set; }

        /// <summary>
        /// 学校Id
        /// </summary>
        public Guid SchoolId { get; set; }

        /// <summary>
        /// 学校学部ID
        /// </summary>
        public Guid SchoolSectionId { get; set; }

        public string SchoolName { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
