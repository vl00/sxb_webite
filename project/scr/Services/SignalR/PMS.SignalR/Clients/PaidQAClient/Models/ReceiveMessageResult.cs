using ProductManagement.Framework.AspNetCoreHelper.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PMS.SignalR.Clients.PaidQAClient.Models
{
    public class ReceiveMessageResult
    {
        ///// <summary>
        ///// 返回时间
        ///// </summary>
        //[JsonConverter(typeof(HmDateTimeConverter))]
        public DateTime ServerTime { get; set; } = DateTime.Now;

        public List<MessageResult> Messages { get; set; }

    }
}
