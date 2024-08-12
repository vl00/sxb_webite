using PMS.PaidQA.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.PaidQA.Domain.Message
{
    public class LiveCardMessage : PaidQAMessage
    {
        protected override MsgMediaType MediaType => MsgMediaType.LiveCard;
        public string Id { get; set; }
     

    }
}
