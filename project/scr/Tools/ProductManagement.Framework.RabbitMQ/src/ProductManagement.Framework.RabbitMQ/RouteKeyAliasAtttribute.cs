using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Framework.RabbitMQ
{
    public class RouteKeyAtttribute : Attribute
    {
        public RouteKeyAtttribute(string key)
        {
            Key = key;
        }

        public string Key { get; }
    }
}
