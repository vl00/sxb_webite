using System;

namespace Sxb.Web.Areas.Common.WeChatQRCallBackHandle.DataJson
{

    /*
     预约成功！我们将在【{SubTitle}】每个重要事项开始与结束的前一天通过本公众号发送提醒通知。

若您取消关注了，我们就无法给您发送提醒通知了哦！

<a href="{DataUrl}">>>点此返回刚刚的页面继续阅读</a> 
     */
    public class SubscribeRemindDataJson
    {
        /// <summary> 
        /// 开始提醒时间 
        /// </summary> 
        public DateTime? StartTime { get; set; }

        /// <summary> 
        /// 结束提醒时间 
        /// </summary> 
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 日程一级标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 日程二级标题
        /// </summary>
        public string SubTitle { get; set; }
    }
}
