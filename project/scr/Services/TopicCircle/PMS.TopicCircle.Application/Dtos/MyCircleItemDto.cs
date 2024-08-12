using PMS.TopicCircle.Domain.Dtos;
using System;
using System.Collections.Generic;

namespace PMS.TopicCircle.Application.Dtos
{

    /// <summary>
    /// "我的圈子"中的项
    /// </summary>
    public class MyCircleItemDto
    {
        public Guid CircleId { get; set; }

        /// <summary>
        /// 圈子名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否有新动态
        /// </summary>
        public bool HasNews { get; set; }

        public string Cover { get; set; }

        /// <summary>
        /// 新贴数
        /// </summary>
        public int NEWTOPICCOUNT { get; set; }

        /// <summary>
        /// 新回复数
        /// </summary>
        public int NEWREPLYCOUNT { get; set; }


        /// <summary>
        /// 是否为圈主
        /// </summary>

        public bool IsCircleMaster { get; set; }


        /// <summary>
        /// 关注者数量
        /// </summary>
        public int FOLLOWERCOUNT { get; set; }


        /// <summary>
        /// 背景颜色
        /// </summary>
        public string BGColor { get; set; }

        public static implicit operator MyCircleItemDto(USPQUERYMYCIRCLESDto USPQUERYMYCIRCLESDto)
        {
            return new MyCircleItemDto()
            {
                CircleId = USPQUERYMYCIRCLESDto.Id,
                HasNews = USPQUERYMYCIRCLESDto.HASNEWS,
                Name = USPQUERYMYCIRCLESDto.Name,
                FOLLOWERCOUNT = USPQUERYMYCIRCLESDto.FOLLOWERCOUNT,
                NEWREPLYCOUNT = USPQUERYMYCIRCLESDto.NEWREPLYCOUNT,
                NEWTOPICCOUNT = USPQUERYMYCIRCLESDto.NEWTOPICCOUNT
            };

        }
        public static implicit operator MyCircleItemDto(USPSTATICCIRCLENEWSINFO USPQUERYMYCIRCLESDto)
        {
            return new MyCircleItemDto()
            {
                CircleId = USPQUERYMYCIRCLESDto.Id,
                HasNews = USPQUERYMYCIRCLESDto.HASNEWS,
                Name = USPQUERYMYCIRCLESDto.Name,
                FOLLOWERCOUNT = USPQUERYMYCIRCLESDto.FOLLOWERCOUNT,
                NEWREPLYCOUNT = USPQUERYMYCIRCLESDto.NEWREPLYCOUNT,
                NEWTOPICCOUNT = USPQUERYMYCIRCLESDto.NEWTOPICCOUNT,
                BGColor = USPQUERYMYCIRCLESDto.BGColor
            };

        }

        public IEnumerable<SimpleTopicDto> NewestTopics { get; set; }
        public string CircleMasterName { get; set; }
    }
}
