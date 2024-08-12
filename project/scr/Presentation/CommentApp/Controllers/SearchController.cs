using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using PMS.Infrastructure.Application.IService;
using PMS.Infrastructure.Application.ModelDto;
using PMS.OperationPlateform.Application.IServices;
using PMS.School.Application.IServices;
using PMS.School.Application.ModelDto;
using PMS.Search.Application.IServices;
using PMS.Search.Application.ModelDto.Query;
using PMS.UserManage.Application.IServices;
using Sxb.Web.Models;
using Sxb.Web.Models.Comment;
using Sxb.Web.Models.Question;
using Sxb.Web.Models.User;
using Sxb.Web.RequestModel.Comment;
using Sxb.Web.RequestModel.School;
using Sxb.Web.Response;
using Sxb.Web.Utils;
using Sxb.Web.ViewModels.Article;
using Sxb.Web.ViewModels.School;
using Sxb.Web.ViewModels.Search;
using ProductManagement.Framework.Foundation;
using PMS.OperationPlateform.Domain.Enums;
using PMS.Search.Domain.Entities;
using Sxb.Web.RequestModel.Question;
using ProductManagement.Framework.Cache.Redis;
using Sxb.Web.Common;
using System.Threading;
using Sxb.Web.RequestModel.Map;
using Microsoft.AspNetCore.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PMS.UserManage.Domain.Common;
using ProductManagement.API.Http.Interface;
using System.ComponentModel;
using PMS.School.Domain.Dtos;
using Sxb.Web.ViewModels.Live;
using PMS.Search.Domain.Common;
using Sxb.Web.ViewModels.TopicCircle;
using PMS.TopicCircle.Application.Services;
using static PMS.Search.Domain.Common.GlobalEnum;
using NSwag.Annotations;
using MongoDB.Driver.Linq;
using Microsoft.AspNetCore.Builder;
using Namotion.Reflection;
using PMS.UserManage.Application.ModelDto;
using PMS.Search.Domain.QueryModel;

namespace Sxb.Web.Controllers
{

    [OpenApiIgnore]
    public class SearchController : Controller
    {
        private readonly ISchoolInfoService _schoolInfoService;
        private readonly ImageSetting _imageSetting;
        private readonly ISearchService _dataSearch;
        private readonly IImportService _dataImport;
        private readonly ISchoolCommentService _commentService;
        private readonly IQuestionInfoService _questionService;
        private readonly ICityInfoService _cityService;
        private readonly ISchoolService _schoolService;
        private readonly IUserService _userService;
        private readonly IArticleService _articleService;
        private readonly ISchoolRankService _schoolRankService;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly IMapper _mapper;
        private readonly IOperationClient _operationClient;
        private readonly IUserRecommendClient _userRecommendClient;
        private readonly IArticleCoverService _articleCoverService;
        private readonly IArticleCommentService _articleCommentService;
        private readonly ILiveServiceClient _liveServiceClient;
        private readonly ICircleFollowerService _circleFollowerService;
        private readonly ICircleService _circleService;
        readonly ISchService _SchService;

        public static string HOTWORD_KEY = "SearchHotword:{0}";

        public ChannelIndex GetChannel(int channel)
        {
            ChannelIndex channelIndex;
            switch (channel)
            {
                case 0:
                    channelIndex = ChannelIndex.School;
                    break;
                case 1:
                    channelIndex = ChannelIndex.Comment;
                    break;
                case 2:
                    channelIndex = ChannelIndex.Question;
                    break;
                case 3:
                    channelIndex = ChannelIndex.SchoolRank;
                    break;
                case 4:
                    channelIndex = ChannelIndex.Article;
                    break;
                case 5:
                    channelIndex = ChannelIndex.Live;
                    break;
                case 7:
                    channelIndex = ChannelIndex.Circle;
                    break;
                default:
                    channelIndex = ChannelIndex.School;
                    break;
            }
            return channelIndex;
        }

        public string GetHotwordKey(ChannelIndex channelIndex)
        {
            return string.Format(HOTWORD_KEY, channelIndex);
        }

        public SearchController(ISearchService dataSearch, IImportService dataImport, IMapper mapper,
            ISchoolCommentService commentService, IQuestionInfoService questionService,
            ISchoolInfoService schoolInfoService, IOptions<ImageSetting> set, IEasyRedisClient easyRedisClient,
            ISchoolService schoolService, ICityInfoService cityService, IUserService userService,
            IArticleService articleService, ISchoolRankService schoolRankService, IOperationClient operationClient,
            IUserRecommendClient userRecommendClient, ILiveServiceClient liveServiceClient,
            IArticleCoverService articleCoverService, IArticleCommentService articleCommentService,
            ICircleFollowerService circleFollowerService, ICircleService circleService, ISchService schService)
        {
            _SchService = schService;
            _dataSearch = dataSearch;
            _dataImport = dataImport;
            _schoolService = schoolService;
            _cityService = cityService;
            _commentService = commentService;
            _questionService = questionService;
            _mapper = mapper;
            _schoolInfoService = schoolInfoService;
            _imageSetting = set.Value;
            _userService = userService;
            _articleService = articleService;
            _schoolRankService = schoolRankService;
            _easyRedisClient = easyRedisClient;
            _articleCoverService = articleCoverService;
            _articleCommentService = articleCommentService;
            _operationClient = operationClient;
            _userRecommendClient = userRecommendClient;
            _liveServiceClient = liveServiceClient;
            _circleFollowerService = circleFollowerService;
            _circleService = circleService;
        }

        /// <summary>
        /// 联想词搜索
        /// </summary>
        /// <param name="keyWords"></param>
        /// <param name="channel">0学校 1点评 2问答 3排行榜 4.攻略 5.直播 -1.所有</param>
        /// <returns></returns>
        [Description("联想词搜索")]
        public ResponseResult GetSuggestList(string keyWords, int channel)
        {
            _ = SetHotword(keyWords, GetChannel(channel));


            int cityCode = Request.GetLocalCity();
            //登录用户获取搜索历史记录
            if (User.Identity.IsAuthenticated)
            {
                var loginUser = User.Identity.GetUserInfo();
            }

            var list = _dataSearch.ListSuggest(keyWords, channel, cityCode);


            var Group = new List<int> { 1, 7, 2, 3, 4, 5, 6 };
            var result = Group.Select(s => new SuggestGroupViewModel { GroupType = s }).ToList();


            for (int i = 0; i < result.Count(); i++)
            {
                var suggests = list.Suggests.FirstOrDefault(q => q.Type == result[i].GroupType)?.Items;

                if (suggests != null)
                {
                    List<Guid> ids = suggests.GroupBy(q => q.SourceId).Select(p => p.Key).ToList();
                    //反查数据库获取学校点评数、点评回复数、问题回答数、文章浏览数
                    List<SchoolTotalDto> total = null;
                    switch (result[i].GroupType)
                    {
                        case 1:
                            total = _commentService.GetCommentCountBySchool(ids);

                            if (total == null) total = new List<SchoolTotalDto>();
                            var schoolNos = _SchService.GetSchoolextNo(ids.Distinct().ToArray());
                            if (schoolNos?.Any() == true)
                            {
                                foreach (var no in schoolNos)
                                {
                                    var find = total.FirstOrDefault(p => p.Id == no.Item1);
                                    if (find == null)
                                    {
                                        find = new SchoolTotalDto();
                                        total.Add(find);
                                    }
                                    find.Id = no.Item1;
                                    find.No = UrlShortIdUtil.Long2Base32(no.Item2).ToLower();
                                }
                            }
                            break;
                        case 2:
                            total = _commentService.GetReplyCount(ids);
                            if (total?.Any() == true)
                            {
                                var commentNos = _commentService.GetCommentsByIds(ids)?.ToDictionary(k => k.Id, v => v.No);
                                if (commentNos?.Any() == true)
                                {
                                    foreach (var no in commentNos)
                                    {
                                        var find = total.FirstOrDefault(p => p.Id == no.Key);
                                        if (find == null)
                                        {
                                            find = new SchoolTotalDto();
                                            total.Add(find);
                                        }
                                        find.Id = no.Key;
                                        find.No = UrlShortIdUtil.Long2Base32(no.Value).ToLower();
                                    }
                                }
                            }
                            break;
                        case 3:
                            total = _questionService.GetAnswerCount(ids);
                            if (total?.Any() == true)
                            {
                                var questionNos = _questionService.GetQuestionInfoByIds(ids)?.ToDictionary(k => k.Id, v => v.No);
                                if (questionNos?.Any() == true)
                                {
                                    foreach (var no in questionNos)
                                    {
                                        var find = total.FirstOrDefault(p => p.Id == no.Key);
                                        if (find == null)
                                        {
                                            find = new SchoolTotalDto();
                                            total.Add(find);
                                        }
                                        find.Id = no.Key;
                                        find.No = UrlShortIdUtil.Long2Base32(no.Value).ToLower();
                                    }
                                }
                            }
                            break;
                        case 5:
                            var articles = _articleService.GetByIds(ids.ToArray());
                            total = articles.Select(q => new SchoolTotalDto { Id = q.id, No = UrlShortIdUtil.Long2Base32(q.No), Total = q.VirualViewCount }).ToList();
                            break;
                        //话题圈 粉丝数
                        case 7:
                            var circleFollowers = _circleFollowerService.GetFollowerCount(ids.ToArray());
                            total = circleFollowers.Select(s => new SchoolTotalDto { Id = s.CircleId, Total = s.FollowerCount }).ToList();
                            break;
                    }

                    result[i].Items = suggests.Select(q => new SuggestViewModel
                    {
                        Channel = result[i].GroupType,
                        Type = 1,
                        KeyWord = q.Name,
                        Value = total == null ? new { Id = q.SourceId, No = "" } : new { Id = q.SourceId, No = total.FirstOrDefault(p => p.Id == q.SourceId)?.No ?? "" },
                        Show = total == null ? null : new { count = total.FirstOrDefault(p => p.Id == q.SourceId)?.Total ?? 0 }
                    }).ToList();
                }
                else
                {
                    result[i].Items = new List<SuggestViewModel>();
                }

            }
            return ResponseResult.Success(new
            {
                keyWords,
                total = channel == -1 ? list.Total : list.Suggests.FirstOrDefault(q => q.Type == (channel + 1))?.Total ?? 0,
                list = result
            });
        }

        /// <summary>
        /// 历史搜索列表
        /// </summary>
        /// <returns></returns>
        [Description("历史搜索列表")]
        public async Task<ResponseResult> GetHistoryList()
        {
            var result = new List<SearchWordViewModel>();

            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                string Key = $"SearchHistory:{user.UserId}";

                var list = await _easyRedisClient.SortedSetRangeByRankAsync<string>(Key, 0, 10, StackExchange.Redis.Order.Descending);
                result = list.Select(q => new SearchWordViewModel { Name = q }).ToList();
            }
            return ResponseResult.Success(result);
        }

        /// <summary>
        /// 清除历史搜索
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Description("清除历史搜索")]
        public async Task<ResponseResult> RemoveHistoryList()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                string Key = $"SearchHistory:{user.UserId}";
                await _easyRedisClient.RemoveAsync(Key);
            }
            return ResponseResult.Success();
        }


        /// <summary>
        /// 热门搜索词列表
        /// </summary>
        /// <returns></returns>
        [Description("热门搜索词列表")]
        public async Task<ResponseResult> GetHotwordList(int channel = 0)
        {
            string key = GetHotwordKey(GetChannel(channel));
            var list = await _easyRedisClient.SortedSetRangeByRankAsync<string>(key, 0, 5, StackExchange.Redis.Order.Descending);
            var result = list.Select(q => new SearchWordViewModel { Name = q }).ToList();

            return ResponseResult.Success(result);
        }

        [HttpGet]
        [Description("地图搜校-获取选项列表")]
        public async Task GetOptionList(string callBack)
        {
            int cityCode = Request.GetLocalCity();

            List<MetroDto> metros = new List<MetroDto>();
            List<GradeTypeDto> types = new List<GradeTypeDto>();
            List<PMS.School.Application.ModelDto.TagDto> tags = new List<PMS.School.Application.ModelDto.TagDto>();
            //统计国际学校的标签
            var interAuthTags = new Dictionary<Guid, string>();
            var interCharacTags = new Dictionary<Guid, string>();

            //统计非国际学校的标签
            var notInterAuthTags = new Dictionary<Guid, string>();
            var notInterCharacTags = new Dictionary<Guid, string>();


            Task t2 = Task.Run(async () => metros = await _cityService.GetMetroList(cityCode));
            Task t3 = Task.Run(() => tags = _schoolService.GetTagList());
            Task t4 = Task.Run(() => types = _schoolService.GetGradeTypeList());

            await Task.WhenAll(t2, t3, t4);

            //统计国际学校的标签
            interAuthTags = await _schoolService.GetAuthTagsBySchoolType(true);
            interCharacTags = await _schoolService.GetCharacTagsBySchoolType(true);

            //统计非国际学校的标签
            notInterAuthTags = await _schoolService.GetAuthTagsBySchoolType(false);
            notInterCharacTags = await _schoolService.GetAuthTagsBySchoolType(false);

            var dzAuthTags = interAuthTags.Keys.Intersect(notInterAuthTags.Keys);
            var dzCharacTags = interCharacTags.Keys.Intersect(notInterCharacTags.Keys);

            var Authentications = tags.Where(q => q.Type == 2).Select(q => new SchoolExtTagViewModel
            {
                Id = q.Id,
                Name = q.Name,
                International = dzAuthTags.Contains(q.Id) ? 0 :
                                interAuthTags.Keys.Contains(q.Id) ? 1 :
                                notInterAuthTags.Keys.Contains(q.Id) ? 2 : 0
            }).ToList();
            var Abroads = tags.Where(q => q.Type == 3).Select(q => new SchoolExtTagViewModel
            {
                Id = q.Id,
                Name = q.Name
            }).ToList();
            var Characteristics = tags.Where(q => q.Type == 8).Select(q => new SchoolExtTagViewModel
            {
                Id = q.Id,
                Name = q.Name,
                International = dzCharacTags.Contains(q.Id) ? 0 :
                                interCharacTags.Keys.Contains(q.Id) ? 1 :
                                notInterCharacTags.Keys.Contains(q.Id) ? 2 : 0
            }).ToList();
            var Courses = tags.Where(q => q.Type == 6).Select(q => new SchoolExtTagViewModel
            {
                Id = q.Id,
                Name = q.Name
            }).ToList();

            var result = ResponseResult.Success(new
            {
                metros = metros.Select(q => new MetroViewModel
                {
                    Id = q.MetroId,
                    Value = q.MetroName,
                    Child = q.MetroStations.Select(p => new MetroViewModel.StationViewModel
                    {
                        Id = p.Id,
                        Value = p.Name,
                        Lat = p.Lat,
                        Lng = p.Lng
                    }).ToList()
                }),
                types = types.Select(q => new GradeTypeViewModel
                {
                    GradeId = q.GradeId,
                    GradeDesc = q.GradeDesc,
                    Items = q.Types.Select(p => new TypeItemViewModel
                    {
                        International = p.International,
                        Value = p.TypeValue,
                        Name = p.TypeName
                    }).ToList()
                }),
                Authentications,
                Abroads,
                Characteristics,
                Courses
            });

            string response = JsonConvert.SerializeObject(result, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            string call = callBack + "(" + response + ")";
            Response.ContentType = "text/plain; charset=utf-8";
            await Response.WriteAsync(call, Encoding.UTF8);
        }

        public ActionResult Propose(int channel = 0)
        {
            ViewBag.Channel = channel;
            return View();
        }

        /// <summary>
        /// 搜索筛选列表页
        /// </summary>
        [Description("搜索筛选列表页")]
        public async Task<ActionResult> Index(int channel = 0)
        {
            int cityCode = Request.GetLocalCity();
            ViewBag.CityCode = cityCode;

            //检测当前用户身份 （true：校方用户 |false：普通用户）
            ViewBag.isSchoolRole = false;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                //检测当前用户身份 （true：校方用户 |false：普通用户）
                ViewBag.isSchoolRole = user.Role.Contains((int)UserRole.School);
            }

            List<AreaDto> areas = new List<AreaDto>();
            List<MetroDto> metros = new List<MetroDto>();
            List<PMS.School.Application.ModelDto.TagDto> tags = new List<PMS.School.Application.ModelDto.TagDto>();
            List<GradeTypeDto> types = new List<GradeTypeDto>();

            //统计国际学校的标签
            var interAuthTags = new Dictionary<Guid, string>();
            var interCharacTags = new Dictionary<Guid, string>();

            //统计非国际学校的标签
            var notInterAuthTags = new Dictionary<Guid, string>();
            var notInterCharacTags = new Dictionary<Guid, string>();


            Task t1 = Task.Run(async () => areas = await _cityService.GetAreaCode(cityCode));
            Task t2 = Task.Run(async () => metros = await _cityService.GetMetroList(cityCode));
            Task t3 = Task.Run(() => tags = _schoolService.GetTagList());
            Task t4 = Task.Run(() => types = _schoolService.GetGradeTypeList());

            //统计国际学校的标签
            interAuthTags = await _schoolService.GetAuthTagsBySchoolType(true);
            interCharacTags = await _schoolService.GetCharacTagsBySchoolType(true);

            //统计非国际学校的标签
            notInterAuthTags = await _schoolService.GetAuthTagsBySchoolType(false);
            notInterCharacTags = await _schoolService.GetAuthTagsBySchoolType(false);

            await Task.WhenAll(t1, t2, t3, t4);

            var dzAuthTags = interAuthTags.Keys.Intersect(notInterAuthTags.Keys);
            var dzCharacTags = interCharacTags.Keys.Intersect(notInterCharacTags.Keys);

            ViewBag.Areas = areas;
            ViewBag.Metros = metros.OrderBy(q => q.MetroName);
            ViewBag.Types = types;

            ViewBag.Authentications = tags.Where(q => q.Type == 2).Select(q => new SchoolExtTagViewModel
            {
                Id = q.Id,
                Name = q.Name,
                International = dzAuthTags.Contains(q.Id) ? 0 :
                                interAuthTags.Keys.Contains(q.Id) ? 1 :
                                notInterAuthTags.Keys.Contains(q.Id) ? 2 : 0
            }).ToList();
            ViewBag.Abroads = tags.Where(q => q.Type == 3).ToList();
            ViewBag.Characteristics = tags.Where(q => q.Type == 8).Select(q => new SchoolExtTagViewModel
            {
                Id = q.Id,
                Name = q.Name,
                International = dzCharacTags.Contains(q.Id) ? 0 :
                                interCharacTags.Keys.Contains(q.Id) ? 1 :
                                notInterCharacTags.Keys.Contains(q.Id) ? 2 : 0
            }).ToList();
            ViewBag.Courses = tags.Where(q => q.Type == 6).ToList();


            ViewBag.SelectedChannel = channel;



            return View();
        }

        [Description("搜索学校列表")]
        public async Task<ActionResult> SearchSchoolPage(SchoolExtFilterData query)
        {
            await SetHotword(query.KeyWords, ChannelIndex.School);

            int cityCode = Request.GetLocalCity();
            ViewBag.CityCode = cityCode;

            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                string Key = $"SearchHistory:{user.UserId}";
                await _easyRedisClient.SortedSetAddAsync(Key, query.KeyWords, DateTime.Now.D2I(), StackExchange.Redis.CommandFlags.FireAndForget);
            }

            var lat = Request.GetLatitude();
            var lng = Request.GetLongitude();

            List<MetroQuery> metroIds = new List<MetroQuery>();

            if (query.MetroLineIds.Any())
            {
                if (query.MetroStationIds.Any())
                {
                    var metros = await _cityService.GetMetroList(query.MetroLineIds);

                    metroIds = metros.Select(q => new MetroQuery
                    {
                        LineId = q.MetroId,
                        StationIds = q.MetroStations.Where(p => query.MetroStationIds.Contains(p.Id))
                                      .Select(p => p.Id).ToList()
                    }).ToList();
                }
                else
                {
                    metroIds = query.MetroLineIds.Select(q => new MetroQuery
                    {
                        LineId = q,
                        StationIds = new List<int>()
                    }).ToList();
                }
            }

            query.Type = query.Type.Where(q => q != null).ToList();

            if (query.Type.Any(q => !q.Contains("lx")))
            {
                query.Grade = query.Type.Where(q => !q.Contains("lx")).Select(q => Convert.ToInt32(q)).ToList();
            }
            query.Type = query.Type.Where(q => q.Contains("lx")).ToList();

            List<int> city = new List<int>();
            city = query.AreaCodes.Select(q => q / 100 * 100).GroupBy(q => q).Select(q => q.Key).ToList();

            var list = _dataSearch.SearchSchool(new SearchSchoolQuery
            {
                Keyword = query.KeyWords,
                Latitude = double.Parse(lat),
                Longitude = double.Parse(lng),
                CurrentCity = cityCode,
                CityCode = city,
                AreaCodes = query.AreaCodes,
                MetroIds = metroIds,
                Distance = query.Distance ?? 0,
                Type = query.Type,
                Grade = query.Grade,
                MinCost = query.MinCost,
                MaxCost = query.MaxCost,

                MinStudentCount = query.MinStudent,
                MaxStudentCount = query.MaxStudent,
                MinTeacherCount = query.MinTeacher,
                MaxTeacherCount = query.MaxTeacher,

                //分数
                MinComposite = query.MinCScore,
                MaxComposite = query.MaxCScore,
                MinTeach = query.MinTScore,
                MaxTeach = query.MaxTScore,
                MinHard = query.MinHScore,
                MaxHard = query.MaxHScore,
                MinCourse = query.MinCuScore,
                MaxCourse = query.MaxCuScore,
                MinLearn = query.MinLScore,
                MaxLearn = query.MaxLScore,
                MinCostScore = query.MinCtScore,
                MaxCostScore = query.MaxCtScore,
                MinTotal = query.MinTotal,
                MaxTotal = query.MaxTotal,

                Lodging = query.Lodging,
                Canteen = query.Canteen,
                AuthIds = query.AuthIds,
                CharacIds = query.CharacIds,
                AbroadIds = query.AbroadIds,
                CourseIds = query.CourseIds,
                Orderby = query.Orderby,
                PageNo = query.PageNo,
                PageSize = query.PageSize
            });

            var ids = list.Schools.Select(q => q.Id).ToList();

            var result = ids.Count > 0 ? await _schoolService.ListExtSchoolByBranchIds(ids) : new List<SchoolExtFilterDto>();

            ViewBag.Data = list.Schools.Where(q => result.Any(p => p.ExtId == q.Id)).Select(q =>
             {
                 var r = result.FirstOrDefault(p => p.ExtId == q.Id);
                 return r == null ? new SchoolExtListItemViewModel() : new SchoolExtListItemViewModel
                 {
                     SchoolId = r.Sid,
                     BranchId = r.ExtId,
                     SchoolName = q.Name,
                     CityName = r.City,
                     AreaName = r.Area,
                     Distance = q.Distance,
                     LodgingType = (int)r.LodgingType,
                     LodgingReason = r.LodgingType.Description(),
                     Type = r.Type,
                     International = r.International,
                     Cost = r.Tuition,
                     Tags = r.Tags,
                     Score = r.Score,
                     CommentTotal = r.CommentCount,
                     SchoolNo = r.SchoolNo
                 };
             });
            return View();
        }
        [Description("搜索点评列表")]
        public async Task<ActionResult> SearchCommentPage(CommentData query, int PageNo = 1, int PageSize = 10)
        {
            await SetHotword(query.KeyWords, ChannelIndex.Comment);
            int cityCode = Request.GetLocalCity();
            ViewBag.CityCode = cityCode;
            Guid userId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                userId = user.UserId;
                string Key = $"SearchHistory:{user.UserId}";
                await _easyRedisClient.SortedSetAddAsync(Key, query.KeyWords, DateTime.Now.D2I(), StackExchange.Redis.CommandFlags.FireAndForget);
            }

            var result = _dataSearch.SearchComment(new SearchCommentQuery
            {
                Keyword = query.KeyWords,
                CityIds = new List<int> { cityCode },
                AreaIds = query.AreaCodes,
                GradeIds = query.GradeIds,
                TypeIds = query.TypeIds,
                PageNo = PageNo,
                PageSize = PageSize,
                Orderby = query.Orderby,
                Lodging = query.Lodging
            });

            var comments = _commentService.QueryCommentByIds(result.Comments.Select(q => q.Id).ToList(), userId);

            if (comments == null)
            {
                ViewBag.Rows = new List<CommentList>();
                return View();
            }

            var UserIds = new List<Guid>();
            comments.Where(q => q.CommentReplies != null).ToList().ForEach(p => { UserIds.AddRange(p.CommentReplies.Where(q => q.ParentUserId != null).Select(q => (Guid)q.ParentUserId)); });
            comments.Where(q => q.CommentReplies != null).ToList().ForEach(p => { UserIds.AddRange(p.CommentReplies.Select(q => q.ReplayUserId)); });
            UserIds.AddRange(comments.GroupBy(q => q.UserId).Select(p => p.Key).ToList());
            var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());

            var commentList = _mapper.Map<List<SchoolCommentDto>, List<CommentList>>(comments);

            if (commentList.Any())
            {
                List<Guid> Ids = commentList.GroupBy(q => q.SchoolSectionId).Select(x => x.Key)?.ToList();
                var SchoolInfos = _schoolInfoService.GetSchoolSectionByIds(Ids);

                foreach (var x in commentList)
                {
                    if (x.Images.Any())
                    {
                        var images = x.Images;
                        for (int i = 0; i < images.Count; i++)
                        {
                            images[i] = _imageSetting.QueryImager + images[i];
                        }
                        x.Images = images;
                    }
                    var user = Users.FirstOrDefault(p => p.Id == x.UserId);
                    x.UserInfo = _mapper.Map<UserInfoDto, Models.User.UserInfoVo>(user);
                    var schoolInfo = SchoolInfos.Where(s => s.SchoolSectionId == x.SchoolSectionId).FirstOrDefault();
                    x.School = _mapper.Map<SchoolInfoDto, CommentList.SchoolInfoVo>(schoolInfo);
                }
            }

            ViewBag.Rows = result.Comments.Where(q => commentList.Any(p => p.Id == q.Id))
                .Select(q =>
                {
                    var r = commentList.FirstOrDefault(p => p.Id == q.Id);
                    return r;
                }).ToList();
            return View();
        }
        [Description("搜索问答列表")]
        public async Task<ActionResult> SearchQuestionPage(QuestionData query, int PageNo = 1, int PageSize = 10)
        {
            await SetHotword(query.KeyWords, ChannelIndex.Question);
            int cityCode = Request.GetLocalCity();
            ViewBag.CityCode = cityCode;
            Guid userId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                userId = user.UserId;
                string Key = $"SearchHistory:{user.UserId}";
                await _easyRedisClient.SortedSetAddAsync(Key, query.KeyWords, DateTime.Now.D2I(), StackExchange.Redis.CommandFlags.FireAndForget);
            }
            var result = _dataSearch.SearchQuestion(new SearchQuestionQuery
            {
                Keyword = query.KeyWords,
                CityIds = new List<int> { cityCode },
                AreaIds = query.AreaCodes,
                GradeIds = query.GradeIds,
                TypeIds = query.TypeIds,
                PageNo = PageNo,
                PageSize = PageSize,
                Orderby = query.Orderby,
                Lodging = query.Lodging
            });

            var qeustions = _questionService.GetQuestionByIds(result.Questions.Select(q => q.Id).ToList(), userId);

            var UserIds = new List<Guid>();
            qeustions.Where(q => q.answer != null).ToList().ForEach(p => { UserIds.AddRange(p.answer.Select(q => q.UserId)); });
            UserIds.AddRange(qeustions.GroupBy(q => q.UserId).Select(p => p.Key).ToList());
            var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());

            var schoolIds = qeustions.GroupBy(q => q.SchoolId).Select(q => q.Key).ToList();
            var extIds = qeustions.GroupBy(q => q.SchoolSectionId).Select(q => q.Key).ToList();



            var questionLists = _mapper.Map<List<QuestionDto>, List<QuestionVo>>(qeustions);
            if (questionLists.Any())
            {
                string schoolKey = $"SchoolInfos:{ DesTool.Md5(string.Join("_", extIds))}";
                string schoolQaTotalKey = $"SchoolQaTotalInfos:{ DesTool.Md5(string.Join("_", schoolIds))}";

                var schoolInfos = await _easyRedisClient.GetOrAddAsync(schoolKey, () =>
                {
                    return _schoolInfoService.GetSchoolSectionQaByIds(extIds);
                }, new TimeSpan(0, 5, 0));

                var schoolQuestionTotal = await _easyRedisClient.GetOrAddAsync(schoolQaTotalKey, () =>
                {
                    return _questionService.SchoolTotalQuestion(schoolIds);
                }, new TimeSpan(0, 5, 0));


                questionLists.ForEach(x =>
                {
                    var schoolInfo = schoolInfos.FirstOrDefault(q => q.SchoolSectionId == x.SchoolSectionId);
                    x.School = _mapper.Map<SchoolInfoQaDto, QuestionVo.SchoolInfo>(schoolInfo);
                    if (x.School != null)
                    {
                        x.School.SchoolQuestionTotal = schoolQuestionTotal.FirstOrDefault(q => q.Id == x.SchoolId)?.Total ?? 0;
                    }

                    x.UserInfo = _mapper.Map<UserInfoVo>(Users.FirstOrDefault(q => q.Id == x.UserId));

                    if (x.Images.Any())
                    {
                        var images = x.Images;
                        for (int i = 0; i < images.Count; i++)
                        {
                            images[i] = _imageSetting.QueryImager + images[i];
                        }
                        x.Images = images;
                    }
                    if (x.Answer != null)
                    {
                        x.Answer.ForEach(answer =>
                        {
                            answer.UserInfo = _mapper.Map<UserInfoVo>(Users.FirstOrDefault(q => q.Id == answer.UserId)).ToAnonyUserName(answer.IsAnony);
                        });
                    }
                });
            }

            ViewBag.Rows = result.Questions.Where(q => questionLists.Any(p => p.Id == q.Id))
                .Select(q =>
                {
                    var r = questionLists.FirstOrDefault(p => p.Id == q.Id);
                    return r;
                }).ToList();
            return View();
        }
        [Description("搜索排行榜列表")]
        public async Task<ActionResult> SearchSchoolRankPage(string keyWords, int PageNo = 1, int PageSize = 10)
        {
            await SetHotword(keyWords, ChannelIndex.SchoolRank);
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                string Key = $"SearchHistory:{user.UserId}";
                await _easyRedisClient.SortedSetAddAsync(Key, keyWords, DateTime.Now.D2I(), StackExchange.Redis.CommandFlags.FireAndForget);
            }

            var result = _dataSearch.SearchSchoolRank(new SearchBaseQueryModel(keyWords, PageNo, PageSize));

            var data = result.Ranks.Select(q => new SchoolRankItemViewModel
            {
                Id = q.Id,
                RankName = q.Title,
                Schools = string.Join("、", q.Schools.Take(3))
            });
            ViewBag.Data = data;
            return View();
            //return ResponseResult.Success();
        }
        [Description("搜索文章列表")]
        public async Task<ActionResult> SearchArticlePage(string keyWords, int PageNo = 1, int PageSize = 10)
        {
            await SetHotword(keyWords, ChannelIndex.Article);
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                string Key = $"SearchHistory:{user.UserId}";
                await _easyRedisClient.SortedSetAddAsync(Key, keyWords, DateTime.Now.D2I(), StackExchange.Redis.CommandFlags.FireAndForget);
            }
            var result = _dataSearch.SearchArticle(keyWords, null, PageNo, PageSize);

            var articleIds = result.Articles.Select(q => q.Id).ToArray();
            var articleList = _articleService.GetByIds(articleIds).ToList();

            if (articleList != null && articleList.Count > 0)
            {
                //查询出目标背景图片
                var effactiveIds = articleList.Select(a => a.id).ToArray();
                var covers = _articleCoverService.GetCoversByIds(effactiveIds);
                var comments = _articleCommentService.Statistics_CommentsCount(articleIds);

                var _articles = new List<PMS.OperationPlateform.Domain.Entitys.article>();
                if (articleIds != null)
                {
                    foreach (var id in articleIds)
                    {
                        var article = articleList.Find(a => a.id == id);
                        if (article != null)
                        {
                            article.Covers = covers.Where(c => c.articleID == article.id).ToList();
                            article.CommentCount = comments.Where(c => c.Id == article.id).FirstOrDefault()?.Count ?? 0;
                            _articles.Add(article);
                        }
                    }
                }
                var ogaData = articleList.Select(a => new ArticleListItemViewModel()
                {
                    Id = a.id,
                    Time = a.time.GetValueOrDefault().ConciseTime(),
                    Title = a.title,
                    ViweCount = a.VirualViewCount,
                    Covers = a.Covers.Select(c => !string.IsNullOrWhiteSpace(c.ImgUrl) ? c.ImgUrl : $"https://cos.sxkid.com/images/article/{c.articleID}/{c.photoID}.{((FileExtension)c.ext).ToString()}").ToList(),
                    CommentCount = a.CommentCount,
                    Layout = a.layout,
                    Digest = a.overview,
                    No = UrlShortIdUtil.Long2Base32(a.No)
                }).ToList();
                ViewBag.OGAData = ogaData;
            }
            else
            {
                ViewBag.OGAData = new List<ArticleListItemViewModel>();
            }
            return View();
        }

        [Description("搜索直播列表")]
        public async Task<ActionResult> SearchLivePage(string keyWords, int PageNo = 1, int PageSize = 10)
        {
            await SetHotword(keyWords, ChannelIndex.Live);
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                string Key = $"SearchHistory:{user.UserId}";
                await _easyRedisClient.SortedSetAddAsync(Key, keyWords, DateTime.Now.D2I(), StackExchange.Redis.CommandFlags.FireAndForget);
            }

            var result = _dataSearch.SearchLive(new SearchLiveQueryModel(keyWords, PageNo, PageSize));
            var liveIds = result.Lives.Select(q => q.Id).ToList();

            if (liveIds.Any())
            {
                var lives = await _liveServiceClient.QueryLectures(liveIds, HttpContext.Request.GetAllCookies());

                if (lives.Items != null && lives.Items.Count > 0)
                {
                    var data = result.Lives.Where(q => lives.Items.Any(p => p.Id == q.Id))
                    .Select(q =>
                    {
                        var r = lives.Items.FirstOrDefault(p => p.Id == q.Id);
                        return new LiveItemViewModel
                        {
                            Id = r.Id,
                            Title = r.Subject,
                            StartTime = (long)r.Time,
                            UserCount = r.Onlinecount,
                            BookCount = r.collectioncount,
                            Status = (LectureStatus)r.Status,
                            UserHeadImg = r.lector.Headimgurl,
                            UserId = r.lector.UserId ?? Guid.Empty,
                            UserName = r.lector.Name,
                            CoverHome = r.HomeCover,
                            IsCollection = r.lector.IsFocus ?? false
                        };
                    }).ToList();

                    ViewBag.Data = data;
                }
                else
                {
                    ViewBag.Data = new List<LiveItemViewModel>();
                }
            }
            else
            {
                ViewBag.Data = new List<LiveItemViewModel>();
            }
            return View();
            //return ResponseResult.Success();
        }

        public async Task SetHotword(string keyWords, ChannelIndex channelIndex)
        {
            if (string.IsNullOrWhiteSpace(keyWords))
                return;

            string key = GetHotwordKey(channelIndex);
            keyWords = keyWords.Trim();
            await _easyRedisClient.SortedSetIncrementAsync(key, keyWords, 1, StackExchange.Redis.CommandFlags.FireAndForget);
        }

        [Description("搜索话题圈列表")]
        public async Task<ActionResult> SearchCirclePage(string keyWords, int PageNo = 1, int PageSize = 10)
        {
            await SetHotword(keyWords, ChannelIndex.Circle);
            Guid? userId = null;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                userId = user.UserId;
                string Key = $"SearchHistory:{user.UserId}";
                await _easyRedisClient.SortedSetAddAsync(Key, keyWords, DateTime.Now.D2I(), StackExchange.Redis.CommandFlags.FireAndForget);
            }

            var viewModels = new List<CircleItemViewModel>();

            var searchCircles = _dataSearch.SearchCircle(new SearchBaseQueryModel(keyWords, PageNo, PageSize));
            var circleIds = searchCircles.Select(s => s.Id);
            if (circleIds.Any())
            {
                var circleDtos = _circleService.GetCircles(circleIds, userId);

                //使用原有序列, 原有圈粉数量, 避免排序差异
                var vm = _mapper.Map<List<CircleItemViewModel>>(circleDtos);
                viewModels = searchCircles.Select(m => {
                    var circle = vm.Where(s => s.Id == m.Id).FirstOrDefault();
                    circle.FollowCount = m.FollowCount;
                    return circle;
                }).ToList();
            }

            ViewBag.Data = viewModels;
            return View();
        }

        [Description("猜你喜欢话题圈")]
        public ActionResult GuessCirclePage()
        {
            Guid? userId = null;
            if (User.Identity.IsAuthenticated)
                userId = User.Identity.GetUserInfo()?.UserId;

            int cityCode = Request.GetLocalCity();
            //根据城市推荐话题圈
            var result = this._circleService.GetRecommends(cityCode);
            var circles = result.Data;

            var viewModels = new List<CircleItemViewModel>();
            if (circles.Any())
            {
                var circleIds = circles.Select(s => s.Id);
                var circleDtos = _circleService.GetCircles(circleIds, userId);

                //使用原有序列
                var vm = _mapper.Map<List<CircleItemViewModel>>(circleDtos);
                viewModels = circleIds.Select(id => vm.Where(s => s.Id == id).FirstOrDefault()).ToList();
            }

            ViewBag.Data = viewModels;
            return View("SearchCirclePage");
        }

        /// <summary>
        /// 学校猜你喜欢
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> GuessSchoolPage(int index)
        {
            //用户id or  设备id
            var id = Request.GetDevice(Guid.NewGuid().ToString());//"df279433-217f-4098-a346-57b96789f4ea";
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                id = user.UserId.ToString();
            }

            string key = $"UserRecommendExtApi:{id}:{index}";

            var list = await _easyRedisClient.GetOrAddAsync(key, async () =>
            {
                try
                {
                    var response = await _userRecommendClient.UserRecommendSchool(Guid.Parse(id), index, 10);
                    return new RecommenderResponseDto
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
                }
                catch (Exception)
                {
                    return null;
                }
            }, TimeSpan.FromMinutes(5));

            if (list == null)
            {
                ViewBag.Data = new List<SchoolExtListItemViewModel>();
            }
            else
            {
                var ids = list.Items
                    .Where(p => Guid.TryParse(p.ContentId, out Guid guid))
                    .Select(q => Guid.Parse(q.ContentId)).ToList();

                var result = ids.Count > 0 ? await _schoolService.ListExtSchoolByBranchIds(ids) : new List<SchoolExtFilterDto>();

                ViewBag.Data = list.Items.Where(q => result.Any(p => p.ExtId.ToString() == q.ContentId)).Select(q =>
                {
                    var r = result.FirstOrDefault(p => p.ExtId.ToString() == q.ContentId);
                    return r == null ? new SchoolExtListItemViewModel() : new SchoolExtListItemViewModel
                    {
                        SchoolNo =  r.SchoolNo,
                        SchoolId = r.Sid,
                        BranchId = r.ExtId,
                        SchoolName = r.Name,
                        CityName = r.City,
                        AreaName = r.Area,
                        Distance = r.Distance,
                        LodgingType = (int)r.LodgingType,
                        LodgingReason = r.LodgingType.Description(),
                        Type = r.Type,
                        International = r.International,
                        Cost = r.Tuition,
                        Tags = r.Tags,
                        Score = r.Score,
                        CommentTotal = r.CommentCount
                    };
                });
            }
            return View();
        }

        /// <summary>
        /// 点评猜你喜欢
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> GuessCommentPage(int index)
        {
            //用户id or  设备id
            var id = Request.GetDevice(Guid.NewGuid().ToString());
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                id = user.UserId.ToString();
            }

            string key = $"UserRecommendExtApi:{id}:{index}";

            var list = await _easyRedisClient.GetOrAddAsync(key, async () =>
            {
                try
                {
                    var response = await _userRecommendClient.UserRecommendSchool(Guid.Parse(id), index, 10);
                    return new RecommenderResponseDto
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
                }
                catch (Exception)
                {
                    return null;
                }
            }, TimeSpan.FromMinutes(5));

            if (list == null)
            {
                ViewBag.Data = new List<SchoolExtListItemViewModel>();
            }
            else
            {
                var ids = list.Items
                    .Where(p => Guid.TryParse(p.ContentId, out Guid guid))
                    .Select(q => Guid.Parse(q.ContentId)).ToList();
            }
            return View();
        }

        /// <summary>
        /// 问答猜你喜欢
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> GuessQuestionPage(int index)
        {
            //用户id or  设备id
            var id = Request.GetDevice(Guid.NewGuid().ToString());
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                id = user.UserId.ToString();
            }

            string key = $"UserRecommendExtApi:{id}:{index}";

            var list = await _easyRedisClient.GetOrAddAsync(key, async () =>
            {
                try
                {
                    var response = await _userRecommendClient.UserRecommendSchool(Guid.Parse(id), index, 10);
                    return new RecommenderResponseDto
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
                }
                catch (Exception)
                {
                    return null;
                }
            }, TimeSpan.FromMinutes(5));

            if (list == null)
            {
                ViewBag.Data = new List<SchoolExtListItemViewModel>();
            }
            else
            {
                var ids = list.Items
                    .Where(p => Guid.TryParse(p.ContentId, out Guid guid))
                    .Select(q => Guid.Parse(q.ContentId)).ToList();
            }
            return View();
        }

        /// <summary>
        /// 文章猜你喜欢
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> GuessArticlePage(int index)
        {
            //用户id or  设备id
            var id = Request.GetDevice(Guid.NewGuid().ToString());
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                id = user.UserId.ToString();
            }

            string key = $"UserRecommendExtApi:{id}:{index}";

            var list = await _easyRedisClient.GetOrAddAsync(key, async () =>
            {
                try
                {
                    var response = await _userRecommendClient.UserRecommendSchool(Guid.Parse(id), index, 10);
                    return new RecommenderResponseDto
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
                }
                catch (Exception)
                {
                    return null;
                }
            }, TimeSpan.FromMinutes(5));

            if (list == null)
            {
                ViewBag.Data = new List<SchoolExtListItemViewModel>();
            }
            else
            {
                var ids = list.Items
                    .Where(p => Guid.TryParse(p.ContentId, out Guid guid))
                    .Select(q => Guid.Parse(q.ContentId)).ToList();
            }
            return View();
        }





        /// <summary>
        /// 地图搜索学校
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Route("jsonp")]
        [Description("地图搜校页")]
        public ActionResult Map()
        {
            return View();
        }



        /// <summary>
        /// 地图搜索学校
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[Route("jsonp")]
        [Description("地图搜校列表")]
        public async Task SearchSchoolMap(string callBack, MapData data)
        {
            int cityCode = Request.GetLocalCity();
            data.Type = data.Type.Where(q => q != null).ToList();

            if (data.Type.Any(q => !q.Contains("lx")))
            {
                data.Grade = data.Type.Where(q => !q.Contains("lx")).Select(q => Convert.ToInt32(q)).ToList();
            }
            data.Type = data.Type.Where(q => q.Contains("lx")).ToList();
            var mapResult = new SearchMapViewModel();
            var re = _dataSearch.SearchSchoolMap(new SearchMapSchoolQuery
            {
                KeyWords = data.KeyWords,
                CityCode = cityCode,
                Level = data.Level,
                Grade = data.Grade,
                Type = data.Type,
                MetroLineIds = data.MetroLineIds,
                MetroStationIds = data.MetroStationIds,
                Canteen = data.Canteen,
                Lodging = data.Lodging,
                AbroadIds = data.AbroadIds,
                AuthIds = data.AuthIds,
                CharacIds = data.CharacIds,
                CourseIds = data.CourseIds,
                SWLat = data.SWLat,
                SWLng = data.SWLng,
                NELat = data.NELat,
                NELng = data.NELng
            });

            var Locations = new List<LocationViewModel>();
            if (data.Level == 1)
            {
                var areas = await _cityService.GetAreaPolyline(cityCode);

                Locations = re.List.Select(q => new LocationViewModel
                {
                    Name = areas.FirstOrDefault(p => p.Code.ToString() == q.Name)?.Name,
                    Latitude = areas.FirstOrDefault(p => p.Code.ToString() == q.Name)?.Latitude ?? 0,
                    Longitude = areas.FirstOrDefault(p => p.Code.ToString() == q.Name)?.Longitude ?? 0,
                    Count = q.Count,
                    Ext = new { areaCode = q.Name }
                }).ToList();
            }
            else if (data.Level == 2)
            {
                Locations = re.List.Select(q => new LocationViewModel
                {
                    Name = q.Name,
                    Latitude = q.Latitude,
                    Longitude = q.Longitude,
                    Count = q.Count,
                    Ext = new { q.Id }
                }).ToList();
            }
            else
            {
                Locations = re.List.Select(q => new LocationViewModel
                {
                    Name = q.Name,
                    Latitude = q.Latitude,
                    Longitude = q.Longitude,
                    Count = q.Count,
                    Ext = new { q.Id, q.SchoolId }
                }).ToList();
            }

            var result = ResponseResult.Success(new SearchMapViewModel
            {
                Level = data.Level,
                Count = re.Count,
                ViewCount = re.ViewCount,
                Locations = Locations,
                SelectType = data.Type
            });

            string response = JsonConvert.SerializeObject(result, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            string call = callBack + "(" + response + ")";
            Response.ContentType = "text/plain; charset=utf-8";
            //Response.Headers.Add("Access-Control-Allow-Origin", "*");
            await Response.WriteAsync(call, Encoding.UTF8);
        }
    }
}
