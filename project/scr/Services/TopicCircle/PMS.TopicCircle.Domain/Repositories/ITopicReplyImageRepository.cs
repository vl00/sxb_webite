using PMS.TopicCircle.Domain.Entities;
using PMS.TopicCircle.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static PMS.TopicCircle.Domain.Enum.GlobalEnum;

namespace PMS.TopicCircle.Domain.Repositories
{
    public interface ITopicReplyImageRepository : IRepository<TopicReplyImage>
    {
        int AddOrUpdate(IEnumerable<TopicReplyImage> images);
        int Delete(IEnumerable<TopicReplyImage> images);
        IEnumerable<TopicReplyImage> GetList(IEnumerable<Guid> topicReplyIds);
    }
}
