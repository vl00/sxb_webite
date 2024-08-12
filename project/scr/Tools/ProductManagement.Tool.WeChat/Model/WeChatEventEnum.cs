using System;
using System.Collections.Generic;
using System.Text;

namespace WeChat.Model
{
    /// <summary>
    /// 微信回调事件
    /// </summary>
    public enum WeChatEventEnum
    {
        unkonw,

        /// <summary>
        /// 关注事件
        /// </summary>
        subscribe,

        /// <summary>
        /// 取关事件
        /// </summary>
        unsubscribe,

        /// <summary>
        /// 扫描场景二维码事件（在已关注的条件下才会触发）
        /// </summary>
        SCAN,

        /// <summary>
        /// 点击菜单事件
        /// </summary>
        CLICK,

        /// <summary>
        /// 上报地理位置事件
        /// </summary>
        LOCATION,

        /// <summary>
        /// 点击菜单跳链接事件
        /// </summary>
        VIEW

    }
}
