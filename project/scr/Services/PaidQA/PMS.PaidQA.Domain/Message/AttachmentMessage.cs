using PMS.PaidQA.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.PaidQA.Domain.Message
{
    public class AttachmentMessage: PaidQAMessage
    {
        public string Url { get; set; }

        public string Title { get; set; }

        public string Cover { get; set; }

        protected override MsgMediaType MediaType => MsgMediaType.Attachment;
    }
}
