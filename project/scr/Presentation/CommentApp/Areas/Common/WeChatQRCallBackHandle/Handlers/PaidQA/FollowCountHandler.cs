using MediatR;
using PMS.MediatR.Events.PaidQA;
using PMS.PaidQA.Application.Services;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Entities;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model;
using Sxb.Web.Areas.Common.Models;
using Sxb.Web.Response;
using System;
using System.Threading.Tasks;
using WeChat;
using WeChat.Interface;
using IUserService = PMS.UserManage.Application.IServices.IUserService;

namespace Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers.PaidQA
{

    public class FollowCountHandler : ISubscribeCallBackHandler
    {
        IWeChatAppClient _weChatAppClient;
        IAssessService _assessService;
        IUserService _userService;
        IMediator _mediator;
        ICustomMsgService _customMsgService;
        public FollowCountHandler(IWeChatAppClient weChatAppClient, IAssessService assessService
            , IUserService userService
            , IMediator mediator
            , ICustomMsgService customMsgService)
        {
            _mediator = mediator;
            _assessService = assessService;
            _userService = userService;
            _weChatAppClient = weChatAppClient;
            _customMsgService = customMsgService;
        }

        public async Task<ResponseResult> Process(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest)
        {
            var find = _assessService.Get(qrequest.DataId);
            UserInfo userInfo = null;
            if (find?.RecommendTalentUserID != Guid.Empty) userInfo = _userService.GetUserInfoDetail(find.RecommendTalentUserID);
            _mediator.Publish(new AssessStatisticsEvent(find.Type, 3, find.UserID)).GetAwaiter().GetResult();
            NewsCustomMsg msg = new NewsCustomMsg()
            {
                ToUser =frequest.OpenId,
                Url = qrequest.DataUrl,
                Description = $"您正在向达人【{userInfo?.NickName}】咨询，返回支付咨询费用，获取答案！",
                PicUrl = "https://cos.sxkid.com/images/navigationicon/cf90a312-3385-41d2-83c7-ea64d1d02ad5/da0a23eb-971b-41a0-98d8-90af75519f54.png",
                Title = "点击这里，继续向达人咨询"
            };
            var accessToken = await _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" });
            var result = await _customMsgService.Send(accessToken.token, msg);

            return new ResponseResult() { Succeed = result.Success };
        }

    }

}