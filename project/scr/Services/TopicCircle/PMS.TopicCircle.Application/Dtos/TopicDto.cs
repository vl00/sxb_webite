using PMS.TopicCircle.Domain.Dtos;
using PMS.TopicCircle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PMS.TopicCircle.Application.Dtos
{
    public class TopicDto
    {
        public Guid Id { get; set; }

        public string Content { get; set; }

        public int Type { get; set; }

        public int TopType { get; set; }

        public long ReplyCount { get; set; }

        public long FollowCount { get; set; }

        public long LikeCount { get; set; }
        public Guid Creator { get; set; }

        /// <summary>
        /// 是否达人自动同步话题
        /// </summary>
        public bool IsAutoSync { get; set; }

        public List<TopicReplyImage> Images { get; set; }

        public TopicReplyAttachment Attachment { get; set; }

        public List<TopicReplyDto> Replies { get; set; }

        public List<SimpleTagDto> Tags { get; set; }

        /// <summary>
        /// 点赞的用户昵称
        /// </summary>
        public List<string> LikeUserNames { get; set; }

        public bool Follow { get; set; }

        public bool Like { get; set; }

        /// <summary>
        /// 动态时间
        /// </summary>
        public string Time { get; set; }
        /// <summary> 
        /// 最后编辑时间 
        /// </summary> 
        public string LastEditTime { get; set; }
        /// <summary> 
        /// 最后回复时间 
        /// </summary> 
        public string LastReplyTime { get; set; }


        public bool IsQA { get; set; }

        public bool IsOpen { get; set; }
        public bool IsGood { get; set; }

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
        public string CircleCover { get; set; }
        public string CircleIntro { get; set; }
        public Guid? CircleUserId { get; set; }
        public long CircleFollowCount { get; set; }
        public long CircleTopicCount { get; set; }

        /// <summary>
        /// 是否是圈主的话题
        /// </summary>
        public bool IsCircleOwner { get; set; }

        /// <summary>
        /// 是否是登录人的话题
        /// </summary>
        public bool IsLoginUserOwner { get; set; }

        /// <summary>
        /// 是否是登录
        /// </summary>
        public bool IsLogin { get; set; }

        /// <summary>
        /// 是否是话题圈的粉丝
        /// </summary>
        public bool IsCircleFollower { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
