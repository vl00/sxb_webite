using PMS.PaidQA.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.PaidQA.Domain.Message
{
    public class SchoolRankCardMessage : PaidQAMessage
    {
        protected override MsgMediaType MediaType => MsgMediaType.SchoolRankCard;

        public string Id { get; set; }


    }
}
