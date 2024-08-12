using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.PaidQA.Models.Question
{
    public class GetMessagesRequest
    {
        [Description("订单ID")]
        public Guid OrderID { get; set; }

        [Description("轮询次数，每秒/1次")]
        public int RepeatTime { get; set; } = 5;
    }

    public class GetMessagesResult
    {
        [Description("聊天记录")]
        public List<MessageResult> ChatRecords { get; set; } = new List<MessageResult>();
    }
}
