using ProductManagement.Framework.RabbitMQ;
using ProductManagement.Framework.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.RabbitMQ.Message
{

    /// <summary>
    /// 支付中心结算通知
    /// </summary>
    [MessageAlias("SyncWallet_QUEUE")]
    public class SyncWalletMessage : IMessage
    {
        public Guid userId { get; set; }
        public decimal virtualAmount { get; set; }

        public decimal amount { get; set; }
        public int statementType { get; set; }
        public int io { get; set; }
        public Guid orderId { get; set; }
        public int orderType { get; set; }
        public string remark { get; set; }
    }
}
