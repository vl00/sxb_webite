using AutoMapper;
using CommentApp.Models.School;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PMS.CommentsManage.Application.Common;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.IServices.IMQService;
using PMS.CommentsManage.Application.ModelDto;
using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.Entities.Total;
using PMS.Infrastructure.Application.IService;
using PMS.Infrastructure.Application.ModelDto;
using PMS.School.Application.IServices;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Application.ModelDto;
using PMS.UserManage.Domain.Common;
using ProductManagement.API.Http.Interface;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using ProductManagement.Infrastructure.Exceptions;
using Sxb.Web.Common;
using Sxb.Web.Models;
using Sxb.Web.Models.Comment;
using Sxb.Web.Models.Replay;
using Sxb.Web.Models.User;
using Sxb.Web.Response;
using Sxb.Web.Utils;
using Sxb.Web.ViewModels.Comment;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace Sxb.Web.Controllers
{
    public class CommentController : BaseController
    {

        private readonly ICollectionService _collection;
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
        private readonly IUserGrantAuthService _userGrantAuthService;

        private readonly IUserServiceClient _userServiceClient;


        private readonly IEasyRedisClient _easyRedisClient;
        private readonly ICityInfoService _cityService;

        //事件总线
        private readonly ISchoolCommentLikeMQService _commentLikeMQService;

        private readonly ILogger<SchoolCommentController> _logger;

        private readonly IMapper _mapper;

        readonly ISchService _SchService;

        public CommentController(IOptions<ImageSetting> set,
            ICollectionService collection,
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
            IUserGrantAuthService userGrantAuthService,
            IMapper mapper, ILoggerFactory loggerFactory, ISchService schService)
        {
            _SchService = schService;
            _collection = collection;
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

            _userGrantAuthService = userGrantAuthService;

            _userServiceClient = userServiceClient;
            _easyRedisClient = easyRedisClient;
            _mapper = mapper;

            _userMessage = userMessage;

            _logger = loggerFactory.CreateLogger<SchoolCommentController>();
        }

        [Description("学校点评列表页")]
        [Route("/{controller}/{action}-{schoolNo}")]
        [Route("/{controller}/{action}/{Id}")]
        public async Task<IActionResult> School(Guid Id, int query = 1, CommentListOrder commentListOrder = CommentListOrder.None, string schoolNo = default)
        {
            if (!string.IsNullOrWhiteSpace(schoolNo))
            {
                var ext = await _SchService.GetSchextSimpleInfoViaShortNo(schoolNo);
                if (ext != null)
                {
                    Id = ext.Eid;
                }
                else
                {
                    return new ExtNotFoundViewResult();
                }
            }


            var school = _schoolInfoService.QueryESchoolInfo(Id);

            if (school == null) return new ExtNotFoundViewResult();

            _ = _userServiceClient.AddHistory(new ProductManagement.API.Http.Model.AddHistory() { dataID = Id, dataType = MessageDataType.School, cookies = HttpContext.Request.GetAllCookies() });
            //检测当前用户身份 （true：校方用户 |false：普通用户）
            ViewBag.isSchoolRole = false;
            //检测是否关注该学校
            ViewBag.isCollected = false;
            Guid UserId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                UserId = user.UserId;
                //检测当前用户身份 （true：校方用户 |false：普通用户）
                ViewBag.isSchoolRole = user.Role.Contains((int)PMS.UserManage.Domain.Common.UserRole.School);
                //检测改用户是否关注该学校
                ViewBag.isCollected = _collection.IsCollected(UserId, Id, CollectionDataType.School);
            }

            List<AreaDto> history = new List<AreaDto>();
            if (UserId != default(Guid))
            {
                string Key = $"SearchCity:{UserId}";
                var historyCity = await _easyRedisClient.SortedSetRangeByRankAsync<AreaDto>(Key, 0, 10, StackExchange.Redis.Order.Descending);
                history.AddRange(historyCity.ToList());
            }


            ViewBag.HotCity = JsonConvert.SerializeObject(await _cityService.GetHotCity());

            ViewBag.History = JsonConvert.SerializeObject(history);

            var adCodes = await _cityService.IntiAdCodes();
            //城市编码
            ViewBag.AllCity = JsonConvert.SerializeObject(adCodes);

            var extension = _mapper.Map<SchoolInfoDto, SchoolExtensionVo>(school);

            //var schoolComment = _commentScoreService.GetSchoolScoreById(SchoolId);

            extension.Score = school.SchoolAvgScore;
            extension.StartTotal = school.SchoolStars;

            //学校数据统计
            var schoolTotal = _mapper.Map<List<SchoolCommentTotal>, List<SchoolCommentTotalVo>>(_commentService.CurrentCommentTotalBySchoolId(school.SchoolId));
            var AllExtension = _schoolService.GetSchoolExtName(school.SchoolId);
            for (int i = 0; i < AllExtension.Count(); i++)
            {
                var item = schoolTotal.Where(x => x.SchoolSectionId == AllExtension[i].Value).FirstOrDefault();
                int currentIndex = schoolTotal.IndexOf(item);
                if (item == null)
                {
                    if (AllExtension[i].Key != null)
                    {
                        schoolTotal.Add(new SchoolCommentTotalVo() { Name = AllExtension[i].Key, SchoolSectionId = AllExtension[i].Value, TotalType = QueryCondition.Other.Description() });
                    }
                }
                else
                {
                    item.Name = AllExtension[i].Key;
                    schoolTotal[currentIndex] = item;
                }
            }

            //将其他分部类型学校进行再次排序
            for (int i = 8; i < schoolTotal.Count(); i++)
            {
                schoolTotal[i].TotalTypeNumber = i + 1;
            }

            //排除所有下架学校标签筛选
            schoolTotal.Where(x => x.Name == null && x.SchoolSectionId != Guid.Empty)?.ToList().ForEach(x =>
            {
                schoolTotal.Remove(x);
            });

            ViewBag.CommentTotal = schoolTotal;

            //只有筛选条件为全部时，才加载最精选点评数据
            if ((int)query == 1)
            {
                List<CommentExhibition> commentDetail = new List<CommentExhibition>();
                //获取前三篇精选点评
                var comment = _commentService.SelectedThreeComment(extension.SchoolId, UserId, (QueryCondition)query, commentListOrder);

                var Exhibtion = _mapper.Map<List<PMS.CommentsManage.Application.ModelDto.SchoolCommentDto>, List<CommentList>>(comment);

                var UserIds = new List<Guid>();
                UserIds.AddRange(comment.GroupBy(q => q.UserId).Select(p => p.Key).ToList());
                var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());

                var extSchoolIds = Exhibtion.GroupBy(q => q.SchoolSectionId).Select(q => q.Key).ToList();

                if (extSchoolIds.Count() > 0)
                {
                    var schoolInfos = _schoolInfoService.GetSchoolSectionByIds(extSchoolIds);
                    for (int i = 0; i < Exhibtion.Count(); i++)
                    {
                        for (int j = 0; j < Exhibtion[i].Images.Count; j++)
                        {
                            string tempImage = Exhibtion[i].Images[j];

                            Exhibtion[i].Images[j] = _setting.QueryImager + tempImage;
                        }
                        var schoolInfo = schoolInfos.FirstOrDefault(q => q.SchoolSectionId == Exhibtion[i].SchoolSectionId);
                        Exhibtion[i].School = _mapper.Map<SchoolInfoDto, CommentList.SchoolInfoVo>(schoolInfo);

                        var user = Users.FirstOrDefault(q => q.Id == comment[i].UserId);
                        if (Exhibtion[i].IsAnony)
                        {
                            Exhibtion[i].UserInfo = new UserInfoVo { NickName = "匿名用户", HeadImager = "https://cdn.sxkid.com//images/AppComment/defaultUserHeadImage.png" };
                        }
                        else
                        {
                            Exhibtion[i].UserInfo = _mapper.Map<UserInfoDto, UserInfoVo>(user);
                        }
                        Exhibtion[i].No = Exhibtion[i].No.ToLower();
                    }
                }
                ViewBag.CommentExhibition = Exhibtion;
            }
            ViewBag.School = extension;


            //保存当前选项卡选中值
            ViewBag.SelectedTab = query;

            ViewBag.SchoolId = school;

            return View();
        }

        [Description("点评详情页")]
        [Route("comment/{No}.html")]
        [Route("comment/detail/{CommentId}")]
        public IActionResult Detail(Guid CommentId, string No)
        {
            //检测当前用户身份 （true：校方用户 |false：普通用户）
            ViewBag.isSchoolRole = false;
            //检测是否关注该点评
            ViewBag.isCollected = false;

            long numberId = 0;
            if (No != null)
            {
                numberId = UrlShortIdUtil.Base322Long(No);
            }

            SchoolComment Comment = new SchoolComment();

            if (numberId > 0)
            {
                Comment = _commentService.QuerySchoolCommentNo(numberId);
            }
            else
            {
                Comment = _commentService.QuerySchoolComment(CommentId);
            }

            if (Comment == null)
            {
                throw new NotFoundException();
            }
            var schoolDto = _schoolInfoService.QueryESchoolInfo(Comment.SchoolSectionId);
            if (schoolDto == null)
            {
                throw new NotFoundException();
            }

            CommentId = Comment.Id;
            Guid userId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                userId = user.UserId;
                //检测当前用户身份 （true：校方用户 |false：普通用户）
                ViewBag.isSchoolRole = user.Role.Contains((int)PMS.UserManage.Domain.Common.UserRole.School);

                ViewBag.isCollected = _collection.IsCollected(userId, CommentId, CollectionDataType.Comment);
            }

            _ = _userServiceClient.AddHistory(new ProductManagement.API.Http.Model.AddHistory() { dataID = Comment.Id, dataType = MessageDataType.School, cookies = HttpContext.Request.GetAllCookies() });

            _commentService.UpdateCommentViewCount(CommentId);

            var commentReplyselected = _commentReplyService.SelectedCommentReply(CommentId);

            var UserIds = new List<Guid>()
            {
                Comment.CommentUserId
            };
            UserIds.AddRange(commentReplyselected.GroupBy(q => q.UserId).Select(p => p.Key).ToList());
            UserIds.Add(userId);

            var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());


            List<Guid> replyIds = commentReplyselected.GroupBy(q => q.Id).Select(s => s.Key).ToList();
            replyIds.Add(CommentId);
            var isLikes = _likeservice.CheckCurrentUserIsLikeBulk(userId, replyIds);



            var schoolComment = _mapper.Map<SchoolComment, SchoolCommentViewModel>(Comment);

            if (schoolComment.IsAnony)
            {
                schoolComment.UserInfo = new UserInfoVo
                {
                    NickName = "匿名用户",
                    HeadImager = "https://cdn.sxkid.com//images/AppComment/defaultUserHeadImage.png"
                };
            }
            else
            {
                schoolComment.UserInfo = _mapper.Map<UserInfoVo>(Users.FirstOrDefault(p => p.Id == Comment.CommentUserId));
            }


            var school = _mapper.Map<SchoolInfoDto, SchoolExtensionVo>(schoolDto);

            var schoolImages = _schoolImageService.GetImageByDataSourceId(Comment.Id).ToList();
            schoolImages.ForEach(x => x.ImageUrl = _setting.QueryImager + x.ImageUrl);


            var tags = _schoolTagService.GetSchoolTagByCommentId(Comment.Id);
            var isLike = isLikes.FirstOrDefault(p => p.SourceId == Comment.Id) != null
                                && isLikes.FirstOrDefault(p => p.SourceId == Comment.Id).IsLike;

            if (commentReplyselected.Count > 0)
            {
                //点评回复精选
                var SchoolCommentReplyVoselected = _mapper.Map<List<SchoolCommentReply>, List<SchoolCommentReplyVo>>(commentReplyselected);

                foreach (var q in SchoolCommentReplyVoselected)
                {
                    q.IsSelected = true;
                    q.IsLike = isLikes.FirstOrDefault(p => p.SourceId == q.Id) != null && isLikes.FirstOrDefault(p => p.SourceId == q.Id).IsLike;
                    q.UserInfo = _mapper.Map<UserInfoVo>(Users.FirstOrDefault(p => p.Id == q.UserId)).ToAnonyUserName(q.IsAnony);
                }
                var commentReplies = SchoolCommentReplyVoselected;
                ViewBag.CommentReplies = commentReplies;
            }

            //ViewBag.commentDetail = commentDetail;

            schoolComment.SchoolCommentScore = schoolComment.SchoolCommentScore ?? new SchoolCommentScoreViewModel();
            ViewBag.SchoolComment = schoolComment;
            ViewBag.School = school;
            ViewBag.SchoolImages = schoolImages;
            ViewBag.Tags = tags;
            ViewBag.IsLike = isLike;

            ViewBag.CurrentUser = Users.Where(x => x.Id == userId).FirstOrDefault();

            //学校平均分
            ViewBag.SchoolScore = Math.Round(schoolDto.SchoolAvgScore / 20, 1);
            ViewBag.StartTotal = schoolDto.SchoolStars;
            return View();
        }



        /// <summary>
        /// 点评卡片信息
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        public async Task<ResponseResult> GetCommentCardInfo(Guid commentId)
        {
            try
            {
                //获取当前点评实体
                var comment = await _easyRedisClient.GetOrAddAsync($"Comment:Detail_{commentId}", () =>
                {
                    return _commentService.QueryComment(commentId);
                }, new TimeSpan(0, 5, 0));

                //获取当前点评所在的分部信息
                var school = await _schoolInfoService.QuerySchoolInfo(comment.SchoolSectionId);

                var info = new
                {
                    ExtId = school.SchoolSectionId,
                    SchoolName = school.SchoolName,
                    Stars = SchoolScoreToStart.GetCurrentSchoolstart(school.SchoolAvgScore),
                    SchoolAvgScore = Math.Round(school.SchoolAvgScore / 20, 1),
                    ShortSchoolNo = school.ShortSchoolNo,
                    Content = comment.Content,
                    ShortCommentNo = comment.ShortCommentNo
                };
                return ResponseResult.Success(info);
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }



        /// <summary>
        /// 写点评页
        /// </summary>
        /// <param name="currentId">操作的分部id</param>
        /// <param name="SchoolId">学校id</param>
        /// <returns></returns>
        [Description("写点评页")]
        [Authorize]
        public async Task<IActionResult> Write(Guid eid)
        {
            //检测当前用户身份 （true：校方用户 |false：普通用户）
            ViewBag.isSchoolRole = false;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                //检测当前用户身份 （true：校方用户 |false：普通用户）
                ViewBag.isSchoolRole = user.Role.Contains((int)PMS.UserManage.Domain.Common.UserRole.School);
            }

            Guid userId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                userId = user.UserId;
            }

            var allSchoolExt = _schoolInfoService.GetSchoolName(new List<Guid>() { eid });

            var currentSchool = allSchoolExt.Where(x => x.SchoolSectionId == eid).FirstOrDefault();
            if (currentSchool == null)
            {
                throw new NotFoundException();
            }

            ViewBag.CurrentSchool = currentSchool;

            ViewBag.Tag = _schoolTagService.GetSchoolTagBySchoolId(eid).Select(x => x.Tag).ToList();
            ViewBag.AllSchoolBranch = allSchoolExt.Where(x => x.SchoolSectionId != eid);

            List<AreaDto> history = new List<AreaDto>();
            if (userId != default(Guid))
            {
                string Key = $"SearchCity:{userId}";
                var historyCity = await _easyRedisClient.SortedSetRangeByRankAsync<AreaDto>(Key, 0, 10, StackExchange.Redis.Order.Descending);
                history.AddRange(historyCity.ToList());
            }


            ViewBag.HotCity = JsonConvert.SerializeObject(await _cityService.GetHotCity());

            ViewBag.History = JsonConvert.SerializeObject(history);

            var adCodes = await _cityService.IntiAdCodes();

            //获取当前用户所在城市
            var localCityCode = Request.GetLocalCity();

            //将数据保存在前台
            ViewBag.LocalCity = adCodes.SelectMany(p => p.CityCodes).FirstOrDefault(p => p.Id == localCityCode);

            //城市编码
            ViewBag.AllCity = JsonConvert.SerializeObject(adCodes);

            //保存学校id
            ViewBag.SchoolId = currentSchool.SchoolId;

            //检测是否成功授权过
            //ViewBag.IsWriteAuth = _commentService.UserAgreement(userId);

            ViewBag.IsWriteAuth = _userGrantAuthService.IsGrantAuth(userId);

            return View();
        }

        /// <summary>
        /// 点评回复页
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Description("点评回复页")]
        public IActionResult Reply(Guid Id)
        {
            Guid userId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                userId = user.UserId;
            }


            ViewBag.DatascoureId = Id;
            ViewBag.Type = 1;
            var isLike = _likeservice.CheckCurrentUserIsLike(userId, Id);


            //检测当前用户身份 （true：校方用户 |false：普通用户）
            ViewBag.isSchoolRole = false;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                //检测当前用户身份 （true：校方用户 |false：普通用户）
                ViewBag.isSchoolRole = user.Role.Contains((int)UserRole.School);
            }
            ViewBag.Type = 1;

            var commentReplyId = Id;
            var comment = _commentReplyService.GetCommentReply(commentReplyId);

            if (comment == null)
            {
                throw new NotFoundException();
            }
            _commentReplyService.UpdateReplyViewCount(commentReplyId);

            var hottestCommentReply = _commentReplyService.GetSchoolCommentHottestReplies(commentReplyId);

            List<Guid> userIds = hottestCommentReply.Select(x => x.UserId)?.ToList();
            userIds.Add(comment.UserId);

            var users = _userService.ListUserInfo(userIds);

            var replyUser = users.FirstOrDefault(q => q.Id == comment.UserId);

            var reply = new ReplyDetailViewModel
            {
                Id = comment.Id,
                DatascoureId = comment.SchoolCommentId,
                SchoolSectionId = comment.SchoolComment.SchoolSectionId,
                Content = comment.Content,
                IsLike = isLike,
                IsAttend = comment.IsAttend,
                IsSchoolPublish = false,
                RumorRefuting = false,
                ReplyCount = comment.ReplyCount,
                LikeCount = comment.LikeCount,
                CreateTime = comment.CreateTime.ConciseTime(),
                UserInfo = UserHelper.ToAnonyUserName(new UserInfoVo
                {
                    Id = comment.UserId,
                    NickName = replyUser.NickName,
                    HeadImager = replyUser.HeadImgUrl,
                    Role = replyUser.VerifyTypes.ToList()
                }, comment.IsAnony)
            };


            List<Guid> replyIds = hottestCommentReply.GroupBy(q => q.Id).Select(s => s.Key).ToList();
            //var isLikes = _likeservice.CheckCurrentUserIsLikeBulk(userId, replyIds);
            //var hottestReplyViewModel = hottestCommentReply.Select(q => new ReplyViewModel
            //{
            //    Id = q.Id,
            //    Content = q.Content,
            //    DatascoureId = q.SchoolCommentId,
            //    ReplyId = q.ReplyId,
            //    IsLike = isLikes.FirstOrDefault(p => p.SourceId == q.Id) != null && isLikes.FirstOrDefault(p => p.SourceId == q.Id).IsLike,
            //    IsTop = q.IsTop,
            //    IsAttend = q.IsAttend,
            //    IsSchoolPublish = q.IsSchoolPublish,
            //    RumorRefuting = q.RumorRefuting,
            //    ReplyCount = q.ReplyCount,
            //    LikeCount = q.LikeCount,
            //    UserInfo = _mapper.Map<UserInfoVo>(users.FirstOrDefault(p => p.Id == q.UserId)).ToAnonyUserName(q.IsAnony),
            //    CreateTime = q.CreateTime.ConciseTime()
            //}).ToList();

            //ViewBag.replyHottest = hottestReplyViewModel;
            ViewBag.replyDetail = reply;

            return View();
        }

    }
}