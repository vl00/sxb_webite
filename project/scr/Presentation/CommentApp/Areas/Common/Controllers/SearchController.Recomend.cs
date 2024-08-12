using Microsoft.AspNetCore.Mvc;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PMS.Search.Domain.QueryModel;
using ProductManagement.API.Http.Model.StaticInside;
using ProductManagement.Framework.Foundation;
using Sxb.Web.Common;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PMS.Search.Domain.Common.GlobalEnum;
using static ProductManagement.API.Http.Service.StaticInsideClient;

namespace Sxb.Web.Areas.Common.Controllers
{
    public partial class SearchController
    {


        //[HttpGet("Questions/Total")]
        //public async Task<ResponseResult> QuestionsTotal(int cityId)
        //{
        //    return ResponseResult.Success(new
        //    {
        //        RecommendNo = Guid.NewGuid(),
        //        Total = 300,
        //    });
        //}
        //[HttpGet("Questions/Nos")]
        //public async Task<ResponseResult> Questions(string recommendNo, int[] nos)
        //{
        //    return ResponseResult.Success();
        //}
        private DateTime startTime => DateTime.Now.AddDays(-7);
        private DateTime endTime => DateTime.Now;
        public async Task<ResponseResult> GetRecommendsDefault(int channel, int cityId = 0, int pageIndex = 1, int pageSize = 120)
        {
            ChannelIndex channelIndex = GetChannelIndex(channel);
            cityId = cityId == 0 ? Request.GetAvaliableCity() : cityId;

            switch (channelIndex)
            {
                case ChannelIndex.All:
                case ChannelIndex.School:
                    return await Schools(cityId, pageIndex, pageSize);
                case ChannelIndex.Comment:
                    return await Comments(cityId, pageIndex, pageSize);
                case ChannelIndex.Question:
                    return await Questions(cityId, pageIndex, pageSize);
                case ChannelIndex.SchoolRank:
                    return await SchoolRanks(cityId, pageIndex, pageSize);
                case ChannelIndex.Article:
                    return await Articles(cityId, pageIndex, pageSize);
                case ChannelIndex.Live:
                    return await Lives(cityId, pageIndex, pageSize);
                case ChannelIndex.Circle:
                    return await Circles(pageIndex, pageSize);
                case ChannelIndex.Talent:
                    return await Talents(cityId, pageIndex, pageSize);
                case ChannelIndex.SvsSchool:
                    break;
                case ChannelIndex.Org:
                    break;
                case ChannelIndex.OrgEvaluation:
                    return await Evelations(pageIndex, pageSize);
                case ChannelIndex.Course:
                    return await Courses(pageIndex, pageSize);
                default:
                    break;
            }
            return ResponseResult.Failed();
        }


        public StaticInsideType? ChannelToStaticInsideType(ChannelIndex channelIndex)
        {
            switch (channelIndex)
            {
                case ChannelIndex.All:
                case ChannelIndex.School:
                    return StaticInsideType.School;
                case ChannelIndex.Comment:
                    return StaticInsideType.Comment;
                case ChannelIndex.Question:
                    return StaticInsideType.Question;
                case ChannelIndex.SchoolRank:
                    return StaticInsideType.SchoolRank;
                case ChannelIndex.Article:
                    return StaticInsideType.Article;
                case ChannelIndex.Live:
                    return StaticInsideType.Live;
                case ChannelIndex.Circle:
                    return StaticInsideType.Circle;
                case ChannelIndex.Talent:
                    return StaticInsideType.Talent;
                case ChannelIndex.SvsSchool:
                    break;
                case ChannelIndex.Org:
                    break;
                case ChannelIndex.OrgEvaluation:
                    return StaticInsideType.Evaluation;
                case ChannelIndex.Course:
                    return StaticInsideType.Course;
                default:
                    break;
            }
            return null;
        }

        /// <summary>
        /// 获取uv推荐数据
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="cityId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public async Task<ResponseResult> GetRecommends(int channel, int cityId = 0, int pageIndex = 1, int pageSize = 120)
        {
            ChannelIndex channelIndex = GetChannelIndex(channel);
            cityId = cityId == 0 ? Request.GetAvaliableCity() : cityId;

            var staticInsideType = ChannelToStaticInsideType(channelIndex) ?? throw new NotSupportedException("不支持的Channel推荐类型");
            var key = string.Format(RedisKeys.DataUvRecommendKey, staticInsideType, cityId, pageIndex, pageSize);

            //30分钟缓存 (存入redis后, 不定义取出类型, object不会自动序列)
            var dataStr = await _easyRedisClient.GetOrAddAsync(key, async () =>
            {
                IEnumerable<Guid> ids = await GetRecommendIds(staticInsideType, cityId, pageIndex, pageSize);
                var cData = await GetChannelData(channelIndex, ids);
                return JsonConvert.SerializeObject(cData.ToList(), new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
            }, TimeSpan.FromMinutes(30));

            var data = JsonConvert.DeserializeObject<IEnumerable<dynamic>>(dataStr);
            //for test StatisticsLog是正式环境的id,匹配不到数据
            if (!data.Any())
            {
                return await GetRecommendsDefault(channel, cityId, pageIndex, pageSize);
            }

            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 根据channel的ids换取对应数据
        /// </summary>
        /// <param name="channelIndex"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        private async Task<IEnumerable<dynamic>> GetChannelData(ChannelIndex channelIndex, IEnumerable<Guid> ids)
        {
            IEnumerable<dynamic> data = null;
            switch (channelIndex)
            {
                case ChannelIndex.All:
                case ChannelIndex.School:
                    data = await _schoolService.SearchSchools(ids);
                    break;
                case ChannelIndex.Comment:
                    data = _questionService.SearchQuestions(ids, UserIdOrDefault);
                    break;
                case ChannelIndex.Question:
                    data = _commentService.SearchComments(ids, UserIdOrDefault);
                    break;
                case ChannelIndex.SchoolRank:
                    data = _schoolRankService.GetSchoolRanks(ids).Select(s => new
                    {
                        Id = s.Id,
                        RankName = s.Title
                    });
                    break;
                case ChannelIndex.Article:
                    data = HandleArticle(_articleService.SearchArticles(ids));
                    break;
                case ChannelIndex.Live:
                    data = await HandleLive(ids);
                    break;
                case ChannelIndex.Circle:
                    data = HandleCircle(_circleService.SearchCircles(ids, UserId));
                    break;
                case ChannelIndex.Talent:
                    data = _talentService.SearchTalents(ids, UserId);
                    break;
                case ChannelIndex.SvsSchool:
                    break;
                case ChannelIndex.Org:
                    break;
                case ChannelIndex.OrgEvaluation:
                    data = await _orgServiceClient.GetEvaluations(ids);
                    break;
                case ChannelIndex.Course:
                    data = await _orgServiceClient.GetCourses(ids);
                    break;
                default:
                    break;
            }

            return data;
        }

        [HttpGet("Schools")]
        public async Task<ResponseResult> Schools(int cityId, int pageIndex = 1, int pageSize = 10)
        {
            var ids = await GetRecommendIds(StaticInsideType.School, cityId, pageIndex, pageSize);
            var data = _schoolService.SearchSchools(ids);
            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 获取redis缓存中的uv数据
        /// </summary>
        /// <param name="staticInsideType"></param>
        /// <param name="cityId"></param>
        /// <returns></returns>
        private async Task<IEnumerable<Guid>> GetRecommendIds(StaticInsideType staticInsideType, int cityId, int pageIndex = 1, int pageSize = 10)
        {
            var key = string.Format(RedisKeys.DataUvKey, staticInsideType, cityId);

            var dataUvs = await _easyRedisClient.GetAsync<IEnumerable<StaticDataUV>>(key);
            if (dataUvs?.Any() == true)
            {
                int skip = (pageIndex - 1) * pageSize;
                return dataUvs.Skip(skip).Take(pageSize).Select(s => s.DataId);
            }
            return Enumerable.Empty<Guid>();
        }

        /// <summary>
        /// 定时缓存数据到redis
        /// </summary>
        /// <param name="staticInsideType"></param>
        /// <returns></returns>
        public async Task<ResponseResult> SyncRecommendIds([FromQuery] List<StaticInsideType> staticInsideTypes)
        {
            //null is all
            if (!(staticInsideTypes?.Any() == true))
            {
                var type = typeof(StaticInsideType);
                staticInsideTypes = Enum.GetValues(type).Cast<StaticInsideType>().ToList();
            }

            var size = 1000;
            var cities = await _cityInfoService.GetAllCityCodes();
            foreach (var staticInsideType in staticInsideTypes)
            {
                //七日全国uv
                var allCityDataUvs = await _staticInsideClient.GetDatUv(staticInsideType, 0, 7);
                //top七日全国uv
                var topAllCityDataUvs = allCityDataUvs.OrderByDescending(s => s.Uv).Take(size);
                foreach (var city in cities)
                {
                    var cityId = city.Id;
                    var key = string.Format(RedisKeys.DataUvKey, staticInsideType, cityId);
                    //七日本城uv
                    var dataUvs = await _staticInsideClient.GetDatUv(staticInsideType, cityId, 7);
                    //top七日本城数据不足, 取全国
                    var topDataUvs = dataUvs.Any() ? dataUvs.OrderByDescending(s => s.Uv).Take(size) : topAllCityDataUvs;
                    await _easyRedisClient.AddAsync(key, topDataUvs, TimeSpan.FromHours(25));
                }
            }
            return ResponseResult.Success();
        }

        [HttpGet("Comments")]
        public async Task<ResponseResult> Comments(int cityId, int pageIndex = 1, int pageSize = 10)
        {
            var data = (await SearchComments(new Models.RequestSearchQuery() { PageNo = pageIndex, PageSize = pageSize })).Data;
            data = ((dynamic)data).Data;
            return ResponseResult.Success(data);
        }

        [HttpGet("Questions")]
        public async Task<ResponseResult> Questions(int cityId, int pageIndex = 1, int pageSize = 10)
        {
            var data = (await SearchQuestions(new Models.RequestSearchQuery() { PageNo = pageIndex, PageSize = pageSize })).Data;
            data = ((dynamic)data).Data;
            return ResponseResult.Success(data);
        }


        [HttpGet("SchoolRanks")]
        public async Task<ResponseResult> SchoolRanks(int cityId, int pageIndex = 1, int pageSize = 10)
        {
            object data = await SearchSchoolRanks(new SearchBaseQueryModel("", pageIndex, pageSize));
            data = ((dynamic)data).Data.Data;
            return ResponseResult.Success(data);
        }


        [HttpGet("Articles")]
        public async Task<ResponseResult> Articles(int cityId, int pageIndex = 1, int pageSize = 10)
        {
            object data = await SearchArticles(new SearchArticleQueryModel("", pageIndex, pageSize));
            data = ((dynamic)data).Data.Data;
            return ResponseResult.Success(data);
        }

        [HttpGet("Lives")]
        public async Task<ResponseResult> Lives(int cityId, int pageIndex = 1, int pageSize = 10)
        {
            object data = await SearchLives(new SearchLiveQueryModel("", pageIndex, pageSize));
            data = ((dynamic)data).Data.Data;
            return ResponseResult.Success(data);
        }


        [HttpGet("Talents")]
        public async Task<ResponseResult> Talents(int cityId, int pageIndex = 1, int pageSize = 10)
        {
            object data = await SearchTalents(new SearchBaseQueryModel("", pageIndex, pageSize));
            data = ((dynamic)data).Data.Data;
            return ResponseResult.Success(data);
        }


        [HttpGet("Circles")]
        public async Task<ResponseResult> Circles(int pageIndex = 1, int pageSize = 10)
        {
            object data = await SearchCircles(new SearchBaseQueryModel("", pageIndex, pageSize));
            data = ((dynamic)data).Data.Data;
            return ResponseResult.Success(data);
        }

        [HttpGet("Courses")]
        public async Task<ResponseResult> Courses(int pageIndex = 1, int pageSize = 10)
        {
            object data = await SearchCoursesPagination(new SearchCourseQueryModel("", pageIndex, pageSize));
            data = ((dynamic)data).Data.Data;
            return ResponseResult.Success(data);
        }

        [HttpGet("Evelations")]
        public async Task<ResponseResult> Evelations(int pageIndex = 1, int pageSize = 10)
        {
            object data = await SearchEvaluationsPagination(new SearchEvaluationQueryModel("", pageIndex, pageSize));
            data = ((dynamic)data).Data.Data;
            return ResponseResult.Success(data);
        }


    }
}
