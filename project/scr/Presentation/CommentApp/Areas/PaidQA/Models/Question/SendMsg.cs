using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.Enums;
using PMS.PaidQA.Domain.Message;
using ProductManagement.Framework.AspNetCoreHelper.RequestModel;
using Sxb.Web.Areas.PaidQA.Models.Order;
using Sxb.Web.RequestModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.PaidQA.Models.Question
{
    public class SendMsgRequest : WebContentRequest
    {
        [Description("订单ID")]
        public Guid OrderID { get; set; }

        [Description("消息体")]
        public string Content { get; set; }

        [Description("媒体类型")]
        public MsgMediaType MediaType { get; set; }

        public override string GetContent()
        {
            if (MediaType == MsgMediaType.SchoolCard) return string.Empty;
            if (MediaType == MsgMediaType.ArticleCard) return string.Empty;
            if (MediaType == MsgMediaType.SchoolRankCard) return string.Empty;
            if (MediaType == MsgMediaType.LiveCard) return string.Empty;
            if (MediaType == MsgMediaType.OrgCard) return string.Empty;
            if (MediaType == MsgMediaType.OrgEvaluationCard) return string.Empty;
            if (MediaType == MsgMediaType.CourseCard) return string.Empty;
            return PaidQAMessage.GetContent(new Message()
            {
                Content = Content,
                MediaType = MediaType
            });
        }
    }

    public class SendMsgResult
    {

        public MessageResult Message { get; set; }

        public OrderInfoResult OrderInfo { get; set; }


    }


}
