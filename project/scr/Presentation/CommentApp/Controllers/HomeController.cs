using AutoMapper;
using iSchool;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NSwag.Annotations;
using Org.BouncyCastle.Asn1.Esf;
using PMS.CommentsManage.Application.Common;
using PMS.CommentsManage.Application.IServices;
using PMS.Infrastructure.Application.IService;
using PMS.Infrastructure.Application.ModelDto;
using PMS.Live.Application.IServices;
using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.School.Application.IServices;
using PMS.School.Application.ModelDto;
using PMS.School.Domain.Common;
using PMS.School.Domain.Dtos;
using PMS.School.Domain.Entities;
using PMS.School.Infrastructure.Common;
using PMS.TopicCircle.Application.Services;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Entities;
using ProductManagement.API.Http.Interface;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using Sxb.Web;
using Sxb.Web.Common;
using Sxb.Web.Models;
using Sxb.Web.Models.Comment;
using Sxb.Web.Models.Question;
using Sxb.Web.Models.School;
using Sxb.Web.Response;
using Sxb.Web.Utils;
using Sxb.Web.ViewModels.FeedBack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DrawingCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static PMS.Search.Domain.Common.GlobalEnum;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace CommentApp.Controllers
{
    [OpenApiIgnore]
    public class HomeController : Controller
    {
        private readonly ISchoolService _schoolService;
        private readonly ISchoolCommentService _commentService;
        private readonly ISchoolCommentScoreService _commentScoreService;
        private readonly ISchoolRankService _schoolRankService;
        private readonly IQuestionInfoService _questionInfoService;
        private readonly IHttpClientFactory _clientFactory;
        private IConfiguration _configuration;
        private IArticleService _articleService;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly ICityInfoService _cityCodeService;
        private readonly ISchoolInfoService _schoolInfo;
        private readonly IMapper _mapper;
        private readonly ImageSetting _setting;
        private readonly IUserService _userService;
        private readonly IUserServiceClient _userServiceClient;
        private readonly IUserRecommendClient _userRecommendClient;
        private readonly ILogger Log;
        private readonly ILiveServiceClient _liveClient;
        private readonly IFeedbackService _feedbackService;
        private readonly IJumpUrlService _jumpUrlService;
        INavigationService _navigationService;
        ITalentService _talentService;
        ILectureService _lectureService;
        ITopicService _topicService;
        ICircleService _circleService;

        public HomeController(ICircleService circleService,
            ITopicService topicService,
            ILectureService lectureService,
            ITalentService talentService,
            ICommentReportService commentReportService,
            IAnswerReportService reportService,
            ISchoolService schoolService,
            ISchoolCommentService schoolComment,
            ISchoolCommentScoreService commentScoreService,
            IQuestionInfoService questionInfoService,
            IUserService userService,
            ICityInfoService cityService,
            IHttpClientFactory clientFactory,
            IMapper mapper, IOptions<ImageSetting> set,
            IConfiguration iConfig,
            IArticleService articleService,
            IEasyRedisClient easyRedisClient,
            IUserServiceClient userServiceClient,
            IUserRecommendClient userRecommendClient,
            ISchoolInfoService schoolInfo,
            ISchoolRankService schoolRankService,
            IJumpUrlService jumpUrlService,
            IFeedbackService feedbackService, ILiveServiceClient liveClient,
            ILoggerFactory log, INavigationService navigationService)
        {
            _circleService = circleService;
            _topicService = topicService;
            _lectureService = lectureService;
            _talentService = talentService;
            _navigationService = navigationService;
            _cityCodeService = cityService;
            _schoolService = schoolService;
            _commentService = schoolComment;
            _commentScoreService = commentScoreService;
            _questionInfoService = questionInfoService;
            _configuration = iConfig;
            _clientFactory = clientFactory;
            _articleService = articleService;
            _easyRedisClient = easyRedisClient;
            _userService = userService;
            _mapper = mapper;
            _setting = set.Value;
            _userServiceClient = userServiceClient;
            _userRecommendClient = userRecommendClient;
            _schoolInfo = schoolInfo;
            _feedbackService = feedbackService;
            _schoolRankService = schoolRankService;
            _jumpUrlService = jumpUrlService;
            _liveClient = liveClient;
            Log = log.CreateLogger(GetType().FullName);
        }

        /// <summary>
        /// 学校首页
        /// </summary>
        /// <param name="type"></param>
        /// <param name="orderBy">orderby 1推荐 2口碑 3.附近</param>
        /// <param name="index"></param>
        /// <param name="distance"></param>
        /// <param name="size"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        [Description("首页")]
        public async Task<IActionResult> Index2(int type = 0, int orderBy = 1, int index = 1, int distance = 0, int size = 10, int province = 0, int city = 0, int area = 0)
        {
            //首页请求user.sxkid.com,透传cookie记录devicetoken
            _ = _userServiceClient.ReMarkDeviceToken(Request.Headers["user-agent"], Request.GetAllCookies());
            int historyCity = city;

            //从cookie中  取出省市区  年级
            if (city == 0)
                city = Request.GetLocalCity();
            else
            {
                Response.SetLocalCity(city.ToString());
                if (province == 0 && area == 0)
                {
                    Response.SetProvince("0");
                    Response.SetArea("0");
                }
            }

            if (province == 0)
                province = Request.GetProvince();
            else
                Response.SetProvince(province.ToString());

            if (area == 0)
                area = Request.GetArea();
            else
                Response.SetArea(area.ToString());

            var latitude = Request.GetLatitude();
            var longitude = Request.GetLongitude();

            //获取当前的城市
            var localCityCode = city == 0 ? Request.GetLocalCity() : city;

            //判断当前平台是否是app
            var isApp = UserAgentHelper.Check(Request) == UA.Mobile;
            ViewBag.IsApp = isApp;

            //将数据保存在前台
            ViewBag.LocalCity = await _cityCodeService.GetCityName(localCityCode);
            ViewBag.Province = province;
            ViewBag.City = city;
            ViewBag.Area = area;
            ViewBag.Latitude = latitude;
            ViewBag.Longitude = longitude;
            ViewBag.OrderBy = orderBy;

            //如果有推荐列表  采用推荐列表
            //否则采用默认列表
            var list = new SchoolExtListDto();
            var recommend = await GetUserRecommendExtAsync(index, size);
            if (recommend != null && recommend.Items != null && recommend.Items.Any())
            {
                var extids = recommend.Items
                    .Where(p => Guid.TryParse(p.ContentId, out Guid result))
                    .Select(p => Guid.Parse(p.ContentId)).ToArray();
                var exts = await _schoolService.ListExtSchoolByBranchIds(extids.ToList(), Convert.ToDouble(latitude), Convert.ToDouble(longitude));


                var extList = new List<SchoolExtItemDto>();
                //var extList = exts
                //    .Where(p => p.SchoolValid == true && p.ExtValid == true)
                //    .Select(p => new SchoolExtItemDto
                //    {
                //        Name = p.Name,
                //        Grade = p.Grade,
                //        Type = (byte)p.Type,
                //        Area = p.Area,
                //        AreaCode = p.AreaCode,
                //        City = p.City,
                //        CityCode = p.CityCode,
                //        Province = p.Province,
                //        ProvinceCode = p.ProvinceCode,
                //        Score = p.Score,
                //        Distance = p.Distance,
                //        ExtId = p.ExtId,
                //        Sid = p.Sid,
                //        Lodging = p.Lodging,
                //        Tags = p.Tags,
                //        Tuition = p.Tuition,
                //    }).ToList();
                foreach (var extid in extids)
                {
                    var ext = exts.FirstOrDefault(e => e.SchoolValid == true && e.ExtValid == true && e.ExtId == extid);
                    if (ext != null)
                    {
                        extList.Add(new SchoolExtItemDto
                        {
                            Name = ext.Name,
                            Grade = ext.Grade,
                            Type = (byte)ext.Type,
                            Area = ext.Area,
                            AreaCode = ext.AreaCode,
                            City = ext.City,
                            CityCode = ext.CityCode,
                            Province = ext.Province,
                            ProvinceCode = ext.ProvinceCode,
                            Score = ext.Score,
                            Distance = ext.Distance,
                            ExtId = ext.ExtId,
                            Sid = ext.Sid,
                            Lodging = ext.Lodging,
                            Sdextern = ext.Sdextern,
                            Tags = ext.Tags,
                            Tuition = ext.Tuition,
                            SchoolNo = ext.SchoolNo
                        });
                    }
                }

                //获取评论与排行榜
                var rank = _schoolRankService.GetH5SchoolRankInfoBy(extids);
                var comment = _commentService.GetSchoolCardComment(extids.ToList(), PMS.CommentsManage.Domain.Common.SchoolSectionOrIds.SchoolSectionIds, default(Guid));
                extList.ForEach(p =>
                {
                    p.Ranks = rank.Where(g => g.SchoolId == p.ExtId).ToList();
                    var commentData = comment.FirstOrDefault(g => g.SchoolSectionId == p.ExtId);
                    p.Comment = commentData?.Content;
                    p.CommentCount = commentData == null ? 0 : commentData.ReplyCount;
                    p.CommontId = commentData?.Id;
                    p.CommontNo = UrlShortIdUtil.Long2Base32(commentData?.No ?? 0).ToLower();
                });

                list.IsRecommend = true;
                list.List = extList;
                list.PageCount = recommend.PageInfo.TotalCount;
                list.PageIndex = index;
                list.PageSize = size;
                ViewBag.PageSize = recommend.PageInfo.TotalPage;
            }
            else
            {
                //根据用户问卷获取学校列表
                //获取年级  类型  住宿
                var grades = new int[] { };
                var types = new int[] { };
                var Lodging = new int[] { };

                ApiInterest interest = await GetInterestAsync(historyCity);
                if (interest != null)
                {
                    grades = interest.interest.grade;
                    types = interest.interest.nature;
                    Lodging = interest.interest.lodging;
                }
                list = await _schoolService.GetSchoolExtListAsync(Convert.ToDouble(longitude), Convert.ToDouble(latitude), province, city, area, grades, orderBy, types, distance, Lodging, index, size);
                list.PageSize = size;
                list.IsRecommend = false;
                ViewBag.PageSize = (list.PageCount + size - 1) / size;
            }

            if (Request.GetClientType() != UA.Mobile)
            {
                //获取首页直播banner
                var lectures = await _easyRedisClient.GetOrAddAsync($"Lectures:HomeList:{localCityCode}", () => { return _liveClient.StickLectures(localCityCode).Result; }, new TimeSpan(0, 5, 0));
                ViewBag.Lectures = lectures?.Items != null ? lectures.Items.Select(q => new LectureVM
                {
                    Id = q.Id,
                    HomeCover = q.HomeCover,
                    Status = q.Status
                }).ToList() : new List<LectureVM>();
            }
            return View(list);
        }

        /// <summary>
        /// 学校首页
        /// </summary>
        /// <param name="type"></param>
        /// <param name="orderBy">orderby 1推荐 2口碑 3.附近</param>
        /// <param name="index"></param>
        /// <param name="distance"></param>
        /// <param name="size"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        [Description("首页")]
        public async Task<IActionResult> Index(int type = 0, int orderBy = 1, int index = 1, int distance = 0, int size = 10, int province = 0, int city = 0, int area = 0)
        {
            if (Request.GetClientType() == UA.Mobile)
            {
                return RedirectToRoute(new { controller = "Home", action = "Index2", type, orderBy, index, distance, size, province, city, area });
            }

            _ = _userServiceClient.ReMarkDeviceToken(Request.Headers["user-agent"], Request.GetAllCookies());
            int historyCity = city;
            //从cookie中  取出省市区  年级
            if (city == 0)
                city = Request.GetLocalCity();
            else
            {
                Response.SetLocalCity(city.ToString());
                if (province == 0 && area == 0)
                {
                    Response.SetProvince("0");
                    Response.SetArea("0");
                }
            }

            if (province == 0)
                province = Request.GetProvince();
            else
                Response.SetProvince(province.ToString());

            if (area == 0)
                area = Request.GetArea();
            else
                Response.SetArea(area.ToString());

            var localCityCode = city == 0 ? Request.GetLocalCity() : city;

            ViewBag.LocalCity = await _cityCodeService.GetCityName(localCityCode);

            //导航
            var navigations = await _navigationService.GetNavigations();

            //直播Banner
            var lectures = await _easyRedisClient.GetOrAddAsync($"Lectures:HomeList:{localCityCode}", async () =>
            {
                return await _liveClient.StickLectures(localCityCode);
            }, TimeSpan.FromMinutes(5));


            //达人榜
            IEnumerable<KeyValueDto<string, string, string, Guid, int>> talents = await _easyRedisClient.GetOrAddAsync("Home:TalentList", async () =>
             {
                 var finds = await _talentService.GetTodayTalentRanking();
                 if (finds?.ID != default)
                 {
                     try
                     {
                         var talentItems = JsonConvert.DeserializeObject<List<TalentRankingItem>>(finds.DataJson);
                         if (talentItems?.Any() == true)
                         {
                             var userIDs = talentItems.Select(p => p.UserID).Distinct();
                             Dictionary<Guid, string> userHeadImgs = new Dictionary<Guid, string>();
                             var talentDetails = _userService.GetTalentDetails(userIDs);
                             if (userIDs?.Any() == true)
                             {
                                 userHeadImgs = _userService.ListUserInfo(userIDs.ToList())?.ToDictionary(k => k.Id, v => v.HeadImgUrl);
                             }
                             return talentItems.Take(4).Select(p => new KeyValueDto<string, string, string, Guid, int>()
                             {
                                 Key = p.TalentName,
                                 Value = userHeadImgs.GetValueOrDefault(p.UserID),
                                 Message = p.TalentTitle,
                                 Data = p.UserID,
                                 Other = talentDetails.FirstOrDefault(o => o.Id == p.UserID)?.Role ?? -1
                             });
                         }
                     }
                     catch
                     { }
                 }
                 return null;
             }, TimeSpan.FromMinutes(30));

            //搜索框轮询建议
            var searchSuggest = await _easyRedisClient.GetOrAddAsync("Home:SearchSuggest", async () =>
            {
                //获取最新文章
                var newestArticle = await _articleService.HotPointArticles(localCityCode, null, 0, 1);
                //获取最新直播
                var newestLive = await _lectureService.GetNewestLecture(1);
                //获取最新帖子
                var newestTopic = _topicService.GetHotList(null, localCityCode);
                //0 全部, 1 学校 ,2 点评 ,4 问答, 8 学校排名, 16 文章 ,32 直播, 64 话题圈 , 128 达人
                var result = new Dictionary<string, ChannelIndex>();
                if (newestTopic?.Any() == true)
                {
                    result.Add(newestTopic.OrderByDescending(p => p.CreateTime).First().CircleName, ChannelIndex.Circle);
                }
                if (newestLive?.Any() == true)
                {
                    result.Add(newestLive.First().Title, ChannelIndex.Live);
                }
                if (newestArticle.articles?.Any() == true)
                {
                    result.Add(newestArticle.articles.OrderBy(p => p.createTime).First().title, ChannelIndex.Article);
                }
                return result;
            }, TimeSpan.FromHours(1));



            if (talents?.Any() == true)
            {
                ViewBag.Talents = talents;
            }
            if (navigations?.Any() == true)
            {
                if (localCityCode != 440100)
                {
                    var nav = navigations.FirstOrDefault(q => q.Name == "上学问");
                    if (nav != null)
                    {
                        nav.Name = "问答广场";
                        nav.TargetUrl = "/comment?type=1";
                        nav.IconUrl = "https://cos.sxkid.com/images/navigationicon/cf90a312-3385-41d2-83c7-ea64d1d02ad5/6140b09d-78af-4e62-ba93-d5ed5e66d44f.png";
                    }
                }
                ViewBag.Navigations = navigations;
            }
            if (lectures?.Items?.Any() == true)
            {
                ViewBag.Lectures = lectures.Items.Select(q => new LectureVM
                {
                    Id = q.Id,
                    HomeCover = q.HomeCover,
                    Status = q.Status
                });
            }
            ViewBag.SearchSuggest = searchSuggest;

            var latitude = Request.GetLatitude();
            var longitude = Request.GetLongitude();

            //判断当前平台是否是app
            var isApp = UserAgentHelper.Check(Request) == UA.Mobile;
            ViewBag.IsApp = isApp;

            //将数据保存在前台
            ViewBag.LocalCity = await _cityCodeService.GetCityName(localCityCode);
            ViewBag.Province = province;
            ViewBag.City = city;
            ViewBag.Area = area;
            ViewBag.Latitude = latitude;
            ViewBag.Longitude = longitude;
            ViewBag.OrderBy = orderBy;
            var list = new SchoolExtListDto();
            var recommend = await GetUserRecommendExtAsync(index, size);
            if (recommend != null && recommend.Items != null && recommend.Items.Any())
            {
                var extids = recommend.Items
                    .Where(p => Guid.TryParse(p.ContentId, out Guid result))
                    .Select(p => Guid.Parse(p.ContentId)).ToArray();
                var exts = await _schoolService.ListExtSchoolByBranchIds(extids.ToList(), Convert.ToDouble(latitude), Convert.ToDouble(longitude));


                var extList = new List<SchoolExtItemDto>();
                foreach (var extid in extids)
                {
                    var ext = exts.FirstOrDefault(e => e.SchoolValid == true && e.ExtValid == true && e.ExtId == extid);
                    if (ext != null)
                    {
                        extList.Add(new SchoolExtItemDto
                        {
                            Name = ext.Name,
                            Grade = ext.Grade,
                            Type = (byte)ext.Type,
                            Area = ext.Area,
                            AreaCode = ext.AreaCode,
                            City = ext.City,
                            CityCode = ext.CityCode,
                            Province = ext.Province,
                            ProvinceCode = ext.ProvinceCode,
                            Score = ext.Score,
                            Distance = ext.Distance,
                            ExtId = ext.ExtId,
                            Sid = ext.Sid,
                            Lodging = ext.Lodging,
                            Sdextern = ext.Sdextern,
                            Tags = ext.Tags,
                            Tuition = ext.Tuition,
                            SchoolNo = ext.SchoolNo
                        });
                    }
                }

                //获取评论与排行榜
                var rank = _schoolRankService.GetH5SchoolRankInfoBy(extids);
                var comment = _commentService.GetSchoolCardComment(extids.ToList(), PMS.CommentsManage.Domain.Common.SchoolSectionOrIds.SchoolSectionIds, default(Guid));
                extList.ForEach(p =>
                {
                    p.Ranks = rank.Where(g => g.SchoolId == p.ExtId).ToList();
                    var commentData = comment.FirstOrDefault(g => g.SchoolSectionId == p.ExtId);
                    p.Comment = commentData?.Content;
                    p.CommentCount = commentData == null ? 0 : commentData.ReplyCount;
                    p.CommontId = commentData?.Id;
                    p.CommontNo = UrlShortIdUtil.Long2Base32(commentData?.No ?? 0).ToLower();
                });

                list.IsRecommend = true;
                list.List = extList;
                list.PageCount = recommend.PageInfo.TotalCount;
                list.PageIndex = index;
                list.PageSize = size;
                ViewBag.PageSize = recommend.PageInfo.TotalPage;
            }
            else
            {
                //根据用户问卷获取学校列表
                //获取年级  类型  住宿
                var grades = new int[] { };
                var types = new int[] { };
                var Lodging = new int[] { };

                ApiInterest interest = await GetInterestAsync(historyCity);
                if (interest != null)
                {
                    grades = interest.interest.grade;
                    types = interest.interest.nature;
                    Lodging = interest.interest.lodging;
                }
                list = await _schoolService.GetSchoolExtListAsync(Convert.ToDouble(longitude), Convert.ToDouble(latitude), province, city, area, grades, orderBy, types, distance, Lodging, index, size);
                list.PageSize = size;
                list.IsRecommend = false;
                ViewBag.PageSize = (list.PageCount + size - 1) / size;
            }
            return View(list);
        }
        public class LectureVM
        {
            public Guid Id { get; set; }
            public string HomeCover { get; set; }
            public int Status { get; set; }
        }

        /// <summary>
        /// 动态加载分页数据
        /// </summary>
        /// <param name="index"></param>
        /// <param name="orderBy">orderby 1推荐 2口碑 3.附近</param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="area"></param>
        /// <param name="distance"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        [Description("首页加载分页数据")]
        public async Task<IActionResult> SchoolExtsPartial(int index = 1, int orderBy = 1, int province = 0, int city = 0, int area = 0, int distance = 0, int size = 10, bool isrecommend = false)
        {
            var latitude = Request.GetLatitude();
            var longitude = Request.GetLongitude();
            //如果orderby 1
            //推荐列表 先判断是否有推荐信息
            if (orderBy == 1 && isrecommend)
            {
                var recommend = await GetUserRecommendExtAsync(index, size);
                if (recommend != null && recommend.Items != null && recommend.Items.Count() > 0)
                {
                    var data = new SchoolExtListDto();
                    var extids = recommend.Items.Select(p => new Guid(p.ContentId)).ToArray();
                    var exts = await _schoolService.ListExtSchoolByBranchIds(extids.ToList(), Convert.ToDouble(latitude), Convert.ToDouble(longitude));

                    var extList = exts
                        .Where(p => p.SchoolValid == true && p.ExtValid == true)
                        .Select(p => new SchoolExtItemDto
                        {
                            Name = p.Name,
                            Grade = p.Grade,
                            Type = (byte)p.Type,
                            Area = p.Area,
                            AreaCode = p.AreaCode,
                            City = p.City,
                            CityCode = p.CityCode,
                            Province = p.Province,
                            ProvinceCode = p.ProvinceCode,
                            Score = p.Score,
                            Distance = p.Distance,
                            ExtId = p.ExtId,
                            Sid = p.Sid,
                            Lodging = p.Lodging,
                            Sdextern = p.Sdextern,
                            Tags = p.Tags,
                            Tuition = p.Tuition,
                            SchoolNo = p.SchoolNo
                        }).ToList();
                    //获取评论与排行榜
                    var rank = _schoolRankService.GetH5SchoolRankInfoBy(extids);
                    var comment = _commentService.GetSchoolCardComment(extids.ToList(), PMS.CommentsManage.Domain.Common.SchoolSectionOrIds.SchoolSectionIds, default(Guid));
                    extList.ForEach(p =>
                    {
                        p.Ranks = rank.Where(g => g.SchoolId == p.ExtId).ToList();
                        var commentData = comment.FirstOrDefault(g => g.SchoolSectionId == p.ExtId);
                        p.Comment = commentData?.Content;
                        p.CommentCount = commentData == null ? 0 : commentData.ReplyCount;
                        p.CommontId = commentData?.Id;
                        p.CommontNo = UrlShortIdUtil.Long2Base32(commentData?.No ?? 0);
                    });

                    data.IsRecommend = true;
                    data.List = extList;
                    data.PageCount = recommend.PageInfo.TotalCount;
                    data.PageIndex = index;
                    data.PageSize = size;

                    ViewBag.UA = UserAgentHelper.Check(Request);

                    return PartialView(data);
                }
            }

            var grades = new int[] { };
            var types = new int[] { };
            var Lodging = new int[] { };
            ApiInterest interest = await GetInterestAsync();
            if (interest != null)
            {
                grades = interest.interest.grade;
                types = interest.interest.nature;
                Lodging = interest.interest.lodging;
            }

            var list = await _schoolService.GetSchoolExtListAsync(Convert.ToDouble(longitude), Convert.ToDouble(latitude), province, city, area, grades, orderBy, types, distance, Lodging, index, size);
            list.PageSize = size;
            list.IsRecommend = false;
            ViewBag.UA = UserAgentHelper.Check(Request);
            return PartialView(list);
        }

        /// <summary>
        /// 切换城市
        /// </summary>
        /// <returns></returns>
        [Description("首页切换城市")]
        public async Task<ActionResult> Select()
        {
            var localCityCode = Request.GetLocalCity();
            var adCodes = await _cityCodeService.IntiAdCodes();
            var adPinYinCodes = (await _cityCodeService.GetAllCityPinYin()).SelectMany(p => p.Value);
            List<AreaDto> history = new List<AreaDto>();
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                string Key = $"SearchCity:{user.UserId}";
                var historyCity = await _easyRedisClient.SortedSetRangeByRankAsync<AreaDto>(Key, 0, 10, StackExchange.Redis.Order.Descending);
                history.AddRange(historyCity.ToList());
            }

            var ua = UserAgentHelper.Check(Request);

            //城市编码
            var newAdCodes = adCodes.Select(p => new
            {
                Key = p.Key,
                CityCodes = p.CityCodes.Select(x =>
                new
                {
                    AdCode = x.Id,
                    Name = x.Name,
                    url = ua == UA.Mobile ? "ischool://goback?action=refresh_home&url=" + Uri.EscapeDataString(Url.Action("Index", new { orderBy = 1, Index = 1, city = x.Id }))
                    : Url.Action("Index", new { orderBy = 1, Index = 1, city = x.Id }),
                    pinyin = string.Join("", adPinYinCodes.FirstOrDefault(g => g.CityCode == x.Id)?.Pinyin)
                })
            });
            ViewBag.AllCity = JsonHelper.ObjectToJSON(newAdCodes);

            //获取所有code
            var allCode = adCodes
                .SelectMany(p => p.CityCodes)
                .Select(x => new
                {
                    AdCode = x.Id,
                    Name = x.Name,
                    url = ua == UA.Mobile ? "ischool://goback?action=refresh_home&url=" + Uri.EscapeDataString(Url.Action("Index", new { orderBy = 1, Index = 1, city = x.Id }))
                    : Url.Action("Index", new { orderBy = 1, Index = 1, city = x.Id })
                });
            ViewBag.AllCode = JsonHelper.ObjectToJSON(allCode);

            //当前城市adcode
            ViewBag.LocalCity = allCode.FirstOrDefault(p => p.AdCode == localCityCode)?.Name;
            //热门城市
            var hotCity = await _cityCodeService.GetHotCity();
            ViewBag.HotCity = JsonHelper.ObjectToJSON(new
            {
                Key = "",
                CityCodes = hotCity.Select(x =>
                  new
                  {
                      AdCode = x.AdCode,
                      Name = x.AreaName,
                      url = ua == UA.Mobile ? "ischool://goback?action=refresh_home&url=" + Uri.EscapeDataString(Url.Action("Index", new { orderBy = 1, Index = 1, city = x.AdCode }))
                    : Url.Action("Index", new { orderBy = 1, Index = 1, city = x.AdCode })
                  })
            });
            //历史记录
            ViewBag.History = JsonHelper.ObjectToJSON(new
            {
                Key = "",
                CityCodes = history.Select(x =>
                    new
                    {
                        AdCode = x.AdCode,
                        Name = x.AreaName,
                        url = ua == UA.Mobile ? "ischool://goback?action=refresh_home&url=" + Uri.EscapeDataString(Url.Action("Index", new { orderBy = 1, Index = 1, city = x.AdCode }))
                    : Url.Action("Index", new { orderBy = 1, Index = 1, city = x.AdCode })
                    })
            });

            return View();
        }

        /// <summary>
        /// 获取学校分部基本信息
        /// 查询数据添加缓存
        /// </summary>
        /// <returns></returns>
        [Description("获取学校分部基本信息")]
        public async Task<IActionResult> Detail(Guid extId = default(Guid), Guid sid = default(Guid), Guid id = default(Guid))
        {
            var latitude = Request.GetLatitude();
            var longitude = Request.GetLongitude();

            //兼容旧的seo
            if (extId == default(Guid)) extId = id;

            var data = await _schoolService.GetSchoolExtSimpleDtoAsync(Convert.ToDouble(longitude), Convert.ToDouble(latitude), extId);
            //如果学校分部为null的话跳转到错误页面
            if (data == null)
                return new ExtNotFoundViewResult();

            //获取文章列表
            //分部文章
            var extArticles = await Task<IEnumerable<article>>.Run(() =>
            {
                return _articleService.GetComparisionArticles(new List<Guid> { extId }, 2);
            });
            //政策文章
            var policyArticles = await Task.Run(() =>
            {
                return _articleService.GetPolicyArticles(new List<PMS.OperationPlateform.Domain.DTOs.SchoolType> { new PMS.OperationPlateform.Domain.DTOs.SchoolType { Chinese = data.Chinese ?? false, Diglossia = data.Diglossia ?? false, Discount = data.Discount ?? false, SchoolGrade = data.Grade, Type = data.Type } }, new List<PMS.OperationPlateform.Domain.DTOs.Local> { new PMS.OperationPlateform.Domain.DTOs.Local { AreaId = data.Area, CityId = data.City, ProvinceId = data.Province } }, 2);
            });
            //相关推荐学校
            var Recommend = await _schoolService.RecommendSchoolAsync(data.ExtId, data.Type, data.Grade, data.City, 2);

            ViewBag.ExtId = data.ExtId;
            ViewBag.Sid = data.Sid;
            //<<<<<<该学部是否有被收藏 获取用户名<<<<<<<<
            bool collectStatat = false;
            string userName = "";
            Guid userId = default(Guid);
            ViewBag.isSchoolRole = false;

            //添加历史
            _userServiceClient.AddHistory(new ProductManagement.API.Http.Model.AddHistory
            {
                dataID = extId,
                dataType = MessageDataType.School,
                cookies = HttpContext.Request.GetAllCookies()
            });

            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                userId = user.UserId;
                userName = user.Name + "，";
                //var config = _configuration.GetSection("SchoolCollection");
                //var ip = config.GetSection("ServerUrl").Value;
                //var port = string.IsNullOrEmpty(config.GetSection
                //    ("Port").Value) ? "" : ":" + config.GetSection("Port").Value;
                //var request = new HttpRequestMessage(HttpMethod.Get, $"{ip}{port}?userId={user.UserId}&&dataId={data.ExtId}");
                //var client = _clientFactory.CreateClient();
                //var response = await client.SendAsync(request);

                var isCollection = await _userServiceClient.IsCollected(new ProductManagement.API.Http.Model.IsCollected { dataID = data.ExtId, userId = userId });
                ////检测当前用户身份 （true：校方用户 |false：普通用户）
                ViewBag.isSchoolRole = user.Role.Contains((int)UserRole.School);
                //if (isCollection.status == 200)
                //{
                //var responseResult = await response.Content.ReadAsStringAsync();
                //var dir = JsonHelper.DataRowFromJSON(responseResult);
                if (isCollection.status == 0)
                {
                    if (isCollection.iscollected == true)
                    {
                        collectStatat = true;
                    }
                }
                //}
            }
            ViewBag.CollectStatus = collectStatat;
            ViewBag.UserName = userName;

            //外教比例
            double numerator = 0;
            double denominator = 0;
            if (data.ForeignTea != null && data.ForeignTea != 0)
            {
                var foreignTea = Math.Round(Convert.ToDecimal(data.ForeignTea), 0);
                var number = NumberHelper.ConvertToFraction((int)foreignTea, 100);
                numerator = number.Item1;
                denominator = number.Item2;
            }
            ViewBag.Numerator = numerator;
            ViewBag.Denominator = denominator;
            //师生比例
            string teaProportion = "-";
            if (data.Studentcount != null && data.Teachercount != null && data.Studentcount != 0 && data.Teachercount != null)
            {
                var teaNumber = NumberHelper.ConvertToFraction(data.Teachercount.Value, data.Studentcount.Value);
                teaProportion = $"{teaNumber.Item1}:{teaNumber.Item2}";
            }
            ViewBag.TeaProportion = teaProportion;
            //招生比例
            var proportion = "-";
            if (data.Proportion != null && data.Proportion != 0)
            {
                var pro = Math.Round(Convert.ToDecimal(data.Proportion), 0);
                //var proNumber = NumberHelper.ConvertToFraction((float)pro);
                proportion = $"1:{pro}";
            }
            ViewBag.Proportion = proportion;

            //对口学校地址
            var counterPart = new List<KeyValueDto<Guid>>();
            if (!string.IsNullOrEmpty(data.CounterPart))
            {
                //反序列化
                counterPart = JsonHelper.JSONToObject<List<KeyValueDto<Guid>>>(data.CounterPart);
                if (counterPart.Count > 0)
                {
                    var counterPartExtId = counterPart.Select(p => p.Value);
                    var address = _schoolService.GetSchoolExtAdress(counterPartExtId.ToArray());
                    counterPart = counterPart.Select(p => new KeyValueDto<Guid> { Key = p.Key, Value = p.Value, Message = address.FirstOrDefault(a => a.Value == p.Value).Key }).ToList();
                }
            }
            ViewBag.CounterPart = counterPart;
            //升学成绩内容
            object achData = null;
            if (data.AchYear != null)
            {
                achData = _schoolService.GetAchData(extId, data.Grade, data.Type, data.AchYear.Value);
            }
            ViewBag.AchData = achData;
            //其他分部
            var schoolExts = _schoolService.GetSchoolExtName(data.Sid);
            ViewBag.SchoolExts = schoolExts;
            //附近学位房
            var buildings = new StringBuilder();
            if (data.Longitude != null && data.Latitude != null)
            {
                var distance = _configuration.GetSection("BuildDistance").Get<int>();
                var list = (await _schoolService.GetBuildingDataAsync(data.ExtId, data.Latitude, data.Longitude)).Select(p => p.Key + "," + p.Value + "," + p.Message);
                buildings.Append(string.Join(",", list));
            }
            ViewBag.Build = buildings.ToString();

            //获取学校的分数
            //var extCharacter = await _schoolService.GetSchoolCharacterAsync(extId);
            //ViewBag.Character = extCharacter;
            var scoreData = await GetSchoolExtScoreAsync(extId);
            ViewBag.Score = scoreData.score.FirstOrDefault(p => p.IndexId == 22)?.Score;
            //文章列表
            ViewBag.ExtArticles = extArticles;
            ViewBag.PolicyArticles = policyArticles;
            //推荐学校
            ViewBag.Recommend = Recommend;

            //学校点评
            var schoolComment = _commentService.GetSchoolSelectedComment(extId, userId);
            CommentList commentDetail = null;
            if (schoolComment != null)
            {
                var userInfo = _userService.GetUserInfo(schoolComment.UserId);
                var user = UserHelper.ToAnonyUserName(new Sxb.Web.Models.User.UserInfoVo() { HeadImager = userInfo.HeadImgUrl, Id = userInfo.Id, NickName = userInfo.NickName, Role = userInfo.VerifyTypes?.ToList() }, schoolComment.IsAnony);
                commentDetail = _mapper.Map<PMS.CommentsManage.Application.ModelDto.SchoolCommentDto, CommentList>(schoolComment);
                for (int i = 0; i < commentDetail.Images.Count(); i++)
                {
                    commentDetail.Images[i] = _setting.QueryImager + commentDetail.Images[i];
                }
                commentDetail.UserInfo = user;
            }
            ViewBag.SelectedComment = commentDetail;

            //学校问答
            var schoolQuestion = _questionInfoService.PushSchoolInfo(new List<Guid>() { extId }, userId);
            QuestionVo questionDetail = null;
            if (schoolQuestion.Count() > 0)
            {
                if (schoolQuestion[0].Id != default(Guid))
                {
                    questionDetail = _mapper.Map<PMS.CommentsManage.Application.ModelDto.QuestionDto, QuestionVo>(schoolQuestion[0]);
                    for (int i = 0; i < questionDetail.Images.Count(); i++)
                    {
                        questionDetail.Images[i] = _setting.QueryImager + questionDetail.Images[i];
                    }
                    if (questionDetail.Answer.Any())
                    {
                        //获取写回答用户信息
                        var userInfo = _userService.GetUserInfo(questionDetail.Answer[0].UserId);
                        var user = UserHelper.ToAnonyUserName(new Sxb.Web.Models.User.UserInfoVo() { HeadImager = userInfo.HeadImgUrl, Id = userInfo.Id, NickName = userInfo.NickName, Role = userInfo.VerifyTypes?.ToList() }, questionDetail.Answer[0].IsAnony);

                        questionDetail.Answer[0].UserInfo = user;
                    }
                }
            }

            ViewBag.SelectedQuestion = questionDetail;
            //ua
            ViewBag.UA = UserAgentHelper.Check(Request);

            return View(data);
        }

        /// <summary>
        /// 获取学校详情
        /// 先读取缓存，在填充数据
        /// </summary>
        /// <returns></returns>
        [Description("获取学校详情")]
        public async Task<IActionResult> Data(Guid extId, Guid sid = default(Guid))
        {
            var latitude = Request.GetLatitude();
            var longitude = Request.GetLongitude();

            var data = await _schoolService.GetSchoolExtDtoAsync(extId, Convert.ToDouble(latitude), Convert.ToDouble(longitude));
            //如果学校分部为null的话跳转到错误页面
            if (data == null)
                return new ExtNotFoundViewResult();

            //添加历史
            _userServiceClient.AddHistory(new ProductManagement.API.Http.Model.AddHistory { dataID = extId, dataType = MessageDataType.School, cookies = HttpContext.Request.GetAllCookies() });

            var result = data;
            //<<<<<<<<该学部是否有被收藏 获取用户名<<<<<<<<<
            bool collectStatat = false;
            string userName = "";
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                userName = user.Name + "，";
                //var config = _configuration.GetSection("SchoolCollection");
                //var ip = config.GetSection("ServerUrl").Value;
                //var port = string.IsNullOrEmpty(config.GetSection
                //    ("Port").Value) ? "" : ":" + config.GetSection("Port").Value;
                //var request = new HttpRequestMessage(HttpMethod.Get, $"{ip}{port}?userId={user.UserId}&&dataId={result.ExtId}");
                //var client = _clientFactory.CreateClient();
                //var response = await client.SendAsync(request);
                //if (response.StatusCode == HttpStatusCode.OK)
                //{
                //var responseResult = await response.Content.ReadAsStringAsync();
                var isCollection = await _userServiceClient.IsCollected(new ProductManagement.API.Http.Model.IsCollected { dataID = result.ExtId, userId = user.UserId });
                //var dir = JsonHelper.DataRowFromJSON(responseResult);
                if (isCollection.status == 0)
                {
                    if (isCollection.iscollected == true)
                    {
                        collectStatat = true;
                    }
                }
                //}
            }
            ViewBag.CollectStatus = collectStatat;
            ViewBag.UserName = userName;
            //开放日
            var openhour = new List<KeyValueDto<DateTime?>>();
            (string.IsNullOrEmpty(result.OpenHours) ? new List<KeyValueDto<string>>() : JsonHelper.JSONToObject<List<KeyValueDto<string>>>(result.OpenHours))
                .ForEach(p =>
                {
                    if (string.IsNullOrEmpty(p.Value))
                    {
                        openhour.Add(new KeyValueDto<DateTime?> { Key = p.Key, Value = null });
                    }
                    else
                    {
                        DateTime date = DateTime.MinValue;
                        DateTime.TryParse(p.Value, out date);
                        openhour.Add(new KeyValueDto<DateTime?>() { Key = p.Key, Value = date });
                    }
                });
            ViewBag.OpenHours = openhour.OrderBy(p => p.Value).ToList();
            //行事日历
            var calendar = new List<KeyValueDto<DateTime?>>();
            (string.IsNullOrEmpty(result.Calendar) ? new List<KeyValueDto<string>>() : JsonHelper.JSONToObject<List<KeyValueDto<string>>>(result.Calendar)).ForEach(p =>
            {
                if (string.IsNullOrEmpty(p.Value))
                {
                    calendar.Add(new KeyValueDto<DateTime?> { Key = p.Key, Value = null });
                }
                else
                {
                    DateTime date = DateTime.MinValue;
                    DateTime.TryParse(p.Value, out date);
                    calendar.Add(new KeyValueDto<DateTime?>() { Key = p.Key, Value = date });
                }
            });
            ViewBag.Calendar = calendar.OrderBy(p => p.Value).ToList();
            //对口学校地址
            var counterPart = new List<KeyValueDto<Guid>>();
            if (!string.IsNullOrEmpty(result.CounterPart))
            {
                //反序列化
                counterPart = JsonHelper.JSONToObject<List<KeyValueDto<Guid>>>(result.CounterPart);
                if (counterPart.Count > 0)
                {
                    var counterPartExtId = counterPart.Select(p => p.Value);
                    var address = _schoolService.GetSchoolExtAdress(counterPartExtId.ToArray());
                    counterPart = counterPart.Select(p => new KeyValueDto<Guid> { Key = p.Key, Value = p.Value, Message = address.FirstOrDefault(a => a.Value == p.Value).Key }).ToList();
                }
            }
            ViewBag.CounterPart = counterPart;
            //出国方向
            //外教比例
            double numerator = 0;
            double denominator = 0;
            if (result.ForeignTea != null)
            {
                var foreignTea = Math.Round(Convert.ToDecimal(result.ForeignTea), 0);
                var number = NumberHelper.ConvertToFraction((int)foreignTea, 100);
                numerator = number.Item1;
                denominator = number.Item2;
            }
            ViewBag.Numerator = numerator;
            ViewBag.Denominator = denominator;
            //师生比例
            string teaProportion = "-";
            if (result.Studentcount != null && result.Teachercount != null && result.Studentcount != 0 && result.Teachercount != null)
            {
                var tea = Math.Round(Convert.ToDecimal(result.Teachercount / result.Studentcount), 0);
                var teaNumber = NumberHelper.ConvertToFraction(result.Teachercount.Value, result.Studentcount.Value);
                teaProportion = $"{teaNumber.Item1}:{teaNumber.Item2}";
            }
            ViewBag.TeaProportion = teaProportion;
            //出国方向
            var Abroad = string.IsNullOrEmpty(result.Abroad) ? new List<KeyValueDto<string>>() : JsonHelper.JSONToObject<List<KeyValueDto<string>>>(result.Abroad);
            ViewBag.Abroad = Abroad;
            //学校认证
            var Authentication = string.IsNullOrEmpty(result.Authentication) ? new List<KeyValueDto<string>>() : JsonHelper.JSONToObject<List<KeyValueDto<string>>>(result.Authentication);
            ViewBag.Authentication = Authentication;
            //学校特色课程
            var Characteristic = string.IsNullOrEmpty(result.Characteristic) ? new List<KeyValueDto<string>>() : JsonHelper.JSONToObject<List<KeyValueDto<string>>>(result.Characteristic);
            ViewBag.Characteristic = Characteristic;
            //附近学位房
            var buildings = new StringBuilder();
            if (result.Longitude != null && result.Latitude != null)
            {
                var distance = _configuration.GetSection("BuildDistance").Get<int>();
                var list = (await _schoolService.GetBuildingDataAsync(result.ExtId, result.Latitude, result.Longitude, distance)).Select(p => p.Key + "," + p.Value + "," + p.Message);
                buildings.Append(string.Join(",", list));
            }
            ViewBag.Build = buildings.ToString();

            //获取学校的特征
            //var extCharacter = await _schoolService.GetSchoolCharacterAsync(extId);
            //ViewBag.Character = extCharacter;

            var schoolScore = await GetSchoolExtScoreAsync(extId);
            ViewBag.ScoreIndex = schoolScore.index;
            ViewBag.Score = schoolScore.score;

            return View(result);
        }

        /// <summary>
        /// 搜索页面
        /// </summary>
        /// <returns></returns>
        [Description("搜索页面")]
        public IActionResult Search()
        {
            return View();
        }

        /// <summary>
        /// 下载页面
        /// </summary>
        /// <returns></returns>
        public IActionResult Download()
        {
            return View();
        }

        public ResponseResult CheckLogin()
        {
            var config = HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
            var userServiceUrl = config.GetSection("UserSystemConfig").GetValue<string>("ServerUrl");


            bool IsLogin = User.Identity.IsAuthenticated;
            return ResponseResult.Success(new { IsLogin, LoginUrl = userServiceUrl + "/login/?return=" });
        }

        /// <summary>
        /// 获取学校分部列表
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        [Description("获取学校分部列表")]
        public ResponseResult GetSchoolExtension(int PageIndex, int PageSize)
        {
            var schools = _mapper.Map<List<SchoolDto>, List<SchoolListVo>>(_schoolService.GetSchoolExtensions(PageIndex, PageSize));

            for (int i = 0; i < schools.Count(); i++)
            {
                Guid SchoolId = schools[i].Id;
                schools[i].SchoolCommentTotal = _commentService.SchoolTotalComment(SchoolId);
                schools[i].Score = int.Parse(Math.Ceiling(_commentScoreService.GetAvgScoreBybaraBranchSchool(SchoolId)).ToString());
                schools[i].StartTotal = SchoolScoreToStart.GetCurrentSchoolstart(schools[i].Score);
                var schoolComment = (_commentService.SelectedThreeComment(SchoolId, Guid.Empty)).FirstOrDefault();
                if (schoolComment != null)
                {
                    schools[i].SelectedCommentContent = schoolComment.Content;
                    schools[i].SelectedReplyTotal = schoolComment.ReplyCount;
                    schools[i].CommentId = schoolComment.Id;
                }
                else
                {
                    schools[i].SelectedCommentContent = "";
                }
            }
            //schools.ForEach(x => { x.SchoolCommentTotal = ; x.Score = ; x.SchoolComment = ; x.StartTotal = SchoolScoreToStart.GetCurrentSchoolstart(x.Score); });

            //string json = JsonConvert.SerializeObject(schools);
            return ResponseResult.Success(schools);
        }

        /// <summary>
        /// 获取用户的问卷
        /// 0走读  1寄宿
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [Description("获取用户的问卷")]
        private async Task<ApiInterest> GetInterestAsync(int historyCity = 0)
        {
            ApiInterest interest = null;
            //http请求api
            var config = _configuration.GetSection("UserSystemConfig");
            var ip = config.GetSection("ServerUrl").Value;
            var uuid = Request.GetDevice(Guid.NewGuid().ToString());
            var url = string.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                interest = await _easyRedisClient.GetAsync<ApiInterest>($"interest-{user.UserId.ToString()}-{uuid}");
                url = $"{ip}/Info/GetUserInterest?userID={user.UserId.ToString()}&uuid={uuid}";
                //添加历史记录
                if (historyCity != 0)
                {
                    var cityinfo = await _cityCodeService.GetInfoByCityCode(historyCity);
                    string historyKey = $"SearchCity:{user.UserId.ToString()}";
                    await _easyRedisClient.SortedSetAddAsync(historyKey, cityinfo, DateTime.Now.D2I(), StackExchange.Redis.CommandFlags.FireAndForget);
                }
            }
            else
            {
                interest = await _easyRedisClient.GetAsync<ApiInterest>($"interest--{Request.GetDevice()}");
                url = $"{ip}/Info/GetUserInterest?uuid={uuid}";
            }
            if (interest == null)
            {
                //url请求
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var client = _clientFactory.CreateClient();
                try
                {
                    var response = await client.SendAsync(request);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var responseResult = await response.Content.ReadAsStringAsync();
                        var jsonData = JsonHelper.JSONToObject<ApiInterest>(responseResult);
                        if (jsonData.status == 0)
                        { return jsonData; }
                        else { return null; }
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return interest;
        }

        [Description("获取用户分部分数")]
        public async Task<(List<SchoolExtScoreIndex> index, List<SchoolExtScore> score)> GetSchoolExtScoreAsync(Guid extId)
        {
            var index = await _schoolService.schoolExtScoreIndexsAsync();
            if (index == null || index.Count == 0)
                return (null, null);
            else
            {
                var score = await _schoolService.GetSchoolExtScoreAsync(extId);
                return (index, score);
            }
        }

        /// <summary>
        /// 获取用户推荐学校
        /// </summary>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [Description("获取用户推荐学校")]
        public async Task<RecommenderResponseDto> GetUserRecommendExtAsync(int index, int size = 10)
        {
            //用户id or  设备id
            var id = Request.GetDevice(Guid.NewGuid().ToString());
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                id = user.UserId.ToString();
            }
            //测试id
            //id = "44a7285a-66b8-45e4-bbf8-e60b14e14590";

            //string key = $"UserRecommendExtApi:{id}:{index}";
            try
            {
                var response = await _userRecommendClient.UserRecommendSchool(Guid.Parse(id), index, size);
                var result = new RecommenderResponseDto
                {
                    ErrorDescription = response.ErrorDescription,
                    Status = response.Status,
                    PageInfo = new PageInfo
                    {
                        CountPerpage = response.PageInfo?.CountPerpage ?? 0,
                        Curpage = response.PageInfo?.Curpage ?? 0,
                        TotalCount = response.PageInfo?.TotalCount ?? 0,
                        TotalPage = response.PageInfo?.TotalPage ?? 0,
                    },
                    Items = response.Items.Select(q => new RecommenderDto
                    {
                        BuType = q.BuType,
                        ContentId = q.ContentId
                    }).ToList()
                };

                if (result.PageInfo.TotalPage < 1 || result.PageInfo.TotalCount < 10)
                {
                    return null;
                }

                return result;

            }
            catch
            {
            }
            return null;
        }


        /// <summary>
        /// 第三方页面跳转中转
        /// </summary>
        /// <returns></returns>
        [Description("第三方页面跳转中转")]
        [Route("jump")]
        public async Task<ActionResult> Jump(string url,string fw)
        {
            url = string.IsNullOrEmpty(url) ? "/" : Uri.UnescapeDataString(url);

            await _jumpUrlService.AddCount(url, fw);
            return Redirect(url);
        }

        [Description("常见问题")]
        public ActionResult Problems(int idx)
        {
            ViewBag.idx = idx;
            return View();
        }


        [Description("客户服务")]
        public ActionResult Customer()
        {
            return View();
        }

        [Description("客服留言")]
        [Authorize]
        public ActionResult CustomerService()
        {
            return View();
        }

        /// <summary>
        /// 提交点评回复
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        [Description("提交意见反馈")]
        [HttpPost]
        public ResponseResult FeedbackSubmit(List<string> imagesArr, string text, byte type)
        {
            try
            {
                var user = User.Identity.GetUserInfo();
                Feedback feedback = new Feedback()
                {
                    Id = Guid.NewGuid(),
                    UserID = user.UserId,
                    time = DateTime.Now,
                    text = text,
                    type = type
                };

                bool insert = _feedbackService.AddFeedback(feedback);

                if (insert)
                {
                    //数据添加成功
                    if (imagesArr.Any())
                    {
                        //带有图片
                        var imgs = HelperCenterUploadImage(imagesArr, feedback.Id);
                        _feedbackService.AddFeedbackImg(imgs);
                    }
                }
                return ResponseResult.Success();
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        public List<Feedback_Img> HelperCenterUploadImage(List<string> imagesArr, Guid feedbackId)
        {
            List<Feedback_Img> feedbacks = new List<Feedback_Img>();
            for (int i = 0; i < imagesArr.Count(); i++)
            {
                if (!String.IsNullOrEmpty(imagesArr[i]))
                {
                    Feedback_Img feedback_Img = new Feedback_Img();
                    string baseStr = imagesArr[i];

                    feedback_Img.Id = Guid.NewGuid();
                    feedback_Img.Feedback_Id = feedbackId;

                    string[] sources = baseStr.Split(',');
                    //base64转字节
                    byte[] postData = Convert.FromBase64String(sources[1]);
                    //字节转image
                    Image img = GetImageExtHelper.byte2img(postData);
                    //获取图片扩展名
                    string ext = GetImageExtHelper.GetImageExt(img);
                    string commentImg = _setting.HelperCenterUpload + $"{feedbackId}/{feedback_Img.Id}.{ext}";
                    //上传图片
                    var rez = UploadImager.UploadImagerByFeedback(commentImg, postData);
                    if (rez.status != -1)
                    {
                        feedback_Img.url = rez.url;
                        feedbacks.Add(feedback_Img);
                    }
                }
            }
            return feedbacks;
        }

        [Description("学校纠错")]
        [Authorize]
        public IActionResult SchCorrection(Guid Eid)
        {
            SchoolFeedbackVo feedback = new SchoolFeedbackVo();

            var gradeType = _schoolService.GetGradeTypeList();
            if (Eid != default)
            {
                var school = _schoolService.SchoolFeedback(Eid);
                feedback = _mapper.Map<SchoolFeedback, SchoolFeedbackVo>(school);

                feedback.GradeName = gradeType.Where(x => x.GradeId == school.Grade).FirstOrDefault().Types.Where(x => x.TypeValue == school.SchFType).FirstOrDefault().TypeName;
            }

            ViewBag.feedback = feedback;



            string grade = "";

            if (gradeType.Any())
            {
                grade = JsonConvert.SerializeObject(gradeType.Select(x => new GradeType() { key = x.GradeDesc, values = x.Types?.Select(v => v.TypeName).ToList() }).ToList());
            }

            ViewBag.GradeType = grade;
            ViewBag.Eid = Eid;

            return View();
        }

        [HttpPost]
        public ResponseResult FeedbackCorrected(FeedbackCorrectedVo feedbackCorrected)
        {
            try
            {
                var user = User.Identity.GetUserInfo();
                Feedback feedback = new Feedback()
                {
                    Id = Guid.NewGuid(),
                    UserID = user.UserId,
                    time = DateTime.Now,
                    text = feedbackCorrected.Remark,
                    type = 6
                };

                bool insert = _feedbackService.AddFeedback(feedback);
                if (insert)
                {
                    if (feedbackCorrected.ImagesArr != null)
                    {
                        if (feedbackCorrected.ImagesArr.Any())
                        {
                            //带有图片
                            var imgs = HelperCenterUploadImage(feedbackCorrected.ImagesArr, feedback.Id);
                            _feedbackService.AddFeedbackImg(imgs);
                        }
                    }

                    var feedbackName = _schoolService.SchoolFeedback(feedbackCorrected.Eid);
                    string Before_Corrected = "", After_Corrected = "";
                    if (feedbackCorrected.Type == CorrectedType.Name)
                    {
                        Before_Corrected = feedbackName.Name;
                    }
                    else if (feedbackCorrected.Type == CorrectedType.Location)
                    {
                        Before_Corrected = feedbackName.Address;
                    }
                    else if (feedbackCorrected.Type == CorrectedType.SchType)
                    {
                        Before_Corrected = SchFTypeUtil.GetItem(feedbackName.SchFType)?.desc;
                    }

                    After_Corrected = feedbackCorrected.Content;

                    Feedback_corrected corrected = new Feedback_corrected()
                    {
                        Id = Guid.NewGuid(),
                        Feedback_Id = feedback.Id,
                        Type = feedbackCorrected.Type,
                        School_Id = feedbackCorrected.Eid.ToString(),
                        School_name = feedbackName.Name,
                        Before_Corrected = Before_Corrected,
                        After_Corrected = After_Corrected
                    };
                    _feedbackService.AddfeedbackCorrect(corrected);
                }
                return ResponseResult.Success();
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }
        [HttpGet]
        public IActionResult Contact()
        {
            var redirectUrl = $"/";
            Response.Redirect(redirectUrl);
            Response.StatusCode = 302;

            return Json(new { });
        }
    }
}