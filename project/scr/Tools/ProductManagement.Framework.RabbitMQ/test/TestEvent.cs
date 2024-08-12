using System;
using System.Collections.Generic;
using System.Text;
using ProductManagement.Framework.RabbitMQ.EventBus;

namespace ProductManagement.Framework.RabbitMQ.Test
{
    public class TestEvent : IMessage
    {
        public string Message { get; set; }
    }
}
