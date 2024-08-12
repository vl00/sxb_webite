using PMS.PaidQA.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.PaidQA.Domain.Message
{
    public class OrgEvaluationCardMessage : PaidQAMessage
    {
        protected override MsgMediaType MediaType => MsgMediaType.OrgEvaluationCard;

        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }


    }
}
