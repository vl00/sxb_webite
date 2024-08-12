using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
using iSchool.Library.Logs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PMS.OperationPlateform.Application.IServices;
using PMS.TopicCircle.Application.Services;
using PMS.TopicCircle.Domain.Entities;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Application.ModelDto;
using PMS.UserManage.Domain.Entities;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using ProductManagement.Infrastructure.Configs;
using Sxb.Web.Areas.TopicCircle.Filters;
using Sxb.Web.Areas.TopicCircle.Models;
using Sxb.Web.Areas.TopicCircle.Utils;
using Sxb.Web.Response;
using Sxb.Web.Utils;
using WeChat;
using WeChat.Interface;
using WeChat.Model;
using IUserService = PMS.UserManage.Application.IServices.IUserService;
using SenceQRCoder = Sxb.Web.Areas.TopicCircle.Utils.SenceQRCoder;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.Web.Areas.TopicCircle.Controllers
{
    [ApiController]
    [Route("tpc/[controller]/[action]")]
    public class ShareController : ControllerBase
    {

        IConfiguration config;
        internal IUserService _userService;
        internal IAccountService _accountService;
        internal ICircleService _circleService;
        internal ITemplateMessageService  _templateMessageService;
        internal IWeChatQRCodeService _weChatQRCodeService;
        internal IKeyValueService _keyValueService;
        internal ICircleFollowerService _circleFollowerService;
        internal ITalentService _talentService;
        internal Microsoft.Extensions.Logging.ILogger _logger;
        internal ITopicService _topicService;
        internal IWeChatAppClient _weChatAppClient;
        internal TopicOption _topicOption;
        internal IEasyRedisClient _easyRedisClient;
        internal IArticleService _articleService;
        ICustomMsgService _customMsgService;
        public ShareController(IUserService userService,
            ICircleService circleService,
            ITemplateMessageService templateMessageService,
            IKeyValueService keyValueService,
            ICircleFollowerService circleFollowerService,
            ITalentService talentService,
            ILogger<ShareController> logger,
            ITopicService topicService,
            IConfiguration configuration
            , IAccountService accountService,
            IWeChatAppClient weChatAppClient
            , IWeChatQRCodeService weChatQRCodeService
            , IOptions<TopicOption> topicOption,
             IEasyRedisClient easyRedisClient
            , IArticleService articleService
            , ICustomMsgService customMsgService)
        {
            _userService = userService;
            this._circleService = circleService;
            this._templateMessageService = templateMessageService;
            this._keyValueService = keyValueService;
            this._circleFollowerService = circleFollowerService;
            this._talentService = talentService;
            this._logger = logger;
            this._topicService = topicService;
            this.config = configuration;
            _accountService = accountService;
            _weChatAppClient = weChatAppClient;
            _weChatQRCodeService = weChatQRCodeService;
            _topicOption = topicOption.Value;
            _easyRedisClient = easyRedisClient;
            _articleService = articleService;
            _customMsgService = customMsgService;
        }


        [HttpGet]
        [Authorize]
        [Description("检查当前登录用户是否为话题圈粉丝")]
        public ResponseResult CheckIsCircleMember(Guid circleId)
        {
            var userId = this.GetUserInfo().UserId;
            bool isFollow = _circleFollowerService.CheckIsFollow(new PMS.TopicCircle.Application.Dtos.CheckIsFollowRequestDto()
            {
                CircleId = circleId,
                UserId = userId
            }); ;
            if (isFollow)
            {
                return ResponseResult.Success("当前用户已关注话题圈");
            }
            else
            {
                return ResponseResult.Failed("当前用户未关注话题圈");
            }
        }


        [HttpGet]
        [Description("检查是否有创建话题圈的权限")]
        public ResponseResult CheckIsHaveCreateCirclePermission()
        {

            Guid userId = this.GetUserInfo().UserId;
            if (userId == default(Guid))
            {
                return ResponseResult.Failed("当前用户未登录");
            }
            //检查用户是否为达人身份
            var isTalent = this._talentService.IsTalent(userId.ToString());
            if (isTalent)
            {
                //如果是达人，检查用户是不是已经创建了圈子
                var isHasCricle = this._circleService.CheckIsHasCircle(userId);
                if (isHasCricle)
                {
                    return ResponseResult.Failed("当前用户已经创建了话题圈");
                }
                else
                {
                    return ResponseResult.Success("当前用户拥有创建话题圈的权限");
                }

            }
            else
            {
                return ResponseResult.Failed("只有达人身份用户才拥有创建圈子权限");
            }

        }

        [HttpGet]
        [Authorize]
        [Description("校验当前登录用户是否为达人身份")]
        public ResponseResult CheckIsTalent()
        {
            Guid userId = this.GetUserInfo().UserId;
            var isTalent = this._talentService.IsTalent(userId.ToString());
            if (!isTalent)
            {
                return ResponseResult.Failed("当前用户非达人身份");
            }
            else
            {
                return ResponseResult.Success("当前用户为达人身份");
            }

        }




        [HttpGet]
        [Authorize]
        [Description("检查是否关注了服务号")]
        public async Task<ResponseResult> CheckSubscribeFwh([FromQuery] int repeatTime = 1)
        {
            return await Task.Run<ResponseResult>(() =>
            {
                int count = 0;
                while (true)
                {
                    count++;
                    var userId = this.GetUserInfo().UserId;
                    var isBindFwh = _userService.CheckIsSubscribeFwh(userId, ConfigHelper.GetFwh());
                    if (isBindFwh)
                    {
                        return ResponseResult.Success("已关注");
                    }
                    else if (count >= repeatTime)
                    {
                        int expire_second = (int)TimeSpan.FromDays(2).TotalSeconds;
                        var accessToken = _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" }).GetAwaiter().GetResult();
                        var Scene = new CreateTempQRStrSceneCodeRequest(expire_second);
                        var qrcodeResponse = _weChatQRCodeService.GenerateTempQRCode(accessToken.token, Scene).GetAwaiter().GetResult();
                        return new ResponseResult()
                        {
                            Msg = "未关注上学帮微信服务号",
                            status = ResponseCode.UnSubScribeFWH,
                            Succeed = false,
                            Data = new
                            {
                                isBindFwh,
                                qrcode = qrcodeResponse.ImgUrl
                            }

                        };
                    }

                    Thread.Sleep(1000);
                }


            });
        }

        [HttpGet]
        [Authorize]
        [Description("检查是否绑定了完整的账户信息")]
        public async Task<ResponseResult> CheckAccoutBind([FromQuery] int repeatTime = 1)
        {
            return await Task.Run<ResponseResult>(() =>
             {
                 int count = 0;
                 while (true)
                 {
                     count++;
                     var userId = this.GetUserInfo().UserId;
                     var isBindPhone = _accountService.IsBindPhone(userId);
                     var isBindWx = _accountService.IsBindWx(userId);
                     if (isBindPhone && isBindWx)
                     {
                         return ResponseResult.Success("已绑定");
                     }
                     else if (count >= repeatTime)
                     {
                         using (var scope = base.HttpContext.RequestServices.CreateScope())
                         {
                             //不适用构造函数注入的IAccountService，因为那个实例的生命周期不满足当前场景。
                             IAccountService accountService = scope.ServiceProvider.GetService<IAccountService>();
                             string imgUrl = accountService.GenerateBindAccountQRCode($"{ConfigHelper.GetHost()}/tpc/Share/SubscribeCallBack?DataId={userId}&type={(int)SubscribeCallBackType.bindAccount}", 1);
                             return new ResponseResult()
                             {
                                 Msg = "未进行完整的账户信息绑定",
                                 status = ResponseCode.UnBindAcount,
                                 Succeed = false,
                                 Data = new
                                 {
                                     isBindPhone,
                                     isBindWx,
                                     qrcode = imgUrl
                                 }
                             };
                         }

                     };
                     Thread.Sleep(1000);
                 }


             });

        }


        /// <summary>
        /// 检测是否绑定手机, 微信, 和关注服务号
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ResponseResult CheckJoinCircle()
        {

            var userId = this.GetUserInfo().UserId;
            var isBindPhone = _accountService.IsBindPhone(userId);
            var isBindWx = _accountService.IsBindWx(userId);
            var isBindFwh = _userService.CheckIsSubscribeFwh(userId, ConfigHelper.GetFwh());
            return ResponseResult.Success(new
            {
                isBindPhone,
                isBindWx,
                isBindFwh
            });
        }



        [HttpGet]
        [Authorize]
        public async Task<ResponseResult> GetWelcomeQrCode(string redirectUrl, bool isMiniApp = false)
        {
            if (redirectUrl.Contains(".sxkid.com") || isMiniApp)
            {
                double storeDay = 1;
                SenceQRCoder qrCoder = new SenceQRCoder(this._weChatAppClient, this._easyRedisClient,_weChatQRCodeService);
                var code = await qrCoder.GetSenceQRCode(new SubscribeCallBackQueryRequest()
                {
                    DataUrl = HttpUtility.UrlEncode(redirectUrl),
                    Type = SubscribeCallBackType.welcomTalent,
                }, storeDay, isMiniApp);
                return ResponseResult.Success(new
                {
                    qrcode = code
                });

            }
            else
            {
                return ResponseResult.Failed("你的链接不符合规定.");
            }


        }


        [Description("开放一个接口供二维码扫码关注时触发的回调")]
        [HttpPost]
        public ResponseResult SubscribeCallBack([FromQuery] SubscribeCallBackQueryRequest qrequest, [FromBody] SubscribeCallBackFormRequest frequest)
        {
            this._logger.LogDebug("接收到上学帮微信业务应用回调，type:{type},openid:{openid},dataId:{dataId}", qrequest.Type, frequest.OpenId, qrequest.DataId);
            UserInfoDto userInfo = this._userService.GetUserByWeixinOpenId(frequest.OpenId);
            var handler = SubscribeCallBackHandlerFactory.Create(this._circleService
                , this._topicService
                , this.config
                , this._keyValueService
                , this._templateMessageService
                , qrequest.Type
                , userInfo
                , this._logger
                , this._userService
                , this._weChatAppClient
                , this._topicOption
                , _articleService
                ,_customMsgService);
            if (handler == null)
            {
                return ResponseResult.Failed("找不到回调Handler");
            }
            return handler.Process(qrequest, frequest);

        }



    }

    /// <summary>
    /// 微信关注回调处理者对象构造工厂
    /// </summary>
    public class SubscribeCallBackHandlerFactory
    {

        public static ISubscribeCallBackHandler Create(ICircleService circleService
            , ITopicService topicService
            , IConfiguration configuration
            , IKeyValueService keyValueService
            , ITemplateMessageService  templateMessageService
            , SubscribeCallBackType subscribeCallBackType
            , UserInfoDto userInfoDto
            , ILogger logger
            , IUserService userService
            , IWeChatAppClient weChatAppClient
            , TopicOption topicOption
            , IArticleService articleService
            , ICustomMsgService customMsgService)
        {
            switch (subscribeCallBackType)
            {
                case SubscribeCallBackType.joincircle:
                    return new JoinCricleHandler(circleService
                        , configuration
                        , keyValueService
                        , templateMessageService
                        , userInfoDto
                        , logger
                        , userService
                        , weChatAppClient);
                case SubscribeCallBackType.viewTopicDetail:
                    return new ViewTopicDetailHandler(circleService
                      , topicService
                      , configuration
                      , keyValueService
                      ,templateMessageService
                      , userInfoDto
                      , logger
                      , userService
                      , weChatAppClient);
                case SubscribeCallBackType.bindAccount:
                    return new BindAccountHandler(weChatAppClient, topicOption, customMsgService);
                case SubscribeCallBackType.welcomTalent:
                    return new WelcomTalentHandler(weChatAppClient, topicOption,customMsgService);
                case SubscribeCallBackType.articleview:
                    return new ArticleViewHandler(weChatAppClient, articleService, customMsgService);
                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// 定义默认的微信关注回调的处理者
    /// </summary>
    public interface ISubscribeCallBackHandler
    {

        ResponseResult Process(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest);
    }

    /// <summary>
    /// 针对JoinCircle这个微信模板封装的抽象类
    /// </summary>
    public abstract class WXJoinCircleTemplate
    {
        protected ILogger logger;
        protected IKeyValueService _keyValueService;
        protected ITemplateMessageService  _templateMessageService;
        protected UserInfoDto sendUser;
        protected string templateId;
        protected TemplateDataFiled first;
        protected TemplateDataFiled keyword1;
        protected TemplateDataFiled keyword2;
        protected TemplateDataFiled remark;
        protected IWeChatAppClient _weChatAppClient;
        public WXJoinCircleTemplate(IConfiguration configuration
            , IKeyValueService keyValueService
            , ITemplateMessageService   templateMessageService
            , UserInfoDto userInfoDto
            , ILogger logger
            , IWeChatAppClient weChatAppClient
            )
        {
            this._keyValueService = keyValueService;
            this._templateMessageService = templateMessageService;
            this.sendUser = userInfoDto;
            this._weChatAppClient = weChatAppClient;
            this.logger = logger;
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


        public async Task SendTemplateMessage(string name, string url, string talentName, string openId, string appid = null, string pagepath = null)
        {

            try
            {
                var accessToken = await _weChatAppClient.GetAccessToken(new ProductManagement.API.Http.Model.WeChatGetAccessTokenRequest() { App = "fwh" });
                SendTemplateRequest sendTemplateRequest = new SendTemplateRequest(openId, templateId);
                sendTemplateRequest.Url = url;

                if(!string.IsNullOrWhiteSpace(appid) && !string.IsNullOrWhiteSpace(pagepath))
                {
                    sendTemplateRequest.SetMiniprogram(appid, pagepath);
                }

                first.Value = first.Value.Replace("{CircleName}", name).Replace("{TalentName}", talentName);
                keyword1.Value = keyword1.Value.Replace("{NickName}", sendUser.NickName);
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



    /// <summary>
    /// 定义加入话题圈场景下的微信关注回调处理者
    /// </summary>
    public class JoinCricleHandler : WXJoinCircleTemplate, ISubscribeCallBackHandler
    {
        ICircleService _circleService;
        IUserService _userService;
        public JoinCricleHandler(ICircleService circleService,
            IConfiguration configuration,
            IKeyValueService keyValueService,
            ITemplateMessageService  templateMessageService,
            UserInfoDto userInfoDto,
            ILogger logger,
            IUserService userService,
            IWeChatAppClient weChatAppClient)
            : base(configuration, keyValueService, templateMessageService, userInfoDto, logger, weChatAppClient)
        {
            this._circleService = circleService;
            _userService = userService;
        }


        public ResponseResult Process(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest)
        {
            if (this.sendUser == null)
            {
                return ResponseResult.Failed("用户不存在，无法加入话题圈。");
            }
            //todo:
            //1.加入圈子
            var response = _circleService.JoinCircle(new PMS.TopicCircle.Application.Dtos.CircleJoinRequestDto()
            {
                CircleId = qrequest.DataId.GetValueOrDefault(),
                UserId = this.sendUser.Id
            });
            if (response.Status)
            {
                //获取圈子信息
                Circle circle = _circleService.Get(qrequest.DataId);
                //获取达人信息
                Talent talentUser = _userService.GetTalentDetail(circle.UserId.GetValueOrDefault());
                //2.推送模板消息
                if (qrequest.IsMiniApp)
                {
                    string appId = ConfigHelper.GetWxMiniProgramAppId();
                    string pagePath = $"pages/topic-circle/topic-circle?circleId={circle.Id}";
                    _ = SendTemplateMessage(circle.Name, $"{ConfigHelper.GetHost()}/topic/subject-details.html?circleId={circle.Id}", talentUser.Nickname, frequest.OpenId, appId, pagePath);
                }
                else
                {
                    _ = SendTemplateMessage(circle.Name, $"{ConfigHelper.GetHost()}/topic/subject-details.html?circleId={circle.Id}", talentUser.Nickname, frequest.OpenId);
                }
                return ResponseResult.Success($"用户【{this.sendUser.Id}】成功加入话题圈");
            }
            else
            {
                return ResponseResult.Failed(response.Msg);
            }

        }


    }

    /// <summary>
    /// 定义查看帖子详情场景下的微信关注回调处理者
    /// </summary>
    public class ViewTopicDetailHandler : WXJoinCircleTemplate, ISubscribeCallBackHandler
    {
        ICircleService _circleService;
        ITopicService _topicService;
        IUserService _userService;
        public ViewTopicDetailHandler(ICircleService circleService,
            ITopicService topicService,
            IConfiguration configuration,
            IKeyValueService keyValueService,
            ITemplateMessageService  templateMessageService,
            UserInfoDto userInfoDto,
            ILogger logger,
            IUserService userService,
            IWeChatAppClient weChatAppClient)
            : base(configuration, keyValueService, templateMessageService, userInfoDto, logger, weChatAppClient)
        {
            this._circleService = circleService;
            this._topicService = topicService;
            _userService = userService;
        }

        public ResponseResult Process(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest)
        {
            if (this.sendUser == null)
            {
                return ResponseResult.Failed("用户不存在，无法加入话题圈。");
            }
            //todo:
            //0.获取帖子信息
            Topic topic = _topicService.Get(qrequest.DataId);
            //1.加入圈子(ps：圈子ID可通过帖子ID获得)
            var response = _circleService.JoinCircle(new PMS.TopicCircle.Application.Dtos.CircleJoinRequestDto()
            {
                CircleId = topic.CircleId,
                UserId = this.sendUser.Id
            });
            //2.推送帖子详情微信模板消息
            if (response.Status)
            {
                //获取圈子信息
                Circle circle = _circleService.Get(topic.CircleId);
                //获取达人信息
                Talent talentUser = _userService.GetTalentDetail(circle.UserId.GetValueOrDefault());
                //2.推送模板消息
                if (qrequest.IsMiniApp)
                {
                    string appId = ConfigHelper.GetWxMiniProgramAppId();
                    string pagePath = $"pages/topic-post/topic-post?id={topic.Id}";
                    _ = SendTemplateMessage(circle.Name, $"{ConfigHelper.GetHost()}/topic/post-full.html?id={topic.Id}", talentUser.Nickname, frequest.OpenId, appId, pagePath);
                }
                else
                {
                    _ = SendTemplateMessage(circle.Name, $"{ConfigHelper.GetHost()}/topic/post-full.html?id={topic.Id}", talentUser.Nickname, frequest.OpenId);
            }
                return ResponseResult.Success($"用户【{this.sendUser.Id}】成功加入话题圈");
            }
            else
            {
                return ResponseResult.Failed(response.Msg);
            }
        }
    }


    /// <summary>
    /// 处理绑定账户场景回调
    /// </summary>
    public class BindAccountHandler : ISubscribeCallBackHandler
    {
        IWeChatAppClient _weChatAppClient;
        TopicOption _topicOption;
        ICustomMsgService _customMsgService;
        public BindAccountHandler(IWeChatAppClient weChatAppClient
            , TopicOption topicOption
            , ICustomMsgService customMsgService)
        {
            _weChatAppClient = weChatAppClient;
            _topicOption = topicOption;
            _customMsgService = customMsgService;
        }

        public ResponseResult Process(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest)
        {
            NewsCustomMsg msg = new NewsCustomMsg()
            {
                ToUser = frequest.OpenId,
                Url = _topicOption.BindAccountKFMsg.RedirectUrl.Replace("{userId}", qrequest.DataId.ToString()),
                Description = _topicOption.BindAccountKFMsg.Description,
                PicUrl = _topicOption.BindAccountKFMsg.ImgUrl,
                Title = _topicOption.BindAccountKFMsg.Title
            };
            var accessToken = _weChatAppClient.GetAccessToken(new ProductManagement.API.Http.Model.WeChatGetAccessTokenRequest() { App = "fwh" }).GetAwaiter().GetResult();
            var result = _customMsgService.Send(accessToken.token, msg).GetAwaiter().GetResult();

            return new ResponseResult() { Succeed = result.Success };
        }
    }
    /// <summary>
    /// 欢迎达人关注场景回调
    /// </summary>
    public class WelcomTalentHandler : ISubscribeCallBackHandler
    {
        IWeChatAppClient _weChatAppClient;
        TopicOption _topicOption;
        ICustomMsgService _customMsgService;
        public WelcomTalentHandler(IWeChatAppClient weChatAppClient
            , TopicOption topicOption
            , ICustomMsgService customMsgService)
        {
            _weChatAppClient = weChatAppClient;
            _topicOption = topicOption;
            _customMsgService = customMsgService;
        }

        public ResponseResult Process(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest)
        {

            if (qrequest.IsMiniApp)
            {
                string appId = ConfigHelper.GetWxMiniProgramAppId();
                MiniProgramCustomMsg  msg = new MiniProgramCustomMsg()
                {
                    ToUser = frequest.OpenId,
                    Title = _topicOption.WelcomTalentCreateKFMiniAppMsg.Title,
                    Appid = appId,
                    Pagepath = qrequest.DataUrl,
                    ThumbMediaId = _topicOption.WelcomTalentCreateKFMiniAppMsg.ImgUrl
                };
                var accessToken = _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" }).GetAwaiter().GetResult();
                var result = _customMsgService.Send(accessToken.token, msg).GetAwaiter().GetResult();

                return new ResponseResult() { Succeed = result.Success };
            }
            else
            {
                NewsCustomMsg msg = new NewsCustomMsg()
                {
                    ToUser =frequest.OpenId,
                    Url = qrequest.DataUrl,
                    Description = _topicOption.WelcomTalentCreateKFMsg.Description,
                    PicUrl = _topicOption.WelcomTalentCreateKFMsg.ImgUrl,
                    Title = _topicOption.WelcomTalentCreateKFMsg.Title
                };
                var accessToken = _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" }).GetAwaiter().GetResult();
                var result = _customMsgService.Send(accessToken.token, msg).GetAwaiter().GetResult();

                return new ResponseResult() { Succeed = result.Success };
            }
        }
    }

    /// <summary>
    /// 展开文章全文关注场景回调
    /// </summary>
    public class ArticleViewHandler : ISubscribeCallBackHandler
    {
        IWeChatAppClient _weChatAppClient;
        IArticleService _articleService;
        ICustomMsgService _customMsgService;
        public ArticleViewHandler(IWeChatAppClient weChatAppClient
            , IArticleService articleService
            , ICustomMsgService customMsgService)
        {
            _weChatAppClient = weChatAppClient;
            _articleService = articleService;
            _customMsgService = customMsgService;
        }

        public ResponseResult Process(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest)
        {
            var article = _articleService.GetByIds(new[] { qrequest.DataId.GetValueOrDefault() }, true).FirstOrDefault();
            //无cover, 默认上学帮logo
            var articleCoverUrl = "https://cos.sxkid.com/v4source/pc/imgs/top.png";
            if (true == article.Covers?.Any())
            {
                articleCoverUrl = _articleService.ArticleCoverToLink(article.Covers.FirstOrDefault());
            }

            NewsCustomMsg msg = new NewsCustomMsg()
            {
                ToUser = frequest.OpenId,
                Url = $"{ConfigHelper.GetHost()}/article/{UrlShortIdUtil.Long2Base32(article.No)}.html",
                Description = "点击链接继续查看全文，使用公众号下方菜单栏，还可解锁更多实用功能",
                PicUrl = articleCoverUrl,
                Title = $"您正在阅读：{article.title}"
            };
            var accessToken = _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" }).GetAwaiter().GetResult();
            var result = _customMsgService.Send(accessToken.token,  msg).GetAwaiter().GetResult();

            return new ResponseResult() { Succeed = result.Success };
        }
    }
}
