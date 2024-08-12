using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;


namespace PMS.SignalR.Clients.PaidQAClient.Models
{
    public class EvaluateResult
    {

        [Description("ID")]
        public Guid ID { get; set; }

        [Description("内容")]
        public string Content { get; set; }

        [Description("订单ID")]
        public Guid OrderID { get; set; }

        [Description("分数")]
        public int Score { get; set; }

        //[JsonConverter(typeof(HmDateTimeConverter))]
        [Description("评价时间")]
        public DateTime? CreateTime { get; set; }

        [Description("是否系统自动评价")]
        public bool? IsAuto { get; set; }

        [Description("评价标签")]
        public List<string> Tags { get; set; }
    }
}
