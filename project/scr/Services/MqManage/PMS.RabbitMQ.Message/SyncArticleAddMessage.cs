using ProductManagement.Framework.RabbitMQ;
using ProductManagement.Framework.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.RabbitMQ.Message
{
    [MessageAlias("SyncArticleAddMessage_QUEUE")]
    public class SyncArticleAddMessage : IMessage
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
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 页面  No to Url
        /// </summary>
        public string Url { get; set; }
        public long No { get; set; }
    }
}
