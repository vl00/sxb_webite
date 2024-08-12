using System;
using System.Collections.Generic;
using PMS.CommentsManage.Domain.Common;

namespace PMS.CommentsManage.Application.ModelDto
{
    public class QuestionAnswerDto
    {
        public Guid AnswerId { get; set; }
        public string AnswerContent { get; set; }
        public Guid AnswerUserId { get; set; }
        public string AnswerUserName { get; set; }

        public Guid? ParentId { get; set; }

        public string Phone { get; set; }

        public ExamineStatus State { get; set; }

        public bool IsTop { get; set; }

        public DateTime CreateTime { get; set; }

        public List<QuestionAnswerDto> AnswerReply { get; set; }
    }
}
