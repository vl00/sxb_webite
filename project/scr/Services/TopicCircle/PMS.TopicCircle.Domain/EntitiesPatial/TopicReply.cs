using PMS.TopicCircle.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.TopicCircle.Domain.Entities
{
    public partial class TopicReply
    {
        /// <summary> 
        /// 创建人
        /// </summary> 
        public string CreatorName { get; set; }
        public string HeadImgUrl { get; set; }

        /// <summary> 
        /// 回复的人
        /// </summary> 
        public string ParentUserName { get; set; }

        public TopicReply()
        {
        }

        /// <summary>
        /// 创建话题本身对应TopicReply
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="content"></param>
        /// <param name="userId"></param>
        public TopicReply(Guid topicId, string content, Guid userId)
        {
            Id =  topicId;
            Content = content;
            TopicId = topicId;
            Creator = userId;
            Updator = userId;
        }

        public bool Delete(ITopicReplyRepository _topicReplyRepository)
        {
            return _topicReplyRepository.Delete(this) > 0;
        }
    }
}
