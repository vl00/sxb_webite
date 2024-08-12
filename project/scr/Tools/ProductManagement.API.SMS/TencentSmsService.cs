using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TencentCloud.Sms.V20190711;
using TencentCloud.Sms.V20190711.Models;

namespace ProductManagement.API.SMS
{
    public class TencentSmsService:ITencentSmsService
    {
        SmsClient _tencentSmsClient;
        public TencentSmsService(SmsClient tencentSmsClient)
        {
            _tencentSmsClient = tencentSmsClient;
        }
        public  async Task<SendSmsResponse> SendSmsAsync(string[] phones, string templateId, string[] templateParamSet, App app = App.Sxb, Sign sign = Sign.上学帮)
        {
            var request = new SendSmsRequest()
            {
                PhoneNumberSet = phones,
                SmsSdkAppid = IServiceCollectionExtend.Configuration.GetSection(string.Format(Keys.SMSAppId, app.ToString())).Value,
                TemplateID = templateId,
                TemplateParamSet = templateParamSet,
                Sign = sign.ToString()
            };
            var response = await _tencentSmsClient.SendSms(request);
            return response;
        }

        public  async Task SendSmsAsync( string phone, string templateId, string[] templateParamSet, App app = App.Sxb, Sign sign = Sign.上学帮)
        {
            var request = new SendSmsRequest()
            {
                PhoneNumberSet = new[] { phone },
                SmsSdkAppid = IServiceCollectionExtend.Configuration.GetSection(string.Format(Keys.SMSAppId, app.ToString())).Value,
                TemplateID = templateId,
                TemplateParamSet = templateParamSet,
                Sign = sign.ToString()
            };
            var response = await _tencentSmsClient.SendSms(request);
        }
    }
}
