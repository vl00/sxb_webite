using PMS.TopicCircle.Domain.Entities;
using PMS.TopicCircle.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static PMS.TopicCircle.Domain.Enum.GlobalEnum;

namespace PMS.TopicCircle.Domain.Repositories
{
    public interface ITopicReplyAttachmentRepository : IRepository<TopicReplyAttachment>
    {
        int Add(TopicReplyAttachment attachment);
        int AddOrUpdate(TopicReplyAttachment attachment);
        int Delete(TopicReplyAttachment attachment);
        IEnumerable<TopicReplyAttachment> GetList(IEnumerable<Guid> topicReplyIds);
        int Update(TopicReplyAttachment attachment);
    }
}
