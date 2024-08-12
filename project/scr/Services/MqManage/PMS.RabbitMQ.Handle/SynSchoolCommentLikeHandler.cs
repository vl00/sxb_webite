using Microsoft.Extensions.Logging;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities;
using PMS.RabbitMQ.Message;
using ProductManagement.Framework.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static PMS.RabbitMQ.Message.SyncSchoolCommentLikeMessage;

namespace PMS.RabbitMQ.Handle
{
    public class SynSchoolCommentLikeHandler : IEventHandler<SyncSchoolCommentLikeMessage>
    {
        private readonly IGiveLikeService _likeservice;

        private readonly ILogger<SynSchoolCommentLikeHandler> _logger;

        public SynSchoolCommentLikeHandler(ILoggerFactory loggerFactory,
            IGiveLikeService likeservice)
        {
            _likeservice = likeservice;
            _logger = loggerFactory.CreateLogger<SynSchoolCommentLikeHandler>();
        }

        public Task Handle(SyncSchoolCommentLikeMessage message)
        {
            try
            {
                var model = message.SchoolCommentModel;
                GiveLike giveLike = new GiveLike
                {
                    UserId = model.UserId,
                    LikeType = (LikeType)model.LikeType,
                    SourceId = model.CommentId,
                    Channel = model.Channel
                };
                _likeservice.AddLike(giveLike, out LikeStatus status);
                //_logger.LogWarning($"点赞类型状态变更： userId：{model.UserId} 类型:{(int)giveLike.LikeType}，数据Id：{model.CommentId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "点评点赞更新失败：SourceId:{0} ; LikeType:{1};", message.SchoolCommentModel.CommentId, (LikeType)message.SchoolCommentModel.LikeType);
            }
            return Task.CompletedTask;
        }

    }
}
