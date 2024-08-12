using PMS.TopicCircle.Application.Dtos;
using PMS.TopicCircle.Domain.Dtos;
using PMS.TopicCircle.Domain.Entities;
using ProductManagement.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static PMS.TopicCircle.Domain.Enum.GlobalEnum;

namespace PMS.TopicCircle.Application.Services
{
    public interface ITopicService : IApplicationService<Topic>
    {
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        AppServiceResultDto<object> Add(TopicAddDto topicDto);
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="topicDto"></param>
        /// <returns></returns>
        AppServiceResultDto Edit(TopicAddDto topicDto);
        /// <summary>
        /// 删除话题及其评论
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AppServiceResultDto Delete(Guid id, Guid userId);
        /// <summary>
        /// 删除评论及其回复
        /// </summary>
        /// <param name="id"></param>
        /// <param name="topicReplyId"></param>
        /// <returns></returns>
        AppServiceResultDto DeleteReply(Guid id, Guid topicReplyId, Guid userId);
        /// <summary>
        /// 置顶
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AppServiceResultDto Top(Guid id, Guid userId, bool cancel);
        /// <summary>
        /// 设为精品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AppServiceResultDto Good(Guid id, Guid userId, bool isGood);
        /// <summary>
        /// get one
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <param name="childChount"></param>
        /// <returns></returns>
        Task<TopicDto> Get(Guid id, Guid? userId, int childChount = 0);
        /// <summary>
        /// get by ids
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="loginUserId"></param>
        /// <returns></returns>
        List<TopicDto> GetByIds(IEnumerable<Guid> ids, Guid? loginUserId);
        List<TopicDto> GetByIds(IEnumerable<Guid> ids);
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="keyword"></param>
        /// <param name="circleId"></param>
        /// <param name="type"></param>
        /// <param name="isGood"></param>
        /// <param name="isQA"></param>
        /// <param name="tags"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<PaginationModel<TopicDto>> GetPagination(Guid? userId, string keyword, Guid? circleId, int? type, bool? isGood, bool? isQA,
            List<int> tags, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, bool getCoverAndFollow = true);
        Task<PaginationModel<TopicDto>> GetPaginationByEs(Guid? userId, string keyword, Guid? circleId, bool? isCircleOwner, int? type, bool? isGood, bool? isQA,
            List<string> tags, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize);

        /// <summary>
        /// 获取话题列表
        /// </summary>
        /// <param name="circleId"></param>
        /// <param name="userId"></param>
        /// <param name="topType"></param>
        /// <param name="isGood"></param>
        /// <param name="isQA"></param>
        /// <returns></returns>
        IEnumerable<TopicDto> GetList(Guid? circleId, Guid? userId, TopicTopType? topType, bool? isGood, bool? isQA);
        /// <summary>
        /// 我的圈子动态(显示条数：3条)
        /// 显示用户关注所有圈子里有最新动态的帖子。最新动态指：有最新回复的帖子，或最新发出/编辑的帖子
        /// 按帖子最新动态的时间降序排列。
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IEnumerable<TopicDto> GetDynamicList(Guid userIdm, int pageIndex = 1, int pageSize = 3);
        /// <summary>
        /// 获取圈子内热门话题(显示数量：5个)
        /// </summary>
        /// <param name="circleId"></param>
        /// <returns></returns>
        IEnumerable<TopicDto> GetCircleHotList(Guid circleId, int pageIndex, int pageSize);
        /// <summary>
        /// 获取热门话题列表(显示总数量：10个)
        /// 1.管理员置顶话题
        /// 2.同城达人的话题
        /// 3.全无, 搜其他话题
        /// </summary>
        /// <param name="circleId"></param>
        /// <returns></returns>
        IEnumerable<TopicDto> GetHotList(Guid? userId, int? cityCode);
        /// <summary>
        /// 获取热门话题列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cityCode"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        Task<IEnumerable<Domain.Dtos.SimpleTopicDto>> GetHotList(Guid? userId, int? cityCode, int? count);
        /// <summary>
        /// 获取置顶话题列表
        /// 1.管理员推广链接(最多两个)
        /// 2.圈主置顶话题(最多三个)
        /// </summary>
        /// <param name="circleId"></param>
        /// <returns></returns>
        TopDto GetTopList(Guid circleId);
        /// <summary>
        /// 收藏话题
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        AppServiceResultDto Follow(Guid id, Guid userId);
        AppServiceResultDto AddAutoSyncTopic(TopicAddDto topicDto);
        void UpdateTopicData(Guid id);
        /// <summary>
        /// 根据置顶类型获取话题
        /// </summary>
        /// <param name="type"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        Task<IEnumerable<Domain.Dtos.SimpleTopicDto>> GetListByTopType(int type, int count = 10);

        Task<IEnumerable<Domain.Dtos.SimpleTopicDto>> GetListByIsHandPick(bool isHandPick, int count = 10);

        Task<IEnumerable<Domain.Dtos.SimpleTopicDto>> GetIsGoodList(int count = 10);

        /// <summary>
        /// 获取圈子最新动态帖子
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        Task<IEnumerable<Domain.Dtos.SimpleTopicDto>> GetNewestDynamicTimeTopics(Guid circleID, int count = 10);


        /// <summary>
        /// 最多讨论列表
        /// </summary>
        /// <param name="circleID"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        IEnumerable<Topic> MoreDiscuss(Guid circleID, int offset, int limit);

        /// <summary>
        /// 相关话题
        /// </summary>
        /// <param name="topicID">被相关话题</param>
        /// <param name="tagIDs">标签IDs</param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="inCircle">是否只查找本圈</param>
        /// <returns></returns>
        Task<IEnumerable<SimpleTopicDto>> GetRelatedTopics(Guid topicID, IEnumerable<int> tagIDs, int offset = 0, int limit = 5, bool inCircle = false);

        /// <summary>
        /// 根据回复量倒序获取帖子
        /// </summary>
        /// <param name="topicID"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        Task<IEnumerable<SimpleTopicDto>> GetByReplyCount(Guid circleID, int offset = 0, int limit = 5, IEnumerable<Guid> excludeTopicIDs = null);
        SimpleTopicDto GetSimple(Guid id);
    }
}
