using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.TopicCircle.Models
{
    public class CircleMasterInfoResponse
    {
        public Guid CircleId { get; set; }

        public Guid UserId { get; set; }

        /// <summary>
        /// 达人昵称
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// 达人认证信息
        /// </summary>
        public string AuthInfo { get; set; }


        public string HeadImgUrl { get; set; }

        public string CircleIntro { get; set; }
    }
}
