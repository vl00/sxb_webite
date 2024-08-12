using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PMS.RabbitMQ.Message;
using PMS.TopicCircle.Application.Services;
using ProductManagement.Framework.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.RabbitMQ.Handle
{
    public class SyncTopicLikeHandler : IEventHandler<SyncTopicLikeMessage>
    {
        public readonly ILogger<SyncTopicLikeHandler> _logger;
        public readonly ITopicReplyService _topicReplyService;

        public SyncTopicLikeHandler(ILogger<SyncTopicLikeHandler> logger, ITopicReplyService topicReplyService)
        {
            _logger = logger;
            _topicReplyService = topicReplyService;
        }

        public Task Handle(SyncTopicLikeMessage message)
        {
            try
            {
                var result = _topicReplyService.Like(message.Id, message.UserId);
                if (!result.Status)
                {
                    _logger.LogError("点赞操作失败, 返回信息:{0},数据:{1}", result.Msg, JsonConvert.SerializeObject(message));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "点赞操作异常,数据:{0}", JsonConvert.SerializeObject(message));
            }
            return Task.CompletedTask;
        }
    }
}
