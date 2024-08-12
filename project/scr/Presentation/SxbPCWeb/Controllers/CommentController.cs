using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using PMS.CommentsManage.Domain.Entities;
using PMS.School.Application.IServices;
using PMS.School.Domain.Entities;
using PMS.UserManage.Application.IServices;
using ProductManagement.Infrastructure.Toolibrary;
using Sxb.PCWeb.Models;
using PMS.CommentsManage.Domain.Common;
using ProductManagement.Framework.Foundation;
using PMS.School.Application.ModelDto;
using PMS.CommentsManage.Application.ModelDto.Reply;
using PMS.CommentsManage.Application.Common;
using Sxb.PCWeb.RequestModel;
using Sxb.PCWeb.Response;
using Sxb.PCWeb.Models.User;
using Sxb.PCWeb.Utils;
using Microsoft.AspNetCore.Http;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Sxb.PCWeb.Authentication.Attribute;
using PMS.UserManage.Domain.Common;
using PMS.Infrastructure.Application.ModelDto;
using ProductManagement.Framework.Cache.Redis;
using Newtonsoft.Json;
using PMS.Infrastructure.Application.IService;
using PMS.CommentsManage.Application.IServices.IMQService;
using System.Diagnostics;
using PMS.CommentsManage.Domain.Entities.Total;
using ProductManagement.API.Http.Interface;
using Sxb.PCWeb.Middleware;
using PMS.UserManage.Application.ModelDto;
using Sxb.PCWeb.Utils.CommentHelper;
using Sxb.PCWeb.ViewModels.Comment;
using Sxb.PCWeb.Common;
using Sxb.Web.RequestModel;
using System.ComponentModel;
using PMS.Search.Application.IServices;
using PMS.Search.Application.ModelDto.Query;
using Sxb.PCWeb.ViewModels.School;
using Sxb.PCWeb.RequestModel.Comment;
using ProductManagement.Infrastructure.Exceptions;
using ProductManagement.API.Aliyun;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace Sxb.PCWeb.Controllers
{
    public class CommentController : BaseController
    {
        private readonly IMessageService _messageService;

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
        private readonly IQuestionInfoService _questionInfo;
        private readonly IHistoryService _historyService;
        private readonly ITagService _tagService;

        private readonly ISearchService _dataSearch;
        private readonly IImportService _dataImport;

        private readonly IEasyRedisClient _easyRedisClient;
        private readonly ICityInfoService _cityService;

        private readonly IPartTimeJobAdminService _adminService;


        private readonly ISchoolImageService _imageService;

        private readonly ISettlementAmountMoneyService _amountMoneyService;

        //事件总线
        private readonly ISchoolCommentLikeMQService _commentLikeMQService;

        private readonly IPartTimeJobAdminRolereService _adminRolereService;

        private readonly ICollectionService _collectionService;

        private readonly IText _text;
        readonly ISchService _SchService;

        //热门点评、学校组合方法
        PullHottestCSHelper hot;

        private readonly IMapper _mapper;
        public CommentController(IOptions<ImageSetting> set,
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
            IUserServiceClient userMessage,
            ISearchService dataSearch, IImportService dataImport,
            IPartTimeJobAdminService adminService,
             IPartTimeJobAdminRolereService adminRolereService,
              ISettlementAmountMoneyService amountMoneyService,
              IQuestionInfoService questionInfo,
              ITagService tagService,
              ISchoolImageService imageService,
              ICollectionService collectionService,
              IMessageService inviteStatus, IHistoryService historyService,
            IMapper mapper, IText text, ISchService schService)
        {
            _collectionService = collectionService;
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
            _questionInfo = questionInfo;
            _historyService = historyService;
            _messageService = inviteStatus;

            _cityService = cityService;
            _imageService = imageService;
            _tagService = tagService;
            _dataSearch = dataSearch;
            _dataImport = dataImport;
            _commentLikeMQService = commentLikeMQService;
            _adminRolereService = adminRolereService;
            _amountMoneyService = amountMoneyService;

            _easyRedisClient = easyRedisClient;
            _mapper = mapper;

            _userMessage = userMessage;

            _adminService = adminService;

            _text = text;
            _SchService = schService;

            hot = new PullHottestCSHelper(_likeservice, _userService, schoolInfoService, _setting, schService);
        }

        #region 

        /// <summary>
        /// 点评列表视图页
        /// </summary>
        /// <returns></returns>
        //[Description("点评视图页")]
        //public IActionResult CommentView() 
        //{
        //    //热门点评
        //    string key = "Comment:Hottest";
        //    //热评学校
        //    string SchoolKey = "Comment:HottestSchool";

        //    //获取热搜词
        //    ViewBag.HotCommentItems = _easyRedisClient.GetOrAddAsync("HostItems",()=> { return _dataSearch.GetHistoryList(1, 6); },new TimeSpan(0,10,0)).Result;

        //    //获取用户当前城市cityCode
        //    int CityCode = Request.GetLocalCity();
        //    //用户id
        //    Guid UserId = Guid.Empty;
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        UserId = User.Identity.GetId();
        //    }

        //    //获取用户当前城市的点评列表页前十条数据
        //    var commentDtos = _commentService.CommentList_Pc(CityCode, 1, 10, UserId);

        //    //得到所有点评的分部学校id
        //    var schoolExtIds = commentDtos?.Select(x => x.SchoolSectionId).ToList();

        //    //学校列表实体
        //    var schoolExts = _schoolInfoService.GetSchoolSectionByIds(schoolExtIds);

        //    //获取侧边栏热门点评
        //    List<SchoolCommentDto> HottestComment = new List<SchoolCommentDto>();

        //    //获取侧边热评学校
        //    List<HotCommentSchoolDto> HottestSchool = new List<HotCommentSchoolDto>();

        //    //如果没有拿到当前用户定位城市则直接展示全国点评数据
        //    if (CityCode > 0) 
        //    {
        //        //热评
        //        //先调用沈总接口获取用户学校推荐算法值【如果为游客用户，可以则直接推荐该城市的全类型数据】
        //        HottestComment =  _commentService.HotComment(new HotCommentQuery()
        //        {
        //            City = 310100,
        //            Grade = 1,
        //            Type = 1,
        //            Discount = false,
        //            Diglossia = false,
        //            Chinese = false,
        //            StartTime = DateTime.Parse("2019-07-31"),
        //            EndTime = DateTime.Parse("2019-08-02")
        //        }, UserId).Result;

        //        //如果数据库中展示不存在5条及以上的数据，则获取全国数据进行补充
        //        if (HottestComment.Count() < 5) 
        //        {
        //            var rez = _easyRedisClient.GetAsync<List<SchoolCommentDto>>(key).Result;
        //            if (rez != null) 
        //            {
        //                HottestComment.AddRange(rez.ToList().Take(5 - HottestComment.Count()));
        //            }
        //        }

        //        //热评学校
        //        HottestSchool = _commentService.HottestSchool(new HotCommentQuery() {
        //            City = 440100,
        //            Grade = 1,
        //            Type = 1,
        //            Discount = false,
        //            Diglossia = false,
        //            Chinese = false,
        //            StartTime = DateTime.Parse("2019-07-31"),
        //            EndTime = DateTime.Parse("2019-08-02")
        //        }, false);

        //        //检测热评学校是否存在5条及以上
        //        if(HottestSchool.Count() < 6) 
        //        {
        //            var rez = _easyRedisClient.GetAsync<List<HotCommentSchoolDto>>(SchoolKey).Result;
        //            if(rez != null) 
        //            {
        //                HottestSchool.AddRange(rez.ToList().Take(6 - HottestSchool.Count()));
        //            }
        //        }
        //    }
        //    else 
        //    {
        //        //热评
        //        HottestComment = _easyRedisClient.GetAsync<List<SchoolCommentDto>>(key).Result;
        //        //热评学校
        //        HottestSchool =  _easyRedisClient.GetAsync<List<HotCommentSchoolDto>>(SchoolKey).Result;
        //    }

        //    //获取点评写入用户信息
        //    var UserIds = new List<Guid>();
        //    UserIds.AddRange(commentDtos.GroupBy(q => q.UserId).Select(p => p.Key).ToList());
        //    UserIds.AddRange(HottestComment.GroupBy(q=>q.UserId).Select(p=>p.Key).ToList());
        //    var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());

        //    //点评分数实体
        //    var score = ScoreToStarHelper.CommentScoreToStar(commentDtos.Select(x => x.Score)?.ToList());

        //    //热门点评分数实体
        //    var HottestScore = ScoreToStarHelper.CommentScoreToStar(HottestComment.Select(x => x.Score)?.ToList());

        //    //热门点评列表【侧边栏】
        //    var HottestCommentView = CommentDtoToVoHelper.CommentDtoToViewModel(_setting.QueryImager, HottestComment, UserHelper.UserDtoToVo(Users), HottestScore);

        //    //热评学校【侧边栏】
        //    var HottestSchoolView = SchoolDtoToSchoolCardHelper.HottestSchoolItem(HottestSchool);

        //    //点评列表
        //    var comment = CommentDtoToVoHelper.CommentDtoToViewModel(_setting.QueryImager, commentDtos, UserHelper.UserDtoToVo(Users), score, SchoolDtoToSchoolCardHelper.SchoolDtoToSchoolCommentCard(schoolExts));

        //    ViewBag.HottestComment = HottestCommentView;
        //    ViewBag.HottestSchoolView = HottestSchoolView;
        //    return View(comment);
        //}

        //[Description("学校点评列表视图")]
        //public IActionResult CommentSchoolView() 
        //{
        //    //热门点评
        //    string key = "Comment:Hottest";
        //    //热评学校
        //    string SchoolKey = "Comment:HottestSchool";

        //    //获取热搜词
        //    ViewBag.HotCommentItems = _easyRedisClient.GetOrAddAsync("HostItems", () => { return _dataSearch.GetHistoryList(1, 6); }, new TimeSpan(0, 10, 0)).Result;

        //    //获取用户当前城市cityCode
        //    int CityCode = Request.GetLocalCity();
        //    //用户id
        //    Guid UserId = Guid.Empty;
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        UserId = User.Identity.GetId();
        //    }

        //    //获取侧边栏热门点评
        //    List<SchoolCommentDto> HottestComment = new List<SchoolCommentDto>();

        //    //获取侧边热评学校
        //    List<HotCommentSchoolDto> HottestSchool = new List<HotCommentSchoolDto>();

        //    //学校列表
        //    List<SchoolInfoDto> SchoolDtos = new List<SchoolInfoDto>();

        //    //如果没有拿到当前用户定位城市则直接展示全国点评数据
        //    if (CityCode > 0)
        //    {
        //        //热评
        //        //先调用沈总接口获取用户学校推荐算法值【如果为游客用户，可以则直接推荐该城市的全类型数据】
        //        HottestComment = _commentService.HotComment(new HotCommentQuery()
        //        {
        //            City = 310100,
        //            Grade = 1,
        //            Type = 1,
        //            Discount = false,
        //            Diglossia = false,
        //            Chinese = false,
        //            StartTime = DateTime.Parse("2019-07-31"),
        //            EndTime = DateTime.Parse("2019-08-02")
        //        }, UserId).Result;

        //        //如果数据库中展示不存在5条及以上的数据，则获取全国数据进行补充
        //        if (HottestComment.Count() < 5)
        //        {
        //            var rez = _easyRedisClient.GetAsync<List<SchoolCommentDto>>(key).Result;
        //            if (rez != null)
        //            {
        //                HottestComment.AddRange(rez.ToList().Take(5 - HottestComment.Count()));
        //            }
        //        }

        //        //热评学校
        //        HottestSchool = _commentService.HottestSchool(new HotCommentQuery()
        //        {
        //            City = 440100,
        //            Grade = 1,
        //            Type = 1,
        //            Discount = false,
        //            Diglossia = false,
        //            Chinese = false,
        //            StartTime = DateTime.Parse("2019-07-31"),
        //            EndTime = DateTime.Parse("2019-08-02")
        //        }, false);

        //        //检测热评学校是否存在5条及以上
        //        if (HottestSchool.Count() < 6)
        //        {
        //            var rez = _easyRedisClient.GetAsync<List<HotCommentSchoolDto>>(SchoolKey).Result;
        //            if (rez != null)
        //            {
        //                HottestSchool.AddRange(rez.ToList().Take(6 - HottestSchool.Count()));
        //            }
        //        }
        //    }
        //    else
        //    {
        //        //热评
        //        HottestComment = _easyRedisClient.GetAsync<List<SchoolCommentDto>>(key).Result;
        //        //热评学校
        //        HottestSchool = _easyRedisClient.GetAsync<List<HotCommentSchoolDto>>(SchoolKey).Result;
        //    }

        //    //获取点评写入用户信息
        //    var UserIds = new List<Guid>();
        //    UserIds.AddRange(HottestComment.GroupBy(q => q.UserId).Select(p => p.Key).ToList());
        //    var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());

        //    //热门点评分数实体
        //    var HottestScore = ScoreToStarHelper.CommentScoreToStar(HottestComment.Select(x => x.Score)?.ToList());

        //    //热门点评列表【侧边栏】
        //    var HottestCommentView = CommentDtoToVoHelper.CommentDtoToViewModel(_setting.QueryImager, HottestComment, UserHelper.UserDtoToVo(Users), HottestScore);

        //    //热评学校【侧边栏】
        //    var HottestSchoolView = SchoolDtoToSchoolCardHelper.HottestSchoolItem(HottestSchool);

        //    //获取学校列表数据
        //    string SchooKey = $"schoolList:city_{CityCode}:pageindex_{1}:pagesize_{10}";

        //    //查询数据库
        //    SchoolDtos =  _easyRedisClient.GetOrAddAsync(SchooKey, () => {
        //        return _schoolInfoService.QuerySchoolCards(true, CityCode, 1, 10);
        //    }, new TimeSpan(0, 10, 0)).Result;


        //    var SchoolCommentCard = SchoolDtoToSchoolCardHelper.SchoolDtoToSchoolCommentCard(SchoolDtos);
        //    ViewBag.HottestComment = HottestCommentView;
        //    ViewBag.HottestSchoolView = HottestSchoolView;
        //    return View(SchoolCommentCard);
        //}

        #endregion


        /// <summary>
        /// 热门点评列表
        /// 先调用沈总接口获取用户学校推荐算法值【如果为游客用户，可以则直接推荐该城市的全类型数据】
        /// 热门点评列表【侧边栏】
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{controller}/{action}")]
        public async Task<ResponseResult> GetHotComments(int cityCode = 0)
        {
            //获取当前的城市
            var localCityCode = cityCode == 0 ? Request.GetLocalCity() : cityCode;
            ViewBag.LocalCity = await _cityService.GetCityName(localCityCode);

            //用户id
            Guid userId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                userId = User.Identity.GetId();
            }

            var now = DateTime.Now;
            var data = hot.HotComment(await _commentService.HotComment(new HotCommentQuery()
            {
                Condition = true,
                City = localCityCode,
                Grade = 1,
                Type = 1,
                Discount = false,
                Diglossia = false,
                Chinese = false,
                StartTime = now.AddMonths(-6),
                EndTime = now,
            }, userId), userId);
            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 点评列表
        /// </summary>
        /// <returns></returns>
        [Description("点评列表")]
        [Route("{controller}/{action=List}")]
        public async Task<IActionResult> List(string search = "", string type = "comment", int page = 1, int city = 0)
        {
            int pageSize = 10;
            ViewBag.PageIndex = page;
            ViewBag.PageSize = pageSize;

            ViewBag.KeyWords = search;
            ViewBag.ListType = type;
            //获取当前的城市
            var localCityCode = city == 0 ? Request.GetLocalCity() : city;
            ViewBag.LocalCity = await _cityService.GetCityName(localCityCode);
            //获取热搜词
            ViewBag.HotSearchWord = await _easyRedisClient.GetOrAddAsync("HotSearchCommentWord", () => { return _dataSearch.GetHotHistoryList(1, 6, null, localCityCode); }, new TimeSpan(0, 10, 0));

            //热门城市
            var hotCity = await _cityService.GetHotCity();
            ViewBag.HotCity = hotCity;

            //用户id
            Guid UserId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                UserId = User.Identity.GetId();
            }
            string UUID = Request.GetDevice();

            //点评视图实体
            List<CommentInfoViewModel> comment = new List<CommentInfoViewModel>();
            //点评dto
            List<SchoolCommentDto> commentDtos = new List<SchoolCommentDto>();

            if (type == "comment")
            {
                //检测是否带有文本模糊匹配，带有文本 带入city查询es数据返回点评id列表
                string UA = Request.Headers["User-Agent"].ToString();
                if (!string.IsNullOrWhiteSpace(search) && !UA.ToLower().Contains("spider") && !UA.ToLower().Contains("bot") && Request.GetAvaliableCity() != 0)
                {
                    if (User.Identity.IsAuthenticated)
                    {
                        var user = User.Identity.GetUserInfo();
                        _dataImport.SetHistory("PC", 1, search, Request.GetAvaliableCity(), user.UserId, UUID);
                    }
                    else
                    {
                        _dataImport.SetHistory("PC", 1, search, Request.GetAvaliableCity(), null, UUID);
                    }
                }
                //根据es返回的点评ids反查点评数据
                var result = _dataSearch.SearchComment(new SearchCommentQuery
                {
                    Keyword = search,
                    Orderby = string.IsNullOrWhiteSpace(search) ? 1 : 0,//没有搜索词默认已发布时间倒序
                    CityIds = new List<int> { localCityCode },
                    PageNo = page,
                    PageSize = pageSize
                });

                //检测是否有数据返回
                if (result.Total > 0)
                {
                    commentDtos = _commentService.QueryCommentByIds(result.Comments.Select(x => x.Id).ToList(), UserId);
                    commentDtos = result.Comments.Where(q => commentDtos.Any(p => p.Id == q.Id))
                    .Select(q => commentDtos.FirstOrDefault(p => p.Id == q.Id)).ToList();
                }
                ViewBag.TotalPage = result.Total;
                //}
                //else
                //{
                //    string CommentKey = $"commentList:city_{localCityCode}:pageindex_{page}:pagesize_{pageSize}";

                //    commentDtos = _commentService.CommentList_Pc(localCityCode, page, pageSize, UserId, out int Total);
                //    ViewBag.TotalPage = Total;
                //}

                //得到所有点评的分部学校id
                var schoolExtIds = commentDtos?.Select(x => x.SchoolSectionId).ToList();

                //获取点评写入用户信息
                var UserIds = new List<Guid>();
                UserIds.AddRange(commentDtos.GroupBy(q => q.UserId).Select(p => p.Key).ToList());
                var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());
                //学校列表实体
                var schoolExts = _schoolInfoService.GetSchoolSectionByIds(schoolExtIds);

                //点评分数实体【需要获取沈总的分数】
                var score = ScoreToStarHelper.CommentScoreToStar(commentDtos.Select(x => x.Score)?.ToList());

                //检测是否关注该点评
                List<Guid> checkCollection = _collectionService.GetCollection(commentDtos.Select(x => x.Id).ToList(), UserId);

                comment = CommentDtoToVoHelper.CommentDtoToViewModel(_setting.QueryImager, commentDtos, UserHelper.UserDtoToVo(Users), score, SchoolDtoToSchoolCardHelper.SchoolDtoToSchoolCommentCard(schoolExts), checkCollection);
                ViewBag.CommentList = comment;
            }
            else
            {
                var schoolQueryData = new SearchSchoolQuery();
                if (string.IsNullOrWhiteSpace(search))
                {
                    schoolQueryData = new SearchSchoolQuery
                    {
                        CurrentCity = localCityCode,
                        PageNo = page,
                        PageSize = pageSize,
                        Orderby = 5
                    };
                }
                else
                {

                    if (User.Identity.IsAuthenticated)
                    {
                        var user = User.Identity.GetUserInfo();
                        _dataImport.SetHistory("PC", 2, search, localCityCode, user.UserId, UUID);
                    }
                    else
                    {
                        _dataImport.SetHistory("PC", 2, search, localCityCode, null, UUID);
                    }

                    schoolQueryData = new SearchSchoolQuery
                    {
                        Keyword = search,
                        PageNo = page,
                        PageSize = pageSize,
                    };
                }

                var list = _dataSearch.SearchSchool(schoolQueryData);

                var ids = list.Schools.Select(q => q.Id).ToList();

                var result = ids.Count > 0 ? _schoolInfoService.GetSchoolSectionByIds(ids) : new List<SchoolInfoDto>();

                //学校列表实体

                var data = list.Schools.Where(q => result.Any(p => p.SchoolSectionId == q.Id)).Select(q =>
                {
                    var r = result.FirstOrDefault(p => p.SchoolSectionId == q.Id);
                    return r == null ? new CommentSchoolViewModel() : new CommentSchoolViewModel
                    {
                        ExtId = r.SchoolSectionId,
                        //Url = SchoolDetailUrl(r.ExtId, r.Sid),
                        SchoolName = r.SchoolName,
                        LodgingType = (int)r.LodgingType,
                        LodgingReason = r.LodgingType.Description(),
                        Type = (int)r.SchoolType,
                        International = r.SchoolType == PMS.School.Domain.Common.SchoolType.International,
                        Auth = r.IsAuth,
                        CommentTotal = r.SectionQuestionTotal,
                        Star = r.SchoolStars,
                        Score = r.SchoolAvgScore,
                        ShortSchoolNo = r.ShortSchoolNo
                    };
                });
                ViewBag.TotalPage = list.Total;
                ViewBag.SchoolList = data?.ToList();
            }

            //热评
            //先调用沈总接口获取用户学校推荐算法值【如果为游客用户，可以则直接推荐该城市的全类型数据】
            //热门点评列表【侧边栏】
            var date_Now = DateTime.Now;
            ViewBag.HottestComment = hot.HotComment(await _commentService.HotComment(new HotCommentQuery()
            {
                Condition = true,
                City = localCityCode,
                Grade = 1,
                Type = 1,
                Discount = false,
                Diglossia = false,
                Chinese = false,
                StartTime = date_Now.AddMonths(-6),
                EndTime = date_Now,
            }, UserId), UserId);

            //热评学校【侧边栏】
            //热评学校
            ViewBag.HottestSchoolView = hot.HottestSchool(await _commentService.HottestSchool(new HotCommentQuery()
            {
                Condition = true,
                City = localCityCode,
                Grade = 1,
                Type = 1,
                Discount = false,
                Diglossia = false,
                Chinese = false,
                StartTime = date_Now.AddMonths(-6),
                EndTime = date_Now,
            }, false));

            return View();
        }

        /// <summary>
        /// 点评学校列表
        /// </summary>
        /// <returns></returns>
        [Description("点评学校列表")]
        public async Task<IActionResult> SchoolList(string search = "", int pageindex = 1, int pagesize = 10, int city = 0)
        {
            //获取当前的城市
            var localCityCode = city == 0 ? Request.GetLocalCity() : city;
            //获取热搜词
            ViewBag.HotCommentItems = _easyRedisClient.GetOrAddAsync("HostItems", () => { return _dataSearch.GetHotHistoryList(1, 6, null, localCityCode); }, new TimeSpan(0, 10, 0)).Result;


            //用户id
            Guid UserId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                UserId = User.Identity.GetId();
            }

            //学校列表
            List<SchoolInfoDto> SchoolDtos = new List<SchoolInfoDto>();


            //获取点评写入用户信息
            var UserIds = new List<Guid>();
            var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());

            if (search == "")
            {
                string SchooKey = $"schoolList:city_{localCityCode}:pageindex_{pageindex}:pagesize_{pagesize}";
                //查询数据库
                SchoolDtos = await _easyRedisClient.GetOrAddAsync(SchooKey, () =>
                {
                    return _schoolInfoService.QuerySchoolCards(true, localCityCode, pageindex, pagesize);
                }, new TimeSpan(0, 10, 0));
            }
            else
            {
                //根据es返回的点评ids反查点评数据
                var result = _dataSearch.SearchSchool(new SearchSchoolQuery
                {
                    Keyword = search,
                    CityCode = new List<int> { localCityCode },
                    PageNo = pageindex,
                    PageSize = pagesize
                });

                if (result.Total > 0)
                {
                    //根据es返回的学校id查询学校实体
                    SchoolDtos = _schoolInfoService.GetSchoolSectionByIds(result.Schools.Select(x => x.Id).ToList());
                }
            }

            //热门点评列表【侧边栏】
            ViewBag.HottestComment = hot.HotComment(await _commentService.HotComment(new HotCommentQuery()
            {
                Condition = true,
                City = 310100,
                Grade = 1,
                Type = 1,
                Discount = false,
                Diglossia = false,
                Chinese = false,
                StartTime = DateTime.Parse("2019-07-31"),
                EndTime = DateTime.Parse("2019-08-02")
            }, UserId), UserId);


            //热评学校【侧边栏】
            ViewBag.HottestSchoolView = hot.HottestSchool(await _commentService.HottestSchool(new HotCommentQuery()
            {
                Condition = true,
                City = 440100,
                Grade = 1,
                Type = 1,
                Discount = false,
                Diglossia = false,
                Chinese = false,
                StartTime = DateTime.Parse("2019-07-31"),
                EndTime = DateTime.Parse("2019-08-02")
            }, false));


            var SchoolCommentCard = SchoolDtoToSchoolCardHelper.SchoolDtoToSchoolCommentCard(SchoolDtos);

            return View(SchoolCommentCard);
        }

        /// <summary>
        /// 学校点评列表页
        /// </summary>
        /// <param name="ExtId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/{controller}/{action}")]
        [Route("/{controller}/{action}/{Id}")]
        [Route("/{controller}/{action}-{schoolNo}")]
        [Route("/{controller}/{action}-{schoolNo}/{Tab}/")]
        [Description("学校点评列表页")]
        public async Task<IActionResult> School(Guid Id, string search, int page = 1, int Tab = 1, string schoolNo = default)
        {
            Guid ExtId = Id;

            if (!string.IsNullOrWhiteSpace(schoolNo))
            {
                var ext = await _SchService.GetSchextSimpleInfoViaShortNo(schoolNo);
                if (ext != null)
                {
                    ExtId = ext.Eid;
                }
                else
                {
                    return new ExtNotFoundViewResult();
                }
            }
            int SelectedTab = Tab;
            int pageSize = 10;
            ViewBag.PageIndex = page;
            ViewBag.PageSize = pageSize;
            ViewBag.ExtId = ExtId;
            ViewBag.KeyWords = search;
            //用户id
            Guid UserId = Guid.Empty;

            if (User.Identity.IsAuthenticated)
            {
                UserId = User.Identity.GetId();
            }
            ViewBag.SelectedTab = SelectedTab;

            //获取该校统计数据
            var SchoolInfo = await _schoolInfoService.QuerySchoolInfo(ExtId);
            if (SchoolInfo.SchoolNo > 0 && schoolNo == default)
            {
                return RedirectPermanent($"/comment/school-{SchoolInfo.ShortSchoolNo}/");
            }

            var schoolData = await _schoolService.GetSchoolExtDtoAsync(ExtId, 0, 0);

            //获取学校综综合评价分值
            var scoreResult = _commentScoreService.GetSchoolScoreById(ExtId);
            var SchoolScore = ScoreToStarHelper.SchoolScoreToStar(new List<PMS.CommentsManage.Application.ModelDto.SchoolScoreDto> { scoreResult })?.FirstOrDefault() ?? new SchoolCmScoreViewModel();
            ViewBag.SchoolScore = SchoolScore;

            //获取点评标签、学校分部标签
            var commentTotal = _commentService.CurrentCommentTotalBySchoolId(SchoolInfo.SchoolId)
            .Select(q => new SchoolCommentTotalViewModel
            {
                SchoolSectionId = q.SchoolSectionId,
                Name = q.Name,
                Total = q.Total,
                TotalType = q.TotalType.Description(),
                TotalTypeNumber = (int)q.TotalType
            }).ToList();
            var AllExtension = _schoolService.GetSchoolExtName(SchoolInfo.SchoolId);
            for (int i = 0; i < AllExtension.Count(); i++)
            {
                var item = commentTotal.Where(x => x.SchoolSectionId == AllExtension[i].Value).FirstOrDefault();
                int currentIndex = commentTotal.IndexOf(item);
                if (item == null)
                {
                    commentTotal.Add(new SchoolCommentTotalViewModel() { Name = AllExtension[i].Key, SchoolSectionId = AllExtension[i].Value, TotalType = QueryCondition.Other.Description(), TotalTypeNumber = (int)QueryCondition.Other });
                }
                else
                {
                    item.Name = AllExtension[i].Key;
                    commentTotal[currentIndex] = item;
                }
            }
            //将其他分部类型学校进行再次排序
            int TotalTypeNumber = 8;
            commentTotal.Where(x => x.TotalTypeNumber == 8)?.ToList().ForEach(x =>
            {
                x.TotalTypeNumber = TotalTypeNumber;
                TotalTypeNumber++;
            });

            //领域实体转 视图实体【学校点评卡片】
            var SchoolCard = SchoolDtoToSchoolCardHelper.SchoolDtoToSchoolCommentCard(new List<SchoolInfoDto>() { SchoolInfo });
            ViewBag.SchoolCard = SchoolCard.FirstOrDefault();

            ViewBag.CommentTotal = commentTotal;


            if (string.IsNullOrWhiteSpace(search))
            {
                List<SchoolCommentDto> HotComment = new List<SchoolCommentDto>();
                if (SelectedTab == 1)
                {
                    //获取学校热门点评数据
                    HotComment = _commentService.SelectedThreeComment(SchoolInfo.SchoolId, UserId, (QueryCondition)1, CommentListOrder.None);
                }

                List<SchoolCommentDto> NewComment;
                int total = 0;

                if (SelectedTab >= 8)
                {
                    Guid eid = commentTotal.Where(x => x.TotalTypeNumber == SelectedTab).FirstOrDefault().SchoolSectionId;
                    NewComment = _commentService.PageSchoolCommentBySchoolId(eid, UserId, page, pageSize, out total, QueryCondition.Other, CommentListOrder.None);
                }
                else
                {
                    //获取学校最新点评数据
                    NewComment = _commentService.PageSchoolCommentBySchoolId(SchoolInfo.SchoolId, UserId, page, pageSize, out total, (QueryCondition)SelectedTab, CommentListOrder.None);
                }

                //获取热门点评写入用户信息
                var UserIds = new List<Guid>();
                UserIds.AddRange(HotComment.GroupBy(q => q.UserId).Select(p => p.Key).ToList());
                UserIds.AddRange(NewComment.GroupBy(q => q.UserId).Select(p => p.Key).ToList());
                var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());

                //抽出点评实体中的评分
                var CommentScore = HotComment.Select(x => x.Score).ToList();
                CommentScore.AddRange(NewComment.Select(x => x.Score).ToList());

                //检测是否关注该点评
                List<Guid> checkCollection = _collectionService.GetCollection(HotComment.Select(x => x.Id).ToList(), UserId);
                checkCollection.AddRange(_collectionService.GetCollection(NewComment.Select(x => x.Id).ToList(), UserId));

                //点评转视图实体
                List<CommentInfoViewModel> HotCommentViewModel = CommentDtoToVoHelper.CommentDtoToViewModel(_setting.QueryImager, HotComment, UserHelper.UserDtoToVo(Users), ScoreToStarHelper.CommentScoreToStar(CommentScore), null, checkCollection);
                List<CommentInfoViewModel> NewCommentViewModel = CommentDtoToVoHelper.CommentDtoToViewModel(_setting.QueryImager, NewComment, UserHelper.UserDtoToVo(Users), ScoreToStarHelper.CommentScoreToStar(CommentScore), null, checkCollection);

                ViewBag.HotCommentList = HotCommentViewModel;
                ViewBag.NewCommentList = NewCommentViewModel;
                ViewBag.TotalPage = total;
            }
            else
            {
                var result = _dataSearch.SearchComment(new SearchCommentQuery
                {
                    Keyword = search,
                    PageNo = page,
                    PageSize = pageSize,
                    Eid = ExtId
                });
                List<SchoolCommentDto> SearchComment = new List<SchoolCommentDto>();
                //检测是否有数据返回
                if (result.Total > 0)
                {
                    SearchComment = _commentService.QueryCommentByIds(result.Comments.Select(x => x.Id).ToList(), UserId);
                }
                ViewBag.TotalPage = result.Total;
                //获取热门点评写入用户信息
                var UserIds = new List<Guid>();
                UserIds.AddRange(SearchComment.GroupBy(q => q.UserId).Select(p => p.Key).ToList());
                var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());

                //抽出点评实体中的评分
                var CommentScore = SearchComment.Select(x => x.Score).ToList();

                //检测是否关注该点评
                List<Guid> checkCollection = _collectionService.GetCollection(SearchComment.Select(x => x.Id).ToList(), UserId);

                //点评转视图实体
                List<CommentInfoViewModel> NewCommentViewModel = CommentDtoToVoHelper.CommentDtoToViewModel(_setting.QueryImager, SearchComment, UserHelper.UserDtoToVo(Users), ScoreToStarHelper.CommentScoreToStar(CommentScore), null, checkCollection);

                ViewBag.NewCommentList = NewCommentViewModel;
            }

            //热评
            //先调用沈总接口获取用户学校推荐算法值【如果为游客用户，可以则直接推荐该城市的全类型数据】
            //热门点评列表【侧边栏】
            var date_Now = DateTime.Now;
            ViewBag.HottestComment = hot.HotComment(await _commentService.HotComment(new HotCommentQuery()
            {
                Condition = true,
                City = Request.GetLocalCity(),
                Grade = 1,
                Type = 1,
                Discount = false,
                Diglossia = false,
                Chinese = false,
                StartTime = date_Now.AddMonths(-6),
                EndTime = date_Now
            }, UserId), UserId);

            //热评学校【侧边栏】
            //热评学校
            ViewBag.HottestSchoolView = hot.HottestSchool(await _commentService.HottestSchool(new HotCommentQuery()
            {
                Condition = true,
                City = Request.GetLocalCity(),
                Grade = 1,
                Type = 1,
                Discount = false,
                Diglossia = false,
                Chinese = false,
                StartTime = date_Now.AddMonths(-6),
                EndTime = date_Now
            }, false));

            //热问
            ViewBag.HottestSchoolQuesitonView = hot.HotQuestion(await _questionInfo.GetHotQuestion());

            //热问学校【侧边栏】
            //热问学校
            ViewBag.HottestQuestionSchoolView = hot.HottestQuestionItem(await _questionInfo.HottestSchool());

            ViewBag.Page_Description = schoolData?.Intro?.GetHtmlHeaderString(150) ?? string.Empty;

            return View();
        }

        /// <summary>
        /// 点评详情
        /// </summary>
        /// <param name="CommentId"></param>
        /// <returns></returns>
        [Description("点评详情")]
        [Route("comment/{No}.html")]
        [Route("comment/detail/{CommentId}")]
        public async Task<IActionResult> Detail(Guid CommentId, string No, int page = 1)
        {
            int pageSize = 10;
            ViewBag.PageIndex = page;
            ViewBag.PageSize = pageSize;


            //检测当前用户身份 （true：校方用户 |false：普通用户）
            ViewBag.isSchoolRole = false;
            //检测是否关注该点评
            ViewBag.isCollected = false;


            Guid userId = Guid.Empty;

            //获取当前点评实体
            var Comment = new SchoolCommentDto();

            long numberId = 0;

            if (No != null)
            {
                numberId = UrlShortIdUtil.Base322Long(No);
            }

            if (numberId > 0)
            {
                Comment = await _easyRedisClient.GetOrAddAsync($"Comment:Detail_No{No}", () =>
                {
                    return _commentService.QueryCommentByNo(numberId);
                }, new TimeSpan(0, 5, 0));
            }
            else
            {
                Comment = await _easyRedisClient.GetOrAddAsync($"Comment:Detail_{CommentId}", () =>
                {
                    return _commentService.QueryComment(CommentId);
                }, new TimeSpan(0, 5, 0));
            }

            if (Comment == null)
            {

            }

            CommentId = Comment.Id;

            var score = await _easyRedisClient.SortedSetScoreAsync("Comment:LikeCount", CommentId.ToString());

            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                userId = user.UserId;
                //检测当前用户身份 （true：校方用户 |false：普通用户）
                ViewBag.isSchoolRole = user.Role.Contains((int)PMS.UserManage.Domain.Common.UserRole.School);
                //检测改用户是否关注该点评
                var isCollected = _collectionService.IsCollected(userId, CommentId, CollectionDataType.Comment);
                ViewBag.isCollected = isCollected;
                //添加用户访问历史记录
                _historyService.AddHistory(userId, Comment.Id, (byte)MessageDataType.Comment);
            }

            ViewBag.Id = Comment.Id;
            ViewBag.No = UrlShortIdUtil.Long2Base32(Comment.No);

            _commentService.UpdateCommentViewCount(Comment.Id);

            if (score != null)
            {
                Comment.LikeCount = (int)score;
            }

            //点评详情实体走的是缓存，点赞的状态不为最新状态，再次进行反查检测最新数据状态

            var isLike = _likeservice.CheckCurrentUserIsLikeBulk(userId, new List<Guid>() { Comment.Id });

            if (isLike?.Any(p => p.SourceId == Comment.Id) == true)
            {
                Comment.IsLike = isLike.FirstOrDefault(p => p.SourceId == Comment.Id).IsLike;
            }

            //var NewCommentStatus = _likeservice.CheckLike(new List<Guid>() { Comment.Id }, userId);
            //if (NewCommentStatus.Contains(Comment.Id))
            //{
            //    //包含该点评详情id则为该用户点赞
            //    Comment.IsLike = true;
            //}
            //else
            //{
            //    //非点赞状态
            //    Comment.IsLike = false;
            //}

            //获取当前点评所在的分部信息
            var School = await _schoolInfoService.QuerySchoolInfo(Comment.SchoolSectionId);

            if (School == null)
            {
                return new ExtNotFoundViewResult();
            }

            //学校平均分
            //var SchoolCommentScore = _commentScoreService.GetSchoolScoreById(Comment.SchoolSectionId);

            //学校卡片
            var SchoolCommentCard = SchoolDtoToSchoolCardHelper.SchoolDtoToSchoolCommentCard(new List<SchoolInfoDto>() { School }).FirstOrDefault();
            ViewBag.SchoolCommentCard = SchoolCommentCard;

            //获取该点评热门回复
            var CommentReplyselected = _commentReplyService.SelectedCommentReply(Comment.Id, userId);

            //获取该点评下最新回复数据
            var Reply = _commentReplyService.CommentReply(Comment.Id, userId, page, pageSize, out int total);



            var UserIds = new List<Guid>()
            {
                Comment.UserId
            };
            UserIds.Add(userId);
            UserIds.AddRange(CommentReplyselected.Select(x => x.ReplyUserId).ToList());
            UserIds.AddRange(Reply.Select(p => p.ReplyUserId).ToList());
            //获取用户列表实体


            var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());


            var extIds = new List<Guid>() {
                Comment.SchoolSectionId
            };
            //学校列表实体
            var schoolExts = _schoolInfoService.GetSchoolSectionByIds(extIds);

            //检测是否关注该点评
            List<Guid> checkCollection = _collectionService.GetCollection(new List<Guid>() { Comment.Id }, userId);

            //获取点评详情视图实体
            var CommentDetail = CommentDtoToVoHelper.CommentDtoToViewModel(_setting.QueryImager, new List<SchoolCommentDto>() { Comment },
                UserHelper.UserDtoToVo(Users),
                ScoreToStarHelper.CommentScoreToStar(new List<CommentScoreDto>() { Comment.Score }),
                schoolExts.Select(q => new SchoolCommentCardViewModel
                {
                    ExtId = q.SchoolSectionId,
                    Sid = q.SchoolId,
                    SchoolName = q.SchoolName,
                    IsAuth = q.IsAuth,
                    International = q.SchoolType == PMS.School.Domain.Common.SchoolType.International,
                    LodgingType = (int)q.LodgingType,
                    LodgingReason = q.LodgingType.Description(),
                    CommentTotal = q.CommentTotal,
                    Stars = SchoolScoreToStart.GetCurrentSchoolstart(q.SchoolAvgScore),
                    SchoolType = q.SchoolType,
                    ShortSchoolNo = q.ShortSchoolNo
                }).ToList(), checkCollection
            ).FirstOrDefault();

            ViewBag.CommentDetail = CommentDetail;

            //热门回复视图实体
            var ReplyHotViewModel = ReplyDtoToViewModelHelper.ReplyDtoToViewModel(CommentReplyselected, UserHelper.UserDtoToVo(Users));
            ViewBag.HotReply = ReplyHotViewModel;

            var ReplyViewModel = ReplyDtoToViewModelHelper.ReplyDtoToViewModel(Reply, UserHelper.UserDtoToVo(Users));
            ViewBag.NewReply = ReplyViewModel;

            ViewBag.TotalPage = total;
            //获取点评标签
            //ViewBag.Tags = _schoolTagService.GetSchoolTagByCommentId(CommentDetail.Id);

            //热评
            //先调用沈总接口获取用户学校推荐算法值【如果为游客用户，可以则直接推荐该城市的全类型数据】
            //热门点评列表【侧边栏】
            var date_Now = DateTime.Now;
            ViewBag.HottestComment = hot.HotComment(await _commentService.HotComment(new HotCommentQuery()
            {
                Condition = true,
                City = Request.GetLocalCity(),
                Grade = 1,
                Type = 1,
                Discount = false,
                Diglossia = false,
                Chinese = false,
                StartTime = date_Now.AddMonths(-6),
                EndTime = date_Now
            }, userId), userId);

            //热评学校【侧边栏】
            //热评学校
            ViewBag.HottestSchoolView = hot.HottestSchool(await _commentService.HottestSchool(new HotCommentQuery()
            {
                Condition = true,
                City = Request.GetLocalCity(),
                Grade = 1,
                Type = 1,
                Discount = false,
                Diglossia = false,
                Chinese = false,
                StartTime = date_Now.AddMonths(-6),
                EndTime = date_Now
            }, false));



            return View();
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
                var user = User.Identity.GetUserInfo();

                GiveLike giveLike = new GiveLike
                {
                    UserId = user.UserId,
                    LikeType = (LikeType)LikeType,
                    SourceId = CommentId
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


        /// <summary>
        /// 提交点评回复
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Description("提交点评回复")]
        public async Task<ResponseResult> CommentReply(ReplyAdd commentReply)
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
                    //检测是否为自己回复自己 
                    if (ReplyUserId != user.UserId)
                    {
                        bool states = _messageService.AddMessage(new PMS.UserManage.Domain.Entities.Message()
                        {
                            title = title,
                            Content = commentReply.Content,
                            DataID = Reply.Id,
                            DataType = (byte)MessageDataType.Comment,
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
        /// 点评回复页
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Description("点评回复页")]
        public async Task<IActionResult> Reply(Guid Id, int page = 1)
        {
            int pageSize = 10;
            ViewBag.PageIndex = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Id = Id;

            Guid userId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                userId = user.UserId;
            }
            var commentReplyId = Id;

            var comment = _commentReplyService.GetCommentReply(commentReplyId);

            if (comment == null)
            {
                throw new NotFoundException();
            }
            _commentReplyService.UpdateReplyViewCount(commentReplyId);

            var isLike = _likeservice.CheckCurrentUserIsLike(userId, commentReplyId);

            var commentReplies = _commentReplyService.PageCommentReplyById(commentReplyId, 1, page, pageSize, out int total);


            List<Guid> replyIds = commentReplies.GroupBy(q => q.Id).Select(s => s.Key).ToList();

            var isLikes = _likeservice.CheckCurrentUserIsLikeBulk(userId, replyIds);


            List<Guid> userIds = new List<Guid> { comment.UserId };
            userIds.AddRange(commentReplies.GroupBy(q => q.UserId).Select(s => (Guid)s.Key).ToList());
            var users = _userService.ListUserInfo(userIds);


            var replyUser = users.FirstOrDefault(q => q.Id == comment.UserId);

            List<Guid> parentUserIds = commentReplies.Where(p => p.ReplyId != null).GroupBy(q => q.ReplyUserId).Select(s => (Guid)s.Key).ToList();
            var parentUsers = _userService.ListUserInfo(parentUserIds);

            //学校卡片
            //获取当前点评所在的分部信息
            var school = await _schoolInfoService.QuerySchoolInfo(comment.SchoolComment.SchoolSectionId);
            if (school == null)
            {
                return new ExtNotFoundViewResult();
            }
            var SchoolCard = SchoolDtoToSchoolCardHelper.SchoolDtoToSchoolCommentCard(new List<SchoolInfoDto>() { school }).FirstOrDefault();
            ViewBag.SchoolCard = SchoolCard;

            var reply = new CommentReplyViewModel
            {
                Id = comment.Id,
                CommentId = comment.SchoolCommentId,
                Content = comment.Content,
                IsLike = isLike,
                IsAttend = comment.IsAttend,
                IsStudent = comment.IsSchoolPublish,
                IsAnonymou = comment.IsAnony,
                ReplyTotal = comment.ReplyCount,
                LikeTotal = comment.LikeCount,
                AddTime = comment.CreateTime.ConciseTime(),
                UserInfoVo = UserHelper.ToAnonyUserName(new UserInfoVo
                {
                    Id = comment.UserId,
                    NickName = replyUser.NickName,
                    HeadImager = replyUser.HeadImgUrl,
                    Role = replyUser.VerifyTypes.ToList()
                }, comment.IsAnony)
            };

            ViewBag.ReplyDetail = reply;

            var replyList = commentReplies.Select(q => new CommentReplyViewModel
            {
                Id = q.Id,
                Content = q.Content,
                CommentId = q.SchoolCommentId,
                IsLike = isLikes.FirstOrDefault(p => p.SourceId == q.Id) != null && isLikes.FirstOrDefault(p => p.SourceId == q.Id).IsLike,
                IsAttend = q.IsAttend,
                IsStudent = q.IsSchoolPublish,
                ReplyTotal = q.ReplyCount,
                LikeTotal = q.LikeCount,
                UserInfoVo = UserHelper.UserDtoToVo(users).FirstOrDefault(),
                AddTime = q.CreateTime.ConciseTime(),
                IsAnonymou = q.IsAnony
            }).ToList();
            ViewBag.ReplyList = replyList;
            ViewBag.TotalPage = total;

            //热评
            //先调用沈总接口获取用户学校推荐算法值【如果为游客用户，可以则直接推荐该城市的全类型数据】
            //热门点评列表【侧边栏】
            ViewBag.HottestComment = hot.HotComment(await _commentService.HotComment(new HotCommentQuery()
            {
                Condition = true,
                City = 310100,
                Grade = 1,
                Type = 1,
                Discount = false,
                Diglossia = false,
                Chinese = false,
                StartTime = DateTime.Parse("2019-07-31"),
                EndTime = DateTime.Parse("2019-08-02")
            }, userId), userId);

            //热评学校【侧边栏】
            //热评学校
            ViewBag.HottestSchoolView = hot.HottestSchool(await _commentService.HottestSchool(new HotCommentQuery()
            {
                Condition = true,
                City = 440100,
                Grade = 1,
                Type = 1,
                Discount = false,
                Diglossia = false,
                Chinese = false,
                StartTime = DateTime.Parse("2019-07-31"),
                EndTime = DateTime.Parse("2019-08-02")
            }, false));

            return View();

        }
        /// <summary>
        /// 获取最新回复
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [Description("获取点评最新回复")]
        public ResponseResult GetCurrentCommentNewReply(Guid CommentId, int PageIndex, int PageSize)
        {
            try
            {
                Guid UserId = Guid.Empty;
                if (User.Identity.IsAuthenticated)
                {
                    UserId = User.Identity.GetId();
                }

                List<CommentReplyViewModel> NewReply = new List<CommentReplyViewModel>();

                //获取该点评下最新回复数据
                var Reply = _commentReplyService.CommentReply(CommentId, UserId, PageIndex, PageSize, out int total);

                if (Reply.Count() > 0)
                {
                    //获取用户列表实体
                    var Users = _userService.ListUserInfo(Reply.Select(p => p.ReplyUserId).ToList());

                    //回复列表Dto 转 视图实体
                    var ReplyViewModel = ReplyDtoToViewModelHelper.ReplyDtoToViewModel(Reply, UserHelper.UserDtoToVo(Users));
                    NewReply.AddRange(ReplyViewModel);
                }
                return ResponseResult.Success(new { rows = NewReply });
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        /// <summary>
        /// 最新点评，滑动分页
        /// </summary>
        /// <param name="SchoolId"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [Description("获取最新点评")]
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

                //获取学校最新点评数据
                var Comment = _commentService.PageSchoolCommentBySchoolId(SchoolId, userId, PageIndex, PageSize, out int total, query, commentListOrder);
                if (Comment.Any())
                {
                    var UserIds = new List<Guid>();
                    UserIds.AddRange(Comment.GroupBy(q => q.UserId).Select(p => p.Key).ToList());
                    var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());

                    //抽出点评实体中的评分
                    var CommentScore = Comment.Select(x => x.Score).ToList();

                    //检测是否关注该点评
                    List<Guid> checkCollection = _collectionService.GetCollection(Comment.Select(x => x.Id).ToList(), userId);

                    var CommentViewModel = CommentDtoToVoHelper.CommentDtoToViewModel(_setting.QueryImager, Comment, UserHelper.UserDtoToVo(Users), ScoreToStarHelper.CommentScoreToStar(CommentScore), null, checkCollection);
                    return ResponseResult.Success(new { rows = CommentViewModel });
                }
                else
                {
                    return ResponseResult.Success(new { rows = new List<CommentInfoViewModel>() });
                }
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        /// <summary>
        /// 写点评页
        /// </summary>
        /// <param name="QuestionId"></param>
        /// 
        /// <returns></returns>
        [Authorize]
        [Description("写点评页")]
        public async Task<IActionResult> Write(Guid eid)
        {
            var schoolInfo = _schoolInfoService.GetSchoolName(new List<Guid>() { eid });

            bool isSchool = false;
            var userInfo = User.Identity.GetUserInfo();

            if (User.Identity.GetUserInfo().Role.Contains((int)PMS.UserManage.Domain.Common.UserRole.School))
            {
                isSchool = true;
            }

            ViewBag.IsSchool = isSchool;
            var schoolViewModel = SchoolDtoToSchoolCardHelper.SchoolDtoToSchoolCommentCard(schoolInfo).FirstOrDefault();
            ViewBag.SchoolInfo = schoolViewModel;

            var allSchoolExt = _schoolInfoService.GetCurrentSchoolAllExt(schoolViewModel.Sid);
            var currentSchool = allSchoolExt.Where(x => x.SchoolSectionId == eid).FirstOrDefault();
            ViewBag.AllSchoolBranch = allSchoolExt.Where(x => x.SchoolSectionId != eid);

            //热评
            //先调用沈总接口获取用户学校推荐算法值【如果为游客用户，可以则直接推荐该城市的全类型数据】
            //热门点评列表【侧边栏】
            var date_Now = DateTime.Now;
            ViewBag.HottestComment = hot.HotComment(await _commentService.HotComment(new HotCommentQuery()
            {
                Condition = true,
                City = 310100,
                Grade = 1,
                Type = 1,
                Discount = false,
                Diglossia = false,
                Chinese = false,
                StartTime = date_Now.AddMonths(-6),
                EndTime = date_Now
            }, userInfo.UserId), userInfo.UserId);

            //热评学校【侧边栏】
            //热评学校
            ViewBag.HottestSchoolView = hot.HottestSchool(await _commentService.HottestSchool(new HotCommentQuery()
            {
                Condition = true,
                City = 440100,
                Grade = 1,
                Type = 1,
                Discount = false,
                Diglossia = false,
                Chinese = false,
                StartTime = date_Now.AddMonths(-6),
                EndTime = date_Now
            }, false));

            return View();
        }

        [Authorize]
        [Description("提交点评")]
        public ResponseResult CommitComment(CommentWriterViewModel comment, Guid commentId, List<string> imgUrls, List<CommentTagViewModel> Tags)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(comment.Content))
                {
                    return ResponseResult.Failed("还未输入点评内容");
                }
                else if
               (
                    (!comment.IsAttend && comment.AggScore == 0) ||
                    (comment.IsAttend && (comment.ManageScore == 0 || comment.LifeScore == 0 || comment.EnvirScore == 0 ||
                    comment.TeachScore == 0 || comment.HardScore == 0)))
                {
                    return ResponseResult.Failed("还未评分");
                }

                var ext = _schoolService.GetSchoolExtension(comment.Eid);
                if (ext == null)
                {
                    return ResponseResult.Failed("该学部不存在");
                }

                var contentCheck = _text.GarbageCheck(new ProductManagement.API.Aliyun.Model.GarbageCheckRequest()
                {
                    scenes = new[] { "antispam" },
                    tasks = new List<ProductManagement.API.Aliyun.Model.GarbageCheckRequest.Task>() {
                      new ProductManagement.API.Aliyun.Model.GarbageCheckRequest.Task(){
                       content=comment.Content,
                         dataId = Guid.NewGuid().ToString()
                      }
                      }
                }).Result;

                if (contentCheck.data.FirstOrDefault().results.FirstOrDefault().suggestion == "block")
                {
                    return ResponseResult.Failed("您发布的内容包含敏感词，<br>请重新编辑后再发布。");
                }

                Guid userId = Guid.Empty;
                UserRole userRole = new UserRole();
                if (User.Identity.IsAuthenticated)
                {
                    var user = User.Identity.GetUserInfo();
                    userId = user.UserId;
                    userRole = user.Role.Length > 0 ? (UserRole)user.Role[0] : UserRole.Member;
                }

                bool checkWriter = true;
                List<int> roles = new List<int>();

                var Job = _adminService.GetModelById(userId);
                //检测该用户是否为存在于兼职管理平台中的用户，平台用户写点评不允许存在相同点评
                if (Job != null)
                {
                    checkWriter = _commentService.Checkisdistinct(comment.Content);
                    roles = _adminRolereService.GetPartJobRoles(userId).Select(x => x.Role).ToList();
                }

                //检测是否为兼职领队用户，领队用户写点评，将自动新建兼职身份
                if (roles.Contains(2) && !roles.Contains(1) && Job.SettlementType == SettlementType.SettlementWeChat)
                {
                    //包含兼职领队身份，但还并未包含兼职身份时，写点评需要新增兼职用户身份，且直接归属到自身下的兼职

                    //新增兼职身份
                    _adminRolereService.Add(new PartTimeJobAdminRole() { AdminId = userId, Role = 1, ParentId = userId });

                    //新增任务周期
                    _amountMoneyService.NextSettlementData(Job, 1);
                }

                //首先创建点评对象，得到该点评id，进行拼接点评图片路径
                SchoolComment schoolComment = _mapper.Map<CommentWriterViewModel, SchoolComment>(comment);

                if (!checkWriter)
                {
                    schoolComment.State = ExamineStatus.Block;
                }

                schoolComment.Id = commentId;
                schoolComment.SchoolId = comment.Sid;
                schoolComment.SchoolSectionId = comment.Eid;
                schoolComment.CommentUserId = userId;
                schoolComment.PostUserRole = userRole;

                //_log.Debug($"点评调试{schoolComment.Content},{schoolComment.SchoolSectionId},{schoolComment.SchoolId}");


                List<SchoolImage> imgs = new List<SchoolImage>();
                //上传图片
                if (imgUrls.Count() > 0)
                {
                    //标记该点评有上传过图片
                    schoolComment.IsHaveImagers = true;

                    //需要上传图片
                    imgs = new List<SchoolImage>();

                    for (int i = 0; i < imgUrls.Count(); i++)
                    {
                        SchoolImage commentImg = new SchoolImage
                        {
                            DataSourcetId = schoolComment.Id,
                            ImageType = ImageType.Comment,
                            ImageUrl = imgUrls[i]
                        };

                        //图片上传成功，提交数据库
                        imgs.Add(commentImg);
                    }
                }

                _commentService.AddSchoolComment(schoolComment);

                //所有邀请我点评这个学校的消息, 全部设为已读,已处理
                _messageService.UpdateMessageHandled(true, MessageType.InviteComment, MessageDataType.School, userId, default, new List<Guid>() { schoolComment.SchoolSectionId });


                //_log.Debug($"点评调试{schoolComment.Content},{schoolComment.SchoolSectionId},{schoolComment.SchoolId}");

                SchoolCommentScore commentScore = _mapper.Map<CommentWriterViewModel, SchoolCommentScore>(comment);
                commentScore.CommentId = schoolComment.Id;
                //已入读，录入该点评本次平均分
                if (commentScore.IsAttend)
                {
                    //总分录入平均分
                    commentScore.AggScore = Convert.ToInt32(commentScore.AggScore) / 5;
                }

                _commentScoreService.AddSchoolComment(commentScore);

                foreach (CommentTagViewModel tag in Tags)
                {
                    if (tag.Id == default(Guid))
                    {
                        var newTag = _tagService.CheckTagIsExists(tag.TagName);
                        //检测该标签是否存在
                        if (newTag == null)
                        {
                            //标签不存在
                            tag.Id = Guid.NewGuid();
                            _tagService.Add(new CommentTag() { Id = tag.Id, Content = tag.TagName });

                            SchoolTag schoolTag = new SchoolTag
                            {
                                TagId = tag.Id,
                                SchoolCommentId = schoolComment.Id,
                                UserId = userId,
                                SchoolId = schoolComment.SchoolSectionId
                            };
                            _schoolTagService.Add(schoolTag);
                        }
                        else
                        {
                            //标签已存在，添加关系
                            SchoolTag schoolTag = new SchoolTag
                            {
                                TagId = newTag.Id,
                                SchoolCommentId = schoolComment.Id,
                                UserId = userId,
                                SchoolId = schoolComment.SchoolSectionId
                            };
                            _schoolTagService.Add(schoolTag);
                        }
                    }
                    else
                    {
                        SchoolTag schoolTag = new SchoolTag
                        {
                            TagId = tag.Id,
                            SchoolCommentId = schoolComment.Id,
                            UserId = userId,
                            SchoolId = schoolComment.SchoolSectionId
                        };
                        _schoolTagService.Add(schoolTag);
                    }
                }

                if (imgs.Count != 0)
                {
                    foreach (var img in imgs)
                    {
                        _imageService.AddSchoolImage(img);
                    }
                }

                //_log.Debug($"点评调试{schoolComment.Content},{schoolComment.SchoolSectionId},{schoolComment.SchoolId}");



                return ResponseResult.Success(new
                {
                    Eid = schoolComment.SchoolSectionId,
                    No = ext.ShortSchoolNo
                });
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }


        [Description("写点评页获取学校标签")]
        public ResponseResult GetTagBySchoolId(Guid SchoolId)
        {
            var Tags = _schoolTagService.GetSchoolTagBySchoolId(SchoolId).Select(x => x.Tag).ToList();
            var tagvos = Tags.Select(q => new CommentTagViewModel
            {
                Id = q.Id,
                TagName = q.Content
            }).ToList();
            return ResponseResult.Success(new
            {
                row = tagvos
            });
        }

    }
}
