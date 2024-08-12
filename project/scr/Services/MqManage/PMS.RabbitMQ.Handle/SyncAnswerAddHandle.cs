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
    public class SyncAnswerAddHandle : IEventHandler<SyncAnswerAddMessage>
    {
        private readonly ILogger<SyncAnswerAddHandle> _logger;

        private readonly ITopicService _topicService;
        private readonly ITalentService _talentService;

        public SyncAnswerAddHandle(ILogger<SyncAnswerAddHandle> logger, ITopicService topicService, ITalentService talentService)
        {
            _logger = logger;
            _topicService = topicService;
            _talentService = talentService;
        }

        public Task Handle(SyncAnswerAddMessage message)
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
        public void AddTopic(SyncAnswerAddMessage message)
        {
            var isTalent = _talentService.IsTalent(message.UserId.ToString());
            if (!isTalent) return;

            var topicDto = new TopicAddDto();
            topicDto.UserId = message.UserId;
            topicDto.Content = $"我回答了关于【{message.SchoolName}】的问题，快来围观！";
            topicDto.Attachment = new TopicAddDto.AttachmentDto
            {
                AttchId = message.Id,
                Content = message.QuestionContent, //使用问题内容
                AttachUrl = $"/question/Reply/{message.Id}",
                Type = TopicType.Answer
            };

            var result = _topicService.AddAutoSyncTopic(topicDto);
            if (!result.Status)
            {
                _logger.LogError("达人同步回答失败, 返回信息:{0},数据:{1}", result.Msg, JsonConvert.SerializeObject(message));
            }
        }
    }
}
