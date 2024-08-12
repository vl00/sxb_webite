using PMS.PaidQA.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.PaidQA.Domain.Message
{
    public class ImageMessage:PaidQAMessage
    {
        public List<string> Images { get; set; }

        protected override MsgMediaType MediaType => MsgMediaType.Image;
    }
}
