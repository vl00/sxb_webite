using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PMS.RabbitMQ.Message;
using PMS.TopicCircle.Application.Dtos;
using PMS.TopicCircle.Application.Services;
using PMS.UserManage.Application.IServices;
using ProductManagement.Framework.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static PMS.TopicCircle.Domain.Enum.GlobalEnum;

namespace PMS.RabbitMQ.Handle
{
    public class SyncArticleAddHandle : IEventHandler<SyncArticleAddMessage>
    {
        private readonly ILogger<SyncArticleAddHandle> _logger;

        private readonly ITopicService _topicService;
        private readonly ITalentService _talentService;

        public SyncArticleAddHandle(ILogger<SyncArticleAddHandle> logger, ITopicService topicService, ITalentService talentService)
        {
            _logger = logger;
            _topicService = topicService;
            _talentService = talentService;
        }

        public Task Handle(SyncArticleAddMessage message)
        {
            try
            {
                AddTopic(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "操作异常,数据:{0}", JsonConvert.SerializeObject(message));
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 达人发布点评, 同步到话题
        /// </summary>
        /// <param name="message"></param>
        public void AddTopic(SyncArticleAddMessage message)
        {
            var isTalent = _talentService.IsTalent(message.UserId.ToString());
            if (!isTalent) return;

            var topicDto = new TopicAddDto();
            topicDto.UserId = message.UserId;
            topicDto.Content = $"我新发表的原创干货文章，欢迎指点！";
            topicDto.Attachment = new TopicAddDto.AttachmentDto
            {
                AttchId = message.Id,
                Content = message.Title,
                AttachUrl = message.Url,
                Type = TopicType.Article
            };

            var result = _topicService.AddAutoSyncTopic(topicDto);
            if (!result.Status)
            {
                _logger.LogError("达人同步文章失败, 返回信息:{0},数据:{1}", result.Msg, JsonConvert.SerializeObject(message));
            }
        }
    }
}
