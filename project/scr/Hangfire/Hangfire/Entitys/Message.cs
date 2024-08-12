using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hangfire.ConsoleWeb.Entitys
{
    public class Message
    {
        /// <summary>
        /// 消息id
        /// </summary>
        public Guid id { get; set; }
        /// <summary>
        /// 受邀人id
        /// </summary>
        public Guid userId { get; set; }
        /// <summary>
        /// 发送人
        /// </summary>
        public Guid senderId { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 消息标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 详情id
        /// </summary>
        public Guid dataID { get; set; }
        /// <summary>
        /// 详情id类型
        /// </summary>
        public int dataType { get; set; }
        /// <summary>
        /// 学校分部id
        /// </summary>
        public Guid eID { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime time { get; set; }
        /// <summary>
        /// 推送时间
        /// </summary>
        public bool push { get; set; }
    }
}
