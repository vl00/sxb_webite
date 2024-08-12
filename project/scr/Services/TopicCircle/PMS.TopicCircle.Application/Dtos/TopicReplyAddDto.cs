using PMS.TopicCircle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PMS.TopicCircle.Application.Dtos
{
    public class TopicReplyAddDto
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 评论的话题Id
        /// </summary>
        public Guid TopicId { get; set; }
        /// <summary>
        /// 如果是回复评论,  这里是被回复的评论Id
        /// </summary>
        public Guid? ParentId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; }
    }
}
