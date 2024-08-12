using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ProductManagement.Framework.RabbitMQ.EventBus;

namespace ProductManagement.Framework.RabbitMQ
{
    public interface IEventBus
    {
        /// <summary>
        /// 发布一个事件
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        void Publish(IMessage message);
    }
}
