using AutoMapper;
using iSchool;
using iSchool.Internal.API.RankModule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using MongoDB.Bson;
using MongoDB.Driver;
using PMS.CommentsManage.Application.IServices;
using PMS.Infrastructure.Application.IService;
using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using PMS.School.Application.IServices;
using PMS.School.Domain.Dtos;
using PMS.School.Infrastructure.Common;
using PMS.Search.Application.IServices;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Entities;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using Sxb.PCWeb;
using Sxb.PCWeb.Common;
using Sxb.PCWeb.Models;
using Sxb.PCWeb.Response;
using Sxb.PCWeb.Utils;
using Sxb.PCWeb.ViewModels;
using Sxb.PCWeb.ViewModels.Article;
using Sxb.PCWeb.ViewModels.Common;
using Sxb.PCWeb.ViewModels.FeedBack;
using Sxb.PCWeb.ViewModels.Rank;
using Sxb.PCWeb.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DrawingCore;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommentApp.Controllers
{

    public class HomeController : Controller
    {
        IArticleCoverService _articleCoverService;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ISchoolService _schoolService;
        private readonly ISchoolCommentService _commentService;
        private readonly ISchoolCommentScoreService _commentScoreService;
        private readonly IQuestionInfoService _questionInfoService;
        private IConfiguration _configuration;
        private IArticleService _articleService;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly ICityInfoService _cityService;
        private readonly IMapper _mapper;
        private readonly ImageSetting _setting;
        private readonly IUserService _userService;
        private readonly ISearchService _dataSearch;
        private readonly RankApiServices _rankApiServices;
        private readonly ILogger Log;

        private readonly IFeedbackService _reportService;

        private readonly ILinksService _linksService;

        private readonly ISchService _schService;

        private readonly ISchoolRankService _schoolRankService;

        private readonly string _AddRankList = "addRankList";
        private readonly string _HotSchoolKey = "ext:PChotSchools:{0}";
        //根据城市跟类型获取评分最高的学校
        private readonly string _ScoreSchoolKey = "ext:ScoreSchools:{0}";
        //根据城市跟类型获取评论分数最高的学校
        private readonly string _CommentScoreSchoolKey = "ext:CommentScoreSchools:{0}";

        //获取学段类型
        private readonly int[] _Grade = new int[] { (int)PMS.School.Domain.Common.SchoolGrade.Kindergarten, (byte)PMS.School.Domain.Common.SchoolGrade.PrimarySchool, (int)PMS.School.Domain.Common.SchoolGrade.JuniorMiddleSchool, (int)PMS.School.Domain.Common.SchoolGrade.SeniorMiddleSchool };

        //口碑评分最高：全国当前最热门的学校信息【点评数最高的】
        private readonly string _CommentHottestKey = "ext:WholeCountryCommentScoreSchools:{0}";

        //评分最高：全国当前最热门的学校信息
        private readonly string _SchoolScoreHottestKey = "ext:WholeCountryScoreSchools:{0}";


        public HomeController(
            ISchoolService schoolService,
            ISchoolCommentService schoolComment,
            ISchoolCommentScoreService commentScoreService,
            IQuestionInfoService questionInfoService,
            IUserService userService,
            ICityInfoService cityService, IHttpClientFactory clientFactory,
            ISearchService dataSearch, ILinksService linksService,
            IMapper mapper, IOptions<ImageSetting> set, RankApiServices rankApiServices,
            IConfiguration iConfig, IArticleService articleService, IEasyRedisClient easyRedisClient, IFeedbackService reportService, ILoggerFactory log,
            ISchService schService, IArticleCoverService articleCoverService, ISchoolRankService schoolRankService)
        {
            _cityService = cityService;
            _schoolService = schoolService;
            _commentService = schoolComment;
            _commentScoreService = commentScoreService;
            _questionInfoService = questionInfoService;
            _configuration = iConfig;
            _articleService = articleService;
            _easyRedisClient = easyRedisClient;
            _dataSearch = dataSearch;
            _userService = userService;
            _rankApiServices = rankApiServices;
            _mapper = mapper;
            _setting = set.Value;

            _reportService = reportService;

            _linksService = linksService;
            Log = log.CreateLogger(GetType().FullName);
            _clientFactory = clientFactory;

            _schService = schService;
            _articleCoverService = articleCoverService;
            _schoolRankService = schoolRankService;
        }

        /// <summary>
        /// 学校首页
        /// </summary>
        /// <returns></returns>
        [Description("学校首页")]
        public async Task<IActionResult> Index(int province = 0, int city = 0, int area = 0)
        {
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
            //当前城市
            //获取当前的城市
            var localCityCode = city == 0 ? Request.GetLocalCity() : city;
            //if (localCityCode % 1000 == 0)//容错
            //{
            //    var cityCodes = _cityService.GetCityCodes(localCityCode);
            //    if (cityCodes.Any())
            //    {
            //        localCityCode = cityCodes[0].Id;
            //    }
            //    else
            //    {
            //        localCityCode = 440100;
            //    }
            //    Response.SetLocalCity(localCityCode.ToString());
            //}
            ViewBag.LocalCity = await _cityService.GetCityName(localCityCode);
            //热门城市
            var hotCity = await _cityService.GetHotCity();
            ViewBag.HotCity = hotCity;
            //热门搜索
            //string Key = $"SearchHotword";
            //var hotwordList = await _easyRedisClient.SortedSetRangeByRankAsync<string>(Key, 0, 10, StackExchange.Redis.Order.Descending);
            //var result = hotwordList.Select(q => new SearchWordViewModel { Name = q }).Take(3).ToList();
            //ViewBag.HotWord = result;

            //0.学校  1.点评  2.问答  3.排行榜  4.文章
            ViewBag.HotSchoolWord = _dataSearch.GetHotHistoryList(0, 3,null, localCityCode);
            ViewBag.HotCommentWord = _dataSearch.GetHotHistoryList(1, 3, null, localCityCode);
            ViewBag.HotQuestionWord = _dataSearch.GetHotHistoryList(2, 3, null, localCityCode);
            ViewBag.HotArticleWord = _dataSearch.GetHotHistoryList(4, 3, null, localCityCode);

            //获取排行榜
            var rank = await GetRankAsync(localCityCode);
            ViewBag.Rank = rank ?? new List<SchoolRankApiModel>(); ;

            //热门学校
            //var hotSchool = await _schoolService.GetPCHotSchool(city);
            //var hotSchool = await _schoolService.GetPCIndexDataAsync(city);
            //ViewBag.HotSchool = hotSchool;
            var data = await GetHotSchoolAsync(localCityCode);
            //热门学校
            ViewBag.PcHotSchool = data.hotschool.Select(x => x?.Take(6).ToList()).ToList();
            //评分最高学校
            ViewBag.SorceSchool = data.sorceschool.Select(x => x?.Take(6).ToList()).ToList();
            //点评分数最高的学校
            ViewBag.CommentSchool = data.commentschool.Select(x => x?.Take(6).ToList()).ToList();

            //await GetSchoolNav(localCityCode);

            //首页友情链接
            var links = _linksService.GetLinks();
            ViewBag.Links = links;

            #region SchoolNos
            var eids = new List<Guid>();
            if (rank?.Count > 0)
            {
                foreach (var item in rank)
                {
                    if (item.Items?.Count() > 0)
                    {
                        eids.AddRange(item.Items.Select(p => p.SchoolExtId).Distinct().ToList());
                    }
                }
            }
            if (data.hotschool?.Count > 0)
            {
                foreach (var item in data.hotschool)
                {
                    if (item?.Count > 0)
                    {
                        eids.AddRange(item.Select(p => p.ExtId).Distinct().ToList());
                    }
                }
            }
            if (data.commentschool?.Count > 0)
            {
                foreach (var item in data.commentschool)
                {
                    if (item?.Count > 0)
                    {
                        eids.AddRange(item.Select(p => p.ExtId).Distinct().ToList());
                    }
                }
            }
            if (data.sorceschool?.Count > 0)
            {
                foreach (var item in data.sorceschool)
                {
                    if (item?.Count > 0)
                    {
                        eids.AddRange(item.Select(p => p.ExtId).Distinct().ToList());
                    }
                }
            }
            var schoolNos = GetSchoolNos(eids);
            if (schoolNos?.Count > 0) ViewBag.SchoolNos = schoolNos;

            #endregion

            var result = _articleService.PCSearchList(localCityCode, null, 0, 30);
            List<article> articles = result.articles?.ToList();
            if (articles == null) articles = new List<article>();
            if (articles.Count < 30)
            {
                var leftData = _articleService.PCSearchList(0, null, 0, 30 - articles.Count);
                if (leftData.articles?.Count() > 0)
                {
                    articles.AddRange(leftData.articles);

                    articles = articles.GroupBy(p => p.No).Select(p => p.First()).ToList();
                }
            }

            var covers = _articleCoverService.GetCoversByIds(articles.Select(p => p.id).Distinct().ToArray());
            ViewBag.StrategyPageCount = 0;
            ViewBag.Articles = new List<ArticleListItemViewModel>();
            if (articles?.Any() == true)
            {
                ViewBag.StrategyPageCount = (articles.Count + 5) / 6;


                ViewBag.Articles = articles?.OrderByDescending(p => p.time).Select(a =>
                {
                    return new ArticleListItemViewModel()
                    {
                        Id = a.id,
                        No = UrlShortIdUtil.Long2Base32(a.No),
                        Layout = a.layout,
                        Type = string.IsNullOrEmpty(a.type) ? 1 : int.Parse(a.type),
                        ShortTitle = covers.Any(c => c.articleID == a.id) ? a.title.GetShortString(20) : a.title.GetShortString(40),
                        Title = a.title,
                        ViewCount = a.VirualViewCount,
                        Time = a.time.GetValueOrDefault().ArticleListItemTimeFormart(),
                        Covers = covers.Where(c => c.articleID == a.id).Select(c => Article_CoverToLink(c)).ToList()
                    };
                }).ToList();
            }

            return View();
        }

        /// <summary>
        /// 热门学校
        /// </summary>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        public async Task<IActionResult> HotSchools(int province = 0, int city = 0, int area = 0)
        {
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
            //当前城市
            //获取当前的城市
            var localCityCode = city == 0 ? Request.GetLocalCity() : city;
            ViewBag.LocalCity = await _cityService.GetCityName(localCityCode);

            var data = await GetHotSchoolAsync(city);
            //热门学校
            ViewBag.PcHotSchool = data.hotschool;
            //评分最高学校
            ViewBag.SorceSchool = data.sorceschool;
            //点评分数最高的学校
            ViewBag.CommentSchool = data.commentschool;


            //热门城市
            var hotCity = await _cityService.GetHotCity();
            ViewBag.HotCity = hotCity;
            //热门搜索
            ViewBag.HotSchoolWord = _dataSearch.GetHotHistoryList(0, 3);
            ViewBag.HotQuestionWord = _dataSearch.GetHotHistoryList(1, 3);
            ViewBag.HotCommentWord = _dataSearch.GetHotHistoryList(2, 3);
            ViewBag.HotArticleWord = _dataSearch.GetHotHistoryList(4, 3);

            #region SchoolNos
            var eids = new List<Guid>();
            if (data.hotschool?.Count > 0)
            {
                foreach (var item in data.hotschool)
                {
                    if (item?.Count > 0)
                    {
                        eids.AddRange(item.Select(p => p.ExtId).Distinct().ToList());
                    }
                }
            }
            if (data.commentschool?.Count > 0)
            {
                foreach (var item in data.commentschool)
                {
                    if (item?.Count > 0)
                    {
                        eids.AddRange(item.Select(p => p.ExtId).Distinct().ToList());
                    }
                }
            }
            if (data.sorceschool?.Count > 0)
            {
                foreach (var item in data.sorceschool)
                {
                    if (item?.Count > 0)
                    {
                        eids.AddRange(item.Select(p => p.ExtId).Distinct().ToList());
                    }
                }
            }
            var schoolNos = GetSchoolNos(eids);
            if (schoolNos?.Count > 0) ViewBag.SchoolNos = schoolNos;

            #endregion

            return View();
        }


        /// <summary>
        /// 更多的特色课程
        /// </summary>
        /// <returns></returns>
        [Description("更多特色课程")]
        public async Task<ActionResult> Features(int city = 0)
        {
            //从cookie中  取出省市区  年级
            if (city == 0)
                city = Request.GetLocalCity();
            else
            {
                Response.SetLocalCity(city.ToString());
            }
            //当前城市
            ViewBag.LocalCity = await _cityService.GetCityName(city);


            //获取特殊课程的列表
            var data = _schoolService.GetTagList().Where(p => p.Type == 8);
            //热门城市
            var hotCity = await _cityService.GetHotCity();
            ViewBag.HotCity = hotCity;
            //热门搜索
            //0.学校  1.点评  2.问答  3.排行榜  4.文章
            ViewBag.HotSchoolWord = _dataSearch.GetHotHistoryList(0, 3);
            ViewBag.HotQuestionWord = _dataSearch.GetHotHistoryList(1, 3);
            ViewBag.HotCommentWord = _dataSearch.GetHotHistoryList(2, 3);
            ViewBag.HotArticleWord = _dataSearch.GetHotHistoryList(4, 3);

            return View(data);
        }


        /// <summary>
        /// 切换城市
        /// </summary>
        /// <returns></returns>
        [Description("切换城市")]
        public async Task<ActionResult> Select(bool filter = false)
        {
            //返回链接
            var callback = Request.Headers["Referer"].ToString();
            //是否是短链接
            //var isShort = callback.IndexOf("~") > 0;


            var localCityCode = Request.GetLocalCity();
            var adCodes = await _cityService.IntiAdCodes();
            var adPinYinCodes = (await _cityService.GetAllCityPinYin()).SelectMany(p => p.Value);

            //List<AreaDto> history = new List<AreaDto>();
            //if (User.Identity.IsAuthenticated)
            //{
            //    var user = User.Identity.GetUserInfo();
            //    string Key = $"SearchCity:{user.UserId}";
            //    var historyCity = await _easyRedisClient.SortedSetRangeByRankAsync<AreaDto>(Key, 0, 10, StackExchange.Redis.Order.Descending);
            //    history.AddRange(historyCity.ToList());
            //}

            //var ua = UserAgentHelper.Check(Request);

            //城市编码
            var newAdCodes = adCodes.Select(p => new CityVo
            {
                Key = p.Key,
                CityCodes = p.CityCodes.Select(x =>
                new CityCodes
                {
                    AdCode = x.Id,
                    Name = x.Name,
                    url = filter ? BuildFilterCityUrl(callback, x.Id) : BuildCityUrl(callback, x.Id),
                    pinyin = string.Join("", adPinYinCodes?.FirstOrDefault(g => g.CityCode == x.Id)?.Pinyin ?? new List<string>())
                })
            });
            ViewBag.AllCity = JsonHelper.ObjectToJSON(newAdCodes);
            ViewBag.CityList = newAdCodes;

            //当前城市adcode
            ViewBag.LocalCity = adCodes.SelectMany(p => p.CityCodes).FirstOrDefault(p => p.Id == localCityCode)?.Name;
            //热门城市
            var hotCity = await _cityService.GetHotCity();
            ViewBag.HotCity = hotCity.Select(p => new KeyValueDto<int>
            {
                Key = p.AreaName,
                Value = p.AdCode,
                Message = filter ? BuildFilterCityUrl(callback, p.AdCode) : BuildCityUrl(callback, p.AdCode)
            });

            //省区联动
            var province = await _cityService.GetProvinceInfos();
            ViewBag.Province = JsonHelper.ObjectToJSON(
                province.Select(p => new
                {
                    Province = p.Province,
                    ProvinceId = p.ProvinceId,
                    CityCodes = p.CityCodes.Select(x => new
                    {
                        AdCode = x.Id,
                        Name = x.Name,
                        url = filter ? BuildFilterCityUrl(callback, x.Id) : BuildCityUrl(callback, x.Id)
                    })
                }));
            ViewBag.CallbackURL = callback;
            return View();
        }

        /// <summary>
        /// 关于我们
        /// </summary>
        /// <returns></returns>
        [Description("关于我们")]
        public ActionResult About()
        {
            return View();
        }

        /// <summary>
        /// 招聘
        /// </summary>
        /// <returns></returns>
        [Description("招聘")]
        public ActionResult HR()
        {
            return View();
        }

        /// <summary>
        /// 联系方式
        /// </summary>
        /// <returns></returns>
        [Description("联系方式")]
        public ActionResult Contact()
        {
            return View();
        }


        /// <summary>
        /// 协议声明
        /// </summary>
        /// <returns></returns>
        [Description("协议声明")]
        public ActionResult Principle()
        {
            return View();
        }

        /// <summary>
        /// 帮助中心
        /// </summary>
        /// <returns></returns>
        [Description("帮助中心")]
        public ActionResult Center()
        {
            return View();
        }


        /// <summary>
        /// 获取学校排行榜
        /// </summary>
        [Description("获取学校排行榜")]
        public async Task<List<SchoolRankApiModel>> GetRankAsync(int city = 0)
        {
            string cacheKey = $"GetTheNationwideAndNewsRanks-{city}";
            var result = await _easyRedisClient.GetOrAddAsync(cacheKey, async () =>
            {

                var RankResult = _schoolRankService.GetTheNationwideAndNewsRanks(city);
                var schoolIds = RankResult.schoolranks.SelectMany(sr => {
                    return sr.SchoolRankBinds.Select(sb => sb.SchoolId);
                });
                var schools = await _schoolService.ListExtSchoolByBranchIds(schoolIds.ToList());
                return RankResult.schoolranks.Select(sr => {
                    SchoolRankApiModel schoolRankApiModel = new SchoolRankApiModel()
                    {
                        RankId = sr.Id,
                        RankName = sr.Title,
                        Cover = sr.Cover,
                        IsShow = sr.IsShow.GetValueOrDefault(),
                        No = sr.No,
                        Sort = (int)sr.Rank,
                        ToTop = sr.ToTop.GetValueOrDefault()

                    };
                    schoolRankApiModel.Items = sr.SchoolRankBinds.Select(sb =>
                    {
                        var school = schools.FirstOrDefault(s => s.ExtId == sb.SchoolId);
                        return new SchoolRankApiModel.SchoolRankBindItems()
                        {
                            SchoolExtId = sb.SchoolId,
                            SchoolName = school?.Name,
                            Sort = (int)sb.Sort
                        };
                    });

                    return schoolRankApiModel;
                }).ToList();

            },DateTimeOffset.Now.AddDays(7));
            return result;
        }


        /// <summary>
        /// 生成城市链接
        /// </summary>
        /// <param name="url"></param>
        /// <param name="city"></param>
        /// <returns></returns>
        private string BuildCityUrl(string url, int city)
        {
            StringBuilder cityUrl = new StringBuilder();

            //去除域名部分
            //Regex regEx = new Regex(@"^[a-zA-z]+://[^\s]*(:\d+)?/");
            //url = regEx.Replace(url, "");
            if (!string.IsNullOrEmpty(url))
            {
                url = url.ToLower();
                Uri uri = new Uri(url);
                url = url.Replace(uri.Authority, "").Replace(uri.Scheme + "://", "");
                var query = uri.Query.Replace("?", "");
                if (query.Contains("city="))
                {
                    //拆分参数
                    var paths = query.Split("&");
                    query = string.Join("&", paths.Where(p => !p.Contains("city=")));
                }
                if (!string.IsNullOrWhiteSpace(uri.Query))
                    url = url.Replace(uri.Query.ToLower(), "");
                if (!string.IsNullOrWhiteSpace(query))
                {
                    cityUrl.Append($"{url}?{query}&city={city}");
                }
                else
                {
                    cityUrl.Append($"{url}?city={city}");
                }
            }
            else
            {
                cityUrl.Append($"{url}?city={city}");
            }

            return cityUrl.ToString().ToLower();
        }

        /// <summary>
        /// 筛选学校列表切换城市url
        /// </summary>
        /// <returns></returns>
        private string BuildFilterCityUrl(string url, int city)
        {
            StringBuilder cityUrl = new StringBuilder();

            if (!string.IsNullOrEmpty(url))
            {
                url = url.ToLower();
                Uri uri = new Uri(url);
                url = url.Replace(uri.Authority, "").Replace(uri.Scheme + "://", "");

                var query = uri.Query.Replace("?", "");
                string keywords = "";
                if (query.Contains("keywords="))
                {
                    var param = query.Split("&");
                    keywords = param.FirstOrDefault(q => q.ToLower().Contains("keywords="));
                    keywords = keywords.Replace("keywords=", "");
                }


                //移除页码
                string patternPage = @"/pg\d+";
                var regPage = new Regex(patternPage);
                url = regPage.Replace(url, "");

                //获取已选的城市或区
                List<int> cityCode = new List<int>();
                List<int> areaCode = new List<int>();

                string patternCity = @"(?<=ci)\d+";
                var regCity = new Regex(patternCity);
                var cityMatch = regCity.Matches(url);
                foreach (var item in cityMatch.Select(q => q.Value))
                {
                    cityCode.Add(Convert.ToInt32(item));
                }

                string patternArea = @"(?<=ar)\d+";
                var regArea = new Regex(patternArea);
                var areaMatch = regArea.Matches(url);
                foreach (var item in areaMatch.Select(q => q.Value))
                {
                    areaCode.Add(Convert.ToInt32(item));
                }

                //拆分参数
                if (!string.IsNullOrWhiteSpace(uri.Query))
                    url = url.Replace(uri.Query.ToLower(), "");
                var paths = url.Replace("/school/", "").Replace("/school", "").Split("_");

                cityUrl.Append($"/school/");



                if (paths.Any() && !string.IsNullOrWhiteSpace(paths[0]))
                {
                    //移除掉城市有关的参数
                    var newPara = string.Join("_", paths.Where(p => !p.Contains("cs") && !p.Contains("ci") && !p.Contains("ar") && !p.Contains("ms")));



                    cityCode.Add(city);

                    var cityPara = "";
                    var areaPara = "";

                    foreach (var item in areaCode.GroupBy(q => q).Select(p => p.Key))
                    {
                        //移除已经存在在区列表里的城市
                        //因为有些城市没有区，下一级就是镇，镇的编号有9位，因此区推导出市编号去掉后三位补0，镇推导出市编号去掉后三位
                        var c = item / 1000000 == 0 ? item / 100 * 100 : item / 1000;
                        cityCode = cityCode.Where(q => q != c).ToList();

                        areaPara += "ar" + item;
                    }
                    foreach (var item in cityCode.GroupBy(q => q).Select(p => p.Key))
                    {
                        cityPara += "ci" + item;
                    }

                    if (!string.IsNullOrWhiteSpace(keywords) && !newPara.Contains("kw-"))
                    {
                        cityUrl.Append($"kw-{keywords}_");
                    }

                    if (string.IsNullOrWhiteSpace(newPara))
                    {
                        cityUrl.Append($"{cityPara}{areaPara}_cs{city}");
                    }
                    else
                    {
                        cityUrl.Append($"{cityPara}{areaPara}_{newPara}_cs{city}");
                    }

                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(keywords))
                    {
                        cityUrl.Append($"kw-{keywords}_");
                    }
                    cityUrl.Append($"ci{city}_cs{city}");
                }
            }
            else
            {
                cityUrl.Append($"ci{city}_cs{city}");
            }

            return cityUrl.ToString().ToLower();
        }

        /// <summary>
        /// 获取学校自编号码
        /// </summary>
        /// <param name="eids">SchoolExtIds</param>
        /// <returns></returns>
        Dictionary<Guid, string> GetSchoolNos(List<Guid> eids)
        {
            if (eids?.Count > 0)
            {
                var schoolNos = _schService.GetSchoolextNo(eids.Distinct().ToArray());
                if (schoolNos != null && schoolNos.Length > 0)
                {
                    return schoolNos.ToDictionary(k => k.Item1, v => v.Item2 > 0 ? UrlShortIdUtil.Long2Base32(v.Item2).ToLower() : "");
                }
            }
            return null;
        }


        /// <summary>
        /// 设置友情链接
        /// </summary>
        /// <returns></returns>
        [Description("设置友情链接")]
        public ResponseResult AddLinks(string title, string link, int sort = 0)
        {
            _linksService.SetLinks(title, link, sort);
            return ResponseResult.Success("设置成功");
        }

        /// <summary>
        /// 删除友情链接
        /// </summary>
        /// <returns></returns>
        [Description("删除友情链接")]
        public ResponseResult RemoveLinks(string title)
        {
            _linksService.RemoveLinks(title);
            return ResponseResult.Success("删除成功");
        }

        [Description("获取热门学校")]
        private async Task<(List<List<SchoolCollectionDto>> hotschool, List<List<SchoolExtItemDto>> sorceschool, List<List<SchoolExtItemDto>> commentschool)> GetHotSchoolAsync(int city, int day = 7, int count = 10)
        {
            var now = DateTime.Now;
            var time = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
            //await _easyRedisClient.RemoveAsync(string.Format(_HotSchoolKey, city));
            //await _easyRedisClient.RemoveAsync(string.Format(_ScoreSchoolKey, city));
            //await _easyRedisClient.RemoveAsync(string.Format(_CommentScoreSchoolKey, city));
            //foreach (var grade in _Grade)
            //{
            //    //if (!commentSchool.ContainsKey($"comment_{grade}"))
            //    //{
            //        var list = await _schoolService.GetSchoolExtListAsync(0, 0, 0, 0, 0, new int[] { grade }, orderBy: 2, new int[] { }, 0, new int[] { }, 1, count);
            //    //}
            //}


            //缓存热门学校
            var hotschool = await _easyRedisClient.GetAsync<List<List<SchoolCollectionDto>>>(string.Format(_HotSchoolKey, city));
            //如果为空，那就调用接口获取
            if (hotschool == null)
            {
                hotschool = new List<List<SchoolCollectionDto>>();

                // //http请求api
                var config = _configuration.GetSection("HostsSchools");
                var ip = config.GetSection("ServerUrl").Value;
                var url = $"{ip}?curpage=1&countperpage=15";

                // //http post请求获取 ua 最高的学校
                var client = _clientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(10);
                var requset = new HttpRequestMessage(HttpMethod.Post, url);
                var content = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("citycode", city.ToString())});
                try
                {
                    var response = await client.PostAsync(url, content);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var responseResult = await response.Content.ReadAsStringAsync();
                        var jsonData = JsonHelper.JSONToObject<HostsSchoolResponse<List<ExtIdItem>>>(responseResult);
                        if (jsonData.Status == 0)
                        {
                            foreach (var grade in _Grade)
                            {
                                //学校ids
                                var item = jsonData.Items.FirstOrDefault(p => p.Grade == grade);
                                if (item == null)
                                {
                                    hotschool.Add(new List<SchoolCollectionDto>());
                                }
                                else
                                {
                                    //2020-11-13,商务临时需求，将[广州市番禺区诺德安达学校-双语小学部]显示在PC首页本周热度TOP榜小学，过期可删
                                    if (grade == 2 && DateTime.Now.Date>=Convert.ToDateTime("2020-11-13") && DateTime.Now.Date < Convert.ToDateTime("2020-11-21"))
                                    {
                                        if (item.Ids.Any())
                                        {
                                            item.Ids[0] = Guid.Parse("7bc75c8a-8142-4eb5-89c3-fe285baad9ae");
                                        }
                                    }

                                    //获取学校信息
                                    var exts = await _schoolService.GetCollectionExtsAsync(item.Ids.ToArray(), null, true);
                                    hotschool.Add(exts.Where(x => x != null && x.Sid != Guid.Empty).ToList());
                                }
                            }
                        }
                        else
                        {
                            Log.LogError("PC请求热门学校失败," + jsonData.ErrorDescription);
                        }
                    }
                    else
                    {
                        Log.LogError("PC请求热门学校失败，状态码为：" + response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    Log.LogError("PC请求热门学校失败," + ex.Message);
                }
                //如果字典长度为0，添加空数据
                if (hotschool.Count == 0)
                {
                    foreach (var grade in _Grade)
                    {
                        hotschool.Add(new List<SchoolCollectionDto>());
                    }
                }
                //往缓存中添加数据
                await _easyRedisClient.AddAsync(string.Format(_HotSchoolKey, city), hotschool, time);
            }
            //orderby 1推荐 2口碑 3.附近
            //缓存评分最高的学校
            var scoreSchool = await _easyRedisClient.GetOrAddAsync(string.Format(_ScoreSchoolKey, city), async () =>
            {
                List<List<SchoolExtItemDto>> data = new List<List<SchoolExtItemDto>>();
                //Dictionary<string, List<SchoolExtItemDto>> data = new Dictionary<string, List<SchoolExtItemDto>>();
                foreach (var grade in _Grade)
                {
                    var list = await _schoolService.GetSchoolExtListAsync(Convert.ToDouble(Request.GetLongitude()), Convert.ToDouble(Request.GetLatitude()), 0, city, 0, new int[] { grade }, orderBy: 1, new int[] { }, 0, new int[] { }, 1, count);
                    if (list.List.Any())
                    {
                        //data.Add($"score_{grade}", list.List);
                        data.Add(list.List);
                    }
                    else
                    {
                        data.Add(null);
                    }
                }
                return data;
            }, time);



            //缓存点评最高的学校  HottestCommentSchool  List<List<SchoolCollectionDto>>
            var commentSchool = await _easyRedisClient.GetOrAddAsync(string.Format(_CommentScoreSchoolKey, city), async () =>
           {
               List<List<SchoolExtItemDto>> data = new List<List<SchoolExtItemDto>>();

               //Dictionary<string, List<SchoolExtItemDto>> data = new Dictionary<string, List<SchoolExtItemDto>>();
               foreach (var grade in _Grade)
               {
                   var list = await _schoolService.GetSchoolExtListAsync(Convert.ToDouble(Request.GetLongitude()), Convert.ToDouble(Request.GetLatitude()), 0, city, 0, new int[] { grade }, orderBy: 2, new int[] { }, 0, new int[] { }, 1, count);
                   if (list.List.Any())
                   {
                       //data.Add($"comment_{grade}", list.List);
                       data.Add(list.List);
                   }
                   else
                   {
                       data.Add(null);
                   }
               }
               return data;
           }, time);

            //检测评分数据是否齐全，不齐全则取全国数据
            if (scoreSchool.Where(x => x == null).FirstOrDefault() == null)
            {
                for (int i = 0; i < scoreSchool.Count(); i++)
                {
                    string key = string.Format(_SchoolScoreHottestKey, (i + 1));
                    if (scoreSchool[i] == null)
                    {
                        var list = await _easyRedisClient.GetAsync<List<SchoolExtItemDto>>(key);
                        if (list != null)
                        {
                            scoreSchool[i] = list;
                        }
                        else
                        {
                            var rez = _schoolService.GetSchoolExtListAsync(0, 0, 0, 0, 0, new int[] { (i + 1) }, orderBy: 2, new int[] { }, 0, new int[] { }, 1, 10).Result;
                            await _easyRedisClient.AddAsync(key, rez.List, time, StackExchange.Redis.CommandFlags.FireAndForget);
                            scoreSchool[i] = rez.List;
                        }
                    }
                }
            }

            //检测点评最高的学校是否存在数据，不存在则获取全国数据
            if (commentSchool.Where(x => x == null).FirstOrDefault() == null)
            {
                for (int i = 0; i < commentSchool.Count(); i++)
                {
                    string key = string.Format(_CommentHottestKey, (i + 1));
                    if (commentSchool[i] == null)
                    {
                        var list = await _easyRedisClient.GetAsync<List<SchoolExtItemDto>>(key);
                        if (list != null)
                        {
                            commentSchool[i] = list;
                        }
                        else
                        {
                            var rez = _schoolService.GetSchoolExtListAsync(0, 0, 0, 0, 0, new int[] { (i + 1) }, orderBy: 1, new int[] { }, 0, new int[] { }, 1, 10).Result;
                            await _easyRedisClient.AddAsync(key, rez.List, time, StackExchange.Redis.CommandFlags.FireAndForget);
                            commentSchool[i] = rez.List;
                        }
                    }
                }
            }

            //异步等待
            //await Task.WhenAll(hotschoolTask, scoreSchoolTask, commentSchoolTask);
            //var hotschool = await hotschoolTask;
            //var scoreSchool = await scoreSchoolTask;
            //var commentSchool = await commentSchoolTask;

            return (hotschool, scoreSchool, commentSchool);
        }

        /// <summary>
        /// 首页底部学校列表导航
        /// </summary>
        private async Task GetSchoolNav(int city = 0)
        {
            if (city == 0)
                city = Request.GetLocalCity();
            //获取当前的城市
            var localCityCode = city == 0 ? Request.GetLocalCity() : city;
            var types = _schoolService.GetSchoolTypeList();
            var areaList = await _cityService.GetAreaCode(localCityCode);


            var internaTypeList = types.Where(q => q.International == 1).ToList();
            var privateTypeList = types.Where(q => q.TypeId == (int)PMS.OperationPlateform.Domain.Enums.SchoolType.Private
                    || (q.International == 1 && q.Diglossia == 1)).ToList();
            var publicTypeList = types.Where(q => q.TypeId == (int)PMS.OperationPlateform.Domain.Enums.SchoolType.Public).ToList();

            List<SchoolListNavViewModel> list = new List<SchoolListNavViewModel>()
            {
                new SchoolListNavViewModel
                {
                    No = 1,
                    Name = "国际化学校",
                    Id = "internationalSchool",
                    List = areaList.Select(q=>new SchoolNavViewModel{
                        Title = q.AreaName + "区国际化学校",
                        Src = Url.Action("List","School",new { option = $"ar{q.AdCode}_" + string.Join("",internaTypeList.Select(p=>"ty"+p.TypeValue))+"_gs1" })
                    }).ToList()
                },
                new SchoolListNavViewModel
                {
                    No = 2,
                    Name = "民办类学校",
                    Id = "privateSchools",
                    List = areaList.Select(q=>new SchoolNavViewModel{
                        Title = q.AreaName + "区民办类学校",
                        Src = Url.Action("List","School",new { option = $"ar{q.AdCode}_" + string.Join("",privateTypeList.Select(p=>"ty"+p.TypeValue))+"_gs1" })
                    }).ToList()
                },
                new SchoolListNavViewModel
                {
                    No = 3,
                    Name = "公办类学校",
                    Id = "publicSchool",
                    List = areaList.Select(q=>new SchoolNavViewModel{
                        Title = q.AreaName + "区公办类学校",
                        Src = Url.Action("List","School",new { option = $"ar{q.AdCode}_" + string.Join("",publicTypeList.Select(p=>"ty"+p.TypeValue))+"_gs1" })
                    }).ToList()
                }
            };
            ViewBag.SchoolNav = list;
        }

        [Description("帮助中心")]
        public IActionResult Help(int idx = 0, Guid eid = default)
        {
            string name = "", addr = "";
            if (eid != default)
            {
                var school = _schoolService.SchoolFeedback(eid);
                name = school.Name;
                addr = school.Address;
            }

            ViewBag.name = name;
            ViewBag.addr = addr;
            ViewBag.eid = eid;
            ViewBag.gradetype = _schoolService.GetGradeTypeList();
            return View();
        }

        [Description("学校纠错")]
        public IActionResult SchCorrection()
        {
            return View();
        }

        /// <summary>
        /// 提交点评回复
        /// </summary>
        /// <returns></returns>
        [Authorize]
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

                bool insert = _reportService.AddFeedback(feedback);

                if (insert)
                {
                    //数据添加成功
                    if (imagesArr.Any())
                    {
                        //带有图片
                        var imgs = HelperCenterUploadImage(imagesArr, feedback.Id);
                        _reportService.AddFeedbackImg(imgs);
                    }
                }
                return ResponseResult.Success();
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        [Authorize]
        [HttpPost]
        public ResponseResult FeedbackCorrected(FeedbackCorrectedViewModel feedbackCorrected)
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

                bool insert = _reportService.AddFeedback(feedback);
                if (insert)
                {
                    if (feedbackCorrected.ImagesArr != null)
                    {
                        if (feedbackCorrected.ImagesArr.Any())
                        {
                            //带有图片
                            var imgs = HelperCenterUploadImage(feedbackCorrected.ImagesArr, feedback.Id);
                            _reportService.AddFeedbackImg(imgs);
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
                    _reportService.AddfeedbackCorrect(corrected);
                }
                return ResponseResult.Success();
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        public ResponseResult PrincipleSch(string keyword)
        {
            List<PrincipleSchViewModel> schViewModels = new List<PrincipleSchViewModel>();
            if (!String.IsNullOrEmpty(keyword))
            {
                var searchRez = _dataSearch.SearchSchool(new PMS.Search.Application.ModelDto.Query.SearchSchoolQuery()
                {
                    Keyword = keyword
                });

                if (searchRez.Total > 0)
                {
                    searchRez.Schools.ForEach(x => schViewModels.Add(new PrincipleSchViewModel() { ExtId = x.Id, Name = x.Name }));
                }
            }
            return ResponseResult.Success(schViewModels);
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




        /// <summary>
        /// 第三方页面跳转中转
        /// </summary>
        /// <returns></returns>
        [Description("第三方页面跳转中转")]
        [Route("jump")]
        public ActionResult Jump(string url)
        {
            url = string.IsNullOrEmpty(url) ? "/" : Uri.UnescapeDataString(url);
            return Redirect(url);
        }

        /// <summary>
        /// 文章背景图转string 链接
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        string Article_CoverToLink(article_cover c)

        {
            return !string.IsNullOrWhiteSpace(c.ImgUrl) ? c.ImgUrl : $"https://cos.sxkid.com/images/article/{c.articleID}/{c.photoID}.{((FileExtension)c.ext).ToString()}";
        }
    }
}
