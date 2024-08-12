using System;
using Dapper.Contrib.Extensions;
using PMS.PaidQA.Domain.Enums;

namespace PMS.PaidQA.Domain.Entities
{
    [Serializable]
    [Table("Message")]
    public partial class Message
    {
        /// <summary> 
        /// </summary> 
        [ExplicitKey]
        public Guid ID { get; set; }

        /// <summary> 
        /// </summary> 
        public Guid? OrderID { get; set; }

        /// <summary> 
        /// 发送者ID 
        /// </summary> 
        public Guid? SenderID { get; set; }

        /// <summary> 
        /// </summary> 
        public string Content { get; set; }

        /// <summary> 
        /// 1文字 2图片 3语音 4推荐卡片 
        /// </summary> 
        public MsgMediaType MediaType { get; set; }

        /// <summary> 
        /// 1系统消息 2客服消息 3提问 4回答 
        /// </summary> 
        public MsgType MsgType { get; set; }

        /// <summary> 
        /// 接收者ID 
        /// </summary> 
        public Guid? ReceiveID { get; set; }

        /// <summary> 
        /// 发送时间 
        /// </summary> 
        public DateTime? CreateTime { get; set; }

        /// <summary> 
        /// </summary> 
        public bool IsValid { get; set; } = true;

        /// <summary> 
        /// 阅读时间 
        /// </summary> 
        public DateTime? ReadTime { get; set; }


    }
}