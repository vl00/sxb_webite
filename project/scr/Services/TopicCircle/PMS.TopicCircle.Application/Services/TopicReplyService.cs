using AutoMapper;
using Microsoft.EntityFrameworkCore.Internal;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.TopicCircle.Application.Dtos;
using PMS.TopicCircle.Domain.Entities;
using PMS.TopicCircle.Domain.Repositories;
using PMS.UserManage.Domain.IRepositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using ProductManagement.Infrastructure.AppService;
using ProductManagement.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.TopicCircle.Application.Services
{
    public class TopicReplyService : ApplicationService<TopicReply>, ITopicReplyService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        private readonly ICircleRepository _circleRepository;
        private readonly ITopicRepository _topicRepository;
        private readonly ITopicReplyRepository _topicReplyRepository;
        private readonly ITopicReplyLikeRepository _topicReplyLikeRepository;

        private readonly IUserRepository _userRepository;

        /// <summary>
        /// 以及评论. 默认取出的二级评论数
        /// </summary>
        public static int TOP_REPLY_COUNT = 5;

        public TopicReplyService(TopicCircleDBContext unitOfWork, ITopicRepository topicRepository, ITopicReplyRepository topicReplyRepository, ICircleRepository circleRepository, ITopicReplyLikeRepository topicReplyLikeRepository, IMapper mapper, IUserRepository userRepository) : base(topicReplyRepository)
        {
            _unitOfWork = unitOfWork;
            _topicRepository = topicRepository;
            _topicReplyRepository = topicReplyRepository;
            _circleRepository = circleRepository;
            _topicReplyLikeRepository = topicReplyLikeRepository;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        /// <summary>
        /// 获取顶级评论, 及其前5条Children评论
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PaginationModel<TopicReplyDto> GetPaginationByTopic(Guid? topicId, int pageIndex, int pageSize, bool createTimeDesc = false, int topReplyCount = 5)
        {
            //一级评论
            var topicReplies = _topicReplyRepository.GetPagination(topicId, null, null, 1, null, null, pageIndex, pageSize, createTimeDesc);
            var data = _mapper.Map<List<TopicReplyDto>>(topicReplies.data);

            data.ForEach(reply =>
            {
                var firstParentId = reply.Id;
                var topReplies = _topicReplyRepository.GetPagination(topicId, null, firstParentId, null, null, null, 1, topReplyCount, createTimeDesc);
                reply.Children = _mapper.Map<List<TopicReplyDto>>(topReplies.data);
                reply.ChildrenTotal = topReplies.total;
            });

            return PaginationModel<TopicReplyDto>.Build(data, topicReplies.total);
        }

        /// <summary>
        /// 获取评论的回复（分页列表）
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="firstParentId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PaginationModel<TopicReplyDto> GetChildren(Guid? topicId, Guid? firstParentId, int pageIndex, int pageSize)
        {
            //二以后评论
            long offset = (pageIndex - 1) * pageSize;
            var topicReplies = _topicReplyRepository.GetPaginationByOffset(topicId, null, firstParentId, null, null, null, offset, pageSize);
            var data = _mapper.Map<List<TopicReplyDto>>(topicReplies.data);
            return PaginationModel<TopicReplyDto>.Build(data, topicReplies.total);
        }

        /// <summary>
        /// 获取话题的前n条评论
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public List<TopicReplyDto> GetTopTopicReplies(Guid topicId, int size)
        {
            var topicReplies = _topicReplyRepository.GetTopList(new List<Guid>() { topicId }, size);
            var dto = _mapper.Map<List<TopicReplyDto>>(topicReplies);
            return dto;
        }

        /// <summary>
        /// 获取话题的所有评论(Children二级嵌套)
        /// </summary>
        /// <param name="topicId"></param>
        /// <returns></returns>
        public List<TopicReplyDto> GetTopicReplies(Guid topicId)
        {
            var replies = _topicReplyRepository.GetList(topicId).ToList();
            var dto = _mapper.Map<List<TopicReplyDto>>(replies);
            //评论
            var repliesFirst = dto.Where(s => s.Depth == 1).ToList();

            //评论的评论
            repliesFirst.ForEach(s => s.Children = dto.Where(t => t.FirstParentId == s.Id).ToList());
            return repliesFirst;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="topicReplyDto"></param>
        /// <returns></returns>
        public AppServiceResultDto<object> Add(TopicReplyAddDto topicReplyDto)
        {
            var topicId = topicReplyDto.TopicId;
            if (topicReplyDto.ParentId == null || topicReplyDto.ParentId == Guid.Empty)
                topicReplyDto.ParentId = topicId;

            Topic topic = _topicRepository.Get(topicId);
            if (topic == null)
                return AppServiceResultDto.Failure<object>("话题已关闭");

            var user = _userRepository.GetUserInfo(topicReplyDto.UserId);
            if (user == null)
                return AppServiceResultDto.Failure<object>("请先登录");

            TopicReply parent = _topicReplyRepository.Get(topicReplyDto.ParentId);
            if (parent == null)
                return AppServiceResultDto.Failure<object>("回复的评论已关闭");

            //回复层级+1
            int depth = parent.Depth + 1;
            Guid? firstParentId = null;
            if (depth == 2)
                firstParentId = parent.Id;
            else if (depth > 2)
                firstParentId = parent.FirstParentId;

            //评论的回复,  不能回复自己
            if (depth > 1 && parent.Creator == topicReplyDto.UserId)
                return AppServiceResultDto.Failure<object>("您不能回复自己的帖子");

            Guid? parentId = null, parentUserId = null;
            if (parent.Depth != 0)
            {
                parentId = parent.Id;
                parentUserId = parent.Creator;
            }

            Guid id = Guid.NewGuid();
            //构建话题评论
            TopicReply topicReply = new TopicReply()
            {
                Id = id,
                Depth = depth,
                Content = topicReplyDto.Content,
                TopicId = topicId,
                ParentId = parentId,
                ParentUserId = parentUserId,
                FirstParentId = firstParentId,
                Creator = topicReplyDto.UserId,
                Updator = topicReplyDto.UserId,
            };

            _unitOfWork.BeginTransaction();

            //添加评论
            _topicReplyRepository.Add(topicReply);
            //话题回复数+1, 更新话题回复时间
            _topicRepository.UpdateReplyTotal(topicId, topic.ReplyCount + 1, DateTime.Now);

            _unitOfWork.Commit();

            return AppServiceResultDto.Success<object>(new { Id = id, UserName = user.NickName, user.HeadImgUrl });
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="topicReplyDto"></param>
        /// <returns></returns>
        public AppServiceResultDto Edit(TopicReplyAddDto topicReplyDto)
        {
            TopicReply topicReply = _topicReplyRepository.Get(topicReplyDto.Id);
            if (topicReply == null)
                return AppServiceResultDto.Failure("回复已关闭");

            topicReply.Content = topicReplyDto.Content;
            topicReply.Updator = topicReplyDto.UserId;
            //修改评论
            _topicReplyRepository.Update(topicReply);

            return AppServiceResultDto.Success();
        }

        /// <summary>
        /// 点赞
        /// </summary>
        /// <param name="topicReplyDto"></param>
        /// <returns></returns>
        public AppServiceResultDto Like(Guid id, Guid userId)
        {
            TopicReply reply = _topicReplyRepository.Get(id);
            if (reply == null)
                return AppServiceResultDto.Failure("评论已关闭");
            Topic topic = _topicRepository.Get(reply.TopicId);
            if (topic == null)
                return AppServiceResultDto.Failure("话题已关闭");


            _unitOfWork.BeginTransaction();

            var like = _topicReplyLikeRepository.GetUow(userId, id);
            if (like == null)
            {
                //点赞
                like = new TopicReplyLike()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    TopicReplyId = id,
                    Status = 1
                };
                _topicReplyLikeRepository.Add(like);
            }
            else
            {
                //切换点赞   点赞/取消点赞
                like.Status = like.Status == 1 ? 0 : 1;
                _topicReplyLikeRepository.Update(like);
            }

            int diff = like.Status == 1 ? 1 : -1;
            if (reply.Depth == 0)
            {
                //修改话题点赞数
                _topicRepository.UpdateLikeTotal(topic.Id, topic.LikeCount + diff);
            }

            //修改话题评论点赞数
            _topicReplyRepository.UpdateLikeTotal(reply.Id, reply.LikeCount + diff);

            _unitOfWork.Commit();
            return AppServiceResultDto.Success();
        }

        public Dictionary<Guid, bool> GetIsLike(Guid userID, IEnumerable<Guid> topicReplyIDs)
        {
            if (userID == default || topicReplyIDs == null || !topicReplyIDs.Any()) return null;
            var finds = _topicReplyLikeRepository.GetIdList(topicReplyIDs, userID);
            if (finds?.Any() == true)
            {
                var result = new Dictionary<Guid, bool>();
                foreach (var item in topicReplyIDs)
                {
                    result[item] = finds.Contains(item);
                }
                return result;
            }
            return new Dictionary<Guid, bool>();
        }

        public IEnumerable<TopicReplyDto> GetLevelReplies(Guid replyID, int? maxDepth = null)
        {
            var replies = _topicReplyRepository.GetLevelReplies(replyID, maxDepth);
            var dto = _mapper.Map<List<TopicReplyDto>>(replies);
            return dto.OrderBy(k => k.Depth);
        }

        public async Task<IEnumerable<TopicReplyDto>> GetLevelRepliesEx(Guid replyID, int limit = 7)
        {
            var replies = await _topicReplyRepository.GetLevelRepliesEx(replyID, limit);
            if (replies?.Any() == true)
            {
                var dto = _mapper.Map<List<TopicReplyDto>>(replies);
                return dto.OrderBy(k => k.Depth);
            }
            return new List<TopicReplyDto>();
        }
    }
}
