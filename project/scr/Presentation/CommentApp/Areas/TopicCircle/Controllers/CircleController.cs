using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NPOI.XSSF.UserModel;
using PMS.TopicCircle.Application.Dtos;
using PMS.TopicCircle.Application.Services;
using PMS.TopicCircle.Domain.Entities;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Entities;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model;
using ProductManagement.Framework.AspNetCoreHelper.Filters;
using ProductManagement.Framework.AspNetCoreHelper.RequestModel;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using ProductManagement.Infrastructure.Configs;
using ProductManagement.Tool.Email;
using Sxb.Web.Areas.TopicCircle.Filters;
using Sxb.Web.Areas.TopicCircle.Models;
using Sxb.Web.Common;
using Sxb.Web.Controllers;
using Sxb.Web.Filters;
using Sxb.Web.RequestModel;
using Sxb.Web.Response;
using Sxb.Web.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.DrawingCore.Imaging;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using WeChat;
using WeChat.Interface;
using WeChat.Model;
using IUserService = PMS.UserManage.Application.IServices.IUserService;
using ValidateFWHSubscribeAttribute = Sxb.Web.Areas.TopicCircle.Filters.ValidateFWHSubscribeAttribute;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.Web.Areas.TopicCircle.Controllers
{


    [ApiController]
    [Route("tpc/[controller]/[action]")]
    public class CircleController : ApiBaseController
    {
        ICircleService _circleService;
        ICircleFollowerService _circleFollowerService;
        IFileUploadService _fileUploadService;
        IUserService _userService;
        ICircleAccessLogService _circleAccessLogService;
        ITopicService _topicService;
        IWeChatAppClient _weChatAppClient;
        IAccountService _accountService;
        TopicOption _topicOption;
        ILogger _logger;
        IEmailClient _emailClient;
        ICustomMsgService _customMsgService;

        public CircleController(ITopicService topicService,
            ICircleService circleService,
            IFileUploadService fileUploadService,
            ICircleFollowerService circleFollowerService,
            IUserService userService,
            ICircleAccessLogService circleAccessLogService,
            IWeChatAppClient weChatAppClient,
            IAccountService accountService,
            IOptions<TopicOption> topicOption,
            ILogger<CircleController> logger
            , IEmailClient emailClient
            , ICustomMsgService customMsgService)
        {
            _topicService = topicService;
            this._circleService = circleService;
            this._fileUploadService = fileUploadService;
            this._circleFollowerService = circleFollowerService;
            this._userService = userService;
            this._circleAccessLogService = circleAccessLogService;
            this._weChatAppClient = weChatAppClient;
            _accountService = accountService;
            _topicOption = topicOption.Value;
            _logger = logger;
            _emailClient = emailClient;
            _customMsgService = customMsgService;
        }


        /// <summary>
        /// 获取达人创建的圈子
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Description("获取当前用户创建的圈子")]
        public ResponseResult GetMyCircles()
        {
            var userInfo = this.GetUserInfo();
            var circlesResult = this._circleService.GetCircles(userInfo.UserId);
            TalentCirclesResponse response = new TalentCirclesResponse()
            {
                circles = circlesResult.Data
            };
            return ResponseResult.Success(response);

        }

        /// <summary>
        /// 获取用户加入的圈子列表
        /// </summary>
        /// <param name="timeNode"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Description("获取用户加入的圈子列表")]
        public ResponseResult GetUserCircles()
        {
            var userInfo = this.GetUserInfo();
            var talentCircles = this._circleService.GetTalentCircles(userInfo.UserId);
            var userCircles = this._circleService.GetMyCircles(new GetMyCirclesRequestDto()
            {
                UserId = userInfo.UserId
            });
            List<MyCircleItemDto> myCircleItemDtos = new List<MyCircleItemDto>();
            if (talentCircles.Data != null && talentCircles.Data.Any())
            {
                myCircleItemDtos.AddRange(talentCircles.Data);
            }
            if (userCircles.Data != null && userCircles.Data.Any())
            {
                myCircleItemDtos.AddRange(userCircles.Data);
            }
            return ResponseResult.Success<IEnumerable<MyCircleItemDto>>(myCircleItemDtos);

        }


        /// <summary>
        /// 创建粉丝圈
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ValidateTalentIdentity]
        [Description("创建话题圈")]
        public ResponseResult<CircleCreateResponse> Create([FromBody] CircleCreateRequest request)
        {
            var userId = this.GetUserInfo().UserId;
            //如果是达人，检查用户是不是已经创建了圈子
            var isHasCricle = this._circleService.CheckIsHasCircle(userId);
            if (isHasCricle)
            {
                return ResponseResult<CircleCreateResponse>.Failed(ResponseCode.HasCreateCircle, ResponseCode.HasCreateCircle.GetDescription());
            }
            CircleCreateRequestDto input = new CircleCreateRequestDto()
            {
                UserId = userId,
                Name = request.Name,
                CoverUrl = request.CoverUrl,
                Intro = request.Intro,
                BGColor = request.BGColor

            };
            var result = this._circleService.Create(input);
            if (result.Status)
            {
                //查询信息绑定状态
                var isBindPhone = _accountService.IsBindPhone(userId);
                var isBindWx = _accountService.IsBindWx(userId);
                var isSubscribeFwh = _userService.TryGetOpenId(userId, "fwh", out string openId);
                CircleCreateResponse response = new CircleCreateResponse()
                {
                    CircleId = result.Data.CircleId,
                    IsBindPhone = isBindPhone,
                    IsBindWx = isBindWx,
                    IsSubscribeFwh = isSubscribeFwh
                };
                _ = this.SendWelcomCreateCirclMsg(result.Data.CircleId, openId);
                return ResponseResult<CircleCreateResponse>.Success(response, result.Msg);
            }
            else
            {
                return ResponseResult<CircleCreateResponse>.Failed(result.Msg);
            }
        }


        /// <summary>
        /// 获取推荐圈子列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Description("获取推荐圈子列表")]
        public ResponseResult<IEnumerable<CircleDetailDto>> GetRecommend()
        {
            int locationCityCode = Request.GetLocalCity();
            return this._circleService.GetRecommends(locationCityCode);
        }


        /// <summary>
        /// 编辑话题圈资料
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ValidateCircleMaster]
        [Description("编辑话题圈资料")]
        public ResponseResult Edit([FromQuery] Guid circleID, [FromBody] CircleEditRequest request)
        {
            CircleEditRequestDto input = new CircleEditRequestDto()
            {
                CircleId = circleID,
                CoverUrl = request.CoverUrl,
                Intro = request.Intro,
                Name = request.Name,
                BGColor = request.BGColor
            };
            AppServiceResultDto result = this._circleService.Edit(input);
            if (result.Status)
            {
                return ResponseResult.Success();
            }
            else
            {
                return ResponseResult.Failed(result.Msg);
            }
        }


        /// <summary>
        /// 获取Circle详情信息
        /// </summary>
        /// <param name="circleID"></param>
        /// <returns></returns>
        [HttpGet]
        [Description("话题圈详情信息")]
        public ResponseResult Detail(Guid circleID)
        {
            var userId = this.GetUserInfo()?.UserId;
            CircleAccessLog circleAccessLog = this._circleAccessLogService.GetLatest(circleID, userId.Value);
            AppServiceResultDto<CircleDetailDto> result = this._circleService.GetDetail(circleID, circleAccessLog?.CreateTime);
            if (result.Status)
            {
                //记录访问日志
                this._circleAccessLogService.Record(new Circle() { Id = circleID }, new UserInfo() { Id = userId.Value });
                var circle = result.Data;
                var talent = this._userService.GetTalentDetail(circle.UserId);
                var isFollow = false;
                var isLogin = User.Identity.IsAuthenticated;
                if (isLogin)
                    isFollow = _circleFollowerService.CheckIsFollow(new CheckIsFollowRequestDto() { CircleId = circleID, UserId = userId.Value });
                var isCircleMaster = userId == circle.UserId;
                return ResponseResult.Success(new CircleDetailResponse()
                {
                    Cover = circle.Cover,
                    FollowerCount = circle.FollowerCount,
                    Id = circle.Id,
                    Intro = circle.Intro,
                    IsDisable = circle.IsDisable,
                    Name = circle.Name,
                    UserId = talent.Id,
                    Nickname = talent.Nickname,
                    HeadImgUrl = talent.HeadImgUrl,
                    AuthInfo = talent.Certification_preview,
                    IsLogin = isLogin,
                    IsFollow = isCircleMaster ? true : isFollow,
                    IsCircleMaster = isCircleMaster,
                    NewFollower = circle.NewFollower,
                    TopicCount = circle.TopicCount,
                    MasterIntroduce = talent.Introduction,
                    YesterdayFollowerCount = circle.YesterdayFollowerCount,
                    YesterdayTopicCount = circle.YesterdayTopicCount,
                    BgColor = circle.BGColor

                });
            }
            else
            {

                return ResponseResult<CircleDetailResponse>.Failed("无该圈子");
            }


        }

        [HttpGet]
        [Description("获取达人创建的圈子详情信息")]
        public ResponseResult TalentCircleDetail(Guid userId)
        {
            var userInfo = this.GetUserInfo();
            var circlesResult = this._circleService.GetTalentCircleDetail(userId);
            if (!circlesResult.Status)
                return ResponseResult<CircleDetailResponse>.Failed(circlesResult.Msg);

            var circle = circlesResult.Data;
            var talent = this._userService.GetTalentDetail(circle.UserId);
            var isFollow = false;
            var isLogin = User.Identity.IsAuthenticated;
            if (isLogin)
                isFollow = _circleFollowerService.CheckIsFollow(new CheckIsFollowRequestDto() { CircleId = circle.Id, UserId = userInfo.UserId });

            var isCircleMaster = userId == circle.UserId;

            return ResponseResult.Success(new CircleDetailResponse()
            {
                Cover = circle.Cover,
                FollowerCount = circle.FollowerCount,
                Id = circle.Id,
                Intro = circle.Intro,
                IsDisable = circle.IsDisable,
                Name = circle.Name,
                UserId = talent.Id,
                Nickname = talent.Nickname,
                HeadImgUrl = talent.HeadImgUrl,
                AuthInfo = talent.Certification_preview,
                IsLogin = isLogin,
                IsFollow = isFollow,
                IsCircleMaster = isCircleMaster
            });


        }

        /// <summary>
        /// 获取圈主信息
        /// </summary>
        /// <param name="circleId"></param>
        /// <returns></returns>
        [HttpGet]
        [Description("获取圈主信息")]
        public ResponseResult GetCircleMasterInfo(Guid circleId)
        {
            Circle circle = this._circleService.Get(circleId);
            if (circle == null)
            {
                return ResponseResult.Failed("无该圈子");
            }
            else
            {
                var talent = this._userService.GetTalentDetail(circle.UserId.GetValueOrDefault());

                return ResponseResult.Success(new CircleMasterInfoResponse()
                {
                    UserId = talent.Id,
                    CircleId = circle.Id,
                    CircleIntro = circle.Intro,
                    Nickname = talent.Nickname,
                    HeadImgUrl = talent.HeadImgUrl,
                    AuthInfo = talent.Certification_preview,
                });
            }
        }


        /// <summary>
        /// 加入粉丝圈
        /// </summary>
        /// <param name="circleID"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [ValidateAccoutBind]
        [ValidateFWHSubscribe(DataParamName = "circleID", QRCodeType = ValidateFWHSubscribeAttribute.FWHQRCodeType.JoinCircle)]
        [Description("加入话题圈")]
        public ResponseResult JoinCircle(Guid circleID)
        {
            var joinResult = this._circleService.JoinCircle(new CircleJoinRequestDto()
            {
                CircleId = circleID,
                UserId = this.GetUserInfo().UserId
            });

            return joinResult;
        }

        [HttpGet]
        [Authorize]
        [Description("加入粉丝圈Test")]
        public ResponseResult JoinCircleTest(Guid circleID)
        {
            var joinResult = this._circleService.JoinCircle(new CircleJoinRequestDto()
            {
                CircleId = circleID,
                UserId = this.GetUserInfo().UserId
            });

            return joinResult;
        }

        public ResponseResult JoinCircleTestr2(Guid userId, Guid circleID)
        {
            var joinResult = this._circleService.JoinCircle(new CircleJoinRequestDto()
            {
                CircleId = circleID,
                UserId = userId
            });

            return joinResult;
        }

        /// <summary>
        /// 退出话题圈
        /// </summary>
        /// <param name="circleID"></param>
        /// <returns></returns>
        [HttpGet]
        [ValidateCriclePartof]
        [Description("退出话题圈")]
        public ResponseResult ExitCircle(Guid circleID)
        {
            AppServiceResultDto resultDto = this._circleService.ExitCircle(new CircleExitRequestDto()
            {
                CircleId = circleID,
                UserId = this.GetUserInfo().UserId
            });
            return resultDto;
        }


        /// <summary>
        /// 获取加入圈子场景的关注二维码
        /// </summary>
        /// <param name="circleId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Description("获取加入圈子场景的关注二维码")]
        public async Task<ResponseResult> GetSubscribeQRCode(Guid circleId)
        {
            string subscribeHandleUrl = $"{ConfigHelper.GetHost()}/tpc/Share/SubscribeCallBack?DataId={circleId}&type={(int)SubscribeCallBackType.joincircle}";
            string qrcode = await this._circleService.GenerateWeChatCode(circleId, subscribeHandleUrl);
            //获取登录账号绑定的微信昵称
            var weixinNickName = _userService.GetUserWeixinNickName(this.GetUserInfo().UserId);
            return ResponseResult.Success(new GetSubscribeQRCodeResponse()
            {
                ImgUrl = qrcode,
                WeixinNickName = weixinNickName
            });
        }

        /// <summary>
        /// 获取加入圈子场景的关注二维码（微信小程序用）
        /// </summary>
        /// <param name="circleId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Description("获取加入圈子场景的关注二维码（微信小程序用）")]
        public async Task<ResponseResult> GetSubscribeQRCodeWxMiniProgram(Guid circleId)
        {
            string subscribeHandleUrl = $"{ConfigHelper.GetHost()}/tpc/Share/SubscribeCallBack?DataId={circleId}&type={(int)SubscribeCallBackType.joincircle}&isMiniApp=true";
            string qrcode = await this._circleService.GenerateWeChatCode(circleId, subscribeHandleUrl, true);
            //获取登录账号绑定的微信昵称
            var weixinNickName = _userService.GetUserWeixinNickName(this.GetUserInfo().UserId);
            return ResponseResult.Success(new GetSubscribeQRCodeResponse()
            {
                ImgUrl = qrcode,
                WeixinNickName = weixinNickName
            });
        }

        /// <summary>
        /// 切换话题圈的禁用状态
        /// </summary>
        /// <param name="circleId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [ValidateCircleMaster]
        [Description("切换话题圈的禁用状态")]
        public ResponseResult<CircleToggleDisableStatuDto> ToggleDisableStatu(Guid circleId)
        {
            return this._circleService.ToggleDisableStatu(circleId);

        }


        /// <summary>
        /// 移除关注者
        /// </summary>
        /// <param name="followerId"></param>
        /// <param name="circleId"></param>
        /// <returns></returns>
        [HttpGet]
        [ValidateCircleMaster]
        [Description("移除关注者")]
        public ResponseResult RemoveFollower(Guid followerId, Guid circleId)
        {
            return this._circleFollowerService.RemoveFollower(new RemoveFollowerRequestDto()
            {
                CircleId = circleId,
                UserId = followerId
            });
        }

        /// <summary>
        /// 获取圈子的关注用户列表
        /// </summary>
        /// <param name="circleId"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateCircleMaster]
        [Description("获取圈子的关注用户列表")]
        public ResponsePageResult<IEnumerable<CircleFollowerDetailDto>> GetFollowers([FromQuery] Guid circleId, [FromBody] GetFollowersRequest request)
        {
            return this._circleFollowerService.GetFollowerDetail(new AppServicePageRequestDto<GetFollowerDetailRequestDto>()
            {
                input = new GetFollowerDetailRequestDto()
                {
                    CircleId = circleId,
                    SortType = request.SortType,
                    SearchContent = request.SearchContent
                },
                limit = request.Limit,
                offset = request.Offset
            });
        }

        /// <summary>
        /// 上传话题圈背景图
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("上传话题圈背景图")]
        [Authorize]
        public async Task<ResponseResult> UploadCover(IFormFile img)
        {
            Stream imgStream = img.OpenReadStream();
            var up = await this._fileUploadService.UploadCircleCover(img.FileName, imgStream);
            if (up != null)
            {
                string color = GetImageMainColor(imgStream);
                UploadCircleCoverResponse response = new UploadCircleCoverResponse()
                {
                    CdnUrl = up.CdnUrl,
                    Url = up.Url,
                    MainColor = color

                };
                return ResponseResult.Success(response);
            }
            else
            {
                return ResponseResult.Failed();
            }

        }


        private string GetImageMainColor(Stream img)
        {

            Image image = Image.FromStream(img);
            var thumbnailImage = image.GetThumbnailImage((int)(image.Width * 0.1), (int)(image.Height * 0.1), null, IntPtr.Zero);
            Bitmap bitmap = new Bitmap(thumbnailImage);
            List<int> argbs = new List<int>();
            for (int i = 0; i < thumbnailImage.Width; i++)
            {
                for (int j = 0; j < thumbnailImage.Height; j++)
                {
                    Color color = bitmap.GetPixel(i, j);
                    if (color.R == color.G && color.R == color.B)
                    {
                        continue;
                    }
                    else
                    {

                        argbs.Add(color.ToArgb());
                    }

                }
            }
            Color iwant = Color.FromArgb(argbs.GroupBy(a => a).OrderByDescending(a => a.Count()).FirstOrDefault().Key);
            Utils.CustomColorTranslator.RGB2HSL(iwant, out double h, out double s, out double v);
            v = 0.35;
            iwant = Utils.CustomColorTranslator.HSL2RGB(h, s, v);
            return ColorTranslator.ToHtml(iwant);
        }



        /// <summary>
        /// 推荐话题圈
        /// <para>
        /// 【规则】
        ///1、推荐位：8个（提醒，预留后期可能会增加推荐位）
        ///2、推荐层级安排如下：
        ///①4个推荐位：推荐与用户同城市的达人所建的话题圈；
        ///②2个推荐位：推荐与用户同省但不同城市的达人所建的话题圈；
        ///③2个推荐位：其他省份的达人所建话题圈；
        ///3、思路参考如下：
        ///①检测上学帮系统内的话题圈总数是否多于8个；
        ///如否，则在推荐位显示上学帮系统内的所有话题圈；
        ///如是，进入第②步；
        ///
        ///②检测用户所在当前城市内的达人创建的话题圈是否大于4；
        ///如是，随机选出4个话题圈显示，进入第③步；
        ///如否，显示检测到的所有话题圈，置于相应数量的推荐位，进入第③步；
        ///
        ///③检测用户所在省份除当前城市以外的城市所有达人创建话题圈的数量是否大于6-②；
        ///如是，随机抽取相应数量的话题圈显示，使②+③=6，进入第④步；
        ///如否，显示检测到的所有话题圈，进入第④步；
        ///
        ///④检测其他省份的达人所建话题圈是否大于8-②-③；
        ///如是，随机抽取相应数量的话题圈，使②+③+④=8，结束；
        ///如否，显示检测到的所有话题圈，重复进行第②步但需排重掉已显示的话题圈，直到②+③+④=8，结束。
        /// </para>
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseResult> RecommendCircles(int count = 8)
        {
            var circles = new List<CircleDetailDto>();

            //if (User.Identity.IsAuthenticated)
            //{
            //    var loginUser = _userService.GetUserInfoDetail(UserId.Value);
            //    if (loginUser?.City.HasValue == true)
            //    {
            //        var finds = await _circleService.GetCircles(4, 0, loginUser.City.Value);
            //        if (finds?.Any() == true) circles.AddRange(finds);
            //        if (finds.Count() < 4)
            //        {
            //            finds = await _circleService.GetCircles(6 - circles.Count(), loginUser.City.Value / 1000, 0, circles.Select(p => p.Id));
            //            if (finds?.Any() == true) circles.AddRange(finds);
            //            //loginUser = _userService.GetUserInfoDetail(UserId.Value);
            //        }
            //    }
            //}

            var cityCode = Request.GetLocalCity();
            //拿本市圈子
            var finds = await _circleService.GetCircles(10, 0, cityCode);
            if (finds?.Any() == true) circles.AddRange(CommonHelper.ListRandom(finds).Take(4));
            //拿本省圈子
            finds = await _circleService.GetCircles(10 - circles.Count(), cityCode / 1000, 0, circles.Select(p => p.Id));
            if (finds?.Any() == true) circles.AddRange(CommonHelper.ListRandom(finds).Take(6 - circles.Count()));
            //拿全国圈子
            finds = await _circleService.GetCircles(20 - circles.Count(), 0, 0, circles.Select(p => p.Id));
            if (finds?.Any() == true) circles.AddRange(CommonHelper.ListRandom(finds).Take(8 - circles.Count()));

            if (circles.Any())
            {
                if (User.Identity.IsAuthenticated)
                {
                    var checks = await _circleFollowerService.CheckIsFollow(circles.Select(p => p.Id), UserId.Value);
                    circles.ForEach(item =>
                    {
                        item.IsFollowed = checks[item.Id];
                    });
                }

                return ResponseResult.Success(circles);
            }
            else
            {
                return ResponseResult.Failed("参数错误");
            }
        }

        /// <summary>
        /// 我的圈子
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseResult> My(int pageIndex = 1, int pageSize = 4)
        {
            var result = new List<MyCircleItemDto>();
            long totalCount = 0;
            if (!User.Identity.IsAuthenticated) return ResponseResult.Success(new { isLogin = false, result });

            var isCircleMaster = await Task.Run(() =>
            {
                return _circleService.CheckIsHasCircle(UserId.Value);
            });


            var offset = pageSize * (pageIndex - 1);

            if (isCircleMaster)
            {
                totalCount++;
                if (pageIndex < 2) //第一页
                {
                    var selfCirclesResult = _circleService.GetCircles(UserId.Value);
                    if (selfCirclesResult.Status && selfCirclesResult.Data?.Any() == true)//一个达人只有一个自建圈子 , 不用考虑排序
                    {
                        foreach (var item in selfCirclesResult.Data)
                        {
                            result.Add(new MyCircleItemDto()
                            {
                                CircleId = item.Id,
                                Cover = item.Cover,
                                IsCircleMaster = true,
                                Name = item.Name,
                                CircleMasterName = item.CircleMasterName,
                                BGColor = item.BGColor,
                                FOLLOWERCOUNT = item.FollowCount
                            });
                        }
                    }
                }
                else
                {
                    offset--;
                }
            }
            if (result.Count < pageSize)//获取Ta关注的圈子
            {
                var joinCircles = await _circleService.GetMyCircles(UserId.Value, offset, pageSize - result.Count);
                if (joinCircles.Status && joinCircles.Data?.Any() == true)
                {
                    totalCount += joinCircles.Total;

                    //foreach (var item in CommonHelper.ListRandom(joinCircles.Data))
                    foreach (var item in joinCircles.Data)
                    {
                        result.Add(new MyCircleItemDto()
                        {
                            CircleId = item.CircleId,
                            Cover = item.Cover,
                            Name = item.Name,
                            BGColor = item.BGColor,
                            NEWTOPICCOUNT = item.NEWTOPICCOUNT,
                            NEWREPLYCOUNT = item.NEWREPLYCOUNT,
                            CircleMasterName = item.CircleMasterName,
                            FOLLOWERCOUNT = item.FOLLOWERCOUNT
                        });
                    }
                }
            }

            if (result.Any())
            {
                //圈子新动态
                var hasNews = _circleService.GetCirclesHasNews(result.Select(p => p.CircleId), UserId.Value);

                foreach (var item in result)
                {
                    item.NewestTopics = await _topicService.GetNewestDynamicTimeTopics(item.CircleId, 3);
                    item.HasNews = hasNews.GetValueOrDefault(item.CircleId, false);
                }
            }

            return ResponseResult.Success(new { isLogin = true, result, total = totalCount });
        }

        /// <summary>
        /// 全部圈子
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="orderType"></param>
        /// <returns></returns>
        [HttpGet]
        //[Authorize]
        public async Task<ResponseResult> AllCircle(int pageIndex = 1, int pageSize = 14, int orderType = 1)
        {
            var finds = await _circleService.GetAllCircles(UserId.Value, pageSize * --pageIndex, pageSize, orderType);
            if (finds.Item2 > 0)
            {
                return ResponseResult.Success(new
                {
                    Items = finds.Item1,
                    Total = finds.Item2,
                    IsLogin = User.Identity.IsAuthenticated
                });
            }
            return ResponseResult.Failed(ResponseCode.NoFound);
        }

        /// <summary>
        /// 更新圈子统计数据
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseResult> UpdateCircleCountData()
        {
            var result = await _circleService.UpdateCircleCountData();
            if (result > 0)
            {
                return ResponseResult.Success($"Affected rows: {result}");
            }
            return ResponseResult.Failed("update error");
        }



        private async Task SendWelcomCreateCirclMsg(Guid circleId, string openId)
        {
            if (!string.IsNullOrEmpty(openId))
            {
                try
                {
                    NewsCustomMsg msg = new NewsCustomMsg()
                    {
                        ToUser =openId,
                        Url = this._topicOption.WelcomTalentCreateKFMsg.RedirectUrl.Replace("{circleId}", circleId.ToString("N")),
                        Description = _topicOption.WelcomTalentCreateKFMsg.Description,
                        PicUrl = _topicOption.WelcomTalentCreateKFMsg.ImgUrl,
                        Title = _topicOption.WelcomTalentCreateKFMsg.Title
                    };
                    var accessToken = _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" }).GetAwaiter().GetResult();
                    await _customMsgService.Send(accessToken.token, msg);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, null);
                }
            }
        }


        [HttpPost]
        [ExportDataFilter( ExportDataToMailRequestParamName = "request")]
        public async Task<ResponseResult> DataExportToMails([FromBody] ExportDataToMailRequest request)
        {

            XSSFWorkbook workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet("sheet1");
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 2, 0, 0));
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 2, 1, 1));
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 2, 2, 2));
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 2, 3, 3));
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 2, 4, 4));
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 5, 16));
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 2, 17, 17));
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 18, 19));

            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, 5, 6));
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, 7, 8));
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, 9, 10));
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, 11, 12));
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, 13, 14));
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 1, 15, 16));
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 2, 18, 18));
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(1, 2, 19, 19));

            var row = sheet.CreateRow(0);
            var cell00 = row.CreateCell(0);
            cell00.SetCellValue("话题圈ID");
            var cell01 = row.CreateCell(1);
            cell01.SetCellValue("话题圈名称");
            var cell02 = row.CreateCell(2);
            cell02.SetCellValue("新用户数");
            var cell03 = row.CreateCell(3);
            cell03.SetCellValue("关注话题圈用户数（除去灌水账号部分）");
            var cell04 = row.CreateCell(4);
            cell04.SetCellValue("留存用户数");
            var cell05 = row.CreateCell(5);
            cell05.SetCellValue("互动数");
            var cell017 = row.CreateCell(17);
            cell017.SetCellValue("圈主发帖数");
            var cell018 = row.CreateCell(18);
            cell018.SetCellValue("圈主回复数");
            var row1 = sheet.CreateRow(1);
            var cell15 = row1.CreateCell(5);
            var cell17 = row1.CreateCell(7);
            var cell19 = row1.CreateCell(9);
            var cell111 = row1.CreateCell(11);
            var cell113 = row1.CreateCell(13);
            var cell115 = row1.CreateCell(15);
            var cell118 = row1.CreateCell(18);
            var cell119 = row1.CreateCell(19);
            cell15.SetCellValue("用户点赞数	");
            cell17.SetCellValue("用户收藏数	");
            cell19.SetCellValue("用户分享数	");
            cell111.SetCellValue("用户提问数");
            cell113.SetCellValue("用户发帖数");
            cell115.SetCellValue("用户评论数");
            var row2 = sheet.CreateRow(2);
            for (int i = 5; i < 17; i++)
            {

                var cell2i = row2.CreateCell(i);
                cell2i.SetCellValue((i % 2) == 0 ? "真实" : "灌水");
            }

            cell118.SetCellValue("有效回复数（字数≥50字）");
            cell119.SetCellValue("无效回复数（字数<50字）");

            var now = DateTime.Now;
            var btime = now.Date.AddDays(-7);
            var etime = now.Date;
            int dataRowIndex = 3;
            DataTable table = new DataTable();
            var datas = await _circleService.ExportStaticData(btime, etime);
            foreach (var item in datas)
            {
                var props = item as IDictionary<string, object>;
                var datarow = sheet.CreateRow(dataRowIndex++);
                int cellindex = 0;
                foreach (var prop in props)
                {
                    var cell = datarow.CreateCell(cellindex++);
                    cell.SetCellValue(prop.Value?.ToString());
                }

            }

            MemoryStream ms = new MemoryStream();
            workbook.Write(ms, true);
            ms.Position = 0;

            Attachment attachment = new Attachment(contentStream: ms, name: $"话题圈运营数据报表【{DateTime.Now.ToString("yyMMdd")}】.xlsx");
            await _emailClient.NotifyByMailAsync($"话题圈运营数据报表【{DateTime.Now.ToString("yyMMdd")}】", "话题圈运营数据报表。", request.MainMails.ToArray(), new List<object>() { attachment }, request.CCMails.ToArray());
            return ResponseResult.Success("ok");
        }
    }
}
