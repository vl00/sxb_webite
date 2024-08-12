using PMS.PaidQA.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.PaidQA.Domain.Message
{
    public class CourseCardMessage : PaidQAMessage
    {
        protected override MsgMediaType MediaType => MsgMediaType.CourseCard;

        public string Id { get; set; }
        public string  Title { get; set; }

        public decimal Price { get; set; }
        public string Image { get; set; }
    }
}
