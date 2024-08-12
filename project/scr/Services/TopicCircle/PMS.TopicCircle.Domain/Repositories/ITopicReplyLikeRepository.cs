using PMS.TopicCircle.Domain.Entities;
using PMS.TopicCircle.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static PMS.TopicCircle.Domain.Enum.GlobalEnum;

namespace PMS.TopicCircle.Domain.Repositories
{
    public interface ITopicReplyLikeRepository : IRepository<TopicReplyLike>
    {
        TopicReplyLike GetUow(Guid userId, Guid topicReplyId);

        int Add(TopicReplyLike like);

        int Update(TopicReplyLike like);

        IEnumerable<Guid> GetIdList(IEnumerable<Guid> dataIds, Guid userId);
        IEnumerable<TopicReplyLike> GetPagination(IEnumerable<Guid> topicReplyIds, IEnumerable<Guid> userIds, int pageIndex, int pageSize);
        IEnumerable<TopicReplyLike> GetPagination(Guid? topicReplyId, Guid? userId, int pageIndex, int pageSize);
        /// <summary>
        /// 获取每个topic的前n条点赞
        /// </summary>
        /// <param name="topicReplyIds"></param>
        /// <param name="groupSize"></param>
        /// <returns></returns>
        IEnumerable<TopicReplyLike> GetList(IEnumerable<Guid> topicReplyIds, int groupSize);
    }
}
