using PMS.PaidQA.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.PaidQA.Domain.Message
{
    public class SystemMessage:PaidQAMessage
    {
        public string Content { get; set; }

        protected override MsgMediaType MediaType => MsgMediaType.System;

    }
}
