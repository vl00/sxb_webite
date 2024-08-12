using PMS.PaidQA.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.PaidQA.Domain.Message
{
    public class TXTMessage : PaidQAMessage
    {
        public string Content { get; set; }

        public List<string> Images { get; set; }

        protected override MsgMediaType MediaType => MsgMediaType.TXT;
    }
}
