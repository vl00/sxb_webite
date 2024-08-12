using PMS.TopicCircle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace PMS.TopicCircle.Application.Dtos
{
    /// <summary>
    /// 话题圈关注者详情信息
    /// </summary>
    public class CircleFollowerDetailDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 圈子ID
        /// </summary>
        public Guid CircleId { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        public string HeadImg { get; set; }

        /// <summary>
        /// 加入天数
        /// </summary>
        public int JoinDays { get; set; }

        /// <summary>
        /// 发帖数量
        /// </summary>
        public int TopicCount { get; set; }

        public DateTime LoginTime { get; set; }


        public static implicit operator CircleFollowerDetailDto(CircleFollower circleFollower)
        {
            return new CircleFollowerDetailDto()
            {
                CircleId = circleFollower.CircleId,
                JoinDays = circleFollower.JoinDays,
                TopicCount = circleFollower.SendCount,
                UserId = circleFollower.UserId,
                NickName = circleFollower.NickName,
                HeadImg = circleFollower.HeadImgUrl,
                LoginTime = circleFollower.LoginTime
            };
        }

    }
}
