using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProductManagement.Framework.RabbitMQ.Web
{
    public class MessageEventHandler : IEventHandler<SendMessageEvent>
    {
        public Task Handle(SendMessageEvent message)
        {
            return Task.Run(() =>
            {
                Console.WriteLine("等1秒");
                Thread.Sleep(1000);
                Console.WriteLine($"Comsumer {message.Message}");
            });
        }
    }
}
