using PMS.TopicCircle.Application.Dtos;
using ProductManagement.Framework.AspNetCoreHelper.RequestModel;
using Sxb.Web.RequestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.TopicCircle.Models
{
    public class TopicReplyAddRequest:WebContentRequest
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
        public  string Content { get; set; }

        public override string GetContent()
        {
            return this.Content;
        }

        public static implicit operator TopicReplyAddDto(TopicReplyAddRequest topicReplyAddRequest)
        {
            return new TopicReplyAddDto()
            {
                Content = topicReplyAddRequest.Content,
                Id = topicReplyAddRequest.Id,
                ParentId = topicReplyAddRequest.ParentId,
                TopicId = topicReplyAddRequest.TopicId,
                UserId = topicReplyAddRequest.UserId
            };
        }
    }
}
