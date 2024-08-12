using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PMS.TopicCircle.Application.Dtos;
using PMS.TopicCircle.Application.Services;
using PMS.TopicCircle.Domain.Dtos;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Common;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.RabbitMQ;
using Sxb.Web.Areas.TopicCircle.Filters;
using Sxb.Web.Areas.TopicCircle.Models;
using Sxb.Web.Authentication.Attribute;
using Sxb.Web.Common;
using Sxb.Web.Controllers;
using Sxb.Web.Filters;
using Sxb.Web.Response;
using Sxb.Web.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using WeChat;
using WeChat.Model;
using static PMS.TopicCircle.Domain.Enum.GlobalEnum;
using ProductManagement.Framework.Foundation;
using WeChat.Interface;
using ProductManagement.Framework.AspNetCoreHelper.Filters;
using IUserService = PMS.UserManage.Application.IServices.IUserService;

namespace Sxb.Web.Areas.TopicCircle.Controllers
{
    [Route("tpc/[controller]/[action]")]
    [ApiController]
    public class TopicController : ApiBaseController
    {
        private readonly IEventBus _eventBus;

        private readonly ITopicService _topicService;
        private readonly ITopicReplyService _topicReplyService;
        private readonly ITalentService _talentService;
        private readonly IFileUploadService _fileUploadService;
        private readonly IKeyValueService _keyValueService;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly IWeChatQRCodeService  _weChatQRCodeService;
        private readonly IUserService _userService;
        ICircleFollowerService _circleFollowerService;
        IWeChatAppClient _weChatAppClient;
        ITagService _tagService;
        ICircleService _circleService;


        public TopicController(ICircleFollowerService circleFollowerService,
            ITopicService topicService, ITopicReplyService topicReplyService, IEventBus eventBus, ITalentService talentService, IFileUploadService fileUploadService
            , IKeyValueService keyValueService, ITagService tagService, ICircleService circleService
            , IUserService userService
            , IEasyRedisClient easyRedisClient
            , IWeChatQRCodeService  weChatQRCodeService
            , IWeChatAppClient weChatAppClient)
        {
            _circleService = circleService;
            _circleFollowerService = circleFollowerService;
            _topicService = topicService;
            _topicReplyService = topicReplyService;
            _eventBus = eventBus;
            _talentService = talentService;
            _fileUploadService = fileUploadService;
            _keyValueService = keyValueService;
            _easyRedisClient = easyRedisClient;
            _weChatQRCodeService = weChatQRCodeService;
            _userService = userService;
            _weChatAppClient = weChatAppClient;
            _tagService = tagService;
        }

        /// <summary>
        /// 关注 for test
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ResponseResult AttentionUser(Guid attionUserId)
        {
            var data = _talentService.AttentionUser(UserId.Value, attionUserId);
            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 查看话题详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ValidateTopicPermission(TopicPermission.Read)]
        public async Task<ResponseResult> Get(Guid id)
        {
            var data = await _topicService.Get(id, UserId);
            CompleteUrl(data);
            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 获取话题卡片信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseResult GetTopicCardInfo(Guid id)
        {
            var data = _topicService.GetSimple(id);
            return ResponseResult.Success(new {
                Time = data.Time.ConciseTime("yyyy-MM-dd HH:mm"),
                data.CircleName,
                data.UserName,
                data.HeadImgUrl,
                data.ReplyCount,
                data.Content
            });
        }

        /// <summary>
        /// 新增话题
        /// </summary>
        /// <param name="topicAddDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [BindMobile]
        [ValidateTopicPermission(TopicPermission.Create)]
        [ValidateWebContent(ContentParam = "topicAddDto")]
        public ResponseResult<object> Add([FromBody] TopicAddRequest topicAddDto)
        {

            topicAddDto.UserId = UserId.Value;
            return _topicService.Add(topicAddDto);
        }

        /// <summary>
        /// 编辑话题
        /// </summary>
        /// <param name="topicAddDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [BindMobile]
        [ValidateWebContent(ContentParam = "topicAddDto")]
        public ResponseResult Edit([FromBody] TopicAddRequest topicAddDto)
        {
            topicAddDto.UserId = UserId.Value;
            return _topicService.Edit(topicAddDto);
        }

        /// <summary>
        /// 删除话题
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ResponseResult Delete(Guid id)
        {
            return _topicService.Delete(id, UserId.Value);
        }

        /// <summary>
        /// 点赞话题
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ResponseResult Like(Guid id)
        {
            return _topicReplyService.Like(id, UserId.Value);

            //_eventBus.Publish(new SyncTopicLikeMessage()
            //{
            //    Id = id,
            //    UserId = userId.Value
            //});
            //return ResponseResult.Success();
        }

        /// <summary>
        /// 置顶话题
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ResponseResult Top(Guid id)
        {
            return _topicService.Top(id, UserId.Value, cancel: false);
        }

        /// <summary>
        /// 取消置顶话题
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ResponseResult CancelTop(Guid id)
        {
            return _topicService.Top(id, UserId.Value, cancel: true);
        }

        /// <summary>
        /// 设为精品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ResponseResult Good(Guid id)
        {
            return _topicService.Good(id, UserId.Value, isGood: true);
        }

        /// <summary>
        /// 取消设为精品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ResponseResult CancelGood(Guid id)
        {
            return _topicService.Good(id, UserId.Value, isGood: false);
        }

        /// <summary>
        /// 收藏话题 / 取消收藏话题
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ResponseResult Follow(Guid id)
        {
            return _topicService.Follow(id, UserId.Value);
        }

        /// <summary>
        /// 获取话题列表
        /// </summary>
        /// <param name="circleId"></param>
        /// <param name="userId"></param>
        /// <param name="topType"></param>
        /// <param name="isGood"></param>
        /// <param name="isQA"></param>
        /// <returns></returns>
        public ResponseResult GetList([FromQuery] TopicSearchRequest search)
        {
            search.UserId = UserId;
            var data = _topicService.GetList(search.CircleId, search.UserId, search.TopType, search.IsGood, search.IsQA);
            CompleteUrl(data);
            return ResponseResult.Success(data);
        }
        /// <summary>
        /// 我的圈子动态(显示条数：3条)
        /// 显示用户关注所有圈子里有最新动态的帖子。最新动态指：有最新回复的帖子，或最新发出/编辑的帖子
        /// 按帖子最新动态的时间降序排列。
        /// </summary>
        /// <returns></returns>
        public ResponseResult GetDynamicList(int pageIndex = 1, int pageSize = 3)
        {
            if (!User.Identity.IsAuthenticated) return ResponseResult.Success();
            var data = _topicService.GetDynamicList(UserId.Value, pageIndex, pageSize);
            CompleteUrl(data);
            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 获取圈子内热门话题(显示数量：5个)
        /// </summary>
        /// <param name="circleId"></param>
        /// <returns></returns>
        [Description("圈内热论")]
        public ResponseResult GetCircleHotList(Guid circleId, int page = 1, int pageSize = 5)
        {
            var data = _topicService.GetCircleHotList(circleId, page, pageSize);
            CompleteUrl(data);
            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 获取热门话题列表(显示总数量：10个)
        /// 1.管理员置顶话题
        /// 2.同城达人的话题
        /// 3.全无, 搜其他话题
        /// </summary>
        /// <returns></returns>
        [Description("热门话题")]
        public ResponseResult GetHotList()
        {
            //int cityCode = Request.GetLocalCity();
            var data = _topicService.GetHotList(userId: null, cityCode: null);
            if (data != null)
            {
                foreach (var item in data)
                {
                    //热门话题显示最后回复时间
                    item.Time = item.LastReplyTime;
                }
            }

            CompleteUrl(data);
            return ResponseResult.Success(data);
        }
        /// <summary>
        /// 获取置顶话题列表
        /// </summary>
        /// <param name="circleId"></param>
        /// <returns></returns>
        public ResponseResult GetTopList(Guid circleId)
        {
            var data = _topicService.GetTopList(circleId);
            return ResponseResult.Success(data);
        }
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="circleId"></param>
        /// <param name="userId"></param>
        /// <param name="topType"></param>
        /// <param name="isGood"></param>
        /// <param name="isQA"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Description("搜索话题")]
        public async Task<ResponseResult> GetPagination([FromQuery] TopicSearchRequest search)
        {
            search.UserId = UserId;
            //var pagination = await _topicService.GetPagination(search.UserId, search.Keyword, search.CircleId, search.Type, search.IsGood, search.IsQA,
            //    search.Tags, search.StartTime, search.EndTime, search.PageIndex, search.PageSize);
            //if (search.CircleId == null)
            //    return ResponseResult.Failed("请选择圈子");

            var pagination = await _topicService.GetPaginationByEs(search.UserId, search.Keyword, search.CircleId, search.IsCircleOwner, search.Type, search.IsGood, search.IsQA,
                search.Tags, search.StartTime, search.EndTime, search.PageIndex, search.PageSize);

            CompleteUrl(pagination.Data);
            return ResponseResult.Success(pagination);
        }




        private void CompleteUrl(TopicDto topicDto)
        {
            return;
            if (topicDto == null || topicDto.Attachment == null)
                return;

            var attachUrl = topicDto.Attachment.AttachUrl;
            if (string.IsNullOrWhiteSpace(attachUrl) || attachUrl.StartsWith("https://") || attachUrl.StartsWith("http://"))
                return;

            if (topicDto.Attachment.Type != (int)TopicType.OuterUrl)
                topicDto.Attachment.AttachUrl = ConfigHelper.GetHost() + attachUrl;
        }

        private void CompleteUrl(List<TopicDto> data)
        {
            if (data == null || data.Count == 0)
                return;

            foreach (var item in data)
            {
                CompleteUrl(item);
            }
        }
        private void CompleteUrl(IEnumerable<TopicDto> data)
        {
            return;

            if (data == null || !data.Any())
                return;
            CompleteUrl(data.ToList());

        }


        /// <summary>
        /// 上传话题图片
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Description("上传单图")]
        public async Task<ResponseResult> UploadTopicImage()
        {
            if (Request.Form.Files.Any())
            {
                var img = Request.Form.Files[0];
                if (img == null)
                    return ResponseResult.Failed("请选择图片");
                var resp = await _fileUploadService.UploadTopicImage(img.FileName, img.OpenReadStream());
                if (resp != null)
                {
                    return ResponseResult.Success(new
                    {
                        resp.CdnUrl,
                        resp.Url
                    });
                }
            }
            return ResponseResult.Failed();

        }

        /// <summary>
        /// 批量上传话题图片
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Description("批量上传话题图片")]
        public async Task<ResponseResult> UploadTopicImages()
        {
            var img = Request.Form.Files;
            if (!img.Any())
            {
                return ResponseResult.Failed("请选择图片");
            }
            else
            {

                var results = new List<UploadImgeResponseDto>(img.Count);
                foreach (var file in img)
                {
                    var result = await _fileUploadService.UploadTopicImage(file.FileName, file.OpenReadStream());
                    if (result != null)
                        results.Add(result);
                }

                if (results != null)
                {
                    return ResponseResult.Success(results);
                }
                return ResponseResult.Failed();
            }


        }

        /// <summary>
        /// 获取加入圈子场景的关注二维码
        /// </summary>
        /// <param name="circleId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Description("获取查看帖子详情场景的关注二维码")]
        public async Task<ResponseResult> GetSubscribeQRCode(Guid topicId)
        {
            string sceneKey = $"{SubscribeCallBackType.viewTopicDetail}:isMiniApp_0:" + topicId.ToString("N");
            int expire_second = (int)TimeSpan.FromDays(30).TotalSeconds; //30天有效期的二维码
            var accessToken = await this._weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" });
            string subscribeHandleUrl = $"{ConfigHelper.GetHost()}/tpc/Share/SubscribeCallBack?DataId={topicId}&type={(int)SubscribeCallBackType.viewTopicDetail}";
            _ = this._easyRedisClient.AddStringAsync(sceneKey, subscribeHandleUrl, TimeSpan.FromSeconds(expire_second));
            var Scene = new CreateTempQRStrSceneCodeRequest(expire_second);
            Scene.SetScene(sceneKey);
            var qrcodeResponse = await _weChatQRCodeService.GenerateTempQRCode(accessToken.token, Scene);

            //获取登录账号绑定的微信昵称
            var weixinNickName = _userService.GetUserWeixinNickName(this.GetUserInfo().UserId);


            return ResponseResult.Success(new GetSubscribeQRCodeResponse()
            {
                ImgUrl = qrcodeResponse.ImgUrl,
                WeixinNickName = weixinNickName
            });
        }

        /// <summary>
        /// 获取加入圈子场景的关注二维码(微信小程序用)
        /// </summary>
        /// <param name="circleId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Description("获取查看帖子详情场景的关注二维码（微信小程序用）")]
        public async Task<ResponseResult> GetSubscribeQRCodeWxMiniProgram(Guid topicId)
        {
            string sceneKey = $"{SubscribeCallBackType.viewTopicDetail}:isMiniApp_1:" + topicId.ToString("N");
            int expire_second = (int)TimeSpan.FromDays(30).TotalSeconds; //30天有效期的二维码
            var accessToken = await this._weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" });
            string subscribeHandleUrl = $"{ConfigHelper.GetHost()}/tpc/Share/SubscribeCallBack?DataId={topicId}&type={(int)SubscribeCallBackType.viewTopicDetail}&isMiniApp=true";
            _ = this._easyRedisClient.AddStringAsync(sceneKey, subscribeHandleUrl, TimeSpan.FromSeconds(expire_second));
            var Scene = new CreateTempQRStrSceneCodeRequest(expire_second);
            Scene.SetScene(sceneKey);
            var qrcodeResponse = await _weChatQRCodeService.GenerateTempQRCode(accessToken.token, Scene);

            //获取登录账号绑定的微信昵称
            var weixinNickName = _userService.GetUserWeixinNickName(this.GetUserInfo().UserId);


            return ResponseResult.Success(new GetSubscribeQRCodeResponse()
            {
                ImgUrl = qrcodeResponse.ImgUrl,
                WeixinNickName = weixinNickName
            });
        }

        /// <summary>
        /// 精选话题
        /// <para>
        /// 【规则】
        ///1、显示总数量：10个
        ///2、推荐优先级与排序规则：
        ///（1）运营人员置顶帖子，如有多条置顶帖子，按帖子的总评论数（即评论+回复总数）降序排列，如帖子均无回复，则按发帖时间降序排列；
        ///（2）
        ///①读取所有圈子（不分城市）最新动态（最新动态包括帖子内发帖时间、帖子最后编辑时间、评论时间、回复时间，时间最新的即为最新动态时间点）
        ///的设为精华的帖子（不含仅圈主可见的帖子）；
        ///②读取的帖子数=10-话题圈首页置顶的帖子数
        ///③按读取到的帖子最新动态时间点降序排列；
        ///④如最新动态时间点相同的，则随机排序
        /// </para>
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseResult> ChosenTopics(int count = 10)
        {
            var result = new List<SimpleTopicDto>();
            var handPickTopics = await _topicService.GetListByIsHandPick(true, count);
            if (handPickTopics?.Any() == true) result.AddRange(handPickTopics);
            if (result.Count() < 10)
            {
                var isGoodTopics = await _topicService.GetIsGoodList(10 - handPickTopics.Count());
                if (isGoodTopics?.Any() == true) result.AddRange(isGoodTopics);
            }
            if (result?.Any() == true)
            {
                if (UserId.HasValue)
                {
                    var checks = await _circleFollowerService.CheckIsFollow(result.Select(p => p.CircleId), UserId);
                    result.ForEach(item =>
                    {
                        item.IsFollowed = checks[item.CircleId];
                    });
                }
                return ResponseResult.Success(result);
            }
            return ResponseResult.Failed(ResponseCode.NoFound);
        }

        [HttpGet]
        /// <summary>
        /// 热门话题
        /// </summary>
        /// <param name="count">获取条数</param>
        /// <returns></returns>
        public async Task<ResponseResult> HotTopics(int count = 10)
        {
            var finds = await _topicService.GetHotList(UserId, null, count);
            if (finds?.Any() == true) return ResponseResult.Success(data: finds);
            return ResponseResult.Failed(ResponseCode.NoFound);
        }


        [HttpGet]
        [Description("获取最多讨论")]
        public ResponseResult GetMoreDiscuss(Guid circleID, int pageIndex = 1, int pageSize = 10)
        {
            var topics = _topicService.MoreDiscuss(circleID, pageSize * --pageIndex, pageSize);

            return ResponseResult.Success(topics);
        }

        /// <summary>
        /// 相关话题
        /// <para>
        /// 不够limit时 , 用ReplyCount倒序填充
        /// </para>
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseResult> GetRelatedTopics(Guid topicID, int pageIndex = 1, int pageSize = 5)
        {
            List<SimpleTopicDto> topics = new List<SimpleTopicDto>();
            var tags = await _tagService.GetByTopicID(topicID);
            if (tags?.Any() == true)
            {
                var relatedFinds = await _topicService.GetRelatedTopics(topicID, tags.Select(p => p.Id), pageSize * --pageIndex, pageSize, true);
                if (relatedFinds?.Any() == true)
                {
                    topics.AddRange(relatedFinds);
                }
            }
            if (topics.Count() < pageSize)
            {
                var topic = _topicService.Get(topicID);
                if (topic != null)
                {
                    var finds = await _topicService.GetByReplyCount(topic.CircleId, 0, pageSize - topics.Count(), topics.Select(p => p.Id).Append(topicID));
                    if (finds?.Any() == true)
                    {
                        topics.AddRange(finds);
                    }
                }
            }
            return ResponseResult.Success(topics);
        }
    }
}
