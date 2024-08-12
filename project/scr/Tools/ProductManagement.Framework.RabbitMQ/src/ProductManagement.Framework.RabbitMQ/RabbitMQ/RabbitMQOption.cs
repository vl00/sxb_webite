using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Framework.RabbitMQ
{
    public class RabbitMQOption
    {
        public string Uri { get; set; }

        public List<string> AmqpUris { get; set; }

        public string ExtName { get; set; }
    }
}
