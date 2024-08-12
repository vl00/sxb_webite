using PMS.TopicCircle.Application.Dtos;
using PMS.TopicCircle.Domain.Dtos;
using PMS.TopicCircle.Domain.Entities;
using PMS.TopicCircle.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PMS.UserManage.Application;
using PMS.UserManage.Application.IServices;
using System.Threading.Tasks;

namespace PMS.TopicCircle.Application.Services
{
    public class CircleFollowerService : ApplicationService<CircleFollower>, ICircleFollowerService
    {
        ICircleFollowerRepository _repository;
        IUserService _userService;
        public CircleFollowerService(ICircleFollowerRepository repository,
            IUserService userService
            ) : base(repository)
        {
            this._repository = repository;
            this._userService = userService;
        }

        public AppServiceResultDto RemoveFollower(RemoveFollowerRequestDto input)
        {
            bool res = this._repository.Delete(input.UserId, input.CircleId);
            if (res)
            {
                return AppServiceResultDto.Success("移除成功");
            }
            else
            {
                return AppServiceResultDto.Failure("移除失败");
            }
        }

        public bool CheckIsFollow(CheckIsFollowRequestDto input)
        {
            CircleFollower follower = this._repository.GetBy("USERID =@userId AND CIRCLEID = @circleId", new { input.UserId, input.CircleId }).FirstOrDefault();
            return follower != null;
        }

        public AppServicePageResultDto<IEnumerable<CircleFollowerDetailDto>> GetFollowerDetail(AppServicePageRequestDto<GetFollowerDetailRequestDto> request)
        {
            IEnumerable<CircleFollowerDetailDto> dtos;
            IEnumerable<CircleFollower> followers = this._repository.ExcuteUSP_QUERYCIRCLEFOLLOWERDETAIL(request.input.CircleId);
            if (followers == null || !followers.Any())
            {
                dtos = new List<CircleFollowerDetailDto>();
                return AppServiceResultDto.Success(dtos, 0);
            }
            if (!string.IsNullOrEmpty(request.input.SearchContent))
            {
                followers = followers.Where(fl => fl.NickName?.Contains(request.input.SearchContent) ?? false);
            }
            switch (request.input.SortType)
            {
                case SortEnum.JoinDayAsc:
                    followers = followers.OrderBy(fl => fl.JoinDays);
                    break;
                case SortEnum.JoinDayDesc:
                    followers = followers.OrderByDescending(fl => fl.JoinDays);
                    break;
                case SortEnum.EndLogTime:
                    followers = followers.OrderByDescending(fl => fl.LoginTime);
                    break;
                default:
                    break;
            }
            dtos = followers.Skip(request.offset).Take(request.limit).Select<CircleFollower, CircleFollowerDetailDto>(follower => follower);
            int? totalFollowers = this._repository.GetFollowerCount(request.input.CircleId).FirstOrDefault()?.FollowerCount;
            return AppServiceResultDto.Success(dtos, totalFollowers.GetValueOrDefault());
        }

        public AppServicePageResultDto<IEnumerable<CircleFollowerDetailDto>> Search(AppServicePageRequestDto<CricleFollowerSearchRequestDto> input)
        {
            IEnumerable<CircleFollower> followers = this._repository.ExcuteUSP_SEARCHCIRCLEFOLLOWER(input.input.CircleId, input.input.NickName);
            IEnumerable<CircleFollowerDetailDto> dtos = followers.Select<CircleFollower, CircleFollowerDetailDto>(follower => follower);
            return AppServiceResultDto.Success(dtos, 0);
        }

        /// <summary>
        /// 获取话题圈的关注者数量
        /// </summary>
        /// <param name="circle"></param>
        /// <returns></returns>
        internal Circle GetFollowerCount(Circle circle)
        {
            CircleFollowerCountDto followerCount = this._repository.GetFollowerCount(circle.Id).FirstOrDefault();
            circle.FollowerCount = (followerCount?.FollowerCount).GetValueOrDefault();
            return circle;
        }

        /// <summary>
        /// 获取话题圈的关注者数量
        /// </summary>
        /// <param name="circle"></param>
        /// <returns></returns>
        internal IEnumerable<Circle> GetFollowerCount(IEnumerable<Circle> circles)
        {
            IEnumerable<CircleFollowerCountDto> followerCounts = this._repository.GetFollowerCount(circles.Select(c => c.Id).ToArray());
            foreach (var circle in circles)
            {
                circle.FollowerCount = (followerCounts.FirstOrDefault(fc => fc.CircleId == circle.Id)?.FollowerCount).GetValueOrDefault();
            }
            return circles;
        }

        /// <summary>
        /// 获取话题圈的关注者数量
        /// </summary>
        /// <param name="circleIds"></param>
        /// <returns></returns>
        public IEnumerable<CircleFollowerCountDto> GetFollowerCount(IEnumerable<Guid> circleIds)
        {
            IEnumerable<CircleFollowerCountDto> followerCounts = _repository.GetFollowerCount(circleIds.ToArray());
            return followerCounts;
        }

        /// <summary>
        /// 用户是否关注圈子
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="circleIds"></param>
        /// <returns></returns>
        public IEnumerable<CircleFollower> GetFollowers(Guid userId, IEnumerable<Guid> circleIds)
        {
            IEnumerable<CircleFollower> followers = _repository.GetBy(" UserId = @userId and CircleId in @circleIds ", new { userId, circleIds });
            return followers;
        }

        /// <summary>
        /// 删除圈子订阅关系
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="circleId"></param>
        /// <returns></returns>
        internal bool Delete(Guid userId, Guid circleId)
        {
            return this._repository.Delete(userId, circleId);
        }

        internal int GetNewFollowCounts(Guid circleID, DateTime? time)
        {
            return this._repository.GetNewFollowers(circleID, time);
        }

        public async Task<Dictionary<Guid, bool>> CheckIsFollow(IEnumerable<Guid> circleIDs, Guid? userID)
        {
            if (circleIDs?.Any() == true && userID.HasValue)
            {
                var finds = await _repository.CheckIsFollowCircle(circleIDs.Distinct(), userID.Value);
                if (finds?.Any() == true)
                {
                    return finds.ToDictionary(k => k.Key, v => v.Value);
                }
            }
            return new Dictionary<Guid, bool>();
        }
    }
}
