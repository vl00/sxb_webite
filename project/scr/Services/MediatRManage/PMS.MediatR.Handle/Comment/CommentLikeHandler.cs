using MediatR;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Domain.IRepositories;
using PMS.MediatR.Events.Comment;
using ProductManagement.Framework.Cache.Redis;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PMS.MediatR.Handle.PaidQA
{
    /// <summary>
    /// 拒绝订单时所需的处理
    /// </summary>
    public class CommentLikeHandler : INotificationHandler<CommentLikeEvent>
    {
        IGiveLikeService _giveLikeService;
        IEasyRedisClient _easyRedisClient;
        ISchoolCommentService _schoolCommentService;
        IGiveLikeRepository _giveLikeRepository;
        public CommentLikeHandler(IGiveLikeService giveLikeService, IEasyRedisClient easyRedisClient, ISchoolCommentService schoolCommentService, IGiveLikeRepository giveLikeRepository)
        {
            _giveLikeRepository = giveLikeRepository;
            _schoolCommentService = schoolCommentService;
            _easyRedisClient = easyRedisClient;
            _giveLikeService = giveLikeService;
        }

        public async Task Handle(CommentLikeEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                if (notification?.GiveLike?.SourceId != Guid.Empty)
                {
                    var likeCount = _giveLikeService.AddLike(new CommentsManage.Domain.Entities.GiveLike
                    {
                        UserId = notification.GiveLike.UserId,
                        LikeType = notification.GiveLike.LikeType,
                        Channel = notification.GiveLike.Channel,
                        SourceId = notification.GiveLike.SourceId
                    }, out CommentsManage.Domain.Common.LikeStatus status);
                    var score = await _easyRedisClient.SortedSetScoreAsync("Comment:LikeCount", notification.GiveLike.SourceId.ToString());

                    switch (status)
                    {
                        case CommentsManage.Domain.Common.LikeStatus.Like:
                            if (score == null) { likeCount--; } else { likeCount = 0; }
                            await _easyRedisClient.SortedSetIncrementAsync("Comment:LikeCount", notification.GiveLike.SourceId.ToString(), likeCount + 1, StackExchange.Redis.CommandFlags.FireAndForget);
                            break;
                        default:
                            if (score == null) { likeCount++; } else { likeCount = 0; }
                            await _easyRedisClient.SortedSetIncrementAsync("Comment:LikeCount", notification.GiveLike.SourceId.ToString(), likeCount - 1, StackExchange.Redis.CommandFlags.FireAndForget);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
            }
        }
    }
}
