using System;

namespace Sxb.Web.Application.Event
{
    public class AddWeChatSubscribeIntegrationEvent
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 微信openId
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 关注时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
