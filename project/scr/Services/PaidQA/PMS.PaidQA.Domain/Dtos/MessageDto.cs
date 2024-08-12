using PMS.PaidQA.Domain.Enums;
using PMS.PaidQA.Domain.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.PaidQA.Domain.Dtos
{
    public class MessageDto 
    {
        [Description("消息ID")]
        public Guid ID { get; set; }

        [Description("发送者ID")]
        public Guid? SenderID { get; set; }

        [Description("接收者ID")]
        public Guid? ReceiveID { get; set; }

        [Description("内容")]
        public string Content { get; set; }

        [Description("媒体类型：1文字 2图片 3语音 4推荐卡片 5图文")]
        public MsgMediaType MediaType { get; set; }

        [Description("消息类型：1系统消息 2客服消息 3提问 4回答 ")]
        public MsgType MsgType { get; set; }

        [Description("发送时间")]
        public DateTime? CreateTime { get; set; }

        [Description("阅读时间")]
        public DateTime? ReadTime { get; set; }

        [Description("是否是自己的消息")]
        public bool IsBelongSelf { get; set; }

        [Description("发送者信息")]
        public UserInfoDto SenderInfo { get; set; }

    }



}
