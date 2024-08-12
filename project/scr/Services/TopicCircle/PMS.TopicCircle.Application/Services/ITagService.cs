using PMS.TopicCircle.Application.Dtos;
using PMS.TopicCircle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.TopicCircle.Application.Services
{
    public interface ITagService : IApplicationService<Tag>
    {

        /// <summary>
        /// 获取标签枚举
        /// </summary>
        /// <returns></returns>
        AppServiceResultDto<IEnumerable<TagDto>> GetTags();

        /// <summary>
        /// 获取圈子热门标签
        /// </summary>
        /// <param name="circleId"></param>
        /// <param name="takeCount"></param>
        /// <returns></returns>
        AppServiceResultDto<IEnumerable<TagHotDto>> GetHotTags(Guid circleId, int takeCount);

        /// <summary>
        /// 获取帖子标签
        /// </summary>
        /// <param name="topicID">帖子ID</param>
        /// <returns></returns>
        Task<IEnumerable<TagDto>> GetByTopicID(Guid topicID);
    }
}
