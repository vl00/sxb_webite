using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.TopicCircle.Domain.Dtos
{
    /// <summary>
    /// 话题圈的关注人数统计结果值
    /// </summary>
    public class CircleFollowerCountDto
    {
        public Guid CircleId { get; set; }

        public int FollowerCount { get; set; }
    }
}
