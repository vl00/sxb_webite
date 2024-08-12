using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PMS.CommentsManage.Application.Services.Like
{
    public class GiveLikeService : IGiveLikeService
    {
        private ISchoolCommentReplyRepository _replyRepository;
        private IGiveLikeRepository _repository;


        public GiveLikeService(IGiveLikeRepository repository, ISchoolCommentReplyRepository replyRepository)
        {
            _repository = repository;
            _replyRepository = replyRepository;
        }

        public int AddLike(GiveLike giveLike, out LikeStatus status)
        {
            return _repository.AddLike(giveLike, out status);
        }

        public bool CheckCurrentUserIsLike(Guid UserId, Guid SourceId)
        {
            return _repository.CheckCurrentUserIsLike(UserId, SourceId);
        }

        public List<GiveLike> GiveLikeEntitys(Expression<Func<GiveLike, bool>> where)
        {
            return _repository.GiveLikeEntitys(where);
        }

        /// <summary>
        /// 批量获取是否点赞
        /// </summary>
        /// <returns>The current user is like bulk.</returns>
        /// <param name="UserId">User identifier.</param>
        /// <param name="SourceIds">Source identifiers.</param>
        public List<LikeDto> CheckCurrentUserIsLikeBulk(Guid UserId, List<Guid> SourceIds)
        {
            var list = _repository.CheckCurrentUserIsLikeBulk(UserId, SourceIds);

            return SourceIds.Select(q => new LikeDto
            {
                SourceId = q,
                IsLike = list.Exists(p => p.SourceId == q)
            }).ToList();
        }

        /// <summary>
        /// 修改回复的回复统计总数
        /// </summary>
        /// <param name="ReplayId"></param>
        /// <param name="operaValue"></param>
        /// <param name="Field"></param>
        /// <returns></returns>
        public int UpdateCommentReplyLikeorReplayCount(Guid ReplayId)
        {
            return _replyRepository.UpdateCommentReplyLikeorReplayCount(ReplayId, +1, false);
        }

        public LikeCommentAndAnswerTotal CommentAndAnswerTotals(Guid UserId)
        {
            return _repository.CommentAndAnswerTotals(UserId);
        }

        public LikeCommentAndAnswerTotal StatisticalSum(Guid UserId)
        {
            return _repository.StatisticalSum(UserId);
        }

        public List<Guid> CheckLike(List<Guid> DataSourceId, Guid UserId)
        {
            if (!DataSourceId.Any())
            {
                return new List<Guid>();
            }
            return _repository.CheckLike(DataSourceId, UserId)?.Select(x => x.SourceId).ToList();
        }

        public int GetLikeCount(Guid dataID)
        {
            if (dataID == Guid.Empty) return 0;
            return _repository.GetLikeCount(dataID);
        }
    }
}
