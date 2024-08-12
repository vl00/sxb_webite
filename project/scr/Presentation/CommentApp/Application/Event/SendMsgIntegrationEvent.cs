using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using WeChat.Model;

namespace Sxb.Web.Application
{
    public class SendMsgIntegrationEvent
    {
        private SendMsgIntegrationEvent(string templateSettingCode, string dataJson)
        {
            TemplateSettingCode = templateSettingCode ?? throw new ArgumentNullException(nameof(templateSettingCode));
            ExtraData = TryToDic(dataJson);
        }

        public SendMsgIntegrationEvent(string templateSettingCode, string openId, string dataJson) : this(templateSettingCode, dataJson)
        {
            OpenId = openId;
        }

        public SendMsgIntegrationEvent(string templateSettingCode, Guid userId, string dataJson) : this(templateSettingCode, dataJson)
        {
            UserId = userId;
        }

        public string TemplateSettingCode { get; set; }

        public Guid? UserId { get; set; }

        public string OpenId { get; set; }

        public Dictionary<string, string> ExtraData { get; set; }

        private static Dictionary<string, string> TryToDic(string dataJson)
        {
            if (!string.IsNullOrWhiteSpace(dataJson))
            {
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(dataJson);
            }
            return new Dictionary<string, string>();
        }
    }
}
