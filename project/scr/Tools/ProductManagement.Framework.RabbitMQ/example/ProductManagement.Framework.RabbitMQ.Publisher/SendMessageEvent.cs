using System;
using System.Collections.Generic;
using System.Text;
using ProductManagement.Framework.RabbitMQ;
using ProductManagement.Framework.RabbitMQ.EventBus;

namespace ProductManagement.Framework.RabbitMQ.Publisher
{
    [MessageAlias("alias.test")]
    public class SendMessageEvent : IMessage
    {
        public string Message { get; set; }
    }
}
