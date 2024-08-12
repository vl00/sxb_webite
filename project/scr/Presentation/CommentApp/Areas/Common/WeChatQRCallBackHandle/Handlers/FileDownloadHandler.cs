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
    public class FileDownloadHandler : ISubscribeCallBackHandler
    {
        ICapPublisher _capPublisher;
        ILogger<FileDownloadHandler> _logger;

        public FileDownloadHandler(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher ?? throw new ArgumentNullException(nameof(capPublisher));
        }

        public async Task<ResponseResult> Process(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest)
        {
            if (qrequest.DataUrl == null)
            {
                return ResponseResult.Failed("DataUrl is null");
            }
            try
            {
                var dataJson = JsonConvertExtension.TryDeserializeObject<FileDownloadDataJson>(qrequest.DataJson);

                await _capPublisher.PublishAsync(nameof(SendMsgIntegrationEvent)
                    , new SendMsgIntegrationEvent("FileDownloadMessage", frequest.OpenId, dataJson: null)
                    {
                        ExtraData = new Dictionary<string, string>(){
                            { "FilePath", qrequest.DataUrl },
                            { "FileName", dataJson.FileName }
                        }
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return ResponseResult.Success();
        }
    }
}
