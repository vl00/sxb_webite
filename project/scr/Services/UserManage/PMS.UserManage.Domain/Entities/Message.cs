using PMS.UserManage.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Domain.Entities
{
    public class Message
    {
        public Message()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public Guid userID { get; set; }
        public Guid senderID { get; set; }
        public byte Type { get; set; }
        public string title { get; set; }
        public string Content { get; set; }
        public string Data { get; set; }
        public Guid DataID { get; set; }
        public byte DataType { get; set; }
        public Guid? EID { get; set; }
        public DateTime time { get; set; }
        public bool? push { get; set; }
        public bool IsAnony { get; set; }
        public bool? Read { get; set; }
        public DateTime? ReadChangeTime { get; set; }
    }

    /// <summary>
    /// 消息 提示统计
    /// </summary>
    public class MessageTipsTotal 
    {
        public Tips Type { get; set; }
        public int Total { get; set; }
    }


}
