using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMS.Infrastructure.Application.IService;
using PMS.School.Application.ModelDto;
using PMS.Search.Application.IServices;
using PMS.Search.Application.ModelDto.Query;
using ProductManagement.Framework.Cache.Redis;
using Sxb.Web.Common;
using Sxb.Web.RequestModel.School;
using Sxb.Web.ViewModels.School;
using ProductManagement.Framework.Foundation;
using PMS.School.Application.IServices;
using PMS.UserManage.Application.IServices;
using PMS.School.Domain.Dtos;
using PMS.CommentsManage.Application.IServices;
using PMS.School.Infrastructure.Common;
using Sxb.Web.Models.Comment;
using PMS.CommentsManage.Domain.Entities;
using AutoMapper;
using Sxb.Web.Utils;
using Sxb.Web.Models;
using PMS.CommentsManage.Domain.Entities.Total;
using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.DTOs;
using Sxb.Web.ViewModels.Article;
using MongoDB.Driver;
using CommentApp.Models.School;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Options;
using PMS.Infrastructure.Application.ModelDto;
using PMS.School.Domain.Common;
using iSchool;
using NSwag.Annotations;

namespace Sxb.Web
{
    [OpenApiIgnore]
    public class SchoolCompareApiController : Controller
    {
        private readonly ISearchService _dataSearch;
        private readonly ICityInfoService _cityService;
        private readonly ISchoolService _schoolService;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly ICollectionService _collectionService;
        private readonly ISchService _schService;
        private readonly ISchoolCommentService _commentService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly ImageSetting _setting;
        private readonly IQuestionInfoService _questionInfoService;
        private readonly ISchoolRankService _schoolRankService;
        private readonly IArticleService _articleService;
        private readonly MongoSetting _mongosetting;
        private readonly string _ExtDataKey = "ext:paramData:";

        public SchoolCompareApiController(
            ISearchService dataSearch,
            ICityInfoService cityService,
            ISchoolService schoolService,
            IEasyRedisClient easyRedisClient,
            ICollectionService collectionService,
            ISchoolCommentService commentService,
            ISchService schService,
            IMapper mapper,
            IUserService userService,
            IOptions<ImageSetting> set,
            IOptions<MongoSetting> mongoset,
            IQuestionInfoService questionInfoService,
            ISchoolRankService schoolRankService,
            IArticleService articleService
            )
        {
            _dataSearch = dataSearch;
            _cityService = cityService;
            _schoolService = schoolService;
            _easyRedisClient = easyRedisClient;
            _collectionService = collectionService;
            _commentService = commentService;
            _schService = schService;
            _mapper = mapper;
            _userService = userService;
            _questionInfoService = questionInfoService;
            _schoolRankService = schoolRankService;
            _articleService = articleService;
            _setting = set.Value;
            _mongosetting = mongoset.Value;
        }

        /// <summary>
        /// 默认视图
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 学校（学部）筛选列表页
        /// </summary>
        public async Task<ResultDto> ExtSchoolFilter(int type)
        {
            var result = new FilterOutputDto();
            int cityCode = Request.GetLocalCity();
            //ViewBag.CityCode = cityCode;
            result.CityCode= cityCode;
            var areas = new List<AreaDto>();
            var metros = new List<MetroDto>();
            var tags = new List<PMS.School.Application.ModelDto.TagDto>();
            var types = new List<GradeTypeDto>();

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

            //ViewBag.Areas = areas;
            //ViewBag.Metros = metros.OrderBy(q => q.MetroName);
            //ViewBag.Types = types;
            result.Areas = areas;
            result.Metros = metros.OrderBy(q => q.MetroName).ToList();
            result.Types = types;



            //ViewBag.Authentications = tags.Where(q => q.Type == 2).Select(q => new SchoolExtTagViewModel
            //{
            //    Id = q.Id,
            //    Name = q.Name,
            //    International = dzAuthTags.Contains(q.Id) ? 0 :
            //                    interAuthTags.Keys.Contains(q.Id) ? 1 :
            //                    notInterAuthTags.Keys.Contains(q.Id) ? 2 : 0
            //}).ToList();
            //ViewBag.Abroads = tags.Where(q => q.Type == 3).ToList();
            //ViewBag.Characteristics = tags.Where(q => q.Type == 8).Select(q => new SchoolExtTagViewModel
            //{
            //    Id = q.Id,
            //    Name = q.Name,
            //    International = dzCharacTags.Contains(q.Id) ? 0 :
            //                    interCharacTags.Keys.Contains(q.Id) ? 1 :
            //                    notInterCharacTags.Keys.Contains(q.Id) ? 2 : 0
            //}).ToList();
            //ViewBag.Courses = tags.Where(q => q.Type == 6).ToList();

            result.Authentications = tags.Where(q => q.Type == 2).Select(q => new SchoolExtTagViewModel
            {
                Id = q.Id,
                Name = q.Name,
                International = dzAuthTags.Contains(q.Id) ? 0 :
                                interAuthTags.Keys.Contains(q.Id) ? 1 :
                                notInterAuthTags.Keys.Contains(q.Id) ? 2 : 0
            }).ToList(); ;
            result.Abroads = tags.Where(q => q.Type == 3).ToList();
            result.Characteristics = tags.Where(q => q.Type == 8).Select(q => new SchoolExtTagViewModel
            {
                Id = q.Id,
                Name = q.Name,
                International = dzCharacTags.Contains(q.Id) ? 0 :
                                interCharacTags.Keys.Contains(q.Id) ? 1 :
                                notInterCharacTags.Keys.Contains(q.Id) ? 2 : 0
            }).ToList();
            result.Courses = tags.Where(q => q.Type == 6).ToList();



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
                        "lx130","lx432","lx430","lx230","lx231","lx330","lx331","lx431","lx432"
                    });
                    break;
            }

            //ViewBag.SelectedType = selectTypeValues;
            result.SelectedType = selectTypeValues;
            return ApiResult.Success("操作成功", result);
        }


        /// <summary>
        /// 根据分部Id获取学校列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<ResultDto> GetSchoolListBranchIds(string IdsJson)
        {
            var result = new List<SchoolExtListItemViewModel>();
            if (!string.IsNullOrEmpty(IdsJson))
            {
                var Ids = JsonHelper.JSONToObject<List<Guid>>(IdsJson);
                var data = await _schoolService.ListExtSchoolByBranchIds(Ids) ?? new List<SchoolExtFilterDto>();
                result.AddRange(data.Select(d =>
                {
                    return new SchoolExtListItemViewModel()
                    {
                        SchoolId = d.Sid,
                        BranchId = d.ExtId,
                        SchoolName = d.Name,
                        CityName = d.City,
                        AreaName = d.Area,
                        Distance = d.Distance,
                        LodgingType = (int)LodgingUtil.Reason(d.Lodging,d.Sdextern),
                        Type = d.Type,
                        International = d.International,
                        Cost = d.Tuition,
                        Tags = d.Tags,
                        Score = d.Score,
                        CommentTotal = d.CommentCount
                    };
                }).ToList());
            }
            return ApiResult.Success("操作成功", result);
        }


        /// <summary>
        /// 学校筛选列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<ResultDto> GetPageSchoolList(SchoolExtFilterData query)
        {
            var lat = Request.GetLatitude();
            var lng = Request.GetLongitude();
            var cityCode = Request.GetLocalCity();
            var metroIds = new List<MetroQuery>();

            if (User.Identity.IsAuthenticated && !string.IsNullOrWhiteSpace(query.KeyWords))
            {
                var user = User.Identity.GetUserInfo();
                string Key = $"SearchHistory:{user.UserId}";
                await _easyRedisClient.SortedSetAddAsync(Key, query.KeyWords, DateTime.Now.D2I(), StackExchange.Redis.CommandFlags.FireAndForget);
            }

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
            //获取学校列表
            var schoolList = _dataSearch.SearchSchool(new SearchSchoolQuery
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
                MinCost = query.MinCost,
                MaxCost = query.MaxCost,
                Grade= query.Grade,
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

            var data = schoolList.Schools.Count > 0 ? await _schoolService.ListExtSchoolByBranchIds(schoolList.Schools.Select(q => q.Id).ToList()) : new List<SchoolExtFilterDto>();

            var result = from school in schoolList.Schools
                         join d in data
                         on school.Id equals d.ExtId
                         select new
                         {
                             SchoolId = d.Sid,
                             BranchId = d.ExtId,
                             SchoolName = school.Name,
                             CityName = d.City,
                             AreaName = d.Area,
                             Distance = school.Distance,
                             Lodging = d.Lodging,
                             Sdextern = d.Sdextern,
                             LodgingType = (int)d.LodgingType,
                             LodgingTypeDesc = d.LodgingType.Description(),
                             Type = d.Type,
                             International = d.International,
                             Cost = d.Tuition,
                             Tags = d.Tags,
                             Score = d.Score,
                             CommentTotal = d.CommentCount,
                             active=false
                         };

            return ApiResult.Success("操作成功", result.ToList());
        }

        /// <summary>
        /// 获取推荐学校
        /// </summary>
        /// <param name="extId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<ResultDto> GetRecommendSchool(Guid extId, int pageIndex, int pageSize)
        {
            var schext = await _schoolService.GetSchoolExtDetailsAsync(extId);
            if (schext == null)
                return ApiResult.Fail("分部Id无效", new SchoolExtListDto() { PageIndex = pageIndex, PageCount = 0, PageSize = pageSize, IsRecommend = false, List = new List<SchoolExtItemDto>() }); ;
            var latitude = Convert.ToDouble(Request.GetLatitude());
            var longitude = Convert.ToDouble(Request.GetLongitude());
            if (schext.Latitude != null && schext.Latitude != null)
            {
                latitude = Convert.ToDouble(schext.Latitude);
                longitude = Convert.ToDouble(schext.Longitude);
            }

            //学校类型1:公办 2:民办 3.国际
            //规则:
            //1.有上传学校硬件设施的照片 
            //2.若A为公办则列举附近3所公办学校 
            //var test = await _schoolService.GetSchoolExtListAsync(Convert.ToDouble(Request.GetLongitude()), Convert.ToDouble(Request.GetLatitude()), 0, schext.City, 0, new int[] { grade }, orderBy: 1, new int[] { }, 0, new int[] { }, 1, 3);
            //3.若A为民办或者双语则列举该校所在区域的民办或双语学校 
            //var test = await _schoolService.GetSchoolExtListAsync(Convert.ToDouble(Request.GetLongitude()), Convert.ToDouble(Request.GetLatitude()), 0, city, 0, new int[] { grade }, orderBy: 1, new int[] { }, 0, new int[] { }, 1, count);
            //4.如果A校为国际化学校，则列举与该校同课程体系的国际学
            var data = new SchoolExtListDto();
            switch (schext.Type)
            {
                case 1:
                    data = await _schoolService.GetSchoolExtListsAsync(extId,Convert.ToDouble(longitude), Convert.ToDouble(latitude), schext.Province, schext.City, schext.Area, new int[] { 1, 2, 3, 4 }, orderBy: 3, new int[] { 1}, 0, new int[] {  },"",null, pageIndex, pageSize);
                    return new ResultDto() { data = data };
                case 2:
                    data = await _schoolService.GetSchoolExtListsAsync(extId, Convert.ToDouble(longitude), Convert.ToDouble(latitude), schext.Province, schext.City, schext.Area, new int[] { 1, 2, 3, 4 }, orderBy: 1, new int[] { 2 },0, new int[] { }, "",1, pageIndex, pageSize);
                    return new ResultDto() { data = data };
                case 3:
                    data = await _schoolService.GetSchoolExtListsAsync(extId, Convert.ToDouble(longitude), Convert.ToDouble(latitude), schext.Province, schext.City, schext.Area, new int[] { 1, 2, 3, 4 }, orderBy: 1, new int[] { 3}, 0, new int[] { }, schext.Courses, null, pageIndex, pageSize);
                    return new ResultDto() { data = data };
            }
            return ApiResult.Success("操作成功", new SchoolExtListDto() { PageIndex = pageIndex, PageCount = 0, PageSize = pageSize, IsRecommend = false, List = new List<SchoolExtItemDto>() }); ;
        }

        /// <summary>
        /// 综合对比
        /// </summary>
        /// <param name="comprehensiveInputDto"></param>
        /// <returns></returns>
        public async Task<ResultDto> ComprehensiveComparison(string comprehensiveInputDtoJson)
        {
            var latitude = Request.GetLatitude();
            var longitude = Request.GetLongitude();
            var userId = User.Identity.IsAuthenticated ? User.Identity.GetUserInfo().UserId : Request.GetDeviceToGuid();
            var mongoDatabase = new MongoClient(_mongosetting.ConnectionString).GetDatabase(_mongosetting.Database);
            //结果
            var result = new List<ComprehensiveOutputDto>();
            if (!string.IsNullOrEmpty(comprehensiveInputDtoJson))
            {
                var comprehensiveInputDto = JsonHelper.JSONToObject<List<ComprehensiveInputDto>>(comprehensiveInputDtoJson);
                for (int i = 0; i < comprehensiveInputDto.Count; i++)
                {
                    //分部数据
                    var SchoolExtDto = await _schoolService.GetSchoolExtDtoAsync(comprehensiveInputDto[i].ExId, Convert.ToDouble(latitude), Convert.ToDouble(longitude));
                    //传出数据
                    var SchoolExtData = new ComprehensiveOutputDto();
                    SchoolExtData.initData(SchoolExtDto);

                    //暂时不处理
                    if (SchoolExtData == null)
                    {
                        ApiResult.Fail($"ExId为：{comprehensiveInputDto[i].ExId}的学校分部为空！");
                        //return;
                    }
                    //师生比例
                    SchoolExtData.TeacherStudentRatio = "-";
                    if (SchoolExtData.Studentcount != null && SchoolExtData.Teachercount != null && SchoolExtData.Studentcount != 0 && SchoolExtData.Teachercount != 0)
                    {
                        var Number = NumberHelper.ConvertToFraction(SchoolExtData.Teachercount.Value, SchoolExtData.Studentcount.Value);
                        SchoolExtData.TeacherStudentRatio = $"{Number.Item1}:{Number.Item2}";
                    }
                    //学校点评
                    var SchoolComment = _commentService.GetSchoolSelectedComment(comprehensiveInputDto[i].ExId, userId);
                    if (SchoolComment != null)
                    {
                        var UserInfo = _userService.GetUserInfo(SchoolComment.UserId);
                        var User = UserHelper.ToAnonyUserName(new Models.User.UserInfoVo() { HeadImager = UserInfo.HeadImgUrl, Id = UserInfo.Id, NickName = UserInfo.NickName, Role = UserInfo.VerifyTypes?.ToList() }, SchoolComment.IsAnony);

                        SchoolExtData.CommentDetail = _mapper.Map<PMS.CommentsManage.Application.ModelDto.SchoolCommentDto, CommentList>(SchoolComment);
                        for (int j = 0; j < SchoolExtData.CommentDetail.Images.Count(); j++)
                        {
                            SchoolExtData.CommentDetail.Images[j] = _setting.QueryImager + SchoolExtData.CommentDetail.Images[j];
                        }
                         SchoolExtData.CommentDetail.UserInfo = User;
                    }
                    //分数
                    SchoolExtData.ScoreStatistics = _commentService.GetSchoolScoreBySchoolId(comprehensiveInputDto[i].ExId);
                    //点评
                    //学校点评 =》点评数量
                    var totalComment = _commentService.GetTotalBySchoolSectionIds(new List<Guid> { comprehensiveInputDto[i].ExId });
                    SchoolExtData.TotalReviewsNumber = totalComment.Count > 0 ? totalComment.FirstOrDefault().Total : 0;
                    var SchoolCommentDto = _commentService.AllSchoolSelectedComment(userId, totalComment.Select(t => t.SchoolSectionId).ToList(), 1);
                    foreach (var itm in SchoolCommentDto)
                    {
                        if (_userService.IsCollection(itm.Id, 3))
                            SchoolExtData.CollCommentCount++;
                    }
                    SchoolExtData.CommentData = _commentService.GetCommentDataByID(comprehensiveInputDto[i].ExId);
                    //问答
                    SchoolExtData.QuestionData = _questionInfoService.GetQuestionDataByID(comprehensiveInputDto[i].ExId);
                    var SchoolQuestion = _questionInfoService.PushSchoolInfo(new List<Guid>() { comprehensiveInputDto[i].ExId }, userId);
                    foreach (var it in SchoolQuestion)
                    {
                        if (_userService.IsCollection(it.Id, 2))
                            SchoolExtData.CollQuestionCount++;
                    }
                    //分数
                    var SchoolScore = await GetSchoolExtScoreAsync(comprehensiveInputDto[i].ExId);
                    SchoolExtData.Index = SchoolScore.index;
                    SchoolExtData.Scores = SchoolScore.score;
                    //学校排行
                    SchoolExtData.Ranks = _schoolRankService.GetRankInfoBySchID(comprehensiveInputDto[i].ExId).ToList();
                    // 升学成绩内容
                    if (SchoolExtData.AchYear != null)
                    {
                        SchoolExtData.AchData = _schoolService.GetAchData(SchoolExtData.ExtId, SchoolExtData.Grade, SchoolExtData.Type, SchoolExtData.AchYear.Value);
                    }
                    //文章
                    var Article = _articleService.GetComparisionArticles_PageVersion(new List<Guid>() { comprehensiveInputDto[i].ExId });
                    SchoolExtData.ArticleCount = Article.total;
                    SchoolExtData.Articles = Article.articles.Select(a => new ViewModels.Article.ArticleListItemViewModel()
                    {
                        Id = a.id,
                        Time = ArticleListItemTimeFormart(a.time.GetValueOrDefault()),
                        CommentCount = a.CommentCount,
                        Covers = null,
                        Digest = a.overview,
                        Layout = a.layout,
                        Title = a.title,
                        ViweCount = a.VirualViewCount
                    }).ToList();


                    var filter = Builders<BsonDocument>.Filter;
                    var projection = Builders<BsonDocument>.Projection;
                    //var collection = mongoDatabase.GetCollection<AmbientModel>("GDParams");
                    var collection = mongoDatabase.GetCollection<BsonDocument>("GDParams");
                    var doc = collection.Find(filter.Eq("eid", comprehensiveInputDto[i].ExId.ToString().ToUpper())).Project(projection.Exclude("poiinfos").Exclude("_id")).FirstOrDefault();
                    if (doc != null)
                    {
                        var ambientModel = JObject.Parse(doc.ToString()).ToObject<AmbientModel>();
                        SchoolExtData.AmbientScore = new AmbientScore()
                        {
                            Eid = new Guid(ambientModel.eid),
                            Museum = (int)ambientModel.museum,
                            Metro = (int)ambientModel.metro,
                            Market = (int)ambientModel.market,
                            Library = (int)ambientModel.library,
                            ShoppingInfo = (int)ambientModel.shoppinginfo,
                            Police = (int)ambientModel.police,
                            BookMarket = (int)ambientModel.bookmarket,
                            River = (int)ambientModel.river,
                            Rubbish = (int)ambientModel.rubbish,
                            Hospital = (int)ambientModel.hospital,
                            Subway = (int)ambientModel.subway,
                            Buildingprice = (int)ambientModel.buildingprice,
                            Traininfo = (int)ambientModel.traininfo,
                            Poiinfo = (int)ambientModel.poiinfo,
                            Play = (int)ambientModel.play
                        };
                    }
                    else
                    {
                        SchoolExtData.AmbientScore = new AmbientScore();
                    }

                    foreach (var itm in SchoolExtData.Articles)
                    {
                        if (_userService.IsCollection(itm.Id, 0))
                            SchoolExtData.CollArticleCount++;
                    }
                    //处理数据
                    SchoolExtData.Otherfees = SchoolExtData.Otherfee == null?new List<KeyValue>(): JsonHelper.JSONToObject<List<KeyValue>>(SchoolExtData.Otherfee);
                    SchoolExtData.Authentications = SchoolExtData.Authentication == null ? new List<KeyValue>() : JsonHelper.JSONToObject<List<KeyValue>>(SchoolExtData.Authentication);
                    SchoolExtData.OpenHourses = SchoolExtData.OpenHours == null ? new List<KeyValue>() : JsonHelper.JSONToObject<List<KeyValue>>(SchoolExtData.OpenHours);
                    SchoolExtData.Targets = SchoolExtData.Target == null ? new List<KeyValue>() : JsonHelper.JSONToObject<List<KeyValue>>(SchoolExtData.Target);
                    SchoolExtData.Points = SchoolExtData.Point == null ? new List<KeyValue>() : JsonHelper.JSONToObject<List<KeyValue>>(SchoolExtData.Point);
                    SchoolExtData.Courseses = SchoolExtData.Courses == null ? new List<KeyValue>() : JsonHelper.JSONToObject<List<KeyValue>>(SchoolExtData.Courses);
                    SchoolExtData.CounterParts = SchoolExtData.CounterPart == null ? new List<KeyValue>() : JsonHelper.JSONToObject<List<KeyValue>>(SchoolExtData.CounterPart);
                    SchoolExtData.Characteristics = SchoolExtData.Characteristic == null ? new List<KeyValue>() : JsonHelper.JSONToObject<List<KeyValue>>(SchoolExtData.Characteristic);
                    SchoolExtData.Calendars = SchoolExtData.Calendar == null ? new List<KeyValue>() : JsonHelper.JSONToObject<List<KeyValue>>(SchoolExtData.Calendar);
                    SchoolExtData.Code = new SchFType0(SchoolExtDto.Grade, SchoolExtDto.Type, SchoolExtDto.Discount, SchoolExtDto.Diglossia, SchoolExtDto.Chinese).Code;
                    result.Add(SchoolExtData);
                }
            }

            return ApiResult.Success("操作成功", result);
        }

        /// <summary>
        /// 获取详细数据
        /// </summary>
        /// <param name="extIdJson"></param>
        /// <returns></returns>
        public async Task<ResultDto> DetailedComparison(string extIdJson)
        {
            var latitude = Request.GetLatitude();
            var longitude = Request.GetLongitude();
            var data = new List<PMS.School.Domain.Dtos.SchoolExtDto>();
            if (!string.IsNullOrEmpty(extIdJson))
            {
                var extId = JsonHelper.JSONToObject<List<ComprehensiveInputDto>>(extIdJson);
                for (int i = 0; i < extId.Count; i++)
                {
                    var SchoolExtDto = await _schoolService.GetSchoolExtDtoAsync(extId[i].ExId, Convert.ToDouble(latitude), Convert.ToDouble(longitude));
                    SchoolExtDto.Code = new SchFType0(SchoolExtDto.Grade, SchoolExtDto.Type, SchoolExtDto.Discount, SchoolExtDto.Diglossia, SchoolExtDto.Chinese).Code;
                    data.Add(SchoolExtDto);
                }
            }
            return ApiResult.Success("操作成功", data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
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
        /// 文章列表项的时间格式
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        string ArticleListItemTimeFormart(DateTime dateTime)
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
        /// <summary>
        /// 添加一个值
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<ResultDto> SetShareParam(string param)
        {
            if(string.IsNullOrEmpty(param))
                return ApiResult.Fail("参数不能为空");
            var id = Guid.NewGuid().ToString();
            await _easyRedisClient.AddAsync($"{_ExtDataKey}{id}", param);
            return ApiResult.Success("操作成功", id);
        }
        /// <summary>
        /// 获取一个值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<ResultDto> GetShareParam(string key)
        {
            if (string.IsNullOrEmpty(key))
                return ApiResult.Fail("key不能为空");
            var data=await _easyRedisClient.GetAsync<string>($"{_ExtDataKey}{key}");
            return ApiResult.Success("操作成功", data);
        }

        #region Dto
        /// <summary>
        /// 综合对比传入参数
        /// </summary>
        public class ComprehensiveInputDto
        {
            public Guid ExId { get; set; }
            public Guid SId { get; set; }
        }
        /// <summary>
        /// 键值对
        /// </summary>
        public class KeyValue {
            public string Key{ get; set; }
            public string Value { get; set; }
        }
        /// <summary>
        /// 过滤条件传输类
        /// </summary>
        public class FilterOutputDto
        {
            /// <summary>
            /// 城市编码
            /// </summary>
            public int CityCode { get; set; }
            public List<AreaDto> Areas { get; set; }
            public List<MetroDto> Metros { get; set; }
            public List<GradeTypeDto> Types { get; set; }
            public List<SchoolExtTagViewModel> Authentications { get; set; }
            public List<PMS.School.Application.ModelDto.TagDto> Abroads { get; set; }
            public List<SchoolExtTagViewModel> Characteristics { get; set; }
            public List<PMS.School.Application.ModelDto.TagDto> Courses { get; set; }
            /// <summary>
            /// 默认选择
            /// </summary>
            public List<string> SelectedType { get; set; }
        }

        /// <summary>
        /// 综合对比输出类
        /// </summary>
        public class ComprehensiveOutputDto : PMS.School.Domain.Dtos.SchoolExtDto
        {
            /// <summary>
            /// 师生比例
            /// </summary>
            public string TeacherStudentRatio { get; set; }
            /// <summary>
            /// 点评
            /// </summary>

            public CommentList CommentDetail { get; set; }
            /// <summary>
            /// 分数统计
            /// </summary>
            public SchoolScore ScoreStatistics { get; set; }
            /// <summary>
            /// 点评总数
            /// </summary>
            public int TotalReviewsNumber { get; set; } = 0;
            /// <summary>
            /// 点评关注数
            /// </summary>
            public int? CollCommentCount { get; set; } = 0;
            /// <summary>
            /// 点评数据
            /// </summary>
            public SchCommentData CommentData { get; set; }
            /// <summary>
            /// 问答数据
            /// </summary>
            public SchQuestionData QuestionData { get; set; }
            /// <summary>
            /// 问答关注数
            /// </summary>
            public int CollQuestionCount { get; set; } = 0;

            public List<SchoolExtScoreIndex> Index { get; set; }

            /// <summary>
            /// 分数
            /// </summary>
            public List<SchoolExtScore> Scores { get; set; }
            /// <summary>
            /// 排行榜
            /// </summary>
            public List<H5SchoolRankInfoDto> Ranks { get; set; }
            /// <summary>
            /// 升学成绩内容
            /// </summary>
            public object AchData { get; set; }
            /// <summary>
            /// 文章
            /// </summary>
            public List<ArticleListItemViewModel> Articles { get; set; }
            /// <summary>
            /// 文章数量
            /// </summary>
            public int ArticleCount { get; set; } = 0;
            /// <summary>
            /// 
            /// </summary>
            public AmbientScore AmbientScore { get; set; }
            /// <summary>
            /// 文章关注数
            /// </summary>
            public int CollArticleCount { get; set; } = 0;
            //otherfee，characteristic，authentication，openHours，calendar，target，point，courses，counterPart
            public List<KeyValue> Otherfees { get; set; }
            public List<KeyValue> Characteristics { get; set; }
            public List<KeyValue> Authentications { get; set; }
            public List<KeyValue> Calendars { get; set; }
            public List<KeyValue> OpenHourses { get; set; }
            public List<KeyValue> Targets { get; set; }
            public List<KeyValue> Points { get; set; }
            public List<KeyValue> Courseses { get; set; }
            public List<KeyValue> CounterParts { get; set; }
            //public string Code { get; set; }


            public void initData(PMS.School.Domain.Dtos.SchoolExtDto schoolExtDto)
            {
                Logo = schoolExtDto.Logo;
                Sid = schoolExtDto.Sid;
                ExtId = schoolExtDto.ExtId;
                Intro = schoolExtDto.Intro;
                Distance = schoolExtDto.Distance;
                Name = schoolExtDto.Name;
                SchoolName = schoolExtDto.SchoolName;
                ExtName = schoolExtDto.ExtName;
                EName = schoolExtDto.EName;
                Grade = schoolExtDto.Grade;
                Type = schoolExtDto.Type;
                Discount = schoolExtDto.Discount;
                Diglossia = schoolExtDto.Diglossia;
                Chinese = schoolExtDto.Chinese;
                Studentcount = schoolExtDto.Studentcount;
                Teachercount = schoolExtDto.Teachercount;
                TsPercent = schoolExtDto.TsPercent;
                Province = schoolExtDto.Province;
                City = schoolExtDto.City;
                Area = schoolExtDto.Area;
                CityName = schoolExtDto.CityName;
                AreaName = schoolExtDto.AreaName;
                Tuition = schoolExtDto.Tuition;
                Applicationfee = schoolExtDto.Applicationfee;
                Otherfee = schoolExtDto.Otherfee;
                Tel = schoolExtDto.Tel;
                WebSite = schoolExtDto.WebSite;
                Address = schoolExtDto.Address;
                Lodging = schoolExtDto.Lodging;
                Sdextern = schoolExtDto.Sdextern;
                Canteen = schoolExtDto.Canteen;
                Meal = schoolExtDto.Meal;
                Characteristic = schoolExtDto.Characteristic;
                Authentication = schoolExtDto.Authentication;
                ForeignTea = schoolExtDto.ForeignTea;
                Abroad = schoolExtDto.Abroad;
                OpenHours = schoolExtDto.OpenHours;
                Calendar = schoolExtDto.Calendar;
                Range = schoolExtDto.Range;
                AfterClass = schoolExtDto.AfterClass;
                CounterPart = schoolExtDto.CounterPart;
                ExtPoint = schoolExtDto.ExtPoint;
                Comment = schoolExtDto.Comment;
                Tags = schoolExtDto.Tags;
                AchYear = schoolExtDto.AchYear;
                Achievement = schoolExtDto.Achievement;
                Age = schoolExtDto.Age;
                MaxAge = schoolExtDto.MaxAge;
                Target = schoolExtDto.Target;
                Proportion = schoolExtDto.Proportion;
                Point = schoolExtDto.Point;
                this.Count = schoolExtDto.Count;
                Courses = schoolExtDto.Courses;
                CourseAuthentication = schoolExtDto.CourseAuthentication;
                CourseCharacteristic = schoolExtDto.CourseCharacteristic;
                ExperienceVideos = schoolExtDto.ExperienceVideos;
                InterviewVideos = schoolExtDto.InterviewVideos;
                Latitude = schoolExtDto.Latitude;
                Longitude = schoolExtDto.Longitude;
                Code = schoolExtDto.Code;
            }
        }

        #endregion
    }
}
