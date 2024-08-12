using PMS.TopicCircle.Application.Dtos;
using PMS.TopicCircle.Application.Services;
using PMS.TopicCircle.Domain.Entities;
using ProductManagement.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.TopicCircle.Application.Services
{
    public interface ITopicReplyService : IApplicationService<TopicReply>
    {
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="topicReplyDto"></param>
        /// <returns></returns>
        AppServiceResultDto<object> Add(TopicReplyAddDto topicReplyDto);
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="topicReplyDto"></param>
        /// <returns></returns>
        AppServiceResultDto Edit(TopicReplyAddDto topicReplyDto);
        /// <summary>
        /// 获取顶级评论, 及其前5条Children评论
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        PaginationModel<TopicReplyDto> GetPaginationByTopic(Guid? topicId, int pageIndex, int pageSize, bool createTimeDesc = false, int topReplyCount = 5);
        /// <summary>
        /// 获取顶级评论的子评论
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="firstParentId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        PaginationModel<TopicReplyDto> GetChildren(Guid? topicId, Guid? firstParentId, int pageIndex, int pageSize);

        /// <summary>
        /// 获取话题的所有评论(Children二级嵌套)
        /// </summary>
        /// <param name="topicId"></param>
        /// <returns></returns>
        List<TopicReplyDto> GetTopicReplies(Guid topicId);
        /// <summary>
        /// 获取话题的前n条评论
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        List<TopicReplyDto> GetTopTopicReplies(Guid topicId, int size);

        /// <summary>
        /// 点赞
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AppServiceResultDto Like(Guid id, Guid userId);

        /// <summary>
        /// 获取传入的userId对于topicReplyIDs的话题或话题评论是否已点赞
        /// </summary>
        Dictionary<Guid, bool> GetIsLike(Guid userID, IEnumerable<Guid> topicReplyIDs);

        /// <summary>
        /// 获取回复链
        /// </summary>
        /// <param name="replyID">指定的回复</param>
        /// <param name="maxDepth">最深层次，Null表示递归到顶层</param>
        /// <returns></returns>
        IEnumerable<TopicReplyDto> GetLevelReplies(Guid replyID, int? maxDepth = null);

        /// <summary>
        /// 获取回复链
        /// </summary>
        /// <param name="replyID">指定的回复</param>
        /// <returns></returns>
        Task<IEnumerable<TopicReplyDto>> GetLevelRepliesEx(Guid replyID, int limit = 7);
    }
}
