using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ProductManagement.Framework.RabbitMQ.EventBus;

namespace ProductManagement.Framework.RabbitMQ
{
    public interface IEventHandler
    {
        
    }
    public interface IEventHandler<in TMessage> : IEventHandler where TMessage : IMessage
    {
        Task Handle(TMessage message);
    }
}
