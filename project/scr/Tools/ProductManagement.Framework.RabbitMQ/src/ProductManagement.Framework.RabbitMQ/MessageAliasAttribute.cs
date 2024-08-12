using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Framework.RabbitMQ
{
    public class MessageAliasAttribute : Attribute
    {
        public MessageAliasAttribute(string @alias)
        {
            Alias = alias;
        }

        public string Alias { get; }
    }
}
