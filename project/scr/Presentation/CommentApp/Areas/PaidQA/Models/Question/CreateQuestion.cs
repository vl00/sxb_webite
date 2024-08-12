using Newtonsoft.Json;
using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.Message;
using ProductManagement.Framework.AspNetCoreHelper.RequestModel;
using Sxb.Web.RequestModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.PaidQA.Models.Question
{

    public class CreateQuestionRequest : WebContentRequest
    {


        [Description("订单ID")]
        public Guid OrderID { get; set; }

        [Description("提问内容")]
        public List<Message> Messages { get; set; }


        [Description("是否匿名")]
        public bool IsAnonymity { get; set; }

        [Description("是否公开")]
        public bool IsPublic { get; set; }

        public override string GetContent()
        {
            if (Messages != null && Messages.Any())
            {
                var contents = Messages.Select(msg => { return PaidQAMessage.GetContent(msg); });
                return string.Join("|", contents);
            }
            else {
                return "";
            }
        }
    }

    public class CreateQuestionResult
    {
        [Description("订单ID")]
        public Guid OrderID { get; set; }

        [Description("消息ID")]
        public Guid MsgID { get; set; }

    }
}
