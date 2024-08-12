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
    public class SyncLectureAddHandle : IEventHandler<SyncLectureAddMessage>
    {
        private readonly ILogger<SyncLectureAddHandle> _logger;

        private readonly ITopicService _topicService;
        private readonly ITalentService _talentService;

        public SyncLectureAddHandle(ILogger<SyncLectureAddHandle> logger, ITopicService topicService, ITalentService talentService)
        {
            _logger = logger;
            _topicService = topicService;
            _talentService = talentService;
        }

        public Task Handle(SyncLectureAddMessage message)
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
        /// 审核达人直播成功, 同步到话题
        /// </summary>
        /// <param name="message"></param>
        public void AddTopic(SyncLectureAddMessage message)
        {
            var isTalent = _talentService.IsTalent(message.UserId.ToString());
            if (!isTalent) return;

            var topicDto = new TopicAddDto();
            topicDto.UserId = message.UserId;

            //我将在7月22日16:00举行直播，欢迎参加！
            var time = message.StartTime.ToString("M月d日 HH:mm");
            topicDto.Content = $"我将在{time}举行直播，欢迎参加！";
            topicDto.Attachment = new TopicAddDto.AttachmentDto
            {
                AttchId = message.Id,
                Content = message.Content,
                Type = TopicType.Live,
                AttachUrl = $"/live/client/livedetail.html?showtype=1&contentid={message.Id}"
            };

            var result = _topicService.AddAutoSyncTopic(topicDto);
            if (!result.Status)
            {
                _logger.LogError("达人同步直播失败, 返回信息:{0},数据:{1}", result.Msg, JsonConvert.SerializeObject(message));
            }
        }
    }
}
