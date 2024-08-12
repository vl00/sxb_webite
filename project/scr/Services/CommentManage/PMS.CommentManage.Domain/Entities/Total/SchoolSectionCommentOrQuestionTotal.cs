using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.Entities.Total
{
    public class SchoolSectionCommentOrQuestionTotal
    {
        public Guid SchoolSectionId { get; set; }
        public Guid School { get; set; }
        public int Total { get; set; }
    }

    public class QuestionTotal : SchoolSectionCommentOrQuestionTotal
    {
        public DateTime CreateTime { get; set; }
    }
}
