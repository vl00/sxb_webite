using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
using IKeyValueService = PMS.TopicCircle.Application.Services.IKeyValueService;
using IUserService = PMS.UserManage.Application.IServices.IUserService;

namespace Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers.Topic
{

    /// <summary>
    /// 定义查看帖子详情场景下的微信关注回调处理者
    /// </summary>
    public class ViewTopicDetailHandler : WXJoinCircleTemplate, ISubscribeCallBackHandler
    {
        ICircleService _circleService;
        ITopicService _topicService;
        IUserService _userService;
        IWeChatService _weChatService;
        IWebHostEnvironment _webHostEnvironment;

        public ViewTopicDetailHandler(ICircleService circleService,
            ITopicService topicService,
            IConfiguration configuration,
            IKeyValueService keyValueService,
            ITemplateMessageService templateMessageService,
            ILogger<ViewTopicDetailHandler> logger,
            IUserService userService,
            IWeChatAppClient weChatAppClient
            , IWeChatService weChatService
            , IWebHostEnvironment webHostEnvironment)
            : base(configuration, keyValueService, logger, weChatAppClient, templateMessageService)
        {
            this._circleService = circleService;
            this._topicService = topicService;
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
            //0.获取帖子信息
            var topic = _topicService.Get(qrequest.DataId);
            //1.加入圈子(ps：圈子ID可通过帖子ID获得)
            var response = _circleService.JoinCircle(new PMS.TopicCircle.Application.Dtos.CircleJoinRequestDto()
            {
                CircleId = topic.CircleId,
                UserId = sendUser.Id
            });
            //2.推送帖子详情微信模板消息
            if (response.Status)
            {
                //获取圈子信息
                Circle circle = _circleService.Get(topic.CircleId);
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

                        //2.推送模板消息
                        await SendTemplateMessage(circle.Name, $"{ConfigHelper.GetHost()}/topic/post-full.html?id={topic.Id}"
                           , talentUser.Nickname
                           , frequest.OpenId
                           , sendUser.NickName
                           , frequest.MiniProgramCardInfo.AppId
                           , qrequest.DataUrl);
                    }
                    else
                    {
                        //模板消息和小程序信息分开发
                        await SendTemplateMessage(circle.Name, $"{ConfigHelper.GetHost()}/topic/post-full.html?id={topic.Id}"
                           , talentUser.Nickname
                           , frequest.OpenId
                           , sendUser.NickName
                           , frequest.MiniProgramCardInfo.AppId
                           , qrequest.DataUrl);

                        string content = string.Format("appId:{0};pagePath:{1};"
                               , frequest.MiniProgramCardInfo.AppId
                               , qrequest.DataUrl);
                        await _weChatService.SendText(frequest.OpenId, content, "fwh");
                    }
                }
                else {
                    //2.推送模板消息
                    await SendTemplateMessage(circle.Name, $"{ConfigHelper.GetHost()}/topic/post-full.html?id={topic.Id}", talentUser.Nickname, frequest.OpenId, sendUser.NickName);

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
