using PMS.TopicCircle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMS.TopicCircle.Application.Dtos
{
    public class CircleDetailDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Intro { get; set; }

        public string Cover { get; set; }

        public int FollowerCount { get; set; }

        public bool IsDisable { get; set; }

        public Guid UserId { get; set; }

        /// <summary>
        /// 上一次访问后新增加的粉丝数
        /// </summary>
        public int NewFollower { get; set; }

        /// <summary>
        /// 昨日新增粉丝数
        /// </summary>
        public int YesterdayFollowerCount { get; set; }

        /// <summary>
        /// 话题总数
        /// </summary>
        public long TopicCount { get; set; }

        /// <summary>
        /// 昨日新增话题数
        /// </summary>
        public int YesterdayTopicCount { get; set; }

        public int City { get; set; }
        /// <summary>
        /// 互动数
        /// </summary>
        public int DynamicCount { get; set; }
        /// <summary>
        /// 是否圈主
        /// </summary>
        public bool IsCircleMaster { get; set; }
        /// <summary>
        /// 是否已关注
        /// </summary>
        public bool IsFollowed { get; set; }

        public string BGColor { get; set; }
        public string UserName { get; set; }


        public static implicit operator CircleDetailDto(Circle circle)
        {
            return new CircleDetailDto()
            {
                Id = circle.Id,
                Name = circle.Name,
                Intro = circle.Intro,
                Cover = circle.Cover.Url,
                FollowerCount = circle.FollowerCount,
                IsDisable = circle.IsDisable,
                UserId = circle.UserId.GetValueOrDefault(),
                BGColor = circle.BGColor
            };
        }




    }
}
