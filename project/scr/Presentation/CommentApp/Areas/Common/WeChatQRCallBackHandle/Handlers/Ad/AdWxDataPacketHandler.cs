using PMS.OperationPlateform.Application.IServices;
using PMS.TopicCircle.Application.Services;
using PMS.UserManage.Application.IServices;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model;
using Sxb.Web.Areas.Common.Models;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeChat;
using WeChat.Interface;
using IUserService = PMS.UserManage.Application.IServices.IUserService;

namespace Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers.Ad
{
    /// <summary>
    /// 扫码广告图片上的二维码, 关注后发送微信资料包
    /// </summary>
    public class AdWxDataPacketHandler : ISubscribeCallBackHandler
    {
        private readonly IWeChatAppClient _weChatAppClient;
        private readonly IKeyValueService _keyValueService;
        private readonly PMS.Infrastructure.Application.IService.IWeixinTemplateService _weixinTemplateService;
        private readonly IDataPacketService _dataPacketService;
        private readonly IUserService _userService;
        ICustomMsgService _customMsgService;
        public AdWxDataPacketHandler(IWeChatAppClient weChatAppClient, IKeyValueService keyValueService, PMS.Infrastructure.Application.IService.IWeixinTemplateService weixinTemplateService, IDataPacketService dataPacketService, IUserService userService
            , ICustomMsgService customMsgService)
        {
            _weChatAppClient = weChatAppClient;
            _keyValueService = keyValueService;
            _weixinTemplateService = weixinTemplateService;
            _dataPacketService = dataPacketService;
            _userService = userService;
            _customMsgService = customMsgService;
        }

        public async Task<ResponseResult> Process(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest)
        {
            var text = await GetTemplateTextAsync();
            if (string.IsNullOrWhiteSpace(text))
            {
                return ResponseResult.Failed("资料包关注回复模板为空");
            }

            TextCustomMsg msg = new TextCustomMsg()
            {
                ToUser = frequest.OpenId,
                content = text
            };
            var accessToken = await _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" });
            var result = await _customMsgService.Send(accessToken.token, msg);

            var userInfo = _userService.GetUserByWeixinOpenId(frequest.OpenId);
            if (userInfo != null && qrequest.DataId != null)
            {
                _dataPacketService.SubscribeWxCallback(qrequest.DataId.Value, userInfo.Id);
            }

            return new ResponseResult() { Succeed = result.Success };
        }

        /// <summary>
        /// 资料包关注回复
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetTemplateTextAsync()
        {
            var templateIdStr = _keyValueService.Get("weixin_data_packet_subscribe_auto_reply");
            if (!Guid.TryParse(templateIdStr, out Guid templateId))
            {
                return string.Empty;
            }
            return await _weixinTemplateService.GetTemplateTextFromCache(templateId);
        }
    }
}
