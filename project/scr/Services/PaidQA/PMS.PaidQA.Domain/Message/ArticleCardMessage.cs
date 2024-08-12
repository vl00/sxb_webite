using PMS.PaidQA.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.PaidQA.Domain.Message
{
    public class ArticleCardMessage : PaidQAMessage
    {
        protected override MsgMediaType MediaType => MsgMediaType.ArticleCard;

        public  string Id { get; set; }



    }
}
