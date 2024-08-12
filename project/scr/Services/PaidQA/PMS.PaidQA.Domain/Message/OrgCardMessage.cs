using PMS.PaidQA.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.PaidQA.Domain.Message
{
    public class OrgCardMessage : PaidQAMessage
    {
        protected override MsgMediaType MediaType => MsgMediaType.OrgCard;
        public string Id { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        public string SubDesc { get; set; }
        public int CourseNum { get; set; }
        public int EvaNum { get; set; }
        public string Image { get; set; }



    }
}
