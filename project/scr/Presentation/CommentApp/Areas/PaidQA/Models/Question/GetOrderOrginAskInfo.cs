using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.PaidQA.Models.Question
{

    public class GetOrderOrginAskInfoRequest
    {
        public Guid OrderID { get; set; }

    }
    public class GetOrderOrginAskInfoResult
    {

        [Description("是否匿名")]
        public bool IsAnonymity { get; set; }

        [Description("是否公开")]
        public bool IsPublic { get; set; }

        public MessageResult MessageResult  { get; set; }
    }
}
