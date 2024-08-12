using ProductManagement.Framework.RabbitMQ;
using ProductManagement.Framework.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.RabbitMQ.Message
{
    [MessageAlias("SyncTopicAddMessage_QUEUE")]
    public class SyncTopicAddMessage : IMessage
    {
        public Guid Id { get; set; }
    }
}
