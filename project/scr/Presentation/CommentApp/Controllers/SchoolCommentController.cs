using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CommentApp.Models.School;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using PMS.CommentsManage.Domain.Entities;
using PMS.School.Application.IServices;
using PMS.School.Domain.Entities;
using PMS.UserManage.Application.IServices;
using ProductManagement.Infrastructure.Toolibrary;
using Sxb.Web.Models;
using Sxb.Web.Models.Comment;
using PMS.CommentsManage.Domain.Common;
using ProductManagement.Framework.Foundation;
using PMS.School.Application.ModelDto;
using Sxb.Web.Models.Replay;
using PMS.CommentsManage.Application.ModelDto.Reply;
using PMS.CommentsManage.Application.Common;
using Sxb.Web.RequestModel;
using Sxb.Web.Response;
using Sxb.Web.Models.User;
using Sxb.Web.Utils;
using Microsoft.AspNetCore.Http;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Sxb.Web.Authentication.Attribute;
using PMS.UserManage.Domain.Common;
using PMS.Infrastructure.Application.ModelDto;
using ProductManagement.Framework.Cache.Redis;
using Newtonsoft.Json;
using PMS.Infrastructure.Application.IService;
using PMS.CommentsManage.Application.IServices.IMQService;
using System.Diagnostics;
using PMS.CommentsManage.Domain.Entities.Total;
using Sxb.Web.ViewModels.Comment;
using ProductManagement.API.Http.Interface;
using Sxb.Web.Middleware;
using ProductManagement.API.Http.RequestCommon;
using PMS.UserManage.Application.ModelDto;
using Sxb.Api.RequestModel.RequestOption;
using Sxb.Api.RequestModel.RequestEnum;
using Sxb.Web.ViewModels.Api;
using Sxb.Web.Common;
using System.ComponentModel;
using ProductManagement.Infrastructure.Exceptions;
using Microsoft.Extensions.Logging;
using ProductManagement.API.Aliyun;

namespace Sxb.Web.Controllers
{
    using ProductManagement.API.Aliyun;
    using static PMS.UserManage.Domain.Common.EnumSet;

    public class SchoolCommentController : BaseController
    {

        private readonly IMessageService _inviteStatus;
        private readonly IUserServiceClient _userMessage;
        private readonly ImageSetting _setting;
        private readonly ISchoolService _schoolService;
        private readonly ISchoolInfoService _schoolInfoService;
        private readonly ISchoolCommentScoreService _commentScoreService;
        private readonly ISchoolCommentService _commentService;
        private readonly ISchoolImageService _schoolImageService;
        private readonly ISchoolTagService _schoolTagService;
        private readonly IGiveLikeService _likeservice;
        private readonly IUserService _userService;
        private readonly ISchoolCommentReplyService _commentReplyService;

        private readonly IUserServiceClient _userServiceClient;


        private readonly IEasyRedisClient _easyRedisClient;
        private readonly ICityInfoService _cityService;

        private readonly IText _text;

        //事件总线
        private readonly ISchoolCommentLikeMQService _commentLikeMQService;

        private readonly ILogger<SchoolCommentController> _logger;

        private readonly IMapper _mapper;
        public SchoolCommentController(IOptions<ImageSetting> set,
            IMessageService inviteStatus,
            ISchoolCommentReplyService commentReplyService,
            IUserService userService,
            ISchoolTagService schoolTagService,
            ISchoolService schoolService, ISchoolInfoService schoolInfoService,
            ISchoolCommentScoreService commentScoreService,
            ISchoolCommentService commentService,
            ISchoolImageService schoolImageService,
            IGiveLikeService likeservice,
            IEasyRedisClient easyRedisClient,
            ICityInfoService cityService,
            ISchoolCommentLikeMQService commentLikeMQService,
            IUserServiceClient userServiceClient,
            IUserServiceClient userMessage,
            IMapper mapper, IText text, ILoggerFactory loggerFactory)
        {
            _inviteStatus = inviteStatus;
            _setting = set.Value;
            _schoolService = schoolService;
            _schoolInfoService = schoolInfoService;
            _commentScoreService = commentScoreService;
            _commentService = commentService;
            _schoolImageService = schoolImageService;
            _schoolTagService = schoolTagService;
            _likeservice = likeservice;
            _userService = userService;
            _commentReplyService = commentReplyService;
            _cityService = cityService;

            _commentLikeMQService = commentLikeMQService;

            _userServiceClient = userServiceClient;
            _easyRedisClient = easyRedisClient;
            _mapper = mapper;

            _userMessage = userMessage;

            _logger = loggerFactory.CreateLogger<SchoolCommentController>();

            _text = text;


        }

        [Description("点评页")]
        public IActionResult Index(Guid SchoolId, int query = 1, CommentListOrder commentListOrder = CommentListOrder.None)
        {

            if (SchoolId == null || SchoolId == default(Guid))
            {
                throw new NotFoundException();
            }
            else
            {
                return RedirectToAction("School", "Comment", new { Id = SchoolId });
            }

            #region
            //var school = _schoolInfoService.QueryESchoolInfo(SchoolId);
            //if (school != null)
            //{
            //    _userServiceClient.AddHistory(new ProductManagement.API.Http.Model.AddHistory() { dataID = SchoolId, dataType = ProductManagement.API.Http.RequestCommon.MessageDataType.SchoolSection, cookies = HttpContext.Request.GetAllCookies() });
            //    //检测当前用户身份 （true：校方用户 |false：普通用户）
            //    ViewBag.isSchoolRole = false;
            //    //检测是否关注该学校
            //    ViewBag.isCollected = false;
            //    Guid UserId = Guid.Empty;
            //    if (User.Identity.IsAuthenticated)
            //    {
            //        var user = User.Identity.GetUserInfo();
            //        UserId = user.UserId;
            //        //检测当前用户身份 （true：校方用户 |false：普通用户）
            //        ViewBag.isSchoolRole = user.Role.Contains((int)PMS.UserManage.Domain.Common.UserRole.School);
            //        //检测改用户是否关注该学校
            //        var checkCollectedSchool = await _userServiceClient.IsCollected(new ProductManagement.API.Http.Model.IsCollected() { userId = UserId, dataID = SchoolId });
            //        ViewBag.isCollected = checkCollectedSchool.iscollected;
            //    }

            //    List<AreaDto> history = new List<AreaDto>();
            //    if (UserId != default(Guid))
            //    {
            //        string Key = $"SearchCity:{UserId}";
            //        var historyCity = await _easyRedisClient.SortedSetRangeByRankAsync<AreaDto>(Key, 0, 10, StackExchange.Redis.Order.Descending);
            //        history.AddRange(historyCity.ToList());
            //    }


            //    ViewBag.HotCity = JsonConvert.SerializeObject(await _cityService.GetHotCity());

            //    ViewBag.History = JsonConvert.SerializeObject(history);

            //    var adCodes = await _cityService.IntiAdCodes();
            //    //城市编码
            //    ViewBag.AllCity = JsonConvert.SerializeObject(adCodes);

            //    var extension = _mapper.Map<SchoolInfoDto, SchoolExtensionVo>(school);

            //    //var schoolComment = _commentScoreService.GetSchoolScoreById(SchoolId);

            //    extension.Score = school.SchoolAvgScore;
            //    extension.StartTotal = school.SchoolStars;

            //    //学校数据统计
            //    var schoolTotal = _mapper.Map<List<SchoolCommentTotal>, List<SchoolCommentTotalVo>>(_commentService.CurrentCommentTotalBySchoolId(school.SchoolId));
            //    var AllExtension = _schoolService.GetSchoolExtName(school.SchoolId);
            //    for (int i = 0; i < AllExtension.Count(); i++)
            //    {
            //        var item = schoolTotal.Where(x => x.SchoolSectionId == AllExtension[i].Value).FirstOrDefault();
            //        int currentIndex = schoolTotal.IndexOf(item);
            //        if (item == null)
            //        {
            //            if (AllExtension[i].Key != null) 
            //            {
            //                schoolTotal.Add(new SchoolCommentTotalVo() { Name = AllExtension[i].Key, SchoolSectionId = AllExtension[i].Value, TotalType = QueryCondition.Other.Description() });
            //            }
            //        }
            //        else
            //        {
            //            item.Name = AllExtension[i].Key;
            //            schoolTotal[currentIndex] = item;
            //        }
            //    }

            //    //将其他分部类型学校进行再次排序
            //    for (int i = 8; i < schoolTotal.Count(); i++)
            //    {
            //        schoolTotal[i].TotalTypeNumber = i+1;
            //    }

            //    //排除所有下架学校标签筛选
            //    schoolTotal.Where(x => x.Name == null && x.SchoolSectionId != Guid.Empty)?.ToList().ForEach(x => {
            //        schoolTotal.Remove(x);
            //    });

            //    ViewBag.CommentTotal = schoolTotal;

            //    //只有筛选条件为全部时，才加载最精选点评数据
            //    if ((int)query == 1)
            //    {
            //        List<CommentExhibition> commentDetail = new List<CommentExhibition>();
            //        //获取前三篇精选点评
            //        var comment = await _commentService.SelectedThreeComment(extension.SchoolId, UserId, (QueryCondition)query, commentListOrder);

            //        var Exhibtion = _mapper.Map<List<SchoolCommentDto>, List<CommentList>>(comment);

            //        var UserIds = new List<Guid>();
            //        UserIds.AddRange(comment.GroupBy(q => q.UserId).Select(p => p.Key).ToList());
            //        var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());

            //        var extSchoolIds = Exhibtion.GroupBy(q => q.SchoolSectionId).Select(q => q.Key).ToList();

            //        if (extSchoolIds.Count() > 0)
            //        {
            //            var schoolInfos = _schoolInfoService.GetSchoolSectionByIds(extSchoolIds);
            //            for (int i = 0; i < Exhibtion.Count(); i++)
            //            {
            //                for (int j = 0; j < Exhibtion[i].Images.Count; j++)
            //                {
            //                    string tempImage = Exhibtion[i].Images[j];

            //                    Exhibtion[i].Images[j] = _setting.QueryImager + tempImage;
            //                }
            //                var schoolInfo = schoolInfos.FirstOrDefault(q => q.SchoolSectionId == Exhibtion[i].SchoolSectionId);
            //                Exhibtion[i].School = _mapper.Map<SchoolInfoDto, CommentList.SchoolInfo>(schoolInfo);

            //                if (Exhibtion[i].IsAnony)
            //                {
            //                    Exhibtion[i].UserName = "匿名用户";
            //                    Exhibtion[i].UserHeadImage = "https://cdn.sxkid.com//images/AppComment/defaultUserHeadImage.png";
            //                }
            //                else
            //                {
            //                    Exhibtion[i].UserName = Users.FirstOrDefault(q => q.Id == Exhibtion[i].UserId)?.NickName;
            //                }
            //                Exhibtion[i].UserHeadImage = Users.FirstOrDefault(q => q.Id == Exhibtion[i].UserId)?.HeadImgUrl;
            //            }
            //        }
            //        ViewBag.CommentExhibition = Exhibtion;
            //    }   
            //    ViewBag.School = extension;


            //    //保存当前选项卡选中值
            //    ViewBag.SelectedTab = query;

            //    ViewBag.SchoolId = school;
            //}
            //else 
            //{
            //    throw new NotFoundException();
            //}
            //return View();
            #endregion
        }

        [Description("点评详情页")]
        public IActionResult CommentDetail(Guid CommentId)
        {

            if (CommentId == null || CommentId == default(Guid))
            {
                throw new NotFoundException();
            }
            else
            {
                var redirectUrl = $"/comment/detail/{CommentId}";
                Response.Redirect(redirectUrl);
                Response.StatusCode = 301;
                return Json(new { });

                //return RedirectToAction("Detail", "Comment", new { CommentId = CommentId });
            }

            #region
            ////检测当前用户身份 （true：校方用户 |false：普通用户）
            //ViewBag.isSchoolRole = false;
            ////检测是否关注该点评
            //ViewBag.isCollected = false;

            //_userServiceClient.AddHistory(new ProductManagement.API.Http.Model.AddHistory() { dataID = CommentId, dataType = ProductManagement.API.Http.RequestCommon.MessageDataType.Comment, cookies = HttpContext.Request.GetAllCookies() });

            //Guid userId = Guid.Empty;
            //TimeSpan s12;
            //if (User.Identity.IsAuthenticated)
            //{
            //    var user = User.Identity.GetUserInfo();
            //    userId = user.UserId;
            //    //检测当前用户身份 （true：校方用户 |false：普通用户）
            //    ViewBag.isSchoolRole = user.Role.Contains((int)PMS.UserManage.Domain.Common.UserRole.School);

            //    Stopwatch sw7 = new Stopwatch();
            //    sw7.Start();
            //    //检测改用户是否关注该点评
            //    var checkCollectedSchool = await _userServiceClient.IsCollected(new ProductManagement.API.Http.Model.IsCollected() { userId = userId, dataID = CommentId });
            //    sw7.Stop();
            //    s12 = sw7.Elapsed;

            //    Console.WriteLine("检查是否关注时间："+s12.Seconds);

            //    ViewBag.isCollected = checkCollectedSchool.iscollected;
            //}
            ////CommentExhibition commentDetail = new CommentExhibition();

            //Stopwatch swt = new Stopwatch();
            //swt.Start();
            //var comment = _commentService.QuerySchoolComment(CommentId);
            //swt.Stop();
            //TimeSpan s0 = swt.Elapsed;
            //Console.WriteLine("获取点评信息：" + s0.Seconds);

            //if (comment == null)
            //{
            //    throw new NotFoundException();
            //}
            //_commentService.UpdateCommentViewCount(CommentId);

            //Stopwatch swt1 = new Stopwatch();
            //swt1.Start();
            //var commentReplyselected = _commentReplyService.SelectedCommentReply(CommentId);
            //swt.Stop();
            //TimeSpan s1 = swt1.Elapsed;

            //Console.WriteLine("获取点评精选回复：" + s1.Seconds);

            //var UserIds = new List<Guid>()
            //{
            //    comment.CommentUserId
            //};
            //UserIds.AddRange(commentReplyselected.GroupBy(q => q.UserId).Select(p => p.Key).ToList());
            //UserIds.Add(userId);

            //Stopwatch swt2 = new Stopwatch();
            //swt2.Start();
            //var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());
            //swt2.Stop();
            //TimeSpan s2 = swt1.Elapsed;
            //Console.WriteLine("获取用户信息：" + s2.Seconds);

            //List<Guid> replyIds = commentReplyselected.GroupBy(q => q.Id).Select(s => s.Key).ToList();
            //replyIds.Add(CommentId);
            //var isLikes = _likeservice.CheckCurrentUserIsLikeBulk(userId, replyIds);



            //var schoolComment = _mapper.Map<SchoolComment, SchoolCommentVo>(comment);

            //if (schoolComment.IsAnony)
            //{
            //    schoolComment.UserInfo = new UserInfoVo
            //    {
            //        NickName = "匿名用户",
            //        HeadImager = "https://cdn.sxkid.com//images/AppComment/defaultUserHeadImage.png"
            //    };
            //}
            //else
            //{
            //    schoolComment.UserInfo = _mapper.Map<UserInfoVo>(Users.FirstOrDefault(p => p.Id == comment.CommentUserId));
            //}

            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //var schoolDto = _schoolInfoService.QueryESchoolInfo(comment.SchoolSectionId);
            //sw.Stop();
            //TimeSpan ts = sw.Elapsed;

            //Console.WriteLine("用户信息整合：" + ts.Seconds);

            //var school = _mapper.Map<SchoolInfoDto, SchoolExtensionVo>(schoolDto);
            //if (school == null)
            //{
            //    throw new NotFoundException();
            //}

            //Stopwatch sw5 = new Stopwatch();
            //sw5.Start();
            //var schoolImages = _schoolImageService.GetImageByDataSourceId(comment.Id).ToList();
            //schoolImages.ForEach(x => x.ImageUrl = _setting.QueryImager + x.ImageUrl);
            //var tags = _schoolTagService.GetSchoolTagByCommentId(comment.Id);
            //var isLike = isLikes.FirstOrDefault(p => p.SourceId == comment.Id) != null
            //                    && isLikes.FirstOrDefault(p => p.SourceId == comment.Id).IsLike;
            //sw5.Stop();
            //TimeSpan tsz = sw5.Elapsed;

            //Console.WriteLine("图片信息整合：" + tsz.Seconds);

            //Stopwatch sw6 = new Stopwatch();
            //sw6.Start();
            //if (commentReplyselected.Count > 0)
            //{
            //    //点评回复精选
            //    var SchoolCommentReplyVoselected = _mapper.Map<List<SchoolCommentReply>, List<SchoolCommentReplyVo>>(commentReplyselected);

            //    foreach (var q in SchoolCommentReplyVoselected)
            //    {
            //        q.IsSelected = true;
            //        q.IsLike = isLikes.FirstOrDefault(p => p.SourceId == q.Id) != null && isLikes.FirstOrDefault(p => p.SourceId == q.Id).IsLike;
            //        q.UserInfo = _mapper.Map<UserInfoVo>(Users.FirstOrDefault(p => p.Id == q.UserId)).ToAnonyUserName(q.IsAnony);
            //    }
            //    var commentReplies = SchoolCommentReplyVoselected;
            //    ViewBag.CommentReplies = commentReplies;
            //}
            //sw6.Stop();
            //TimeSpan tszx = sw6.Elapsed;

            //Console.WriteLine("精选回复信息整合：" + tszx.Seconds);

            ////ViewBag.commentDetail = commentDetail;

            //ViewBag.SchoolComment = schoolComment;
            //ViewBag.School = school;
            //ViewBag.SchoolImages = schoolImages;
            //ViewBag.Tags = tags;
            //ViewBag.IsLike = isLike;

            //ViewBag.CurrentUser = Users.Where(x => x.Id == userId).FirstOrDefault();

            ////学校平均分
            //ViewBag.SchoolScore = schoolDto.SchoolStars;
            //return View();
            #endregion
        }

        /// <summary>
        /// 点评点赞
        /// </summary>
        /// <param name="CommentId"></param>
        /// <returns></returns>
        [Authorize]
        [Description("点评点赞")]
        public ResponseResult LikeComment(Guid CommentId, int LikeType)
        {
            try
            {
                if (CommentId == Guid.Empty)
                {
                    _logger.LogInformation($"点赞异常，Id为空，来源地址：{Request.Headers["Referer"].FirstOrDefault()}");
                }

                var user = User.Identity.GetUserInfo();

                //获取渠道标识
                string channel = GetChannelHelper.GetChannelByRequestReferer(HttpContext.Request.Headers["Referer"].ToString());


                GiveLike giveLike = new GiveLike
                {
                    UserId = user.UserId,
                    LikeType = (LikeType)LikeType,
                    SourceId = CommentId,
                    Channel = channel
                };

                _commentLikeMQService.SchoolCommentLike(giveLike);
                //int likeTotal = _likeservice.AddLike(giveLike, out LikeStatus status);
                //bool isLike = status == LikeStatus.Like ? true : false;
                //SchoolCommentLike schoolCommentLike = new SchoolCommentLike(likeTotal, isLike);

                return ResponseResult.Success();
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        [Authorize]
        [Description("点赞点评")]
        public ResponseResult LikeSchoolComment(Guid commentId)
        {
            return LikeComment(commentId, (int)LikeType.Comment);
        }

        /// <summary>
        /// 点评回复
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [BindMobile]
        [Description("提交点评回复")]
        public ResponseResult CommentReply(ReplyAdd commentReply)
        {
            try
            {
                var user = User.Identity.GetUserInfo();
                string iSchoolAuth = HttpContext.Request.Cookies["iSchoolAuth"];
                if (string.IsNullOrWhiteSpace(commentReply.Content))
                {
                    return ResponseResult.Failed("内容不能为空");
                }

                var contentCheck = _text.GarbageCheck(new ProductManagement.API.Aliyun.Model.GarbageCheckRequest()
                {
                    scenes = new[] { "antispam" },
                    tasks = new List<ProductManagement.API.Aliyun.Model.GarbageCheckRequest.Task>() {
                      new ProductManagement.API.Aliyun.Model.GarbageCheckRequest.Task(){
                       content=commentReply.Content,
                         dataId = Guid.NewGuid().ToString()
                      }
                      }
                }).Result;

                if (contentCheck.data.FirstOrDefault().results.FirstOrDefault().suggestion == "block")
                {
                    return ResponseResult.Failed("您发布的内容包含敏感词，<br>请重新编辑后再发布。");
                }

                var Reply = new SchoolCommentReply
                {
                    SchoolCommentId = commentReply.SchoolCommentId,
                    ReplyId = commentReply.ReplyId,
                    Content = commentReply.Content,
                    RumorRefuting = commentReply.RumorRefuting,
                    PostUserRole = user.Role.Length > 0 ? (UserRole)user.Role[0] : UserRole.Member,
                    UserId = user.UserId,
                    IsAnony = commentReply.IsAnony,
                    IsAttend = commentReply.IsAttend,
                    IsSchoolPublish = commentReply.IsSchoolPublish
                };

                var comment = _commentService.QuerySchoolComment(commentReply.SchoolCommentId);
                Guid ReplyUserId = comment.CommentUserId;
                string title = comment.Content;

                if (commentReply.ReplyId != null)
                {
                    //查出最顶级父级
                    var TopParent = _commentReplyService.GetTopLevel((Guid)commentReply.ReplyId);
                    Reply.ParentId = TopParent.Id;

                    var reply = _commentReplyService.GetCommentReply((Guid)commentReply.ReplyId);
                    ReplyUserId = reply.UserId;
                    title = reply.Content;
                }

                int rez = _commentReplyService.Add(Reply, out Guid? replyId);

                //消息推送
                if (rez > 0)
                {

                    //所有邀请我回答这个点评的消息, 全部设为已读,已处理
                    _inviteStatus.UpdateMessageHandled(true, MessageType.InviteReplyComment, MessageDataType.Comment, user.UserId, default, new List<Guid>() { Reply.SchoolCommentId });

                    //检测是否为自己回复自己 
                    if (ReplyUserId != user.UserId)
                    {
                        //通知回复点评
                        bool states = _inviteStatus.AddMessage(new PMS.UserManage.Domain.Entities.Message()
                        {
                            title = title,
                            Content = commentReply.Content,
                            DataID = Reply.Id,
                            DataType = (byte)MessageDataType.CommentReply,
                            EID = comment.SchoolSectionId,
                            Type = (byte)MessageType.Reply,
                            userID = ReplyUserId,
                            senderID = user.UserId
                        });
                    }
                }

                int data = 0;
                if (commentReply.ReplyId != null)
                {
                    data = _commentReplyService.GetCommentReply((Guid)commentReply.ReplyId).ReplyCount;
                    //更改该回复的总数
                    _likeservice.UpdateCommentReplyLikeorReplayCount((Guid)commentReply.ReplyId);
                    if ((Guid)commentReply.ReplyId != (Guid)Reply.ParentId)
                    {
                        //更改该回复顶级的总数
                        _likeservice.UpdateCommentReplyLikeorReplayCount((Guid)Reply.ParentId);
                    }
                }
                else
                {
                    data = comment.ReplyCount;
                }
                return ResponseResult.Success(new { replyId, successRow = data });
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        /// <summary>
        /// Http Post Json
        /// </summary>
        /// <param name="commentReply"></param>
        /// <returns></returns>
        [Authorize]
        [BindMobile]
        [Description("提交点评回复")]
        public ResponseResult ReplyComment([FromBody] ReplyAdd commentReply)
        {
            return CommentReply(commentReply);
        }

        /// <summary>
        /// 最新点评，滑动分页
        /// </summary>
        /// <param name="SchoolId"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [Description("获取最新点评列表")]
        public ResponseResult GetSchoolComment(Guid SchoolId, int PageIndex, int PageSize, QueryCondition query = QueryCondition.All, CommentListOrder commentListOrder = CommentListOrder.None, Guid SearchSchool = default(Guid))
        {
            try
            {
                Guid userId = Guid.Empty;
                if (User.Identity.IsAuthenticated)
                {
                    userId = User.Identity.GetId();
                }

                //学校分部type标签还原
                query = SearchSchool != default(Guid) ? (QueryCondition)8 : query;

                if ((int)query == 8)
                {
                    SchoolId = SearchSchool;
                }

                var comment = _commentService.PageSchoolCommentBySchoolId(SchoolId, userId, PageIndex, PageSize, out int total, query, commentListOrder);
                if (comment.Any())
                {
                    var Exhibtion = _mapper.Map<List<SchoolCommentDto>, List<CommentList>>(comment);
                    var UserIds = new List<Guid>();
                    UserIds.AddRange(comment.GroupBy(q => q.UserId).Select(p => p.Key).ToList());
                    var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());

                    for (int i = 0; i < Exhibtion.Count(); i++)
                    {
                        for (int j = 0; j < Exhibtion[i].Images.Count; j++)
                        {
                            string tempImage = Exhibtion[i].Images[j];

                            Exhibtion[i].Images[j] = _setting.QueryImager + tempImage;
                        }

                        var user = Users.FirstOrDefault(q => q.Id == comment[i].UserId);
                        if (Exhibtion[i].IsAnony)
                        {
                            Exhibtion[i].UserInfo = new Models.User.UserInfoVo { NickName = "匿名用户", HeadImager = "https://cdn.sxkid.com//images/AppComment/defaultUserHeadImage.png" };
                        }
                        else
                        {
                            Exhibtion[i].UserInfo = _mapper.Map<UserInfoDto, Models.User.UserInfoVo>(user);
                        }
                        Exhibtion[i].No = Exhibtion[i].No.ToLower();
                    }

                    var schooinfos = _schoolInfoService.GetSchoolStatuse(Exhibtion.GroupBy(x => x.SchoolSectionId).Select(x => x.Key).ToList());
                    Exhibtion.ForEach(x =>
                    {
                        var schoolTemp = schooinfos.Where(s => s.SchoolSectionId == x.SchoolSectionId).FirstOrDefault();
                        x.School = _mapper.Map<SchoolInfoDto, CommentList.SchoolInfoVo>(schoolTemp);
                    });
                    return ResponseResult.Success(new { rows = Exhibtion });
                }
                else
                {
                    return ResponseResult.Success(new { rows = new List<CommentList>() });
                }
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        /// <summary>
        /// 获取最新回复
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [Description("获取最新点评回复列表")]
        public ResponseResult GetCurrentCommentNewReply(Guid Id, int PageIndex, int PageSize)
        {
            try
            {
                Guid userId = Guid.Empty;
                if (User.Identity.IsAuthenticated)
                {
                    userId = User.Identity.GetId();
                }
                var reply = _commentReplyService.CommentReply(Id, userId, PageIndex, PageSize, out int total);
                var newReply = _mapper.Map<List<ReplyDto>, List<ReplayExhibition>>(reply);

                var UserIds = new List<Guid>();
                UserIds.AddRange(reply.GroupBy(q => q.ReplyUserId).Select(p => p.Key).ToList());
                var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());

                if (newReply.Count() > 0)
                {
                    //回复最新回复
                    List<Guid> CommentReplyIds = newReply.Select(x => x.Id).ToList();
                    var replyExtNewest = _commentReplyService.CommentReplyExtNewest(CommentReplyIds);

                    //拿到用户信息
                    List<UserInfoVo> parentUsers = null;
                    if (replyExtNewest != null)
                    {
                        parentUsers = _mapper.Map<List<UserInfoDto>, List<UserInfoVo>>(_userService.ListUserInfo(replyExtNewest.Select(x => x.UserId).ToList()));
                    }

                    //newReply.ForEach(x=>x.isAnonymou == true?)
                    for (int i = 0; i < newReply.Count(); i++)
                    {
                        var newest = replyExtNewest.Find(x => x.ReplyId == newReply[i].Id);
                        if (newest != null)
                        {
                            newReply[i].NewestReply = new ReplyViewModel() { Id = newest.Id, UserInfo = parentUsers.Find(x => x.Id == newest.UserId).ToAnonyUserName(newest.IsAnony), Content = newest.Content, CreateTime = newest.CreateTime.ConciseTime() };
                        }
                        newReply[i].User = _mapper.Map<UserInfoVo>(Users.FirstOrDefault(q => q.Id == newReply[i].ReplyUserId)).ToAnonyUserName(newReply[i].IsAnonymou);
                    }
                }
                return ResponseResult.Success(new { rows = newReply });
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        /// <summary>
        /// 传一堆ID给你返回一个评论或者回复的list
        /// </summary>
        [HttpPost]
        [Authorize]
        public ResponseResult GetSchooCommentOrReply([FromBody] List<RequestOption> requestOptions)
        {
            try
            {
                if (User.Identity.IsAuthenticated && (requestOptions != null && requestOptions.Any()))
                {
                    var userInfo = User.Identity.GetUserInfo();
                    CommentAndCommentReply commentAndCommentReply = new CommentAndCommentReply();
                    var groups = requestOptions.GroupBy(x => x.dataIdType).ToList();

                    for (int i = 0; i < groups.Count(); i++)
                    {
                        if (groups[i].Key == dataIdType.SchooComment)
                        {
                            List<Guid> commentIds = groups[i].Select(x => x.dataId).ToList();
                            var commentDto = _commentService.QueryCommentByIds(commentIds, userInfo == null ? default(Guid) : userInfo.UserId);
                            var comment = _mapper.Map<List<SchoolCommentDto>, List<CommentList>>(commentDto);
                            var UserIds = new List<Guid>();
                            UserIds.AddRange(commentDto.GroupBy(q => q.UserId).Select(p => p.Key).ToList());
                            var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());
                            for (int k = 0; k < comment.Count(); k++)
                            {
                                for (int j = 0; j < comment[k].Images.Count; j++)
                                {
                                    string tempImage = comment[k].Images[j];

                                    comment[k].Images[j] = _setting.QueryImager + tempImage;
                                }
                                var user = Users.FirstOrDefault(q => q.Id == commentDto[k].UserId);
                                if (comment[k].IsAnony)
                                {
                                    comment[k].UserInfo = new Models.User.UserInfoVo { NickName = "匿名用户", HeadImager = "https://cdn.sxkid.com//images/AppComment/defaultUserHeadImage.png" };
                                }
                                else
                                {
                                    comment[k].UserInfo = _mapper.Map<UserInfoDto, Models.User.UserInfoVo>(user);
                                }

                            }

                            var temp = new List<CommentList>();
                            foreach (var item in commentIds)
                            {
                                var tempItem = comment.Where(x => x.Id == item).FirstOrDefault();
                                if (tempItem != null)
                                {
                                    temp.Add(tempItem);
                                }
                            }

                            commentAndCommentReply.CommentModels = temp;
                        }
                        else if (groups[i].Key == dataIdType.CommentReply)
                        {
                            commentAndCommentReply.CommentReplyModels = _mapper.Map<List<ReplyDto>, List<ReplayExhibition>>(_commentReplyService.GetCommentReplyByIds(groups[i].Select(x => x.dataId).ToList(), userInfo == null ? default(Guid) : userInfo.UserId));
                        }
                    }

                    if (commentAndCommentReply.CommentModels.Count() > 0)
                    {
                        var schools = _schoolInfoService.GetSchoolStatuse(commentAndCommentReply.CommentModels.GroupBy(x => x.SchoolSectionId).Select(x => x.Key).ToList());
                        commentAndCommentReply.CommentModels.ForEach(x =>
                        {
                            var school = schools.Where(s => s.SchoolSectionId == x.SchoolSectionId).FirstOrDefault();
                            x.School = _mapper.Map<SchoolInfoDto, CommentList.SchoolInfoVo>(school);
                        });
                    }

                    return ResponseResult.Success(commentAndCommentReply);
                }
                else
                {
                    return ResponseResult.Success(new CommentAndCommentReply()
                    {
                        CommentModels = new List<CommentList>(),
                        CommentReplyModels = new List<ReplayExhibition>()
                    });
                }
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        /// <summary>
        /// 当前用户发布的评论或者回复
        /// </summary>
        /// <param name="CommentId"></param>
        /// <param name="UserId"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [Authorize]
        public ResponseResult GetCommentAndCommentReply(int PageIndex = 1, int PageSize = 1)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userInfo = User.Identity.GetUserInfo();
                    CommentAndCommentReply commentAndCommentReply = new CommentAndCommentReply();
                    var commentDto = _commentService.QueryNewestCommentByIds(PageIndex, PageSize, userInfo.UserId);
                    var comment = _mapper.Map<List<SchoolCommentDto>, List<CommentList>>(commentDto);
                    var UserIds = new List<Guid>();
                    UserIds.AddRange(commentDto.GroupBy(q => q.UserId).Select(p => p.Key).ToList());
                    var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());
                    for (int i = 0; i < comment.Count(); i++)
                    {
                        for (int j = 0; j < comment[i].Images.Count; j++)
                        {
                            string tempImage = comment[i].Images[j];

                            comment[i].Images[j] = _setting.QueryImager + tempImage;
                        }

                        var user = Users.FirstOrDefault(q => q.Id == commentDto[i].UserId);
                        if (comment[i].IsAnony)
                        {
                            comment[i].UserInfo = new Models.User.UserInfoVo { NickName = "匿名用户", HeadImager = "https://cdn.sxkid.com//images/AppComment/defaultUserHeadImage.png" };
                        }
                        else
                        {
                            comment[i].UserInfo = _mapper.Map<UserInfoDto, Models.User.UserInfoVo>(user);
                        }
                    }

                    var school = _schoolInfoService.GetSchoolStatuse(comment.GroupBy(x => x.SchoolSectionId).Select(x => x.Key).ToList());
                    comment.ForEach(x =>
                    {
                        var schoolTemp = school.Where(s => s.SchoolSectionId == x.SchoolSectionId).FirstOrDefault();
                        x.School = _mapper.Map<SchoolInfoDto, CommentList.SchoolInfoVo>(schoolTemp);
                    });

                    commentAndCommentReply.CommentModels = comment;
                    commentAndCommentReply.CommentReplyModels = _mapper.Map<List<ReplyDto>, List<ReplayExhibition>>(_commentReplyService.GetNewestCommentReplyByUserId(PageIndex, PageSize, userInfo.UserId));
                    return ResponseResult.Success(commentAndCommentReply);
                }
                return ResponseResult.Success();
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        /// <summary>
        /// 其他用户回复当前用户的评论
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [Authorize]
        public ResponseResult GetCurretUserNewestReply(int PageIndex = 1, int PageSize = 1)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userInfo = User.Identity.GetUserInfo();
                    var commentReplyModels = _mapper.Map<List<ReplyDto>, List<ReplayExhibition>>(_commentReplyService.GetCurretUserNewestReply(PageIndex, PageSize, userInfo.UserId));
                    return ResponseResult.Success(commentReplyModels);
                }
                return ResponseResult.Success();
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        /// <summary>
        /// 获取用户点赞的 点评、点评回复、点评回复回复
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [Authorize]
        public ResponseResult GetLikeCommentAndReplies(int PageIndex = 1, int PageSize = 10)
        {
            try
            {
                List<LikeCommentAndReplyModel> commentReplyAndReplies = new List<LikeCommentAndReplyModel>();
                //if (User.Identity.IsAuthenticated)
                //{

                List<Guid> userIds = new List<Guid>();

                var userInfo = User.Identity.GetUserInfo();
                //userInfo.UserId = Guid.Parse("EC2BDC19-D148-4AFA-87ED-40749BB81F31");

                var commentReply = _commentReplyService.CurrentUserLikeCommentAndReply(userInfo.UserId, PageIndex, PageSize);

                List<Guid> commentIds = commentReply.GroupBy(x => x.SchoolCommentId).Select(x => x.Key)?.ToList();

                List<Guid> replyIds = commentReply.Where(x => x.Type == 1).GroupBy(x => x.ReplyId).Select(x => x.Key)?.ToList();

                List<SchoolCommentDto> SchoolCommentDto = new List<SchoolCommentDto>();
                if (commentIds.Count() > 0)
                {
                    SchoolCommentDto.AddRange(_commentService.QueryCommentByIds(commentIds, userInfo.UserId));
                    userIds.AddRange(SchoolCommentDto.GroupBy(x => x.UserId).Select(x => x.Key)?.ToList());
                }

                List<ReplyDto> replyDtos = new List<ReplyDto>();
                if (replyIds.Count() > 0)
                {
                    replyDtos.AddRange(_commentReplyService.GetCommentReplyByIds(replyIds, userInfo.UserId));
                    userIds.AddRange(replyDtos.GroupBy(x => x.ReplyUserId).Select(x => x.Key)?.ToList());
                }

                userIds.AddRange(commentReply.GroupBy(x => x.UserId).Select(x => x.Key)?.ToList());

                //用户
                var Users = _userService.ListUserInfo(userIds.GroupBy(q => q).Select(p => p.Key).ToList());
                //List<SchoolDto> school = new List<SchoolDto>();

                var school = _schoolInfoService.GetSchoolStatuse(SchoolCommentDto.GroupBy(x => x.SchoolSectionId).Select(x => x.Key).ToList());


                foreach (var item in commentReply)
                {
                    LikeCommentAndReplyModel commentReplyAndReply = new LikeCommentAndReplyModel();
                    var comment = SchoolCommentDto.Where(x => x.Id == item.SchoolCommentId).FirstOrDefault();
                    var schoolinfo = school.Where(x => x.SchoolSectionId == comment.SchoolSectionId).FirstOrDefault();
                    var replyUser = Users.Where(x => x.Id == item.UserId).FirstOrDefault();
                    if (item.Type == 0)
                    {
                        //点评回复
                        var commentUser = Users.Where(x => x.Id == comment.UserId).FirstOrDefault();
                        commentReplyAndReply.userInfoVo = UserHelper.ToAnonyUserName(new UserInfoVo()
                        {
                            HeadImager = commentUser.HeadImgUrl,
                            Id = commentUser.Id,
                            NickName = commentUser.NickName,
                            Role = commentUser.VerifyTypes.ToList()
                        }, comment.IsAnony);


                        commentReplyAndReply.replyUserInfoVo = UserHelper.ToAnonyUserName(new UserInfoVo()
                        {
                            Id = replyUser.Id,
                            HeadImager = replyUser.HeadImgUrl,
                            NickName = replyUser.NickName,
                            Role = replyUser.VerifyTypes.ToList()
                        }, item.IsAnony);

                        commentReplyAndReply.Content = comment.Content;
                        commentReplyAndReply.ReplyContent = item.Content;
                        commentReplyAndReply.AddTime = comment.CreateTime.ConciseTime();
                        commentReplyAndReply.ReplyAddTime = item.CreateTime.ConciseTime();
                    }
                    else if (item.Type == 1)
                    {
                        //点评回复的回复
                        var reply = replyDtos.Where(x => x.Id == item.ReplyId).FirstOrDefault();
                        var replyrUser = Users.Where(x => x.Id == reply.ReplyUserId).FirstOrDefault();

                        commentReplyAndReply.userInfoVo = UserHelper.ToAnonyUserName(new UserInfoVo()
                        {
                            Id = replyUser.Id,
                            HeadImager = replyUser.HeadImgUrl,
                            NickName = replyUser.NickName,
                            Role = replyUser.VerifyTypes.ToList()
                        }, item.IsAnony);
                        commentReplyAndReply.replyUserInfoVo = UserHelper.ToAnonyUserName(new UserInfoVo()
                        {
                            Id = replyrUser.Id,
                            HeadImager = replyrUser.HeadImgUrl,
                            NickName = replyrUser.NickName,
                            Role = replyrUser.VerifyTypes.ToList()
                        }, item.IsAnony);


                        commentReplyAndReply.Content = item.Content;
                        commentReplyAndReply.ReplyContent = reply.Content;
                        commentReplyAndReply.AddTime = item.CreateTime.ConciseTime();
                        commentReplyAndReply.ReplyAddTime = reply.AddTime;
                    }
                    else
                    {
                        //点评
                        commentReplyAndReply.userInfoVo = UserHelper.ToAnonyUserName(new UserInfoVo()
                        {
                            Id = replyUser.Id,
                            HeadImager = replyUser.HeadImgUrl,
                            NickName = replyUser.NickName,
                            Role = replyUser.VerifyTypes.ToList()
                        }, item.IsAnony);
                        commentReplyAndReply.Content = item.Content;

                        commentReplyAndReply.AddTime = item.CreateTime.ConciseTime();
                    }
                    commentReplyAndReply.Id = item.Id;
                    commentReplyAndReply.CommentId = item.SchoolCommentId;
                    commentReplyAndReply.ReplyId = item.ReplyId;
                    commentReplyAndReply.SchoolSectionId = schoolinfo.SchoolSectionId;
                    commentReplyAndReply.SchoolId = schoolinfo.SchoolId;
                    commentReplyAndReply.SchoolName = schoolinfo.SchoolName;
                    commentReplyAndReply.Type = item.Type == 2 ? 0 : item.Type == 1 ? 1 : 1;
                    commentReplyAndReply.LikeCount = item.LikeCount;
                    commentReplyAndReplies.Add(commentReplyAndReply);
                }
                return ResponseResult.Success(commentReplyAndReplies);
                //}
                //else
                //{
                //    return ResponseResult.Success(commentReplyAndReplies);
                //}
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        /// <summary>
        /// 当前用户发表的点评
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [Authorize]
        public ResponseResult CurrentPublishComment(int PageIndex = 1, int PageSize = 10)
        {
            try
            {
                var userInfo = User.Identity.GetUserInfo();
                var commentDto = _commentService.QueryNewestCommentByIds(PageIndex, PageSize, userInfo.UserId);
                var comment = _mapper.Map<List<SchoolCommentDto>, List<CommentList>>(commentDto);
                var UserIds = new List<Guid>();
                UserIds.AddRange(commentDto.GroupBy(q => q.UserId).Select(p => p.Key).ToList());
                var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());
                for (int i = 0; i < comment.Count(); i++)
                {
                    for (int j = 0; j < comment[i].Images.Count; j++)
                    {
                        string tempImage = comment[i].Images[j];

                        comment[i].Images[j] = _setting.QueryImager + tempImage;
                    }

                    var user = Users.FirstOrDefault(q => q.Id == commentDto[i].UserId);
                    if (comment[i].IsAnony)
                    {
                        comment[i].UserInfo = new Models.User.UserInfoVo { NickName = "匿名用户", HeadImager = "https://cdn.sxkid.com//images/AppComment/defaultUserHeadImage.png" };
                    }
                    else
                    {
                        comment[i].UserInfo = _mapper.Map<UserInfoDto, Models.User.UserInfoVo>(user);
                    }
                }

                var schoolInfo = _schoolInfoService.GetSchoolStatuse(comment.GroupBy(x => x.SchoolSectionId).Select(x => x.Key)?.ToList());
                //comment[i].School = _mapper.Map<SchoolInfoDto, CommentList.SchoolInfo>(schoolInfo);

                comment.ForEach(x =>
                {
                    var school = schoolInfo.Where(c => c.SchoolSectionId == x.SchoolSectionId).FirstOrDefault();
                    x.School = _mapper.Map<SchoolInfoDto, CommentList.SchoolInfoVo>(school);
                });

                return ResponseResult.Success(comment);
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        /// <summary>
        /// 回复-点评：当前用户发布的“点评的回复”+当前用户发布的“点评的回复”的回复
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public ResponseResult CurrentPublishCommentReplyAndReply(int PageIndex = 1, int PageSize = 10)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    List<CommentReplyAndReplyModel> commentReplyAndReplies = new List<CommentReplyAndReplyModel>();
                    List<Guid> userIds = new List<Guid>();

                    var userInfo = User.Identity.GetUserInfo();
                    var commentReply = _commentReplyService.CurrentSchoolCommenReplyAndReply(userInfo.UserId, PageIndex, PageSize);

                    List<Guid> commentIds = commentReply.GroupBy(x => x.SchoolCommentId).Select(x => x.Key)?.ToList();

                    List<Guid> replyIds = commentReply.Where(x => x.Type == 1).GroupBy(x => x.ReplyId).Select(x => x.Key)?.ToList();

                    List<SchoolCommentDto> SchoolCommentDto = new List<SchoolCommentDto>();
                    if (commentIds.Count() > 0)
                    {
                        SchoolCommentDto.AddRange(_commentService.QueryCommentByIds(commentIds, userInfo.UserId));
                        userIds.AddRange(SchoolCommentDto.GroupBy(x => x.UserId).Select(x => x.Key)?.ToList());
                    }

                    List<ReplyDto> replyDtos = new List<ReplyDto>();
                    if (replyIds.Count() > 0)
                    {
                        replyDtos.AddRange(_commentReplyService.GetCommentReplyByIds(replyIds, userInfo.UserId));
                        userIds.AddRange(replyDtos.GroupBy(x => x.ReplyUserId).Select(x => x.Key)?.ToList());
                    }

                    userIds.AddRange(commentReply.GroupBy(x => x.UserId).Select(x => x.Key)?.ToList());

                    //用户
                    var Users = _userService.ListUserInfo(userIds.GroupBy(q => q).Select(p => p.Key).ToList());
                    //学校
                    var school = _schoolInfoService.GetSchoolStatuse(SchoolCommentDto.Select(x => x.SchoolSectionId)?.ToList());


                    foreach (var item in commentReply)
                    {
                        CommentReplyAndReplyModel commentReplyAndReply = new CommentReplyAndReplyModel();
                        var comment = SchoolCommentDto.Where(x => x.Id == item.SchoolCommentId).FirstOrDefault();
                        var schoolinfo = school.Where(x => x.SchoolSectionId == comment.SchoolSectionId).FirstOrDefault();
                        var replyUser = Users.Where(x => x.Id == item.UserId).FirstOrDefault();
                        if (item.Type == 0)
                        {

                            var commentUser = Users.Where(x => x.Id == comment.UserId).FirstOrDefault();
                            commentReplyAndReply.userInfoVo = UserHelper.ToAnonyUserName(new UserInfoVo()
                            {
                                HeadImager = commentUser.HeadImgUrl,
                                Id = commentUser.Id,
                                NickName = commentUser.NickName,
                                Role = commentUser.VerifyTypes.ToList()
                            }, comment.IsAnony);


                            commentReplyAndReply.replyUserInfoVo = UserHelper.ToAnonyUserName(new UserInfoVo()
                            {
                                Id = replyUser.Id,
                                HeadImager = replyUser.HeadImgUrl,
                                NickName = replyUser.NickName,
                                Role = replyUser.VerifyTypes.ToList()
                            }, item.IsAnony);

                            commentReplyAndReply.Content = comment.Content;
                            commentReplyAndReply.ReplyContent = item.Content;
                            commentReplyAndReply.AddTime = comment.CreateTime.ConciseTime();
                            commentReplyAndReply.ReplyAddTime = item.CreateTime.ConciseTime();
                            commentReplyAndReply.Id = item.Id;
                            commentReplyAndReply.ReplyId = item.ReplyId;
                        }
                        else
                        {
                            var reply = replyDtos.Where(x => x.Id == item.ReplyId).FirstOrDefault();
                            var replyrUser = Users.Where(x => x.Id == reply.ReplyUserId).FirstOrDefault();

                            commentReplyAndReply.userInfoVo = UserHelper.ToAnonyUserName(new UserInfoVo()
                            {
                                Id = replyUser.Id,
                                HeadImager = replyUser.HeadImgUrl,
                                NickName = replyUser.NickName,
                                Role = replyUser.VerifyTypes.ToList()
                            }, item.IsAnony);
                            commentReplyAndReply.replyUserInfoVo = UserHelper.ToAnonyUserName(new UserInfoVo()
                            {
                                Id = replyrUser.Id,
                                HeadImager = replyrUser.HeadImgUrl,
                                NickName = replyrUser.NickName,
                                Role = replyrUser.VerifyTypes.ToList()
                            }, item.IsAnony);

                            commentReplyAndReply.Id = item.Id;
                            commentReplyAndReply.ReplyId = item.ReplyId;
                            commentReplyAndReply.Content = item.Content;
                            commentReplyAndReply.ReplyContent = reply.Content;
                            commentReplyAndReply.AddTime = item.CreateTime.ConciseTime();
                            commentReplyAndReply.ReplyAddTime = reply.AddTime;
                        }

                        commentReplyAndReply.SchoolCommentId = item.SchoolCommentId;
                        commentReplyAndReply.SchoolSectionId = schoolinfo.SchoolSectionId;
                        commentReplyAndReply.SchoolId = schoolinfo.SchoolId;
                        commentReplyAndReply.SchoolName = schoolinfo.SchoolName;
                        commentReplyAndReply.Type = item.Type;
                        commentReplyAndReplies.Add(commentReplyAndReply);
                    }

                    return ResponseResult.Success(commentReplyAndReplies);
                }
                else
                {
                    return ResponseResult.Success();
                }
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }



    }
}