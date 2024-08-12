using AutoMapper;
using iSchool;
using iSchool.Library;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NSwag.Annotations;
using PMS.CommentsManage.Application.Common;
using PMS.CommentsManage.Application.IServices;
using PMS.Infrastructure.Application.IService;
using PMS.Infrastructure.Application.ModelDto;
using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using PMS.School.Application.IServices;
using PMS.School.Application.ModelDto;
using PMS.School.Domain.Common;
using PMS.School.Domain.Dtos;
using PMS.School.Infrastructure.Common;
using PMS.Search.Application.IServices;
using PMS.Search.Application.ModelDto.Query;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Common;
using ProductManagement.API.Http.Interface;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using Sxb.Web.Common;
using Sxb.Web.Models;
using Sxb.Web.Models.Comment;
using Sxb.Web.RequestModel.School;
using Sxb.Web.Response;
using Sxb.Web.Utils;
using Sxb.Web.ViewModels.Article;
using Sxb.Web.ViewModels.School;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace Sxb.Web.Controllers
{
    [OpenApiIgnore]
    public class SchoolController : Controller
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
        private readonly IArticleService _articleService;
        private readonly IArticleCoverService _articleCoverService;
        private readonly ImageSetting _setting;
        private readonly IQuestionInfoService _questionInfoService;
        private readonly IUserServiceClient _userServiceClient;
        private readonly ISchoolInfoService _schoolInfo;
        private readonly ISchService _schService;
        private readonly ISchoolCommentScoreService _commentScoreService;
        private readonly ILSRFSchoolDetailService _lSRFSchoolDetail;
        private readonly ISchoolScoreService _scoreService;

        //private readonly string _AddRankKey = "addRank";
        private readonly string _ExtRecommend = "ad:Recommend:{0}";

        public SchoolController(ISchService schService, ISchoolService schoolService, ICityInfoService cityService, IHttpClientFactory clientFactory, IEasyRedisClient easyRedisClient, IUserService userService, IMapper mapper,
           IArticleService articleService, IArticleCoverService articleCoverService, IQuestionInfoService questionInfoService, IUserServiceClient userServiceClient, ISearchService dataSearch,
           ISchoolCommentService schoolComment, ISchoolInfoService schoolInfo, ISchoolCommentScoreService commentScoreService, IConfiguration iConfig
            , ILSRFSchoolDetailService lSRFSchoolDetail, ISchoolScoreService scoreService, IOptions<ImageSetting> set)
        {
            _dataSearch = dataSearch;
            _schService = schService;
            _schoolService = schoolService;
            _cityService = cityService;
            _configuration = iConfig;
            _clientFactory = clientFactory;
            _easyRedisClient = easyRedisClient;
            _mapper = mapper;
            _questionInfoService = questionInfoService;
            _userService = userService;
            _articleService = articleService;
            _articleCoverService = articleCoverService;
            _userServiceClient = userServiceClient;
            _commentService = schoolComment;
            _schoolInfo = schoolInfo;
            _commentScoreService = commentScoreService;
            _setting = set.Value;
            _lSRFSchoolDetail = lSRFSchoolDetail;

            _scoreService = scoreService;
        }

        /// <summary>
        /// ajax 获取学校的划片范围
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        [Description("获取学校的划片范围")]
        public ResponseResult ExtRange(Guid extId)
        {
            var range = _schoolService.GetSchoolRange(extId);
            return ResponseResult.Success(range);
        }

        /// <summary>
        /// 获取更多的招生信息
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="extId"></param>
        /// <returns></returns>
        [Route("/{controller}/{action}/{extId}")]
        [Route("/{controller}-{schoolNo}/{action}")]
        [Description("招生信息")]
        public async Task<ActionResult> ExtRecruit(Guid extId = default(Guid), Guid id = default(Guid), string schoolNo = default)
        {
            //兼容旧的seo
            if (extId == default(Guid)) extId = id;
            if (!string.IsNullOrWhiteSpace(schoolNo))
            {
                var schExt = _schService.GetSchextSimpleInfoViaShortNo(schoolNo);
                if (schExt?.Result.Eid != default)
                {
                    extId = schExt.Result.Eid;
                }
            }

            var data = _schoolService.GetSchoolExtRecruit(extId);

            //如果学校分部为null的话跳转到错误页面
            if (data == null)
                return new ExtNotFoundViewResult();

            //添加历史
            _userServiceClient.AddHistory(new ProductManagement.API.Http.Model.AddHistory { dataID = extId, dataType = MessageDataType.School, cookies = HttpContext.Request.GetAllCookies() });

            //招生对象
            var targer = string.IsNullOrEmpty(data.Target) ? new List<KeyValueDto<string>>() : JsonHelper.JSONToObject<List<KeyValueDto<string>>>(data.Target);
            ViewBag.Target = targer;
            //招生比例
            var proportion = "-";
            if (data.Proportion != null && data.Proportion != 0)
            {
                var pro = Math.Round(Convert.ToDecimal(data.Proportion), 1);
                //var proNumber = NumberHelper.ConvertToFraction((float)pro);
                proportion = $"1:{pro}";
            }
            ViewBag.Proportion = proportion;
            //录取分数线
            var point = string.IsNullOrEmpty(data.Point) ? new List<KeyValueDto<string>>() : JsonHelper.JSONToObject<List<KeyValueDto<string>>>(data.Point);
            ViewBag.Point = point;
            //招生日期
            var date = string.IsNullOrEmpty(data.Date) ? new List<KeyValueDto<string>>() : JsonHelper.JSONToObject<List<KeyValueDto<string>>>(data.Date);
            ViewBag.Date = date;
            //报名科目
            var subject = string.IsNullOrEmpty(data.Subjects) ? new List<KeyValueDto<string>>() : JsonHelper.JSONToObject<List<KeyValueDto<string>>>(data.Subjects);
            ViewBag.Subjects = subject;
            //学校荣誉
            ViewBag.Schoolhonor = StrHelper.GetHtmlImageUrlList(data.Schoolhonor);
            //学生荣誉
            ViewBag.Studenthonor = StrHelper.GetHtmlImageUrlList(data.Studenthonor);
            //校长风采
            ViewBag.Principal = StrHelper.GetHtmlImageUrlList(data.Principal);
            //教师风采
            ViewBag.Teacher = StrHelper.GetHtmlImageUrlList(data.Teacher);
            //硬件设施
            ViewBag.Hardware = StrHelper.GetHtmlImageUrlList(data.Hardware);
            //社团活动
            ViewBag.Community = StrHelper.GetHtmlImageUrlList(data.Community);
            //各个年级课程表
            ViewBag.TimeTables = StrHelper.GetHtmlImageUrlList(data.TimeTables);
            //作息时间表
            ViewBag.Schedule = StrHelper.GetHtmlImageUrlList(data.Schedule);
            //校车路线
            ViewBag.Diagram = StrHelper.GetHtmlImageUrlList(data.Diagram);

            var examYears = _schoolService.GetSchoolFieldYears(extId.ToString(), "Pastexam");
            if (examYears?.Any() == true)
            {
                ViewBag.ExamYears = JSONHelper.ToJSON(examYears.Select(p => new
                {
                    id = p,
                    value = p
                }));
            }
            var dataYears = _schoolService.GetSchoolFieldYears(extId.ToString(), "Data");
            if (dataYears?.Any() == true)
            {
                ViewBag.dataYears = JSONHelper.ToJSON(dataYears.Select(p => new
                {
                    id = p,
                    value = p
                }));
            }
            var dateYears = _schoolService.GetSchoolFieldYears(extId.ToString(), "Date");
            if (dateYears?.Any() == true)
            {
                ViewBag.dataYears = JSONHelper.ToJSON(dateYears.Select(p => new
                {
                    id = p,
                    value = p
                }));
            }
            var scholarShipYears = _schoolService.GetSchoolFieldYears(extId.ToString(), "ScholarShip");
            if (scholarShipYears?.Any() == true)
            {
                ViewBag.scholarShipYears = JSONHelper.ToJSON(scholarShipYears.Select(p => new
                {
                    id = p,
                    value = p
                }));
            }

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
                //var request = new HttpRequestMessage(HttpMethod.Get, $"{ip}{port}?userId={user.UserId}&&dataId={extId}");
                //var client = _clientFactory.CreateClient();
                try
                {
                    //var response = await client.SendAsync(request);
                    //if (response.StatusCode == HttpStatusCode.OK)
                    //{
                    //var responseResult = await response.Content.ReadAsStringAsync();
                    //var dir = JsonHelper.DataRowFromJSON(responseResult);
                    var isCollection = await _userServiceClient.IsCollected(new ProductManagement.API.Http.Model.IsCollected { dataID = extId, userId = user.UserId });
                    if (isCollection.status == 0)
                    {
                        if (isCollection.iscollected == true)
                        {
                            collectStatat = true;
                        }
                    }
                    //}
                }
                catch (Exception)
                {
                    collectStatat = false;
                }
            }
            ViewBag.CollectStatus = collectStatat;
            ViewBag.UserName = userName;
            var schext = await _schService.GetSchextSimpleInfo(extId);
            ViewBag.Sid = schext.Sid;
            ViewBag.ExtId = extId;
            ViewBag.SchFType = new SchFType0(schext.Grade, schext.Type, schext.Discount, schext.Diglossia, schext.Chinese);
            return View(data);
        }

        /// <summary>
        /// 获取详细的升学成绩
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="extId"></param>
        /// <returns></returns>
        [Description("升学成绩")]
        [Route("/{controller}/{action}/{extId}")]
        public async Task<ActionResult> ExtAchievement(Guid sid, Guid extId, byte grade, byte type)
        {
            //国际升学成绩
            List<AchievementInfos> interData = null;
            //国内升学情况
            List<KeyValueDto<int, string, double, byte, Guid>> localData = null;

            ViewBag.Grade = grade;
            ViewBag.Type = type;
            //获取学校的升学成绩数据
            var data = _schoolService.GetAchievementList(extId, grade);

            //添加历史足迹
            _ = _userServiceClient.AddHistory(new ProductManagement.API.Http.Model.AddHistory { dataID = extId, dataType = MessageDataType.School, cookies = HttpContext.Request.GetAllCookies() });

            //年级为国际高中
            if (grade == (byte)(PMS.School.Domain.Common.SchoolGrade.SeniorMiddleSchool) && type == (byte)(PMS.OperationPlateform.Domain.Enums.SchoolType.International))
            {
                //国内大学 1  国外大学 2
                //区分国际榜单跟国内榜单
                //获取国际榜单
                var rank = await _schService.GetCollegeRankList();
                if (rank != null)
                {
                    foreach (var item in rank)
                    {
                        foreach (var p in item.List)
                        {
                            var year = p.Year;
                            foreach (var c in p.Items)
                            {
                                var count = data.Where(d => d.Other == c.SchoolId && d.Key == year).FirstOrDefault();
                                c.Count = count == null ? 0 : (int)count.Message;
                            }
                        }
                    }
                    interData = rank;
                }
                //获得国内升学成绩
                localData = data.Where(p => p.Data == 1).ToList();
            }
            else
            {
                localData = data;
            }
            //获取年份
            var years = localData.Select(p => p.Key).Distinct().OrderByDescending(p => p).ToList();
            ViewBag.Year = years;
            ViewBag.DataJson = JsonHelper.ObjectToJSON(localData);

            ViewBag.InterData = interData;
            ViewBag.InterJson = interData == null ? JsonHelper.ObjectToJSON(new List<AchievementInfos>()) : JsonHelper.ObjectToJSON(interData);
            return View(localData);
        }


        /// <summary>
        /// 对升学成绩进行筛选
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Description("对升学成绩进行筛选")]
        public ActionResult QueryAchievement(string data, string years, string orderBy)
        {
            var achData = JsonHelper.JSONToObject<List<KeyValueDto<int, string, float>>>(data);
            var yearData = string.IsNullOrEmpty(years) ? new List<string>() : years.Split(",").ToList();
            //筛选年份
            if (yearData.Count() != 0)
                achData = achData.Where(p => yearData.Contains(p.Key.ToString())).ToList();
            // 0 人数   1 名字
            if (orderBy == "0")
            {
                achData = achData.OrderByDescending(p => p.Key).ThenByDescending(p => p.Message).ToList();
            }
            else
            {
                achData = achData.OrderByDescending(p => p.Key).ThenBy(p => p.Value).ToList();
            }
            var result = achData.GroupBy(p => p.Key).Select(p => new { year = p.Key, list = p.Select(x => new { school_name = x.Value, people_num = x.Message }) });
            return Json(result);
        }

        /// <summary>
        /// 获取学部video列表
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        [Description("学部视频")]
        [Route("/{controller}/{action}/{extId}")]
        public ActionResult ExtVideos(Guid extId)
        {
            var videos = _schoolService.GetExtVideo(extId, (byte)VideoType.Interview);

            //添加历史
            _userServiceClient.AddHistory(new ProductManagement.API.Http.Model.AddHistory { dataID = extId, dataType = MessageDataType.School, cookies = HttpContext.Request.GetAllCookies() });

            return View(videos);
        }

        /// <summary>
        /// 评分细项
        /// </summary>
        /// <returns></returns>
        [Description("评分细项")]
        [Route("/{controller}/{action}/{extId}")]
        public async Task<ActionResult> ExtFraction(Guid sid, Guid extId, string SchoolName, string ExtName)
        {
            //判断是否收藏
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
                //var request = new HttpRequestMessage(HttpMethod.Get, $"{ip}{port}?userId={user.UserId}&&dataId={extId}");
                //var client = _clientFactory.CreateClient();
                //var response = await client.SendAsync(request);
                //if (response.StatusCode == HttpStatusCode.OK)
                //{
                //var responseResult = await response.Content.ReadAsStringAsync();
                //var dir = JsonHelper.DataRowFromJSON(responseResult);
                var isCollection = await _userServiceClient.IsCollected(new ProductManagement.API.Http.Model.IsCollected { dataID = extId, userId = user.UserId });
                if (isCollection.status == 0)
                {
                    if (isCollection.iscollected == true)
                    {
                        collectStatat = true;
                    }
                }
                //}
            }
            //获取学校的特征
            //var extCharacter = await _schoolService.GetSchoolCharacterAsync(extId);
            var scoreData = await GetSchoolExtScoreAsync(extId);
            ViewBag.ScoreIndex = scoreData.index;

            //添加历史足迹
            _userServiceClient.AddHistory(new ProductManagement.API.Http.Model.AddHistory { dataID = extId, dataType = MessageDataType.School, cookies = HttpContext.Request.GetAllCookies() });

            //ViewBag.Character = extCharacter;
            ViewBag.CollectStatus = collectStatat;
            ViewBag.UserName = userName;
            ViewBag.Sid = sid;
            ViewBag.ExtId = extId;
            ViewBag.SchoolName = SchoolName;
            ViewBag.ExtName = ExtName;
            return View(scoreData.score);
        }

        /// <summary>
        /// 学校（学部）筛选列表页
        /// </summary>
        [Description("学校筛选列表页")]
        [Route("/{controller}")]
        [Route("/{controller}/{action}")]
        public async Task<ActionResult> ExtSchoolFilter(int type)
        {
            int cityCode = Request.GetLocalCity();
            ViewBag.CityCode = cityCode;

            List<AreaDto> areas = new List<AreaDto>();
            List<MetroDto> metros = new List<MetroDto>();
            List<TagDto> tags = new List<TagDto>();
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

            List<string> selectTypeValues = new List<string>();
            switch (type)
            {
                case 1:
                    selectTypeValues.AddRange(new List<string>{
                        //"1.99.0.0.0","1.1.0.0.0","2.1.0.0.0", "3.1.0.0.0","4.1.0.0.0"
                        "lx199","lx110","lx210","lx310","lx410"
                    });
                    break;

                case 2:
                    selectTypeValues.AddRange(new List<string>
                    {
                         //"1.2.0.0.0", "1.2.1.0.0","2.2.0.0.0", "2.3.0.1.0", "3.3.0.1.0","3.2.0.0.0","4.3.0.1.0","4.2.0.0.0"
                         "lx120","lx121","lx220","lx320","lx420","lx221","lx231","lx331","lx431"
                    });
                    break;

                case 3:
                    selectTypeValues.AddRange(new List<string>
                    {
                        //"1.3.0.0.0", "2.3.0.0.0","3.3.0.0.0","4.3.0.0.1","4.3.0.0.0", "2.3.0.1.0", "3.3.0.1.0","4.3.0.1.0"
                        "lx130","lx432","lx430","lx230","lx231","lx330","lx331","lx431","lx432"
                    });
                    break;
            }

            ViewBag.SelectedType = selectTypeValues;

            return View();
        }

        /// <summary>
        /// 通过学校ID获取相关（学部）列表 Json
        /// </summary>
        /// <param name="schoolId"></param>
        /// <returns></returns>
        [HttpGet]
        [Description("通过学校ID获取相关学部列表Json")]
        public async Task ListExtSchoolBySchoolId(string callBack, Guid schoolId)
        {
            var list = await _schoolService.ListExtSchoolBySchoolId(schoolId);
            var result = ResponseResult.Success(list.Select(q => new SchoolExtListItemViewModel
            {
                SchoolId = q.Sid,
                BranchId = q.ExtId,
                SchoolName = q.Name,
                CityName = q.City,
                AreaName = q.Area,
                Distance = q.Distance,
                LodgingType = (int)q.LodgingType,
                LodgingReason = q.LodgingType.Description(),
                International = q.International,
                Cost = q.Tuition,
                Tags = q.Tags,
                Score = q.Score,
                CommentTotal = q.CommentCount,
                Type = q.Type,
                SchoolNo = q.SchoolNo
            }));
            string response = JsonConvert.SerializeObject(result, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            string call = callBack + "(" + response + ")";
            Response.ContentType = "text/plain; charset=utf-8";
            await Response.WriteAsync(call, Encoding.UTF8);
        }

        /// <summary>
        /// 学校筛选列表 (为解决大明PC跳转移动端)
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [Description("学校筛选列表Json")]
        public async Task<ActionResult> ListExtSchool(SchoolExtFilterData query, int type)
        {
            int cityCode = Request.GetLocalCity();
            ViewBag.CityCode = cityCode;

            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                if (!string.IsNullOrWhiteSpace(query.KeyWords))
                {
                    string Key = $"SearchHistory:{user.UserId}";
                    await _easyRedisClient.SortedSetAddAsync(Key, query.KeyWords, DateTime.Now.D2I(), StackExchange.Redis.CommandFlags.FireAndForget);
                }
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

            var list = _dataSearch.SearchSchool(new SearchSchoolQuery
            {
                Keyword = query.KeyWords,
                Latitude = double.Parse(lat),
                Longitude = double.Parse(lng),
                CurrentCity = cityCode,
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

            //if (!string.IsNullOrWhiteSpace(json) && json.ToLower()=="yes")
            if (Request.IsAjax())
            {
                return Json(ResponseResult.Success(result.Select(q => new SchoolExtListItemViewModel
                {
                    SchoolId = q.Sid,
                    BranchId = q.ExtId,
                    SchoolName = q.Name,
                    CityName = q.City,
                    AreaName = q.Area,
                    Distance = q.Distance,
                    LodgingType = (int)q.LodgingType,
                    LodgingReason = q.LodgingType.Description(),
                    International = q.International,
                    Cost = q.Tuition,
                    Tags = q.Tags,
                    Score = q.Score,
                    CommentTotal = q.CommentCount
                })));
            }
            else
            {
                ViewBag.CityCode = cityCode;

                List<AreaDto> areas = new List<AreaDto>();
                List<MetroDto> metros = new List<MetroDto>();
                List<TagDto> tags = new List<TagDto>();
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

                List<string> selectTypeValues = new List<string>();
                switch (type)
                {
                    case 1:
                        selectTypeValues.AddRange(new List<string>{
                        //"1.99.0.0.0","1.1.0.0.0","2.1.0.0.0", "3.1.0.0.0","4.1.0.0.0"
                        "lx199","lx110","lx210","lx310","lx410"
                    });
                        break;

                    case 2:
                        selectTypeValues.AddRange(new List<string>
                    {
                         //"1.2.0.0.0", "1.2.1.0.0","2.2.0.0.0", "2.3.0.1.0", "3.3.0.1.0","3.2.0.0.0","4.3.0.1.0","4.2.0.0.0"
                         "lx120","lx121","lx220","lx23","lx33","lx320","lx43","lx420"
                    });
                        break;

                    case 3:
                        selectTypeValues.AddRange(new List<string>
                    {
                        //"1.3.0.0.0", "2.3.0.0.0","3.3.0.0.0","4.3.0.0.1","4.3.0.0.0", "2.3.0.1.0", "3.3.0.1.0","4.3.0.1.0"
                        "lx130","lx23","lx33","lx432","lx43","lx23","lx33","lx43"
                    });
                        break;
                }

                ViewBag.SelectedType = selectTypeValues;

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
                        CommentTotal = r.CommentCount
                    };
                });
                var nextPageUrl = ActionUrlM(() =>
                {
                    query.PageNo += 1;
                    return query;
                });
                ViewBag.NextPageUrl = nextPageUrl;
                return View();
            }
        }

        /// <summary>
        /// 学校筛选列表(为解决大明PC跳转移动端)
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [Description("学校筛选列表")]
        public async Task<ActionResult> ListExtSchoolPageM(SchoolExtFilterData query)
        {
            int cityCode = Request.GetLocalCity();
            ViewBag.CityCode = cityCode;

            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                if (!string.IsNullOrWhiteSpace(query.KeyWords))
                {
                    string Key = $"SearchHistory:{user.UserId}";
                    await _easyRedisClient.SortedSetAddAsync(Key, query.KeyWords, DateTime.Now.D2I(), StackExchange.Redis.CommandFlags.FireAndForget);
                }
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
            var list = _dataSearch.SearchSchool(new SearchSchoolQuery
            {
                Keyword = query.KeyWords,
                Latitude = double.Parse(lat),
                Longitude = double.Parse(lng),
                CurrentCity = cityCode,
                CityCode = query.CityCode,
                AreaCodes = query.AreaCodes,
                MetroIds = metroIds,
                Distance = query.Distance ?? 0,
                Type = query.Type,
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
                    CommentTotal = r.CommentCount
                };
            });
            //var req = Request.Query.Where(q => q.Key != "PageNo").Select(kv => string.Join("&", kv.Value.Select(p => $"{kv.Key}={p}")));
            //var data = string.Join("&", req);
            //ViewBag.NextPageUrl = @"/School/ListExtSchoolPage?" + data + "&PageNo=" + (PageNo + 1);

            var nextPageUrl = ActionUrlM(() =>
            {
                query.PageNo += 1;
                return query;
            });
            ViewBag.NextPageUrl = nextPageUrl;

            return View();
        }

        private string ActionUrlM(Func<SchoolExtFilterData> func)
        {
            var query = func.Invoke();
            return Url.Action("listextschoolpagem", "school", query);
        }

        /// <summary>
        /// 学校（学部）筛选列表局部页
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [Description("学校筛选列表")]
        public async Task<ActionResult> ListExtSchoolPage(SchoolExtFilterData query)
        {
            //int cityCode = Request.GetLocalCity();
            //if (query.CityCode == 0)
            //    query.CityCode = cityCode;
            //var result = await _schoolService.PageExtSchool(new ListExtSchoolQuery
            //{
            //    CityCode = new List<int> { query.CityCode },
            //    AreaCodes = query.AreaCodes,
            //    MetroLineIds = query.MetroLineIds,
            //    MetroStationIds = query.MetroStationIds,
            //    Distance = query.Distance,
            //    Type = query.Type,
            //    MinCost = query.MinCost,
            //    MaxCost = query.MaxCost,
            //    Lodging = query.Lodging,
            //    AuthIds = query.AuthIds,
            //    CharacIds = query.CharacIds,
            //    AbroadIds = query.AbroadIds,
            //    CourseIds = query.CourseIds,
            //    Orderby = query.Orderby,
            //    PageNo = PageNo,
            //    PageSize = PageSize
            //});

            int cityCode = Request.GetLocalCity();
            ViewBag.CityCode = cityCode;

            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                if (!string.IsNullOrWhiteSpace(query.KeyWords))
                {
                    string Key = $"SearchHistory:{user.UserId}";
                    await _easyRedisClient.SortedSetAddAsync(Key, query.KeyWords, DateTime.Now.D2I(), StackExchange.Redis.CommandFlags.FireAndForget);
                }
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
            var list = _dataSearch.SearchSchool(new SearchSchoolQuery
            {
                Keyword = query.KeyWords,
                Latitude = double.Parse(lat),
                Longitude = double.Parse(lng),
                CurrentCity = cityCode,
                CityCode = new List<int> { cityCode },
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

            var schoolNos = new (Guid, int)[0];
            if (list?.Schools?.Any() == true)
            {
                var sids = list.Schools.Select(p => p.Id).Distinct().ToArray();
                schoolNos = _schService.GetSchoolextNo(sids);
            }

            var result = ids.Count > 0 ? await _schoolService.ListExtSchoolByBranchIds(ids) : new List<SchoolExtFilterDto>();

            ViewBag.Data = list.Schools.Where(q => result.Any(p => p.ExtId == q.Id)).Select(q =>
            {
                var r = result.FirstOrDefault(p => p.ExtId == q.Id);
                var n = schoolNos.FirstOrDefault(p => p.Item1 == q.Id);
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
                    SchoolNo = n.Item2
                };
            });
            //var req = Request.Query.Where(q => q.Key != "PageNo").Select(kv => string.Join("&", kv.Value.Select(p => $"{kv.Key}={p}")));
            //var data = string.Join("&", req);
            //ViewBag.NextPageUrl = @"/School/ListExtSchoolPage?" + data + "&PageNo=" + (PageNo + 1);

            var nextPageUrl = ActionUrl(() =>
            {
                query.PageNo += 1;
                return query;
            });
            ViewBag.NextPageUrl = nextPageUrl;

            return View();
        }

        private string ActionUrl(Func<SchoolExtFilterData> func)
        {
            var query = func.Invoke();
            return Url.Action("listextschoolpage", "school", query);
        }

        /// <summary>
        /// 学校收藏列表
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        [HttpPost]
        [Description("动态学校收藏列表")]
        public ActionResult GetCollectionExt([FromBody] Guid[] extId)
        {
            //, Guid.Parse("c3b8174a-0d4e-430f-a00f-f3f722c6a853")
            //var extId = new Guid[] { Guid.Parse("00000000-0000-0000-0000-000000000000") };
            var result = _mapper.Map<List<SchoolCollectionDto>, List<SchoolCollectionVo>>(_schoolService.GetCollectionExtsAsync(extId).Result);
            var score = _schoolInfo.GetSchoolSectionByIds(result.Select(x => x.ExtId)?.ToList());
            //var adCodes = (await _cityService.IntiAdCodes()).SelectMany(p => p.CityCodes);
            //var cityCodes = result.Select(p => p.City.Value).Distinct().ToList();
            //List<AreaDto> areaList = new List<AreaDto>();
            //cityCodes.ForEach(async p =>
            //{
            //    areaList.AddRange(await _cityService.GetAreaCode(p));
            //});
            result.ForEach(x =>
            {
                var temp = score.Where(s => s.SchoolSectionId == x.ExtId).FirstOrDefault();
                if (temp != null)
                {
                    x.QuestionCount = temp.SectionQuestionTotal;
                    x.CommentCount = temp.CommentTotal;
                    x.SchoolAvgScore = temp.SchoolAvgScore;
                    x.SchoolStars = temp.SchoolStars;
                    //x.CityName = adCodes.FirstOrDefault(p => p.Id == x.City)?.Name;
                    //x.AreaName = areaList.FirstOrDefault(p => p.AdCode == x.Area)?.AreaName;
                }
            });
            return Json(result);
        }

        /// <summary>
        /// 学校卡片信息
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        public async Task<ResponseResult> GetSchoolCardInfo(Guid extId)
        {
            try
            {
                //获取分部信息
                var school = await _schoolService.GetSchoolExtensionDetails(extId);

                //var scoreData = await GetSchoolExtScoreAsync(extId);
                //var score = scoreData.score.FirstOrDefault(p => p.IndexId == 22)?.Score;
                //var scoreLevel = score == null ? "暂未收录" : Codstring.GetScoreString(score.Value);

                var scoreLevel = school.ExtPoint == null ? "暂未收录" : Codstring.GetScoreString(school.ExtPoint.Value);
                var info = new
                {
                    ExtId = school.ExtId,
                    SchoolName = school.SchoolName + " - " + school.ExtName,
                    Address = school.Address,
                    CityName = school.CityName,
                    AreaName = school.AreaName,
                    ShortSchoolNo = UrlShortIdUtil.Long2Base32(school.SchoolNo),
                    school.TuitionPerYearFee,
                    school.LodgingType,
                    LodgingTypeName = school.LodgingType.GetDescription(),
                    SchoolTypeName = ((PMS.OperationPlateform.Domain.Enums.SchoolType)school.Type).GetDescription(),
                    Score = scoreLevel
                };
                return ResponseResult.Success(info);

            }
            catch (Exception ex)
            {
                return ResponseResult.Failed(ex.Message);
            }
        }

        /// <summary>
        /// 学校分部详情
        /// 查询数据添加缓存
        /// </summary>
        /// <returns></returns>
        [Description("学部详情")]
        [Route("/school-{schoolNo}")]
        [Route("/school-{schoolNo}/{params}")]
        [Route("/{controller}/{action}/{extId}")]
        public async Task<IActionResult> Detail(Guid extId = default(Guid), Guid sid = default(Guid), Guid id = default(Guid),
             CourseType CourseType = CourseType.Unknown, string schoolNo = default)
        {
            var latitude = Request.GetLatitude();
            var longitude = Request.GetLongitude();

            //兼容旧的seo
            if (extId == default) { extId = id; }

            if (!string.IsNullOrWhiteSpace(schoolNo))
            {
                var schExt = _schService.GetSchextSimpleInfoViaShortNo(schoolNo);
                if (schExt?.Result?.Eid != default)
                {
                    extId = schExt.Result.Eid;
                }
                else
                {
                    return new ExtNotFoundViewResult();
                }
            }

            var data = await _schoolService.GetSchoolExtSimpleDtoAsync(Convert.ToDouble(longitude), Convert.ToDouble(latitude), extId);
            //如果学校分部为null的话跳转到错误页面
            if (data == null) return new ExtNotFoundViewResult();

            if (schoolNo != default)
            {
                data.SchoolNo = UrlShortIdUtil.Base322Int(schoolNo);
            }
            else
            {
                var schoolNos = _schService.GetSchoolextNo(new Guid[] { extId });
                if (schoolNos?.Any() == true)
                {
                    data.SchoolNo = schoolNos.First().Item2;
                }
            }

            if (data.City == 440100 || data.City == 440300 || data.City == 500100 || data.City == 510100)
            {
                return RedirectPermanent($"/school/detail/{data.ShortSchoolNo}/");
            }
            ViewBag.Grade = data.Grade;

            var schoolVideos = await _schoolService.GetSchoolVideos(data.ExtId);
            var schoolExtInfo = await _schoolService.GetSchoolExtDtoAsync(data.ExtId, data.Latitude.Value, data.Longitude.Value);
            var brandImages = await _schoolService.GetSchoolImages(data.ExtId, new PMS.School.Domain.Enum.SchoolImageType[] { PMS.School.Domain.Enum.SchoolImageType.SchoolBrand });

            //获取学校图册模块相关数据
            if (schoolVideos == null || !schoolVideos.Any())
            {
                var images = await _schoolService.GetSchoolImages(extId, null);

                if (images?.Any() == true)
                {
                    images = images.OrderBy(p => p.Type).ToList();
                    data.SchoolPhotos = new List<KeyValueDto<string, string, string>>();
                    data.SchoolPhotos.AddRange(images.Where(p => p.Type == PMS.School.Domain.Enum.SchoolImageType.SchoolHonor).OrderBy(p => p.Sort).
                        Select(p => new KeyValueDto<string, string, string>()
                        {
                            Key = p.Url,
                            Value = p.ImageDesc,
                            Message = p.SUrl
                        }));
                    if (data.SchoolPhotos.Count < 3)
                    {
                        data.SchoolPhotos.AddRange(images.Where(p => p.Type != PMS.School.Domain.Enum.SchoolImageType.SchoolHonor &&
                        p.Type != PMS.School.Domain.Enum.SchoolImageType.SchoolBrand).OrderBy(p => p.Type).ThenBy(p => p.Sort).
                            Select(p => new KeyValueDto<string, string, string>()
                            {
                                Key = p.Url,
                                Value = p.ImageDesc,
                                Message = p.SUrl
                            }));
                    }
                    if (data.SchoolPhotos.Any())
                    {
                        data.SchoolPhotos = data.SchoolPhotos.Take(3).ToList();
                    }
                }
            }
            else
            {
                ViewBag.SchoolVideo = schoolVideos.First();
            }


            //匹配榜单，毕业去向学校不一定在榜单中，所以要做排除
            if (data.Grade == (byte)(PMS.School.Domain.Common.SchoolGrade.SeniorMiddleSchool))
            {
                //毕业生去往国际大学榜单列表
                var foreignList = await _schService.GetForeignCollegeList(data.Achievement);
                //毕业生去往国内大学榜单列表
                var domesticList = await _schService.GetDomesticCollegeList(data.Achievement);
                var collegeList = new List<KeyValueDto<Guid, string, double, int, int>>();
                collegeList.AddRange(foreignList);
                collegeList.AddRange(domesticList);
                //国内大学也可能是在国际榜单中，所以要去重
                var colleges = (from cellege in collegeList
                                from achievement in data.Achievement
                                where achievement.Key == cellege.Key //&& achievement.Data == cellege.Data
                                select new KeyValueDto<Guid, string, double, int>()
                                {
                                    Key = cellege.Key,
                                    Value = cellege.Value,
                                    Message = cellege.Message,
                                    Data = achievement.Data
                                }).GroupBy(g => g.Key).Select(s => s.First()).ToList();
                data.Achievement = colleges;
            }
            ViewBag.IsSignSchool = _lSRFSchoolDetail.CheckSchIsLeaving(extId);

            //获取文章列表
            //分部文章
            var extArticles = await Task<IEnumerable<article>>.Run(() =>
            {
                return _articleService.GetComparisionArticles(new List<Guid> { extId }, 2);
            });
            //政策文章
            var policyArticles = await Task.Run(() =>
            {
                return _articleService.GetPolicyArticles(new List<PMS.OperationPlateform.Domain.DTOs.SchoolType>
                {
                    new PMS.OperationPlateform.Domain.DTOs.SchoolType
                    { Chinese = data.Chinese ?? false, Diglossia = data.Diglossia ?? false, Discount = data.Discount ?? false, SchoolGrade = data.Grade, Type = data.Type } },
                    new List<PMS.OperationPlateform.Domain.DTOs.Local>
                    { new PMS.OperationPlateform.Domain.DTOs.Local { AreaId = data.Area, CityId = data.City, ProvinceId = data.Province } },
                    3);
            });
            //相关推荐学校
            //var Recommend = await _easyRedisClient
            //    .GetOrAddAsync<List<KeyValueDto<Guid, Guid, string, string, int>>>(string.Format(_ExtRecommend, extId), () =>
            //    {
            //        return _schoolService.RecommendSchoolAsync(data.ExtId, data.Type, data.Grade, data.City, 2);
            //    }, DateTime.Now.AddDays(7));

            ViewBag.ExtId = data.ExtId;
            ViewBag.Sid = data.Sid;
            //<<<<<<该学部是否有被收藏 获取用户名<<<<<<<<
            bool collectStatat = false;
            string userName = "";
            Guid userId = Request.GetDeviceToGuid();
            ViewBag.isSchoolRole = false;

            //添加历史
            _ = Task.Run(async () =>
            {
                int a = await _userServiceClient.AddHistory(new ProductManagement.API.Http.Model.AddHistory { dataID = extId, dataType = MessageDataType.School, cookies = HttpContext.Request.GetAllCookies() });
            });
            //将学校详情添加到Redis有序集合中
            string Key = $"SchoolPK-0:{userId}";

            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                userId = user.UserId;
                Key = $"SchoolPK-1:{userId}";

                var userInfo = _userService.GetUserInfo(userId);

                userName = userInfo.NickName + "，";
                //var config = _configuration.GetSection("SchoolCollection");
                //var ip = config.GetSection("ServerUrl").Value;
                //var port = string.IsNullOrEmpty(config.GetSection
                //    ("Port").Value) ? "" : ":" + config.GetSection("Port").Value;
                //var request = new HttpRequestMessage(HttpMethod.Get, $"{ip}{port}?userId={user.UserId}&&dataId={data.ExtId}");
                //var client = _clientFactory.CreateClient();
                //var response = await client.SendAsync(request);

                try
                {
                    var isCollection = await _userServiceClient.IsCollected(new ProductManagement.API.Http.Model.IsCollected { dataID = data.ExtId, userId = userId });
                    if (isCollection.status == 0)
                    {
                        if (isCollection.iscollected == true)
                        {
                            collectStatat = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex);
                }
                ////检测当前用户身份 （true：校方用户 |false：普通用户）
                ViewBag.isSchoolRole = user.Role.Contains((int)UserRole.School);
                //if (isCollection.status == 200)
                //{
                //var responseResult = await response.Content.ReadAsStringAsync();
                //var dir = JsonHelper.DataRowFromJSON(responseResult);

                //}
            }

            //var historySchools = await _easyRedisClient.SortedSetRangeByRankAsync<SchoolExtSimpleDto>(Key, 0, 50, StackExchange.Redis.Order.Descending);
            //var historySchs = historySchools.ToList();
            //bool isRead = false;
            //foreach (var it in historySchs)
            //{
            //    if (it.ExtId == extId)
            //        isRead = true;
            //}
            //if (isRead == false)
            //    await _easyRedisClient.SortedSetAddAsync(Key, data, DateTime.Now.D2I(), StackExchange.Redis.CommandFlags.FireAndForget);

            ViewBag.CollectStatus = collectStatat;
            ViewBag.UserName = userName;

            //外教比例
            //string foreignTea = null;
            //if (data.ForeignTea != null && data.ForeignTea != 0)
            //{
            //    foreignTea = $"1:{data.ForeignTea}";
            //}
            //ViewBag.ForeignTea = foreignTea;
            string foreignTea = null;
            if (data.ForeignTea != null && data.ForeignTea != 0)
            {
                foreignTea = $"{data.ForeignTea}%";
            }
            ViewBag.ForeignTea = foreignTea;
            //师生比例
            string teaProportion = null;
            if (data.TsPercent != null)
            {
                //var teaNumber = NumberHelper.ConvertToFraction(data.Teachercount.Value, data.Studentcount.Value);
                teaProportion = $"1:{data.TsPercent}";
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
                    counterPart = counterPart.Select(p => new KeyValueDto<Guid>
                    {
                        Key = p.Key,
                        Value = p.Value,
                        Message = address.FirstOrDefault(a => a.Value == p.Value).Key,
                        Year = UrlShortIdUtil.Long2Base32(address.FirstOrDefault(a => a.Value == p.Value).Other).ToLower()
                    }).ToList();
                }
            }
            ViewBag.CounterPart = counterPart;
            //升学成绩内容
            object achData = null;
            if (data.AchYear != null)
            {
                achData = _schoolService.GetAchData(extId, data.Grade, data.Type, data.AchYear.Value);
                var achYears = _schoolService.GetAchYears(extId, data.Grade, data.Type);
                if (achYears?.Any() == true)
                {
                    ViewBag.AchYears = JSONHelper.ToJSON(achYears.OrderByDescending(p => p).Select(p => new { id = p, value = p }));
                }
            }
            ViewBag.AchData = achData;
            //其他分部
            var schoolExts = _schoolService.GetSchoolExtName(data.Sid);
            ViewBag.SchoolExts = schoolExts;
            //附近学位房
            //var buildings = new StringBuilder();
            //if (data.Longitude != null && data.Latitude != null)
            //{
            //    var distance = _configuration.GetSection("BuildDistance").Get<int>();
            //    var list = (await _schoolService.GetBuildingDataAsync(data.ExtId, data.Latitude, data.Longitude)).Select(p => p.Key + "," + p.Value + "," + p.Message);
            //    buildings.Append(string.Join(",", list));
            //}
            //ViewBag.Build = buildings.ToString();

            //获取学校的分数
            //var extCharacter = await _schoolService.GetSchoolCharacterAsync(extId);
            //ViewBag.Character = extCharacter;

            //ua
            ViewBag.UA = UserAgentHelper.Check(Request);
            var schext = await _schService.GetSchextSimpleInfo(extId);
            var schtype = new SchFType0(schext.Grade, schext.Type, schext.Discount, schext.Diglossia, schext.Chinese);
            ViewBag.SchFType = schtype;

            var scoreData = await GetSchoolExtScoreAsync(extId);
            ViewBag.Score = scoreData.score.FirstOrDefault(p => p.IndexId == 22)?.Score;

            if (scoreData.score?.Any() == true && (data.Area / 100 * 100) == 440100)
            {
                ViewBag.AreaScores = await _scoreService.GetAvgScoreByAreaCode(data.Area, schtype.Code);

                ViewBag.SchoolScores = scoreData.score;
                var schoolTotalScoreRanking = await _scoreService.GetSchoolRankingInCity(data.City, scoreData.score.FirstOrDefault(p => p.IndexId == 22).Score ?? 0, schtype.Code);
                ViewBag.TeachingLevel = scoreData.score.Where(p => p.IndexId > 14 && p.IndexId < 18).Average(p => p.Score).GetValueOrDefault();
                ViewBag.SchoolTotalScoreRanking = (int)(schoolTotalScoreRanking * 100);
                if (ViewBag.SchoolTotalScoreRanking == 0) ViewBag.SchoolTotalScoreRanking = 1;
            }

            //文章列表
            ViewBag.ExtArticles = extArticles.Select(a => new ArticleListItemViewModel()
            {
                Id = a.id,
                Time = a.time.GetValueOrDefault().ConciseTime(),
                Layout = a.layout,
                Title = a.title,
                ViweCount = a.VirualViewCount,
                No = UrlShortIdUtil.Long2Base32(a.No)
            }).ToList();
            ViewBag.PolicyArticles = policyArticles.Select(a => new ArticleListItemViewModel()
            {
                Id = a.id,
                Time = a.time.GetValueOrDefault().ConciseTime(),
                Layout = a.layout,
                Title = a.title,
                ViweCount = a.VirualViewCount,
                No = UrlShortIdUtil.Long2Base32(a.No)
            }).ToList();
            //推荐学校
            ViewBag.Recommend = new List<SchoolNameDto>();// (await _schoolService.RecommendSchoolsAsync(data.ExtId, 1, 2)).ToList();

            //学校评分【点评】
            var schoolscore = _scoreService.GetSchoolScore(data.Sid, data.ExtId);
            ViewBag.CommentSchoolScore = schoolscore == null ? new CommentSchoolScore() : new CommentSchoolScore()
            {
                AggScore = Math.Round(schoolscore.AggScore / 20, 1),
                StarScore = SchoolScoreToStart.GetCurrentSchoolstart(schoolscore.AggScore),
                EnvirScore = SchoolScoreToStart.GetCurrentSchoolstart(schoolscore.EnvirScore),
                HardScore = SchoolScoreToStart.GetCurrentSchoolstart(schoolscore.HardScore),
                LifeScore = SchoolScoreToStart.GetCurrentSchoolstart(schoolscore.LifeScore),
                ManageScore = SchoolScoreToStart.GetCurrentSchoolstart(schoolscore.ManageScore),
                TeachScore = SchoolScoreToStart.GetCurrentSchoolstart(schoolscore.TeachScore),
            };

            //学校点评
            var schoolComment = _commentService.GetSchoolSelectedComment(extId, userId);
            CommentList commentDetail = null;
            if (schoolComment != null)
            {
                var userInfo = _userService.GetUserInfo(schoolComment.UserId);
                var user = UserHelper.ToAnonyUserName(new Sxb.Web.Models.User.UserInfoVo() { HeadImager = userInfo.HeadImgUrl, Id = userInfo.Id, NickName = userInfo.NickName, Role = userInfo.VerifyTypes?.Select(q => q + 1).ToList() }, schoolComment.IsAnony);
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
            Models.Question.QuestionVo questionDetail = null;
            if (schoolQuestion.Count() > 0)
            {
                if (schoolQuestion[0].Id != default(Guid))
                {
                    questionDetail = _mapper.Map<PMS.CommentsManage.Application.ModelDto.QuestionDto, Models.Question.QuestionVo>(schoolQuestion[0]);
                    for (int i = 0; i < questionDetail.Images.Count(); i++)
                    {
                        questionDetail.Images[i] = _setting.QueryImager + questionDetail.Images[i];
                    }
                    if (questionDetail.Answer.Any())
                    {
                        //获取写回答用户信息
                        var userInfo = _userService.GetUserInfo(questionDetail.Answer[0].UserId);
                        var user = UserHelper.ToAnonyUserName(new Sxb.Web.Models.User.UserInfoVo() { HeadImager = userInfo.HeadImgUrl, Id = userInfo.Id, NickName = userInfo.NickName, Role = userInfo.VerifyTypes?.Select(q => q + 1).ToList() }, questionDetail.Answer[0].IsAnony);

                        questionDetail.Answer[0].UserInfo = user;

                    }
                }
            }

            ViewBag.SelectedQuestion = questionDetail;

            //留资类型
            ViewBag.ActiveCourseType = (int)CourseType;

            //header.description
            if (schoolExtInfo != null)
            {
                ViewBag.Page_Description = schoolExtInfo.Intro.GetHtmlHeaderString(150);
            }

            //学校品牌图片
            ViewBag.BrandImages = brandImages?.OrderBy(p => p.Sort).Take(8).ToList();

            ViewBag.UAText = Request.Headers.Keys.Contains("User-Agent") ? Request.Headers["User-Agent"].ToString().ToLower() : "";

            return View(data);
        }



        /// <summary>
        /// 学校更多详细
        /// 先读取缓存，在填充数据
        /// </summary>
        /// <returns></returns>
        [Route("/school-{schoolNo}/{action}")]
        [Route("/{controller}/{action}")]
        [Route("/{controller}/{action}/{extId}")]
        [Description("学部更多详情")]
        public async Task<IActionResult> Data(Guid extId = default(Guid), Guid sid = default(Guid), Guid id = default(Guid), string schoolNo = default)
        {
            var latitude = Request.GetLatitude();
            var longitude = Request.GetLongitude();

            //兼容旧的seo
            if (extId == default(Guid)) extId = id;

            if (!string.IsNullOrWhiteSpace(schoolNo))
            {
                var schExt = _schService.GetSchextSimpleInfoViaShortNo(schoolNo);
                if (schExt?.Result.Eid != default)
                {
                    extId = schExt.Result.Eid;
                }
            }

            ViewBag.ShortSchoolNo = schoolNo;
            ViewBag.ExtID = extId;

            var data = await _schoolService.GetSchoolExtDtoAsync(extId, Convert.ToDouble(latitude), Convert.ToDouble(longitude));
            //如果学校分部为null的话跳转到错误页面
            if (data == null) return new ExtNotFoundViewResult();
            if (data.City == 440100 || data.City == 440300 || data.City == 500100 || data.City == 510100)
            {
                return RedirectPermanent($"/school/detail/{UrlShortIdUtil.Long2Base32(data.SchoolNo)}_dot=1/");
            }

            //添加历史
            _ = _userServiceClient.AddHistory(new ProductManagement.API.Http.Model.AddHistory { dataID = extId, dataType = MessageDataType.School, cookies = HttpContext.Request.GetAllCookies() });

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
            ViewBag.OpenHours = openhour.OrderByDescending(p => p.Value).ToList();
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
            ViewBag.Calendar = calendar.OrderByDescending(p => p.Value).ToList();
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
            //string foreignTea = null;
            //if (data.ForeignTea != null && data.ForeignTea != 0)
            //{
            //    foreignTea = $"1:{data.ForeignTea}";
            //}
            //ViewBag.ForeignTea = foreignTea;
            string foreignTea = null;
            if (result.ForeignTea != null && result.ForeignTea != 0)
            {
                foreignTea = $"{result.ForeignTea}%";
            }
            ViewBag.ForeignTea = foreignTea;
            //师生比例
            string teaProportion = null;
            if (data.TsPercent != null)
            {
                //var teaNumber = NumberHelper.ConvertToFraction(data.Teachercount.Value, data.Studentcount.Value);
                teaProportion = $"1:{data.TsPercent}";
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
            //附近学位房(改成异步请求)
            //var buildings = new StringBuilder();
            //if (result.Longitude != null && result.Latitude != null)
            //{
            //    var distance = _configuration.GetSection("BuildDistance").Get<int>();
            //    var list = (await _schoolService.GetBuildingDataAsync(result.ExtId, result.Latitude, result.Longitude, distance)).Select(p => p.Key + "," + p.Value + "," + p.Message);
            //    buildings.Append(string.Join(",", list));
            //}
            //ViewBag.Build = buildings.ToString();

            //获取学校的特征
            //var extCharacter = await _schoolService.GetSchoolCharacterAsync(extId);
            //ViewBag.Character = extCharacter;

            //var schoolScore = await GetSchoolExtScoreAsync(extId);
            //ViewBag.ScoreIndex = schoolScore.index;
            //ViewBag.Score = schoolScore.score;


            result.Intro = string.IsNullOrWhiteSpace(result.Intro) ? "" : result.Intro.Replace("\r\n", "</br>").Replace("\n", "</br>");

            //学校评分【点评】
            //var schoolscore = _scoreService.GetSchoolScore(result.Sid, result.ExtId);
            //获取学校点评评分
            //ViewBag.SchoolScoreToStart = SchoolScoreToStart.GetCurrentSchoolstart(schoolscore.AggScore);
            ViewBag.SchFType = new SchFType0(result.Grade, result.Type, result.Discount, result.Diglossia, result.Chinese);
            return View(result);
        }

        /// <summary>
        /// 获取学位房接口
        /// </summary>
        /// <param name="extId"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="neLat"></param>
        /// <param name="swLat"></param>
        /// <param name="neLng"></param>
        /// <param name="swLng"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        [HttpGet]
        [Description("学位房接口")]
        public async Task GetBuildingData(string callBack, Guid extId, double? latitude, double? longitude, double neLat, double swLat, double neLng, double swLng)
        {
            var result = ResponseResult.Success("");
            if (latitude != null && (latitude <= 90.0) && (latitude >= -90.0) && longitude != null)
            {
                var distance = _configuration.GetSection("BuildDistance").Get<int>();
                var list = (await _schoolService.GetBuildingDataAsync(extId, latitude, longitude, neLat, swLat, neLng, swLng, distance)).Select(p => p.Key + "," + p.Value + "," + p.Message);
                result = ResponseResult.Success(string.Join(",", list));
            }
            string response = JsonConvert.SerializeObject(result, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            string call = callBack + "(" + response + ")";
            Response.ContentType = "text/plain; charset=utf-8";
            await Response.WriteAsync(call, Encoding.UTF8);
        }

        public async Task<(List<SchoolExtScoreIndex> index, List<SchoolExtScore> score)> GetSchoolExtScoreAsync(Guid extId)
        {
            var index = await _schoolService.schoolExtScoreIndexsAsync();
            //临时变动(大项：课程18 学术屏蔽19  子项：以上子项及入学难度2)
            index = index.Where(p => p.Id != 18 && p.Id != 19 && p.ParentId != 18 && p.ParentId != 19 && p.Id != 2).ToList();
            if (index == null || index.Count == 0)
                return (null, null);
            else
            {
                var score = await _schoolService.GetSchoolExtScoreAsync(extId);
                return (index, score);
            }
        }

        /// <summary>
        /// 获取PK列表
        /// </summary>
        /// <param name="extId"></param>
        /// <param name="sid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetPKList(Guid extId = default(Guid), Guid sid = default(Guid), Guid id = default(Guid))
        {
            var latitude = Request.GetLatitude();
            var longitude = Request.GetLongitude();
            //兼容旧的seo
            if (extId == default(Guid)) extId = id;
            var data = await _schoolService.GetSchoolExtSimpleDtoAsync(Convert.ToDouble(longitude), Convert.ToDouble(latitude), extId);
            //如果学校分部为null的话跳转到错误页面
            if (data == null)
                return new ExtNotFoundViewResult();
            return View(data);
        }

        /// <summary>
        /// 获取PK详情
        /// </summary>
        /// <param name="extId"></param>
        /// <param name="sid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetPKDetail(Guid extId = default(Guid), Guid sid = default(Guid), Guid id = default(Guid))
        {
            if (User.Identity.IsAuthenticated)
            {
                //添加历史
                _userServiceClient.AddHistory(new ProductManagement.API.Http.Model.AddHistory { dataID = extId, dataType = MessageDataType.School, cookies = HttpContext.Request.GetAllCookies() });
            }
            var latitude = Convert.ToDouble(Request.GetLatitude());
            var longitude = Convert.ToDouble(Request.GetLongitude());
            var data = await _schoolService.GetSchoolExtSimpleDtoAsync(longitude, latitude, extId);
            //如果学校分部为null的话跳转到错误页面
            if (data == null)
                return new ExtNotFoundViewResult();

            if (data == null) throw new Exception("");
            var schoolScore = await GetSchoolExtScoreAsync(extId);
            ViewBag.ScoreIndex = schoolScore.index;
            ViewBag.Score = schoolScore.score;
            return View(data);
        }

        /// <summary>
        /// 学校总评PK
        /// </summary>
        /// <param name="extId"></param>
        /// <param name="sid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetSchoolFractionPK(Guid extId = default(Guid), Guid sid = default(Guid), Guid id = default(Guid))
        {
            if (User.Identity.IsAuthenticated)
            {
                //添加历史
                await _userServiceClient.AddHistory(new ProductManagement.API.Http.Model.AddHistory { dataID = extId, dataType = MessageDataType.School, cookies = HttpContext.Request.GetAllCookies() });
            }
            var schoolScore = await GetSchoolExtScoreAsync(extId);
            ViewBag.ScoreIndex = schoolScore.index;
            ViewBag.Score = schoolScore.score;
            return View();
        }

        [HttpGet]
        public IActionResult Comment(Guid Id)
        {
            var redirectUrl = $"/school/detail/{Id.ToString("N")}";
            Response.Redirect(redirectUrl);
            Response.StatusCode = 302;

            return Json(new { });
        }

        [HttpGet]
        public IActionResult Question(Guid Id)
        {
            var redirectUrl = $"/school/detail/{Id.ToString("N")}";
            Response.Redirect(redirectUrl);
            Response.StatusCode = 302;

            return Json(new { });
        }

        [HttpGet]
        public IActionResult Extatlas(Guid Id)
        {
            var redirectUrl = $"/school/detail/{Id.ToString("N")}";
            Response.Redirect(redirectUrl);
            Response.StatusCode = 302;

            return Json(new { });
        }

        [HttpGet]
        public IActionResult Extstrategy(Guid Id)
        {
            var redirectUrl = $"/school/detail/{Id.ToString("N")}";
            Response.Redirect(redirectUrl);
            Response.StatusCode = 302;

            return Json(new { });
        }
        [HttpGet]
        public IActionResult Extmessage(Guid Id)
        {
            var redirectUrl = $"/school/detail/{Id.ToString("N")}";
            Response.Redirect(redirectUrl);
            Response.StatusCode = 302;

            return Json(new { });
        }

        [HttpPost]
        public ResponseResult GetSchoolPastExamByYear(string eid, string year)
        {
            var result = ResponseResult.Failed();
            var find = _schoolService.GetSchoolPastExam(eid, year);
            if (!string.IsNullOrWhiteSpace(find))
            {
                result.Succeed = true;
                result.Data = find;
            }
            return result;
        }

        [HttpPost]
        public ResponseResult GetSchoolFieldYearContent(string eid, string field, string year)
        {
            var result = ResponseResult.Failed();
            var content = _schoolService.GetSchoolFieldContent(eid, field, year);
            if (!string.IsNullOrWhiteSpace(content))
            {
                result.Succeed = true;
                result.Data = content;
            }
            return result;
        }

        public ResponseResult GetAchDataHtml(Guid extId, byte grade, byte type, int year)
        {
            var result = ResponseResult.Success();

            var achData = _schoolService.GetAchData(extId, grade, type, year);
            var achievementData = _schoolService.GetYearAchievementList(extId, grade, year)?.OrderByDescending(p => p.Message).ToList();
            var achDataHtml = new StringBuilder();
            var achDataHtmlFormat = string.Empty;
            var achievementDataHtmlFormat = string.Empty;
            var achievementDataHtml = new StringBuilder();
            switch (grade)
            {
                case 3:
                    achDataHtmlFormat = "<li><span>最高分：</span>{0}</li><li><span>平均分：</span>{1}</li><li><span>重点率：</span>{2}</li>";
                    if (achData != null)
                    {
                        var middleData = (MiddleSchoolAchievement)achData;
                        achDataHtml.AppendFormat(achDataHtmlFormat, middleData.Highest == null ? "暂未收录" : $"{middleData.Highest}分",
                            middleData.Average == null ? "暂未收录" : $"{middleData.Average}分",
                            middleData.Keyrate == null ? "暂未收录" : $"{middleData.Keyrate}%");
                    }
                    else
                    {
                        achDataHtml.AppendFormat(achDataHtmlFormat, "暂未收录", "暂未收录", "暂未收录");
                    }
                    break;
                case 4:
                    achDataHtmlFormat = "<li><span>重本率：</span>{1}</li><li><span>本科率：</span>{0}</li>";
                    if (achData != null)
                    {
                        var highData = (HighSchoolAchievement)achData;
                        achDataHtml.AppendFormat(achDataHtmlFormat, highData.Undergraduate == null ? "暂未收录" : $"{highData.Undergraduate}%",
                            highData.Keyundergraduate == null ? "暂未收录" : $"{highData.Keyundergraduate}%");
                    }
                    else
                    {
                        achDataHtml.AppendFormat(achDataHtmlFormat, "暂未收录", "暂未收录");
                    }
                    break;
            }

            if (achievementData?.Any() == true)
            {
                achievementDataHtmlFormat = "<li>{0}<span>{1}人</span></li>";
                foreach (var item in achievementData.Take(3))
                {
                    achievementDataHtml.AppendFormat(achievementDataHtmlFormat, item.Value, item.Message);
                }
            }

            result.Data = new
            {
                achData = achDataHtml.ToString(),
                achievementData = achievementDataHtml.ToString(),
                hasAchievement = achievementData?.Any() == true
            };

            return result;
        }

        [HttpGet]
        [Route("/{controller}-{schoolNo}/{action}")]
        public async Task<ActionResult> Extatlas(string schoolNo)
        {
            var extInfo = await _schService.GetSchextSimpleInfoViaShortNo(schoolNo);
            if (extInfo == null) return new ExtNotFoundViewResult();

            var videos = await _schoolService.GetSchoolVideos(extInfo.Eid);
            var images = await _schoolService.GetSchoolImages(extInfo.Eid, null);

            ViewBag.Videos = videos ?? new List<KeyValueDto<DateTime, string, byte, string, string>>();
            ViewBag.Images = images?.OrderBy(p => p.Sort).ToList() ?? new List<SchoolImageDto>();
            ViewBag.SchoolName = $"{extInfo.SchName}-{extInfo.ExtName}";

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

        /// <summary>
        /// 学校政策文章列表
        /// </summary>
        [HttpGet]
        [Route("/{controller}-{schoolNo}/{action}")]
        public async Task<ActionResult> SchoolPolicy(string schoolNo, PMS.OperationPlateform.Domain.DTOs.Local local, int offset = 0, int limit = 20)
        {
            var extInfo = await _schService.GetSchextSimpleInfoViaShortNo(schoolNo);
            if (extInfo == null) return new ExtNotFoundViewResult();


            var schoolType = new PMS.OperationPlateform.Domain.DTOs.SchoolType()
            {
                Type = extInfo.Type,
                SchoolGrade = extInfo.Grade,
                Chinese = extInfo.Chinese ?? false,
                Diglossia = extInfo.Diglossia ?? false,
                Discount = extInfo.Discount ?? false
            };


            var (articles, total) = _articleService.GetPolicyArticles_PageVersion(new List<PMS.OperationPlateform.Domain.DTOs.SchoolType>() {
                 schoolType
            },
              new List<PMS.OperationPlateform.Domain.DTOs.Local>() {
                 local
              },
              offset,
              limit
             );

            var articleIds = articles.Select(a => a.id).ToArray();
            var covers = _articleCoverService.GetCoversByIds(articleIds);

            var models = articles.Select(a => new ArticleListItemViewModel()
            {
                Id = a.id,
                Time = ArticleListItemTimeFormart(a.time.GetValueOrDefault()),
                CommentCount = 0,
                Covers = covers.Where(c => c.articleID == a.id).Select(c => Article_CoverToLink(c)).ToList(),
                Digest = a.html.GetHtmlHeaderString(150),
                Layout = a.layout,
                Title = a.title,
                ViweCount = a.VirualViewCount,
                No = UrlShortIdUtil.Long2Base32(a.No)
            });


            ViewBag.Total = total;
            ViewBag.Type = schoolType.Type;
            ViewBag.SchoolGrade = schoolType.SchoolGrade;
            ViewBag.Discount = Convert.ToInt16(schoolType.Discount);
            ViewBag.Diglossia = Convert.ToInt16(schoolType.Diglossia);
            ViewBag.Chinese = Convert.ToInt16(schoolType.Chinese);
            ViewBag.ProvinceId = local.ProvinceId;
            ViewBag.CityId = local.CityId;
            ViewBag.AreaId = local.AreaId;
            ViewBag.Offset = offset;
            ViewBag.Limit = limit;
            return View(models);
        }

        /// <summary>
        /// 学校动态文章列表
        /// </summary>
        [HttpGet]
        [Route("/{controller}-{schoolNo}/{action}")]
        public async Task<ActionResult> SchoolDynamic(string schoolNo, int offset = 0, int limit = 20)
        {
            var extInfo = await _schService.GetSchextSimpleInfoViaShortNo(schoolNo);
            if (extInfo == null) return new ExtNotFoundViewResult();

            var (articles, total) = _articleService.GetComparisionArticles_PageVersion(new List<Guid>() { extInfo.Eid }, offset, limit);

            var models = articles.Select(a => new ArticleListItemViewModel()
            {
                Id = a.id,
                Time = ArticleListItemTimeFormart(a.time.GetValueOrDefault()),
                CommentCount = 0,
                Covers = null,
                Digest = a.html.GetHtmlHeaderString(150),
                Layout = a.layout,
                Title = a.title,
                ViweCount = a.VirualViewCount,
                No = UrlShortIdUtil.Long2Base32(a.No)
            });

            ViewBag.Total = total;
            ViewBag.SchoolId = extInfo.Sid;
            ViewBag.Offset = offset;
            ViewBag.Limit = limit;
            return View(models);
        }

        /// <summary>
        /// 文章列表项的时间格式
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private string ArticleListItemTimeFormart(DateTime dateTime)
        {
            var passBy_H = (int)(DateTime.Now - dateTime).TotalHours;

            if (passBy_H > 24 || passBy_H < 0)
            {
                //超过24小时,显示日期
                return dateTime.ToString("yyyy年MM月dd日");
            }
            if (passBy_H == 0)
            {
                return dateTime.ToString("刚刚");
            }
            else
            {
                return $"{passBy_H}小时前";
            }
        }
        private string Article_CoverToLink(article_cover c)

        {
            if (c == null)
            {
                return "";
            }
            return $"https://cdn.sxkid.com/images/article/{c.articleID}/{c.photoID}.{((FileExtension)c.ext).ToString()}";
        }



        #region 百度小程序简版院校详情（临时）
        public async Task<SchoolDetailBDViewModel> GetSchoolDetail(Guid extId)
        {
            var latitude = Request.GetLatitude();
            var longitude = Request.GetLongitude();
            var data = await _schoolService.GetSchoolExtDtoAsync(extId, Convert.ToDouble(latitude), Convert.ToDouble(longitude));

            if (data == null)
            {
                return new SchoolDetailBDViewModel
                {
                    Status = 400,
                    ErrMsg = "找不到此学校"
                };
            }

            var recruit = _schoolService.GetSchoolExtRecruit(extId);

            StringBuilder admissionsText = new StringBuilder();

            if (recruit.Age == null && recruit.MaxAge != null)
            {
                admissionsText.AppendFormat("招生年龄：{0}\n", "0-" + recruit.MaxAge);
            }
            else if (recruit.Age != null && recruit.MaxAge == null)
            {
                admissionsText.AppendFormat("招生年龄：{0}\n", recruit.MaxAge);
            }
            else if (recruit.Age != null && recruit.MaxAge != null)
            {
                admissionsText.AppendFormat("招生年龄：{0}\n", $"{recruit.Age}-{recruit.MaxAge}");
            }
            //录取分数线
            var point = string.IsNullOrEmpty(recruit.Point) ? new List<KeyValueDto<string>>() : JsonHelper.JSONToObject<List<KeyValueDto<string>>>(recruit.Point);

            //招生日期
            var date = string.IsNullOrEmpty(recruit.Date) ? new List<KeyValueDto<string>>() : JsonHelper.JSONToObject<List<KeyValueDto<string>>>(recruit.Date);

            //报名科目
            var subject = string.IsNullOrEmpty(recruit.Subjects) ? new List<KeyValueDto<string>>() : JsonHelper.JSONToObject<List<KeyValueDto<string>>>(recruit.Subjects);

            if (recruit.Proportion != null)
            {
                admissionsText.AppendFormat("招录比例：{0}<br>", recruit.Proportion);
            }
            if (point.Count > 0)
            {
                admissionsText.AppendFormat("录取分数：{0}<br>", point.FirstOrDefault()?.Key);
            }
            if (recruit.ScholarShip != null)
            {
                admissionsText.AppendFormat("奖学金计划：{0}<br>", recruit.ScholarShip);
            }
            if (date.Count > 0)
            {
                admissionsText.AppendFormat("招生日期：<br>{0}<br>", string.Join("<br>", date.Select(p => p.Key + ":" + p.Value)));
            }
            if (recruit.Contact != null)
            {
                admissionsText.AppendFormat("报名方式：{0}<br>", recruit.Contact);
            }

            if (subject.Count > 0)
            {
                admissionsText.AppendFormat("考试科目：{0}<br>", string.Join("、", subject.Select(p => p.Key)));
            }

            if (recruit.Data != null)
            {
                admissionsText.AppendFormat("报名所需资料：{0}<br>", recruit.Data);
            }

            if (recruit.Pastexam != null)
            {
                admissionsText.AppendFormat("往期入学考试内容：<br>{0}", recruit.Pastexam);
            }
            var videos = await _schoolService.GetSchoolVideos(extId);
            var images = await _schoolService.GetSchoolImages(extId, null);

            //师资展示
            var teacher = images.Where(p => p.Type == PMS.School.Domain.Enum.SchoolImageType.Teacher || p.Type == PMS.School.Domain.Enum.SchoolImageType.Principal);
            var teacherImages = new List<SchoolDetailBDViewModel.Item>();
            foreach (var item in teacher)
            {
                teacherImages.Add(new SchoolDetailBDViewModel.Item
                {
                    Type = "img",
                    Value = item.Url
                });
                if (!string.IsNullOrEmpty(item.ImageDesc))
                {
                    teacherImages.Add(new SchoolDetailBDViewModel.Item
                    {
                        Type = "text",
                        Value = item.ImageDesc
                    });
                }
            }

            //硬件设施
            var hardware = images.Where(p => p.Type == PMS.School.Domain.Enum.SchoolImageType.Hardware);
            var hardwareImages = new List<SchoolDetailBDViewModel.Item>();
            foreach (var item in hardware)
            {
                teacherImages.Add(new SchoolDetailBDViewModel.Item
                {
                    Type = "img",
                    Value = item.Url
                });
                if (!string.IsNullOrEmpty(item.ImageDesc))
                {
                    teacherImages.Add(new SchoolDetailBDViewModel.Item
                    {
                        Type = "text",
                        Value = item.ImageDesc
                    });
                }
            }

            //学生生活
            var student = images.Where(p => p.Type == PMS.School.Domain.Enum.SchoolImageType.GradeSchedule
                                            || p.Type == PMS.School.Domain.Enum.SchoolImageType.Diagram
                                            || p.Type == PMS.School.Domain.Enum.SchoolImageType.CommunityActivities
                                            || p.Type == PMS.School.Domain.Enum.SchoolImageType.Schedule);
            var studentImages = new List<SchoolDetailBDViewModel.Item>();
            foreach (var item in student)
            {
                teacherImages.Add(new SchoolDetailBDViewModel.Item
                {
                    Type = "img",
                    Value = item.Url
                });
                if (!string.IsNullOrEmpty(item.ImageDesc))
                {
                    teacherImages.Add(new SchoolDetailBDViewModel.Item
                    {
                        Type = "text",
                        Value = item.ImageDesc
                    });
                }
            }
            //教学质量
            var honor = images.Where(p => p.Type == PMS.School.Domain.Enum.SchoolImageType.SchoolHonor
                                    || p.Type == PMS.School.Domain.Enum.SchoolImageType.StudentHonor);
            var honorImages = new List<SchoolDetailBDViewModel.Item>();
            foreach (var item in honor)
            {
                teacherImages.Add(new SchoolDetailBDViewModel.Item
                {
                    Type = "img",
                    Value = item.Url
                });
                if (!string.IsNullOrEmpty(item.ImageDesc))
                {
                    teacherImages.Add(new SchoolDetailBDViewModel.Item
                    {
                        Type = "text",
                        Value = item.ImageDesc
                    });
                }
            }

            return new SchoolDetailBDViewModel
            {
                Status = 0,
                Info = new SchoolDetailBDViewModel.InfoViewModel
                {
                    Show = true,
                    Id = data.Sid,
                    Name = data.SchoolName,
                    Name_e = data.EName,
                    Website = data.WebSite,
                    Intro = new List<SchoolDetailBDViewModel.Item> {
                        new SchoolDetailBDViewModel.Item{
                            Type = "title",
                            Value = "学校简介"
                        },
                        new SchoolDetailBDViewModel.Item{
                            Type = "text",
                            Value = data.Intro
                        }
                    },
                    Logo = data.Logo
                },
                Info_ext = new SchoolDetailBDViewModel.InfoExtViewModel
                {
                    Sid = data.Sid,
                    Id = data.ExtId,
                    Name = data.ExtName,
                    Address = data.Address,
                    Province = data.Province,
                    City = data.City,
                    Area = data.Area,
                    CityName = data.CityName,
                    AreaName = data.AreaName,
                    Latitude = data.Latitude,
                    Longitude = data.Longitude,
                    Tel = data.Tel,
                    Fee = data.Tuition,
                    Tags_s = data.Tags,
                    Grade = new List<int> { data.Grade },
                    Type = data.Type,
                    Nature = data.Type,
                    Feedesc = new List<SchoolDetailBDViewModel.Item> {
                        new SchoolDetailBDViewModel.Item{
                            Type = "title",
                            Value = "学杂费"
                        },
                        new SchoolDetailBDViewModel.Item{
                            Type = "text",
                            Value = data.Tuition == null ? "暂未收录" : data.Tuition + "元/学年"
                        },
                    },
                    Admissions = new List<SchoolDetailBDViewModel.Item>
                    {
                        new SchoolDetailBDViewModel.Item{
                            Type = "title",
                            Value = "招生人数"
                        },
                        new SchoolDetailBDViewModel.Item{
                            Type = "text",
                            Value = data.Count==null?"暂未收录":(data.Count.ToString()+"人")
                        },
                        new SchoolDetailBDViewModel.Item{
                            Type = "title",
                            Value = "招生简章"
                        },
                        new SchoolDetailBDViewModel.Item{
                            Type = "text",
                            Value = admissionsText.ToString()
                        },
                    },
                    Teacher = teacherImages,
                    Hardware = hardwareImages,
                    Quality = honorImages,
                    Student = studentImages,
                    Video = videos.FirstOrDefault()?.Value,
                    VideoDesc = videos.FirstOrDefault()?.Other,
                }
            };
        }


        #endregion
    }
}