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
    public class SyncCommentAddHandle : IEventHandler<SyncCommentAddMessage>
    {
        private readonly ILogger<SyncCommentAddHandle> _logger;

        private readonly ITopicService _topicService;
        private readonly ITalentService _talentService;

        public SyncCommentAddHandle(ILogger<SyncCommentAddHandle> logger, ITopicService topicService, ITalentService talentService)
        {
            _logger = logger;
            _topicService = topicService;
            _talentService = talentService;
        }

        public Task Handle(SyncCommentAddMessage message)
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
        public void AddTopic(SyncCommentAddMessage message)
        {
            var isTalent = _talentService.IsTalent(message.UserId.ToString());
            if (!isTalent) return;

            var topicDto = new TopicAddDto();
            topicDto.UserId = message.UserId;
            topicDto.Content = $"我对【{message.SchoolName}】发表了点评，快来围观！";
            topicDto.Attachment = new TopicAddDto.AttachmentDto
            {
                AttchId = message.Id,
                Content = message.Content,
                AttachUrl = message.Url,
                Type = TopicType.Comment
            };

            var result = _topicService.AddAutoSyncTopic(topicDto);
            if (!result.Status)
            {
                _logger.LogError("达人同步点评话题失败, 返回信息:{0},数据:{1}", result.Msg, JsonConvert.SerializeObject(message));
            }
        }
    }
}
