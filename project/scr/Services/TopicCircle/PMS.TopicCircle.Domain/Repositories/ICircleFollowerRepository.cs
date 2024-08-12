using PMS.TopicCircle.Domain.Dtos;
using PMS.TopicCircle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.TopicCircle.Domain.Repositories
{
    public interface ICircleFollowerRepository : IRepository<CircleFollower>
    {
        /// <summary>
        /// 删除某个用户的圈子关系
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="circleId"></param>
        /// <returns></returns>
        bool Delete(Guid userId, Guid circleId);

        /// <summary>
        /// 统计话题圈的关注者数量
        /// </summary>
        /// <param name="circleIds"></param>
        /// <returns></returns>
        IEnumerable<CircleFollowerCountDto> GetFollowerCount(params Guid[] circleId);

        int GetNewFollowers(Guid circleId, DateTime? time);

        /// <summary>
        /// 执行USP_QUERYCIRCLEFOLLOWERDETAIL存储过程
        /// </summary>
        /// <param name="circleId"></param>
        /// <returns></returns>
        IEnumerable<CircleFollower> ExcuteUSP_QUERYCIRCLEFOLLOWERDETAIL(Guid circleId);

        /// <summary>
        /// 执行USP_SEARCHCIRCLEFOLLOWER存储过程
        /// </summary>
        /// <param name="circleId"></param>
        /// <returns></returns>
        IEnumerable<CircleFollower> ExcuteUSP_SEARCHCIRCLEFOLLOWER(Guid circleId, string like);

        /// <summary>
        /// 检查用户是否关注了圈子
        /// </summary>
        /// <param name="circleIDs">圈子们</param>
        /// <param name="userID"></param>
        /// <returns></returns>
        Task<IEnumerable<KeyValuePair<Guid, bool>>> CheckIsFollowCircle(IEnumerable<Guid> circleIDs, Guid userID);
    }
}
