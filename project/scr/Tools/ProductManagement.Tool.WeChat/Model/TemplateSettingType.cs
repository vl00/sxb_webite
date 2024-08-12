using System;
using System.Collections.Generic;
using System.Text;

namespace WeChat.Model
{
    /// <summary>
    /// 微信回调事件
    /// </summary>
    [Flags]
    public enum TemplateSettingType
    {
        None,
        /// <summary>
        /// app
        /// </summary>
        App,
        /// <summary>
        /// 微信模板消息
        /// </summary>
        Wx,
        /// <summary>
        /// 短信
        /// </summary>
        Sms = 4,
        /// <summary>
        /// 站内信
        /// </summary>
        Sys = 8,
        /// <summary>
        /// 微信文字消息
        /// </summary>
        WxText = 16
    }
}
