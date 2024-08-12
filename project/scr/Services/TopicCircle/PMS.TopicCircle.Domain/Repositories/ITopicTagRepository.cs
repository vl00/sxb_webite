using PMS.TopicCircle.Domain.Dtos;
using PMS.TopicCircle.Domain.Entities;
using PMS.TopicCircle.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static PMS.TopicCircle.Domain.Enum.GlobalEnum;

namespace PMS.TopicCircle.Domain.Repositories
{
    public interface ITopicTagRepository : IRepository<TopicTag>
    {
        int AddOrUpdate(IEnumerable<TopicTag> topicTags);
        int Delete(IEnumerable<TopicTag> topicTags);
        IEnumerable<SimpleTagDto> GetTags();
        IEnumerable<TopicTag> GetList(IEnumerable<Guid> topicIds);
    }
}
