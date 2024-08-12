using PMS.UserManage.Domain.Common;
using System;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace PMS.UserManage.Domain.Entities
{
    /// <summary>
    /// 系统消息
    /// </summary>
    public class SysMessage
    {
        public SysMessage()
        {
            Id = Guid.NewGuid();
            IsRead = false;
            PushTime = DateTime.Now;
        }
        /// <summary>
        /// 系统消息id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public SysMessageType Type { get; set; }
        /// <summary>
        /// MessageDataType
        /// </summary>
        public MessageDataType? DataType { get; set; }
        /// <summary>
        /// 数据ID
        /// </summary>
        public Guid DataId { get; set; }
        /// <summary>
        /// 消息发送者
        /// </summary>
        public Guid SenderUserId { get; set; }
        /// <summary>
        /// 消息接收者
        /// </summary>
        public Guid? UserId { get; set; }
        /// <summary>
        /// 消息标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 原始MessageContent
        /// </summary>
        public string OriContent { get; set; }
        public Guid? OriSenderUserId { get; set; }
        /// <summary>
        /// 原始MessageType
        /// </summary>
        public MessageType OriType { get; set; }
        public Guid? EId { get; set; }
        
        /// <summary>
        /// 是否已读
        /// </summary>
        public bool IsRead { get; set; }
        /// <summary>
        /// 消息录入时间【默认使用默认值值getdate()】
        /// </summary>
        public DateTime PushTime { get; set; }
    }

    /// <summary>
    /// 消息阅读状态   泛指用户时该表记录消息阅读状态 【文章、微课】
    /// </summary>
    public class SysMessageState 
    {
        /// <summary>
        /// 消息id
        /// </summary>
        public Guid MessageId { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }
    }

    public class SysMessageDetail : SysMessage
    {
        /// <summary>
        /// 发布者名
        /// </summary>
        public string SenderNickname { get; set; }
        /// <summary>
        /// 发布者头像
        /// </summary>
        public string SenderHeadImgUrl { get; set; }
    }

    /// <summary>
    /// 消息提醒
    /// </summary>
    public class SysMessageTips : SysMessage
    {
        /// <summary>
        /// 消息提示数
        /// </summary>
        public int TipsTotal { get; set; }
        /// <summary>
        /// 发布者名
        /// </summary>
        public string nickname { get; set; }
        /// <summary>
        /// 发布者头像
        /// </summary>
        public string HeadImgUrl { get; set; }
    }

    /// <summary>
    /// 个人动态
    /// </summary>
    public class DynamicItem 
    {
        /// <summary>
        /// 数据id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public UserDynamicType Type { get; set; }
    }

}
