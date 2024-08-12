using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.UserCenter.Models.TopicCircle
{
    public class CollectionTopicViewModel
    {
        public Guid Id { get; set; }

        public string Content { get; set; }

        public int Type { get; set; }

        public long ReplyCount { get; set; }

        public long FollowCount { get; set; }

        public long LikeCount { get; set; }

        public string Image { get; set; }

        public bool Follow { get; set; }

        public bool Like { get; set; }

        public string Time { get; set; }

        /// <summary>
        /// 是否达人自动同步话题
        /// </summary>
        public bool IsAutoSync { get; set; }

        /// <summary>
        /// Create User Info
        /// </summary>
        public string UserName { get; set; }
        public string HeadImgUrl { get; set; }

        /// <summary>
        /// Circle Info
        /// </summary>
        public Guid CircleId { get; set; }
        public string CircleName { get; set; }
    }
}
