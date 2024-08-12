using PMS.PaidQA.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.PaidQA.Domain.Message
{
    public class RandomTenlentCardMessage:PaidQAMessage
    {
        public string Content { get; set; }

        public List<Guid> TalentUserIDs { get; set; }

        protected override MsgMediaType MediaType => MsgMediaType.RandomTenlentCard;
    }
}
