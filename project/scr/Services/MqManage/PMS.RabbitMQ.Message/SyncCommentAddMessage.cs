using ProductManagement.Framework.RabbitMQ;
using ProductManagement.Framework.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.RabbitMQ.Message
{
    [MessageAlias("SyncCommentAddMessage_QUEUE")]
    public class SyncCommentAddMessage :IMessage
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
        /// 页面  UrlShortIdUtil.Long2Base32(No)
        /// </summary>
        public string Url { get; set; }
        public long No { get; set; }

        /// <summary>
        /// 学部Id
        /// </summary>
        public Guid ExtId { get; set; }

        /// <summary>
        /// 学部名称
        /// </summary>
        public string SchoolName { get; set; }
    }
}
