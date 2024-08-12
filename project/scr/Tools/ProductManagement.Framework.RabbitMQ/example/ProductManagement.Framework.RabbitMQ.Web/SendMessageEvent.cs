using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using ProductManagement.Framework.RabbitMQ;
using ProductManagement.Framework.RabbitMQ.EventBus;

namespace ProductManagement.Framework.RabbitMQ.Web
{
    [MessageAlias("ProductManagement.Framework.RabbitMQ.Publisher.SendMessageEvent")]
    public class SendMessageEvent : IMessage
    {
        public string Message { get; set; }
    }
}
