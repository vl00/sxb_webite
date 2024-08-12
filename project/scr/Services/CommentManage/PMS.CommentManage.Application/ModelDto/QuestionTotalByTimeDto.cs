using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.ModelDto
{
    public class QuestionTotalByTimeDto
    {
        public Guid School { get; set; }

        public Guid SchoolSectionId { get; set; }
        
        public int Total { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
