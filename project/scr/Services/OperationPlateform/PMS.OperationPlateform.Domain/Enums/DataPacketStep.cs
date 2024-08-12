using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PMS.OperationPlateform.Domain.Enums
{
    /// <summary>
    /// 0 无 1 关注消息  2 24小时消息 3 36小时消息 4 48小时消息
    /// </summary>
    public enum DataPacketStep
    {
        /// <summary>
        /// 无
        /// </summary>
        [DefaultValue(-1)]
        None = 0,

        /// <summary>
        /// 关注消息  0h < date < 24h  
        /// </summary>
        [DefaultValue(0)]
        [Description("weixin_data_packet_subscribe_auto_reply")]
        Subcribe = 1,

        /// <summary>
        /// 24小时消息 24h < date < 36h  
        /// </summary>
        [DefaultValue(24)]
        [Description("weixin_data_packet_subscribe_reply_24h")]
        Above24h = 2,

        /// <summary>
        /// 36小时消息   36h < date < 46h
        /// </summary>
        [DefaultValue(36)]
        [Description("weixin_data_packet_subscribe_reply_36h")]
        Above36h = 3,

        /// <summary>
        /// 48小时消息  //提前两小时, 避免超过48小时不能发送消息
        /// </summary>
        [DefaultValue(46)]
        [Description("weixin_data_packet_subscribe_reply_48h")]
        [DefaultImageKey("weixin_data_packet_subscribe_reply_48h_image")]
        Above48h = 4
    }
}
