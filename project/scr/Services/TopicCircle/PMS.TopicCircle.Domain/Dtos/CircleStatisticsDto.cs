using System;

namespace PMS.TopicCircle.Domain.Dtos
{
    /// <summary>
    /// 话题圈统计
    /// </summary>
    public class CircleStatisticsDto
    {
        /// <summary>
        /// 圈子ID
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 贴子总数
        /// </summary>
        public int TopicCount { get; set; }
        /// <summary>
        /// 所有帖子回复总数
        /// </summary>
        public int ReplyCount { get; set; }
        /// <summary>
        /// 所有帖子点赞总数
        /// </summary>
        public int LikeCount { get; set; }
        /// <summary>
        /// 互动数(话题圈内的主题帖总数（包含仅圈主可见）+所有话题帖评论总数+所有话题帖回复总数+所有话题帖点赞总人次数)
        /// </summary>
        public int DynamicCount { get; set; }
    }
}
