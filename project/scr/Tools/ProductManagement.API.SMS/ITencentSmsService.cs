using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TencentCloud.Sms.V20190711.Models;

namespace ProductManagement.API.SMS
{
    public interface ITencentSmsService
    {
        Task<SendSmsResponse> SendSmsAsync(string[] phones, string templateId, string[] templateParamSet, App app = App.Sxb, Sign sign = Sign.上学帮);
        Task SendSmsAsync(string phone, string templateId, string[] templateParamSet, App app = App.Sxb, Sign sign = Sign.上学帮);
    }
}
