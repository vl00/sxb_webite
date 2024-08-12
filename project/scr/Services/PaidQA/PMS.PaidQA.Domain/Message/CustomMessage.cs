using Newtonsoft.Json;
using PMS.PaidQA.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.PaidQA.Domain.Message
{
    public  class CustomMessage: PaidQAMessage
    {
        protected override MsgMediaType MediaType => MsgMediaType.Custom;

        public string Content { get; set; }

    }
}
