using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Domain.Common;
using PMS.OperationPlateform.Application.Dtos;
using PMS.OperationPlateform.Application.IServices;
using PMS.PaidQA.Application.Services;
using PMS.PaidQA.Domain.Entities;
using PMS.School.Application.IServices;
using PMS.School.Domain.Dtos;
using PMS.Search.Application.IServices;
using PMS.Search.Domain.Common;
using PMS.UserManage.Application.IServices;
using ProductManagement.API.Aliyun;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using Sxb.PCWeb.Areas.School.Models;
using Sxb.PCWeb.Common;
using Sxb.PCWeb.Controllers;
using Sxb.PCWeb.Models;
using Sxb.PCWeb.Response;
using Sxb.PCWeb.Utils;
using Sxb.PCWeb.Utils.CommentHelper;
using Sxb.PCWeb.ViewModels.ViewComponent;
using static PMS.Search.Domain.Common.GlobalEnum;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace Sxb.PCWeb.Areas.School
{
    [ApiController]
    [Route("api/[controller]")]
    public class AggHomeController : ApiBaseController
    {
        private readonly ImageSetting _imageSetting;
        private readonly IOrderService _orderService;
        private readonly IHotTypeService _hotTypeService;
        private readonly ITalentSettingService _talentSettingService;
        private readonly PMS.PaidQA.Application.Services.IMessageService _messageService;
        private readonly IConfiguration _configuration;

        private readonly IHotPopularService _hotPopularService;
        private readonly ISchService _schService;
        private readonly ISchoolCommentService _commentService;
        private readonly IQuestionInfoService _questionInfoService;
        //热门点评、学校组合方法
        private readonly PullHottestCSHelper _hot;
        private readonly string _apiUrl;
        private readonly IText _text;
        private readonly ISearchService _searchService;
        private readonly ISchoolService _schoolService;
        private readonly IArticleService _articleService;
        private readonly IWikiService _wikiService;
        private readonly IHistoryService _historyService;
        private readonly IEasyRedisClient _easyRedisClient;

        public AggHomeController(IOptions<ImageSetting> options, IOrderService orderService, IHotTypeService hotTypeService, ITalentSettingService talentSettingService,
            PMS.PaidQA.Application.Services.IMessageService messageService, IConfiguration configuration, IHotPopularService hotPopularService, ISchService schService, ISchoolCommentService commentService,
            IQuestionInfoService questionInfoService, IGiveLikeService likeservice, IUserService userService, ISchoolInfoService schoolInfoService, IText text, ISearchService searchService, ISchoolService schoolService, IArticleService articleService, IWikiService wikiService, IHistoryService historyService, IEasyRedisClient easyRedisClient)
        {
            _imageSetting = options.Value;
            _orderService = orderService;
            _hotTypeService = hotTypeService;
            _talentSettingService = talentSettingService;
            _messageService = messageService;
            _configuration = configuration;
            _hotPopularService = hotPopularService;
            _schService = schService;
            _commentService = commentService;
            _questionInfoService = questionInfoService;

            _hot = new PullHottestCSHelper(likeservice, userService, schoolInfoService, _imageSetting, schService);

            _configuration = configuration;
            if (_configuration != null)
            {
                var baseUrl = _configuration.GetSection("ExternalInterface").GetValue<string>("OrganizationAddress");
                if (!string.IsNullOrWhiteSpace(baseUrl))
                {
                    _apiUrl = $"{baseUrl}api/ToSchools/hotsell/coursesandorgs?minage={{0}}&maxage={{1}}";
                }
            }
            _text = text;
            _searchService = searchService;
            _schoolService = schoolService;
            _articleService = articleService;
            _wikiService = wikiService;
            _historyService = historyService;
            _easyRedisClient = easyRedisClient;
        }

        /// <summary>
        /// 推荐达人 - 升学顾问
        /// Copy from PaidQAViewComponent
        /// <see cref="ViewComponents.PaidQAViewComponent"/>
        /// </summary>
        /// <param name="grade">1:幼儿园 2:小学 3:初中 4:高中</param>
        /// <returns></returns>
        [HttpGet("RecommendPaidQAs")]
        public ResponseResult GetRecommendPaidQAs(byte grade)
        {
            var gradeType = 0;
            var random = new Random();
            var result = new List<PaidQAViewModel>();
            switch (grade)
            {
                case 1:
                    gradeType = 1;
                    break;
                case 2:
                    gradeType = random.Next(2, 4);
                    break;
                case 3:
                    gradeType = 4;
                    break;
                case 4:
                    gradeType = 5;
                    break;
                default:
                    break;
            }
            if (gradeType < 1) return ResponseResult.Success(result);

            var ids = _hotTypeService.GetRandomOrderIDByGrade(gradeType);
            if (ids.Result?.Any() == true)
            {
                var orders = _orderService.ListByIDs(ids.Result);
                if (orders.Result?.Any() == true)
                {
                    var userInfos = _talentSettingService.GetDetails(orders.Result.Select(p => p.AnswerID).Distinct());
                    var contents = _messageService.GetOrdersQuetion(orders.Result.Select(p => p.ID).Distinct());
                    foreach (var order in orders.Result)
                    {
                        var item = new PaidQAViewModel();
                        var userinfo = userInfos.Result.FirstOrDefault(p => p.TalentUserID == order.AnswerID);
                        if (userinfo != null)
                        {
                            item.NickName = userinfo.NickName;
                            item.HeadImgUrl = userinfo.HeadImgUrl;
                            item.RegionNames = userinfo.TalentRegions?.Select(p => p.Name);
                            item.TalentUserID = userinfo.TalentUserID;
                        }
                        var content = contents.Result.FirstOrDefault(p => p.OrderID == order.ID);
                        if (content != null)
                        {

                            if (!string.IsNullOrWhiteSpace(content.Content))
                            {
                                try
                                {
                                    var messageContent = Newtonsoft.Json.JsonConvert.DeserializeObject<PMS.PaidQA.Domain.Message.TXTMessage>(content.Content);
                                    if (messageContent != null) item.Content = messageContent.Content;
                                }
                                catch
                                {
                                }
                            }

                        }
                        result.Add(item);
                    }
                }
            }
            return ResponseResult.Success(result);
        }


        /// <summary>
        /// 推荐机构和课程
        /// Copy from PaidQAViewComponent
        /// <see cref="ViewComponents.OrgLessonViewComponent"/>
        /// </summary>
        /// <param name="grade">1:幼儿园 2:小学 3:初中 4:高中</param>
        /// <returns></returns>
        [HttpGet("RecommendOrgsAndCourses")]
        public ResponseResult GetRecommendOrgsAndCourses(byte grade)
        {
            var ageMin = 3;
            var ageMax = 6;
            switch (grade)
            {
                case 2:
                    ageMin = 6;
                    ageMax = 12;
                    break;
                case 3:
                    ageMin = 12;
                    ageMax = 15;
                    break;
                case 4:
                    //ageMin = 15;
                    //ageMax = 18;
                    ageMin = 0;
                    ageMax = 3;
                    break;
            }
            try
            {
                var webClient = new WebUtils();
                var apiResponse = webClient.DoGet(string.Format(_apiUrl, ageMin, ageMax));
                if (!string.IsNullOrWhiteSpace(apiResponse))
                {

                    var object_Response = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseResult<OrgLessonViewModel>>(apiResponse);

                    if (object_Response.Succeed)
                    {
                        var result2 = Newtonsoft.Json.JsonConvert.DeserializeObject<OrgLessonViewModel>(Newtonsoft.Json.JsonConvert.SerializeObject(object_Response.Data));
                        return ResponseResult.Success(result2);
                    }
                }
            }
            catch
            {
            }

            var result = new OrgLessonViewModel()
            {
                HotSellCourses = new List<HotSellCourse>(),
                RecommendOrgs = new List<RecommendOrg>()
            };
            return ResponseResult.Success(result);
        }


        /// <summary>
        /// 推荐学校, 以下四种类型
        /// 周边学校
        /// 热门学校
        /// 热评学校
        /// 热问学校
        /// Copy from RelatedSchoolViewComponent
        /// <see cref="ViewComponents.RelatedSchoolViewComponent"/>
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        [HttpGet("RecommenRelatedSchools")]
        public async Task<ResponseResult> GetRecommenRelatedSchools(Guid? eid = null)
        {
            var key  =string.Format(RedisKeys.AggRecommendKey, eid);
            var result = await _easyRedisClient.GetOrAddAsync(key, () => {
                return GetRelatedSchools(eid);
            }, TimeSpan.FromHours(13));
            return ResponseResult.Success(result);
        }

        public RelatedSchoolViewModel GetRelatedSchools(Guid? eid = null) {

            var result = new RelatedSchoolViewModel();

            var date_Now = DateTime.Now;

            result.NearSchool = GetNearSchool(eid).Result;

            result.HotSchools = GetHotSchool(eid).Result;
            //热门学校点评|热问学校
            result.HotCommentSchools = _hot.HottestSchool(_commentService.HottestSchool(new HotCommentQuery()
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
            }, false).Result);

            //热问学校【侧边栏】
            result.HotQuestionSchools = _hot.HottestQuestionItem(_questionInfoService.HottestSchool().Result);
            return result;
        }

        /// <summary>
        /// 获取周边学校
        /// Copy from RelatedSchoolViewComponent
        /// <see cref="ViewComponents.RelatedSchoolViewComponent"/>
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        async Task<IEnumerable<SchExtDto0>> GetNearSchool(Guid? eid = null)
        {
            int randomCount = 16;
            IEnumerable<SchExtDto0> finds;
            if (eid.HasValue && eid.Value != Guid.Empty)
            {
                finds = await _schService.GetNearSchoolsByEID(eid.Value, randomCount);
            }
            else
            {
                double latitude = NumberParse.DoubleParse(Request.Cookies["latitude"], 0);
                double longitude = NumberParse.DoubleParse(Request.Cookies["longitude"], 0);
                finds = await _schService.GetNearSchoolsBySchType((longitude, latitude), null, randomCount);
            }
            if (finds?.Any() == true)
            {
                finds = CommonHelper.ListRandom(finds);
                return finds.Take(6);
            }
            return null;
        }

        /// <summary>
        /// 获取热门学校
        /// Copy from RelatedSchoolViewComponent
        /// <see cref="ViewComponents.RelatedSchoolViewComponent"/>
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        async Task<IEnumerable<SimpleHotSchoolDto>> GetHotSchool(Guid? eid = null)
        {
            var userCitycode = Request.GetLocalCity();
            iSchool.SchFType0[] schtypes = null;

            var result = await _hotPopularService.GetHotVisitSchools(userCitycode, schtypes, 50);
            if (result?.Length > 0)
            {
                var list_Result = result.ToList();
                CommonHelper.ListRandom(list_Result);
                result = list_Result.Take(6).OrderByDescending(p => p.TotalScore).ToArray();

                var eids = result.Select(p => p.Eid).ToArray();
                var nos = _schService.GetSchoolextNo(eids);
                if (nos?.Length > 0)
                {
                    foreach (var no in nos)
                    {
                        if (result.Any(p => p.Eid == no.Item1)) result.FirstOrDefault(p => p.Eid == no.Item1).SchoolNo = no.Item2;
                    }
                    //ViewBag.SchoolNos = nos.Result.ToDictionary(k => k.Item1, v => UrlShortIdUtil.Long2Base32(v.Item2));
                }
            }
            return result;
        }
    }
}

