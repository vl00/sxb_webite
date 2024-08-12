using ProductManagement.Framework.RabbitMQ;
using ProductManagement.Framework.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.RabbitMQ.Message
{
    [MessageAlias("SyncTopicLikeMessage_QUEUE")]
    public class SyncTopicLikeMessage : IMessage
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
    }
}
