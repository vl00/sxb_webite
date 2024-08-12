using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using WeChat.Model;

namespace PMS.MediatR.Events.WeChat
{
    public class ScanSubscribeEvent : INotification
    {
        public string AppId { get; set; }
        public string AppName { get; set; }

        public string OpenId { get; set; }

        [Description("业务ID")]
        public Guid? DataId { get; set; }

        [Description("业务链接")]
        public string DataUrl { get; set; }

        [Description("额外数据")]
        public string DataJson { get; set; }

        [Description("回调类型")]
        public string Type { get; set; }

        [Description("微信回调事件类型")]
        public WeChatEventEnum WeChatEvent { get; set; }
    }
}
