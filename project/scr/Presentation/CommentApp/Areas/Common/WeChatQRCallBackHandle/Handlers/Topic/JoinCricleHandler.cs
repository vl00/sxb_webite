using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PMS.Infrastructure.Application.IService;
using PMS.TopicCircle.Application.Services;
using PMS.TopicCircle.Domain.Entities;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Application.ModelDto;
using PMS.UserManage.Domain.Entities;
using ProductManagement.API.Http.Interface;
using Sxb.Web.Areas.Common.Models;
using Sxb.Web.Response;
using Sxb.Web.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeChat;
using WeChat.Interface;
using WeChat.Model;
using IKeyValueService = PMS.TopicCircle.Application.Services.IKeyValueService;
using IUserService = PMS.UserManage.Application.IServices.IUserService;

namespace Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers.Topic
{

    /// <summary>
    /// 定义加入话题圈场景下的微信关注回调处理者
    /// </summary>
    public class JoinCricleHandler : WXJoinCircleTemplate, ISubscribeCallBackHandler
    {
        ICircleService _circleService;
        IUserService _userService;
        IWeChatService _weChatService;
        IWebHostEnvironment _webHostEnvironment;
        public JoinCricleHandler(ICircleService circleService,
            IConfiguration configuration,
            IKeyValueService keyValueService,
            ILogger<JoinCricleHandler> logger,
            IUserService userService,
            IWeChatAppClient weChatAppClient
            , ITemplateMessageService templateMessageService
            , IWeChatService weChatService
            , IWebHostEnvironment webHostEnvironment)
            : base(configuration, keyValueService, logger, weChatAppClient, templateMessageService)
        {
            this._circleService = circleService;
            _userService = userService;
            _weChatService = weChatService;
            _webHostEnvironment = webHostEnvironment;
        }


        public async Task<ResponseResult> Process(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest)
        {

            UserInfoDto sendUser = _userService.GetUserByWeixinOpenId(frequest.OpenId);
            if (sendUser == null)
            {
                return ResponseResult.Failed("用户不存在，无法加入话题圈。");
            }
            //todo:
            //1.加入圈子
            var response = _circleService.JoinCircle(new PMS.TopicCircle.Application.Dtos.CircleJoinRequestDto()
            {
                CircleId = qrequest.DataId.GetValueOrDefault(),
                UserId = sendUser.Id
            });
            if (response.Status)
            {
                //获取圈子信息
                Circle circle = _circleService.Get(qrequest.DataId);
                //获取达人信息
                Talent talentUser = _userService.GetTalentDetail(circle.UserId.GetValueOrDefault());
                if (qrequest.IsForwardByWXMP)
                {
                    //来自于微信小程序转发
                    frequest.MiniProgramCardInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<MiniProgramCardInfo>(qrequest.ForwardData);
                    if (frequest.MiniProgramCardInfo == null)
                    {

                        return ResponseResult.Failed("找不到推送小程序卡片所需要的消息。");
                    }
                    if (_webHostEnvironment.IsProduction())
                    {
                        //2.推送模板消息,跳转小程序
                        await SendTemplateMessage(circle.Name, $"{ConfigHelper.GetHost()}/topic/subject-details.html?circleId={circle.Id}", talentUser.Nickname
                           , frequest.OpenId
                           , sendUser.NickName
                           , frequest.MiniProgramCardInfo.AppId
                           , qrequest.DataUrl);
                    }
                    else
                    {
                        //模板消息和小程序信息分开发
                        await SendTemplateMessage(circle.Name, $"{ConfigHelper.GetHost()}/topic/subject-details.html?circleId={circle.Id}", talentUser.Nickname
                         , frequest.OpenId
                         , sendUser.NickName);

                        string content = string.Format("appId:{0};pagePath:{1};"
                               , frequest.MiniProgramCardInfo.AppId
                               , qrequest.DataUrl);
                        await _weChatService.SendText(frequest.OpenId, content, "fwh");
                    }

                }
                else
                {
                    //2.推送模板消息
                    await SendTemplateMessage(circle.Name, $"{ConfigHelper.GetHost()}/topic/subject-details.html?circleId={circle.Id}", talentUser.Nickname
                       , frequest.OpenId
                       , sendUser.NickName);

                }
                return ResponseResult.Success($"用户【{sendUser.Id}】成功加入话题圈");
            }
            else
            {
                return ResponseResult.Failed(response.Msg);
            }

        }


    }

}
