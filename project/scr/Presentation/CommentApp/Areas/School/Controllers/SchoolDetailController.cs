using AutoMapper;
using iSchool;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PMS.CommentsManage.Application.IServices;
using PMS.OperationPlateform.Application.IServices;
using PMS.PaidQA.Application.Services;
using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.EntityExtend;
using PMS.School.Application.IServices;
using PMS.School.Domain.Common;
using PMS.School.Domain.Entities.WechatDemo;
using PMS.School.Domain.Enum;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model.Org;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using Sxb.Web.Areas.School.Models.SchoolDetail;
using Sxb.Web.Common;
using Sxb.Web.Controllers;
using Sxb.Web.Response;
using Sxb.Web.ViewModels.Article;
using Sxb.Web.ViewModels.ViewComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductManagement.Framework.Foundation;
using PMS.PaidQA.Domain.EntityExtend;
using PMS.School.Domain.Dtos;
using PMS.School.Infrastructure.Common;
using Newtonsoft.Json;
using PMS.School.Domain.Enum;
using PMS.School.Domain.Attributes;
using Newtonsoft.Json.Linq;
using ProductManagement.API.Http.Interface;
using PMS.Infrastructure.Application.IService;

namespace Sxb.Web.Areas.School.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SchoolDetailController : ApiBaseController
    {
        ISchoolService _schoolService;
        IQuestionInfoService _questionInfoService;
        IMapper _mapper;
        IArticleService _articleService;
        IConfiguration _configuration;
        string _apiUrl;
        IOnlineSchoolRecruitService _onlineSchoolRecruitService;
        IOnlineSchoolAchievementService _onlineSchoolAchievementService;
        IOnlineSchoolQuotaService _onlineSchoolQuotaService;
        IOnlineSchoolFractionService _onlineSchoolFractionService;
        ITalentSettingService _talentSettingService;
        IGradeService _gradeService;
        IOnlineSchoolProjectService _onlineSchoolProjectService;
        IOnlineSchoolExtLevelService _onlineSchoolExtLevelService;
        IRegionTypeService _regionTypeService;
        IOrgServiceClient _orgServiceClient;
        ISchoolOverViewService _schoolOverViewService;
        IAreaRecruitPlanService _areaRecruitPlanService;
        IEasyRedisClient _easyRedisClient;
        IWikiService _wikiService;
        ICityInfoService _cityInfoService;

        public SchoolDetailController(ISchoolService schoolService
            , IMapper mapper
            , IArticleService articleService
            , IConfiguration configuration
            , IQuestionInfoService questionInfoService
            , IOnlineSchoolRecruitService onlineSchoolRecruitService
            , IOnlineSchoolAchievementService onlineSchoolAchievementService
            , IOnlineSchoolQuotaService onlineSchoolQuotaService
            , IOnlineSchoolFractionService onlineSchoolFractionService
            , ITalentSettingService talentSettingService
            , IGradeService gradeService
            , IOnlineSchoolExtLevelService onlineSchoolExtLevelService
            , IOnlineSchoolProjectService onlineSchoolProjectService
            , IRegionTypeService regionTypeService
            , IOrgServiceClient orgServiceClient
            , IEasyRedisClient easyRedisClient
            , IAreaRecruitPlanService areaRecruitPlanService
            , ISchoolOverViewService schoolOverViewService
            , IWikiService wikiService, ICityInfoService cityInfoService)
        {
            _easyRedisClient = easyRedisClient;
            _areaRecruitPlanService = areaRecruitPlanService;
            _schoolOverViewService = schoolOverViewService;
            _onlineSchoolExtLevelService = onlineSchoolExtLevelService;
            _onlineSchoolProjectService = onlineSchoolProjectService;
            _gradeService = gradeService;
            _talentSettingService = talentSettingService;
            _onlineSchoolFractionService = onlineSchoolFractionService;
            _onlineSchoolQuotaService = onlineSchoolQuotaService;
            _onlineSchoolAchievementService = onlineSchoolAchievementService;
            _onlineSchoolRecruitService = onlineSchoolRecruitService;
            _questionInfoService = questionInfoService;
            _configuration = configuration;
            _articleService = articleService;
            _mapper = mapper;
            _schoolService = schoolService;
            if (_configuration != null)
            {
                var baseUrl = _configuration.GetSection("ExternalInterface").GetValue<string>("OrganizationAddress");
                if (!string.IsNullOrWhiteSpace(baseUrl))
                {
                    _apiUrl = $"{baseUrl}api/ToSchools/hotsell/coursesandorgs?minage={{0}}&maxage={{1}}";
                }
            }
            _regionTypeService = regionTypeService;
            _orgServiceClient = orgServiceClient;
            _wikiService = wikiService;
            _cityInfoService = cityInfoService;
        }

        /// <summary>
        /// 学校详情For微信对接
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <returns></returns>
        public async Task<ResponseResult> GetDetailForWechat(Guid eid)
        {
            var result = ResponseResult.Failed();
            var schoolInfo = await _schoolService.GetSchoolExtensionDetails(eid);
            if (schoolInfo == null || schoolInfo.ExtId == Guid.Empty)
            {
                result.Msg = "eid not found";
                return result;
            }
            var resultData = new GetDetailForWechatResponse();
            var schoolVideo = await _schoolService.GetSchoolVideos(eid);

            #region loading...
            var recommendExtensions = await _easyRedisClient
                .GetOrAddAsync($"ad:Recommend:{schoolInfo.ExtId}", () =>
                {
                    return _schoolService.RecommendSchoolAsync(schoolInfo.ExtId, schoolInfo.Type, schoolInfo.Grade, schoolInfo.City, 2);
                }, DateTime.Now.AddDays(7));
            #endregion

            #region 招生信息
            var recruits = await _onlineSchoolRecruitService.GetByEID(eid, type: -1);
            if (recruits?.Any() == true)
            {
                var recruitIDs = recruits.Select(p => p.ID).Distinct();
                var recruitTypes = recruits.Select(p => p.Type).Distinct();
                IEnumerable<RecruitScheduleInfo> recruitSchedules = null;
                if (schoolInfo.Grade == (byte)SchoolGrade.SeniorMiddleSchool)
                {
                    recruitSchedules = await _onlineSchoolRecruitService.GetRecruitSchedules(schoolInfo.City, recruitTypes, schoolInfo.SchFType.Code);
                }
                else
                {
                    recruitSchedules = await _onlineSchoolRecruitService.GetRecruitSchedules(schoolInfo.City, recruitTypes, schoolInfo.SchFType.Code, schoolInfo.Area);
                }
                var recruitDatas = new List<Recruit>();
                var addressEIDs = new List<Guid>();
                foreach (var item in recruits)
                {
                    var recruit = CommonHelper.MapperProperty<OnlineSchoolRecruitInfo, Recruit>(item);
                    //招生日程
                    if (recruitSchedules?.Any(p => p.RecruitType == item.Type) == true)
                    {
                        recruit.RecruitSchedules = recruitSchedules.Where(p => p.RecruitType == item.Type).OrderBy(p => p.Index);
                    }
                    if (item.AllocationPrimaryEIDs_Obj?.Any() == true) addressEIDs.AddRange(item.AllocationPrimaryEIDs_Obj);
                    if (item.CounterpartPrimaryEIDs_Obj?.Any() == true) addressEIDs.AddRange(item.CounterpartPrimaryEIDs_Obj);
                    if (item.ScribingScopeEIDs_Obj?.Any() == true) addressEIDs.AddRange(item.ScribingScopeEIDs_Obj);
                    recruitDatas.Add(recruit);
                }
                if (addressEIDs?.Any() == true)
                {
                    addressEIDs = addressEIDs.Distinct().ToList();
                    var addresses = await _schoolService.GetExtAddresses(addressEIDs);
                    if (addresses?.Any() == true)
                    {
                        foreach (var item in recruitDatas)
                        {
                            if (item.AllocationPrimaryEIDs_Obj?.Any() == true)
                            {
                                item.AllocationPrimary = JsonConvert.SerializeObject(addresses.Where(p => item.AllocationPrimaryEIDs_Obj.Contains(p.Key)).Select(p => new string[] {
                                p.Message,
                                p.Data,
                                p.Key.ToString()
                            }));
                            }
                            if (item.CounterpartPrimaryEIDs_Obj?.Any() == true)
                            {
                                item.CounterpartPrimary = JsonConvert.SerializeObject(addresses.Where(p => item.CounterpartPrimaryEIDs_Obj.Contains(p.Key)).Select(p => new string[] {
                                p.Message,
                                p.Data,
                                p.Key.ToString()
                            }));
                            }
                            if (item.ScribingScopeEIDs_Obj?.Any() == true)
                            {
                                item.ScribingScope = JsonConvert.SerializeObject(addresses.Where(p => item.ScribingScopeEIDs_Obj.Contains(p.Key)).Select(p => new string[] {
                                p.Message,
                                p.Data,
                                p.Key.ToString()
                            }));
                            }
                        }
                    }
                }
                resultData.Recruits = recruitDatas.OrderBy(p => p.Type);
            }
            #endregion

            #region 学部其他信息
            var overviewOtherInfo = await _schoolOverViewService.GetByEID(eid);
            #endregion

            #region 区域招生政策
            if (schoolInfo.Grade == (byte)SchoolGrade.SeniorMiddleSchool)
            {
                resultData.AreaRecruitPlan = await _areaRecruitPlanService.GetByAreaCodeAndSchFType(schoolInfo.City.ToString(),
                    schoolInfo.SchFType.Code);
            }
            else
            {
                resultData.AreaRecruitPlan = await _areaRecruitPlanService.GetByAreaCodeAndSchFType(schoolInfo.Area.ToString(),
                    schoolInfo.SchFType.Code);
            }
            #endregion

            #region 相关推荐
            if (recommendExtensions?.Any() == true)
            {
                resultData.RecommendExtensions = recommendExtensions.Select(p => new
                {
                    SchoolName = p.Message,
                    ExtName = p.Data,
                    ShortNo = UrlShortIdUtil.Long2Base32(p.Other),
                    EID = p.Value
                });
            }
            #endregion

            #region 费用相关
            if (resultData.Recruits?.Any(p => !string.IsNullOrWhiteSpace(p.OtherCost) || !string.IsNullOrWhiteSpace(p.ApplyCost) || !string.IsNullOrWhiteSpace(p.Tuition)) == true)
            {
                var find = resultData.Recruits.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.OtherCost) || !string.IsNullOrWhiteSpace(p.ApplyCost) || !string.IsNullOrWhiteSpace(p.Tuition));
                resultData.ApplyCost = find.ApplyCost;
                resultData.Tuition = find.Tuition;
                resultData.OtherCost = find.OtherCost;
            }
            #endregion

            var userId = Request.GetDeviceToGuid();

            #region 学校问答
            var schoolQuestion = _questionInfoService.PushSchoolInfo(new List<Guid>() { eid }, userId);
            if (schoolQuestion?.Any() == true)
            {
                if (schoolQuestion[0].Id != default(Guid))
                {
                    var questionDetail = _mapper.Map<PMS.CommentsManage.Application.ModelDto.QuestionDto, Web.Models.Question.QuestionVo>(schoolQuestion[0]);
                    //for (int i = 0; i < questionDetail.Images.Count(); i++)
                    //{
                    //    questionDetail.Images[i] = _imageSetting.QueryImager + questionDetail.Images[i];
                    //}
                    questionDetail.Answer = null;
                    resultData.Question = questionDetail;
                }
            }
            #endregion

            #region 分部文章
            var extArticles = await Task.Run(() =>
            {
                return _articleService.GetComparisionArticles(new List<Guid> { eid }, 2);
            });
            if (extArticles?.Any() == true)
            {
                resultData.ExtArticles = extArticles.Select(a => new ArticleListItemViewModel()
                {
                    Id = a.id,
                    Time = a.time.GetValueOrDefault().ConciseTime(),
                    Layout = a.layout,
                    Title = a.title,
                    ViweCount = a.VirualViewCount,
                    No = UrlShortIdUtil.Long2Base32(a.No)
                }).ToList();
            }
            #endregion

            #region 其他分部
            await Task.Run(() =>
            {
                resultData.ExtSchools = _schoolService.GetSchoolExtName(schoolInfo.Sid)?.Where(p => p.Value != eid);
            });
            #endregion

            #region 机构课程
            //resultData.OrgLessons = await GetOrgLesson(schoolInfo.Grade);
            var cityId = schoolInfo.City;
            var areaId = schoolInfo.Area;
            resultData.OrgLessons = await GetRecommendsOrgLesson(cityId, areaId, schoolInfo.Sid, schoolInfo.ExtId, schoolInfo.SchFType);
            #endregion

            resultData.TypeName = ReplaceSchFTypeName(schoolInfo.SchFType);
            resultData.Grade = schoolInfo.Grade;
            resultData.SchoolName = schoolInfo.SchoolName;
            //resultData.SchoolImages = schoolInfo.SchoolImages;
            resultData.ExtName = schoolInfo.ExtName;
            resultData.Latitude = schoolInfo.Latitude;
            resultData.Longitude = schoolInfo.Longitude;
            resultData.EnglishName = schoolInfo.EName;
            resultData.SchoolImageUrl = schoolInfo.SchoolImageUrl;

            #region BrandImages
            //var brandImages = await _schoolService.GetSchoolImages(eid, new PMS.School.Domain.Enum.SchoolImageType[] { PMS.School.Domain.Enum.SchoolImageType.SchoolBrand });
            //if (brandImages?.Any() == true)
            //{
            //    if (resultData.SchoolImages == null) resultData.SchoolImages = new List<SchoolImageDto>();
            //    resultData.SchoolImages.AddRange(brandImages);
            //}
            resultData.SchoolImages = await _schoolService.GetSchoolImages(eid, Enum.GetValues(typeof(SchoolImageType)).OfType<SchoolImageType>().ToArray());
            #endregion

            #region Tags
            //resultData.Tags = schoolInfo.Tags;
            resultData.Tags = new List<string>();
            if (!string.IsNullOrEmpty(resultData.TypeName)) resultData.Tags.Add(resultData.TypeName);
            if (schoolInfo.EduSysType.HasValue) resultData.Tags.Add(CommonHelper.GetDescriptionFromEnumValue(schoolInfo.EduSysType.Value));
            if (schoolInfo.AuthenticationList?.Any() == true)
            {
                var extLevels = await _onlineSchoolExtLevelService.GetByCityCodeSchFType(schoolInfo.City, schoolInfo.SchFType.Code);
                if (extLevels?.Any() == true)
                {
                    var schoolAuths = schoolInfo.AuthenticationList.Select(p => p.Key).Distinct();
                    foreach (var item in extLevels)
                    {
                        if (schoolAuths.Any(p => p == item.ReplaceSource)) resultData.Tags.Add(item.LevelName);
                    }
                }
            }
            #endregion

            #region 对口学校
            if (schoolInfo.Grade == (byte)SchoolGrade.PrimarySchool)
            {
                if (!string.IsNullOrWhiteSpace(schoolInfo.CounterPart) || schoolInfo.CounterPart != "[]")
                {
                    try
                    {
                        var obj_CounterPart = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, Guid>>>(schoolInfo.CounterPart);
                        var eids = obj_CounterPart.Select(p => p.Value).Distinct();
                        var addresses = _schoolService.GetSchoolExtAdress(eids.ToArray());
                        resultData.CounterPart = addresses.Select(p => (string.Join(" - ",
                            obj_CounterPart.FirstOrDefault(x => x.Value == p.Value).Key.Split('_')), p.Key, p.Value.ToString()));
                    }
                    catch { }
                }
            }
            #endregion

            #region OverView
            resultData.Overview = new SchoolOverview()
            {
                Abroad = schoolInfo.Abroad,
                Address = schoolInfo.Address,
                HasCanteen = schoolInfo.Canteen.GetValueOrDefault(),
                Intro = schoolInfo.Intro,
                LodgingType = schoolInfo.LodgingType,
                StudentQuantity = schoolInfo.Studentcount ?? 0,
                TeacherStudentScale = schoolInfo.TsPercent.GetValueOrDefault().ToString(),
                Tel = schoolInfo.Tel,
                WebSite = schoolInfo.WebSite,
                HasSchoolBus = schoolInfo.HasSchoolBus,
                CityCode = schoolInfo.City
            };
            if (schoolInfo.SchFType.Code != "lx432" && schoolInfo.SchFType.Code != "lx430") resultData.Overview.Abroad = null;//国际高中+外籍人员子女高中显示出国方向
            if (overviewOtherInfo != null)
            {
                //招生方式
                resultData.Overview.RecruitWay = overviewOtherInfo.RecruitWay_Obj;
                resultData.Overview.HasSchoolBus = overviewOtherInfo.HasSchoolBus;
            }
            #endregion

            #region 非幼儿园不能显示手机
            if (!string.IsNullOrWhiteSpace(resultData.Overview.Tel) && schoolInfo.Grade != (byte)SchoolGrade.Kindergarten)
            {
                var schoolPhone = new List<string>();
                if (resultData.Overview.Tel.Contains('；'))
                {
                    foreach (var item in resultData.Overview.Tel.Split('；'))
                    {
                        if (!CommonHelper.isMobile(item)) schoolPhone.Add(item);
                    }
                }
                else
                {
                    if (!CommonHelper.isMobile(resultData.Overview.Tel)) schoolPhone.Add(resultData.Overview.Tel);
                }
                if (schoolPhone.Any()) resultData.Overview.Tel = string.Join('；', schoolPhone);
            }
            #endregion

            //升学成绩
            resultData.Achievements = await _onlineSchoolAchievementService.GetByEID(eid);
            //指标分配
            resultData.Quotas = await _onlineSchoolQuotaService.GetByEID(eid);
            //分数线
            resultData.Fractions = await _onlineSchoolFractionService.GetByEID(eid);
            resultData.Fractions2 = await _onlineSchoolFractionService.Get2ByEID(eid);
            //推荐达人
            resultData.Talent = await GetRecommendTalent(schoolInfo.Grade, schoolInfo.Sid);

            if (resultData.Fractions?.Any() == true)
            {
                var fractions_Obj = new List<dynamic>();
                var type1Items = resultData.Fractions.Where(p => p.LowestFraction.HasValue).ToList();
                var type2Items = resultData.Fractions.Where(p => !p.LowestFraction.HasValue).ToList();
                if (type1Items?.Any() == true)
                {
                    //foreach (var item in type1Items.Where(p => p.LowestFraction == -1))
                    //{
                    //    item.LowestFraction = null;
                    //}
                    fractions_Obj.Add(new
                    {
                        Type = 0,
                        Data = type1Items.Select(p => p.BatchIndex).Distinct().OrderBy(p => p).Select(p => new
                        {
                            BatchType = type1Items.FirstOrDefault(x => x.BatchIndex == p).BatchType,
                            Items = type1Items.Where(x => x.BatchIndex == p).OrderByDescending(x => x.StudentType)
                        })
                    });
                }
                if (type2Items?.Any() == true)
                {
                    fractions_Obj.Add(new
                    {
                        Type = 1,
                        Data = type2Items.Select(p => p.BatchIndex).Distinct().OrderBy(p => p).Select(p => new
                        {
                            BatchType = type2Items.FirstOrDefault(x => x.BatchIndex == p).BatchType,
                            Items = type2Items.Where(x => x.BatchIndex == p).OrderByDescending(x => x.StudentType)
                        })
                    });
                }
                resultData.Fractions_Obj = fractions_Obj;
            }

            result = ResponseResult.Success(resultData);
            return result;
        }

        /// <summary>
        /// 获取详情总览
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        public async Task<ResponseResult> GetDetailForWechat1(Guid eid)
        {
            var result = ResponseResult.Failed();
            var schoolInfo = await _schoolService.GetSchoolExtensionDetails(eid);
            if (schoolInfo == null || schoolInfo.ExtId == Guid.Empty)
            {
                result.Msg = "eid not found";
                return result;
            }
            var resultData = new GetDetailForWechat1Response();

            #region loading...
            var recommendExtensions = await _easyRedisClient
                .GetOrAddAsync($"ad:Recommend:{schoolInfo.ExtId}", () =>
                {
                    return _schoolService.RecommendSchoolAsync(schoolInfo.ExtId, schoolInfo.Type, schoolInfo.Grade, schoolInfo.City, 2);
                }, DateTime.Now.AddDays(7));
            #endregion

            #region 招生信息
            var recruits = await _onlineSchoolRecruitService.GetByEID(eid, type: -1);
            if (recruits?.Any() == true)
            {
                var recruitIDs = recruits.Select(p => p.ID).Distinct();
                var recruitTypes = recruits.Select(p => p.Type).Distinct();
                IEnumerable<RecruitScheduleInfo> recruitSchedules = null;
                if (schoolInfo.Grade == (byte)SchoolGrade.SeniorMiddleSchool)
                {
                    recruitSchedules = await _onlineSchoolRecruitService.GetRecruitSchedules(schoolInfo.City, recruitTypes, schoolInfo.SchFType.Code);
                }
                else
                {
                    recruitSchedules = await _onlineSchoolRecruitService.GetRecruitSchedules(schoolInfo.City, recruitTypes, schoolInfo.SchFType.Code, schoolInfo.Area);
                }
                var recruitDatas = new List<Recruit>();
                var addressEIDs = new List<Guid>();
                foreach (var item in recruits)
                {
                    var recruit = CommonHelper.MapperProperty<OnlineSchoolRecruitInfo, Recruit>(item);
                    //招生日程
                    if (recruitSchedules?.Any(p => p.RecruitType == item.Type) == true)
                    {
                        recruit.RecruitSchedules = recruitSchedules.Where(p => p.RecruitType == item.Type).OrderBy(p => p.Index);
                    }
                    if (item.AllocationPrimaryEIDs_Obj?.Any() == true) addressEIDs.AddRange(item.AllocationPrimaryEIDs_Obj);
                    if (item.CounterpartPrimaryEIDs_Obj?.Any() == true) addressEIDs.AddRange(item.CounterpartPrimaryEIDs_Obj);
                    if (item.ScribingScopeEIDs_Obj?.Any() == true) addressEIDs.AddRange(item.ScribingScopeEIDs_Obj);
                    recruitDatas.Add(recruit);
                }
                if (addressEIDs?.Any() == true)
                {
                    addressEIDs = addressEIDs.Distinct().ToList();
                    var addresses = await _schoolService.GetExtAddresses(addressEIDs);
                    if (addresses?.Any() == true)
                    {
                        foreach (var item in recruitDatas)
                        {
                            if (item.AllocationPrimaryEIDs_Obj?.Any() == true)
                            {
                                item.AllocationPrimary = JsonConvert.SerializeObject(addresses.Where(p => item.AllocationPrimaryEIDs_Obj.Contains(p.Key)).Select(p => new string[] {
                                p.Message,
                                p.Data,
                                p.Key.ToString()
                            }));
                            }
                            if (item.CounterpartPrimaryEIDs_Obj?.Any() == true)
                            {
                                item.CounterpartPrimary = JsonConvert.SerializeObject(addresses.Where(p => item.CounterpartPrimaryEIDs_Obj.Contains(p.Key)).Select(p => new string[] {
                                p.Message,
                                p.Data,
                                p.Key.ToString()
                            }));
                            }
                            if (item.ScribingScopeEIDs_Obj?.Any() == true)
                            {
                                item.ScribingScope = JsonConvert.SerializeObject(addresses.Where(p => item.ScribingScopeEIDs_Obj.Contains(p.Key)).Select(p => new string[] {
                                p.Message,
                                p.Data,
                                p.Key.ToString()
                            }));
                            }
                        }
                    }
                }
                resultData.Recruits = recruitDatas.OrderBy(p => p.Type);
            }
            #endregion

            resultData.RecruitYears = await _onlineSchoolRecruitService.GetYears(eid);
            resultData.Quotas = await _onlineSchoolQuotaService.GetByEID(eid);
            resultData.QuotaYears = await _onlineSchoolQuotaService.GetYears(eid);
            resultData.Fractions = await _onlineSchoolFractionService.GetByEID(eid);
            resultData.FractionYears = await _onlineSchoolFractionService.GetYears(eid);
            resultData.Achievements = await _onlineSchoolAchievementService.GetByEID(eid);
            resultData.AchievemenYears = await _onlineSchoolAchievementService.GetYears(eid);
            resultData.Projects = await _onlineSchoolProjectService.GetByEID(eid);
            var images = await _schoolService.GetSchoolImages(eid, Enum.GetValues(typeof(SchoolImageType)).OfType<SchoolImageType>().ToArray());
            resultData.CostYears = await _onlineSchoolRecruitService.GetCostYears(eid);

            #region Overview
            resultData.Overview = new SchoolOverviewEx()
            {
                Abroad = schoolInfo.Abroad,
                Address = schoolInfo.Address,
                HasCanteen = schoolInfo.Canteen.GetValueOrDefault(),
                HasSchoolBus = schoolInfo.HasSchoolBus,
                Intro = schoolInfo.Intro,
                LodgingType = schoolInfo.LodgingType,
                StudentQuantity = schoolInfo.Studentcount.GetValueOrDefault(),
                TeacherStudentScale = schoolInfo.TsPercent.GetValueOrDefault().ToString(),
                Tel = schoolInfo.Tel,
                WebSite = schoolInfo.WebSite,
                CityCode = schoolInfo.City
            };
            if (schoolInfo.SchFType.Code != "lx432" || schoolInfo.SchFType.Code != "lx430") resultData.Overview.Abroad = null;//国际高中+外籍人员子女高中显示出国方向
            #region 学部其他信息
            var overviewOtherInfo = await _schoolOverViewService.GetByEID(eid);
            if (overviewOtherInfo != null)
            {
                resultData.Overview.HasSchoolBus = overviewOtherInfo.HasSchoolBus;
                //招生方式
                resultData.Overview.RecruitWay = overviewOtherInfo.RecruitWay_Obj;
            }
            #endregion
            #endregion

            #region CounterParts
            if (schoolInfo.Grade == (byte)SchoolGrade.PrimarySchool)
            {
                if (!string.IsNullOrWhiteSpace(schoolInfo.CounterPart) || schoolInfo.CounterPart != "[]")
                {
                    try
                    {
                        var obj_CounterPart = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, Guid>>>(schoolInfo.CounterPart);
                        var eids = obj_CounterPart.Select(p => p.Value).Distinct();
                        var addresses = _schoolService.GetSchoolExtAdress(eids.ToArray());
                        resultData.CounterParts = addresses.Select(p => (string.Join(" - ", obj_CounterPart.FirstOrDefault(x => x.Value == p.Value).Key.
                        Split('_')), p.Key, p.Value.ToString()));
                    }
                    catch { }
                }
                resultData.CounterPartYears = _schoolService.GetSchoolFieldYears(eid.ToString(), "CounterPart");
            }
            #endregion

            resultData.Videos = schoolInfo.InterviewVideos;

            #region Images
            if (images?.Any() == true)
            {
                //var image_Obj = new List<dynamic>();
                //image_Obj.Add(new KeyValuePair<string, dynamic>("教学质量", schoolInfo.SchoolImages.Where(p => p.Type == PMS.School.Domain.Enum.SchoolImageType.SchoolHonor || p.Type == PMS.School.Domain.Enum.SchoolImageType.StudentHonor).OrderBy(p => p.Sort)));
                //image_Obj.Add(new KeyValuePair<string, dynamic>("师资展示", schoolInfo.SchoolImages.Where(p => p.Type == PMS.School.Domain.Enum.SchoolImageType.Principal || p.Type == PMS.School.Domain.Enum.SchoolImageType.Teacher).OrderBy(p => p.Sort)));
                //image_Obj.Add(new KeyValuePair<string, dynamic>("硬件设施", schoolInfo.SchoolImages.Where(p => p.Type == PMS.School.Domain.Enum.SchoolImageType.Hardware).OrderBy(p => p.Sort)));
                //image_Obj.Add(new KeyValuePair<string, dynamic>("学生生活", schoolInfo.SchoolImages.Where(p => p.Type == PMS.School.Domain.Enum.SchoolImageType.CommunityActivities || p.Type == PMS.School.Domain.Enum.SchoolImageType.GradeSchedule ||
                //      p.Type == PMS.School.Domain.Enum.SchoolImageType.Schedule || p.Type == PMS.School.Domain.Enum.SchoolImageType.Diagram).OrderBy(p => p.Sort)));
                //resultData.Images = schoolInfo.SchoolImages.Select(p => p.Type).Distinct().OrderBy(p => p).Select(p => new
                //{
                //    Type = p,
                //    Data = schoolInfo.SchoolImages.Where(x => x.Type == p).OrderBy(x => x.Sort)
                //});
                //resultData.Images = image_Obj;
                //resultData.Images = schoolInfo.SchoolImages.OrderBy(p => p.Type).ThenBy(p => p.Sort);
                resultData.Images = images;
            }
            #endregion

            #region 推荐达人
            resultData.Talent = await GetRecommendTalent(schoolInfo.Grade, schoolInfo.Sid);
            #endregion

            #region 其他学部
            resultData.OtherExt = await Task.Run(() =>
            {
                return _schoolService.GetSchoolExtName(schoolInfo.Sid)?.Where(p => p.Value != eid);
            });
            #endregion

            resultData.SchoolName = schoolInfo.SchoolName;
            resultData.SchoolExtName = schoolInfo.ExtName;
            resultData.Grade = schoolInfo.Grade;
            resultData.Longitude = schoolInfo.Longitude;
            resultData.Latitude = schoolInfo.Latitude;

            #region 区域招生政策
            if (schoolInfo.Grade == (byte)SchoolGrade.SeniorMiddleSchool)
            {
                resultData.AreaRecruitPlans = await _areaRecruitPlanService.GetByAreaCodeAndSchFType(schoolInfo.City.ToString(),
                    schoolInfo.SchFType.Code);
                resultData.AreaRecruitPlanYears = await _areaRecruitPlanService.GetYears(schoolInfo.City.ToString(),
                schoolInfo.SchFType.Code);
            }
            else
            {
                resultData.AreaRecruitPlans = await _areaRecruitPlanService.GetByAreaCodeAndSchFType(schoolInfo.Area.ToString(),
                    schoolInfo.SchFType.Code);
                resultData.AreaRecruitPlanYears = await _areaRecruitPlanService.GetYears(schoolInfo.Area.ToString(),
                schoolInfo.SchFType.Code);
            }
            #endregion

            #region 收费标准
            if (resultData.Recruits?.Any(p => !string.IsNullOrWhiteSpace(p.OtherCost) || !string.IsNullOrWhiteSpace(p.ApplyCost) ||
            !string.IsNullOrWhiteSpace(p.Tuition)) == true)
            {
                var find = resultData.Recruits.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.OtherCost) || !string.IsNullOrWhiteSpace(p.ApplyCost) || !string.IsNullOrWhiteSpace(p.Tuition));
                resultData.ApplyCost = find.ApplyCost;
                resultData.Tuition = find.Tuition;
                resultData.OtherCost = find.OtherCost;
            }
            #endregion

            #region Courses
            if (schoolInfo.CoursesList?.Any() == true) resultData.Courses = schoolInfo.CoursesList.Select(p => p.Key);
            if (!string.IsNullOrWhiteSpace(schoolInfo.CourseCharacteristic)) resultData.CourseCharacteristic = schoolInfo.CourseCharacteristic;
            if (!string.IsNullOrWhiteSpace(schoolInfo.CourseAuthentication))
            {
                try
                {
                    var courseAuth_Obj = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, string>>>(schoolInfo.CourseAuthentication);
                    if (courseAuth_Obj?.Any() == true)
                    {
                        resultData.CourseAuths = courseAuth_Obj.Select(p => p.Key);
                    }
                }
                catch { }
            }
            #endregion

            #region 学校认证
            if (overviewOtherInfo?.Certifications_Obj != null) resultData.SchoolAuths = overviewOtherInfo.Certifications_Obj;
            #endregion

            #region Fractions
            if (resultData.Fractions?.Any() == true)
            {
                var fractions_Obj = new List<dynamic>();
                var type1Items = resultData.Fractions.Where(p => p.LowestFraction.HasValue).ToList();
                var type2Items = resultData.Fractions.Where(p => !p.LowestFraction.HasValue).ToList();
                if (type1Items?.Any() == true)
                {
                    //foreach (var item in type1Items.Where(p => p.LowestFraction == -1))
                    //{
                    //    item.LowestFraction = null;
                    //}
                    fractions_Obj.Add(new
                    {
                        Type = 0,
                        Data = type1Items.Select(p => p.BatchIndex).Distinct().OrderBy(p => p).Select(p => new
                        {
                            BatchType = type1Items.FirstOrDefault(x => x.BatchIndex == p).BatchType,
                            Items = type1Items.Where(x => x.BatchIndex == p).OrderByDescending(x => x.StudentType)
                        })
                    });
                }

                if (type2Items?.Any() == true)
                {
                    fractions_Obj.Add(new
                    {
                        Type = 1,
                        Data = type2Items.Select(p => p.BatchIndex).Distinct().OrderBy(p => p).Select(p => new
                        {
                            BatchType = type2Items.FirstOrDefault(x => x.BatchIndex == p).BatchType,
                            Items = type2Items.Where(x => x.BatchIndex == p).OrderByDescending(x => x.StudentType)
                        })
                    });
                }
                resultData.Fractions_Obj = fractions_Obj;
            }
            resultData.Fractions2 = await _onlineSchoolFractionService.Get2ByEID(eid);
            resultData.Fraction2Years = await _onlineSchoolFractionService.Get2Years(eid);
            #endregion

            #region 分部文章
            var extArticles = await Task.Run(() =>
            {
                return _articleService.GetComparisionArticles(new List<Guid> { eid }, 2);
            });
            if (extArticles?.Any() == true)
            {
                resultData.ExtArticles = extArticles.Select(a => new ArticleListItemViewModel()
                {
                    Id = a.id,
                    Time = a.time.GetValueOrDefault().ConciseTime(),
                    Layout = a.layout,
                    Title = a.title,
                    ViweCount = a.VirualViewCount,
                    No = UrlShortIdUtil.Long2Base32(a.No)
                }).ToList();
            }
            #endregion

            #region 机构课程
            //resultData.OrgLessons = await GetOrgLesson(schoolInfo.Grade);
            var cityId = schoolInfo.City;
            var areaId = schoolInfo.Area;
            resultData.OrgLessons = await GetRecommendsOrgLesson(cityId, areaId, schoolInfo.Sid, schoolInfo.ExtId, schoolInfo.SchFType);
            #endregion

            #region 相关推荐
            if (recommendExtensions?.Any() == true)
            {
                resultData.RecommendExtensions = recommendExtensions.Select(p => new
                {
                    SchoolName = p.Message,
                    ExtName = p.Data,
                    ShortNo = UrlShortIdUtil.Long2Base32(p.Other),
                    EID = p.Value
                });
            }
            #endregion

            #region 非幼儿园不能显示手机
            if (!string.IsNullOrWhiteSpace(resultData.Overview.Tel) && schoolInfo.Grade != (byte)SchoolGrade.Kindergarten)
            {
                var schoolPhone = new List<string>();
                if (resultData.Overview.Tel.Contains('；'))
                {
                    foreach (var item in resultData.Overview.Tel.Split('；'))
                    {
                        if (!CommonHelper.isMobile(item)) schoolPhone.Add(item);
                    }
                }
                else
                {
                    if (!CommonHelper.isMobile(resultData.Overview.Tel)) schoolPhone.Add(resultData.Overview.Tel);
                }
                if (schoolPhone.Any()) resultData.Overview.Tel = string.Join('；', schoolPhone);
            }
            #endregion

            result = ResponseResult.Success(resultData);

            return result;
        }

        /// <summary>
        /// 学校类型名称调整规则
        /// </summary>
        /// <param name="schFTypeCode"></param>
        /// <returns></returns>
        string ReplaceSchFTypeName(SchFType0 schFType)
        {
            string result = null;
            switch (schFType.Code)
            {
                //公办幼儿园
                case "lx110":
                    result = "公办幼儿园";
                    break;
                //普通民办幼儿园
                case "lx120":
                    result = "民办幼儿园";
                    break;
                //民办普惠幼儿园
                case "lx121":
                    result = "民办幼儿园";
                    break;
                //国际幼儿园
                case "lx130":
                    result = "国际幼儿园";
                    break;
                //公办小学
                case "lx210":
                    result = "公办小学";
                    break;
                //普通民办小学
                case "lx220":
                    result = "民办小学";
                    break;
                //双语小学
                case "lx231":
                    result = "民办小学";
                    break;
                //外国人小学
                case "lx230":
                    result = "外籍人员子女小学";
                    break;
                //公办初中
                case "lx310":
                    result = "公办初中";
                    break;
                //普通民办初中
                case "lx320":
                    result = "民办初中";
                    break;
                //双语初中
                case "lx331":
                    result = "民办初中";
                    break;
                //外国人初中
                case "lx330":
                    result = "外籍人员子女初中";
                    break;
                //公办高中
                case "lx410":
                    result = "公办高中";
                    break;
                //普通民办高中
                case "lx420":
                    result = "民办高中";
                    break;
                //国际高中
                case "lx432":
                    result = "国际高中";
                    break;
                //外国人高中
                case "lx430":
                    result = "外籍人员子女高中";
                    break;
                default:
                    result = schFType.GetDesc();
                    break;
            }
            return result;
        }

        [HttpGet]
        public async Task<dynamic> TestRecommendsOrgLesson(int cityId, int areaId, Guid schoolId, Guid extId, string schFType)
        {
            SchFType0 schFType0 = new SchFType0(SchFTypeUtil.GetCode(schFType));
            var vm = new OrgLessonViewModel();

            var schFTypes = new string[] { schFType0.GetDesc() };
            var courseShortIds = _wikiService.GetRecommends(PMS.OperationPlateform.Domain.Enums.RecommendOrgGroupCode.WeChatSchool, PMS.OperationPlateform.Domain.Enums.RecommendOrgDataType.Course,
                cityId, areaId, schoolId, extId, schFTypes).ToList();
            var orgShortIds = _wikiService.GetRecommends(PMS.OperationPlateform.Domain.Enums.RecommendOrgGroupCode.WeChatSchool, PMS.OperationPlateform.Domain.Enums.RecommendOrgDataType.Org,
                cityId, areaId, schoolId, extId, schFTypes).ToList();

            vm.HotSellCourses = await _orgServiceClient.GetHotSellCoursesByIds(new List<Guid>(), courseShortIds);
            vm.RecommendOrgs = await _orgServiceClient.GetRecommendOrgsByIds(new List<Guid>(), orgShortIds);
            return new
            {
                courseShortIds,
                vm.HotSellCourses,

                orgShortIds,
                vm.RecommendOrgs
            };
        }

        [HttpGet]
        public async Task<ResponseResult> GetRecommendsOrgLesson(string cityCode, string areaCode, string grade
            , PMS.OperationPlateform.Domain.Enums.RecommendOrgGroupCode groupCode, string schFType0)
        {
            int.TryParse(cityCode, out int cityId);
            int.TryParse(areaCode, out int areaId);
            byte.TryParse(grade, out byte gradeInt);

            if (cityId == 0 && areaId != 0)
            {
                cityId = await _cityInfoService.GetCityCodeByAreaCode(areaId);
            }

            string[] schFTypes;
            if (gradeInt == 0 && schFType0 != null)
            {
                 schFTypes = new string[] { new SchFType0(schFType0).GetDesc() };
            }
            else
            {
                schFTypes = SchFTypeUtil.FindDescs(gradeInt).ToArray();
            }
            OrgLessonViewModel data = await GetRecommendsOrgLessonViewModel(cityId, areaId, gradeInt, schFTypes, groupCode);
            return ResponseResult.Success(data);
        }


        async Task<OrgLessonViewModel> GetRecommendsOrgLessonViewModel(int cityId, int areaId, byte grade, string[] schFTypes
            , PMS.OperationPlateform.Domain.Enums.RecommendOrgGroupCode groupCode = PMS.OperationPlateform.Domain.Enums.RecommendOrgGroupCode.WeChatSchool)
        {
            Guid schoolId = Guid.Empty;
            Guid extId = Guid.Empty;
            var vm = new OrgLessonViewModel();
            try
            {
                var courseShortIds = _wikiService.GetRecommends(groupCode, PMS.OperationPlateform.Domain.Enums.RecommendOrgDataType.Course,
                    cityId, areaId, schoolId, extId, schFTypes).ToList();
                var orgShortIds = _wikiService.GetRecommends(groupCode, PMS.OperationPlateform.Domain.Enums.RecommendOrgDataType.Org,
                    cityId, areaId, schoolId, extId, schFTypes).ToList();

                vm.HotSellCourses = await _orgServiceClient.GetHotSellCoursesByIds(new List<Guid>(), courseShortIds);
                vm.RecommendOrgs = await _orgServiceClient.GetRecommendOrgsByIds(new List<Guid>(), orgShortIds);
            }
            catch (Exception ex)
            {}

            var needCount = 3;
            var courseCount = vm.HotSellCourses.Count();
            var orgCount = vm.RecommendOrgs.Count();
            if (courseCount != needCount || orgCount != needCount)
            {
                //default
                var def = await GetOrgLesson(grade);
                vm.HotSellCourses = courseCount != needCount ? def.HotSellCourses : vm.HotSellCourses;
                vm.RecommendOrgs = orgCount != needCount ? def.RecommendOrgs : vm.RecommendOrgs;
            }
            return vm;
        }


        async Task<OrgLessonViewModel> GetRecommendsOrgLesson(int cityId, int areaId, Guid schoolId, Guid extId, SchFType0 schFType0
            , PMS.OperationPlateform.Domain.Enums.RecommendOrgGroupCode groupCode = PMS.OperationPlateform.Domain.Enums.RecommendOrgGroupCode.WeChatSchool)
        {

            var vm = new OrgLessonViewModel();
            try
            {
                var schFTypes = new string[] { schFType0.GetDesc() };

                var courseShortIds = _wikiService.GetRecommends(groupCode, PMS.OperationPlateform.Domain.Enums.RecommendOrgDataType.Course,
                    cityId, areaId, schoolId, extId, schFTypes).ToList();
                var orgShortIds = _wikiService.GetRecommends(groupCode, PMS.OperationPlateform.Domain.Enums.RecommendOrgDataType.Org,
                    cityId, areaId, schoolId, extId, schFTypes).ToList();

                vm.HotSellCourses = await _orgServiceClient.GetHotSellCoursesByIds(new List<Guid>(), courseShortIds);
                vm.RecommendOrgs = await _orgServiceClient.GetRecommendOrgsByIds(new List<Guid>(), orgShortIds);
            }
            catch (Exception ex)
            {
            }

            var needCount = 3;
            var courseCount = vm.HotSellCourses.Count();
            var orgCount = vm.RecommendOrgs.Count();
            if (courseCount != needCount || orgCount != needCount)
            {
                //default
                var def = await GetOrgLesson(schFType0.Grade);
                vm.HotSellCourses = courseCount != needCount ? def.HotSellCourses : vm.HotSellCourses;
                vm.RecommendOrgs = orgCount != needCount ? def.RecommendOrgs : vm.RecommendOrgs;
            }
            return vm;
        }
        /// <summary>
        /// 获取机构课程
        /// </summary>
        /// <param name="grade"></param>
        /// <returns></returns>
        async Task<OrgLessonViewModel> GetOrgLesson(byte grade)
        {
            if (string.IsNullOrWhiteSpace(_apiUrl)) return null;
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
                    ageMin = 0;
                    ageMax = 3;
                    break;
            }
            try
            {
                var webClient = new WebUtils();
                var apiResponse = await Task.Run(() =>
                {
                    return webClient.DoGet(string.Format(_apiUrl, ageMin, ageMax));
                });
                if (!string.IsNullOrWhiteSpace(apiResponse))
                {

                    var object_Response = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseResult<OrgLessonViewModel>>(apiResponse);

                    if (object_Response.Succeed)
                    {
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<OrgLessonViewModel>(Newtonsoft.Json.JsonConvert.SerializeObject(object_Response.Data));
                    }
                }
            }
            catch
            {
            }

            return null;
        }

        async Task<TalentDetailExtend> GetRecommendTalent(byte grade, Guid? sid = null)
        {
            var gradeType = 0;
            var random = new Random();
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
            if (gradeType < 1) return null;

            var grades = await _gradeService.GetAllGrades();
            var gradeID = grades.FirstOrDefault(p => p.Sort == grade).ID;
            //CC5A2DB7-EBEF-4DB3-8037-867BB2108103
            var talentUserID = Guid.Empty;
            TalentDetailExtend result = null;
            //var relations = new Dictionary<Guid, Guid>();
            //relations.Add(new Guid("CC5A2DB7-EBEF-4DB3-8037-867BB2108103"), new Guid("DF67DA12-2995-4CE5-9982-8DC6F0AE2B76"));
            //relations.Add(new Guid("4F5E41B1-FFAC-41E2-90EB-51AEBD2A208D"), new Guid("76C60D7A-A7AB-419A-9919-FE8DEAFA6373"));
            //relations.Add(new Guid("20370B90-7670-4953-9ECF-1D43E918E0F2"), new Guid("8AC772E6-FDBB-48BD-9507-5E93FEB17E21"));
            //relations.Add(new Guid("864D65CF-D7AA-43CA-A44B-54684BF170D3"), new Guid("F952A28D-A447-47B1-AE16-3DB239F45C75"));
            //await _easyRedisClient.AddAsync("WeChatDemo:SchoolTalentRelations", relations);

            var relations = await _easyRedisClient.GetAsync<Dictionary<Guid, Guid>>("WeChatDemo:SchoolTalentRelations");

            if (sid.HasValue && relations?.Any(p => p.Key == sid.GetValueOrDefault()) == true)
            {
                talentUserID = relations.FirstOrDefault(p => p.Key == sid.Value).Value;
                if (talentUserID != Guid.Empty) result = await _talentSettingService.GetDetail(talentUserID);
            }
            if (result == null)
            {
                var finds = await _talentSettingService.PageTalentIDs(gradeID: gradeID, isInternal: true);
                if (finds == null || !finds.Any()) return null;
                talentUserID = CommonHelper.ListRandom(finds).FirstOrDefault().TalentUserID;
                result = await _talentSettingService.GetDetail(talentUserID);
            }

            return result;
        }
        /// <summary>
        /// 获取学校概况
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>

        public async Task<ResponseResult> GetSchoolOverview(Guid eid)
        {
            var result = ResponseResult.Failed();
            var schoolInfo = await _schoolService.GetSchoolExtensionDetails(eid);
            if (schoolInfo == null || schoolInfo.ExtId == Guid.Empty)
            {
                result.Msg = "eid not found";
                return result;
            }
            var resultData = new GetSchoolOverviewResponse()
            {
                Abroad = schoolInfo.Abroad,
                Address = schoolInfo.Address,
                HasCanteen = schoolInfo.Canteen.GetValueOrDefault(),
                HasSchoolBus = schoolInfo.HasSchoolBus,
                Intro = schoolInfo.Intro,
                LodgingType = schoolInfo.LodgingType,
                StudentQuantity = schoolInfo.Studentcount.GetValueOrDefault(),
                TeacherStudentScale = schoolInfo.TsPercent.GetValueOrDefault().ToString(),
                Tel = schoolInfo.Tel,
                WebSite = schoolInfo.WebSite,
                CourseCharacteristic = schoolInfo.CourseCharacteristic,
            };


            resultData.CourseAuthentication = schoolInfo.CourseAuthentication;
            resultData.Courses = schoolInfo.Courses;
            resultData.AuthenticationList = schoolInfo.AuthenticationList.Select(p => p.Key);
            resultData.Projects = await _onlineSchoolProjectService.GetByEID(eid);


            result = ResponseResult.Success(resultData);
            return result;
        }

        /// <summary>
        /// 获取招生信息
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <param name="year">年份</param>
        /// <returns></returns>
        public async Task<ResponseResult> GetRecruits(Guid eid, int? year = null, int? type = null)
        {
            var result = ResponseResult.Failed();
            var schoolInfo = await _schoolService.GetSchoolExtensionDetails(eid);
            if (schoolInfo == null || schoolInfo.ExtId == Guid.Empty)
            {
                result.Msg = "eid not found";
                return result;
            }
            var recruits = await _onlineSchoolRecruitService.GetByEID(eid, year ?? 0, type ?? -1);
            if (recruits?.Any() == true)
            {
                var recruitIDs = recruits.Select(p => p.ID).Distinct();
                var recruitTypes = recruits.Select(p => p.Type).Distinct();
                IEnumerable<RecruitScheduleInfo> recruitSchedules = null;
                if (schoolInfo.Grade == (byte)SchoolGrade.SeniorMiddleSchool)
                {
                    recruitSchedules = await _onlineSchoolRecruitService.GetRecruitSchedules(schoolInfo.City, recruitTypes, schoolInfo.SchFType.Code);
                }
                else
                {
                    recruitSchedules = await _onlineSchoolRecruitService.GetRecruitSchedules(schoolInfo.City, recruitTypes, schoolInfo.SchFType.Code, schoolInfo.Area);
                }
                var recruitDatas = new List<Recruit>();
                var addressEIDs = new List<Guid>();
                foreach (var item in recruits)
                {
                    var recruit = CommonHelper.MapperProperty<OnlineSchoolRecruitInfo, Recruit>(item);
                    //招生日程
                    if (recruitSchedules?.Any(p => p.RecruitType == item.Type) == true)
                    {
                        recruit.RecruitSchedules = recruitSchedules.Where(p => p.RecruitType == item.Type).OrderBy(p => p.Index);
                    }
                    if (item.AllocationPrimaryEIDs_Obj?.Any() == true) addressEIDs.AddRange(item.AllocationPrimaryEIDs_Obj);
                    if (item.CounterpartPrimaryEIDs_Obj?.Any() == true) addressEIDs.AddRange(item.CounterpartPrimaryEIDs_Obj);
                    if (item.ScribingScopeEIDs_Obj?.Any() == true) addressEIDs.AddRange(item.ScribingScopeEIDs_Obj);
                    recruitDatas.Add(recruit);
                }
                if (addressEIDs?.Any() == true)
                {
                    addressEIDs = addressEIDs.Distinct().ToList();
                    var addresses = await _schoolService.GetExtAddresses(addressEIDs);
                    if (addresses?.Any() == true)
                    {
                        foreach (var item in recruitDatas)
                        {
                            if (item.AllocationPrimaryEIDs_Obj?.Any() == true)
                            {
                                item.AllocationPrimary = JsonConvert.SerializeObject(addresses.Where(p => item.AllocationPrimaryEIDs_Obj.Contains(p.Key)).Select(p => new string[] {
                                p.Message,
                                p.Data,
                                p.Key.ToString()
                            }));
                            }
                            if (item.CounterpartPrimaryEIDs_Obj?.Any() == true)
                            {
                                item.CounterpartPrimary = JsonConvert.SerializeObject(addresses.Where(p => item.CounterpartPrimaryEIDs_Obj.Contains(p.Key)).Select(p => new string[] {
                                p.Message,
                                p.Data,
                                p.Key.ToString()
                            }));
                            }
                            if (item.ScribingScopeEIDs_Obj?.Any() == true)
                            {
                                item.ScribingScope = JsonConvert.SerializeObject(addresses.Where(p => item.ScribingScopeEIDs_Obj.Contains(p.Key)).Select(p => new string[] {
                                p.Message,
                                p.Data,
                                p.Key.ToString()
                            }));
                            }
                        }
                    }
                }
                result = ResponseResult.Success(recruitDatas.OrderBy(p => p.Type));
            }
            return result;
        }

        /// <summary>
        /// 获取对口学校
        /// </summary>
        /// <param name="eid"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<ResponseResult> GetCounterPart(Guid eid, int? year = null)
        {
            var result = ResponseResult.Failed();
            var resultData = new GetCounterPartResponse();
            if (year == null)
            {
                var schoolInfo = await _schoolService.GetSchoolExtensionDetails(eid);
                if (schoolInfo == null || schoolInfo.ExtId == Guid.Empty)
                {
                    result.Msg = "eid not found";
                    return result;
                }
                if (!string.IsNullOrWhiteSpace(schoolInfo.CounterPart) || schoolInfo.CounterPart != "[]")
                {
                    try
                    {
                        var obj_CounterPart = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, Guid>>>(schoolInfo.CounterPart);
                        var eids = obj_CounterPart.Select(p => p.Value).Distinct();
                        var addresses = _schoolService.GetSchoolExtAdress(eids.ToArray());
                        resultData.CounterPart = addresses.Select(p => (obj_CounterPart.FirstOrDefault(x => x.Value == p.Value).Key, p.Key));
                    }
                    catch { }
                }
                resultData.Years = _schoolService.GetSchoolFieldYears(eid.ToString(), "CounterPart");
                result = ResponseResult.Success(resultData);
            }
            else
            {
                var find = _schoolService.GetSchoolFieldContent(eid.ToString(), "CounterPart", year.ToString());
                if (!string.IsNullOrWhiteSpace(find) || find != "[]")
                {
                    try
                    {
                        var obj_CounterPart = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, Guid>>>(find);
                        var eids = obj_CounterPart.Select(p => p.Value).Distinct();
                        var addresses = _schoolService.GetSchoolExtAdress(eids.ToArray());
                        resultData.CounterPart = addresses.Select(p => (obj_CounterPart.FirstOrDefault(x => x.Value == p.Value).Key, p.Key));
                    }
                    catch { }
                }
                result = ResponseResult.Success(resultData);
            }
            return result;
        }

        /// <summary>
        /// 获取指标分配
        /// </summary>
        /// <param name="eid"></param>
        /// <param name="year"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<ResponseResult> GetQuotas(Guid eid, int? year = null, int? type = null)
        {
            var result = ResponseResult.Failed();
            var finds = await _onlineSchoolQuotaService.GetByEID(eid, year ?? 0, type ?? 0);
            if (finds?.Any() == true)
            {
                result = ResponseResult.Success(finds);
            }
            return result;
        }

        /// <summary>
        /// 获取分数线
        /// </summary>
        /// <param name="eid"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<ResponseResult> GetFractions(Guid eid, int? year = null)
        {
            var result = ResponseResult.Failed();
            var finds = await _onlineSchoolFractionService.GetByEID(eid, year ?? 0);
            if (finds?.Any() == true)
            {
                var fractions_Obj = new List<dynamic>();
                var type1Items = finds.Where(p => p.LowestFraction.HasValue);
                var type2Items = finds.Where(p => !p.LowestFraction.HasValue);
                if (type1Items?.Any() == true)
                {
                    fractions_Obj.Add(new
                    {
                        Type = 0,
                        Data = type1Items.Select(p => p.BatchType).Distinct().OrderBy(p => p).Select(p => new
                        {
                            BatchType = p,
                            Items = type1Items.Where(x => x.BatchType == p).OrderBy(x => x.StudentType)
                        })
                    });
                }
                if (type2Items?.Any() == true)
                {
                    fractions_Obj.Add(new
                    {
                        Type = 1,
                        Data = type2Items.Select(p => p.BatchType).Distinct().OrderBy(p => p).Select(p => new
                        {
                            BatchType = p,
                            Items = type2Items.Where(x => x.BatchType == p).OrderBy(x => x.StudentType)
                        })
                    });
                }
                result = ResponseResult.Success(fractions_Obj);
            }
            return result;
        }
        public async Task<ResponseResult> GetFractions2(Guid eid, int? year = null, int? type = null)
        {
            var result = ResponseResult.Failed();
            var finds = await _onlineSchoolFractionService.Get2ByEID(eid, year ?? 0, type ?? 0);
            if (finds?.Any() == true)
            {
                result = ResponseResult.Success(finds);
            }
            return result;
        }

        /// <summary>
        /// 获取升学成绩
        /// </summary>
        /// <param name="eid"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<ResponseResult> GetAchievements(Guid eid, int? year = null)
        {
            var result = ResponseResult.Failed();
            var finds = await _onlineSchoolAchievementService.GetByEID(eid, year ?? 0);
            if (finds?.Any() == true)
            {
                result = ResponseResult.Success(finds);
            }
            return result;
        }

        /// <summary>
        /// 获取区域招生政策
        /// </summary>
        /// <param name="eid"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<ResponseResult> GetAreaRecruitPlans(Guid eid, int? year = null)
        {
            var result = ResponseResult.Failed();
            var schoolInfo = await _schoolService.GetSchoolExtensionDetails(eid);
            if (schoolInfo == null || schoolInfo.ExtId == Guid.Empty)
            {
                result.Msg = "eid not found";
                return result;
            }
            IEnumerable<AreaRecruitPlanInfo> resultDatas = null;
            if (schoolInfo.Grade == (byte)SchoolGrade.SeniorMiddleSchool)
            {
                resultDatas = await _areaRecruitPlanService.GetByAreaCodeAndSchFType(schoolInfo.City.ToString(),
                    schoolInfo.SchFType.Code, year ?? 0);
            }
            else
            {
                resultDatas = await _areaRecruitPlanService.GetByAreaCodeAndSchFType(schoolInfo.Area.ToString(),
                    schoolInfo.SchFType.Code, year ?? 0);
            }
            if (resultDatas?.Any() == true) result = ResponseResult.Success(resultDatas);
            return result;
        }

        public async Task<ResponseResult> GetCostByYear(Guid eid, int year)
        {
            var result = ResponseResult.Failed();
            var finds = await _onlineSchoolRecruitService.GetCostByYear(eid, year);
            if (finds?.Any() == true)
            {
                result = ResponseResult.Success(new
                {
                    finds.First().Tuition,
                    finds.First().ApplyCost,
                    finds.First().OtherCost_Obj
                });
            }
            return result;
        }
    }
}

