using PMS.TopicCircle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.TopicCircle.Domain.Repositories
{
    public interface ITagRepository : IRepository<Tag>
    {

        IEnumerable<Tag> GetTags();

        IEnumerable<Tag> ExcuteUSP_STASTICHOTTAG(Guid circleId, int takeCount);

        /// <summary>
        /// 获取帖子标签
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Tag>> GetByTopicID(Guid topicID);
    }
}
