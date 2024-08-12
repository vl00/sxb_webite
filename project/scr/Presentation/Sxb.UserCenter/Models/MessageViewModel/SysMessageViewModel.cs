using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.UserCenter.Models.MessageViewModel
{
    /// <summary>
    /// 系统消息
    /// </summary>
    public class SysMessageViewModel
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 用户头像
        /// </summary>
        public string HeadImager { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 消息发送时间
        /// </summary>
        public string SenderTime { get; set; }
        /// <summary>
        /// 最新一条消息内容
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 待读消息总条数
        /// </summary>
        public int TipsTotal { get; set; }
    }
}
