using PMS.PaidQA.Domain.Entities;
using Sxb.Web.Areas.PaidQA.Models.Order;
using Sxb.Web.Areas.PaidQA.Models.Talent;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.PaidQA.Models.Evaluate
{
    public class GetEvaluatePreInfoRequest
    {
        [Description("订单ID")]
        public Guid OrderID { get; set; }
    }

    public class GetEvaluatePreInfoResult
    {
        [Description("订单信息")]
        public OrderInfoResult  OrderInfo { get; set; }

        [Description("专家信息")]
        public TalentInfoResult TalentInfo { get; set; }

        [Description("评价标签")]
        public List<EvaluateTags> EvaluateTags { get; set; }

    }
}
