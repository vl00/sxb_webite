using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Framework.RabbitMQ
{
    public delegate IEnumerable<object> MultiInstanceFactory(Type serviceType);
}
