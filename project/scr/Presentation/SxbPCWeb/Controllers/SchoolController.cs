using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PMS.CommentsManage.Application.IServices;
using PMS.Infrastructure.Application.IService;
using PMS.Infrastructure.Application.ModelDto;
using PMS.OperationPlateform.Application.IServices;
using PMS.School.Application.IServices;
using PMS.School.Application.ModelDto;
using PMS.Search.Application.IServices;
using PMS.Search.Application.ModelDto.Query;
using PMS.UserManage.Application.IServices;
using ProductManagement.API.Http.Interface;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using Sxb.PCWeb.Common;
using Sxb.PCWeb.Models;
using Sxb.PCWeb.RequestModel.School;
using Sxb.PCWeb.Response;
using Sxb.PCWeb.Utils;
using Sxb.PCWeb.ViewModels.School;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.PCWeb.Controllers
{
    using iSchool;
    using iSchool.Internal.API.OperationModule;
    using PMS.CommentsManage.Domain.Common;
    using PMS.OperationPlateform.Domain.Enums;
    using Sxb.PCWeb.Utils.CommentHelper;
    using Sxb.PCWeb.ViewModels.Comment;
    using Sxb.PCWeb.ViewModels.Rank;
    using System.ComponentModel;
    using System.Text.RegularExpressions;

    public partial class SchoolController : Controller
    {
        private readonly ISearchService _dataSearch;
        private readonly ISchoolService _schoolService;
        private readonly ICityInfoService _cityService;
        private readonly IHttpClientFactory _clientFactory;
        private IConfiguration _configuration;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly ISchoolCommentService _commentService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private IArticleService _articleService;
        private readonly ImageSetting _setting;
        private readonly IQuestionInfoService _questionInfoService;
        private readonly IUserServiceClient _userServiceClient;
        private readonly IHotPopularService _hotPopularService;
        private readonly ISchoolRankService _schoolRankService;
        private readonly ISchService _schService;
        private readonly IArticleCoverService _articleCoverService;
        private readonly IImportService _dataImport;
        private readonly IHistoryService _historyService;
        private readonly ISchoolCommentScoreService _commentScoreService;
        private readonly ISchoolInfoService _schoolInfoService;
        private ICollectionService _collectionService;

        private readonly ILSRFSchoolDetailService _lSRFSchoolDetail;
        private readonly ISchoolActivityService _schoolActivityService;

        //热门点评、学校组合方法
        PullHottestCSHelper hot;

        //private readonly string _AddRankKey = "addRank";


        public SchoolController(ISchoolService schoolService, ICityInfoService cityService, IHttpClientFactory clientFactory, IEasyRedisClient easyRedisClient, IUserService userService, IMapper mapper,
           IArticleService articleService, IQuestionInfoService questionInfoService, IUserServiceClient userServiceClient, ISearchService dataSearch, ISchoolRankService schoolRankService, IImportService dataImport,
           ISchoolCommentService schoolComment, IConfiguration iConfig, IOptions<ImageSetting> set, IHotPopularService hotPopularService, ISchService schService, OperationApiServices operationApiService, IArticleCoverService articleCoverService,
           IGiveLikeService likeservice, ISchoolCommentScoreService commentScoreService, ISchoolInfoService schoolInfoService, ICollectionService collectionService,
           ILSRFSchoolDetailService lSRFSchoolDetail, IHistoryService historyService, ISchoolActivityService schoolActivityService)
        {
            _dataSearch = dataSearch;
            _schoolService = schoolService;
            _cityService = cityService;
            _configuration = iConfig;
            _clientFactory = clientFactory;
            _easyRedisClient = easyRedisClient;
            _mapper = mapper;
            _questionInfoService = questionInfoService;
            _userService = userService;
            _articleService = articleService;
            _userServiceClient = userServiceClient;
            _commentService = schoolComment;
            _setting = set.Value;
            _hotPopularService = hotPopularService;
            _schoolRankService = schoolRankService;
            _schService = schService;
            _articleCoverService = articleCoverService;
            _dataImport = dataImport;
            _historyService = historyService;
            _commentScoreService = commentScoreService;
            _schoolInfoService = schoolInfoService;
            _collectionService = collectionService;

            _lSRFSchoolDetail = lSRFSchoolDetail;

            hot = new PullHottestCSHelper(likeservice, userService, schoolInfoService, _setting, schService);
            _schoolActivityService = schoolActivityService;
        }

        /// <summary>
        /// ajax 获取学校的划片范围
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        [Description("动态获取学校划片范围")]
        public ResponseResult ExtRange(Guid extId)
        {
            var range = _schoolService.GetSchoolRange(extId);
            return ResponseResult.Success(range);
        }


        /// <summary>
        /// 学校列表页
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [Description("学校列表页")]
        [Route("school")]
        [Route("school/{option}/pg{pg}")]
        [Route("school/{option}")]

        public async Task<IActionResult> List(string option, string KeyWords, StatRange range,string top10, int pg = 1, int city = 0, int debug = 0)
        {
            ViewBag.Debug = debug != 0;
            int pageSize = 10;
            //获取当前的城市
            var localCityCode = city == 0 ? Request.GetLocalCity() : city;
            var UUID = Request.GetDevice();

            List<CityAreaDto> cityAreas = new List<CityAreaDto>();
            List<AreaDto> areas = new List<AreaDto>();
            List<MetroDto> metros = new List<MetroDto>();
            List<GradeTypeDto> types = new List<GradeTypeDto>();
            List<TagDto> tags = new List<TagDto>();

            var cityOption = GetCityOption(option);

            Task t1 = Task.Run(async () => cityAreas = !cityOption.Item1.Any() ? new List<CityAreaDto>() : await _cityService.GetAreaCode(cityOption.Item1));
            Task t2 = Task.Run(async () => metros = cityOption.Item2 == null ? new List<MetroDto>() : await _cityService.GetMetroList(cityOption.Item2 ?? 0));
            Task t3 = Task.Run(() => types = _schoolService.GetGradeTypeList());
            Task t4 = Task.Run(() => tags = _schoolService.GetTagList());
            await Task.WhenAll(t1, t3, t2, t4);
            areas = cityAreas.FirstOrDefault(q => q.CityCode == cityOption.Item2)?.Areas ?? new List<AreaDto>();

            SchoolExtFilterData query = string.IsNullOrWhiteSpace(option) ?
                new SchoolExtFilterData() : GetQueryData(option, pg, pageSize, metros, tags);

            if (!string.IsNullOrWhiteSpace(KeyWords))
                query.KeyWords = KeyWords;

            string UA = Request.Headers["User-Agent"].ToString();
            if (!string.IsNullOrWhiteSpace(query.KeyWords) && !UA.ToLower().Contains("spider") && !UA.ToLower().Contains("bot") && Request.GetAvaliableCity() != 0)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user = User.Identity.GetUserInfo();
                    _dataImport.SetHistory("PC", 0, query.KeyWords, Request.GetAvaliableCity(), user.UserId, UUID);
                }
                else
                {
                    _dataImport.SetHistory("PC", 0, query.KeyWords, Request.GetAvaliableCity(), null, UUID);
                }
            }

            if (range != null)
            {
                if (range.MinCost != null) query.MinCost = range.MinCost;
                if (range.MaxCost != null) query.MaxCost = range.MaxCost;
                if (range.MinStudent != null) query.MinStudent = range.MinStudent;
                if (range.MaxStudent != null) query.MaxStudent = range.MaxStudent;
                if (range.MinTeacher != null) query.MinTeacher = range.MinTeacher;
                if (range.MaxTeacher != null) query.MaxTeacher = range.MaxTeacher;
                if (range.MinCScore != null) query.MinCScore = range.MinCScore;
                if (range.MaxCScore != null) query.MaxCScore = range.MaxCScore;
                if (range.MinCtScore != null) query.MinCtScore = range.MinCtScore;
                if (range.MaxCtScore != null) query.MaxCtScore = range.MaxCtScore;
                if (range.MinCuScore != null) query.MinCuScore = range.MinCuScore;
                if (range.MaxCuScore != null) query.MaxCuScore = range.MaxCuScore;

                if (range.MinHScore != null) query.MinHScore = range.MinHScore;
                if (range.MaxHScore != null) query.MaxHScore = range.MaxHScore;
                if (range.MinLScore != null) query.MinLScore = range.MinLScore;
                if (range.MaxLScore != null) query.MaxLScore = range.MaxLScore;
                if (range.MinTotal != null) query.MinTotal = range.MinTotal;
                if (range.MaxTotal != null) query.MaxTotal = range.MaxTotal;
                if (range.MinTScore != null) query.MinTScore = range.MinTScore;
                if (range.MaxTScore != null) query.MaxTScore = range.MaxTScore;
            }

            List<Guid> characids = query.CharacIds;
            ViewBag.characids = characids.Select(q => q.ToString()).ToList();

            var options = await GetExtOption(query, areas, metros, tags, cityAreas, types, localCityCode);

            var newOption = GetOption(query, metros, tags);
            var list = await GetExtList(query, newOption, localCityCode, top10);

            //if (!string.IsNullOrWhiteSpace(json) && json.ToLower()=="yes")
            if (Request.IsAjax())
            {
                var stats = await GetExtStats(query, city, metros, tags);

                if (list.List?.Any() == true)
                {
                    foreach (var item in list.List)
                    {
                        item.Url = $"/school-{item.ShortSchoolNo.ToLower()}/";
                    }
                }

                return Json(ResponseResult.Success(new
                {
                    options,
                    list,
                    stats
                }));
            }
            else
            {
                ViewBag.KeyWords = query.KeyWords;

                ViewBag.LocalCity = options.LocalCity;


                ViewBag.HotCity = options.HotCity;

                ViewBag.Controller = RouteData.Values["controller"]?.ToString();
                ViewBag.Action = RouteData.Values["action"]?.ToString();

                var queryDic = Request.Query.ToDictionary(kv => kv.Key.ToLower(), kv =>
                {
                    var v = kv.Value.ToString();
                    if (Guid.TryParse(v, out var gid)) return gid.ToString("n");
                    return v;
                });
                queryDic.Remove("city");
                queryDic.Remove("citycode");
                queryDic.Remove("keywords");
                queryDic.Remove("areacodes");
                queryDic.Remove("metroselect");
                queryDic.Remove("metrolineids");
                queryDic.Remove("metrostationids");
                ViewBag.Query = queryDic;
                ViewBag.SchoolTypeList = _schoolService.GetSchoolTypeList();


                ViewBag.SelectCity = options.HotCity;


                //ViewBag.OptionHref = await GetExtOptionHref(query, areas, metros, tags, cityAreas);


                ViewBag.Options = options;

                ViewBag.SelectStat = new StatRange
                {
                    MinCost = query.MinCost,
                    MaxCost = query.MaxCost,

                    MinStudent = query.MinStudent,
                    MaxStudent = query.MaxStudent,

                    MinTeacher = query.MinTeacher,
                    MaxTeacher = query.MaxTeacher,

                    MinCScore = query.MinCScore,
                    MaxCScore = query.MaxCScore,

                    MinTScore = query.MinTScore,
                    MaxTScore = query.MaxTScore,

                    MinHScore = query.MinHScore,
                    MaxHScore = query.MaxHScore,

                    MinCuScore = query.MinCuScore,
                    MaxCuScore = query.MaxCuScore,

                    MinLScore = query.MinLScore,
                    MaxLScore = query.MaxLScore,

                    MinCtScore = query.MinCtScore,
                    MaxCtScore = query.MaxCtScore,

                    MinTotal = query.MinTotal,
                    MaxTotal = query.MaxTotal
                };

                ViewBag.SchoolExts = list;

                //用户id
                Guid UserId = Guid.Empty;
                if (User.Identity.IsAuthenticated)
                {
                    UserId = User.Identity.GetId();
                }
                //0.学校  1.点评  2.问答  3.排行榜  4.文章
                ViewBag.HotSchoolWord = _dataSearch.GetHotHistoryList(0, 3 ,null ,localCityCode);

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
                    EndTime = date_Now
                }, UserId), UserId);

                //热问 -> 只从Redis获取数据
                ViewBag.HottestSchoolQuesitonView = hot.HotQuestion(await _questionInfoService.GetHotQuestion());

                return View();
            }
        }

        private (List<int>, int?) GetCityOption(string option)
        {
            if (string.IsNullOrWhiteSpace(option))
            {
                return (new List<int>(), null);
            }

            int? SelectCity = null;
            List<int> cityCode = new List<int>();
            List<int> areaCode = new List<int>();

            string patternCity = @"(?<=ci)\d+";
            var regCity = new Regex(patternCity);
            var cityMatch = regCity.Matches(option);
            foreach (var item in cityMatch.Select(q => q.Value))
            {
                cityCode.Add(Convert.ToInt32(item));
            }


            string patternArea = @"(?<=ar)\d+";
            var regArea = new Regex(patternArea);
            var areaMatch = regArea.Matches(option);
            foreach (var item in areaMatch.Select(q => q.Value))
            {
                int code = Convert.ToInt32(item);
                areaCode.Add(code);
                //因为有些城市没有区，下一级就是镇，镇的编号有9位，因此区推导出市编号去掉后三位补0，镇推导出市编号去掉后三位
                var c = code / 1000000 == 0 ? code / 100 * 100 : code / 1000;
                cityCode.Add(c);
            }
            cityCode = cityCode.GroupBy(q => q).Select(p => p.Key).ToList();
            if (cityCode.Count() == 1)
            {
                SelectCity = cityCode[0];
            }
            else if (cityCode.Any())
            {
                string patternSelectCity = @"(?<=cs)\d+";
                var csMatch = new Regex(patternSelectCity).Match(option);
                if (!string.IsNullOrWhiteSpace(csMatch.Value))
                    SelectCity = Convert.ToInt32(csMatch.Value);
            }
            return (cityCode, SelectCity);
        }


        private SchoolExtFilterData GetQueryData(string option, int pageNo, int pageSize, List<MetroDto> metros, List<TagDto> tags)
        {
            //选择： at选择区域类型 cs选择城市  ms选择地铁线路  gs选择年级
            //搜索关键词 kw-
            //排序 ob

            //城市部分
            //选择单城市:如：ci440100   选择城市下的区域时省略城市ID 如： ar440101
            //选择多城市:如：ci440100ci440200 选择某城市下的区域时省略城市ID 如： ci440200ar440101 或 ar440103ar440201

            //选择城市地铁：因为学校列表的地铁只能选择一个城市，选择地铁时只选最后选择的城市。地铁线路ID由原来GUID更换为纯数字ID，
            //GUID更换为对应数字ID或者按城市编码（1、2、3、4……）
            //选择单线路时：ci440100me9 单站点 ci440100me9st5912
            //选择多线路时：ci440100me8me9 多站点  ci440100me8me9st5912st5817

            //学校类型：选择学段：gr1gr2 选择类型：ty1.3.0.0.0ty1.99.0.0.0 选择学段+类型：gr2ty1.3.0.0.0ty1.99.0.0.0

            //寄宿（饭堂） 有1无0 如有饭堂ca1
            //学校认证、课程设置、特色课程、出国方向原来GUID更换为纯数字ID：如au1ch1co2ab3
            var authentications = tags.Where(q => q.Type == 2).OrderBy(q => q.Sort).ToList();
            var abroads = tags.Where(q => q.Type == 3).OrderBy(q => q.Sort).ToList();
            var characteristics = tags.Where(q => q.Type == 8).OrderBy(q => q.Sort).ToList();
            var courses = tags.Where(q => q.Type == 6).OrderBy(q => q.Sort).ToList();

            string KeyWords = null;
            int AreaSelect = 1;
            int? GradeSelect = null;
            int? SelectCity = null;
            Guid? MetroSelect = null;
            int Orderby = 0;

            List<int> cityCode = new List<int>();
            List<int> areaCode = new List<int>();

            List<Guid> MetroLineIds = new List<Guid>();
            List<int> MetroStationIds = new List<int>();

            bool? Lodging = null;
            bool? Canteen = null;
            //学校认证
            List<Guid> AuthIds = new List<Guid>();
            //特色课程
            List<Guid> CharacIds = new List<Guid>();
            //出国方向
            List<Guid> AbroadIds = new List<Guid>();
            //课程
            List<Guid> CourseIds = new List<Guid>();


            string patternKeyWords = @"(?<=kw-)(.+?(?=_)|.+)";
            var regKW = new Regex(patternKeyWords);
            var kwMatch = regKW.Match(option);
            if (kwMatch != null)
                KeyWords = kwMatch.Value;

            string patternOrderby = @"(?<=ob)\d+";
            var obMatch = new Regex(patternOrderby).Match(option).Value;
            if (!string.IsNullOrWhiteSpace(obMatch))
                Orderby = Convert.ToInt32(obMatch);


            string patternCity = @"(?<=ci)\d+";
            var regCity = new Regex(patternCity);
            var cityMatch = regCity.Matches(option);
            foreach (var item in cityMatch.Select(q => q.Value))
            {
                cityCode.Add(Convert.ToInt32(item));
            }

            string patternMetro = @"ci\d+(me\d+)+(st\d+)*";
            var regMetro = new Regex(patternMetro);
            var metroMatch = regMetro.Matches(option);
            if (metroMatch.Count > 0)
            {
                AreaSelect = 2;
                //获取城市地铁
                var metroOption = metroMatch[0].Value;

                string patternLine = @"(?<=me)\d+";
                var lineMatch = new Regex(patternLine).Matches(metroOption);
                foreach (var item in lineMatch.Select(q => q.Value))
                {
                    var metroNo = Convert.ToInt32(item);
                    MetroLineIds.Add(metros.FirstOrDefault(q => q.MetroNo == metroNo)?.MetroId ?? Guid.Empty);
                }

                string patternStation = @"(?<=st)\d+";
                var stationMatch = new Regex(patternStation).Matches(metroOption);
                foreach (var item in stationMatch.Select(q => q.Value))
                {
                    MetroStationIds.Add(Convert.ToInt32(item));
                }

                if (MetroLineIds.Count == 1)
                {
                    MetroSelect = MetroLineIds[0];
                }
                else
                {
                    string patternMetroSelect = @"(?<=ms)\d+";
                    var msMatch = new Regex(patternMetroSelect).Match(option).Value;
                    if (!string.IsNullOrWhiteSpace(msMatch))
                    {
                        var metroNo = Convert.ToInt32(msMatch);
                        MetroSelect = metros.FirstOrDefault(q => q.MetroNo == metroNo)?.MetroId ?? Guid.Empty;
                    }
                }
            }
            else
            {
                string patternArea = @"(?<=ar)\d+";
                var regArea = new Regex(patternArea);
                var areaMatch = regArea.Matches(option);
                foreach (var item in areaMatch.Select(q => q.Value))
                {
                    int code = Convert.ToInt32(item);
                    areaCode.Add(code);
                    //因为有些城市没有区，下一级就是镇，镇的编号有9位，因此区推导出市编号去掉后三位补0，镇推导出市编号去掉后三位
                    var c = code / 1000000 == 0 ? code / 100 * 100 : code / 1000;
                    cityCode.Add(c);
                }
            }
            cityCode = cityCode.GroupBy(q => q).Select(p => p.Key).ToList();
            if (cityCode.Count() == 1)
            {
                SelectCity = cityCode[0];
            }
            else if (cityCode.Any())
            {
                string patternSelectCity = @"(?<=cs)\d+";
                var csMatch = new Regex(patternSelectCity).Match(option);
                if (!string.IsNullOrWhiteSpace(csMatch.Value))
                    SelectCity = Convert.ToInt32(csMatch.Value);
            }

            List<int> grade = new List<int>();
            List<string> type = new List<string>();
            string patternGrade = @"(?<=gr)\d+";
            var gradeMatch = new Regex(patternGrade).Matches(option);
            foreach (var item in gradeMatch.Select(q => q.Value))
            {
                grade.Add(Convert.ToInt32(item));
            }
            string patternType = @"(?<=ty)lx\d+";
            var typeMatch = new Regex(patternType).Matches(option);
            foreach (var item in typeMatch.Select(q => q.Value))
            {
                string t = item.ToString();
                type.Add(t);

                //var tt = t.Split(".");
                grade.Add(new SchFType0(t).Grade);
            }
            grade = grade.GroupBy(q => q).Select(p => p.Key).ToList();

            if (grade.Count() == 1)
            {
                GradeSelect = grade[0];
            }
            else if (grade.Any())
            {
                string patternGradeSelect = @"(?<=gs)\d+";
                var gsMatch = new Regex(patternGradeSelect).Match(option);
                if (!string.IsNullOrWhiteSpace(gsMatch.Value))
                    GradeSelect = Convert.ToInt32(gsMatch.Value);
            }

            string patternLodging = @"(?<=lo)\d+";
            string patternCanteen = @"(?<=ca)\d+";
            var lodgingMatch = new Regex(patternLodging).Match(option);
            var canteenMatch = new Regex(patternCanteen).Match(option);
            if (!string.IsNullOrWhiteSpace(lodgingMatch.Value))
            {
                Lodging = lodgingMatch.ToString() != "0";
            }
            if (!string.IsNullOrWhiteSpace(canteenMatch.Value))
            {
                Canteen = canteenMatch.ToString() != "0";
            }


            string patternAuth = @"(?<=au)\d+";
            var authMatch = new Regex(patternAuth).Matches(option);
            foreach (var item in authMatch.Select(q => q.Value))
            {
                var No = Convert.ToInt32(item);
                AuthIds.Add(authentications.FirstOrDefault(q => q.No == No)?.Id ?? Guid.Empty);
            }
            string patternCharac = @"(?<=ch)\d+";
            var characMatch = new Regex(patternCharac).Matches(option);
            foreach (var item in characMatch.Select(q => q.Value))
            {
                var No = Convert.ToInt32(item);
                CharacIds.Add(characteristics.FirstOrDefault(q => q.No == No)?.Id ?? Guid.Empty);
            }
            string patternAbroad = @"(?<=ab)\d+";
            var abroadMatch = new Regex(patternAbroad).Matches(option);
            foreach (var item in abroadMatch.Select(q => q.Value))
            {
                var No = Convert.ToInt32(item);
                AbroadIds.Add(abroads.FirstOrDefault(q => q.No == No)?.Id ?? Guid.Empty);
            }
            string patternCourse = @"(?<=co)\d+";
            var courseMatch = new Regex(patternCourse).Matches(option);
            foreach (var item in courseMatch.Select(q => q.Value))
            {
                var No = Convert.ToInt32(item);
                CourseIds.Add(courses.FirstOrDefault(q => q.No == No)?.Id ?? Guid.Empty);
            }
            #region 范围
            int? MinCost = null;
            int? MaxCost = null;
            int? MinStudent = null;
            int? MaxStudent = null;
            int? MinTeacher = null;
            int? MaxTeacher = null;

            //分数
            int? MinCScore = null;
            int? MaxCScore = null;
            int? MinTScore = null;
            int? MaxTScore = null;
            int? MinHScore = null;
            int? MaxHScore = null;
            int? MinCuScore = null;
            int? MaxCuScore = null;
            int? MinLScore = null;
            int? MaxLScore = null;
            int? MinCtScore = null;
            int? MaxCtScore = null;
            int? MinTotal = null;
            int? MaxTotal = null;

            var nx = new Regex(@"(?<=nx)\d+").Match(option).Value;
            if (!string.IsNullOrWhiteSpace(nx))
                MinCost = Convert.ToInt32(nx);
            var mx = new Regex(@"(?<=mx)\d+").Match(option).Value;
            if (!string.IsNullOrWhiteSpace(mx))
                MaxCost = Convert.ToInt32(mx);

            var ny = new Regex(@"(?<=ny)\d+").Match(option).Value;
            if (!string.IsNullOrWhiteSpace(ny))
                MinStudent = Convert.ToInt32(ny);
            var my = new Regex(@"(?<=my)\d+").Match(option).Value;
            if (!string.IsNullOrWhiteSpace(my))
                MaxStudent = Convert.ToInt32(my);
            var nz = new Regex(@"(?<=nz)\d+").Match(option).Value;
            if (!string.IsNullOrWhiteSpace(nz))
                MinTeacher = Convert.ToInt32(nz);
            var mz = new Regex(@"(?<=mz)\d+").Match(option).Value;
            if (!string.IsNullOrWhiteSpace(mz))
                MaxTeacher = Convert.ToInt32(mz);

            var na = new Regex(@"(?<=na)\d+").Match(option).Value;
            if (!string.IsNullOrWhiteSpace(na))
                MinCScore = Convert.ToInt32(na);
            var ma = new Regex(@"(?<=ma)\d+").Match(option).Value;
            if (!string.IsNullOrWhiteSpace(ma))
                MaxCScore = Convert.ToInt32(ma);
            var nb = new Regex(@"(?<=nb)\d+").Match(option).Value;
            if (!string.IsNullOrWhiteSpace(nb))
                MinTScore = Convert.ToInt32(nb);
            var mb = new Regex(@"(?<=mb)\d+").Match(option).Value;
            if (!string.IsNullOrWhiteSpace(mb))
                MaxTScore = Convert.ToInt32(mb);
            var nc = new Regex(@"(?<=nc)\d+").Match(option).Value;
            if (!string.IsNullOrWhiteSpace(nc))
                MinHScore = Convert.ToInt32(nc);
            var mc = new Regex(@"(?<=my)\d+").Match(option).Value;
            if (!string.IsNullOrWhiteSpace(mc))
                MaxHScore = Convert.ToInt32(mc);
            var nd = new Regex(@"(?<=nd)\d+").Match(option).Value;
            if (!string.IsNullOrWhiteSpace(nd))
                MinCuScore = Convert.ToInt32(nd);
            var md = new Regex(@"(?<=md)\d+").Match(option).Value;
            if (!string.IsNullOrWhiteSpace(md))
                MaxCuScore = Convert.ToInt32(md);
            var nf = new Regex(@"(?<=nf)\d+").Match(option).Value;
            if (!string.IsNullOrWhiteSpace(nf))
                MinLScore = Convert.ToInt32(nf);
            var mf = new Regex(@"(?<=mf)\d+").Match(option).Value;
            if (!string.IsNullOrWhiteSpace(mf))
                MaxLScore = Convert.ToInt32(mf);
            var ng = new Regex(@"(?<=ng)\d+").Match(option).Value;
            if (!string.IsNullOrWhiteSpace(ng))
                MinCtScore = Convert.ToInt32(ng);
            var mg = new Regex(@"(?<=mg)\d+").Match(option).Value;
            if (!string.IsNullOrWhiteSpace(mg))
                MaxCtScore = Convert.ToInt32(mg);
            var nh = new Regex(@"(?<=nh)\d+").Match(option).Value;
            if (!string.IsNullOrWhiteSpace(nh))
                MinTotal = Convert.ToInt32(nh);
            var mh = new Regex(@"(?<=mh)\d+").Match(option).Value;
            if (!string.IsNullOrWhiteSpace(mh))
                MaxTotal = Convert.ToInt32(mh);
            #endregion

            int pgno = pageNo;
            var pg = new Regex(@"(?<=pg)\d+").Match(option).Value;
            if (!string.IsNullOrWhiteSpace(pg))
                pgno = Convert.ToInt32(pg);


            SchoolExtFilterData data = new SchoolExtFilterData
            {
                SelectCity = SelectCity,
                AreaSelect = AreaSelect,
                GradeSelect = GradeSelect,
                MetroSelect = MetroSelect,

                KeyWords = KeyWords,

                Grade = grade,
                Type = type,

                CityCode = cityCode,
                AreaCodes = areaCode,

                MetroLineIds = MetroLineIds,
                MetroStationIds = MetroStationIds,

                Lodging = Lodging,
                Canteen = Canteen,

                AuthIds = AuthIds,
                CharacIds = CharacIds,
                AbroadIds = AbroadIds,
                CourseIds = CourseIds,

                MinCost = MinCost,
                MaxCost = MaxCost,

                MinStudent = MinStudent,
                MaxStudent = MaxStudent,
                MinTeacher = MinTeacher,
                MaxTeacher = MaxTeacher,

                //分数
                MinCScore = MinCScore,
                MaxCScore = MaxCScore,
                MinTScore = MinTScore,
                MaxTScore = MaxTScore,
                MinHScore = MinHScore,
                MaxHScore = MaxHScore,
                MinCuScore = MinCuScore,
                MaxCuScore = MaxCuScore,
                MinLScore = MinLScore,
                MaxLScore = MaxLScore,
                MinCtScore = MinCtScore,
                MaxCtScore = MaxCtScore,
                MinTotal = MinTotal,
                MaxTotal = MaxTotal,

                Orderby = Orderby,
                PageNo = pgno,
                PageSize = pageSize
            };

            return data;
        }

        private string GetOption(SchoolExtFilterData data, List<MetroDto> metros, List<TagDto> tags)
        {
            //选择： at选择区域类型 cs选择城市  ms选择地铁线路  gs选择年级
            //搜索关键词 kw-
            //排序 ob

            //城市部分
            //选择单城市:如：ci440100   选择城市下的区域时省略城市ID 如： ar440101
            //选择多城市:如：ci440100ci440200 选择某城市下的区域时省略城市ID 如： ci440200ar440101 或 ar440103ar440201

            //选择城市地铁：因为学校列表的地铁只能选择一个城市，选择地铁时只选最后选择的城市。地铁线路ID由原来GUID更换为纯数字ID，
            //GUID更换为对应数字ID或者按城市编码（1、2、3、4……）
            //选择单线路时：ci440100me9 单站点 ci440100me9st5912
            //选择多线路时：ci440100me8me9 多站点  ci440100me8me9st5912st5817

            //学校类型：选择学段：gr1gr2 选择类型：ty1.3.0.0.0ty1.99.0.0.0 选择学段+类型：gr2ty1.3.0.0.0ty1.99.0.0.0

            //寄宿（饭堂） 有1无0 如有饭堂ca1
            //学校认证、课程设置、特色课程、出国方向原来GUID更换为纯数字ID：如au1ch1co2ab3
            var authentications = tags.Where(q => q.Type == 2).OrderBy(q => q.Sort).ToList();
            var abroads = tags.Where(q => q.Type == 3).OrderBy(q => q.Sort).ToList();
            var characteristics = tags.Where(q => q.Type == 8).OrderBy(q => q.Sort).ToList();
            var courses = tags.Where(q => q.Type == 6).OrderBy(q => q.Sort).ToList();


            List<string> optionList = new List<string>();
            StringBuilder selectOption = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(data.KeyWords))
            {
                optionList.Add("kw-" + data.KeyWords);
            }


            if (data.CityCode.Count > 0)
            {
                StringBuilder cityOption = new StringBuilder();

                if (data.CityCode.Count > 1)
                {
                    selectOption.Append("cs" + data.SelectCity);
                }

                var includeCity = data.AreaCodes.Select(q => q / 1000000 == 0 ? q / 100 * 100 : q / 1000).ToList();
                foreach (var city in data.CityCode.Where(q => !includeCity.Contains(q)))
                {
                    cityOption.Append("ci" + city);
                }

                if (data.AreaCodes.Count > 0)
                {
                    foreach (var area in data.AreaCodes)
                    {
                        cityOption.Append("ar" + area);
                    }
                }
                else if (data.MetroLineIds.Count > 0)
                {
                    if (data.MetroLineIds.Count > 1)
                    {
                        var metroNo = metros.FirstOrDefault(q => q.MetroId == data.MetroSelect)?.MetroNo ?? 0;
                        selectOption.Append("ms" + metroNo);
                    }

                    foreach (var lineId in data.MetroLineIds)
                    {
                        var metroNo = metros.FirstOrDefault(q => q.MetroId == lineId)?.MetroNo ?? 0;
                        cityOption.Append("me" + metroNo);
                    }
                    foreach (var stationId in data.MetroStationIds)
                    {
                        cityOption.Append("st" + stationId);
                    }
                }

                optionList.Add(cityOption.ToString());
            }

            if (data.Grade.Count > 0)
            {
                StringBuilder gradeOption = new StringBuilder();

                if (data.Grade.Count > 1)
                {
                    selectOption.Append("gs" + data.GradeSelect);
                }

                var includeGrade = data.Type.Select(q => (int)(new SchFType0(q)).Grade).ToList();
                foreach (var grade in data.Grade.Where(q => !includeGrade.Contains(q)))
                {
                    gradeOption.Append("gr" + grade);
                }

                if (data.Type.Count > 0)
                {
                    foreach (var type in data.Type)
                    {
                        gradeOption.Append("ty" + type);
                    }
                }

                optionList.Add(gradeOption.ToString());
            }

            if (data.Lodging != null)
            {
                optionList.Add(data.Lodging == true ? "lo1" : "lo0");
            }
            if (data.Canteen != null)
            {
                optionList.Add(data.Canteen == true ? "ca1" : "ca0");
            }

            if (data.AuthIds.Count > 0 || data.CourseIds.Count > 0 || data.AbroadIds.Count > 0 || data.CharacIds.Count > 0)
            {
                StringBuilder ttOption = new StringBuilder();

                foreach (var item in data.AuthIds)
                {
                    var no = authentications.FirstOrDefault(q => q.Id == item)?.No ?? 0;
                    ttOption.Append("au" + no);
                }
                foreach (var item in data.CourseIds)
                {
                    var no = courses.FirstOrDefault(q => q.Id == item)?.No ?? 0;
                    ttOption.Append("co" + no);
                }
                foreach (var item in data.AbroadIds)
                {
                    var no = abroads.FirstOrDefault(q => q.Id == item)?.No ?? 0;
                    ttOption.Append("ab" + no);
                }
                foreach (var item in data.CharacIds)
                {
                    var no = characteristics.FirstOrDefault(q => q.Id == item)?.No ?? 0;
                    ttOption.Append("ch" + no);
                }

                optionList.Add(ttOption.ToString());
            }

            #region 范围

            StringBuilder rangOption = new StringBuilder();
            if (data.MinCost != null)
            {
                rangOption.Append("nx" + data.MinCost.ToString());
            }
            if (data.MaxCost != null)
            {
                rangOption.Append("mx" + data.MaxCost.ToString());
            }
            if (data.MinStudent != null)
            {
                rangOption.Append("ny" + data.MinStudent.ToString());
            }
            if (data.MaxStudent != null)
            {
                rangOption.Append("my" + data.MaxStudent.ToString());
            }
            if (data.MinTeacher != null)
            {
                rangOption.Append("nz" + data.MinTeacher.ToString());
            }
            if (data.MaxTeacher != null)
            {
                rangOption.Append("mz" + data.MaxTeacher.ToString());
            }

            if (data.MinCScore != null)
            {
                rangOption.Append("na" + data.MinCScore.ToString());
            }
            if (data.MaxCScore != null)
            {
                rangOption.Append("ma" + data.MaxCScore.ToString());
            }
            if (data.MinTScore != null)
            {
                rangOption.Append("nb" + data.MinTScore.ToString());
            }
            if (data.MaxTScore != null)
            {
                rangOption.Append("mb" + data.MaxTScore.ToString());
            }
            if (data.MinHScore != null)
            {
                rangOption.Append("nc" + data.MinHScore.ToString());
            }
            if (data.MaxHScore != null)
            {
                rangOption.Append("mc" + data.MaxHScore.ToString());
            }
            if (data.MinCuScore != null)
            {
                rangOption.Append("nd" + data.MinCuScore.ToString());
            }
            if (data.MaxCuScore != null)
            {
                rangOption.Append("md" + data.MaxCuScore.ToString());
            }
            if (data.MinLScore != null)
            {
                rangOption.Append("nf" + data.MinLScore.ToString());
            }
            if (data.MaxLScore != null)
            {
                rangOption.Append("mf" + data.MaxLScore.ToString());
            }
            if (data.MinCtScore != null)
            {
                rangOption.Append("ng" + data.MinCtScore.ToString());
            }
            if (data.MaxCtScore != null)
            {
                rangOption.Append("mg" + data.MaxCtScore.ToString());
            }
            if (data.MinTotal != null)
            {
                rangOption.Append("nh" + data.MinTotal.ToString());
            }
            if (data.MaxTotal != null)
            {
                rangOption.Append("mh" + data.MaxTotal.ToString());
            }
            if (rangOption.Length > 0)
                optionList.Add(rangOption.ToString());
            #endregion
            if (data.Orderby > 0)
            {
                optionList.Add("ob" + data.Orderby);
            }

            if (selectOption.Length > 0)
            {
                optionList.Add(selectOption.ToString());
            }

            return string.Join("_", optionList);
        }


        /// <summary>
        /// 学校列表页
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [Description("学校列表页")]
        public async Task<IActionResult> ListExtSchool(SchoolExtFilterData query, int city = 0)
        {

            List<MetroDto> metros = new List<MetroDto>();

            List<TagDto> tags = new List<TagDto>();


            Task t1 = Task.Run(async () => metros = query.SelectCity == null ? new List<MetroDto>() : await _cityService.GetMetroList(query.SelectCity ?? 0));

            Task t2 = Task.Run(() => tags = _schoolService.GetTagList());
            await Task.WhenAll(t1, t2);

            var redirectUrl = ActionShortIdUrl(() => { return query; }, metros, tags);

            Response.StatusCode = 301;
            Response.Headers.Add("Location", redirectUrl);



            return View();
        }

        public class StatRange
        {
            public int? MinCost { get; set; }
            public int? MaxCost { get; set; }
            public int? MinStudent { get; set; }
            public int? MaxStudent { get; set; }
            public int? MinTeacher { get; set; }
            public int? MaxTeacher { get; set; }
            public int? MinCScore { get; set; }
            public int? MaxCScore { get; set; }
            public int? MinTScore { get; set; }
            public int? MaxTScore { get; set; }
            public int? MinHScore { get; set; }
            public int? MaxHScore { get; set; }
            public int? MinCuScore { get; set; }
            public int? MaxCuScore { get; set; }
            public int? MinLScore { get; set; }
            public int? MaxLScore { get; set; }
            public int? MinCtScore { get; set; }
            public int? MaxCtScore { get; set; }
            public int? MinTotal { get; set; }
            public int? MaxTotal { get; set; }
        }

        private async Task<SchoolExtListViewModel> GetExtList(SchoolExtFilterData query, string option, int localCityCode = 0 ,string top10 = null)
        {
            var lat = Request.GetLatitude();
            var lng = Request.GetLongitude();

            var includeGrade = query.Type.Select(q => q[0].ToString()).GroupBy(q => q).Select(q => q.Key).ToList();
            //query.Type.AddRange(query.Grade.Where(q=> !includeGrade.Any(p =>p == q.ToString())).Select(q=>q.ToString()));


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

            //top10学校的逻辑：按照当前城市指定分数维度倒序取前10的学校
            if (!string.IsNullOrEmpty(top10))
            {
                query.PageNo = 1;
                query.PageSize = 10;
                if (query.CityCode.Count == 0)
                {
                    query.CityCode = new List<int> { localCityCode };
                }
                switch (top10.ToLower())
                {
                    case "teach": query.Orderby = 6; break;
                    case "hard": query.Orderby = 7; break;
                    case "cost": query.Orderby = 3; break;
                    case "envir": query.Orderby = 9; break;
                }
            }

            var schoolQueryData = new SearchSchoolQuery
            {
                Keyword = query.KeyWords,
                Latitude = double.Parse(lat),
                Longitude = double.Parse(lng),
                CurrentCity = localCityCode,
                CityCode = query.CityCode,
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

                Lodging = query.Lodging == null ? 3 : query.Lodging == true ? 2 : 1,
                Canteen = query.Canteen,
                AuthIds = query.AuthIds,
                CharacIds = query.CharacIds,
                AbroadIds = query.AbroadIds,
                CourseIds = query.CourseIds,
                Orderby = query.Orderby,
                PageNo = query.PageNo,
                PageSize = query.PageSize
            };

            var list = _dataSearch.SearchSchool(schoolQueryData);

            var ids = list.Schools.Select(q => q.Id).ToList();

            var result = ids.Count > 0 ? await _schoolService.ListExtSchoolByBranchIds(ids) : new List<SchoolExtFilterDto>();


            //请求点评数据,榜单数据
            //拼接数据
            var rank = _schoolRankService.GetH5SchoolRankInfoBy(result.Select(p => p.ExtId).ToArray());
            var comment = _commentService.GetSchoolCardComment(result.Select(p => p.ExtId).ToList(),
                PMS.CommentsManage.Domain.Common.SchoolSectionOrIds.SchoolSectionIds, default(Guid));

            var data = list.Schools.Where(q => result.Any(p => p.ExtId == q.Id)).Select(q =>
            {
                var r = result.FirstOrDefault(p => p.ExtId == q.Id);
                return r == null ? new SchoolExtListItemViewModel() : new SchoolExtListItemViewModel
                {
                    SchoolId = r.Sid,
                    BranchId = r.ExtId,
                    Url = SchoolDetailUrl(r.ExtId, r.Sid),
                    SchoolName = q.Name,
                    CityName = r.City,
                    AreaName = r.Area,
                    Distance = q.Distance,
                    LodgingType = (int)r.LodgingType,
                    LodgingReason = r.LodgingType.Description(),
                    Type = r.Type,
                    International = r.International,
                    Cost = r.Tuition,
                    StudentCount = r.StudentCount,
                    TeacherCount = r.TeacherCount,
                    CreationDate = r.CreationDate,
                    Tags = r.Tags,
                    Score = r.Score,
                    CommentTotal = r.CommentCount,
                    Comment = comment.FirstOrDefault(p => p.SchoolSectionId == q.Id)?.Content,
                    CommentReplyCount = comment.FirstOrDefault(p => p.SchoolSectionId == q.Id)?.ReplyCount ?? 0,
                    CommontId = q.Id,
                    CommontNo = UrlShortIdUtil.Long2Base32(comment.FirstOrDefault(p => p.SchoolSectionId == q.Id)?.No ?? 0),
                    Ranks = rank.Where(p => p.SchoolId == q.Id).Select(p => new SchoolRankInfoViewModel
                    {
                        RankId = p.RankId,
                        Cover = p.Cover,
                        Ranking = p.Ranking,
                        RankUrl = p.rank_url,
                        SchoolId = p.SchoolId,
                        Title = p.Title,
                        No = p.No
                    }).ToList(),
                    SearchScore = q.SearchScore,
                    SchoolNo = r.SchoolNo
                };
            });

            var TotalPage = Convert.ToInt64(Math.Ceiling(Convert.ToDecimal(list.Total) / query.PageSize));

            var url = string.IsNullOrWhiteSpace(option) || option.Contains("pg") ? "/school/pg{0}" : "/school/" + option + "/pg{0}";

            return new SchoolExtListViewModel
            {
                Total = (!string.IsNullOrEmpty(top10) && list.Total > 10) ? 10 : list.Total,
                PageNav = new PageNavViewModel
                {
                    TotalPage = !string.IsNullOrEmpty(top10) ? 1 : TotalPage,
                    CurrentPage = query.PageNo,
                    Url = url
                },
                List = data.ToList()
            };
        }
        private async Task<SchoolExtOptionViewModel> GetExtOption(SchoolExtFilterData query,
            List<AreaDto> areas, List<MetroDto> metros, List<TagDto> tags,
            List<CityAreaDto> cityAreas, List<GradeTypeDto> types, int localCityCode = 0)
        {
            //热门城市
            var hotCity = await _cityService.GetHotCity();

            var LocalCityName = await _cityService.GetCityName(localCityCode);



            List<decimal> rangs = new List<decimal>() {
                1000,2000,3000,4000
            };

            List<string> costs = new List<string>() {
                "1万以下","1-5万","5-10万","10万以上"
            };
            List<string> students = new List<string>() {
                "500人以下","500-1500人","1500-3000人","3000人以上"
            };
            List<string> teachers = new List<string>() {
                "100人以下","100-500人","500人以上"
            };

            var authentications = tags.Where(q => q.Type == 2).OrderBy(q => q.Sort).ToList();
            var abroads = tags.Where(q => q.Type == 3).OrderBy(q => q.Sort).ToList();
            var characteristics = tags.Where(q => q.Type == 8).OrderBy(q => q.Sort).ToList();
            var courses = tags.Where(q => q.Type == 6).OrderBy(q => q.Sort).ToList();


            List<bool> boolean = new List<bool> { true, false };
            Dictionary<int, string> orderDic = new Dictionary<int, string>
            {
                { 0, "默认排序" },{ 4, "学年总费用" }//, { 2, "距离" }
            };

            return new SchoolExtOptionViewModel
            {
                LocalCity = new SchoolExtOptionViewModel.KV
                {
                    Key = localCityCode,
                    Value = LocalCityName
                },

                SelectCity = query.SelectCity,

                SelectCitys = cityAreas.Select(q => new OptionViewModel
                {
                    Href = ActionShortIdUrl(() =>
                    {
                        var queryDate = CopyData(query);
                        queryDate.MetroSelect = null;
                        queryDate.MetroLineIds = new List<Guid>();
                        queryDate.MetroStationIds = new List<int>();
                        queryDate.Distance = null;

                        queryDate.CityCode.Remove(q.CityCode);
                        if (queryDate.AreaCodes.Any())
                        {
                            foreach (var code in q.Areas.Where(p => query.AreaCodes.Contains(p.AdCode)).Select(u => u.AdCode))
                            {
                                queryDate.AreaCodes.Remove(code);
                            }
                        }

                        if (query.SelectCity == q.CityCode)
                        {
                            if (queryDate.CityCode.Any())
                            {
                                queryDate.SelectCity = queryDate.CityCode[0];
                            }
                            else
                            {
                                queryDate.SelectCity = null;
                            }
                        }

                        return queryDate;
                    }, metros, tags),
                    Active = query.CityCode.Exists(p => p == q.CityCode),
                    OptionCode = q.CityCode.ToString(),
                    OptionName = "selectcity",
                    Title = _cityService.GetCityName(q.CityCode).Result,
                    List = q.Areas.Where(p => query.AreaCodes.Contains(p.AdCode))
                                       .Select(o => new OptionViewModel
                                       {
                                           Href = ActionShortIdUrl(() =>
                                           {
                                               var queryDate = CopyData(query);

                                               queryDate.AreaCodes.Remove(o.AdCode);
                                               return queryDate;
                                           }, metros, tags),
                                           Active = query.AreaCodes.Exists(p => p == o.AdCode),
                                           OptionCode = o.AdCode.ToString(),
                                           OptionName = "selectcity",
                                           Title = o.AreaName
                                       }).ToList()
                }).ToList(),
                AreaSelect = query.AreaSelect,
                GradeSelect = query.GradeSelect,
                MetroSelect = query.MetroSelect,

                HotCity = hotCity.Select(q => new OptionViewModel
                {
                    OptionCode = q.AdCode.ToString(),
                    Title = q.AreaName,
                    Href = ActionShortIdUrl(() =>
                    {
                        var queryDate = CopyData(query);
                        queryDate.MetroSelect = null;
                        queryDate.MetroLineIds = new List<Guid>();
                        queryDate.MetroStationIds = new List<int>();

                        queryDate.SelectCity = q.AdCode;

                        if (!queryDate.CityCode.Exists(p => p == q.AdCode))
                        {
                            queryDate.CityCode.Add(q.AdCode);
                        }
                        return queryDate;
                    }, metros, tags)
                }).ToList(),

                Order = orderDic.Select(q => new OptionViewModel
                {
                    Href = ActionShortIdUrl(() =>
                    {
                        var queryDate = CopyData(query);
                        queryDate.Orderby = q.Key;
                        return queryDate;
                    }, metros, tags),
                    Active = query.Orderby == q.Key,
                    OptionCode = q.Key.ToString(),
                    OptionName = "order",
                    Title = q.Value
                }).ToList(),

                Areas = areas.Select(q => new OptionViewModel
                {
                    Href = ActionShortIdUrl(() =>
                    {
                        var queryDate = CopyData(query);
                        queryDate.AreaSelect = 1;
                        queryDate.MetroSelect = null;
                        queryDate.MetroLineIds = new List<Guid>();
                        queryDate.MetroStationIds = new List<int>();
                        queryDate.Distance = null;
                        if (queryDate.AreaCodes.Exists(p => p == q.AdCode))
                        {
                            queryDate.AreaCodes.Remove(q.AdCode);
                        }
                        else
                        {
                            queryDate.AreaCodes.Add(q.AdCode);
                        }
                        return queryDate;
                    }, metros, tags),
                    Active = query.AreaCodes.Exists(p => p == q.AdCode),
                    OptionCode = q.AdCode.ToString(),
                    OptionName = "areaCode",
                    Title = q.AreaName
                }).ToList(),

                Metros = metros.Select(q => new OptionViewModel
                {
                    Href = ActionShortIdUrl(() =>
                    {
                        var queryDate = CopyData(query);
                        queryDate.AreaSelect = 2;
                        queryDate.Distance = null;
                        queryDate.AreaCodes = new List<int>();
                        queryDate.CityCode = queryDate.SelectCity == null ? new List<int>() : new List<int> { queryDate.SelectCity ?? 0 };
                        if (queryDate.MetroLineIds.Exists(p => p == q.MetroId))
                        {
                            queryDate.MetroSelect = q.MetroId;
                        }
                        else
                        {
                            queryDate.MetroSelect = q.MetroId;
                            queryDate.MetroLineIds.Add(q.MetroId);
                        }
                        return queryDate;
                    }, metros, tags),
                    Title = q.MetroName,
                    OptionCode = q.MetroId.ToString(),
                    OptionName = "metroid",
                    Active = query.MetroLineIds.Exists(p => p == q.MetroId),
                    List = q.MetroStations.Select(p => new OptionViewModel
                    {
                        Href = ActionShortIdUrl(() =>
                        {
                            var queryDate = CopyData(query);
                            queryDate.AreaSelect = 2;
                            if (!queryDate.MetroLineIds.Contains(q.MetroId))
                            {
                                queryDate.MetroSelect = q.MetroId;
                                queryDate.MetroLineIds.Add(q.MetroId);
                            }
                            if (queryDate.MetroStationIds.Exists(o => o == p.Id))
                            {
                                queryDate.MetroStationIds.Remove(p.Id);
                            }
                            else
                            {
                                queryDate.MetroStationIds.Add(p.Id);
                            }
                            return queryDate;
                        }, metros, tags),
                        Title = p.Name,
                        Active = query.MetroStationIds.Exists(o => o == p.Id),
                        OptionName = "stationid",
                        OptionCode = p.Id.ToString()
                    }).ToList(),
                    RemoveHref = ActionShortIdUrl(() =>
                    {
                        var queryDate = CopyData(query);
                        queryDate.AreaSelect = 2;
                        queryDate.Distance = null;
                        queryDate.AreaCodes = new List<int>();
                        queryDate.CityCode = queryDate.SelectCity == null ? new List<int>() : new List<int> { queryDate.SelectCity ?? 0 };
                        if (queryDate.MetroLineIds.Exists(p => p == q.MetroId))
                        {
                            queryDate.MetroLineIds.Remove(q.MetroId);
                            queryDate.MetroSelect = queryDate.MetroLineIds.Where(p => p != q.MetroId).FirstOrDefault();
                        }
                        return queryDate;
                    }, metros, tags),
                }).ToList(),


                //Ranges = rangs.Select(q => new OptionViewModel
                //{
                //    Href = ActionShortIdUrl(() => {
                //        var queryDate = CopyData(query);
                //        queryDate.AreaSelect = 3;
                //        queryDate.MetroSelect = null;
                //        queryDate.MetroLineIds = new List<Guid>();
                //        queryDate.MetroStationIds = new List<int>();
                //        queryDate.AreaCodes = new List<int>();
                //        if (q != query.Distance)
                //        {
                //            queryDate.Distance = q;
                //        }
                //        else
                //        {
                //            queryDate.Distance = null;
                //        }
                //        return queryDate;
                //    }),
                //    Active = q == query.Distance,
                //    OptionCode = rangs.IndexOf(q).ToString(),
                //    OptionName = "distance",
                //    Title = q.ToString() +"m"
                //}).ToList(),

                TypeOption = types.Select(q => new OptionViewModel
                {
                    Href = ActionShortIdUrl(() =>
                    {
                        var queryDate = CopyData(query);
                        if (queryDate.Grade.Contains(q.GradeId))
                        {
                            queryDate.GradeSelect = q.GradeId;
                        }
                        else
                        {
                            queryDate.GradeSelect = q.GradeId;
                            queryDate.Grade.Add(q.GradeId);
                        }
                        return queryDate;
                    }, metros, tags),
                    Title = q.GradeDesc,
                    OptionCode = q.GradeId.ToString(),
                    OptionName = "grade",
                    Active = query.Grade.Contains(q.GradeId),
                    List = q.Types.Select(p => new OptionViewModel
                    {
                        Href = ActionShortIdUrl(() =>
                        {
                            var queryDate = CopyData(query);
                            if (!queryDate.Grade.Contains(q.GradeId))
                            {
                                queryDate.GradeSelect = q.GradeId;
                                queryDate.Grade.Add(q.GradeId);
                            }
                            if (queryDate.Type.Exists(o => o == p.TypeValue))
                            {
                                queryDate.Type.Remove(p.TypeValue);
                            }
                            else
                            {
                                queryDate.Type.Add(p.TypeValue);
                            }
                            return queryDate;
                        }, metros, tags),
                        Title = p.TypeName,
                        Active = query.Type.Exists(o => o == p.TypeValue),
                        OptionName = "gradeType",
                        OptionCode = p.TypeValue
                    }).ToList(),
                    RemoveHref = ActionShortIdUrl(() =>
                    {
                        var queryDate = CopyData(query);
                        if (queryDate.Grade.Exists(p => p == q.GradeId))
                        {
                            queryDate.Grade.Remove(q.GradeId);
                            queryDate.GradeSelect = queryDate.Grade.Where(p => p != q.GradeId).FirstOrDefault();
                        }
                        return queryDate;
                    }, metros, tags),
                }).ToList(),


                Lodging = boolean.Select(q => new OptionViewModel
                {
                    OptionName = "lodging",
                    OptionCode = q ? "1" : "0",
                    Active = query.Lodging != null && query.Lodging == q,
                    Title = q ? "寄读" : "走读",
                    Href = ActionShortIdUrl(() =>
                    {
                        var queryDate = CopyData(query);
                        if (query.Lodging == null || query.Lodging != q)
                        {
                            queryDate.Lodging = q;
                        }
                        else
                        {
                            queryDate.Lodging = null;
                        }
                        return queryDate;
                    }, metros, tags)
                }).ToList(),
                Canteen = boolean.Select(q => new OptionViewModel
                {
                    OptionName = "canteen",
                    OptionCode = q ? "1" : "0",
                    Active = query.Canteen != null && query.Canteen == q,
                    Title = q ? "有" : "无",
                    Href = ActionShortIdUrl(() =>
                    {
                        var queryDate = CopyData(query);
                        if (query.Canteen == null || query.Canteen != q)
                        {
                            queryDate.Canteen = q;
                        }
                        else
                        {
                            queryDate.Canteen = null;
                        }
                        return queryDate;
                    }, metros, tags)
                }).ToList(),


                AuthIds = authentications.Select(q => new OptionViewModel
                {
                    Href = ActionShortIdUrl(() =>
                    {
                        var queryDate = CopyData(query);
                        if (queryDate.AuthIds.Exists(p => p == q.Id))
                        {
                            queryDate.AuthIds.Remove(q.Id);
                        }
                        else
                        {
                            queryDate.AuthIds.Add(q.Id);
                        }
                        return queryDate;
                    }, metros, tags),
                    Active = query.AuthIds.Exists(p => p == q.Id),
                    OptionCode = q.Id.ToString(),
                    OptionName = "authids",
                    Title = q.Name
                }).ToList(),
                AbroadIds = abroads.Select(q => new OptionViewModel
                {
                    Href = ActionShortIdUrl(() =>
                    {
                        var queryDate = CopyData(query);
                        if (queryDate.AbroadIds.Exists(p => p == q.Id))
                        {
                            queryDate.AbroadIds.Remove(q.Id);
                        }
                        else
                        {
                            queryDate.AbroadIds.Add(q.Id);
                        }
                        return queryDate;
                    }, metros, tags),
                    Active = query.AbroadIds.Exists(p => p == q.Id),
                    OptionCode = q.Id.ToString(),
                    OptionName = "abroadids",
                    Title = q.Name
                }).ToList(),
                CharacIds = characteristics.Select(q => new OptionViewModel
                {
                    Href = ActionShortIdUrl(() =>
                    {
                        var queryDate = CopyData(query);
                        if (queryDate.CharacIds.Exists(p => p == q.Id))
                        {
                            queryDate.CharacIds.Remove(q.Id);
                        }
                        else
                        {
                            queryDate.CharacIds.Add(q.Id);
                        }
                        return queryDate;
                    }, metros, tags),
                    Active = query.CharacIds.Exists(p => p == q.Id),
                    OptionCode = q.Id.ToString(),
                    OptionName = "characids",
                    Title = q.Name
                }).ToList(),
                CourseIds = courses.Select(q => new OptionViewModel
                {
                    Href = ActionShortIdUrl(() =>
                    {
                        var queryDate = CopyData(query);
                        if (queryDate.CourseIds.Exists(p => p == q.Id))
                        {
                            queryDate.CourseIds.Remove(q.Id);
                        }
                        else
                        {
                            queryDate.CourseIds.Add(q.Id);
                        }
                        return queryDate;
                    }, metros, tags),
                    Active = query.CourseIds.Exists(p => p == q.Id),
                    OptionCode = q.Id.ToString(),
                    OptionName = "courseids",
                    Title = q.Name
                }).ToList(),


                ClearGrade = new OptionViewModel
                {
                    Title = "清空条件",
                    Href = ActionShortIdUrl(() =>
                    {
                        var queryDate = CopyData(query);
                        queryDate.GradeSelect = null;
                        queryDate.Grade = new List<int>();
                        queryDate.Type = new List<string>();
                        return queryDate;
                    }, metros, tags)
                },
                ClearArea = new OptionViewModel
                {
                    Title = "清空条件",
                    Href = ActionShortIdUrl(() =>
                    {
                        var queryDate = CopyData(query);
                        queryDate.CityCode = new List<int>();
                        queryDate.AreaCodes = new List<int>();
                        queryDate.MetroLineIds = new List<Guid>();
                        queryDate.MetroStationIds = new List<int>();
                        queryDate.Distance = null;
                        return queryDate;
                    }, metros, tags)
                },
                //ClearMetro = new OptionViewModel
                //{
                //    Title = "清空条件",
                //    Href = ActionShortIdUrl(() => {
                //        var queryDate = CopyData(query);
                //        queryDate.AreaCodes = new List<int>();
                //        queryDate.MetroLineIds = new List<Guid>();
                //        queryDate.MetroStationIds = new List<int>();
                //        queryDate.Distance = null;
                //        return queryDate;
                //    })
                //},
                ClearAll = new OptionViewModel
                {
                    Title = "清空条件",
                    Href = ActionShortIdUrl(() =>
                    {
                        var queryDate = CopyData(query);
                        queryDate.CityCode = new List<int>();
                        queryDate.SelectCity = null;
                        queryDate.MinStudent = null;
                        queryDate.MaxStudent = null;
                        queryDate.MinTeacher = null;
                        queryDate.MaxTeacher = null;

                        queryDate.MinCScore = null;
                        queryDate.MaxCScore = null;
                        queryDate.MinCtScore = null;
                        queryDate.MaxCtScore = null;
                        queryDate.MinCuScore = null;
                        queryDate.MaxCuScore = null;
                        queryDate.MinHScore = null;
                        queryDate.MaxHScore = null;
                        queryDate.MinLScore = null;
                        queryDate.MaxLScore = null;
                        queryDate.MinTScore = null;
                        queryDate.MaxTScore = null;

                        queryDate.GradeSelect = null;
                        queryDate.MinCost = null;
                        queryDate.MaxCost = null;
                        queryDate.Grade = new List<int>();
                        queryDate.Type = new List<string>();
                        queryDate.AreaSelect = 1;
                        queryDate.AreaCodes = new List<int>();
                        queryDate.MetroLineIds = new List<Guid>();
                        queryDate.MetroStationIds = new List<int>();
                        queryDate.Distance = null;
                        queryDate.Orderby = 0;
                        queryDate.Lodging = null;
                        queryDate.Canteen = null;

                        queryDate.CourseIds = new List<Guid>();
                        queryDate.AbroadIds = new List<Guid>();
                        queryDate.AuthIds = new List<Guid>();
                        queryDate.CharacIds = new List<Guid>();

                        return queryDate;
                    }, metros, tags)
                },
                Costs = new List<OptionViewModel>
                {
                    new OptionViewModel{ Href = ActionShortIdUrl(() => {
                        var queryDate = CopyData(query);
                        queryDate.MinCost = 0;
                        queryDate.MaxCost = 10000;
                        return queryDate;
                    }, metros, tags),
                    Active = (query.MinCost == 0 &&  query.MaxCost ==10000),
                    OptionCode = "0,10000",
                    OptionName = "cost",
                    Title = "1万以下"
                    },
                    new OptionViewModel{ Href = ActionShortIdUrl(() => {
                        var queryDate = CopyData(query);
                        queryDate.MinCost = 10000;
                        queryDate.MaxCost = 50000;
                        return queryDate;
                    }, metros, tags),
                    Active = (query.MinCost == 10000 &&  query.MaxCost ==50000),
                    OptionCode = "10000,50000",
                    OptionName = "cost",
                    Title = "1-5万"
                    },
                    new OptionViewModel{ Href = ActionShortIdUrl(() => {
                        var queryDate = CopyData(query);
                        queryDate.MinCost = 50000;
                        queryDate.MaxCost = 100000;
                        return queryDate;
                    }, metros, tags),
                    Active = (query.MinCost == 50000 &&  query.MaxCost ==100000),
                    OptionCode = "50000,100000",
                    OptionName = "cost",
                    Title = "5-10万"
                    },
                    new OptionViewModel{ Href = ActionShortIdUrl(() => {
                        var queryDate = CopyData(query);
                        queryDate.MinCost = 100000;
                        queryDate.MaxCost = null;
                        return queryDate;
                    }, metros, tags),
                    Active = (query.MinCost == 100000 && query.MaxCost == null),
                    OptionCode = "100000,*",
                    OptionName = "cost",
                    Title = "10万以上"
                    },
                },
                Students = new List<OptionViewModel>
                {
                    new OptionViewModel{ Href = ActionShortIdUrl(() => {
                        var queryDate = CopyData(query);
                        queryDate.MinStudent = 0;
                        queryDate.MaxStudent = 500;
                        return queryDate;
                    }, metros, tags),
                    Active = (query.MinStudent == 0 &&  query.MaxStudent ==500),
                    OptionCode = "0,500",
                    OptionName = "student",
                    Title = "500人以下"
                    },
                    new OptionViewModel{ Href = ActionShortIdUrl(() => {
                        var queryDate = CopyData(query);
                        queryDate.MinStudent = 500;
                        queryDate.MaxStudent = 1500;
                        return queryDate;
                    }, metros, tags),
                    Active = (query.MinStudent == 500 &&  query.MaxStudent ==1500),
                    OptionCode = "500,1500",
                    OptionName = "student",
                    Title = "500-1500人"
                    },
                    new OptionViewModel{ Href = ActionShortIdUrl(() => {
                        var queryDate = CopyData(query);
                        queryDate.MinStudent = 1500;
                        queryDate.MaxStudent = 3000;
                        return queryDate;
                    }, metros, tags),
                    Active = (query.MinStudent == 1500 &&  query.MaxStudent ==3000),
                    OptionCode = "1500,3000",
                    OptionName = "student",
                    Title = "1500-3000人"
                    },
                    new OptionViewModel{ Href = ActionShortIdUrl(() => {
                        var queryDate = CopyData(query);
                        queryDate.MinStudent = 3000;
                        queryDate.MaxStudent = null;
                        return queryDate;
                    }, metros, tags),
                    Active = (query.MinStudent == 3000 && query.MaxStudent == null),
                    OptionCode = "3000,*",
                    OptionName = "student",
                    Title = "3000人以上"
                    },
                },
                Teachers = new List<OptionViewModel>
                {
                    new OptionViewModel{ Href = ActionShortIdUrl(() => {
                        var queryDate = CopyData(query);
                        queryDate.MinTeacher = 0;
                        queryDate.MaxTeacher = 100;
                        return queryDate;
                    }, metros, tags),
                    Active = (query.MinTeacher == 0 &&  query.MaxTeacher ==100),
                    OptionCode = "0,100",
                    OptionName = "Teacher",
                    Title = "100人以下"
                    },
                    new OptionViewModel{ Href = ActionShortIdUrl(() => {
                        var queryDate = CopyData(query);
                        queryDate.MinTeacher = 100;
                        queryDate.MaxTeacher = 500;
                        return queryDate;
                    }, metros, tags),
                    Active = (query.MinTeacher == 100 &&  query.MaxTeacher ==500),
                    OptionCode = "100,500",
                    OptionName = "Teacher",
                    Title = "100-500人"
                    },
                    new OptionViewModel{ Href = ActionShortIdUrl(() => {
                        var queryDate = CopyData(query);
                        queryDate.MinTeacher = 500;
                        queryDate.MaxTeacher = null;
                        return queryDate;
                    }, metros, tags),
                    Active = (query.MinTeacher == 500 && query.MaxTeacher == null),
                    OptionCode = "500,*",
                    OptionName = "Teacher",
                    Title = "500人以上"
                    }
                }
            };
        }

        private async Task<SchoolExtListHrefViewModel> GetExtOptionHref(SchoolExtFilterData query, List<AreaDto> areas, List<MetroDto> metros, List<TagDto> tags,
            List<CityAreaDto> cityAreas)
        {
            //热门城市
            var hotCity = await _cityService.GetHotCity();

            var authentications = tags.Where(q => q.Type == 2).OrderBy(q => q.Sort).ToList();
            var abroads = tags.Where(q => q.Type == 3).OrderBy(q => q.Sort).ToList();
            var characteristics = tags.Where(q => q.Type == 8).OrderBy(q => q.Sort).ToList();
            var courses = tags.Where(q => q.Type == 6).OrderBy(q => q.Sort).ToList();

            return new SchoolExtListHrefViewModel
            {
                SelectCity = query.SelectCity,

                SelectCitys = cityAreas.Select(q => new SchoolExtListHrefViewModel.HrefViewModel
                {
                    Href = ActionShortIdUrl(() =>
                    {
                        var queryDate = CopyData(query);
                        queryDate.MetroSelect = null;
                        queryDate.MetroLineIds = new List<Guid>();
                        queryDate.MetroStationIds = new List<int>();
                        queryDate.Distance = null;

                        queryDate.CityCode = new List<int>() { q.CityCode };
                        return queryDate;
                    }, metros, tags),
                    Id = q.CityCode.ToString()
                }).ToList(),
                HotCity = hotCity.Select(q => new SchoolExtListHrefViewModel.HrefViewModel
                {
                    Href = ActionShortIdUrl(() =>
                    {
                        var queryDate = CopyData(query);
                        queryDate.MetroSelect = null;
                        queryDate.MetroLineIds = new List<Guid>();
                        queryDate.MetroStationIds = new List<int>();

                        queryDate.SelectCity = q.AdCode;

                        queryDate.CityCode = new List<int>() { q.AdCode };
                        return queryDate;
                    }, metros, tags),
                    Id = q.AdCode.ToString()
                }).ToList(),


                Areas = areas.Select(q => new SchoolExtListHrefViewModel.HrefViewModel
                {
                    Href = ActionShortIdUrl(() =>
                    {
                        var queryDate = CopyData(query);
                        queryDate.AreaSelect = 1;
                        queryDate.MetroSelect = null;
                        queryDate.MetroLineIds = new List<Guid>();
                        queryDate.MetroStationIds = new List<int>();
                        queryDate.Distance = null;

                        queryDate.AreaCodes = new List<int>() { q.AdCode };
                        return queryDate;
                    }, metros, tags),
                    Id = q.AdCode.ToString()
                }).ToList(),

                Metros = metros.Select(q => new SchoolExtListHrefViewModel.HrefViewModel
                {
                    Href = ActionShortIdUrl(() =>
                    {
                        var queryDate = CopyData(query);
                        queryDate.AreaSelect = 2;
                        queryDate.Distance = null;
                        queryDate.AreaCodes = new List<int>();
                        queryDate.CityCode = queryDate.SelectCity == null ? new List<int>() : new List<int> { queryDate.SelectCity ?? 0 };

                        queryDate.MetroLineIds = new List<Guid>() { q.MetroId };
                        return queryDate;
                    }, metros, tags),
                    Id = q.MetroId.ToString(),
                    List = q.MetroStations.Select(p => new SchoolExtListHrefViewModel.HrefViewModel
                    {
                        Href = ActionShortIdUrl(() =>
                        {
                            var queryDate = CopyData(query);
                            queryDate.AreaSelect = 2;
                            if (!queryDate.MetroLineIds.Contains(q.MetroId))
                            {
                                queryDate.MetroSelect = q.MetroId;
                                queryDate.MetroLineIds.Add(q.MetroId);
                            }

                            queryDate.MetroStationIds = new List<int>() { p.Id };
                            return queryDate;
                        }, metros, tags),
                        Id = p.Id.ToString()
                    }).ToList()
                }).ToList(),
                Tags = tags.Select(q => new SchoolExtListHrefViewModel.HrefViewModel
                {
                    Href = ActionShortIdUrl(() =>
                    {
                        var queryDate = CopyData(query);
                        queryDate.AuthIds = new List<Guid>();
                        queryDate.AbroadIds = new List<Guid>();
                        queryDate.CharacIds = new List<Guid>();
                        queryDate.CourseIds = new List<Guid>();
                        if (q.Type == 2)
                        {
                            queryDate.AuthIds.Add(q.Id);
                        }
                        else if (q.Type == 3)
                        {
                            queryDate.AbroadIds.Add(q.Id);
                        }
                        else if (q.Type == 8)
                        {
                            queryDate.CharacIds.Add(q.Id);
                        }
                        else if (q.Type == 6)
                        {
                            queryDate.CourseIds.Add(q.Id);
                        }
                        return queryDate;
                    }, metros, tags),
                    Id = q.Id.ToString()
                }).ToList()
            };
        }


        private SchoolExtFilterData CopyData(SchoolExtFilterData query)
        {
            return new SchoolExtFilterData
            {
                KeyWords = query.KeyWords,

                SelectCity = query.SelectCity,
                CityCode = query.CityCode.Select(q => q).ToList(),

                AreaSelect = query.AreaSelect,
                GradeSelect = query.GradeSelect,
                Grade = query.Grade.Select(q => q).ToList(),
                AreaCodes = query.AreaCodes.Select(q => q).ToList(),
                MetroLineIds = query.MetroLineIds.Select(q => q).ToList(),
                MetroStationIds = query.MetroStationIds.Select(q => q).ToList(),
                Distance = query.Distance,
                Type = query.Type.Select(q => q).ToList(),
                Lodging = query.Lodging,
                Canteen = query.Canteen,
                Orderby = query.Orderby,
                PageNo = query.PageNo,
                PageSize = query.PageSize,
                MetroSelect = query.MetroSelect,
                AbroadIds = query.AbroadIds.Select(q => q).ToList(),
                AuthIds = query.AuthIds.Select(q => q).ToList(),
                CharacIds = query.CharacIds.Select(q => q).ToList(),
                CourseIds = query.CourseIds.Select(q => q).ToList(),

                MinCost = query.MinCost,
                MaxCost = query.MaxCost,
                MinStudent = query.MinStudent,
                MaxStudent = query.MaxStudent,
                MinTeacher = query.MinTeacher,
                MaxTeacher = query.MaxTeacher,

                MinCScore = query.MinCScore,
                MaxCScore = query.MaxCScore,
                MinCtScore = query.MinCtScore,
                MaxCtScore = query.MaxCtScore,
                MinCuScore = query.MinCuScore,
                MaxCuScore = query.MaxCuScore,
                MinHScore = query.MinHScore,
                MaxHScore = query.MaxHScore,
                MinLScore = query.MinLScore,
                MaxLScore = query.MaxLScore,
                MinTScore = query.MinTScore,
                MaxTScore = query.MaxTScore,
                MinTotal = query.MinTotal,
                MaxTotal = query.MaxTotal
            };
        }

        //private string ActionUrl(Func<SchoolExtFilterData> func)
        //{
        //    var query = func.Invoke();
        //    return Url.Action("ListExtSchool", "School", query);
        //}

        private string ActionShortIdUrl(Func<SchoolExtFilterData> func, List<MetroDto> metros, List<TagDto> tags)
        {
            var query = func.Invoke();
            if (query.PageNo > 1)
            {
                return Url.Action("List", "School", new { option = GetOption(query, metros, tags), pg = query.PageNo });
            }
            return Url.Action("List", "School", new { option = GetOption(query, metros, tags) });
        }

        private string SchoolDetailUrl(Guid eid, Guid sid)
        {
            return $"/school/detail/{eid.ToString().Replace("-", "")}";
        }

        //直方图统计
        public async Task<SchoolExtStatsViewModel> GetExtStats(SchoolExtFilterData query, int city, List<MetroDto> metroLines, List<TagDto> tags)
        {
            var lat = Request.GetLatitude();
            var lng = Request.GetLongitude();
            //获取当前的城市
            var localCityCode = city == 0 ? Request.GetLocalCity() : city;

            var includeGrade = query.Type.Select(q => q[0].ToString()).GroupBy(q => q).Select(q => q.Key).ToList();
            //query.Type.AddRange(query.Grade.Where(q => !includeGrade.Any(p => p == q.ToString())).Select(q => q.ToString()));


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

            var schoolQueryData = new SearchSchoolQuery
            {
                Keyword = query.KeyWords,
                Latitude = double.Parse(lat),
                Longitude = double.Parse(lng),
                CurrentCity = localCityCode,
                CityCode = query.CityCode,
                AreaCodes = query.AreaCodes,
                MetroIds = metroIds,
                Distance = query.Distance ?? 0,
                Type = query.Type,
                Grade = query.Grade,
                Lodging = query.Lodging == null ? 3 : query.Lodging == true ? 2 : 1,
                Canteen = query.Canteen,
                AuthIds = query.AuthIds,
                CharacIds = query.CharacIds,
                AbroadIds = query.AbroadIds,
                CourseIds = query.CourseIds,
                Orderby = query.Orderby,

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
            };
            var liststat = _dataSearch.SearchSchoolCostStats(schoolQueryData);

            var url = new StatsUrl()
            {
                Origin = ActionShortIdUrl(() =>
                {
                    return query;
                }, metroLines, tags).Replace("/school", ""),
                Cost = ActionShortIdUrl(() =>
                {
                    var queryDate = CopyData(query);
                    queryDate.MinCost = null;
                    queryDate.MaxCost = null;
                    return queryDate;
                }, metroLines, tags).Replace("/school", ""),
                Student = ActionShortIdUrl(() =>
                {
                    var queryDate = CopyData(query);
                    queryDate.MinStudent = null;
                    queryDate.MaxStudent = null;
                    return queryDate;
                }, metroLines, tags).Replace("/school", ""),
                Teacher = ActionShortIdUrl(() =>
                {
                    var queryDate = CopyData(query);
                    queryDate.MinTeacher = null;
                    queryDate.MaxTeacher = null;
                    return queryDate;
                }, metroLines, tags).Replace("/school", ""),
                ScoreCompre = ActionShortIdUrl(() =>
                {
                    var queryDate = CopyData(query);
                    queryDate.MinTotal = null;
                    queryDate.MaxTotal = null;
                    queryDate.MinCScore = null;
                    queryDate.MaxCScore = null;
                    return queryDate;
                }, metroLines, tags).Replace("/school", ""),
                ScoreTeach = ActionShortIdUrl(() =>
                {
                    var queryDate = CopyData(query);
                    queryDate.MinTotal = null;
                    queryDate.MaxTotal = null;
                    queryDate.MinTScore = null;
                    queryDate.MaxTScore = null;
                    return queryDate;
                }, metroLines, tags).Replace("/school", ""),
                ScoreHard = ActionShortIdUrl(() =>
                {
                    var queryDate = CopyData(query);
                    queryDate.MinTotal = null;
                    queryDate.MaxTotal = null;
                    queryDate.MinHScore = null;
                    queryDate.MaxHScore = null;
                    return queryDate;
                }, metroLines, tags).Replace("/school", ""),
                ScoreCourse = ActionShortIdUrl(() =>
                {
                    var queryDate = CopyData(query);
                    queryDate.MinTotal = null;
                    queryDate.MaxTotal = null;
                    queryDate.MinCuScore = null;
                    queryDate.MaxCuScore = null;
                    return queryDate;
                }, metroLines, tags).Replace("/school", ""),
                ScoreLearn = ActionShortIdUrl(() =>
                {
                    var queryDate = CopyData(query);
                    queryDate.MinTotal = null;
                    queryDate.MaxTotal = null;
                    queryDate.MinLScore = null;
                    queryDate.MaxLScore = null;
                    return queryDate;
                }, metroLines, tags).Replace("/school", ""),
                ScoreCost = ActionShortIdUrl(() =>
                {
                    var queryDate = CopyData(query);
                    queryDate.MinTotal = null;
                    queryDate.MaxTotal = null;
                    queryDate.MinCtScore = null;
                    queryDate.MaxCtScore = null;
                    return queryDate;
                }, metroLines, tags).Replace("/school", ""),
                ScoreTotal = ActionShortIdUrl(() =>
                {
                    var queryDate = CopyData(query);

                    queryDate.MinCScore = null;
                    queryDate.MaxCScore = null;
                    queryDate.MinTeacher = null;
                    queryDate.MaxTeacher = null;
                    queryDate.MinHScore = null;
                    queryDate.MaxHScore = null;
                    queryDate.MinCuScore = null;
                    queryDate.MaxCuScore = null;
                    queryDate.MinLScore = null;
                    queryDate.MaxLScore = null;
                    queryDate.MinCtScore = null;
                    queryDate.MaxCtScore = null;

                    queryDate.MinTotal = null;
                    queryDate.MaxTotal = null;
                    return queryDate;
                }, metroLines, tags).Replace("/school", "")
            };
            return new SchoolExtStatsViewModel
            {
                Hists = liststat.Select(q => new SchoolExtHistViewModel
                {
                    Name = q.Name,
                    Hist = q.Hist.Select(p => new SchoolExtHistCountViewModel
                    {
                        Key = p.Key,
                        Count = p.Count
                    }).ToList()
                }).ToList(),
                Url = url
            };
        }

        /// <summary>
        /// 学校点评列表页
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Description("学校点评列表页")]
        [Route("/{controller}/{action}/{id}")]
        [Route("/{controller}-{schoolNo}/{action}")]
        public async Task<IActionResult> Comment(Guid Id, string search, int page = 1, int tab = 1,
            CourseType CourseType = CourseType.Unknown, string schoolNo = default)
        {
            if (!string.IsNullOrWhiteSpace(schoolNo))
            {
                var schext = await _schService.GetSchextSimpleInfoViaShortNo(schoolNo);
                if (schext == null)
                {
                    return new ExtNotFoundViewResult();
                }
                Id = schext.Eid;
            }
            else
            {
                var schext = await _schService.GetSchextSimpleInfo(Id);
                if (schext == null)
                {
                    return new ExtNotFoundViewResult();
                }
                return RedirectPermanent($"/school-{schext.ShortSchoolNo}/comment/");
            }
            ViewBag.ExtId = Id;
            int pageSize = 5;
            ViewBag.PageIndex = page;
            ViewBag.PageSize = pageSize;
            ViewBag.KeyWords = search;
            //用户id
            Guid UserId = Guid.Empty;

            if (User.Identity.IsAuthenticated)
            {
                UserId = User.Identity.GetId();
            }
            int SelectedTab = tab;
            ViewBag.SelectedTab = SelectedTab;

            //获取该校统计数据
            var SchoolInfo = await _schoolInfoService.QuerySchoolInfo(Id);

            var schoolData = await _schoolService.GetSchoolExtDtoAsync(Id, 0, 0);

            //获取学校综综合评价分值
            var scoreResult = _commentScoreService.GetSchoolScoreById(Id);
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
            var SchoolCard = SchoolDtoToSchoolCardHelper.SchoolDtoToSchoolCommentCard(new List<PMS.CommentsManage.Application.ModelDto.SchoolInfoDto>() { SchoolInfo });
            ViewBag.SchoolCard = SchoolCard.FirstOrDefault();

            ViewBag.CommentTotal = commentTotal;


            if (string.IsNullOrWhiteSpace(search))
            {
                List<PMS.CommentsManage.Application.ModelDto.SchoolCommentDto> HotComment = new List<PMS.CommentsManage.Application.ModelDto.SchoolCommentDto>();
                if (SelectedTab == 1)
                {
                    //获取学校热门点评数据
                    HotComment = _commentService.SelectedThreeComment(SchoolInfo.SchoolId, UserId, (QueryCondition)1, CommentListOrder.None);
                }

                List<PMS.CommentsManage.Application.ModelDto.SchoolCommentDto> NewComment;
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
                CommentScore.AddRange(NewComment.Select(p => p.Score).ToList());

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
                    Eid = Id
                });
                List<PMS.CommentsManage.Application.ModelDto.SchoolCommentDto> SearchComment = new List<PMS.CommentsManage.Application.ModelDto.SchoolCommentDto>();
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

            ViewBag.IsSignSchool = _lSRFSchoolDetail.CheckSchIsLeaving(Id);

            ViewBag.ActiveCourseType = (int)CourseType;

            ViewBag.SchoolNo = schoolNo;

            ViewBag.Page_Description = schoolData?.Intro?.GetHtmlHeaderString(150) ?? string.Empty;
            ViewBag.Grade = schoolData?.Grade;
            await Get_common(SchoolInfo.SchoolId, SchoolInfo.SchoolSectionId);
            return View();
        }


        /// <summary>
        /// 学校问答列表页
        /// </summary>
        /// <param name="ExtId"></param>
        /// <returns></returns>
        [Description("学校问答列表页")]
        [Route("/{controller}/{action}/{id}")]
        [Route("/{controller}-{schoolNo}/{action}")]
        public async Task<IActionResult> Question(Guid Id, string search, int page = 1, int Tab = 1,
            PMS.OperationPlateform.Domain.Enums.CourseType CourseType = CourseType.Unknown, string schoolNo = default)
        {
            if (!string.IsNullOrWhiteSpace(schoolNo))
            {
                var schext = await _schService.GetSchextSimpleInfoViaShortNo(schoolNo);
                if (schext == null)
                {
                    return new ExtNotFoundViewResult();
                }
                Id = schext.Eid;
            }
            else
            {
                var schext = await _schService.GetSchextSimpleInfo(Id);
                if (schext == null)
                {
                    return new ExtNotFoundViewResult();
                }
                return RedirectPermanent($"/school-{schext.ShortSchoolNo}/question/");
            }
            Guid ExtId = Id;
            int SelectedTab = Tab;
            int pageSize = 5;
            ViewBag.PageIndex = page;
            ViewBag.PageSize = pageSize;
            ViewBag.ExtId = ExtId;
            ViewBag.KeyWords = search;
            //检测当前用户身份 （true：校方用户 |false：普通用户）
            ViewBag.isSchoolRole = false;

            Guid UserId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                //检测当前用户身份 （true：校方用户 |false：普通用户）
                ViewBag.isSchoolRole = user.Role.Contains((int)PMS.UserManage.Domain.Common.UserRole.School);
                UserId = user.UserId;

                //检测改用户是否关注该学校
                //var checkCollectedSchool = await _userServiceClient.IsCollected(new ProductManagement.API.Http.Model.IsCollected() { userId = userId, dataID = SchoolId });
                //ViewBag.isCollected = checkCollectedSchool.iscollected;
            }

            ViewBag.SelectedTab = SelectedTab;

            //获取该校统计数据
            var SchoolInfo = await _schoolInfoService.QuerySchoolInfo(ExtId);

            var schoolData = await _schoolService.GetSchoolExtDtoAsync(ExtId, 0, 0);

            //获取学校提问列表页 标签统计值及标签 【现在统计数据都为统计全校（所有分部）】
            var QuestionTotal = _questionInfoService.CurrentQuestionTotalBySchoolId(SchoolInfo.SchoolId)
            .Select(q => new SchoolQuestionTotalViewModel
            {
                SchoolSectionId = q.SchoolSectionId,
                Name = q.Name,
                Total = q.Total,
                TotalType = q.TotalType.Description(),
                TotalTypeNumber = (int)q.TotalType
            }).ToList();

            //获取该校下所有的分部名称标签
            var AllExtension = _schoolService.GetSchoolExtName(SchoolInfo.SchoolId);
            for (int i = 0; i < AllExtension.Count(); i++)
            {
                var item = QuestionTotal.FirstOrDefault(x => x.SchoolSectionId == AllExtension[i].Value);
                int currentIndex = QuestionTotal.IndexOf(item);
                if (item == null)
                {
                    QuestionTotal.Add(new SchoolQuestionTotalViewModel() { Name = AllExtension[i].Key, SchoolSectionId = AllExtension[i].Value, TotalType = QueryCondition.Other.Description(), TotalTypeNumber = (int)QueryCondition.Other });
                }
                else
                {
                    item.Name = AllExtension[i].Key;
                    QuestionTotal[currentIndex] = item;
                }
            }

            //将其他分部类型学校进行再次排序
            int TotalTypeNumber = 8;
            QuestionTotal.Where(x => x.TotalTypeNumber == 8)?.ToList().ForEach(x =>
            {
                x.TotalTypeNumber = TotalTypeNumber;
                TotalTypeNumber++;
            });

            //领域实体转 视图实体【学校问答卡片】
            var SchoolQuestionCard = SchoolDtoToSchoolCardHelper.SchoolDtoToSchoolQuestionCard(new List<PMS.CommentsManage.Application.ModelDto.SchoolInfoDto>() { SchoolInfo });

            ViewBag.SchoolQuestionCard = SchoolQuestionCard.FirstOrDefault();
            ViewBag.QuestionTotal = QuestionTotal;


            if (string.IsNullOrWhiteSpace(search))
            {
                List<PMS.CommentsManage.Application.ModelDto.QuestionDto> HotQuestion = new List<PMS.CommentsManage.Application.ModelDto.QuestionDto>();
                if (SelectedTab == 1)
                {
                    //获取学校热门问题数据
                    HotQuestion = _questionInfoService.GetHotQuestionInfoBySchoolId(SchoolInfo.SchoolSectionId, UserId, 1, 3, SelectedQuestionOrder.Intelligence) ?? new List<PMS.CommentsManage.Application.ModelDto.QuestionDto>();
                }

                List<PMS.CommentsManage.Application.ModelDto.QuestionDto> NewQuestion;
                int total = 0;

                if (SelectedTab >= 8)
                {
                    Guid eid = QuestionTotal.Where(x => x.TotalTypeNumber == SelectedTab).FirstOrDefault().SchoolSectionId;
                    //获取该校的最新问题
                    NewQuestion = _questionInfoService.GetNewQuestionInfoBySchoolId(eid, UserId, page, pageSize, QueryQuestion.Other, SelectedQuestionOrder.CreateTime, out total) ?? new List<PMS.CommentsManage.Application.ModelDto.QuestionDto>();
                }
                else
                {
                    //获取该校的最新问题
                    NewQuestion = _questionInfoService.GetNewQuestionInfoBySchoolId(SchoolInfo.SchoolId, UserId, page, pageSize, (QueryQuestion)SelectedTab, SelectedQuestionOrder.CreateTime, out total) ?? new List<PMS.CommentsManage.Application.ModelDto.QuestionDto>();
                }

                //获取热门提问写入用户信息
                var userIds = HotQuestion.Select(p => p.UserId).ToList();
                userIds.AddRange(NewQuestion.Select(p => p.UserId).ToList());
                var Users = _userService.ListUserInfo(userIds);

                //检测提问是否关注
                var checkCollection = _collectionService.GetCollection(HotQuestion.Select(x => x.Id).ToList(), UserId);
                checkCollection.AddRange(_collectionService.GetCollection(NewQuestion.Select(x => x.Id).ToList(), UserId));

                //提问领域实体转视图实体
                var HotQuestionViewModels = QuestionDtoToVoHelper.QuestionDtoToVo(_setting.QueryImager, HotQuestion, UserHelper.UserDtoToVo(Users), null, checkCollection);
                ViewBag.HotQuestionList = HotQuestionViewModels;

                var NewQuestionViewModels = QuestionDtoToVoHelper.QuestionDtoToVo(_setting.QueryImager, NewQuestion, UserHelper.UserDtoToVo(Users), null, checkCollection);
                ViewBag.NewQuestionList = NewQuestionViewModels;
                ViewBag.TotalPage = total;

            }
            else
            {
                var result = _dataSearch.SearchQuestion(new SearchQuestionQuery
                {
                    Keyword = search,
                    PageNo = page,
                    PageSize = pageSize,
                    Eid = ExtId
                });
                List<PMS.CommentsManage.Application.ModelDto.QuestionDto> SearchQuestion = new List<PMS.CommentsManage.Application.ModelDto.QuestionDto>();
                //检测是否有数据返回
                if (result.Total > 0)
                {
                    SearchQuestion = _questionInfoService.GetQuestionByIds(result.Questions.Select(x => x.Id).ToList(), UserId);
                }
                ViewBag.TotalPage = result.Total;
                //获取热门点评写入用户信息
                var UserIds = new List<Guid>();
                UserIds.AddRange(SearchQuestion.GroupBy(q => q.UserId).Select(p => p.Key).ToList());
                var Users = _userService.ListUserInfo(UserIds.GroupBy(q => q).Select(p => p.Key).ToList());

                var NewQuestionViewModels = QuestionDtoToVoHelper.QuestionDtoToVo(_setting.QueryImager, SearchQuestion, UserHelper.UserDtoToVo(Users));
                ViewBag.NewQuestionList = NewQuestionViewModels;
            }
            
            await Get_common(SchoolInfo.SchoolId, SchoolInfo.SchoolSectionId);

            ViewBag.IsSignSchool = _lSRFSchoolDetail.CheckSchIsLeaving(ExtId);

            ViewBag.ActiveCourseType = (int)CourseType;

            ViewBag.ShortSchoolNo = schoolNo;

            ViewBag.Page_Description = schoolData?.Intro?.GetHtmlHeaderString(150) ?? string.Empty;
            ViewBag.Grade = schoolData?.Grade;
            return View();
        }



        [HttpGet]
        [Route("/{controller}/{action}")]
        public async Task<ResponseResult> GetSchoolExtensionDetails(Guid extId)
        {
            var school = await _schoolService.GetSchoolExtensionDetails(extId);

            if (school != null)
            {
                return ResponseResult.Success(school);
            }
            return ResponseResult.Failed();
        }

    }
}
