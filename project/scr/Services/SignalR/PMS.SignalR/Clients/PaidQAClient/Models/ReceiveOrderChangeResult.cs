using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.SignalR.Clients.PaidQAClient.Models
{
    public class ReceiveOrderChangeResult
    {
        public OrderInfoResult OrderInfo { get; set; }

        [Description("评价信息")]
        public EvaluateResult EvaluateInfo { get; set; }

    }
}
