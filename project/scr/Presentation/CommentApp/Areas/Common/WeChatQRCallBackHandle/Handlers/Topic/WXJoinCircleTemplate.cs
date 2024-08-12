using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PMS.TopicCircle.Application.Services;
using PMS.UserManage.Application.ModelDto;
using ProductManagement.API.Http.Interface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using WeChat;
using WeChat.Interface;
using WeChat.Model;

namespace Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers.Topic
{
    /// <summary>
    /// 针对JoinCircle这个微信模板封装的抽象类
    /// </summary>
    public abstract class WXJoinCircleTemplate
    {
        protected ILogger logger;
        protected IKeyValueService _keyValueService;
        protected string templateId;
        protected TemplateDataFiled first;
        protected TemplateDataFiled keyword1;
        protected TemplateDataFiled keyword2;
        protected TemplateDataFiled remark;
        protected IWeChatAppClient _weChatAppClient;
        ITemplateMessageService  _templateMessageService;
        public WXJoinCircleTemplate(IConfiguration configuration
            , IKeyValueService keyValueService
            , ILogger logger
            , IWeChatAppClient weChatAppClient
            , ITemplateMessageService templateMessageService)
        {
            this._keyValueService = keyValueService;
            this._weChatAppClient = weChatAppClient;
            this.logger = logger;
            _templateMessageService = templateMessageService;
            var wxtemplatesSection = configuration.GetSection("wxtemplates");
            var joincircleTemplateConfig = wxtemplatesSection.GetSection("joincircle");
            templateId = joincircleTemplateConfig["templateId"];
            var firstSection = joincircleTemplateConfig.GetSection("first");
            var keyword1Section = joincircleTemplateConfig.GetSection("keyword1");
            var keyword2Section = joincircleTemplateConfig.GetSection("keyword2");
            var remarkSection = joincircleTemplateConfig.GetSection("remark");
            first = new TemplateDataFiled()
            {
                Filed = "first",
                Value = firstSection["value"],
                Color = firstSection["color"] ?? "#000000"
            };
            keyword1 = new TemplateDataFiled()
            {
                Filed = "keyword1",
                Value = keyword1Section["value"],
                Color = keyword1Section["color"] ?? "#000000"
            };
            keyword2 = new TemplateDataFiled()
            {
                Filed = "keyword2",
                Value = keyword2Section["value"],
                Color = keyword2Section["color"] ?? "#000000"
            };
            remark = new TemplateDataFiled()
            {
                Filed = "remark",
                Value = remarkSection["value"],
                Color = remarkSection["color"] ?? "#000000"
            };

        }


        public async Task SendTemplateMessage(string name, string url, string talentName, string openId,string senderName,string appId=null,string pagePath=null)
        {

            try
            {
                var accessToken = await _weChatAppClient.GetAccessToken(new ProductManagement.API.Http.Model.WeChatGetAccessTokenRequest() { App = "fwh" });
                SendTemplateRequest sendTemplateRequest = new SendTemplateRequest(openId, templateId);
                sendTemplateRequest.Url = url;
                if (!string.IsNullOrEmpty(appId))
                {
                    sendTemplateRequest.SetMiniprogram(appId, pagePath);
                }
                first.Value = first.Value.Replace("{CircleName}", name).Replace("{TalentName}", talentName);
                keyword1.Value = keyword1.Value.Replace("{NickName}", senderName);
                keyword2.Value = keyword2.Value.Replace("{Date}", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss"));
                remark.Value = remark.Value.Replace("{Name}", name);
                sendTemplateRequest.SetData(first, keyword1, keyword2, remark);
                SendTemplateResponse sendResponse = await _templateMessageService.SendAsync(accessToken.token, sendTemplateRequest);

            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, null);
            }
        }
    }
}
