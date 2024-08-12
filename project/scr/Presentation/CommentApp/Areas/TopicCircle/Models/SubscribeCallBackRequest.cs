using Nest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.TopicCircle.Models
{
    public enum SubscribeCallBackType { 
        
        [Description("加入圈子场景")]
        joincircle=1,

        
        [Description("查看完整的帖子场景")]
        viewTopicDetail = 2,


        [Description("绑定账户")]
        bindAccount = 3,

        [Description("欢迎达人关注")]
        welcomTalent=4,

        [Description("文章内关注服务号,推送文章图文")]
        articleview = 5,
    }

    public class SubscribeCallBackQueryRequest
    {

        [Description("业务ID")]
        public Guid? DataId { get; set; }

        [Description("业务链接")]
        public string DataUrl { get; set; }

        [Description("回调类型")]
        public SubscribeCallBackType Type { get; set; }


        [Description("是否小程序")]
        public bool IsMiniApp { get; set; }

    }

    public class SubscribeCallBackFormRequest
    {
        [Description("微信OpenId")]
        public string OpenId { get; set; }
    }
}
