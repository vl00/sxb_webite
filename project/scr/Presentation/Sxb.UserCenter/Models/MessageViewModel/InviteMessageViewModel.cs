using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sxb.UserCenter.Models.SchoolViewModel;
using Sxb.UserCenter.Models.UserInfoViewModel;

namespace Sxb.UserCenter.Models.MessageViewModel
{
    public class InviteMessageViewModel
    {
        /// <summary>
        /// 消息id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 数据id
        /// </summary>
        public Guid DataId { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public int DataType { get; set; }
        /// <summary>
        /// 邀请类型
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 邀请类型
        /// </summary>
        public string InviteTitle { get; set; }
        /// <summary>
        /// 邀请内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 邀请时间
        /// </summary>
        public string InviteTime { get; set; }
        /// <summary>
        /// 邀请者信息
        /// </summary>
        public UserViewModel InviteUser { get; set; }

        /// <summary>
        /// 学校
        /// </summary>
        public SchoolInfoViewModel School { get; set; }
    }

    /// <summary>
    /// 关注的达人正在直播
    /// </summary>
    public class FollowLive 
    {
        /// <summary>
        /// 消息id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 直播id
        /// </summary>
        public Guid DataId { get; set; }
        /// <summary>
        /// 微课title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 收听人数
        /// </summary>
        public int ListenTotal { get; set; }
        /// <summary>
        /// 直播者
        /// </summary>
        public UserViewModel LiveUser { get; set; }
    }
}
