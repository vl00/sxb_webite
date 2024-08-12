using Microsoft.Extensions.Options;
using PMS.UserManage.Application.IServices;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model;
using ProductManagement.Infrastructure.Configs;
using Sxb.Web.Areas.Common.Models;
using Sxb.Web.Response;
using Sxb.Web.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeChat;
using WeChat.Interface;
using IUserService = PMS.UserManage.Application.IServices.IUserService;

namespace Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers.PaidQA
{

    public class PaidQAAskAtOnceHandle : ISubscribeCallBackHandler
    {
        IWeChatAppClient _weChatAppClient;
        PaidQAOption _paidQAOption;
        IUserService _userService;
        ICustomMsgService _customMsgService;
        public PaidQAAskAtOnceHandle(IServiceProvider serviceProvider, ICustomMsgService customMsgService)
        {
            _weChatAppClient = (serviceProvider.GetService(typeof(IWeChatAppClient)) as IWeChatAppClient);
            _userService = (serviceProvider.GetService(typeof(IUserService)) as IUserService);
            _paidQAOption = (serviceProvider.GetService(typeof(IOptions<PaidQAOption>)) as IOptions<PaidQAOption>).Value;
            _customMsgService = customMsgService;
        }

        public async Task<ResponseResult> Process(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest)
        {
            var userInfo = _userService.GetUserInfo(qrequest.DataId.GetValueOrDefault());

            ////高招会
            //CustomMsgSender customMsgSenderUniversity = new NewsCustomMsgSender()
            //{
            //    Url = _paidQAOption.CustomMsgSetting.UniversityAskAtOnceTips.RedirectUrl.Replace("{mHost}", ConfigHelper.GetHost()),
            //    Description = _paidQAOption.CustomMsgSetting.UniversityAskAtOnceTips.Description,
            //    PicUrl = userInfo.HeadImgUrl,
            //    Title = _paidQAOption.CustomMsgSetting.UniversityAskAtOnceTips.Title
            //};

            NewsCustomMsg msg = new NewsCustomMsg()
            {
                ToUser = frequest.OpenId,
                Url = qrequest.DataUrl,
                Description = _paidQAOption.CustomMsgSetting.AskAtOnceTips.Description.Replace("{NickName}", userInfo.NickName),
                PicUrl = userInfo.HeadImgUrl,
                Title = _paidQAOption.CustomMsgSetting.AskAtOnceTips.Title
            };

            var accessToken = await _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" });

            //高招会
            //var resultUniversity = await customMsgSenderUniversity.Send(accessToken.token, frequest.OpenId);
            var result = await _customMsgService.Send(accessToken.token, msg);

            return new ResponseResult() { Succeed = result.Success  };
        }

    }

}