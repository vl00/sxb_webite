using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.TopicCircle.Models
{
    public class CircleCreateResponse
    {
        [Description("话题圈ID")]
        public Guid CircleId { get; set; }

        [Description("是否绑定了手机号码")]
        public bool IsBindPhone { get; set; }

        [Description("是否绑定了微信")]
        public bool IsBindWx { get; set; }

        [Description("是否关注了服务号")]
        public bool IsSubscribeFwh { get; set; }

    }
}
