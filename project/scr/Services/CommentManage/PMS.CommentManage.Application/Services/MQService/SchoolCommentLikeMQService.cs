using PMS.CommentsManage.Application.IServices.IMQService;
using PMS.CommentsManage.Domain.Entities;
using PMS.RabbitMQ.Message;
using ProductManagement.Framework.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.Services.MQService
{
    public class SchoolCommentLikeMQService : ISchoolCommentLikeMQService
    {
        private readonly IEventBus _eventBus;

        public SchoolCommentLikeMQService(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void SchoolCommentLike(GiveLike giveLike)
        {
            _eventBus.Publish(new SyncSchoolCommentLikeMessage(new SyncSchoolCommentLikeMessage.SyncSchoolCommentModel() {
                CommentId = giveLike.SourceId,
                LikeType = (int)giveLike.LikeType,
                UserId = giveLike.UserId,
                Channel = giveLike.Channel
            }));
        }
    }
}
