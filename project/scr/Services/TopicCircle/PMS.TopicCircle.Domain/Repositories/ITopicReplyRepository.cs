using PMS.TopicCircle.Domain.Entities;
using PMS.TopicCircle.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PMS.TopicCircle.Domain.Enum.GlobalEnum;

namespace PMS.TopicCircle.Domain.Repositories
{
    public interface ITopicReplyRepository : IRepository<TopicReply>
    {
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="topicReply"></param>
        /// <returns></returns>
        int Add(TopicReply topicReply);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="topicReply"></param>
        /// <returns></returns>
        int Delete(TopicReply topicReply);
        /// <summary>
        /// 更新评论内容
        /// </summary>
        /// <param name="topicReply"></param>
        /// <returns></returns>
        int UpdateContent(TopicReply topicReply);
        IEnumerable<TopicReply> GetList(Guid topicId);
        /// <summary>
        /// 获取每个topic的前n条回复
        /// </summary>
        /// <param name="topicReplyIds"></param>
        /// <param name="userIds"></param>
        /// <param name="groupSize"></param>
        /// <returns></returns>
        IEnumerable<TopicReply> GetTopList(IEnumerable<Guid> topicIds, int groupSize);
        /// <summary>
        /// 获取评论分页列表(包含depth=0)
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="parentId"></param>
        /// <param name="firstParentId"></param>
        /// <param name="depth"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        (IEnumerable<TopicReply> data, int total) GetPagination(Guid? topicId, Guid? parentId, Guid? firstParentId, int? depth, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, bool createTimeDesc = false);

        /// <summary>
        /// 更新点赞数量
        /// </summary>
        /// <param name="id"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        int UpdateLikeTotal(Guid id, long total);
        (IEnumerable<TopicReply> data, int total) GetPaginationByOffset(Guid? topicId, Guid? parentId, Guid? firstParentId, int? depth, DateTime? startTime, DateTime? endTime, long offset, int size, bool createTimeDesc = false);


        /// <summary>
        /// 获取回复链
        /// </summary>
        /// <param name="replyID">指定的回复</param>
        /// <param name="maxDepth">最深层次，Null表示递归到顶层</param>
        /// <returns></returns>

        IEnumerable<TopicReply> GetLevelReplies(Guid replyID, int? maxDepth = null);

        /// <summary>
        /// 获取回复链
        /// </summary>
        /// <param name="replyID">指定的回复</param>
        /// <returns></returns>
        Task<IEnumerable<TopicReply>> GetLevelRepliesEx(Guid replyID, int limit = 7);
    }
}
