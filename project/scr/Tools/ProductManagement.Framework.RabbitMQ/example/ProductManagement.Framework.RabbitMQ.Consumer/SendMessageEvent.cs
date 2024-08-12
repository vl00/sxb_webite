using System;
using System.Collections.Generic;
using System.Text;
using ProductManagement.Framework.RabbitMQ;
using ProductManagement.Framework.RabbitMQ.EventBus;

namespace ProductManagement.Framework.RabbitMQ.Publisher
{
    public class SendMessageEvent : IMessage
    {
        public string Message { get; set; }
    }
}
