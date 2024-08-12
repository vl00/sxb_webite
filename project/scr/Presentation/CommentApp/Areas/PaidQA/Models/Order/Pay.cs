using Microsoft.AspNetCore.Http;
using Sxb.Web.RequestModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.PaidQA.Models.Order
{
    public class PayCallBack
    {
        [Description("订单ID")]
        public Guid orderid { get; set; }
        [Description("支付状态")]
        public int paystatus { get; set; }

    }
    public class PayRequest: SafeActionRequest
    {

        [Description("专家ID")]
        public Guid TalentID { get; set; }

        [Description("优惠券ID")]
        public Guid? CouponTakeID { get; set; }

        [Description("当前浏览者的OpenID")]
        public string OpenId { get; set; }

        [Description("渠道标识")]
        public string Fw { get; set; }

        [Description("客户端支付类型")]
        public int? ClientType { get; set; }

        public override string LockKey(HttpContext context)
        {
            return $"PaidQAPay:{context.GetUserInfo()?.UserId}:{TalentID}";
        }

        public override string LockValue(HttpContext context)
        {
            return $"PaidQAPay:{context.GetUserInfo()?.UserId}:{TalentID}";
        }
    }

    public class PayResult {
        public bool  NotNeedPay { get; set; }
        public string AppId { get; set; }
        public string TimeStamp { get; set; }
        public string NonceStr { get; set; }
        public string SignType { get; set; }
        public string PaySign { get; set; }
        public string Package { get; set; }

        public OrderInfoResult OrderInfo { get; set; }
    }
}
