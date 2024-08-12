using PMS.TopicCircle.Application.Dtos;
using PMS.TopicCircle.Domain.Dtos;
using PMS.TopicCircle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.TopicCircle.Application.Services
{
    public interface ICircleFollowerService : IApplicationService<CircleFollower>
    {
        /// <summary>
        /// 检查是否关注圈子
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        bool CheckIsFollow(CheckIsFollowRequestDto input);


        /// <summary>
        /// 移除圈子成员
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        AppServiceResultDto RemoveFollower(RemoveFollowerRequestDto input);



        /// <summary>
        /// 获取圈子的关注者详情列表
        /// </summary>
        /// <param name="circleId"></param>
        /// <returns></returns>
        AppServicePageResultDto<IEnumerable<CircleFollowerDetailDto>> GetFollowerDetail(AppServicePageRequestDto<GetFollowerDetailRequestDto> request);

        /// <summary>
        /// 基于数据库的简单关注者搜索
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        AppServicePageResultDto<IEnumerable<CircleFollowerDetailDto>> Search(AppServicePageRequestDto<CricleFollowerSearchRequestDto> input);

        /// <summary>
        /// 获取话题圈的关注者数量
        /// </summary>
        /// <param name="circleIds"></param>
        /// <returns></returns>
        IEnumerable<CircleFollowerCountDto> GetFollowerCount(IEnumerable<Guid> circleIds);


        IEnumerable<CircleFollower> GetFollowers(Guid userId, IEnumerable<Guid> circleIds);

        /// <summary>
        /// 检查是否关注了圈子
        /// </summary>
        /// <param name="CircleIDs"></param>
        /// <returns></returns>
        Task<Dictionary<Guid, bool>> CheckIsFollow(IEnumerable<Guid> CircleIDs, Guid? userID);
    }
}
