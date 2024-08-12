using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Framework.RabbitMQ
{
    public interface IRabbitMQPersistentConnection : IDisposable
    {
        string ExtName();

        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}
