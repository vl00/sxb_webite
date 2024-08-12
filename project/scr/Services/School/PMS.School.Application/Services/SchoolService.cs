using iSchool;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using PMS.CommentsManage.Application.Common;
using PMS.CommentsManage.Application.IServices;
using PMS.OperationPlateform.Application.IServices;
using PMS.School.Application.IServices;
using PMS.School.Application.ModelDto;
using PMS.School.Application.ModelDto.Query;
using PMS.School.Domain.Common;
using PMS.School.Domain.Dtos;
using PMS.School.Domain.Entities;
using PMS.School.Domain.Entities.Mongo;
using PMS.School.Domain.Enum;
using PMS.School.Domain.IMongo;
using PMS.School.Domain.IRepositories;
using PMS.School.Infrastructure.Common;
using PMS.Search.Application.IServices;
using PMS.Search.Application.ModelDto.Query;
using PMS.Search.Domain.IRepositories;
using PMS.Search.Domain.QueryModel;
using ProductManagement.API.Http.Interface;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.School.Application.Services
{
    public class SchoolService : ISchoolService
    {
        private readonly ISchoolRepository _repo;
        private readonly ISchoolScoreRepository _scoreRepo;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly ISchoolRankService _schoolRankService;
        private readonly ISchoolCommentService _schoolCommentService;
        private readonly ISchoolInfoService _schoolInfoService;
        private readonly IHotPopularRepository _hotPopularRepository;
        private readonly ISchoolYearFieldContentRepository _schoolYearFieldContentRepository;
        private readonly string _ExtDataKey = "ext:adData:{0}";
        private readonly string _ExtTagKey = "ext:adTag:{0}";
        private readonly string _ExtAuthTagKey = "ext:auth:adTag:{0}";
        private readonly string _ExtRecommendKey = "ext:adRecommend:{0}";
        private readonly string _ExtCharacterKey = "schoolcharacter";
        private readonly string _ExtScoreKey = "schoolscore_{0}";
        private readonly string _ExtScoreKeyIndex = "ext:adscoreindex";
        private readonly string _HotSchoolKey = "ext:hotSchools:{0}";
        private readonly string _ExtBuildKey = "ext:build_twenty:{0}";
        private readonly string _ExtBuildPriceKey = "ext:buildPrice_twenty:{0}";
        //private readonly string _ExtRangePointsKey = "ext:rangePoints:{0}";
        private readonly string _ExtSidKey = "ext:sid:{0}";
        private readonly ISchService _schService;
        private readonly IDataSearch _dataSearch;
        private readonly ISearchService _searchService;
        private readonly IWeChatSouYiSouSchoolRepository _weChatSouYiSouSchoolRepository;

        ISchoolMongoRepository _schoolMongoRepository;
        private readonly IRecommendClient _recommendClient;

        public SchoolService(ISchoolRepository repo, ISchoolScoreRepository scoreRepo, IEasyRedisClient easyRedisClient, ISchoolRankService schoolRankService, ISchoolCommentService schoolCommentService, ISchoolInfoService schoolInfoService, IHotPopularRepository hotPopularRepository, ISchoolYearFieldContentRepository schoolYearFieldContentRepository, ISchService schService, IDataSearch dataSearch, ISearchService searchService, ISchoolMongoRepository schoolMongoRepository
            , IWeChatSouYiSouSchoolRepository weChatSouYiSouSchoolRepository
            , IRecommendClient recommendClient)
        {
            _schoolMongoRepository = schoolMongoRepository;
            _repo = repo;
            _scoreRepo = scoreRepo;
            _easyRedisClient = easyRedisClient;
            _schoolRankService = schoolRankService;
            _schoolCommentService = schoolCommentService;
            _schoolInfoService = schoolInfoService;
            _hotPopularRepository = hotPopularRepository;
            _schoolYearFieldContentRepository = schoolYearFieldContentRepository;
            _schService = schService;
            _dataSearch = dataSearch;
            _searchService = searchService;
            _weChatSouYiSouSchoolRepository = weChatSouYiSouSchoolRepository;
            _recommendClient = recommendClient;
        }

        public List<SchoolDto> GetSchoolExtensions(int PageIndex, int PageSize, int provinceCode, int cityCode, int grade, int type)
        {
            var list = _repo.GetSchoolExtensions(PageIndex, PageSize, provinceCode, cityCode, grade, type);
            return list.Select(q => ConvertToSchoolDto(q)).ToList();
        }
        public int GetSchoolExtensionsCount(int provinceCode, int cityCode, int grade, int type)
        {
            return _repo.GetSchoolExtensionsCount(provinceCode, cityCode, grade, type);
        }

        public List<SchoolDto> GetSchoolExtensions(int PageIndex, int PageSize, string SchoolName = null)
        {
            var list = _repo.GetSchoolExtensions(PageIndex, PageSize, SchoolName);
            return list.Select(q => ConvertToSchoolDto(q)).ToList();
        }
        public int GetSchoolExtensionsCount(string SchoolName = null)
        {
            return _repo.GetSchoolExtensionsCount(SchoolName);
        }

        public SchoolDto GetSchoolExtensionById(Guid SchoolId, Guid BranchSchoolId)
        {
            var result = _repo.GetSchoolExtensionById(SchoolId, BranchSchoolId);
            return ConvertToSchoolDto(result);
        }

        public List<SchoolDto> GetSchoolByIds(string Ids)
        {
            var list = _repo.GetSchoolByIds(Ids);
            return list.Select(q => ConvertToSchoolDto(q)).ToList();
        }

        public SchoolDto GetSchoolExtension(Guid BranchId)
        {
            var result = _repo.GetSchoolExtension(BranchId);
            return ConvertToSchoolDto(result);
        }

        private SchoolDto ConvertToSchoolDto(SchoolExtension school)
        {

            if (school == null)
            {
                return null;
            }

            return new SchoolDto
            {
                Id = school.Id,
                SchoolId = school.SchoolId,
                SchoolName = school.SchoolName,
                SchoolType = school.SchoolType,
                SchoolGrade = school.SchoolGrade,
                Tuition = school.Tuition,
                LodgingType = LodgingUtil.Reason(school.Lodging, school.Sdextern),
                Latitude = school.Latitude,
                Longitude = school.Longitude,
                City = school.City,
                SchoolNo = school.SchoolNo
            };
        }


        public List<SchoolBranch> GetAllSchoolBranch(Guid SchoolId)
        {
            return _repo.GetAllSchoolBranch(SchoolId);
        }

        public List<Guid> GetSchoolIdBySchoolName(string schoolName)
        {
            return _repo.GetSchoolIdBySchoolName(schoolName);
        }




        /// <summary>
        /// 获取学校分部列表
        /// </summary>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="area"></param>
        /// <param name="grade"></param>
        /// <param name="orderBy"> 1推荐 2口碑 3.附近</param>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<SchoolExtListDto> GetSchoolExtListAsync(double longitude, double latitude, int province, int city, int area, int[] grade, int orderBy, int[] type, int distance, int[] lodging, int index = 1, int size = 10)
        {
            //获取列表数据
            var data = await _repo.GetSchoolExtList(longitude, latitude, province, city, area, grade, orderBy, distance, type, lodging, index, size);
            if (data.List != null && data.List.Count > 0)
            {
                //请求点评数据,榜单数据
                //拼接数据
                var rank = _schoolRankService.GetH5SchoolRankInfoBy(data.List.Select(p => p.ExtId).ToArray());
                var comment = _schoolCommentService.GetSchoolCardComment(data.List.Select(p => p.ExtId).ToList(), CommentsManage.Domain.Common.SchoolSectionOrIds.SchoolSectionIds, default(Guid));
                //comment.ForEach(x => { x.SchoolInfo = _schoolInfoService.QuerySchoolInfo(x.SchoolSectionId).Result; });

                foreach (var item in data.List)
                {
                    item.Ranks = rank.Where(p => p.SchoolId == item.ExtId).ToList();
                    var commentData = comment.FirstOrDefault(p => p.SchoolSectionId == item.ExtId);
                    item.Comment = commentData?.Content;
                    item.CommentCount = commentData == null ? 0 : commentData.ReplyCount;
                    item.CommontId = commentData?.Id;
                    item.CommontNo = UrlShortIdUtil.Long2Base32(commentData?.No ?? 0);
                    List<Tags> tags = await GetSchoolExtTagsAsync(item);
                    item.Tags = tags.Select(p => p.TagName).ToList();
                }
            }
            return data;
        }

        /// <summary>
        /// 获取学校分部列表
        /// </summary>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="area"></param>
        /// <param name="grade"></param>
        /// <param name="orderBy"> 1推荐 2口碑 3.附近</param>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public async Task<SchoolExtListDto> GetSchoolExtListsAsync(Guid extId, double longitude, double latitude, int province, int city, int area, int[] grade, int orderBy, int[] type, int distance, int[] lodging, string course = "", int? diglossia = null, int index = 1, int size = 10)
        {
            //获取列表数据
            var data = await _repo.GetSchoolExts(extId, longitude, latitude, province, city, area, grade, orderBy, distance, type, lodging, course, diglossia, index, size);
            if (data.List != null && data.List.Count > 0)
            {
                //请求点评数据,榜单数据
                //拼接数据
                var rank = _schoolRankService.GetH5SchoolRankInfoBy(data.List.Select(p => p.ExtId).ToArray());
                var comment = _schoolCommentService.GetSchoolCardComment(data.List.Select(p => p.ExtId).ToList(), CommentsManage.Domain.Common.SchoolSectionOrIds.SchoolSectionIds, default(Guid));
                //comment.ForEach(x => { x.SchoolInfo = _schoolInfoService.QuerySchoolInfo(x.SchoolSectionId).Result; });

                foreach (var item in data.List)
                {
                    item.Ranks = rank.Where(p => p.SchoolId == item.ExtId).ToList();
                    var commentData = comment.FirstOrDefault(p => p.SchoolSectionId == item.ExtId);
                    item.Comment = commentData?.Content;
                    item.CommentCount = commentData == null ? 0 : commentData.ReplyCount;
                    item.CommontId = commentData?.Id;
                    item.CommontNo = UrlShortIdUtil.Long2Base32(commentData?.No ?? 0);
                    List<Tags> tags = await GetSchoolExtTagsAsync(item);
                    item.Tags = tags.Select(p => p.TagName).ToList();
                }
            }
            return data;
        }

        /// <summary>
        /// 获取分部的推荐分部
        /// </summary>
        /// <param name="type"></param>
        /// <param name="grade"></param>
        /// <param name="city"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SchoolNameDto>> RecommendSchoolsAsync(Guid extId, int pageIndex, int pageSize)
        {
            var key = string.Format(RedisKeys.RecommendSchoolKey, extId, pageIndex, pageSize);
            return await _easyRedisClient.GetOrAddAsync(key, async () =>
            {
                var ids = await _recommendClient.RecommendSchoolIds(extId, pageIndex, pageSize);
                var schools = _repo.GetSchoolExtListByBranchIds(ids.ToList());

                //维持原有顺序
                List<SchoolNameDto> dtos = new List<SchoolNameDto>();
                foreach (var id in ids)
                {
                    var school = schools.FirstOrDefault(s => s.ExtId == id);
                    if (school != null)
                    {
                        dtos.Add(new SchoolNameDto()
                        {
                            ExtId = school.ExtId,
                            SchoolName = school.Name,
                            ShortId = school.ShortSchoolNo
                        });
                    }
                }
                return dtos;
            }, TimeSpan.FromMinutes(1));
        }

        /// <summary>
        /// 获取分部的推荐分部
        /// </summary>
        /// <param name="type"></param>
        /// <param name="grade"></param>
        /// <param name="city"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public async Task<List<KeyValueDto<Guid, Guid, string, string, int>>> RecommendSchoolAsync(Guid extid, byte type, byte grade, int city, int top)
        {
            var data = await _easyRedisClient.GetAsync<List<KeyValueDto<Guid, Guid, string, string, int>>>(string.Format(_ExtRecommendKey, extid));

            //List<KeyValueDto<Guid, Guid, string, string>> data = null;
            if (data == null)
            {
                data = _repo.RecommendSchool(type, grade, city, extid, top);
                if (data != null)
                {
                    await _easyRedisClient.AddAsync(string.Format(_ExtRecommendKey, extid.ToString()), data, DateTime.Now.AddDays(1));
                }
                return data;
            }
            return data;
        }

        public async Task<SchoolExtListDto> GetSchoolExtListbyIds(List<Guid> extIds)
        {
            SchoolExtListDto data = new SchoolExtListDto();
            //请求点评数据,榜单数据
            //拼接数据
            var rank = _schoolRankService.GetH5SchoolRankInfoBy(data.List.Select(p => p.ExtId).ToArray());
            foreach (var item in data.List)
            {
                item.Ranks = rank.Where(p => p.SchoolId == item.ExtId).ToList();

                List<Tags> tags = await GetSchoolExtTagsAsync(item);
                item.Tags = tags.Select(p => p.TagName).ToList();
            }
            return null;
        }



        /// <summary>
        /// 获取学校分部基本信息
        /// </summary>
        /// <returns>
        /// 如果return null直接return ExtNotFoundViewResult（切记）
        /// </returns>
        public async Task<SchoolExtSimpleDto> GetSchoolExtSimpleDtoAsync(double longitude, double latitude, Guid extId)
        {
            //获取学校的信息
            var data = await GetSchoolExtDetailsAsync(extId);
            if (data == null)
                //throw new NotFoundException("学校分部不存在，或者学校已下架。");
                return null;
            var dto = new SchoolExtSimpleDto();
            //榜单数据
            var rank = _schoolRankService.GetH5SchoolRankInfoBy(new Guid[] { data.ExtId });
            dto.Ranks = rank.ToList();
            dto.Name = data.Name;
            dto.EName = data.EName;
            dto.SchoolName = data.SchoolName;
            dto.ExtName = data.ExtName;
            dto.Sid = data.Sid;
            dto.AchYear = data.AchYear;
            dto.Achievement = data.Achievement;
            dto.Tuition = data.Tuition;
            dto.Applicationfee = data.Applicationfee;
            dto.Otherfee = string.IsNullOrEmpty(data.Otherfee) ? new List<KeyValueDto<double>>() : JsonHelper.JSONToObject<List<KeyValueDto<double>>>(data.Otherfee) ?? new List<KeyValueDto<double>>();
            dto.Tags = data.Tags;
            dto.Teachercount = data.Teachercount;
            dto.Studentcount = data.Studentcount;
            dto.TsPercent = data.TsPercent;
            dto.Grade = data.Grade;
            dto.Type = data.Type;
            dto.Diglossia = data.Diglossia;
            dto.Discount = data.Discount;
            dto.Canteen = data.Canteen;
            dto.Chinese = data.Chinese;
            dto.SchFType = data.SchFType;
            dto.AfterClass = data.AfterClass;
            dto.Abroad = string.IsNullOrEmpty(data.Abroad) ? new List<KeyValueDto<string>>() : JsonHelper.JSONToObject<List<KeyValueDto<string>>>(data.Abroad);
            dto.Area = data.Area;
            dto.City = data.City;
            dto.CityName = data.CityName;
            dto.AreaName = data.AreaName;
            dto.Province = data.Province;
            dto.Logo = data.Logo;
            dto.EName = dto.EName;
            dto.ExtId = data.ExtId;
            dto.ForeignTea = data.ForeignTea;
            dto.Age = data.Age;
            dto.MaxAge = data.MaxAge;
            dto.Target = string.IsNullOrEmpty(data.Target) ? new List<KeyValueDto<string>>() : JsonHelper.JSONToObject<List<KeyValueDto<string>>>(data.Target);
            dto.Proportion = data.Proportion;
            dto.Point = string.IsNullOrEmpty(data.Point) ? new List<KeyValueDto<string>>() : JsonHelper.JSONToObject<List<KeyValueDto<string>>>(data.Point);
            dto.Count = data.Count;
            dto.Courses = string.IsNullOrEmpty(data.Courses) ? new List<KeyValueDto<string>>() : JsonHelper.JSONToObject<List<KeyValueDto<string>>>(data.Courses);
            dto.CourseAuthentication = string.IsNullOrEmpty(data.CourseAuthentication) ? new List<KeyValueDto<string>>() : JsonHelper.JSONToObject<List<KeyValueDto<string>>>(data.CourseAuthentication);
            dto.CourseCharacteristic = data.CourseCharacteristic;
            dto.InterviewVideos = data.InterviewVideos;
            dto.Longitude = data.Longitude;
            dto.Latitude = data.Latitude;
            dto.Lodging = data.Lodging;
            dto.Sdextern = data.Sdextern;
            dto.Range = data.Range;
            dto.CounterPart = data.CounterPart;

            var score = _repo.GetExtScore(extId);
            foreach (var item in score)
            {
                if (item.IndexId == 22)
                    data.ExtPoint = (int)item.Score;
            }
            dto.ExtPoint = data.ExtPoint;

            //计算距离
            if (longitude != 0 && latitude != 0)
            {
                var distance = _repo.GetDistance(extId, longitude, latitude);
                dto.Distance = distance.Value;
            }
            return dto;
        }



        public IEnumerable<KeyValueDto<Guid, string, double, int, int>> GetschoolChoice(string extId, int grade, int type)
        {
            return _repo.GetschoolChoice(extId, grade, type);
        }

        /// <summary>
        /// 获取学校分部详情
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        public async Task<SchoolExtDto> GetSchoolExtDtoAsync(Guid extId, double latitude, double longitude)
        {
            //获取学校的信息
            var data = await GetSchoolExtDetailsAsync(extId);
            if (data == null)
                return null;
            //距离
            if (latitude != 0 && longitude != 0)
            {
                var distance = _repo.GetDistance(extId, longitude, latitude);
                data.Distance = distance?.Value;
            }
            return data;
        }
        /// <summary>
        /// 获取一个学校的特征数据
        /// </summary>
        /// <param name="extId">分部id</param>
        /// <returns></returns>
        public async Task<SchoolExtCharacterDto> GetSchoolCharacterAsync(Guid extId)
        {
            var result = new SchoolExtCharacterDto();
            var data = await _easyRedisClient.HashGetAllAsync<double>(_ExtCharacterKey);
            //Dictionary<string, double> data = null;
            if (data == null || data.Count == 0)
            {
                result = _repo.GetExtCharacter(extId);
                if (result != null)
                {
                    var dic = new Dictionary<string, double>();
                    dic.Add("comfort", result.Comfort ?? 0);
                    dic.Add("cost", result.Cost ?? 0);
                    dic.Add("coursetype", result.CoureType ?? 0);
                    dic.Add("edufund", result.Edufund ?? 0);
                    dic.Add("entereasy", result.EnterEasy ?? 0);
                    dic.Add("populationdensity", result.PopulationDensity ?? 0);
                    dic.Add("regionaleconomic", result.RegionaleConomic ?? 0);
                    dic.Add("safety", result.Safety ?? 0);
                    dic.Add("schoolcredentials", result.TeacherPower ?? 0);
                    dic.Add("teacherpower", result.TrafficEasy ?? 0);
                    dic.Add("trafficeasy", result.SchoolCredentials ?? 0);
                    dic.Add("upgradeeasy", result.UpgradeEasy ?? 0);
                    dic.Add("hardware", result.Hardware ?? 0);
                    dic.Add("totalscore", result.TotalScore ?? 0);
                    await _easyRedisClient.HashSetAsync<double>(_ExtCharacterKey, dic, new TimeSpan(30, 0, 0, 0, 0));
                }
                return result;
            }
            else
            {
                result.Comfort = data["comfort"];
                result.Cost = data["cost"];
                result.CoureType = data["coursetype"];
                result.Edufund = data["edufund"];
                result.EnterEasy = data["entereasy"];
                result.PopulationDensity = data["populationdensity"];
                result.RegionaleConomic = data["regionaleconomic"];
                result.Safety = data["safety"];
                result.SchoolCredentials = data["schoolcredentials"];
                result.TeacherPower = data["teacherpower"];
                result.TrafficEasy = data["trafficeasy"];
                result.UpgradeEasy = data["upgradeeasy"];
                result.Hardware = data["hardware"];
                result.TotalScore = data["totalscore"];
            }
            return result;
        }

        /// <summary>
        /// 获取学校的分数
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        public async Task<List<SchoolExtScore>> GetSchoolExtScoreAsync(Guid extId)
        {
            var result = new List<SchoolExtScore>();
            var json = await _easyRedisClient.GetAsync<string>(string.Format(_ExtScoreKey, extId));
            if (!string.IsNullOrEmpty(json))
            {
                result = JsonHelper.JSONToObject<List<SchoolExtScore>>(json);
            }
            else
            {
                result = _repo.GetExtScore(extId);
                if (result != null && result.Count > 0)
                {
                    await _easyRedisClient.AddAsync(string.Format(_ExtScoreKey, extId), JsonHelper.ObjectToJSON(result), TimeSpan.FromHours(6));
                }
            }
            return result;

        }

        /// <summary>
        /// 获取学校的分数
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<SchoolExtScore>> GetSchoolValidExtScoreAsync(Guid extId)
        {
            var json = await _easyRedisClient.GetAsync<string>(string.Format(_ExtScoreKey, extId));
            if (!string.IsNullOrEmpty(json))
            {
                return JsonHelper.JSONToObject<List<SchoolExtScore>>(json);
            }
            else
            {
                var result = await _repo.GetValidExtScore(extId);
                if (result?.Any() == true)
                {
                    await _easyRedisClient.AddAsync(string.Format(_ExtScoreKey, extId), JsonHelper.ObjectToJSON(result), TimeSpan.FromHours(6));
                }
                return result;
            }
        }

        /// <summary>
        /// 获取周边分数
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        public AmbientScore GetAmbientScore(Guid extId)
        {
            var result = new AmbientScore();
            //var data = await _easyRedisClient.HashGetAllAsync<double>(_ExtCharacterKey);
            Dictionary<string, double> data = null;
            if (data == null || data.Count == 0)
            {
                result = _repo.GetAmbientScore(extId);
                if (result != null)
                {
                    var dic = new Dictionary<string, double>();
                    dic.Add("museum", result.Museum);
                    dic.Add("metro", result.Metro);
                    dic.Add("library", result.Library);
                    dic.Add("shoppingInfo", result.ShoppingInfo);
                    dic.Add("police", result.Police);
                    dic.Add("bookMarket", result.BookMarket);
                    dic.Add("river", result.River);
                    dic.Add("bus", result.Bus);
                    dic.Add("rubbish", result.Rubbish);
                    dic.Add("hospital", result.Hospital);
                    dic.Add("subway", result.Subway);
                    dic.Add("view", result.View);
                    dic.Add("buildingprice", result.Buildingprice);
                    dic.Add("traininfo", result.Traininfo);
                    dic.Add("poiinfo", result.Poiinfo);
                    dic.Add("play", result.Play);
                    //await _easyRedisClient.HashSetAsync<double>(_ExtCharacterKey, dic, new TimeSpan(30, 0, 0, 0, 0));
                }
                return result;
            }
            else
            {
                result.Museum = Convert.ToInt32(data["museum"]);
                result.Metro = Convert.ToInt32(data["metro"]);
                result.Library = Convert.ToInt32(data["library"]);
                result.ShoppingInfo = Convert.ToInt32(data["shoppingInfo"]);
                result.Police = Convert.ToInt32(data["police"]);
                result.BookMarket = Convert.ToInt32(data["bookMarket"]);
                result.River = Convert.ToInt32(data["river"]);
                result.Bus = Convert.ToInt32(data["bus"]);
                result.Rubbish = Convert.ToInt32(data["rubbish"]);
                result.Hospital = Convert.ToInt32(data["hospital"]);
                result.Subway = Convert.ToInt32(data["subway"]);
                result.View = Convert.ToInt32(data["view"]);
                result.Buildingprice = Convert.ToDouble(data["buildingprice"]);
                result.Traininfo = Convert.ToInt32(data["traininfo"]);
                result.Poiinfo = Convert.ToInt32(data["Poiinfo"]);
                result.Play = Convert.ToInt32(data["Play"]);
            }
            return result;
        }

        /// <summary>
        /// 获取学校分数选项
        /// </summary>
        /// <returns></returns>
        public async Task<List<SchoolExtScoreIndex>> schoolExtScoreIndexsAsync()
        {
            List<SchoolExtScoreIndex> result = await _easyRedisClient.GetOrAddAsync(_ExtScoreKeyIndex, () =>
            {
                return _repo.GetExtScoreIndex();
            });

            return result;

        }


        /// <summary>
        /// 获取学校详情
        /// 进行缓存
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<SchoolExtDto> GetSchoolExtDetailsAsync(Guid extId)
        {
            Guid sid = Guid.Empty;
            if (sid == Guid.Empty)
            {
                var sidData = await _easyRedisClient.GetOrAddAsync(string.Format(_ExtSidKey, extId), () => { return _repo.GetSchoolId(extId).FirstOrDefault(); });
                if (sidData == null || sidData.Value == null)
                {
                    return null;
                }
                sid = sidData.Value.Value;
            }
            //var data = await _easyRedisClient.HashGetAsync<SchoolExtDto>(string.Format(_ExtDataKey, sid.ToString()), extId.ToString());
            SchoolExtDto data = null;
            if (data == null)
            {

                data = _repo.GetSchoolExtDetails(extId);
                //查询不到
                if (data == null) return null;
                var videos = _repo.GetExtViedeos(extId);
                //体验课程
                var experience = (byte)VideoType.Experience;
                data.ExperienceVideos = videos.Where(p => p.Message == experience)?.ToList();
                //学校专访
                var interview = (byte)VideoType.Interview;
                data.InterviewVideos = videos.Where(p => p.Message == interview)?.ToList();
                //学校标签
                data.Tags = (await GetSchoolExtTagsAsync(new SchoolExtItemDto { Authentication = data.Authentication, Courses = data.Courses, Characteristic = data.Characteristic, Sid = data.Sid, ExtId = data.ExtId })).Select(p => p.TagName).ToList();
                //var fieldContent = _schoolYearFieldContentRepository.GenAllContent(extId.ToString());
                //#region 字段修改为键值对写入
                ////划片范围
                //data.Range = fieldContent.Where(f => f.Field == EnumExtension.GetName<SchoolExtFiledYearTag>(SchoolExtFiledYearTag.Range)).OrderByDescending(f => f.Year).FirstOrDefault()?.Content;
                ////招生年龄段
                //var AgeCount = fieldContent.Where(f => f.Field == EnumExtension.GetName<SchoolExtFiledYearTag>(SchoolExtFiledYearTag.Age)).OrderByDescending(f => f.Year).FirstOrDefault()?.Content;
                //var MaxAgeCount = fieldContent.Where(f => f.Field == EnumExtension.GetName<SchoolExtFiledYearTag>(SchoolExtFiledYearTag.MaxAge)).OrderByDescending(f => f.Year).FirstOrDefault()?.Content;
                //if(!string.IsNullOrEmpty(AgeCount))
                //    data.Age = Convert.ToByte(AgeCount);
                //if (!string.IsNullOrEmpty(MaxAgeCount))
                //    data.MaxAge = Convert.ToByte(MaxAgeCount);
                ////招生人数
                //var Count = fieldContent.Where(f => f.Field == EnumExtension.GetName<SchoolExtFiledYearTag>(SchoolExtFiledYearTag.Count)).OrderByDescending(f => f.Year).FirstOrDefault()?.Content;
                //if (!string.IsNullOrEmpty(Count))
                //    data.Count = Convert.ToInt32(Count);
                ////招生对象
                //data.Target = fieldContent.Where(f => f.Field == EnumExtension.GetName<SchoolExtFiledYearTag>(SchoolExtFiledYearTag.Target)).OrderByDescending(f => f.Year).FirstOrDefault()?.Content;
                ////申请费用
                //var Applicationfee = fieldContent.Where(f => f.Field == EnumExtension.GetName<SchoolExtFiledYearTag>(SchoolExtFiledYearTag.Applicationfee)).OrderByDescending(f => f.Year).FirstOrDefault()?.Content;
                //if (!string.IsNullOrEmpty(Applicationfee))
                //    data.Applicationfee = Convert.ToDouble(Applicationfee);
                ////学费
                //var Tuition = fieldContent.Where(f => f.Field == EnumExtension.GetName<SchoolExtFiledYearTag>(SchoolExtFiledYearTag.Tuition)).OrderByDescending(f => f.Year).FirstOrDefault()?.Content;
                //if (!string.IsNullOrEmpty(Tuition))
                //    data.Tuition = Convert.ToDouble(Tuition);
                //#endregion
                var result = await _easyRedisClient.HashSetAsync(string.Format(_ExtDataKey, sid.ToString()), extId.ToString(), data, new TimeSpan(7, 0, 0, 0));
            }

            return data;
        }

        /// <summary>
        /// 获取学校划片范围
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        public string GetSchoolRange(Guid extId)
        {
            return _repo.GetSchoolExtRange(extId);
        }
        /// <summary>
        /// 获取附近学位房
        /// </summary>
        /// <param name="extId"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public async Task<List<KeyValueDto<double, double, string>>> GetBuildingDataAsync(Guid extId, double? latitude, double? longitude, float distance = 3000)
        {
            var data = await _easyRedisClient.GetOrAddAsync(string.Format(_ExtBuildKey, extId), () =>
            {
                return _repo.GetBuildingData(extId, latitude, longitude, distance);
            }, DateTime.Now.AddDays(7));
            return data;
        }
        public async Task<List<KeyValueDto<double, double, string, int>>> GetBuildingPriceDataAsync(Guid extId, double? latitude, double? longitude, float distance = 3000)
        {
            var data = await _easyRedisClient.GetOrAddAsync(string.Format(_ExtBuildPriceKey, extId), () =>
            {
                return _repo.GetBuildingPriceData(extId, latitude, longitude, distance);
            }, DateTime.Now.AddDays(7));
            return data;
        }
        public List<SmallLocation> GetSchoolExtRangePointsAsync(Guid extId)
        {
            return _repo.GetSchoolExtRangePoints(extId);
            //var data = await _easyRedisClient.GetOrAddAsync(string.Format(_ExtRangePointsKey, extId), () =>
            //{
            //    return _repo.GetSchoolExtRangePoints(extId);
            //}, DateTime.Now.AddDays(7));
            //return data;
        }
        /// <summary>
        /// 获取附近学位房
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
        public async Task<List<KeyValueDto<double, double, string>>> GetBuildingDataAsync(Guid extId, double? latitude, double? longitude, double neLat, double swLat, double neLng, double swLng, float distance = 3000)
        {
            //获取范围内的学位房
            var data = await GetBuildingDataAsync(extId, latitude, longitude, distance);
            ////设置矩形的坐标点
            //var g = Microsoft.SqlServer.Types.SqlGeography.STGeomFromText(
            //new System.Data.SqlTypes.SqlChars($"POLYGON(({neLng} {neLat}, {neLng} {swLat}, {swLng} {swLat},{swLng} {neLat},{neLng} {neLat}))"), 4326);
            ////筛选出来
            //var result = data.Where(p =>
            //  {
            //      var h = Microsoft.SqlServer.Types.SqlGeography.Point(p.Value, p.Key, 4326);
            //      return h.STIntersects(g).IsTrue;
            //  }).ToList();
            double maxlat, minlat, maxlng, minlng;
            //取出精度维度最大值
            if (swLat > neLat)
            {
                maxlat = swLat;
                minlat = neLat;
            }
            else
            {
                maxlat = neLat;
                minlat = swLat;
            }

            if (swLng > neLng)
            {
                maxlng = swLng;
                minlng = neLng;
            }
            else
            {
                maxlng = neLng;
                minlng = swLng;
            }

            var result = data.Where(p => p.Key >= minlng && p.Key <= maxlng && p.Value >= minlat && p.Value <= maxlat).ToList();
            return result;
        }

        /// <summary>
        /// 获取学校
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        public List<KeyValueDto<Guid>> GetSchoolExtAdress(params Guid[] extId)
        {
            var data = _repo.GetSchoolExtAddress(extId);
            return data;
        }
        public async Task<IEnumerable<KeyValueDto<Guid, int, string, string>>> GetExtAddresses(IEnumerable<Guid> eids)
        {
            if (eids?.Any() == true)
            {
                return await _repo.GetSchoolExtAddress(eids);
            }
            return null;
        }


        /// <summary>
        /// 获得pc首页热门推荐
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, List<SchoolCollectionDto>>> GetPCHotSchool(int city, int day = 7, int count = 6)
        {
            var now = DateTime.Now;
            var time = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
            //await _easyRedisClient.RemoveAsync(string.Format(_HotSchoolKey, city));
            var hotschool = await _easyRedisClient.GetOrAddAsync(string.Format(_HotSchoolKey, city), async () =>
             {
                 Dictionary<string, List<SchoolCollectionDto>> result = new Dictionary<string, List<SchoolCollectionDto>>();
                 foreach (var grade in new byte[] { (byte)SchoolGrade.Kindergarten, (byte)SchoolGrade.PrimarySchool, (byte)SchoolGrade.JuniorMiddleSchool, (byte)SchoolGrade.SeniorMiddleSchool })
                 {
                     foreach (var type in new byte[] { (byte)SchoolType.Private, (byte)SchoolType.Public, (byte)SchoolType.International })
                     {
                         await Task.Run(async () =>
                         {
                             var data = _hotPopularRepository.GetHotVisitSchextIds(city, grade, type, day, count);
                             var dto = await GetCollectionExtsAsync(data.Select(p => p.Eid).ToArray(), data);
                             result.Add($"{grade}.{type}", dto);
                         });
                     }
                 }
                 return result;
             }, time);
            return hotschool;
        }

        /// <summary>
        /// 获取pc首页学校数据
        /// </summary>
        /// <param name="city"></param>
        /// <param name="day"></param>
        /// <param name="count"></param>
        /// <returns></returns>

        public async Task<Dictionary<string, List<SchoolCollectionDto>>> GetPCIndexDataAsync(int city, int day = 7, int count = 6)
        {

            Dictionary<string, List<SchoolCollectionDto>> result = new Dictionary<string, List<SchoolCollectionDto>>();

            var time = DateTime.Now.AddDays(1);

            //本周热度最高（pv）
            //当前时间倒推7天内，该定位区域、该学段点击量最高的6所学校
            //-------------七天内pv最高---------------

            result.Add("hotschool", new List<SchoolCollectionDto>());

            //-------------口碑评分最高---------------
            var commentScores = await _easyRedisClient.GetOrAddAsync($"ext:pcindex:commentscore:{city}", async () =>
            {
                return await GetScoreData(city, count, time, 1, "ext:pcindex:allcommentscore");
            }, time);
            result.Add("commentscore", commentScores);


            //-------------总评评分最高--------------
            var extScores = await _easyRedisClient.GetOrAddAsync($"ext:pcindex:extscore:{city}", async () =>
            {
                return await GetScoreData(city, count, time, 2, "ext:pcindex:allextscore");
            }, time);
            result.Add("extscore", extScores);

            return result;
        }
        /// <summary>
        /// 获取学部分数
        /// </summary>
        /// <param name="city"></param>
        /// <param name="count"></param>
        /// <param name="time"></param>
        /// <param name="scoreType">1.评论 2.学校</param>
        /// <returns></returns>
        private async Task<List<SchoolCollectionDto>> GetScoreData(int city, int count, DateTime time, byte scoreType, string allscoreName)
        {
            var extIds = _repo.GetScoreExtData(city, count, scoreType);
            if (extIds.Count() < 24)
            {
                //获取全国总评分数最高的学部
                var allCommentScore = await _easyRedisClient.GetOrAddAsync(allscoreName, () =>
                {
                    return _repo.GetScoreExtData(0, count, scoreType);
                }, time);

                //该城市缺少评论的年级
                var lacks = new byte[] {
                        (byte)SchoolGrade.Kindergarten,
                        (byte)SchoolGrade.PrimarySchool,
                        (byte)SchoolGrade.JuniorMiddleSchool,
                        (byte)SchoolGrade.SeniorMiddleSchool }
                   .Where(p => !extIds.Select(e => e.Value).Distinct().Contains(p));
                //查询缺少数据的学校，用全国学校其他学校来替补
                foreach (var item in extIds.GroupBy(p => p.Value).Where(p => p.Count() < count))
                {
                    extIds.AddRange(allCommentScore
                        .Where(p => p.Value == item.Key && !item.Select(i => i.Key).Contains(p.Key))
                        .Take(count - item.Count()));
                }
                foreach (var item in lacks)
                {
                    extIds.AddRange(allCommentScore.Where(p => p.Value == item));
                }
            }
            //查询数据
            List<SchoolCollectionDto> data = await GetCollectionExtsAsync(extIds.Select(p => p.Key).ToArray(), extIds.Select(p => (p.Data, p.Key)).ToArray());
            return data;
        }

        /// <summary>
        /// 获取列表中学校Tags
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private async Task<List<string>> GetSchoolExtTagsInList(SchoolExtItemDto item, bool onlyAuthTag = false)
        {
            var gradeType = GetGradeTypeList();
            List<Tags> tags = !onlyAuthTag ?
                await GetSchoolExtTagsAsync(item) :
                await GetSchoolExtAuthTagsAsync(item);
            List<string> list = tags.Select(p => p.TagName).ToList();

            var typeTag = gradeType.FirstOrDefault(q => q.GradeId == item.Grade)?
                .Types.FirstOrDefault(q => q.TypeId == item.Type
                        && q.Discount == (item.Discount ? 1 : 0)
                        && q.Diglossia == (item.Diglossia ? 1 : 0)
                        && q.Chinese == (item.Chinese ? 1 : 0))?.TypeName;
            if (!string.IsNullOrWhiteSpace(typeTag))
            {
                list.Insert(0, typeTag);
            }
            return list.Take(6).ToList();
        }

        /// <summary>
        /// 获取学校Tags
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private async Task<List<Tags>> GetSchoolExtTagsAsync(SchoolExtItemDto item)
        {
            //重缓存中取出学校分部标签
            List<Tags> tags = await _easyRedisClient.HashGetAsync<List<Tags>>(string.Format(_ExtTagKey, item.Sid.ToString()), item.ExtId.ToString());

            if (tags == null)
            {
                tags = new List<Tags>();
                //学校认证
                if (!string.IsNullOrEmpty(item.Authentication))
                {
                    var auth = JsonHelper.JSONToObject<List<KeyValueDto<string>>>(item.Authentication);
                    if (auth.Count > 0)
                    {
                        var tagCount = auth.Count(p => p.Key != "未收录" && p.Key.Count() <= 8);
                        if (tagCount > 0)
                        {
                            tags.Add(auth.Select(p => new Tags { TagId = p.Value, TagName = p.Key })
            .First(p => p.TagName != "未收录" && p.TagName.Count() <= 8));
                        }

                    }
                }
                //课程设置
                //            if (!string.IsNullOrEmpty(item.Courses))
                //            {
                //                var course = JsonHelper.JSONToObject<List<KeyValueDto<string>>>(item.Courses);
                //                if (course.Count > 0)
                //                {
                //                    var tagCount = course.Count(p => p.Key != "未收录");
                //                    if (tagCount > 0)
                //                    {
                //                        tags.Add(course.Select(p => new Tags { TagId = p.Value, TagName = p.Key })
                //.First(p => p.TagName != "未收录"));
                //                    }
                //                }
                //            }
                //特色课程
                if (!string.IsNullOrEmpty(item.Characteristic))
                {
                    var count = 5 - (tags.Count);
                    var characteristic = JsonHelper.JSONToObject<List<KeyValueDto<string>>>(item.Characteristic);
                    tags.AddRange(characteristic.Select(p => new Tags { TagId = p.Value, TagName = p.Key })
                        .Where(p => p.TagName != "未收录" && p.TagName.Count() <= 8)
                        .Take(count));
                    //添加缓存
                    if (tags.Count() > 0)
                    {
                        var result = _easyRedisClient.HashSetAsync(string.Format(_ExtTagKey, item.Sid.ToString()), item.ExtId.ToString(), tags, new TimeSpan(7, 0, 0, 0));
                    }
                }
            }
            return tags;
        }

        /// <summary>
        /// 学校认证tags
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private async Task<List<Tags>> GetSchoolExtAuthTagsAsync(SchoolExtItemDto item)
        {
            //重缓存中取出学校分部标签
            List<Tags> tags = await _easyRedisClient.HashGetAsync<List<Tags>>(string.Format(_ExtAuthTagKey, item.Sid.ToString()), item.ExtId.ToString());

            if (tags == null)
            {
                tags = new List<Tags>();
                //学校认证
                if (!string.IsNullOrEmpty(item.Authentication))
                {
                    var auth = JsonHelper.JSONToObject<List<KeyValueDto<string>>>(item.Authentication);
                    if (auth.Count > 0)
                    {
                        var tagCount = auth.Count(p => p.Key != "未收录" && p.Key.Count() <= 8);
                        if (tagCount > 0)
                        {
                            tags.Add(auth.Select(p => new Tags { TagId = p.Value, TagName = p.Key })
            .First(p => p.TagName != "未收录" && p.TagName.Count() <= 8));
                        }

                    }
                }
            }
            return tags;
        }

        /// <summary>
        /// 获取学校招生计划
        /// </summary>
        /// <param name="extId"></param>
        /// <param name="sid"></param>
        /// <returns></returns>
        public SchoolExtRecruitDto GetSchoolExtRecruit(Guid extId)
        {
            var data = _repo.GetSchoolExtRecruit(extId);
            if (data == null)
                //throw new NotFoundException("学校被删除或者已经被下架。");
                return null;
            return data;
        }
        /// <summary>
        /// 获取学校升学成绩
        /// </summary>
        /// <param name="extId"></param>
        /// <param name="grade"></param>
        /// <param name="type"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public object GetAchData(Guid extId, byte grade, byte type, int year)
        {
            return _repo.GetAchievementData(extId, type, grade, year);
        }
        /// <summary>
        /// 获取学校升学成绩的所有年份
        /// </summary>
        /// <param name="extId">学部ID</param>
        /// <param name="grade">招生年级</param>
        /// <param name="type">学校类型</param>
        /// <returns></returns>
        public List<int> GetAchYears(Guid extId, byte grade, byte type)
        {
            return _repo.GetAchievementYears(extId, type, grade);
        }

        public List<TagDto> GetTagList()
        {
            var result = _repo.GetTagList();
            return result?.Select(tag => new TagDto
            {
                Id = tag.Id,
                No = tag.No,
                Name = tag.Name,
                Sort = tag.Sort,
                SpellCode = tag.SpellCode,
                Type = tag.Type
            }).ToList();
        }

        public async Task<IEnumerable<TagFlat>> GetTagFlats()
        {
            var key = RedisKeys.SchoolTagsKey;
            //cache first
            var tags = await _easyRedisClient.GetAsync<IEnumerable<TagFlat>>(key) ?? Enumerable.Empty<TagFlat>();
            if (!tags.Any())
            {
                tags = await _repo.GetTagFlats();
                //cache store
                await _easyRedisClient.AddAsync(key, tags, TimeSpan.FromDays(1));
            }
            return tags;
        }

        public List<SchoolTypeDto> GetSchoolTypeList()
        {
            var gradeTypeList = GetGradeTypeList();
            var types = new List<SchoolTypeDto>();

            foreach (var gradeType in gradeTypeList)
            {
                types.AddRange(gradeType.Types);
            }
            return types;
        }

        /// <summary>
        /// 获取学校类型列表
        /// </summary>
        /// <returns></returns>
        public List<GradeTypeDto> GetGradeTypeList()
        {
            var result = SchFTypeUtil.Dict.Select(_ =>
              {
                  SchFTypeUtil.ReslAttrs(_.attrs, out var grade, out var type, out var discount, out var diglossia, out var chinese);
                  return new SchoolTypeDto { TypeName = _.desc, GradeId = grade, TypeId = type, Discount = discount ? 1 : 0, Diglossia = diglossia ? 1 : 0, Chinese = chinese ? 1 : 0 };
              })
            .GroupBy(_ => _.GradeId)
            .Select(tys => new GradeTypeDto
            {
                GradeId = tys.Key,
                Types = tys.Where(t => t.TypeName != "港澳台幼儿园" && t.TypeName != "港澳台小学" && t.TypeName != "港澳台初中" && t.TypeName != "港澳台高中").ToList(),
                GradeDesc = ((SchoolGrade)tys.Key).Description()
            }).ToList();

            return result;
        }

        /// <summary>
        /// 根据分部ID获取学校分部信息
        /// </summary>
        /// <param name="eids"></param>
        /// <returns></returns>
        public async Task<List<SchoolExtFilterDto>> ListExtSchoolByBranchIds(List<Guid> eids, double latitude = 0, double longitude = 0, bool readIntro = false, bool onlyAuthTag = false)
        {
            var result = _repo.GetSchoolExtListByBranchIds(eids, latitude, longitude, readIntro);

            var gradeType = GetGradeTypeList();

            foreach (var item in result)
            {
                var tags = await GetSchoolExtTagsInList(item, onlyAuthTag);
                item.Tags = tags;
            }
            return result.Select(q => new SchoolExtFilterDto
            {
                Sid = q.Sid,
                ExtId = q.ExtId,
                Intro = q.Intro,
                ExtIntro = q.ExtIntro,
                Name = q.Name,

                TeacherCount = q.TeacherCount,
                StudentCount = q.StudentCount,
                CreationDate = q.CreationDate?.DateTime,

                Province = q.Province,
                City = q.City,
                Area = q.Area,
                ProvinceCode = q.ProvinceCode,
                CityCode = q.CityCode,
                AreaCode = q.AreaCode,
                CommentCount = q.CommentCount,
                Score = q.Score,
                Distance = q.Distance,
                Tags = q.Tags,
                Lodging = q.Lodging,
                Sdextern = q.Sdextern,
                Tuition = q.Tuition,
                Grade = q.Grade,
                Type = q.Type,
                ExtValid = q.ExtValid,
                SchoolValid = q.SchoolValid,
                Status = q.Status,
                SchoolNo = q.SchoolNo,
                SchFType0 = new SchFType0((byte)q.Grade, q.Type, q.Discount, q.Diglossia, q.Chinese)
            }).ToList();
        }

        /// <summary>
        /// 根据学校ID获取学校分部信息
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public async Task<List<SchoolExtFilterDto>> ListExtSchoolBySchoolId(Guid sid, double latitude = 0, double longitude = 0)
        {
            var result = _repo.GetSchoolExtListBySchoolId(sid, latitude, longitude);
            var gradeType = GetGradeTypeList();

            foreach (var item in result)
            {
                var tags = await GetSchoolExtTagsInList(item);
                item.Tags = tags;
            }
            return result.Select(q => new SchoolExtFilterDto
            {
                Sid = q.Sid,
                ExtId = q.ExtId,
                Name = q.Name,
                Province = q.Province,
                City = q.City,
                Area = q.Area,
                ProvinceCode = q.ProvinceCode,
                CityCode = q.CityCode,
                AreaCode = q.AreaCode,
                CommentCount = q.CommentCount,
                Score = q.Score,
                Distance = q.Distance,
                Tags = q.Tags,
                Tuition = q.Tuition,
                Lodging = q.Lodging,
                Sdextern = q.Sdextern,
                Type = q.Type,
                Grade = q.Grade,
                ExtValid = q.ExtValid,
                SchoolValid = q.SchoolValid,
                Status = q.Status,
                SchoolNo = q.SchoolNo
            }).ToList();
        }



        public async Task<List<SchoolExtFilterDto>> PageExtSchool(ListExtSchoolQuery query)
        {

            List<int> gradeIds = new List<int>();
            List<int> typeIds = new List<int>();
            List<bool> discount = new List<bool>();
            List<bool> diglossia = new List<bool>();
            List<bool> chinese = new List<bool>();
            if (query.Type.Count > 0)
            {
                foreach (var t in query.Type)
                {
                    var tt = t.Split('.');
                    gradeIds.Add(Convert.ToInt32(tt[0]));
                    if (tt.Count() > 1)
                    {
                        typeIds.Add(Convert.ToInt32(tt[1]));
                    }
                    if (tt.Count() > 2)
                    {
                        discount.Add(tt[2] == "1");
                        diglossia.Add(tt[3] == "1");
                        chinese.Add(tt[4] == "1");
                    }
                }
                gradeIds = gradeIds.GroupBy(q => q).Select(q => q.Key).ToList();
                typeIds = typeIds.GroupBy(q => q).Select(q => q.Key).ToList();
                discount = discount.GroupBy(q => q).Select(q => q.Key).ToList();
                diglossia = diglossia.GroupBy(q => q).Select(q => q.Key).ToList();
                chinese = chinese.GroupBy(q => q).Select(q => q.Key).ToList();
            }
            var result = _repo.GetSchoolExtFilterList(query.Longitude, query.Latitude,
            query.ProvinceCode, query.CityCode, query.AreaCodes, query.MetroLineIds, query.MetroStationIds,
            gradeIds, typeIds, discount, diglossia, chinese,
            query.Orderby, query.Distance, query.MinCost, query.MaxCost,
            query.Lodging, query.AuthIds, query.CharacIds, query.AbroadIds, query.CourseIds, query.PageNo, query.PageSize);

            var gradeType = GetGradeTypeList();
            foreach (var item in result)
            {
                var tags = await GetSchoolExtTagsInList(item);
                item.Tags = tags;
            }
            return result.Select(q => new SchoolExtFilterDto
            {
                Sid = q.Sid,
                ExtId = q.ExtId,
                Name = q.Name,
                Province = q.Province,
                City = q.City,
                Area = q.Area,
                CommentCount = q.CommentCount,
                Score = q.Score,
                Distance = q.Distance,
                Lodging = q.Lodging,
                Sdextern = q.Sdextern,
                Tags = q.Tags,
                Tuition = q.Tuition,
                Type = q.Type
            }).ToList();
        }

        public async Task<Dictionary<Guid, string>> GetAuthTagsBySchoolType(bool International)
        {
            string Key = $"ext:AuthTags:International{International}";
            var result = await _easyRedisClient.GetOrAddAsync(Key,
                () =>
                {
                    var list = _repo.GetAuthTagsBySchoolType(International);
                    return list?.Select(q => new { TagId = q.DataId, TagName = q.TagName }).ToList();
                }, new TimeSpan(1, 0, 0));


            return result.ToDictionary(x => x.TagId, x => x.TagName);
        }

        public async Task<Dictionary<Guid, string>> GetCharacTagsBySchoolType(bool International)
        {
            string Key = $"ext:CharacTags:International{International}";
            var result = await _easyRedisClient.GetOrAddAsync(Key,
                () =>
                {
                    var list = _repo.GetCharacTagsBySchoolType(International);
                    return list?.Select(q => new { TagId = q.DataId, TagName = q.TagName }).ToList();
                }, new TimeSpan(1, 0, 0));
            return result.ToDictionary(x => x.TagId, x => x.TagName);
        }


        public List<KeyValueDto<DateTime, string, byte>> GetExtVideo(Guid extId, byte type = 0)
        {
            var data = _repo.GetExtViedeos(extId, type);
            return data;
        }
        /// <summary>
        /// 获取升学成绩的列表
        /// </summary>
        /// <param name="extId"></param>
        /// <param name="grade"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<KeyValueDto<int, string, double, byte, Guid>> GetAchievementList(Guid extId, byte grade)
        {
            return _repo.GetAchievementList(extId, grade);
        }

        /// <summary>
        /// 获取年份升学成绩的列表
        /// </summary>
        /// <param name="extId"></param>
        /// <param name="grade"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public List<KeyValueDto<Guid, string, double, int>> GetYearAchievementList(Guid extId, byte grade, int year)
        {
            return _repo.GetYearAchievementList(extId, grade, year);
        }

        /// <summary>
        ///根据学校id获取分部名字
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public List<KeyValueDto<Guid>> GetSchoolExtName(Guid sid)
        {
            return _repo.GetSchoolExtName(sid);
        }

        /// <summary>
        /// 根据分部id集合查询学校简介
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<List<SchoolCollectionDto>> GetCollectionExtsAsync(Guid[] ids, (Guid Sid, Guid Eid)[] list = null, bool contain = false)
        {

            List<SchoolCollectionDto> result = new List<SchoolCollectionDto>();
            if (ids == null || ids.Length == 0)
                return result;

            var schIds = new List<KeyValueDto<Guid, Guid?, string>>();

            if (list == null)
            {
                schIds = _repo.GetSchoolId(ids);
            }
            else
            {
                schIds = list.Select(p => new KeyValueDto<Guid, Guid?, string> { Key = p.Eid, Value = p.Sid }).ToList();
            }

            //获取榜单信息和评论信息
            IEnumerable<OperationPlateform.Domain.DTOs.H5SchoolRankInfoDto> rank = null;
            List<CommentsManage.Application.ModelDto.SchoolCommentDto> comment = null;
            if (contain)
            {
                rank = _schoolRankService.GetH5SchoolRankInfoBy(ids);
                comment = _schoolCommentService.GetSchoolCardComment(ids.ToList(), CommentsManage.Domain.Common.SchoolSectionOrIds.SchoolSectionIds, default(Guid));
            }


            foreach (var eid in ids)
            {
                var sid = schIds.Where(p => p.Key == eid).FirstOrDefault();
                if (sid != null && sid.Value != null)
                {
                    //查询到学校
                    var ext = await GetSchoolExtDetailsAsync(eid);
                    if (ext == null)
                    {
                        result.Add(new SchoolCollectionDto { ExtId = eid });
                    }
                    else
                    {
                        //var rank = _schoolRankService.GetH5SchoolRankInfoBy(ids);
                        var dto = new SchoolCollectionDto
                        {
                            Area = ext.Area,
                            City = ext.City,
                            Province = ext.Province,
                            ExtId = eid,
                            ExtName = ext.ExtName,
                            SchoolName = ext.Name,
                            Latitude = ext.Latitude,
                            Longitude = ext.Longitude,
                            Grade = ext.Grade,
                            Sid = ext.Sid,
                            Tags = ext.Tags,
                            Tuition = ext.Tuition,
                            Type = ext.Type,
                            Lodging = ext.Lodging,
                            Sdextern = ext.Sdextern,
                            Chinese = ext.Chinese,
                            Diglossia = ext.Diglossia,
                            Discount = ext.Discount,
                            AreaName = ext.AreaName,
                            CityName = ext.CityName,
                            TypeName = new iSchool.SchFType0(ext.Grade, ext.Type, ext.Discount, ext.Diglossia, ext.Chinese).GetDesc()
                        };
                        //学校总评
                        var score = (await GetSchoolExtScoreAsync(eid)).FirstOrDefault(p => p.IndexId == 22);
                        dto.Score = score?.Score;

                        //包含排行榜 点评 问答
                        if (contain)
                        {
                            dto.Ranks = rank.Where(p => p.SchoolId == eid).ToList();
                            var commentData = comment.FirstOrDefault(p => p.SchoolSectionId == eid);
                            dto.Comment = commentData?.Content;
                            dto.CommentCount = commentData == null ? 0 : commentData.ReplyCount;
                            dto.CommontId = commentData?.Id;
                            dto.CommontNo = UrlShortIdUtil.Long2Base32(commentData?.No ?? 0);
                        }
                        result.Add(dto);
                    }
                }
                else
                {
                    //没有查询到学校
                    result.Add(new SchoolCollectionDto { ExtId = eid });
                }
            }
            return result;
        }

        /// <summary>
        /// 根据分部id获取相同类型的学校
        /// </summary>
        /// <param name="SchoolId"></param>
        /// <returns></returns>
        public async Task<List<SchoolExtItemDto>> GetIdenticalSchool(Guid SchoolExtId, int PageIndex, int PageSize)
        {
            var ExtInfo = _repo.GetSchoolExtInfoDto(new List<Guid>() { SchoolExtId }).FirstOrDefault();
            //根据该类型得到该相同类型的学校，进行推送
            var result = await GetSchoolExtListAsync(ExtInfo.Longitude, ExtInfo.Latitude, ExtInfo.Province, ExtInfo.City, ExtInfo.Area, new int[] { (int)ExtInfo.Grade }, 0, new int[] { (int)ExtInfo.Type }, 0, new int[] { }, PageIndex, PageSize);

            return result.List;
        }



        public List<SchoolESDataDto> GetSchoolDataBySchoolId(Guid SchoolId)
        {
            var data = _repo.GetSchoolDataBySchoolId(SchoolId);
            var gradeType = GetGradeTypeList();
            return data.Select(q => new SchoolESDataDto
            {
                Id = q.Id,
                SchoolId = q.SchoolId,
                Name = q.Name,
                City = q.City,
                Area = q.Area,
                CityCode = q.CityCode,
                AreaCode = q.AreaCode,
                Cityarea = q.City + q.Area,
                Location = new GisPoint(q.Longitude, q.Latitude),
                Canteen = q.Canteen,
                Lodging = q.Lodging,
                Studentcount = q.Studentcount,
                Teachercount = q.Teachercount,
                Abroad = ConvertObject(q.Abroad),
                Authentication = ConvertObject(q.Authentication),
                Characteristic = ConvertObject(q.Characteristic),
                Courses = ConvertObject(q.Courses),
                MetroLineId = string.IsNullOrWhiteSpace(q.MetroLineId) ? new List<Guid>() : q.MetroLineId.Split(',').Select(Guid.Parse).ToList(),
                MetroStationId = string.IsNullOrWhiteSpace(q.MetroStationId) ? new List<int>() : q.MetroStationId.Split(',').Select(int.Parse).ToList(),
                Tags = string.IsNullOrWhiteSpace(q.TagName) ? new List<string>() : q.TagName.Split(',').ToList(),
                Tuition = q.Tuition,
                UpdateTime = q.UpdateTime,
                IsValid = q.IsValid,
                Status = q.Status,
                Grade = q.Grade,
                Score = q.Score22 != null ? new SchoolScoreData
                {
                    Eid = q.Id,
                    Score15 = q.Score15 ?? 0,
                    Score16 = q.Score16 ?? 0,
                    Score17 = q.Score17 ?? 0,
                    Score18 = q.Score18 ?? 0,
                    Score19 = q.Score19 ?? 0,
                    Score20 = q.Score20 ?? 0,
                    Score22 = q.Score22 ?? 0
                } : null,
                SchooltypeNewCode = q.SchFtype,
                SchooltypeCode = string.Format("{0}.{1}.{2}.{3}.{4}", q.Grade.ToString(), q.Type.ToString(), Convert.ToInt32(q.Discount).ToString(),
                             Convert.ToInt32(q.Diglossia).ToString(), Convert.ToInt32(q.Chinese).ToString()),
                Schooltype = gradeType.FirstOrDefault(p => p.GradeId == q.Grade)?.Types
                                .FirstOrDefault(p => p.GradeId == q.Grade && p.TypeId == q.Type
                                                && p.Discount == Convert.ToInt32(q.Discount)
                                                && p.Diglossia == Convert.ToInt32(q.Diglossia)
                                                && p.Chinese == Convert.ToInt32(q.Chinese))?.TypeName
            }).ToList();
        }

        public List<SchoolESDataDto> GetSchoolData(int pageNo, int pageSize, DateTime lastTime)
        {
            var data = _repo.GetSchoolData(pageNo, pageSize, lastTime);
            var gradeType = GetGradeTypeList();
            return data.Select(q => new SchoolESDataDto
            {
                Id = q.Id,
                SchoolId = q.SchoolId,
                Name = q.Name,
                SchoolName = q.SchoolName,
                ExtName = q.ExtName,
                City = q.City,
                Area = q.Area,
                CityCode = q.CityCode,
                AreaCode = q.AreaCode,
                Cityarea = q.City + q.Area,
                Location = new GisPoint(q.Longitude, q.Latitude),
                Canteen = q.Canteen,
                Lodging = q.Lodging,
                Studentcount = q.Studentcount,
                Teachercount = q.Teachercount,
                Abroad = ConvertObject(q.Abroad),
                Authentication = ConvertObject(q.Authentication),
                Characteristic = ConvertObject(q.Characteristic),
                Courses = ConvertObject(q.Courses),
                MetroLineId = string.IsNullOrWhiteSpace(q.MetroLineId) ? new List<Guid>() : q.MetroLineId.Split(',').Select(Guid.Parse).ToList(),
                MetroStationId = string.IsNullOrWhiteSpace(q.MetroStationId) ? new List<int>() : q.MetroStationId.Split(',').Select(int.Parse).ToList(),
                Tags = string.IsNullOrWhiteSpace(q.TagName) ? new List<string>() : q.TagName.Split(',').ToList(),
                Tuition = q.Tuition,
                UpdateTime = q.UpdateTime,
                IsValid = q.IsValid,
                Status = q.Status,
                Grade = q.Grade,
                Score = q.Score22 != null ? new SchoolScoreData
                {
                    Eid = q.Id,
                    Score15 = q.Score15 ?? 0,
                    Score16 = q.Score16 ?? 0,
                    Score17 = q.Score17 ?? 0,
                    Score18 = q.Score18 ?? 0,
                    Score19 = q.Score19 ?? 0,
                    Score20 = q.Score20 ?? 0,
                    Score21 = q.Score21 ?? 0,
                    Score22 = q.Score22 ?? 0
                } : null,
                SchooltypeNewCode = q.SchFtype,
                SchooltypeCode = string.Format("{0}.{1}.{2}.{3}.{4}", q.Grade.ToString(), q.Type.ToString(), Convert.ToInt32(q.Discount).ToString(),
                             Convert.ToInt32(q.Diglossia).ToString(), Convert.ToInt32(q.Chinese).ToString()),
                Schooltype = gradeType.FirstOrDefault(p => p.GradeId == q.Grade)?.Types
                                .FirstOrDefault(p => p.GradeId == q.Grade && p.TypeId == q.Type
                                                && p.Discount == Convert.ToInt32(q.Discount)
                                                && p.Diglossia == Convert.ToInt32(q.Diglossia)
                                                && p.Chinese == Convert.ToInt32(q.Chinese))?.TypeName
            }).ToList();
        }

        private List<Guid> ConvertObject(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new List<Guid>();
            var result = JsonConvert.DeserializeObject<List<KeyValue>>(json);
            return result.Select(q => Guid.Parse(q.Value)).ToList();
        }

        public SchoolFeedback SchoolFeedback(Guid eid)
        {
            return _repo.SchoolFeedback(eid);
        }

        public List<CommentsManage.Application.ModelDto.SchoolInfoDto> AllSchoolSelected(int cityCode, List<SchoolGrade> grade, List<SchoolType> type, List<int> isLodging, int pageIndex, int pageSize, QuestionAndCommentOrder order, List<int> schoolArea, bool queryType)
        {
            var school = _repo.AllSchoolSelected(cityCode, grade, type, isLodging, pageIndex, pageSize, order, schoolArea, queryType);
            if (school == null)
            {
                return null;
            }

            List<CommentsManage.Application.ModelDto.SchoolInfoDto> schoolInfoDtos = new List<CommentsManage.Application.ModelDto.SchoolInfoDto>();

            foreach (var item in school)
            {
                CommentsManage.Application.ModelDto.SchoolInfoDto schoolInfoDto = new CommentsManage.Application.ModelDto.SchoolInfoDto
                {
                    SchoolName = item.SchoolName,
                    CommentTotal = item.CommentTotal,
                    SectionCommentTotal = item.SectionCommentTotal,
                    SectionQuestionTotal = item.SectionQuestionTotal,
                    SchoolAvgScore = Math.Round(item.SchoolAvgScore / 20, 1),
                    SchoolStars = SchoolScoreToStart.GetCurrentSchoolstart(item.SchoolAvgScore),
                    SchoolType = (PMS.School.Domain.Common.SchoolType)item.SchoolType,
                    Lodging = item.Lodging,
                    Sdextern = item.Sdextern,
                    SchoolSectionId = item.Id,
                    SchoolId = item.SchoolId,
                    //上学帮认证，后续调用接口获取该学校是否已经成功认证
                    IsAuth = false,
                    SchoolNo = item.SchoolNo
                };
                schoolInfoDtos.Add(schoolInfoDto);
            }

            return schoolInfoDtos;
            //return school.Select(x=> new SchoolDto() {
            //    SchoolId = x.SchoolId,
            //    Id = x.Id,
            //    SchoolName = x.SchoolName,
            //    Lodging = x.Lodging,
            //    SchoolType = x.SchoolType,
            //    IsAuth = true
            //}).ToList();
        }

        public bool InsertEESchoolData(bool isFirst)
        {
            return _repo.InsertEESchoolData(isFirst);
        }
        public List<EESchool> GetEESchoolData(int pageIndex, int pageSize, DateTime time)
        {
            return _repo.GetEESchoolData(pageIndex, pageSize, time);
        }

        public bool InsertSchoolScore()
        {
            return _repo.InsertSchoolScore();
        }

        public List<SchoolScoreDataDto> GetSchoolScoreData(int pageNo, int pageSize)
        {
            var result = _repo.GetSchoolScoreData(pageNo, pageSize);
            return result.Select(q => new SchoolScoreDataDto
            {
                Eid = q.Eid,
                Score15 = q.Score15,
                Score16 = q.Score16,
                Score17 = q.Score17,
                Score18 = q.Score18,
                Score19 = q.Score19,
                Score20 = q.Score20,
                Score21 = q.Score21,
                Score22 = q.Score22,
            }).ToList();
        }

        public SchoolExtDto GetSchoolDetailById(Guid extId, double latitude = 0, double longitude = 0)
        {
            var data = _repo.GetSchoolExtDetails(extId);

            if (data == null)
            {
                return null;
            }

            var distance = _repo.GetDistance(extId, longitude, latitude);
            data.Distance = distance?.Value;
            var score = _repo.GetExtScore(extId);
            foreach (var item in score)
            {
                if (item.IndexId == 22)
                    data.ExtPoint = (int)item.Score;
            }

            //学校标签
            List<Tags> tags = GetSchoolExtTagsAsync(new SchoolExtItemDto { Authentication = data.Authentication, Courses = data.Courses, Characteristic = data.Characteristic, Sid = data.Sid, ExtId = data.ExtId }).Result;
            data.Tags = tags.Select(p => p.TagName).ToList();
            return data;
        }

        public SchoolExtAnyDto GetSchoolDetailByIdAny(Guid extId, double latitude = 0, double longitude = 0)
        {
            var data = _repo.GetSchoolExtDetailsAny(extId);

            if (data == null)
            {
                return null;
            }

            var distance = _repo.GetDistance(extId, longitude, latitude);
            data.Distance = distance?.Value;
            var score = _repo.GetExtScore(extId);
            foreach (var item in score)
            {
                if (item.IndexId == 22)
                    data.ExtPoint = (int)item.Score;
            }

            //学校标签
            List<Tags> tags = GetSchoolExtTagsAsync(new SchoolExtItemDto { Authentication = data.Authentication, Courses = data.Courses, Characteristic = data.Characteristic, Sid = data.Sid, ExtId = data.ExtId }).Result;
            data.Tags = tags.Select(p => p.TagName).ToList();
            return data;
        }


        /// <summary>
        /// 获取学校与当前定位位置相差的距离
        /// </summary>
        /// <param name="extIds"></param>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <returns></returns>
        public List<DisparityDto> GetSchoolDisparities(List<Guid> extIds, double longitude, double latitude)
        {
            return _repo.GetSchoolDisparities(extIds, longitude, latitude);
        }


        /// <summary>
        /// 人员管理平台 学校数据导入ES，按照学校录入时间排序
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="CreateTime">大于时间，默认值默认时间</param>
        /// <returns></returns>
        public List<SchoolSearchImport> SchoolSearchImport(int PageIndex, DateTime CreateTime = default)
        {
            return _repo.SchoolSearchImport(PageIndex, CreateTime);
        }

        public List<SchoolSearchImport> BDSchDataSearch(List<Guid> SchId)
        {
            return _repo.BDSchDataSearch(SchId);
        }

        class KeyValue
        {
            public string Key { get; set; }
            public string Value { get; set; }
            public string Message { get; set; }
        }

        public SchoolDto GetSchoolByNo(long no)
        {
            var schoolExt = _repo.GetSchoolByNo(no);
            if (schoolExt == null)
                return null;
            return new SchoolDto()
            {
                Id = schoolExt.Id,
                SchoolId = schoolExt.SchoolId,
                SchoolName = schoolExt.SchoolName,
            };
        }


        public async Task<IEnumerable<(long No, Guid ExtId)>> GetExtIdByNosAsync(IEnumerable<long> nos)
        {
            return await _repo.GetExtIdByNosAsync(nos);
        }

        /// <summary>
        /// 获取分部考试年份
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        public List<string> GetSchoolPastExamYears(string eid)
        {
            return _schoolYearFieldContentRepository.GetAllYears(eid, "Pastexam");
        }

        public string GetSchoolPastExam(string eid, string year)
        {
            var result = string.Empty;
            var find = _schoolYearFieldContentRepository.GenFieldContent(eid, "Pastexam", year);
            if (find?.Any() == true)
            {
                result = find.First().Content;
            }
            return result;
        }

        public List<string> GetSchoolFieldYears(string eid, string field)
        {
            return _schoolYearFieldContentRepository.GetAllYears(eid, field);
        }

        public string GetSchoolFieldContent(string eid, string field, string year)
        {
            var find = _schoolYearFieldContentRepository.GenFieldContent(eid, field, year);
            if (find?.Any() == true)
            {
                return find.First().Content;
            }
            return null;
        }

        public async Task<List<SchoolImageDto>> GetSchoolImages(Guid eid, SchoolImageType[] type)
        {
            if (eid != default)
            {
                var result = await _repo.GetSchoolImages(eid, type?.Select(p => (int)p).ToArray());
                return result;
            }
            return null;
        }

        public async Task<List<KeyValueDto<DateTime, string, byte, string, string>>> GetSchoolVideos(Guid extId, byte type = 0)
        {
            var data = await _repo.GetSchoolVideos(extId, type);
            return data;
        }

        /// <summary>
        /// 获取学部详情(包含图片)
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        public async Task<SchoolExtensionDto> GetSchoolExtensionDetails(Guid extId)
        {
            SchoolExtensionDto extension = CommonHelper.MapperProperty<SchoolExtDto, SchoolExtensionDto>(GetSchoolDetailById(extId));
            if (extension == null) return extension;

            extension.SchoolImages = await GetSchoolImages(extId, new SchoolImageType[] {
                //师资展示
                SchoolImageType.Principal,
                SchoolImageType.Teacher, 
                //校园展示
                SchoolImageType.Hardware,
                SchoolImageType.CommunityActivities,
            });

            var schext = await _schService.GetSchextSimpleInfo(extId);
            schext = schext ?? new SchExtDto0();
            var schtype = new SchFType0(schext.Grade, schext.Type, schext.Discount, schext.Diglossia, schext.Chinese);
            extension.SchFType = schtype;
            extension.TuitionPerYearFee = TuitionPerYearFee(extension.SchFType, extension.Tuition);

            var recruit = _repo.GetSchoolExtRecruit(extId);
            extension.Subjects = recruit.Subjects;

            extension.SchoolImageUrl = extension.HardwareImages.FirstOrDefault()?.Url ?? ConstantValue.DefaultSchoolHeadImg;

            CommonHelper.PropertyStringDef(extension, "暂未收录", "[]");
            return extension;
        }



        /// <summary>
        /// 获取学部详情(包含图片)
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        public async Task<SchoolExtensionAnyDto> GetSchoolExtensionDetailsAny(Guid extId)
        {
            SchoolExtensionAnyDto extension = CommonHelper.MapperProperty<SchoolExtAnyDto, SchoolExtensionAnyDto>(GetSchoolDetailByIdAny(extId));
            if (extension == null) return extension;

            extension.SchoolImages = await GetSchoolImages(extId, new SchoolImageType[] {
                //师资展示
                SchoolImageType.Principal,
                SchoolImageType.Teacher, 
                //校园展示
                SchoolImageType.Hardware,
                SchoolImageType.CommunityActivities,
            });

            var schext = await _schService.GetSchextSimpleInfo(extId);
            schext = schext ?? new SchExtDto0();
            var schtype = new SchFType0(schext.Grade, schext.Type, schext.Discount, schext.Diglossia, schext.Chinese);
            extension.SchFType = schtype;
            extension.TuitionPerYearFee = TuitionPerYearFee(extension.SchFType, extension.Tuition);

            var recruit = _repo.GetSchoolExtRecruitAny(extId);
            extension.Subjects = recruit.Subjects;

            extension.SchoolImageUrl = extension.HardwareImages.FirstOrDefault()?.Url ?? ConstantValue.DefaultSchoolHeadImg;

            CommonHelper.PropertyStringDef(extension, "暂未收录", "[]");
            return extension;
        }



        /// <summary>
        /// 该方法衍生自【GetSchoolExtensionDetails】，但是没有为空做“暂未收录默认值”
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        public async Task<SchoolExtensionDto> GetSchoolExtensionDetailsNoZanWeiShouLu(Guid extId)
        {
            SchoolExtensionDto extension = CommonHelper.MapperProperty<SchoolExtDto, SchoolExtensionDto>(GetSchoolDetailById(extId));
            if (extension == null) return extension;

            extension.SchoolImages = await GetSchoolImages(extId, new SchoolImageType[] {
                //师资展示
                SchoolImageType.Principal,
                SchoolImageType.Teacher, 
                //校园展示
                SchoolImageType.Hardware,
                SchoolImageType.CommunityActivities,
            });

            var schext = await _schService.GetSchextSimpleInfo(extId);
            schext = schext ?? new SchExtDto0();
            var schtype = new SchFType0(schext.Grade, schext.Type, schext.Discount, schext.Diglossia, schext.Chinese);
            extension.SchFType = schtype;
            extension.TuitionPerYearFee = TuitionPerYearFee(extension.SchFType, extension.Tuition);

            var recruit = _repo.GetSchoolExtRecruit(extId);
            extension.Subjects = recruit.Subjects;

            extension.SchoolImageUrl = extension.HardwareImages.FirstOrDefault()?.Url ?? ConstantValue.DefaultSchoolHeadImg;
            return extension;
        }



        public string TuitionPerYearFee(SchFType0 schtype, double? tuition)
        {
            bool In<T>(T item, params T[] collection)
            {
                return collection.Contains(item);
            }

            string tuitionPerYearFee;
            if (!SchTypeUtils.Canshow2("学费", schtype))
            {
                tuitionPerYearFee = schtype.Type == (byte)SchoolType.Public
                    && In((SchoolGrade)schtype.Grade, SchoolGrade.PrimarySchool, SchoolGrade.JuniorMiddleSchool)
                    ? "义务教育免学费" : "--";
            }
            else
            {
                tuitionPerYearFee = tuition.ToPerYearFee("暂未收录");
            }
            return tuitionPerYearFee;
        }
        public async Task<string> TuitionPerYearFee(Guid extId, double? tuition)
        {
            var schext = await _schService.GetSchextSimpleInfo(extId);
            schext = schext ?? new SchExtDto0();
            var schtype = new SchFType0(schext.Grade, schext.Type, schext.Discount, schext.Diglossia, schext.Chinese);
            return TuitionPerYearFee(schtype, tuition);
        }

        public async Task<(List<SchoolExtFilterDto> data, long total)> SearchSchools(SearchSchoolQuery query, bool onlyAuthTag = false)
        {
            //es data
            var searchResult = _searchService.SearchSchool(query);
            var ids = searchResult.Schools.Select(q => q.Id).ToList();
            var data = await SearchSchools(ids, query.Latitude, query.Longitude, readIntro: false, onlyAuthTag);

            await SetIncrementSearchSchoolCost(query.MinCost, query.MaxCost);
            return (data, searchResult.Total);
        }

        public async Task<(List<SchoolExtFilterDto> data, long total)> SearchSchoolsFullMatch(SearchBaseQueryModel queryModel, double latitude = 0, double longitude = 0)
        {
            //es data
            var searchResult = await _searchService.SearchSchoolsFullMatch(queryModel);
            var ids = searchResult.Data;
            var data = await SearchSchools(ids, latitude, longitude);
            return (data, searchResult.Total);
        }

        public async Task<List<SchoolExtFilterDto>> GetRecommendSchools(string keyword, int cityId
            , List<int> areaIds
            , List<int> grades, List<string> schoolTypeCodes //学校类型(学段+类型)
            , int? minTotal, int? maxTotal //学校评分
            , List<Guid> authIds
            , List<Guid> courseIds
            , List<Guid> characIds
            , int pageIndex = 1, int pageSize = 10)
        {
            var ids = await _searchService.RecommenSchoolIdsAsync(keyword, cityId, areaIds, grades, schoolTypeCodes, minTotal, maxTotal, authIds, courseIds, characIds, pageIndex, pageSize);
            var data = await SearchSchools(ids, 0, 0, readIntro: false, onlyAuthTag: false);
            return data;
        }

        /// <summary>
        /// 设置搜索学费查询次数
        /// </summary>
        /// <param name="minCost"></param>
        /// <param name="maxCost"></param>
        /// <returns></returns>
        private async Task SetIncrementSearchSchoolCost(int? minCost, int? maxCost)
        {
            if (minCost == null && maxCost == null)
            {
                return;
            }

            var key = RedisKeys.SchoolCostSearch;
            var member = CostSectionMemberDto.Create(minCost, maxCost);

            if (member.Min >= member.Max)
            {
                return;
            }
            //限定取值范围
            if (!CostSectionDto.Default.Any(s => s.Min == member.Min && s.Max == member.Max))
            {
                return;
            }
            await _easyRedisClient.SortedSetIncrementAsync(key, member, 1, StackExchange.Redis.CommandFlags.FireAndForget);
        }

        public async Task<IEnumerable<CostSectionDto>> GetSearchSchoolCostAsync()
        {
            var key = RedisKeys.SchoolCostSearch;
            var data = await _easyRedisClient.SortedSetRangeByRankWithScoresAsync<CostSectionMemberDto>(key, 0, 3);

            if (data.Keys.Count < 3)
            {
                return CostSectionDto.Default;
            }


            var total = data.Sum(s => s.Value);
            total = total == 0 ? 1 : total;

            return data.Select(s => new CostSectionDto()
            {
                Min = s.Key.Min,
                Max = s.Key.Max,
                //Score = s.Value,
                Ratio = s.Value / total
            }).OrderBy(s => s.Min);
        }

        public async Task<List<SchoolExtFilterDto>> SearchSchools(IEnumerable<Guid> ids, double latitude = 0, double longitude = 0, bool readIntro = false, bool onlyAuthTag = false)
        {
            //db data
            var result = ids.Any() ? await ListExtSchoolByBranchIds(ids.ToList(), latitude, longitude, readIntro, onlyAuthTag) : new List<SchoolExtFilterDto>();

            //使用原有序列, 避免排序差异
            var data = ids
                .Where(q => result.Any(p => p.ExtId == q))
                .Select(q =>
                {
                    var extension = result.First(p => p.ExtId == q);
                    extension.TuitionPerYearFee = TuitionPerYearFee(q, extension.Tuition).GetAwaiter().GetResult();

                    return extension;
                })
                .ToList();

            return data;
        }

        public string GetCounterPartByYear(int? year = null)
        {
            return null;
        }
        public async Task<GDParams> GetSurroundInfo(Guid eid)
        {
            if (eid == Guid.Empty) return null;
            return await _schoolMongoRepository.GetGDParamsByEID(eid);
        }

        public async Task<GDParams> GetSurroundAvgInfo(string schFtype, int areaCode)
        {
            if (string.IsNullOrWhiteSpace(schFtype) || areaCode < 1) return null;
            var str_RedisKey = $"Mongo:GDParamsAvg:AreaCode_{areaCode}:SchFType_{schFtype}";
            return await _easyRedisClient.GetOrAddAsync(str_RedisKey, () =>
            {
                return _schoolMongoRepository.GetGDParamsAvg(schFtype, areaCode);
            }, TimeSpan.FromDays(1));
        }

        public async Task<AiParams> GetAiParams(Guid eid)
        {
            if (eid == Guid.Empty) return null;
            return await _schoolMongoRepository.GetAiParamsByEID(eid);
        }

        public async Task<AiParams> GetAiParamsAvg(string schFtype, int areaCode)
        {
            if (string.IsNullOrWhiteSpace(schFtype) || areaCode < 1) return null;
            var str_RedisKey = $"Mongo:AiParamsAvg:AreaCode_{areaCode}:SchFType_{schFtype}";
            return await _easyRedisClient.GetOrAddAsync(str_RedisKey, () =>
            {
                return _schoolMongoRepository.GetAiParamsAvg(schFtype, areaCode);
            }, TimeSpan.FromDays(1));
        }

        public async Task<IEnumerable<KeyValuePair<string, Guid>>> GetCounterPartByEID(Guid eid)
        {
            if (eid == Guid.Empty) return null;
            var find = await _repo.GetCounterPartByEID(eid);
            if (!string.IsNullOrWhiteSpace(find) || find != "[]")
            {
                try
                {
                    return JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, Guid>>>(find);
                }
                catch
                {
                }
            }
            return null;
        }

        public async Task<IEnumerable<KeyValuePair<Guid, SchoolGrade>>> GetGradesByEIDs(IEnumerable<Guid> eids)
        {
            if (eids?.Any() == true)
            {
                return await _repo.GetGrades(eids);
            }
            return null;
        }

        public async Task<IEnumerable<KeyValuePair<Guid, (int, int)>>> GetCityAndAreaByEIDs(IEnumerable<Guid> eids)
        {
            if (eids?.Any() == true)
            {
                return await _repo.GetCityAndAreaByEIDs(eids);
            }
            return null;
        }

        public List<SchoolExtension> GetCurrentSchoolAllExt(Guid sid)
        {
            return _repo.GetCurrentSchoolAllExt(sid);
        }

        public async Task<IEnumerable<WeChatSouYiSouSchool>> GetWeChatSouYiSouSchools(List<Guid> extIds)
        {
            return await _weChatSouYiSouSchoolRepository.GetByEIds(extIds);

        }

        public async Task<IEnumerable<string>> GetTagTypes(Guid eid)
        {
            return await _repo.GetTagTypes(eid);
        }

        public async Task<IEnumerable<(Guid ExtId, string SchoolName)>> GetSchoolNameOnlyAsync(IEnumerable<Guid> extIds)
        {
            return await _repo.GetSchoolNameOnlyAsync(extIds);
        }

        public async Task<IEnumerable<SchoolNameTypeArea>> GetSchoolNameTypeAreaAsync(IEnumerable<Guid> extIds)
        {
            return await _repo.GetSchoolNameTypeAreaAsync(extIds);
        }
    }
}