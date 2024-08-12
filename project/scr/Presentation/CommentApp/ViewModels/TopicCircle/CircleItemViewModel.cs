using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.ViewModels.TopicCircle
{
    public class CircleItemViewModel
    {
        /// <summary>
        /// 点击跳转的链接
        /// </summary>
        public string ActionUrl { get; set; }
        public Guid Id { get; set; }
        /// <summary>
        /// 话题圈名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 话题圈图
        /// </summary>
        public string Cover { get; set; }

        /// <summary>
        /// 当前登录人是否加入
        /// </summary>
        public bool LoginUserFollow { get; set; }

        /// <summary>
        /// 圈粉数
        /// </summary>
        public long FollowCount { get; set; }

        public string Intro { get; set; }   


        /// <summary>
        /// 圈主信息
        /// </summary>
        public Guid UserId { get; set; }
        public string UserName { get; set; }
    }
}
