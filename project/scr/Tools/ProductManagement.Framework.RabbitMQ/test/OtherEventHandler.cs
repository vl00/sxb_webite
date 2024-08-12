using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.Framework.RabbitMQ.Test
{
    public class OtherEventHandler : IEventHandler
    {
        public Task Handle(object sender, string message)
        {
            return Task.Run(() =>
            {
                return sender.GetType().Name;

            });
        }
    }
}
