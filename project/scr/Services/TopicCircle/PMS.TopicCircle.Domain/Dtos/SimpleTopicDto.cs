using PMS.TopicCircle.Domain.Entities;
using ProductManagement.Framework.Foundation;
using System;

namespace PMS.TopicCircle.Domain.Dtos
{
    public class SimpleTopicDto : Topic
    {
        /// <summary>
        /// Create User Info
        /// </summary>
        public string UserName { get; set; }
        public string HeadImgUrl { get; set; }

        /// <summary>
        /// Circle Info
        /// </summary>
        public string CircleName { get; set; }
        public string CircleIntro { get; set; }
        public string CircleCover { get; set; }
        public Guid? CircleUserId { get; set; }

        /// <summary>
        /// 是否是圈主的话题
        /// </summary>
        public bool IsCircleOwner { get; set; }
        /// <summary>
        /// 是否精选
        /// </summary>
        public bool IsHandPick { get; set; }
        public string CreateTimeStr => DynamicTime.ConciseTime();
        /// <summary>
        /// 是否关注圈子
        /// </summary>
        public bool IsFollowed { get; set; }
    }
}
