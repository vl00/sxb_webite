using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Sxb.Web.Application;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Application.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sxb.Web.Areas.Common.WeChatQRCallBackHandle.DataJson;
using ProductManagement.Framework.Foundation;

namespace Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers
{
    public class SubscribeRemindHandler : ISubscribeCallBackHandler
    {
        ICapPublisher _capPublisher;
        ISubscribeRemindService _subscribeRemindService;
        IUserService _userService;
        ILogger<SubscribeRemindHandler> _logger;

        public SubscribeRemindHandler(ICapPublisher capPublisher, ISubscribeRemindService subscribeRemindService, ILogger<SubscribeRemindHandler> logger, IUserService userService)
        {
            _capPublisher = capPublisher ?? throw new ArgumentNullException(nameof(capPublisher));
            _subscribeRemindService = subscribeRemindService;
            _logger = logger;
            _userService = userService;
        }

        public async Task<ResponseResult> Process(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest)
        {
            var resp = await Handle(qrequest, frequest);
            if (!resp.Succeed)
            {
                _logger.LogError("政策大卡订阅提醒错误,errmsg={0},qrequest={1},frequest={1}"
                    , resp.Msg, JsonConvert.SerializeObject(qrequest), JsonConvert.SerializeObject(frequest));
            }
            return resp;
        }

        private async Task<ResponseResult> Handle(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest)
        {
            if (qrequest.DataId == null)
            {
                return ResponseResult.Failed("dataId is null");
            }

            var userId = _userService.GetOpenWeixin(frequest.OpenId)?.UserId;
            if (userId == null)
            {
                return ResponseResult.Failed(" userId is null ");
            }

            var dataJson = JsonConvertExtension.TryDeserializeObject<SubscribeRemindDataJson>(qrequest.DataJson);



            var dto = new SubscribeRemindAddDto()
            {
                GroupCode = SubscribeRemindAddDto.WeChatRecruitGroupCode,
                SubjectId = qrequest.DataId.Value,
                UserId = userId.Value,
                StartTime = dataJson.StartTime,
                EndTime = dataJson.EndTime,
            };

            //frequest.Event == WeChat.Model.WeChatEventEnum.subscribe
            if (!_subscribeRemindService.Exists(dto.GroupCode, dto.SubjectId, dto.UserId))
            {
                //订阅日程
                var ret = _subscribeRemindService.Add(dto);
                if (!ret)
                {
                    return ResponseResult.Failed("add SubscribeRemind fail ");
                }
            }

            var @event = new SendMsgIntegrationEvent("SubscribeRemindSuccessMessage", frequest.OpenId, qrequest.DataJson);
            if (!@event.ExtraData.ContainsKey("DataUrl"))
            {
                @event.ExtraData.Add("DataUrl", qrequest.DataUrl);
            }

            //发送订阅成功消息
            await _capPublisher.PublishAsync(nameof(SendMsgIntegrationEvent), @event);
            return ResponseResult.Success();
        }
    }
}
