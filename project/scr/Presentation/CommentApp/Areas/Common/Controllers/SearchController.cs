using AutoMapper;
using Elasticsearch.Net;
using iSchool;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Application.ModelDto;
using PMS.Infrastructure.Application.IService;
using PMS.Infrastructure.Application.ModelDto;
using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using PMS.School.Application.IServices;
using PMS.School.Application.ModelDto;
using PMS.School.Domain.Entities;
using PMS.School.Domain.Enum;
using PMS.Search.Application.IServices;
using PMS.Search.Application.ModelDto.Query;
using PMS.Search.Domain.Common;
using PMS.Search.Domain.Entities;
using PMS.Search.Domain.QueryModel;
using PMS.TopicCircle.Application.Dtos;
using PMS.TopicCircle.Application.Services;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Application.ModelDto;
using PMS.UserManage.Application.ModelDto.Talent;
using PMS.UserManage.Domain.Common;
using ProductManagement.API.Http.Interface;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using ProductManagement.Infrastructure.Models;
using Sxb.Web.Areas.Common.Models;
using Sxb.Web.Areas.Common.Models.Search;
using Sxb.Web.Common;
using Sxb.Web.Controllers;
using Sxb.Web.Models;
using Sxb.Web.Models.Comment;
using Sxb.Web.Models.Question;
using Sxb.Web.Models.User;
using Sxb.Web.Response;
using Sxb.Web.Utils;
using Sxb.Web.ViewModels.Article;
using Sxb.Web.ViewModels.Live;
using Sxb.Web.ViewModels.School;
using Sxb.Web.ViewModels.Search;
using Sxb.Web.ViewModels.TopicCircle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using System.Threading.Tasks;
using static PMS.Search.Domain.Common.GlobalEnum;

namespace Sxb.Web.Areas.Common.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public partial class SearchController : ApiBaseController
    {
        private readonly ImageSetting _imageSetting;

        private readonly IMapper _mapper;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly ISearchService _searchService;
        private readonly IImportService _importService;
        private readonly ICourseSearchService _courseSearchService;
        private readonly IEvaluationSearchService _evaluationSearchService;
        private readonly IKeywordSearchService _keywordSearchService;

        private readonly IUserService _userService;
        private readonly ITalentService _talentService;

        private readonly ICircleService _circleService;
        private readonly ICircleFollowerService _circleFollowerService;

        private readonly ISchService _schService;
        private readonly ISchoolService _schoolService;
        private readonly ISchoolInfoService _schoolInfoService;
        private readonly ICityInfoService _cityInfoService;
        private readonly ISchoolCommentService _commentService;
        private readonly IQuestionInfoService _questionService;
        private readonly IArticleService _articleService;
        private readonly ISchoolRankService _schoolRankService;

        private readonly ILiveServiceClient _liveServiceClient;
        private readonly IOrgServiceClient _orgServiceClient;
        private readonly IStaticInsideClient _staticInsideClient;

        private string Domain { get; }
        private string UserDomain { get; }
        private readonly ChannelUrlHelper _channelUrlHelper;

        public static string ES_HISTORY_KEY = "H5";

        //e.g. SearchHotword:All  SearchHotword:School
        public static string REDIS_HOTWORD_KEY = "SearchHotword:{0}";//channel 

        //public static string HISTORY_KEY = "SearchHistory:{0}:{1}";// channel-userId
        public static string REDIS_HISTORY_KEY = "SearchHistory:{0}";//userId

        public SearchController(ITalentService talentService, IMapper mapper, IEasyRedisClient easyRedisClient, ICityInfoService cityInfoService, ISchoolCommentService commentService, IQuestionInfoService questionService, ISchoolInfoService schoolInfoService, IOptions<ImageSetting> options, ISearchService searchService, IArticleService articleService, ILiveServiceClient liveServiceClient, IUserService userService, ICircleService circleService, ISchoolService schoolService, ISchService schService, ICircleFollowerService circleFollowerService, IImportService importService, ICourseSearchService courseSearchService, IEvaluationSearchService evaluationSearchService, IOrgServiceClient orgServiceClient, IKeywordSearchService keywordSearchService, IStaticInsideClient staticInsideClient, ISchoolRankService schoolRankService)
        {
            Domain = ConfigHelper.GetHost();
            UserDomain = ConfigHelper.GetUserHost();
            _channelUrlHelper = new ChannelUrlHelper(Domain, UserDomain);

            _talentService = talentService;
            _mapper = mapper;
            _easyRedisClient = easyRedisClient;
            _cityInfoService = cityInfoService;
            _commentService = commentService;
            _questionService = questionService;
            _schoolInfoService = schoolInfoService;
            _imageSetting = options.Value;
            _searchService = searchService;
            _articleService = articleService;
            _liveServiceClient = liveServiceClient;
            _userService = userService;
            _circleService = circleService;
            _schoolService = schoolService;
            _schService = schService;
            _circleFollowerService = circleFollowerService;
            _importService = importService;
            _courseSearchService = courseSearchService;
            _evaluationSearchService = evaluationSearchService;
            _orgServiceClient = orgServiceClient;
            _keywordSearchService = keywordSearchService;
            _staticInsideClient = staticInsideClient;
            _schoolRankService = schoolRankService;
        }

        public ChannelIndex GetChannelIndex(int channel)
        {
            if (channel == 0)
            {
                return ChannelIndex.All;
            }
            return (ChannelIndex)channel;
        }


        #region Obsolete Redis Hotword and History
        [NonAction]
        public string GetRedisHotwordKey(ChannelIndex channel)
        {
            //e.g. SearchHotword:All  SearchHotword:School
            return string.Format(REDIS_HOTWORD_KEY, channel);
        }
        [NonAction]
        public string GetRedisHistoryKey(Guid userId)
        {
            return string.Format(REDIS_HISTORY_KEY, userId);
        }


        [NonAction]
        [Obsolete("已使用ES")]
        public async Task SetHotword(string keyword, ChannelIndex channel)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return;

            string key = GetRedisHotwordKey(channel);
            keyword = keyword.Trim();
            await _easyRedisClient.SortedSetIncrementAsync(key, keyword, 1, StackExchange.Redis.CommandFlags.FireAndForget);
        }

        public async Task<ResponseResult> GetRedisHotwords(int channel)
        {
            var channelIndex = GetChannelIndex(channel);
            string key = GetRedisHotwordKey(channelIndex);
            var list = await _easyRedisClient.SortedSetRangeByRankAsync<string>(key, 0, 5, StackExchange.Redis.Order.Descending);
            var result = list.Select(q => new SearchWordViewModel { Name = q }).ToList();
            return ResponseResult.Success(result);
        }





        [NonAction]
        public async Task SetRedisHistory(string keyword, ChannelIndex channel, Guid? userId)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return;

            if (userId == null || userId == Guid.Empty)
                return;

            //redis history
            string key = GetRedisHistoryKey(userId.Value);
            keyword = keyword.Trim();
            await _easyRedisClient.SortedSetAddAsync(key, keyword, DateTime.Now.D2I(), StackExchange.Redis.CommandFlags.FireAndForget);
        }

        /// <summary>
        /// 历史搜索列表
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseResult> GetRedisHistories()
        {
            var result = new List<SearchWordViewModel>();
            if (UserId == null || UserId == Guid.Empty)
            {
                return ResponseResult.Success(result);
            }

            string key = GetRedisHistoryKey(UserId.Value);
            var list = await _easyRedisClient.SortedSetRangeByRankAsync<string>(key, 0, 10, StackExchange.Redis.Order.Descending);
            result = list.Select(q => new SearchWordViewModel { Name = q }).ToList();
            return ResponseResult.Success(result);
        }

        /// <summary>
        /// 清除历史搜索
        /// </summary>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public async Task<ResponseResult> RemoveRedisHistories()
        {
            if (UserId == null || UserId == Guid.Empty)
            {
                return ResponseResult.Success();
            }

            string key = GetRedisHistoryKey(UserId.Value);
            await _easyRedisClient.RemoveAsync(key);
            return ResponseResult.Success();
        }

        #endregion

        #region es Hotword and History

        /// <summary>
        /// 设置历史记录
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="channel">原定分channel保存历史记录, 暂没用</param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [NonAction]
        public void SetHistory(string keyword, ChannelIndex channel, Guid? userId)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return;

            userId = userId == Guid.Empty ? null : userId;

            var historyChannel = channel.GetDefaultValue<int>();

            //es history
            var cityCode = Request.GetLocalCity();
            var uuid = Request.GetDevice();

            //没有登录又没有uuid则不插入记录
            if (userId == null && string.IsNullOrWhiteSpace(uuid))
            {
                return;
            }
            _importService.SetHistory(ES_HISTORY_KEY, historyChannel, keyword, cityCode, userId, uuid);
        }

        [NonAction]
        public void SetPCHistory(string keyword, ChannelIndex channel)
        {
            var cityCode = Request.GetLocalCity();
            var uuid = Request.GetDevice();
            string ua = Request.Headers["User-Agent"].ToString();

            var historyChannel = channel.GetDefaultValue<int>();

            if (!string.IsNullOrWhiteSpace(keyword)
                && !ua.ToLower().Contains("spider")
                && !ua.ToLower().Contains("bot")
                && cityCode != 0)
            {
                var userId = User.Identity.IsAuthenticated ? UserId : null;
                _importService.SetHistory("PC", historyChannel, keyword, cityCode, userId, uuid);
            }
        }

        /// <summary>
        /// 热门搜索词列表
        /// </summary>
        /// <param name="channel">0 所有 1 学校 2 点评 4 问题 8 学校排名 16 文章  32 直播  64 话题圈 128 达人</param>
        /// <returns></returns>
        public async Task<ResponseResult> GetHotwords(int channel, int pageSize = 10)
        {
            var cityCode = Request.GetLocalCity();
            ChannelIndex channelIndex = GetChannelIndex(channel);
            var channelValue = channelIndex.GetDefaultValue<int>();

            var redisKey = string.Format(RedisKeys.HotwordsChannelKeys, channelIndex.ToString().ToLower());

            //热词  默认选择手动设置的固定热词
            IEnumerable<string> hotwords = await _easyRedisClient.GetAsync<List<string>>(redisKey) ?? new List<string>();
            var surplus = pageSize - hotwords.Count();
            //不足数的取es中的
            if (surplus > 0)
            {
                List<string> list = _searchService.GetHotHistoryList(channelValue, pageSize, null, cityCode);
                //去重
                hotwords = hotwords.Concat(list).Distinct();
            }

            var result = hotwords.Take(pageSize).Select(q => new SearchWordViewModel { Name = q }).ToList();
            return ResponseResult.Success(result);
        }


        /// <summary>
        /// 历史搜索列表
        /// </summary>
        /// <returns></returns>
        public ResponseResult GetHistories(int channel, int pageSize = 10)
        {
            var uuid = Request.GetDeviceToGuid();

            var userId = UserId == Guid.Empty ? null : UserId;
            if (userId == null && uuid == Guid.Empty)
            {
                return ResponseResult.Success(new List<string>());
            }

            var channelIndex = GetChannelIndex(channel);
            var historyChannel = channelIndex.GetDefaultValue<int>();

            List<string> list = _searchService.GetDistinctHistorySearch(historyChannel, userId, uuid, pageSize);
            var result = list.Select(q => new SearchWordViewModel { Name = q }).ToList();

            return ResponseResult.Success(result);
        }

        [HttpGet, HttpPost]
        public ResponseResult RemoveHistories()
        {
            if (UserId == null || UserId == Guid.Empty)
            {
                return ResponseResult.Success();
            }

            var ret = _importService.RemoveUserHistory(UserId.Value);
            if (ret)
            {
                return ResponseResult.Success();
            }
            return ResponseResult.Failed();
        }
        #endregion

        /// <summary>
        /// 搜索筛选列表页数据源
        /// </summary>
        public async Task<ResponseResult> GetSearchTool(ChannelIndex channel, int? cityCode = null, bool searchCostSection = true)
        {
            var cityId = cityCode != null ? cityCode.Value : Request.GetLocalCity();

            ResponsePCSearchTool tools = new ResponsePCSearchTool();
            tools.CityCode = cityId;
            tools.Channel = ChannelIndex.School;

            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                //检测当前用户身份 （true：校方用户 |false：普通用户）
                tools.IsSchoolRole = user.Role.Contains((int)UserRole.School);
            }

            List<AreaDto> areas = new List<AreaDto>();
            List<MetroDto> metros = new List<MetroDto>();
            List<TagFlat> tags = new List<TagFlat>();
            List<GradeTypeDto> types = new List<GradeTypeDto>();
            IEnumerable<CostSectionDto> costs = new List<CostSectionDto>();


            var tasks = new List<Task>() {
                 Task.Run(async () => areas = await _cityInfoService.GetAreaCode(cityId)),
                 Task.Run(async () => metros = await _cityInfoService.GetMetroList(cityId)),
                 Task.Run(async () => tags = (await _schoolService.GetTagFlats()).ToList()),
                 Task.Run(() => types = _schoolService.GetGradeTypeList())
            };
            if (searchCostSection)
            {
                tasks.Add(Task.Run(async () => costs = await _schoolService.GetSearchSchoolCostAsync()));
            }
            await Task.WhenAll(tasks);

            tools.Costs = costs;
            tools.Areas = areas;
            tools.Metros = metros.OrderBy(q => q.MetroName).ToList();
            //按年纪逆序 
            tools.Types = SchoolGradeDetailVM.ConvertTo(types);

            tools.Authentications = TagFlat.GetTags(tags, 2);
            tools.Abroads = TagFlat.GetTags(tags, 3);
            tools.Characteristics = TagFlat.GetTags(tags, 8);
            tools.Courses = TagFlat.GetTags(tags, 6);

            tools.SchoolScores = new string[] { "A+", "A", "B", "C", "D" };
            return ResponseResult.Success(tools);
        }

        public async Task<ResponseResult> GetPCSearchSchoolTool(int? cityCode = null)
        {
            return await GetSearchTool(ChannelIndex.School, cityCode, searchCostSection: false);
        }

        /// <summary>
        /// 联想词搜索
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="channel">0 所有 1 学校 2 点评 4 问题 8 学校排名 16 文章  32 直播  64 话题圈 128 达人</param>
        /// <returns></returns>
        public ResponseResult GetSuggestList(string keyword, int channel)
        {
            var channelIndex = GetChannelIndex(channel);
            var size = channelIndex != ChannelIndex.All ? 8 : 2;

            int cityCode = Request.GetLocalCity();
            var list = _searchService.ListSuggest(keyword, channelIndex, cityCode, size);

            var group = new List<ChannelIndex> {
                ChannelIndex.School, ChannelIndex.Comment, ChannelIndex.Question, ChannelIndex.Talent,
                ChannelIndex.Live, ChannelIndex.Circle, ChannelIndex.SchoolRank, ChannelIndex.Article
            };
            var result = group.Select(s => new SuggestGroupViewModel { Index = s }).ToList();

            for (int i = 0; i < result.Count(); i++)
            {
                var itemIndex = result[i].Index;
                var suggests = list.Suggests.FirstOrDefault(q => q.Index == itemIndex)?.Items;

                if (suggests == null)
                {
                    result[i].Items = new List<SuggestViewModel>();
                    continue;
                }
                List<Guid> ids = suggests.GroupBy(q => q.SourceId).Select(p => p.Key).ToList();
                //反查数据库获取学校点评数、点评回复数、问题回答数、文章浏览数
                List<SchoolTotalDto> total = null;
                List<SearchTalentDto> talents = new List<SearchTalentDto>();
                switch (itemIndex)
                {
                    case ChannelIndex.School:
                        total = _commentService.GetCommentCountBySchool(ids);

                        if (total == null) total = new List<SchoolTotalDto>();
                        var schoolNos = _schService.GetSchoolextNo(ids.Distinct().ToArray());
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
                    case ChannelIndex.Comment:
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
                    case ChannelIndex.Question:
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
                    case ChannelIndex.Article:
                        var articles = _articleService.GetByIds(ids.ToArray());
                        total = articles.Select(q => new SchoolTotalDto { Id = q.id, No = UrlShortIdUtil.Long2Base32(q.No), Total = q.VirualViewCount }).ToList();
                        break;
                    //话题圈 粉丝数
                    case ChannelIndex.Circle:
                        var circleFollowers = _circleFollowerService.GetFollowerCount(ids.ToArray());
                        total = circleFollowers.Select(s => new SchoolTotalDto { Id = s.CircleId, Total = s.FollowerCount }).ToList();
                        break;
                    case ChannelIndex.Talent:
                        talents = _talentService.SearchTalents(ids, UserId);
                        break;
                }

                result[i].Items = suggests.Select(q =>
                {
                    var model = new SuggestViewModel
                    {
                        Channel = result[i].GroupType,
                        Type = itemIndex.GetDefaultValue<int>(),
                        Index = itemIndex,
                        KeyWord = q.Name,
                        KeyWord2 = q.Name2,
                        Value = total == null ? new { Id = q.SourceId, No = "" } : new { Id = q.SourceId, No = total.FirstOrDefault(p => p.Id == q.SourceId)?.No ?? "" },
                        Show = total == null ? null : new { count = total.FirstOrDefault(p => p.Id == q.SourceId)?.Total ?? 0 }
                    };
                    model.ActionUrl = GetActionUrl(q.SourceId, itemIndex, total, talents);
                    return model;
                }).ToList();

            }
            return ResponseResult.Success(new
            {
                keyword,
                total = channelIndex == ChannelIndex.All ? list.Total : list.Suggests.FirstOrDefault(q => q.Index == channelIndex)?.Total ?? 0,
                list = result
            });
        }

        private string GetActionUrl(Guid sourceId, ChannelIndex itemIndex, List<SchoolTotalDto> total, List<SearchTalentDto> talents)
        {
            //学校,点评,问题,文章 短链接
            if (itemIndex == ChannelIndex.School || itemIndex == ChannelIndex.Comment || itemIndex == ChannelIndex.Question || itemIndex == ChannelIndex.Article)
            {
                var actionUrl = itemIndex.GetActionUrlValue().ToWebUrl(Domain);
                return string.Format(actionUrl, total.FirstOrDefault(p => p.Id == sourceId)?.No);
            }
            //达人使用userId
            else if (itemIndex == ChannelIndex.Talent)
            {
                var actionUrl = itemIndex.GetActionUrlValue().ToWebUrl(UserDomain);
                return string.Format(actionUrl, talents.FirstOrDefault(p => p.Id == sourceId)?.UserId);
            }
            //其他使用id
            else
            {
                var actionUrl = itemIndex.GetActionUrlValue().ToWebUrl(Domain);
                return string.Format(actionUrl, sourceId);
            }
        }


        [HttpGet]
        public async Task<ResponseResult> GetSearchAll([FromQuery] SearchAllQueryModel queryModel)
        {
            queryModel.Size = 3;
            var lat = double.Parse(Request.GetLatitude());
            var lng = double.Parse(Request.GetLongitude());
            var channelIndex = queryModel.ChannelIndex;
            queryModel.MustCityCode = true;
            queryModel.IsMatchAllWord = true;
            queryModel.CityCode = queryModel.CityCode != 0 ? queryModel.CityCode : Request.GetLocalCity();

            //搜索记录与热词
            SetHistory(queryModel.Keyword, channelIndex, UserId);
            var list = _searchService.ListSuggest(queryModel);

            var group = new List<ChannelIndex> {
                ChannelIndex.School, ChannelIndex.Comment, ChannelIndex.Question, ChannelIndex.Talent,
                ChannelIndex.Live, ChannelIndex.Circle, ChannelIndex.SchoolRank, ChannelIndex.Article,
                ChannelIndex.Course, ChannelIndex.OrgEvaluation
            };
            var result = group.Select(s => new SearchAllPage { Index = s }).ToList();

            for (int i = 0; i < result.Count(); i++)
            {
                var resultPage = result[i];
                var itemIndex = resultPage.Index;
                var suggests = list.Suggests.FirstOrDefault(q => q.Index == itemIndex)?.Items;
                if (suggests == null)
                {
                    continue;
                }

                List<Guid> ids = suggests.GroupBy(q => q.SourceId).Select(p => p.Key).ToList();
                //反查数据库获取学校点评数、点评回复数、问题回答数、文章浏览数
                IEnumerable<object> items = new List<object>();
                var urlPropName = "Id";
                var highlightKeyPropName = "Name";
                switch (itemIndex)
                {
                    case ChannelIndex.School:
                        urlPropName = "SchoolNo";
                        items = await _schoolService.SearchSchools(ids, lat, lng);
                        break;
                    case ChannelIndex.Comment:
                        urlPropName = "No";
                        highlightKeyPropName = "Content";
                        var comments = _commentService.SearchComments(ids, UserIdOrDefault);
                        items = HandleComment(comments);
                        break;
                    case ChannelIndex.Question:
                        urlPropName = "No";
                        highlightKeyPropName = "QuestionContent";
                        var questions = _questionService.SearchQuestions(ids, UserIdOrDefault);
                        items = await HandleQuestion(questions);
                        break;
                    case ChannelIndex.Article:
                        urlPropName = "No";
                        highlightKeyPropName = "Title";
                        var articles = _articleService.SearchArticles(ids);
                        items = HandleArticle(articles);
                        break;
                    case ChannelIndex.SchoolRank:
                        highlightKeyPropName = "RankName";
                        items = GetSchoolRank(new SearchBaseQueryModel(queryModel.Keyword, 1, queryModel.Size) {
                            Suggestions = queryModel.Suggestions, 
                            MustCityCode = true,
                            IsMatchAllWord = true,
                            CityCode = queryModel.CityCode 
                        });
                        break;
                    case ChannelIndex.Live:
                        highlightKeyPropName = "Title";
                        items = await HandleLive(ids);
                        break;
                    //话题圈 粉丝数
                    case ChannelIndex.Circle:
                        var circles = _circleService.SearchCircles(ids, UserIdOrDefault);
                        items = HandleCircle(circles);
                        break;
                    case ChannelIndex.Talent:
                        urlPropName = "UserId";
                        highlightKeyPropName = "NickName";
                        items = _talentService.SearchTalents(ids, UserId);
                        break;
                    case ChannelIndex.Course:
                        highlightKeyPropName = "Title";
                        var courses = await _orgServiceClient.GetCourses(ids);
                        items = courses.SortTo(ids);
                        break;
                    case ChannelIndex.OrgEvaluation:
                        highlightKeyPropName = "Title";
                        var evaluations = await _orgServiceClient.GetEvaluations(ids);
                        items = evaluations.SortTo(ids);
                        break;
                }
                var refItems = items.ToList();
                _channelUrlHelper.CompleteUrl(ref refItems, itemIndex, urlPropName);
                HighlightHelper.SetHighlights(ref refItems, queryModel.Keyword, highlightKeyPropName);
                resultPage.Items = refItems;
            }
            return ResponseResult.Success(result);
        }

        /// <summary>
        /// es搜索学校列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<ResponseResult> SearchSchools(RequestSearchQuery query)
        {
            var channel = ChannelIndex.School;
            //搜索记录与热词
            //await SetHotword(query.Keyword, channel);
            SetHistory(query.Keyword, channel, UserId);

            query = await HandleQuery(query);
            var (result, total) = await _schoolService.SearchSchools(query);

            bool isOtherCityData = false;
            if (total == 0)
            {
                query.MustCurrentCity = null;
                //查询哪些城市有这些数据
                var cities = _searchService.SearchSchoolCity(query);
                if (cities.Any())
                {
                    isOtherCityData = true;
                    //从别的城市拿数据
                    query.MustCurrentCity = cities.FirstOrDefault().cityId;
                    (result, total) = await _schoolService.SearchSchools(query, onlyAuthTag: true);
                }
            }

            _channelUrlHelper.CompleteUrl(ref result, channel, "SchoolNo");

            return ResponseResult.Success(new
            {
                TotalCount = total,
                Data = result,
                IsOtherCityData = isOtherCityData,
                SearchCityId = query.MustCurrentCity,
                SearchCityName = result.FirstOrDefault()?.City
            });
        }


        public async Task<ResponseResult> SearchSchoolsFullMatch([FromQuery]SearchBaseQueryModel queryModel) {

            var channel = ChannelIndex.School;
            queryModel.MustCityCode = true;
            queryModel.IsMatchAllWord = true;
            queryModel.CityCode = queryModel.CityCode != 0 ? queryModel.CityCode : Request.GetLocalCity();

            //搜索记录与热词
            SetHistory(queryModel.Keyword, channel, UserId);

            var (result, total) = await _schoolService.SearchSchoolsFullMatch(queryModel);
            _channelUrlHelper.CompleteUrl(ref result, channel, "SchoolNo");

            return ResponseResult.Success(new
            {
                TotalCount = total,
                Data = result
            });
        }

        /// <summary>
        /// es预查询搜索结果数量
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<ResponseResult> SearchSchoolTotalCount(RequestSearchQuery query)
        {
            query = await HandleQuery(query);

            var totalCount = await _searchService.SearchSchoolTotalCountAsync(query);

            return ResponseResult.Success(new
            {
                TotalCount = totalCount
            });
        }

        /// <summary>
        /// es搜索学校列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<ResponseResult> PCSearchSchools(RequestSearchQuery query)
        {
            var channel = ChannelIndex.School;
            //搜索记录与热词
            //await SetHotword(query.Keyword, channel);
            //SetHistory(query.Keyword, channel, UserId);
            SetPCHistory(query.Keyword, channel);

            query = await HandleQuery(query);

            var (result, total) = await _schoolService.SearchSchools(query, onlyAuthTag: true);
            bool isOtherCityData = false;
            if (total == 0)
            {
                query.MustCurrentCity = null;
                //查询哪些城市有这些数据
                var cities = _searchService.SearchSchoolCity(query);
                if (cities.Any())
                {
                    isOtherCityData = true;
                    //从别的城市拿数据
                    query.MustCurrentCity = cities.FirstOrDefault().cityId;
                    (result, total) = await _schoolService.SearchSchools(query, onlyAuthTag: true);
                }
            }

            //top10最多显示10条
            if (!string.IsNullOrEmpty(query.Top10))
            {
                total = total > 10 ? 10 : total;
            }

            var data = result.Select(s => SchoolItemVM.Convert(s));
            return ResponseResult.Success(new
            {
                Data = data,
                TotalCount = total,
                IsOtherCityData = isOtherCityData,
                SearchCityId = query.MustCurrentCity,
                SearchCityName = data.FirstOrDefault()?.CityName
            });
        }

        public async Task<ResponseResult> GetRecommendSchools(RequestSearchQuery query)
        {
            query = await HandleQuery(query);
            if (query.MustCurrentCity == null)
            {
                return ResponseResult.Failed("请选择城市");
            }

            //排除已经搜索到的学校id
            List<Guid> excludeExtIds = null;

            query.GradeIds = query.Type.Select(s => (int)new SchFType0(s).Grade).ToList();
            var data = await _schoolService.GetRecommendSchools(query.Keyword, query.MustCurrentCity.Value, query.AreaCodes, query.GradeIds, query.Type, query.MinTotal, query.MaxTotal,
                 query.AuthIds, query.CourseIds, query.CharacIds);
            return ResponseResult.Success(data);
        }

        public async Task<ResponseResult> GetRecommendSchoolsExplain(RequestSearchQuery query)
        {
            query = await HandleQuery(query);
            if (query.MustCurrentCity == null)
            {
                return ResponseResult.Failed("请选择城市");
            }

            //排除已经搜索到的学校id
            List<Guid> excludeExtIds = null;

            query.GradeIds = query.Type.Select(s => (int)new SchFType0(s).Grade).ToList();
            var data = await _searchService.RecommenSchoolExplainAsync(query.Keyword, query.MustCurrentCity.Value, query.AreaCodes, query.GradeIds, query.Type, query.MinTotal, query.MaxTotal,
                 query.AuthIds, query.CourseIds, query.CharacIds, query.PageNo, query.PageSize);
            return ResponseResult.Success(data);
        }

        public async Task<ResponseResult> SearchSchoolCity(RequestSearchQuery query)
        {
            query = await HandleQuery(query);

            var data = _searchService.SearchSchoolCity(query);

            return ResponseResult.Success(data);
        }

        private async Task<RequestSearchQuery> HandleQuery(RequestSearchQuery query)
        {
            int cityCode = Request.GetLocalCity();
            var lat = Request.GetLatitude();
            var lng = Request.GetLongitude();

            query.Type = query.Type ?? new List<string>();
            query.Grade = query.Grade ?? new List<int>();
            query.AreaCodes = query.AreaCodes ?? new List<int>();

            query.AuthIds = query.AuthIds ?? new List<Guid>();
            query.CharacIds = query.CharacIds ?? new List<Guid>();
            query.AbroadIds = query.AbroadIds ?? new List<Guid>();
            query.CourseIds = query.CourseIds ?? new List<Guid>();

            query.MetroLineIds = query.MetroLineIds ?? new List<Guid>();
            query.MetroStationIds = query.MetroStationIds ?? new List<int>();

            //search selected metro line and station id
            List<MetroQuery> metroIds = await _cityInfoService.GetMetroQuerys(query.MetroLineIds, query.MetroStationIds);

            //filter null
            query.Type = query.Type.Where(q => q != null).ToList();
            if (query.Type.Any(q => !q.Contains("lx")))
            {
                query.Grade = query.Type.Where(q => !q.Contains("lx")).Select(q => Convert.ToInt32(q)).ToList();
            }
            query.Type = query.Type.Where(q => q.Contains("lx")).ToList();

            //List<int> city = query.AreaCodes.Select(q => q / 100 * 100).GroupBy(q => q).Select(q => q.Key).ToList();

            query.Latitude = double.Parse(lat);
            query.Longitude = double.Parse(lng);
            query.CurrentCity = cityCode;
            query.CityCode = query.CityCode ?? new List<int>();
            query.MetroIds = metroIds;

            var (Min, Max) = Codstring.GetScore(query.ScoreLevel);
            query.MaxTotal = Max;
            query.MinTotal = Min;


            var top10 = query.Top10;
            //top10学校的逻辑：按照当前城市指定分数维度倒序取前10的学校
            if (!string.IsNullOrEmpty(top10))
            {
                query.PageNo = 1;
                query.PageSize = 10;
                switch (top10.ToLower())
                {
                    case "teach": query.Orderby = 6; break;
                    case "hard": query.Orderby = 7; break;
                    case "cost": query.Orderby = 3; break;
                    case "envir": query.Orderby = 9; break;
                }
            }
            return query;
        }


        /// <summary>
        /// 搜索点评列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<ResponseResult> SearchComments(RequestSearchQuery query)
        {
            var channel = ChannelIndex.Comment;
            var keyword = query.Keyword;
            //搜索记录与热词
            //await SetHotword(keyword, channel);
            SetHistory(keyword, channel, UserId);

            query.TypeIds = query.TypeIds ?? new List<int>();
            query.GradeIds = query.GradeIds ?? new List<int>();
            query.AreaCodes = query.AreaCodes ?? new List<int>();

            int cityCode = Request.GetLocalCity();
            //统一前端传值, 这里转换
            int convertOrderBy = 0; //排序 0 智能排序 1 时间优先 2 口碑评级优先 3 评论数优先
            switch ((ArticleOrderBy)query.Orderby)
            {
                case ArticleOrderBy.HotDesc:
                    convertOrderBy = 3;
                    break;
                case ArticleOrderBy.TimeDesc:
                    convertOrderBy = 1;
                    break;
            }


            //todo:后面重新导入学校数据，lodging字段改为int，解决搜索“寄宿&走读”的学校问题
            bool? lodging = (query.Lodging == 3 || query.Lodging == null) ? (bool?)null : query.Lodging == 2;
            var pagination = _commentService.SearchComments(new SearchCommentQuery
            {
                Keyword = query.Keyword,
                Keywords = query.GetQueryEsKeywords(query.Keyword),
                CityIds = new List<int> { cityCode },
                AreaIds = query.AreaCodes,
                GradeIds = query.GradeIds,
                TypeIds = query.TypeIds,
                PageNo = query.PageNo,
                PageSize = query.PageSize,
                Orderby = convertOrderBy,
                Lodging = lodging
            }, UserIdOrDefault);
            var comments = pagination.Data;

            List<CommentList> commentList = HandleComment(comments);
            _channelUrlHelper.CompleteUrl(ref commentList, channel, "No");
            return ResponseResult.Success(PaginationModel<CommentList>.Build(commentList, pagination.Total));
        }

        private List<CommentList> HandleComment(List<SchoolCommentDto> comments)
        {
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

                    x.UserInfo = _mapper.Map<UserInfoDto, UserInfoVo>(user);
                    var schoolInfo = SchoolInfos.Where(s => s.SchoolSectionId == x.SchoolSectionId).FirstOrDefault();
                    x.School = _mapper.Map<SchoolInfoDto, CommentList.SchoolInfoVo>(schoolInfo);
                }
            }

            return commentList;
        }


        /// <summary>
        /// 搜索问答列表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="PageNo"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public async Task<ResponseResult> SearchQuestions(RequestSearchQuery query)
        {
            var channel = ChannelIndex.Question;
            var keyword = query.Keyword;
            //搜索记录与热词
            //await SetHotword(keyword, channel);
            SetHistory(keyword, channel, UserId);

            query.TypeIds = query.TypeIds ?? new List<int>();
            query.GradeIds = query.GradeIds ?? new List<int>();
            query.AreaCodes = query.AreaCodes ?? new List<int>();
            int cityCode = Request.GetLocalCity();
            //统一前端传值, 这里转换
            int convertOrderBy = 0; //排序 0 智能排序 1 时间优先 2 口碑评级优先 3 评论数优先
            switch ((ArticleOrderBy)query.Orderby)
            {
                case ArticleOrderBy.HotDesc:
                    convertOrderBy = 3;
                    break;
                case ArticleOrderBy.TimeDesc:
                    convertOrderBy = 1;
                    break;
            }

            //todo:后面重新导入学校数据，lodging字段改为int，解决搜索“寄宿&走读”的学校问题
            bool? lodging = (query.Lodging == 3 || query.Lodging == null) ? (bool?)null : query.Lodging == 2;

            var pagination = _questionService.SearchQuestions(new SearchQuestionQuery
            {
                Keyword = query.Keyword,
                Keywords = query.GetQueryEsKeywords(query.Keyword),
                CityIds = new List<int> { cityCode },
                AreaIds = query.AreaCodes,
                GradeIds = query.GradeIds,
                TypeIds = query.TypeIds,
                PageNo = query.PageNo,
                PageSize = query.PageSize,
                Orderby = convertOrderBy,
                Lodging = lodging
            }, UserIdOrDefault);
            var qeustions = pagination.Data;

            List<QuestionVo> questionLists = await HandleQuestion(qeustions);
            _channelUrlHelper.CompleteUrl(ref questionLists, channel, "No");
            return ResponseResult.Success(PaginationModel<QuestionVo>.Build(questionLists, pagination.Total));
        }

        private async Task<List<QuestionVo>> HandleQuestion(List<QuestionDto> qeustions)
        {
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

            return questionLists;
        }

        /// <summary>
        /// 搜索排行榜列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<ResponseResult> SearchSchoolRanks([FromQuery] SearchBaseQueryModel queryModel)
        {
            var channel = ChannelIndex.SchoolRank;
            queryModel.MustCityCode = true;
            queryModel.IsMatchAllWord = true;
            queryModel.CityCode = queryModel.CityCode != 0 ? queryModel.CityCode : Request.GetLocalCity();


            //搜索记录与热词
            //await SetHotword(keyword, channel);
            SetHistory(queryModel.Keyword, channel, UserId);

            var pagination = _searchService.SearchSchoolRank(queryModel);
            var data = pagination.Ranks.Select(q => new SchoolRankItemViewModel
            {
                Id = q.Id,
                RankName = q.Title,
                Schools = GetSchoolNames(q.Schools)
            }).ToList();

            _channelUrlHelper.CompleteUrl(ref data, channel);
            return ResponseResult.Success(PaginationModel.Build(data, pagination.Total));
        }

        public string GetSchoolNames(List<string> schoolNames)
        {
            if (schoolNames.Count == 0)
                return "";

            var name = schoolNames[0];
            if (schoolNames.Count > 1)
                return name += " 等";
            return name;
        }

        public IEnumerable<SchoolRankItemViewModel> GetSchoolRank([FromQuery] SearchBaseQueryModel queryModel)
        {
            var searchResult = _searchService.SearchSchoolRank(queryModel);

            var data = searchResult.Ranks.Select(q => new SchoolRankItemViewModel
            {
                Id = q.Id,
                RankName = q.Title,
                Schools = GetSchoolNames(q.Schools)
            });
            return data;
        }

        /// <summary>
        /// 搜索文章列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<ResponseResult> SearchArticles([FromQuery] SearchArticleQueryModel queryModel)
        {
            var channel = ChannelIndex.Article;
            queryModel.MustCityCode = true;
            queryModel.IsMatchAllWord = true;
            queryModel.CityCode = queryModel.CityCode != 0 ? queryModel.CityCode : Request.GetLocalCity();


            //搜索记录与热词
            SetHistory(queryModel.Keyword, channel, UserId);

            var pagination = _articleService.SearchArticles(queryModel);
            List<ArticleListItemViewModel> data = HandleArticle(pagination.Data);
            _channelUrlHelper.CompleteUrl(ref data, channel, "No");

            return ResponseResult.Success(PaginationModel.Build(data, pagination.Total));
        }

        private static List<ArticleListItemViewModel> HandleArticle(List<article> articles)
        {
            //to vo
            return articles.Select(a => new ArticleListItemViewModel()
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
        }

        /// <summary>
        /// 搜索直播列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<ResponseResult> SearchLives([FromQuery] SearchLiveQueryModel queryModel)
        {
            var channel = ChannelIndex.Live;
            queryModel.MustCityCode = true;
            queryModel.IsMatchAllWord = true;
            queryModel.CityCode = queryModel.CityCode != 0 ? queryModel.CityCode : Request.GetLocalCity();


            //搜索记录与热词
            SetHistory(queryModel.Keyword, channel, UserId);

            var pagination = _searchService.SearchLive(queryModel);
            var ids = pagination.Lives.Select(q => q.Id).ToList();

            var data = await HandleLive(ids);
            _channelUrlHelper.CompleteUrl(ref data, channel);

            return ResponseResult.Success(PaginationModel.Build(data, pagination.Total));
        }

        private async Task<List<LiveItemViewModel>> HandleLive(IEnumerable<Guid> liveIds)
        {
            var data = new List<LiveItemViewModel>();
            if (!liveIds.Any())
            {
                return data;
            }
            var lives = await _liveServiceClient.QueryLectures(liveIds.ToList(), HttpContext.Request.GetAllCookies());
            if (lives.Items == null || lives.Items.Count == 0)
            {
                return data;
            }
            data = liveIds
                .Where(q => lives.Items.Any(p => p.Id == q))
                .Select(q =>
                {
                    var r = lives.Items.FirstOrDefault(p => p.Id == q);
                    return new LiveItemViewModel
                    {
                        Id = r.Id,
                        Title = r.Subject,
                        StartTime = (long)r.Time,
                        UserCount = r.Onlinecount,
                        BookCount = r.collectioncount,
                        Status = (LectureStatus)r.Status,
                        UserHeadImg = (r.lector.Headimgurl + "").ToHeadImgUrl(),
                        UserId = r.lector.UserId ?? Guid.Empty,
                        UserName = r.lector.Name,
                        CoverHome = r.HomeCover,
                        IsCollection = r.lector.IsFocus ?? false
                    };

                }).ToList();
            return data;
        }


        /// <summary>
        /// 搜索话题圈列表
        /// </summary>
        /// <param name="keyWords"></param>
        /// <param name="PageNo"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public async Task<ResponseResult> SearchCircles([FromQuery] SearchBaseQueryModel queryModel)
        {

            var channel = ChannelIndex.Circle;
            queryModel.IsMatchAllWord = true;
            SetHistory(queryModel.Keyword, channel, UserId);

            var serviceResult = _circleService.SearchCircles(UserId, queryModel);
            if (serviceResult.Status)
            {
                var data = HandleCircle(serviceResult.Data);
                _channelUrlHelper.CompleteUrl(ref data, channel);
                return ResponseResult.Success(PaginationModel.Build(data, serviceResult.Total));
            }
            else
            {
                return ResponseResult<List<CircleItemViewModel>>.Failed(serviceResult.Msg);
            }

        }

        private List<CircleItemViewModel> HandleCircle(List<SearchCircleDto> circleDtos)
        {
            return _mapper.Map<List<CircleItemViewModel>>(circleDtos);
        }


        /// <summary>
        /// 搜索达人列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<ResponseResult> SearchTalents([FromQuery] SearchBaseQueryModel queryModel)
        {
            var channel = ChannelIndex.Talent;
            queryModel.MustCityCode = true;
            queryModel.IsMatchAllWord = true;
            queryModel.CityCode = queryModel.CityCode != 0 ? queryModel.CityCode : Request.GetLocalCity();


            //搜索记录与热词
            SetHistory(queryModel.Keyword, channel, UserId);

            var pagination = _talentService.SearchTalents(UserId, queryModel);
            var data = pagination.Data;
            _channelUrlHelper.CompleteUrl(ref data, channel, "UserId");

            return ResponseResult.Success(PaginationModel.Build(data, pagination.Total));
        }

        public ResponseResult SearchSimpleTalents([FromQuery] SearchBaseQueryModel queryModel)
        {
            var pagination = _talentService.SearchTalents(UserId, queryModel);
            return ResponseResult.Success(pagination.Data.Select(s => new
            {
                s.UserId,
                s.NickName,
                s.HeadImgUrl,
                s.AuthTitle
            }));
        }

        /// <summary>
        /// 搜索中职学校列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="province"></param>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> SearchSvsSchool(SvsSchoolPageListQuery query)
        {
            var channel = ChannelIndex.SvsSchool;
            //搜索记录与热词
            //await SetHotword(query.SearchTxt, channel);
            SetHistory(query.SearchTxt, channel, UserId);
            var data = _searchService.SearchSvsSchool(query);
            return ResponseResult.Success(data);
        }


        /// <summary>
        /// 课程搜索推荐 
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseResult> SearchCourseSuggestions(string keyword, int pageSize = 10)
        {
            pageSize = pageSize > 100 || pageSize < 1 ? 10 : pageSize;
            var searchCourses = await _courseSearchService.ListSuggest(keyword, pageSize);
            return ResponseResult.Success(searchCourses.Select(s =>
                new { s.Name, s.ShortId }
            ));
        }


        /// <summary>
        /// 搜索课程 - 网课通小程序
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseResult> SearchCourses([FromQuery] SearchCourseQueryModel queryModel)
        {
            var channel = ChannelIndex.Course;
            //搜索记录与热词
            //await SetHotword(keyword, channel);
            SetHistory(queryModel.Keyword, channel, UserId);


            var pagination = await _courseSearchService.SearchCourses(queryModel);
            var ids = pagination.Data.Select(s => s.Id);
            var data = await _orgServiceClient.GetCourses(ids);

            var sortData = data.SortTo(ids);
            return ResponseResult.Success(sortData);
        }

        /// <summary>
        /// 搜索课程
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseResult> SearchCoursesPagination([FromQuery] SearchCourseQueryModel queryModel)
        {
            var channel = ChannelIndex.Course;
            //搜索记录与热词
            //await SetHotword(keyword, channel);
            SetHistory(queryModel.Keyword, channel, UserId);

            queryModel.SearchTitleOnly = true;
            queryModel.IsMatchAllWord = true;
            var pagination = await _courseSearchService.SearchCourses(queryModel);
            var ids = pagination.Data.Select(s => s.Id);
            var data = await _orgServiceClient.GetCourses(ids);

            var sortData = data.SortTo(ids);
            return ResponseResult.Success(PaginationModel.Build(sortData, pagination.Total));
        }

        /// <summary>
        /// 搜索种草(测评) - 网课通小程序
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        public async Task<ResponseResult> SearchEvaluations([FromQuery] SearchEvaluationQueryModel queryModel)
        {
            var channel = ChannelIndex.OrgEvaluation;
            //搜索记录与热词
            //await SetHotword(keyword, channel);
            SetHistory(queryModel.Keyword, channel, UserId);


            var pagination = _evaluationSearchService.SearchEvaluations(queryModel);
            var ids = pagination.Data.Select(s => s.Id);
            var data = await _orgServiceClient.GetEvaluations(ids);


            var sortData = data.SortTo(ids);
            return ResponseResult.Success(sortData);
        }

        /// <summary>
        /// 搜索种草(测评)
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseResult> SearchEvaluationsPagination([FromQuery] SearchEvaluationQueryModel queryModel)
        {
            var channel = ChannelIndex.OrgEvaluation;
            //搜索记录与热词
            //await SetHotword(keyword, channel);
            SetHistory(queryModel.Keyword, channel, UserId);


            queryModel.SearchTitleOnly = true;
            queryModel.IsMatchAllWord = true;
            var pagination = _evaluationSearchService.SearchEvaluations(queryModel);
            var ids = pagination.Data.Select(s => s.Id);
            var data = await _orgServiceClient.GetEvaluations(ids);


            var sortData = data.SortTo(ids);
            return ResponseResult.Success(PaginationModel.Build(sortData, pagination.Total));
        }


        /// <summary>
        /// 搜索候选词
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseResult> SearchKeywords([FromQuery] SearchKeywordQueryModel queryModel)
        {
            queryModel.CityCode = queryModel.CityCode != 0 ? queryModel.CityCode : Request.GetLocalCity();
            var pagination = await _keywordSearchService.SearchAsync(queryModel);

            var keywords = pagination.Data.Select(s => s.Keyword).ToList();
            var suggestTotals = _searchService.ListSuggestTotal(keywords, queryModel.ChannelIndex, queryModel.CityCode);
            var data = pagination.Data.Select(s =>
            {
                var suggestTotal = suggestTotals.FirstOrDefault(t => t.keyword == s.Keyword);
                return new
                {
                    Keyword = s.Highlight,
                    ResultNumber = suggestTotal.total
                };
            });

            return ResponseResult.Success(PaginationModel.Build(data, pagination.Total));
        }
    }
}
