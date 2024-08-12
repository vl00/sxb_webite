using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMS.Infrastructure.Application.IService;
using PMS.MediatR.Events.PaidQA;
using PMS.PaidQA.Application.Services;
using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.Enums;
using PMS.School.Application.IServices;
using PMS.School.Domain.Dtos;
using PMS.Search.Application.IServices;
using PMS.Search.Application.ModelDto.Query;
using PMS.UserManage.Application.IServices;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using Sxb.Web.Areas.PaidQA.Models.Assess;
using Sxb.Web.Common;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.PaidQA.Controllers
{
    [Route("PaidQA/[controller]/[action]")]
    [ApiController]
    public class AssessController : ControllerBase
    {
        IAssessService _assessService;
        IAssessOptionService _assessOptionService;
        IAssessQuestionService _assessQuestionService;
        IAssessOptionRelationService _assessOptionRelationService;
        IAssessStatisticsService _assessStatisticsService;
        IUserService _userService;
        ISearchService _searchService;
        ICityInfoService _cityInfoService;
        ISchoolService _schoolService;
        ITalentSettingService _talentSettingService;
        IMediator _mediator;
        IOrderService _orderService;
        IEasyRedisClient _easyRedisClient;

        public AssessController(IAssessService assessService, IAssessQuestionService assessQuestionService, IAssessOptionService assessOptionService, IAssessOptionRelationService assessOptionRelationService,
            IUserService userService, ISearchService searchService, ICityInfoService cityInfoService, ISchoolService schoolService, ITalentSettingService talentSettingService,
            IMediator mediator, IAssessStatisticsService assessStatisticsService, IOrderService orderService, IEasyRedisClient easyRedisClient)
        {
            _easyRedisClient = easyRedisClient;
            _orderService = orderService;
            _assessStatisticsService = assessStatisticsService;
            _mediator = mediator;
            _talentSettingService = talentSettingService;
            _schoolService = schoolService;
            _cityInfoService = cityInfoService;
            _searchService = searchService;
            _userService = userService;
            _assessService = assessService;
            _assessQuestionService = assessQuestionService;
            _assessOptionRelationService = assessOptionRelationService;
            _assessOptionService = assessOptionService;
        }

        /// <summary>
        /// 获取问题类型
        /// </summary>
        /// <returns></returns>
        public ResponseResult GetQuestionTypes()
        {
            var result = ResponseResult.Failed();
            var dic_Types = new Dictionary<int, string>();
            foreach (AssessType item in Enum.GetValues(typeof(AssessType)))
            {
                if (item == AssessType.Unknow) continue;
                dic_Types.Add((int)item, item.Description());
            }
            result = ResponseResult.Success(dic_Types);
            return result;
        }

        /// <summary>
        /// 根据类型获取问题
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ResponseResult> GetQuestionsByType(AssessType type)
        {
            var result = ResponseResult.Failed();
            var finds = await _assessQuestionService.GetByType(type);
            if (finds?.Any() == true)
            {
                result = ResponseResult.Success(finds);
                await _mediator.Publish(new AssessStatisticsEvent(type, 1, User.Identity.GetId()));
            }
            return result;
        }

        /// <summary>
        /// 根据问题获取选项
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<ResponseResult> GetOptionsByQID(Guid qid)
        {
            var result = ResponseResult.Failed();
            if (qid == Guid.Empty) return result;
            var userID = User.Identity.GetId();
            var assesses = await _assessService.GetByUserID(userID, AssessStatus.Processing);
            if (assesses == null || !assesses.Any())
            {
                result.Msg = "无进行中的测评";
                return result;
            }
            var entity_Question = await _assessQuestionService.GetAsync(qid);
            if (entity_Question == null) return result;
            var entities_OptionRelation = new List<AssessOptionRelationInfo>();
            if (entity_Question.AssessType == AssessType.Type1 && entity_Question.Level > 1)
            {
                var dic_Options = new Dictionary<int, IEnumerable<Guid>>();
                var dic_dynamic = new Dictionary<int, dynamic>();
                try
                {
                    dic_dynamic = JsonConvert.DeserializeObject<Dictionary<int, dynamic>>(assesses.First().SelectedOptionShortIDs);
                    foreach (var item in dic_dynamic)
                    {
                        try
                        {
                            dic_Options.Add(item.Key, JsonConvert.DeserializeObject<IEnumerable<Guid>>(item.Value.ToString()));
                        }
                        catch { }
                    }
                }
                catch { }
                if (dic_Options.Any())
                {
                    switch (entity_Question.Level)
                    {
                        case 2:
                            entities_OptionRelation = (await _assessOptionRelationService.GetByQID(qid, dic_Options.GetValueOrDefault(1).First()))?.ToList();
                            break;
                        case 3:
                            entities_OptionRelation = (await _assessOptionRelationService.GetByQID(qid, dic_Options.GetValueOrDefault(1).First(), dic_Options.GetValueOrDefault(2).First()))?.ToList();
                            break;
                    }
                }
            }
            else
            {
                entities_OptionRelation = (await _assessOptionRelationService.GetByQID(qid))?.ToList();
            }
            if (entities_OptionRelation == null || !entities_OptionRelation.Any()) return result;
            var relation = entities_OptionRelation.First();
            var shortIDs = new List<int>();
            try
            {
                shortIDs = JsonConvert.DeserializeObject<List<int>>(relation.NextOptionShortIDs);
            }
            catch { }
            if (shortIDs?.Any() == true)
            {
                var options = await _assessOptionService.GetByShortIDs(shortIDs);
                if (options?.Any() == true) result = ResponseResult.Success(options.Select(p => new
                {
                    ID = p.ID,
                    p.Content
                }));
            }
            return result;
        }

        /// <summary>
        /// 开始测评
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ResponseResult> Start(AssessType type)
        {
            var result = ResponseResult.Failed();
            if (type == AssessType.Unknow)
            {
                result.Msg = "Params error";
                return result;
            }
            var userID = User.Identity.GetId();
            var assesses = await _assessService.GetByUserID(userID, AssessStatus.Processing);
            var assess = new AssessInfo()
            {
                CreateTime = DateTime.Now,
                Status = AssessStatus.Processing,
                Type = type,
                UserID = userID,
                ID = Guid.NewGuid()
            };
            if (assesses == null || !assesses.Any())
            {
                if (_assessService.Add(assess)) result = ResponseResult.Success(assess.ID);
            }
            else
            {
                assess.ID = assesses.First().ID;
                if (_assessService.Update(assess)) result = ResponseResult.Success(assess.ID);
            }
            return result;
        }

        /// <summary>
        /// 提交选项
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ResponseResult> Update(UpdateRequest request)
        {
            var result = ResponseResult.Failed();
            var userID = User.Identity.GetId();
            var assesses = await _assessService.GetByUserID(userID, AssessStatus.Processing);
            if (assesses?.Any() == true)
            {
                var assess = assesses.First();
                var selectOptions = new Dictionary<int, dynamic>();
                if (!string.IsNullOrWhiteSpace(assess.SelectedOptionShortIDs))
                {
                    try
                    {
                        selectOptions = JsonConvert.DeserializeObject<Dictionary<int, dynamic>>(assess.SelectedOptionShortIDs);
                    }
                    catch
                    {
                    }
                }
                selectOptions.Remove(request.QuestionLevel);
                if (request.SelectType == AssessSelectType.TextBox)
                {
                    selectOptions.Add(request.QuestionLevel, request.Content);
                }
                else
                {
                    selectOptions.Add(request.QuestionLevel, request.OptionIDs);
                }
                assess.SelectedOptionShortIDs = JsonConvert.SerializeObject(selectOptions);
                if (_assessService.Update(assess)) result = ResponseResult.Success(assess.ID);
            }
            return result;
        }

        /// <summary>
        /// 结束测评
        /// </summary>
        /// <returns></returns>
        [Authorize]
#if DEBUG
        public async Task<ResponseResult> Finish(Guid? debugID)
        {
            var result = ResponseResult.Failed();
            var userID = User.Identity.GetId();
            var assesses = await _assessService.GetByUserID(userID, AssessStatus.Processing);
            if (debugID.HasValue && debugID.Value != Guid.Empty)
            {
                assesses = assesses.Prepend(_assessService.Get(debugID.Value));
            }
#else
        public async Task<ResponseResult> Finish(Guid? debugID)
        {
            var result = ResponseResult.Failed();
            var userID = User.Identity.GetId();
            var assesses = await _assessService.GetByUserID(userID, AssessStatus.Processing);
#endif
            if (assesses?.Any() == true)
            {
                var assess = assesses.First();
                assess.Status = AssessStatus.Finish;
                var talents = new List<(string, string)>();
#if DEBUG
                talents.Add(("2056E816-A554-4C10-A2DE-A9743502CDDC", "黄伊莎老师"));
                talents.Add(("B0B3C8FF-B3BA-4CCE-A24E-2FF446E0AA94", "心心老师"));
                talents.Add(("49FE82B3-5366-43CC-8ED0-E11CDC26B57F", "林二汶"));
                talents.Add(("38705AC6-CDF1-41B0-BFFC-B140742C14E9", "小叶"));
                talents.Add(("C89845E4-1EB8-404F-9D5F-B3BC627A17E9", "麦兜爸"));
#else

                talents.Add(("A0E962F9-B43E-4364-BA39-2916CB801D27", "麦兜爸"));
                talents.Add(("B2415D2D-7D43-494D-84B8-AB555387B46F", "林二汶"));
                talents.Add(("36781A83-B4E6-4CFF-BD64-88DDC8F9DCD9", "心心老师"));
                talents.Add(("99196FBC-0C77-45D8-ACC1-356994B7CF4A", "黄伊莎老师"));
                talents.Add(("3530FD26-6D04-44C4-A8ED-EF5D94E5FACA", "小叶"));
#endif
                var processingOrders = await _orderService.Page(1, 99, assess.UserID, new int[] { 0, 1, 2, 3 }, true);
                if (processingOrders?.Items?.Any() == true)
                {
                    var orderAnwserIDs = processingOrders.Items.Select(x => x.AnswerID.ToString().ToLower()).ToList();
                    var askingTalents = talents.Where(p => !orderAnwserIDs.Contains(p.Item1.ToLower()));
                    if (askingTalents?.Any() == true)
                    {
                        askingTalents = CommonHelper.ListRandom(askingTalents);
                        assess.RecommendTalentUserID = new Guid(askingTalents.First().Item1);
                    }
                }
                if (assess.RecommendTalentUserID == Guid.Empty)
                {
                    CommonHelper.ListRandom(talents);
                    assess.RecommendTalentUserID = new Guid(talents.First().Item1);
                }
                var dic_Options = new Dictionary<int, IEnumerable<Guid>>();
                var dic_dynamic = new Dictionary<int, dynamic>();
                try
                {
                    dic_dynamic = JsonConvert.DeserializeObject<Dictionary<int, dynamic>>(assess.SelectedOptionShortIDs);
                    foreach (var item in dic_dynamic)
                    {
                        try
                        {
                            dic_Options.Add(item.Key, JsonConvert.DeserializeObject<IEnumerable<Guid>>(item.Value.ToString()));
                        }
                        catch { }
                    }
                }
                catch { }
                if (dic_Options.Any())
                {
                    var optionIDs = new List<Guid>();
                    foreach (var item in dic_Options.Values)
                    {
                        if (item?.Any() == true) optionIDs.AddRange(item);
                    }
                    var options = await _assessOptionService.GetByIDs(optionIDs);
                    var query = new SearchSchoolQuery
                    {
                        PageSize = 3,
                        PageNo = 1,
                        Orderby = 5,
                        CityCode = new List<int>() { 440100 }
                    };
                    if (assess.Type == AssessType.Type2 || assess.Type == AssessType.Type3)
                    {
                        var areas = options.Where(p => dic_Options[2].Contains(p.ID)).Select(p => p.Content);
                        if (areas?.Any() == true)
                        {
                            var gzCityAreaCodes = await _cityInfoService.GetAreaCode(440100);//广州
                            var selectedAreaCodes = gzCityAreaCodes.Where(p => areas.Contains(p.AreaName + '区')).Select(p => p.AdCode);
                            if (selectedAreaCodes?.Any() == true) query.AreaCodes = selectedAreaCodes.ToList();
                        }

                        switch (assess.Type)
                        {
                            case AssessType.Type2:
                                query.Grade = new List<int>() { 2 };
                                break;
                            case AssessType.Type3:
                                query.Grade = new List<int>() { 3 };
                                break;
                        }

                        var lodgeTypes = options.Where(p => dic_Options[3].Contains(p.ID)).Select(p => p.Content);
                        if (lodgeTypes?.Any() == true)
                        {
                            switch (lodgeTypes.First())
                            {
                                case "走读":
                                    query.Lodging = 1;
                                    break;
                                case "住宿":
                                    query.Lodging = 2;
                                    break;
                                default:
                                    query.Lodging = 3;
                                    break;
                            }
                        }
                        var prices = options.Where(p => dic_Options[4].Contains(p.ID)).Select(p => p.Content);
                        if (prices?.Any() == true)
                        {
                            switch (prices.First())
                            {
                                case "1万以下":
                                    query.MaxCost = 9999;
                                    break;
                                case "1-5万":
                                    query.MinCost = 10000;
                                    query.MaxCost = 49999;
                                    break;
                                case "5-10万":
                                    query.MinCost = 50000;
                                    query.MaxCost = 99999;
                                    break;
                                case "10万以上":
                                    query.MinCost = 100000;
                                    break;
                            }
                        }
                        var list = _searchService.SearchSchool(query);
                        if (list?.Schools?.Any() == true)
                        {
                            assess.RecommendExtID = JsonConvert.SerializeObject(list.Schools.Select(p => p.Id));
                        }
                    }

                    if (assess.Type == AssessType.Type4)
                    {
                        query.Grade = new List<int>() { 4 };
                        var areas = options.Where(p => dic_Options[1].Contains(p.ID)).Select(p => p.Content);
                        if (areas?.Any() == true)
                        {
                            var gzCityAreaCodes = await _cityInfoService.GetAreaCode(440100);//广州
                            var selectedAreaCodes = gzCityAreaCodes.Where(p => areas.Contains(p.AreaName + '区')).Select(p => p.AdCode);
                            if (selectedAreaCodes?.Any() == true) query.AreaCodes = selectedAreaCodes.ToList();
                        }

                        var tags = _schoolService.GetTagList();

                        var countries = options.Where(p => dic_Options[2].Contains(p.ID)).Select(p => p.Content);
                        if (countries?.Any() == true)
                        {
                            //query.AbroadIds
                            var abroadTags = tags.Where(q => q.Type == 3).ToList();
                            query.AbroadIds = new List<Guid>();
                            foreach (var item in countries)
                            {
                                if (abroadTags.Any(p => p.Name == item))
                                {
                                    query.AbroadIds.Add(abroadTags.FirstOrDefault(p => p.Name == item).Id);
                                }
                            }
                        }

                        var courses = options.Where(p => dic_Options[2].Contains(p.ID)).Select(p => p.Content);
                        if (courses?.Any() == true)
                        {
                            var courseTags = tags.Where(p => p.Type == 6).ToList();
                            query.CourseIds = new List<Guid>();
                            foreach (var item in courses)
                            {
                                if (courseTags.Any(p => p.Name == item))
                                {
                                    query.CourseIds.Add(courseTags.FirstOrDefault(p => p.Name == item).Id);
                                }
                            }
                        }
                        var list = _searchService.SearchSchool(query);
                        if (list?.Schools?.Any() == true)
                        {
                            assess.RecommendExtID = JsonConvert.SerializeObject(list.Schools.Select(p => p.Id));
                        }
                    }
                }

                if (_assessService.Update(assess))
                {
                    await _mediator.Publish(new AssessStatisticsEvent(assess.Type, 2, assess.UserID));
                    result = ResponseResult.Success();
                }
            }
            return result;
        }

        /// <summary>
        /// 获取测评结果
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<ResponseResult> GetReport(Guid id)
        {
            var result = ResponseResult.Failed();
            var find = await _assessService.GetAsync(id);
            if (find?.ID != Guid.Empty)
            {
                var dic_Options = new Dictionary<int, IEnumerable<Guid>>();
                var dic_dynamic = new Dictionary<int, dynamic>();
                try
                {
                    dic_dynamic = JsonConvert.DeserializeObject<Dictionary<int, dynamic>>(find.SelectedOptionShortIDs);
                    foreach (var item in dic_dynamic)
                    {
                        try
                        {
                            dic_Options.Add(item.Key, JsonConvert.DeserializeObject<IEnumerable<Guid>>(item.Value.ToString()));
                        }
                        catch { }
                    }
                }
                catch { }
                var optionIDs = new List<Guid>();
                foreach (var item in dic_Options.Values)
                {
                    if (item?.Any() == true) optionIDs.AddRange(item);
                }
                var options = await _assessOptionService.GetByIDs(optionIDs);
                var questions = await _assessQuestionService.GetByType(find.Type);

                var response = new GetRportResponse()
                {
                    AssessType = find.Type
                };

                response.Questions = questions.OrderBy(p => p.Level).ToDictionary(k => k.Level, v => v.Content);
                response.Options = dic_dynamic.ToDictionary(k => k.Key, v =>
                {
                    var optionStrings = new List<string>();
                    try
                    {
                        IEnumerable<Guid> temp_OptionIDs = JsonConvert.DeserializeObject<IEnumerable<Guid>>(Convert.ToString(v.Value));
                        optionStrings.AddRange(options.Where(p => temp_OptionIDs.Contains(p.ID)).Select(p => p.Content));
                    }
                    catch
                    {
                        optionStrings.Add(v.Value);
                    }
                    return optionStrings;
                });
                if (find.RecommendTalentUserID != Guid.Empty) response.RecommendTalent = await _talentSettingService.GetDetail(find.RecommendTalentUserID);
                if (!string.IsNullOrWhiteSpace(find.RecommendExtID))
                {
                    IEnumerable<Guid> extIDs = null;
                    try
                    {
                        extIDs = JsonConvert.DeserializeObject<IEnumerable<Guid>>(find.RecommendExtID);
                    }
                    catch { }
                    if (extIDs?.Any() == true)
                    {
                        var latitude = Request.GetLatitude();
                        var longitude = Request.GetLongitude();
                        var exts = await _schoolService.ListExtSchoolByBranchIds(extIDs.ToList(), Convert.ToDouble(latitude), Convert.ToDouble(longitude));
                        if (exts?.Any() == true)
                        {
                            response.RecommendSchools = exts.Select(ext => new SchoolExtItemDto()
                            {
                                Name = ext.Name,
                                Grade = ext.Grade,
                                Type = (byte)ext.Type,
                                Area = ext.Area,
                                AreaCode = ext.AreaCode,
                                City = ext.City,
                                CityCode = ext.CityCode,
                                Province = ext.Province,
                                ProvinceCode = ext.ProvinceCode,
                                Score = ext.Score,
                                Distance = ext.Distance,
                                ExtId = ext.ExtId,
                                Sid = ext.Sid,
                                Lodging = ext.Lodging,
                                Sdextern = ext.Sdextern,
                                Tags = ext.Tags,
                                Tuition = ext.Tuition,
                                SchoolNo = ext.SchoolNo
                            }).OrderByDescending(p => p.Score);
                        }
                    }


                }
                result = ResponseResult.Success(response);
            }
            return result;
        }

        /// <summary>
        /// 生成测评每日统计
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseResult> GenerateReportByDay()
        {
            var result = ResponseResult.Failed();
            var finds = await _assessStatisticsService.GetUnCountStatistics();
            if (finds?.Any() == true)
            {
                var date = DateTime.Now.Date;
                foreach (var find in finds)
                {
                    if (find.CountTime.Date == DateTime.Now.Date) continue;

                    var assesses = await _assessService.GetByCountingTime(find.CountTime);
                    if (assesses?.Any() == true)
                    {
                        find.FirstPagePopoutPercent = ((decimal)assesses.Count(p => string.IsNullOrWhiteSpace(p.SelectedOptionShortIDs))) / ((int)assesses.Count());
                    }
                    var orders = await _orderService.GetByTime(find.CountTime.Date, find.CountTime.AddDays(1).Date);
                    if (orders?.Any() == true)
                    {
                        find.PayCount = orders.Count(p => !string.IsNullOrWhiteSpace(p.OriginType) && p.OriginType.ToLower().StartsWith($"assess_{(int)find.AssessType}") && p.Status > OrderStatus.WaitingAsk && p.Status < OrderStatus.Transfered);
                    }

                    await _easyRedisClient.HashDeleteAsync($"AssessStatistics:{find.CountTime.ToString("yyyy-MM-dd")}:AssessType_{(int)find.AssessType}", "Entity");
                    find.IsCounted = true;
                    await _assessStatisticsService.UpdateAsync(find);
                }
                result = ResponseResult.Success();
            }
            return result;
        }
    }
}

