using PMS.Infrastructure.Application.IService;
using ProductManagement.API.Http.Interface;
using Sxb.Web.Areas.Common.Models;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using Newtonsoft.Json;
using WeChat.Interface;
using Sxb.Web.Utils;
using ProductManagement.Framework.Foundation;
using Sxb.Web.Areas.Common.WeChatQRCallBackHandle.DataJson;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using DotNetCore.CAP;
using Sxb.Web.Application;
using Microsoft.Extensions.Logging;

namespace Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers
{
    public class SchoolAssessHandler : ISubscribeCallBackHandler
    {
        ICapPublisher _capPublisher;
        ILogger<SchoolAssessHandler> _logger;

        public SchoolAssessHandler(ICapPublisher capPublisher, ILogger<SchoolAssessHandler> logger)
        {
            _capPublisher = capPublisher ?? throw new ArgumentNullException(nameof(capPublisher));
            _logger = logger;
        }

        public async Task<ResponseResult> Process(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest)
        {
            _logger.LogDebug("SchoolAssessHandler,qrequest={0},frequest={1}", JsonConvert.SerializeObject(qrequest), JsonConvert.SerializeObject(frequest));
            try
            {
                var data = JsonConvertExtension.TryDeserializeObject(qrequest.DataJson, new SchoolAssessDataJson());

                if (data.IsGuangZhouRegister)
                {
                    await _capPublisher.PublishAsync(nameof(SendMsgIntegrationEvent)
                        , new SendMsgIntegrationEvent("SendAssessMessage_GuangZhou", frequest.OpenId, qrequest.DataJson));
                }
                else
                {
                    await _capPublisher.PublishAsync(nameof(SendMsgIntegrationEvent)
                        , new SendMsgIntegrationEvent("SendAssessMessage", frequest.OpenId, qrequest.DataJson));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return ResponseResult.Success();
        }
    }
}
