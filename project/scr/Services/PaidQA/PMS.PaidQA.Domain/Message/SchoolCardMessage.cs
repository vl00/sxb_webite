using PMS.PaidQA.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.PaidQA.Domain.Message
{
    public class SchoolCardMessage : PaidQAMessage
    {
        protected override MsgMediaType MediaType => MsgMediaType.SchoolCard;

        public string Id { get; set; }

    }
}
