using System;
using System.Collections.Generic;

namespace PMS.CommentsManage.Application.Model.Query
{
    public class PageQuestionAnswerQuery
    {
        public int PageNo { get; set; }
        public int PageSize { get; set; }

        public DateTime StartTime { get; set; } = DateTime.MinValue;
        public DateTime EndTime { get; set; } = DateTime.MinValue;

        public List<Guid> SchoolIds { get; set; }
    }
}
