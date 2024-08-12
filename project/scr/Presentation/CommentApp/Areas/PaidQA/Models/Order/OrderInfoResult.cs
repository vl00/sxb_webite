using Newtonsoft.Json;
using Sxb.Web.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.PaidQA.Models.Order
{
    [Description("订单信息")]
    public class OrderInfoResult
    {
        [Description("订单ID")]
        public Guid ID { get; set; }

        [Description("订单编号")]
        public string NO { get; set; }

        [Description("订单状态：1待提问 2待回答 3进行中 4已结束 5已转单  6已超时 7已拒绝 ")]
        public int Status { get; set; }

        [Description("提问剩余次数 ")]
        public int AskRemainCount { get; set; }

        [Description("订单价格")]
        public decimal Amount { get; set; }

        [Description("原问题订单ID")]
        public Guid? OrginAskID { get; set; }

        [Description("创建者ID")]
        public Guid CreatorID { get; set; }

        [Description("被提问者ID")]
        public Guid AnswerID { get; set; }

        [JsonConverter(typeof(HmDateTimeConverter))]
        [Description("创建时间")]
        public DateTime CreateTime { get; set; }

        [JsonConverter(typeof(HmDateTimeConverter))]
        [Description("结束时间")]
        public DateTime? FinishTime { get; set; }

        [Description("是否匿名")]
        public bool? IsAnonymous { get; set; }

        [Description("匿名名称")]
        public string AnonyName { get; set; }

        [Description("是否评价")]
        public bool IsEvaluate { get; set; }



        [Description("是否已经有回复了")]
        public bool IsReply { get; set; }

        [Description("是否已拒绝转单")]
        public bool IsRefusTransiting { get; set; }


        [Description("订单结束剩余时间（毫秒单位）")]
        public long OverRemainTime { get; set; }


    }
}
