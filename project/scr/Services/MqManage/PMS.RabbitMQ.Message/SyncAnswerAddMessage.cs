using ProductManagement.Framework.RabbitMQ;
using ProductManagement.Framework.RabbitMQ.EventBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.RabbitMQ.Message
{
    [MessageAlias("SyncAnswerAddMessage_QUEUE")]
    public class SyncAnswerAddMessage : IMessage
    {
        /// <summary>
        /// Answer Id
        /// </summary>
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        #region question

        /// <summary>
        /// 
        /// </summary>
        public Guid QuestionId { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid QuestionUserId { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string QuestionContent { get; set; }

        /// <summary>
        /// 页面
        /// </summary>
        public string QuestionUrl { get; set; }
        public long QuestionNo { get; set; }
        #endregion

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
