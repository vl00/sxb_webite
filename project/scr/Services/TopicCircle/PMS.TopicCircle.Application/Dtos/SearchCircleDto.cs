using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.TopicCircle.Application.Dtos
{
    public class SearchCircleDto
    {
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
        /// 当前登录人是否加入圈子
        /// </summary>
        public bool LoginUserFollow { get; set; }

        /// <summary>
        /// 圈粉数
        /// </summary>
        public long FollowCount { get; set; }

        /// <summary>
        /// 圈子介绍
        /// </summary>
        public string Intro { get; set; }


        /// <summary>
        /// 圈主信息
        /// </summary>
        public Guid UserId { get; set; }
        public string UserName { get; set; }
    }
}
