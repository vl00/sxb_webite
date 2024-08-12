
using Dapper.Contrib.Extensions;
using System;
using System.ComponentModel;

namespace PMS.Infrastructure.Domain.Entities
{
    [Serializable]
    [Table("weixin_subscribe_log")]
    public class WeixinSubscribeLog
    {
        [ExplicitKey]
        [Write(false)]
        public long Id { get; set; }
        public DateTime CreateTime { get; set; }
        public string FromWhere { get; set; }
        public string Channel { get; set; }

        public string AppId { get; set; }
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
        public string WeChatEvent { get; set; }


        [Description("是否是首次关注公众号")]
        public bool IsFirstSubscribe { get; set; }
    }
}