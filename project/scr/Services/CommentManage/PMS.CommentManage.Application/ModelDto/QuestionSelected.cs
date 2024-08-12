using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.ModelDto
{
    public class QuestionSelected
    {
        public Guid Id { get; set; }
        public string QuestionContent { get; set; }
        public int LikeTotal { get; set; }
        public int AnswerTotal { get; set; }
        public bool isSelected { get; set; }
    }
}
