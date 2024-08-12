using ProductManagement.Framework.RabbitMQ;
using ProductManagement.Framework.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.RabbitMQ.Message
{
    [MessageAlias("SyncLectureAddMessage_QUEUE")]
    public class SyncLectureAddMessage : IMessage
    {
        /// <summary>
        /// 数据Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 开课时间
        /// </summary>
        public DateTime StartTime { get; set; }
    }
}
