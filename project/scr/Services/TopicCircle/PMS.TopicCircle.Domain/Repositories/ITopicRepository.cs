using PMS.TopicCircle.Domain.Dtos;
using PMS.TopicCircle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PMS.TopicCircle.Domain.Enum.GlobalEnum;

namespace PMS.TopicCircle.Domain.Repositories
{
    public interface ITopicRepository : IRepository<Topic>
    {
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        int Add(Topic topic);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int Delete(Topic topic);
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int Update(Topic topic);
        /// <summary>
        /// 更新话题内容
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        int UpdateContent(Topic topic);
        /// <summary>
        /// 更新时间
        /// </summary>
        /// <returns></returns>
        int UpdateReplyTotal(Guid id, long total, DateTime? replyTime);
        int UpdateLikeTotal(Guid id, long total);
        int UpdateFollowTotal(Guid id, long total);
        /// <summary>
        /// 圈主置顶
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int Top(Guid id, TopicTopType topType);
        /// <summary>
        /// 设为精品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int Good(Guid id, bool isGood);
        /// <summary>
        /// get one
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Topic Get(Guid id);
        long GetTotal(string keyword, Guid? circleId, Guid? creator, int? type, TopicTopType? topType, bool? isGood, bool? isQA, IEnumerable<int> tags, DateTime? startTime, DateTime? endTime);
        /// <summary>
        /// 获取话题列表
        /// </summary>
        /// <param name="circleId"></param>
        /// <param name="creator"></param>
        /// <param name="topType"></param>
        /// <param name="isGood"></param>
        /// <param name="isQA"></param>
        /// <returns></returns>
        IEnumerable<SimpleTopicDto> GetList(Guid? circleId, Guid? creator, TopicTopType? topType, bool? isGood, bool? isQA);

        /// <summary>
        /// 公开话题 
        /// </summary>
        /// <param name="circleId"></param>
        /// <param name="topType"></param>
        /// <param name="sort"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IEnumerable<SimpleTopicDto> GetPagination(Guid? circleId, TopicTopType? topType, TopicSort sort = TopicSort.None, int pageIndex = 1, int pageSize = 10);

        /// <summary>
        /// 获取管理员置顶的精选话题
        /// </summary>
        /// <param name="circleId"></param>
        /// <param name="sort"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IEnumerable<SimpleTopicDto> GetPagination(Guid? circleId, TopicSort sort = TopicSort.None, int pageIndex = 1, int pageSize = 10);
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="circleId"></param>
        /// <param name="creator"></param>
        /// <param name="topType"></param>
        /// <param name="isGood"></param>
        /// <param name="isQA"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IEnumerable<SimpleTopicDto> GetPagination(string keyword, Guid? circleId, Guid? creator, int? type, TopicTopType? topType, bool? isGood, bool? isQA, bool? isOpen,
                                        IEnumerable<int> tags, DateTime? startTime, DateTime? endTime, TopicSort sort = TopicSort.None, int pageIndex = 1, int pageSize = 10);
        IEnumerable<SimpleTopicDto> GetPagination(string keyword, IEnumerable<Guid> circleIds, Guid? creator, int? type, TopicTopType? topType, bool? isGood, bool? isQA, bool? isOpen,
                                        IEnumerable<int> tags, DateTime? startTime, DateTime? endTime, TopicSort sort = TopicSort.None, int pageIndex = 1, int pageSize = 10);

        /// <summary>
        /// 同城话题分页列表
        /// </summary>
        /// <param name="cityCode"></param>
        /// <param name="excludeIds"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="banDisable">是否禁用话题圈设置Disable的圈子</param>
        /// <returns></returns>
        IEnumerable<SimpleTopicDto> GetPagination(int? cityCode, IEnumerable<Guid> excludeIds, int pageIndex, int pageSize
             , bool banDisable = false);
        IEnumerable<SimpleTopicDto> GetList(IEnumerable<Guid> ids);
        Topic GetAutoSyncTopic(Guid attachId);

        IEnumerable<Topic> GetByIDs(IEnumerable<Guid> ids);

        Task<IEnumerable<SimpleTopicDto>> GetByTopType(int type, int count = 10);

        Task<IEnumerable<SimpleTopicDto>> GetByIsHandPick(bool isHandPick, int count = 10);

        Task<IEnumerable<SimpleTopicDto>> GetIsGood(int count = 10);


        IEnumerable<Topic> MoreDiscuss(Guid circleID, int offset, int limit);


        /// <summary>
        /// 根据最新动态时间获取帖子
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        Task<IEnumerable<SimpleTopicDto>> GetTopicOrderByDynamicTime(Guid circleID, int count = 10);

        /// <summary>
        /// 获取相关帖子
        /// </summary>
        /// <param name="topicID">帖子ID</param>
        /// <param name="tagIDs">标签ID</param>
        /// <returns></returns>
        Task<IEnumerable<Guid>> GetRelatedTopicIDs(Guid topicID, IEnumerable<int> tagIDs, int offset = 0, int limit = 5, Guid? circleID = null);
        SimpleTopicDto GetSimple(Guid id);
    }
}
