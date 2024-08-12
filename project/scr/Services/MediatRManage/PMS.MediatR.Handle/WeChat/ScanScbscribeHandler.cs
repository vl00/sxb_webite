using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PMS.Infrastructure.Application.IService;
using PMS.Infrastructure.Domain.Entities;
using PMS.MediatR.Events.WeChat;
using PMS.UserManage.Application.IServices;

namespace PMS.MediatR.Handle.WeChat
{
    public class ScanScbscribeHandler : INotificationHandler<ScanSubscribeEvent>
    {
        private readonly ILogger<ScanScbscribeHandler> _logger;

        private readonly IUserService _userService;
        private readonly IWeixinSubscribeLogService _weixinSubscribeLogService;

        public ScanScbscribeHandler(ILogger<ScanScbscribeHandler> logger, IUserService userService, IWeixinSubscribeLogService weixinSubscribeLogService)
        {
            _logger = logger;
            _userService = userService;
            _weixinSubscribeLogService = weixinSubscribeLogService;
        }

        public Task Handle(ScanSubscribeEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                return Main(notification);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "错误,data={0}", JsonConvert.SerializeObject(notification));
            }
            return Task.CompletedTask;
        }

        private Task Main(ScanSubscribeEvent notification)
        {
            //var openWeiXin = _userService.GetOpenWeixin(notification.OpenId);
            ////未关注
            //if (openWeiXin == null)
            //{
            //    return Task.CompletedTask;
            //}

            //DataJson
            //{"fw":"","surl":"https://m3.sxkid.com/school_detail_wechat/data/eid=092D223A-5D02-48FF-AE04-FD1C9683EB5D_type=5_ref=15","eid":"092d223a-5d02-48ff-ae04-fd1c9683eb5d"}
            //dataJson: "{\"pageUrl\":\"https://m3.sxkid.com/school_detail_wechat/data/eid=092D223A-5D02-48FF-AE04-FD1C9683EB5D_type=5_ref=15\",\"eid\":\"092D223A-5D02-48FF-AE04-FD1C9683EB5D\"}"
            var data = JsonConvert.DeserializeObject<DataJson>(notification.DataJson);
            string fromWhere = data?.PageUrl;
            string channel = data?.Fw;
            if (string.IsNullOrWhiteSpace(channel))
                channel = GetChannel(fromWhere);

            var openWx = _userService.GetOpenWeixin(notification.OpenId);
            //是否是第一次关注
            var isFirstSubscribe = openWx == null || openWx.LastUnSubscribeTime == null;

            WeixinSubscribeLog log = new WeixinSubscribeLog()
            {
                AppId = notification.AppId,
                OpenId = notification.OpenId,
                FromWhere = fromWhere,
                Channel = channel,
                CreateTime = DateTime.Now,
                DataId = notification.DataId,
                DataUrl = notification.DataUrl,
                DataJson = notification.DataJson,
                Type = notification.Type,
                WeChatEvent = notification.WeChatEvent.ToString(),
                IsFirstSubscribe = isFirstSubscribe

            };
            return _weixinSubscribeLogService.AddAsync(log);
        }

        private string GetChannel(string fromWhere)
        {
            try
            {
                var uri = new Uri(fromWhere);
                return uri.QueryValues("fw");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "fromWhere:{0}", fromWhere);
            }
            return string.Empty;
        }

        private class DataJson
        {
            public string Fw { get; set; }
            public string PageUrl { get; set; }
            //public string Surl { get; set; }
            public string Eid { get; set; }
        }
    }
}
