using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PMS.PaidQA.Application.Services;
using PMS.PaidQA.Domain.EntityExtend;
using PMS.School.Application.IServices;
using PMS.School.Domain.Common;
using PMS.School.Domain.Dtos;
using PMS.UserManage.Application.IServices;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using Sxb.Web;
using Sxb.Web.Areas.School.Models;
using Sxb.Web.Areas.School.Models.School;
using Sxb.Web.Common;
using Sxb.Web.Controllers;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sxb.Web.Authentication.Attribute;

namespace Sxb.Web.Areas.School
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SchoolController : ApiBaseController
    {
        ISchoolScoreService _schoolScoreService;
        ISchoolService _schoolService;
        IUserService _userService;
        ITalentSettingService _talentSettingService;
        IOrderService _orderService;
        IEasyRedisClient _easyRedisClient;
        ISchService _schService;
        IHotTypeService _hotTypeService;

        public SchoolController(ISchoolService schoolService, ISchoolScoreService schoolScoreService, IUserService userService, ITalentSettingService talentSettingService, IOrderService orderService,
            IEasyRedisClient easyRedisClient, ISchService schService, IHotTypeService hotTypeService)
        {
            _hotTypeService = hotTypeService;
            _schService = schService;
            _easyRedisClient = easyRedisClient;
            _orderService = orderService;
            _talentSettingService = talentSettingService;
            _userService = userService;
            _schoolService = schoolService;
            _schoolScoreService = schoolScoreService;
        }


        [HttpGet]
        [AuthToken]
        public ResponseResult TestGet0()
        {
            return ResponseResult.Success();
        }

        [HttpGet]
        [AuthToken]
        public ResponseResult TestGet(string name, string a)
        {
            return ResponseResult.Success();
        }

        [HttpPost]
        [AuthToken]
        public ResponseResult TestPost([FromBody] GetUpgradeReportRequest getUpgradeReportRequest)
        {
            return ResponseResult.Success();
        }


        /// <summary>
        /// 升学价值分析报告
        /// </summary>
        /// <param name="grade">1.幼|2.小|3.初|4.高</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ResponseResult> GetUpgradeReport(GetUpgradeReportRequest getReportRequest)
        {
            var result = ResponseResult.Failed();
            var userID = User.Identity.GetId();
            var redisKey = $"UpgradeReportShareCheck:{userID}_{getReportRequest.EID}";
            if (!await _easyRedisClient.ExistsAsync(redisKey))
            {
                result.Msg = "need share";
                return result;
            }

            var schoolInfo = await _schoolService.GetSchoolExtensionDetails(getReportRequest.EID);
            if (schoolInfo == null || schoolInfo.ExtId == Guid.Empty) return result;
            var response = new GetUpgradeReportResponse()
            {
                SchoolImgUrl = schoolInfo.SchoolImageUrl,
                ExtName = schoolInfo.ExtName,
                SchoolName = schoolInfo.SchoolName
            };

            var indexs = await _schoolService.schoolExtScoreIndexsAsync();
            var scores = await _schoolService.GetSchoolValidExtScoreAsync(schoolInfo.ExtId);
            var areaAvgScore = await _schoolScoreService.GetAvgScoreByAreaCode(schoolInfo.Area, schoolInfo.SchFType.Code);//区域平均分
            //var cityAvgScore = await _schoolScoreService.GetAvgScoreByCityCode(schoolInfo.City, schoolInfo.SchFType.Code);//市区平均分
            var avgScore = await _schoolScoreService.GetAvgScore(schoolInfo.SchFType.Code);//全国平均分
            var schoolType = new iSchool.SchFType0(schoolInfo.SchFType.Grade, schoolInfo.SchFType.Type, schoolInfo.SchFType.Discount, schoolInfo.SchFType.Diglossia, schoolInfo.SchFType.Chinese).GetDesc();
            //response.Talent = await GetRecommendTalentUserID(schoolType, User.Identity.GetId());  //推荐达人
            response.Talent = await _hotTypeService.GetRecommendTalentBySchFTypeCode(schoolInfo.SchFType.Code, User.Identity.GetId());
            //标签相关

            if (!string.IsNullOrWhiteSpace(schoolType) || schoolInfo.Tags?.Any() == true)
            {
                response.Tags = new List<string>();
                if (!string.IsNullOrWhiteSpace(schoolType)) response.Tags.Add(schoolType);
                if (schoolInfo.Tags?.Any() == true)
                {
                    if (response.Tags.Count() > 0)
                    {
                        response.Tags.Add(schoolInfo.Tags.FirstOrDefault());
                    }
                    else
                    {
                        response.Tags.AddRange(schoolInfo.Tags.Take(2));
                    }
                }
            }
            //分数相关
            if (scores?.Any() == true)
            {
                if (scores.Any(p => p.IndexId == 22))
                {
                    var schoolTotalScore = scores.FirstOrDefault(p => p.IndexId == 22)?.Score ?? 0;
                    response.SchoolTotalScore = (schoolTotalScore + 0.0) / 10;//学校总评分
                    response.Str_Score = schoolTotalScore.GetScoreString();
                    response.SchoolTotalScoreRanking = await _schoolScoreService.GetSchoolRankingInCity(schoolInfo.City, schoolTotalScore, schoolInfo.SchFType.Code);
                    if (response.SchoolTotalScoreRanking == 0) response.SchoolTotalScoreRanking = 0.01;
                    response.TeachingLevel = scores.Where(p => p.IndexId > 14 && p.IndexId < 18).Average(p => p.Score).GetValueOrDefault().CutDoubleWithN(1);
                    response.ScoreLevel = GetScoreLevel(schoolTotalScore);
                    //var findCount = await _schoolScoreService.GetSchoolCountByCityAndSchFType(schoolInfo.City, schoolInfo.SchFType.Code);
                    var areaSchoolCount = await _schoolScoreService.GetSchoolCountByAreaAndSchFType(schoolInfo.Area, schoolInfo.SchFType.Code);
                    var scoreRange = GetScoreRange(response.ScoreLevel);
                    var scoreRangeSchoolCount = await _schoolScoreService.GetSchoolCount(schoolInfo.SchFType.Code, schoolInfo.Area, scoreRange.Item1, scoreRange.Item2);
                    var gtScoreSchoolCount = await _schoolScoreService.GetSchoolCount(schoolInfo.SchFType.Code, schoolInfo.Area, scoreRange.Item2);
                    response.SameScoreSchoolCountPercent = (scoreRangeSchoolCount / (areaSchoolCount + 0.0)).CutDoubleWithN(2);
                    response.HigherScoreSchoolCountPercent = (gtScoreSchoolCount / (areaSchoolCount + 0.0)).CutDoubleWithN(2);
                    var qualification = (scores.FirstOrDefault(p => p.IndexId == 1).Score ?? 0) / avgScore.FirstOrDefault(p => p.Key == 1).Value;
                    if (qualification > 1.15) { response.Qualification = 2; } else if (qualification < 0.85) { response.Qualification = 0; }
                    var qualificationArea = areaAvgScore.FirstOrDefault(p => p.Key == 1).Value / avgScore.FirstOrDefault(p => p.Key == 1).Value;
                    if (qualificationArea > 1.15) { response.QualificationArea = 2; } else if (qualificationArea < 0.85) { response.QualificationArea = 0; }
                    var teachers = (scores.FirstOrDefault(p => p.IndexId == 16).Score ?? 0) / avgScore.FirstOrDefault(p => p.Key == 16).Value;
                    if (teachers > 1.15) { response.Teachers = 2; } else if (teachers < 0.85) { response.Teachers = 0; }
                    var teachersArea = areaAvgScore.FirstOrDefault(p => p.Key == 16).Value / avgScore.FirstOrDefault(p => p.Key == 16).Value;
                    if (teachersArea > 1.15) { response.TeachersArea = 2; } else if (teachersArea < 0.85) { response.TeachersArea = 0; }
                    var hardware = (scores.FirstOrDefault(p => p.IndexId == 17).Score ?? 0) / avgScore.FirstOrDefault(p => p.Key == 17).Value;
                    if (hardware > 1.15) { response.Hardware = 2; } else if (hardware < 0.85) { response.Hardware = 0; }
                    var hardwareArea = areaAvgScore.FirstOrDefault(p => p.Key == 17).Value / avgScore.FirstOrDefault(p => p.Key == 17).Value;
                    if (hardwareArea > 1.15) { response.HardwareArea = 2; } else if (hardwareArea < 0.85) { response.HardwareArea = 0; }

                    response.StudentSourceQuality = (scores.FirstOrDefault(p => p.IndexId == 24)?.Score / 10)?.CutDoubleWithN(1) ?? 0;

                    response.SchoolSurroundInfo = await _schoolService.GetSurroundInfo(schoolInfo.ExtId);
                    response.SchoolSurroundAvgInfo = await _schoolService.GetSurroundAvgInfo(schoolInfo.SchFType.Code, schoolInfo.Area);
                    response.SchoolOtherInfo = await _schoolService.GetAiParams(schoolInfo.ExtId);
                    response.SchoolOtherAvgInfo = await _schoolService.GetAiParamsAvg(schoolInfo.SchFType.Code, schoolInfo.Area);

                    response.Peripheral = (scores.FirstOrDefault(p => p.IndexId == 21)?.Score / 10)?.CutDoubleWithN(1) ?? 0;
                    response.PeripheralAreaAvg = areaAvgScore.FirstOrDefault(p => p.Key == 21).Value.CutDoubleWithN(2);
                    response.ComfortPercent = await _schoolScoreService.GetLowerPercent(schoolInfo.SchFType.Code, schoolInfo.Area, 5, scores.FirstOrDefault(p => p.IndexId == 5)?.Score ?? 0);
                    response.ConveniencePercent = await _schoolScoreService.GetLowerPercent(schoolInfo.SchFType.Code, schoolInfo.Area, 6, scores.FirstOrDefault(p => p.IndexId == 6)?.Score ?? 0);
                    response.SecurityPercent = await _schoolScoreService.GetLowerPercent(schoolInfo.SchFType.Code, schoolInfo.Area, 7, scores.FirstOrDefault(p => p.IndexId == 7)?.Score ?? 0);
                    response.CulturalPercent = await _schoolScoreService.GetLowerPercent(schoolInfo.SchFType.Code, schoolInfo.Area, 29, scores.FirstOrDefault(p => p.IndexId == 29)?.Score ?? 0);

                    response.Heat = (scores.FirstOrDefault(p => p.IndexId == 25)?.Score / 10)?.CutDoubleWithN(1) ?? 0;
                    response.HeatArea = areaAvgScore.FirstOrDefault(p => p.Key == 25).Value / 10;
                    response.HeatPercent = await _schoolScoreService.GetLowerPercent(schoolInfo.SchFType.Code, schoolInfo.Area, 25, scores.FirstOrDefault(p => p.IndexId == 25)?.Score ?? 0);
                    response.SchoolHeatArea = areaAvgScore.FirstOrDefault(p => p.Key == 27).Value / 10;
                    response.SchoolHeatPercent = await _schoolScoreService.GetLowerPercent(schoolInfo.SchFType.Code, schoolInfo.Area, 27, scores.FirstOrDefault(p => p.IndexId == 27)?.Score ?? 0);

                    #region 数据处理
                    if (response.StudentSourceQuality == 0) response.StudentSourceQuality = (areaAvgScore.GetValueOrDefault(23) / 10).CutDoubleWithN(1);
                    if (response.Heat == 0) response.Heat = (areaAvgScore.GetValueOrDefault(25) / 10).CutDoubleWithN(1);
                    #endregion

                }
            }

            result = ResponseResult.Success(response);
            return result;
        }

        int GetScoreLevel(double score)
        {
            if (score >= 90)
            {
                return 1;
            }
            else if (score >= 80 && score < 90)
            {
                return 2;
            }
            else if (score >= 70 && score < 80)
            {
                return 3;
            }
            else if (score >= 60 && score < 70)
            {
                return 4;
            }
            else
            {
                return 5;
            }
        }

        (int, int) GetScoreRange(int level)
        {
            switch (level)
            {
                case 1:
                    return (90, 151);
                case 2:
                    return (80, 90);
                case 3:
                    return (70, 80);
                case 4:
                    return (60, 70);
                case 5:
                    return (0, 60);
                default:
                    return (0, 0);
            }
        }

        public async Task<TalentDetailExtend> GetRecommendTalentUserID(string schoolType, Guid userID)
        {
            IEnumerable<string> selectNicknames = null;
            switch (schoolType)
            {
                case "国际幼儿园":
                    selectNicknames = new List<string>()
                    {
                        "心心老师","晨晨曦曦升学记","灵灵老师"
                    };
                    break;
                case "民办普惠幼儿园":
                case "普通民办幼儿园":
                case "公办幼儿园":
                    selectNicknames = new List<string>()
                    {
                        "心心老师","时爸说校","灵灵老师"
                    };
                    break;
                case "幼托机构":
                    selectNicknames = new List<string>()
                    {
                        "心心老师","晨晨曦曦升学记","灵灵老师"
                    };
                    break;
                case "外国人小学":
                    selectNicknames = new List<string>()
                    {
                        "万能妈妈","黄伊莎老师","玉子老师"
                    };
                    break;
                case "双语小学":
                    selectNicknames = new List<string>()
                    {
                        "文文妈妈","黄伊莎老师","秦奋斗访校"
                    };
                    break;
                case "普通民办小学":
                    selectNicknames = new List<string>()
                    {
                        "万能妈妈","黄伊莎老师","秦奋斗访校"
                    };
                    break;
                case "公办小学":
                    selectNicknames = new List<string>()
                    {
                        "文文妈妈","玉子老师","秦奋斗访校"
                    };
                    break;
                case "外国人初中":
                case "双语初中":
                case "普通民办初中":
                case "公办初中":
                    selectNicknames = new List<string>()
                    {
                        "老四妈妈","小叶","羊城老绵羊"
                    };
                    break;
                case "国际高中":
                    selectNicknames = new List<string>()
                    {
                        "Fiona Z","晓老师晓数据","皮特·潘唠小道"
                    };
                    break;
                case "外国人高中":
                    selectNicknames = new List<string>()
                    {
                        "Fiona Z","教育密探飞云叔","另类升学找苏sue"
                    };
                    break;
                case "双语高中":
                    selectNicknames = new List<string>()
            {
                        "Yutino","晓老师晓数据","皮特·潘唠小道"
                    };
                    break;
                case "普通民办高中":
                    selectNicknames = new List<string>()
                {
                        "Yutino","教育密探飞云叔","另类升学找苏sue"
                    };
                    break;
                case "公办高中":
                    selectNicknames = new List<string>()
                    {
                        "Yutino","晓老师晓数据","皮特·潘唠小道"
                    };
                    break;
            }

            if (selectNicknames?.Any() == true)
            {
                var userInfos = await _userService.GetUserInfosByNames(selectNicknames);
                if (userInfos?.Any() == true)
                {
                    userInfos = userInfos.Where(p => selectNicknames.Contains(p.NickName));

                    var talents = await _talentSettingService.GetDetails(userInfos.Select(p => p.Id).Distinct());

                    var processingOrders = await _orderService.Page(1, 99, userID, new int[] { 0, 1, 2, 3 }, true);
                    if (processingOrders?.Items?.Any() == true)
                    {
                        var orderAnwserIDs = processingOrders.Items.Select(x => x.AnswerID).ToList();
                        var lessUsers = talents.Where(p => !orderAnwserIDs.Contains(p.TalentUserID));
                        if (lessUsers?.Any() == true)
                        {
                            lessUsers = CommonHelper.ListRandom(lessUsers);
                            return lessUsers.First();
                        }
                    }
                    talents = CommonHelper.ListRandom(talents);
                    return talents.First();
                }
            }

            return null;
        }

        /// <summary>
        /// 升学价值报告是否分享检查
        /// </summary>
        /// <param name="eid"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<ResponseResult> CheckUpgradeReportShare(CheckUpgradeReportShareRequest request)
        {
            var result = ResponseResult.Failed();
            if (request.EID == Guid.Empty) return result;
            var userID = User.Identity.GetId();
            var redisKey = $"UpgradeReportShareCheck:{userID}_{request.EID}";
            if (request.Token?.Length == 16)
            {
                result = ResponseResult.Success(_easyRedisClient.AddAsync(redisKey, 1, TimeSpan.FromDays(365)).Result);
            }
            else
            {
                result = ResponseResult.Success(await _easyRedisClient.ExistsAsync(redisKey));
            }


            return result;
        }

        /// <summary>
        /// 获取学校,根据总体评分倒序
        /// </summary>
        /// <param name="grade">1.幼|2.小|3.初|4.高</param>
        /// <returns></returns>
        public async Task<ResponseResult> PageScoreSortSchool(int pageIndex = 1, int pageSize = 12, int grade = 0)
        {
            var result = ResponseResult.Failed();
            var cityCode = Request.GetLocalCity();
            var finds = new SchoolExtListDto();
            var grades = new int[1] { grade };
            var dataCount_Grade = new Dictionary<int, int>();
            if (grade != 0)
            {
                finds = await _schoolService.GetSchoolExtListAsync(Convert.ToDouble(Request.GetLongitude()), Convert.ToDouble(Request.GetLatitude()), 0, cityCode, 0,
                new int[] { grade }, orderBy: 1, new int[] { }, 0, new int[] { }, pageIndex, pageSize);
                if (finds?.PageCount > 0) dataCount_Grade.Add(grade, finds.PageCount);
            }
            else
            {
                finds.List = new List<SchoolExtItemDto>();
                for (int i = 1; i < 5; i++)
                {
                    var find = await _schoolService.GetSchoolExtListAsync(Convert.ToDouble(Request.GetLongitude()), Convert.ToDouble(Request.GetLatitude()), 0, cityCode, 0,
                new int[] { i }, orderBy: 1, new int[] { }, 0, new int[] { }, pageIndex, pageSize);
                    if (find?.List?.Any() == true)
                    {
                        dataCount_Grade.Add(i, find.PageCount);
                        finds.List.AddRange(find.List);
                    }
                }

            }

            if (finds?.List?.Any() == true)
            {
                foreach (var item in finds.List)
                {
                    item.Tags = new List<string>()
                    {
                        ((SchoolType)item.Type).Description()
                    };

                    if (!string.IsNullOrWhiteSpace(item.Authentication))
                    {
                        try
                        {
                            var authes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<KeyValueDto<string>>>(item.Authentication);
                            if (authes.Count > 0)
                            {
                                var tagCount = authes.Count(p => p.Key != "未收录" && p.Key.Count() <= 8);
                                if (tagCount > 0)
                                {
                                    item.Tags.AddRange(authes.Select(p => p.Key));
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }

                result = ResponseResult.Success(
                     finds.List.Select(p => p.Grade).Distinct().OrderBy(p => p).Select(p => new
                     {
                         Grade = p,
                         Total = dataCount_Grade.GetValueOrDefault(p),
                         Items = finds.List.Where(x => x.Grade == p).OrderByDescending(x => x.Score).Select(x => new
                         {
                             x.Grade,
                             Name = x.Name,
                             Str_Score = Codstring.GetScoreString(x.Score.Value),
                             City = x.City,
                             Area = x.Area,
                             Tags = x.Tags,
                             Tuition = x.Tuition.HasValue ? (x.Tuition >= 10000 ? Math.Round((decimal)(x.Tuition / 10000), 2) + "万元/年" : x.Tuition.ToString() + "元/年") : "义务教育免学费",
                             Url = $"/school-{x.ShortSchoolNo}/"
                         })
                     })
                );
            }


            return result;
        }

        /// <summary>
        /// 获取高中升学成绩
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <returns></returns>
        [AuthToken]
        public async Task<ResponseResult> GetHighSchoolAchievement(Guid eid)
        {
            var result = ResponseResult.Failed("No result");

            var data = _schoolService.GetAchievementList(eid, (byte)SchoolGrade.SeniorMiddleSchool);
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
            }
            var resultData = new GetHighSchoolAchievementResponse();

            if (data?.Any() == true)
            {
                var years = data.Select(p => p.Key).Distinct().OrderByDescending(p => p);
                var pdata = new List<dynamic>();
                foreach (var year in years)
                {
                    pdata.Add(new
                    {
                        Year = year,
                        List = data.Where(p => p.Key == year).OrderByDescending(p => p.Message).Select(p => new
                        {
                            SchoolID = p.Other,
                            SchoolName = p.Value,
                            Count = p.Message
                        })
                    });
                }
                resultData.Local = pdata;
            }
            if (rank?.Any() == true) resultData.OutSide = rank;
            if (resultData.Local?.Any() == true || resultData.OutSide?.Any() == true) result = ResponseResult.Success(resultData);
            return result;
        }

        /// <summary>
        /// 获取对口学校
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        [AuthToken]
        public async Task<ResponseResult> GetCounterPartInfo(Guid eid)
        {
            var result = ResponseResult.Failed("No result");
            var finds = await _schoolService.GetCounterPartByEID(eid);
            if (finds?.Any() == true)
            {
                var eids = finds.Select(p => p.Value).Distinct();
                var grades = await _schoolService.GetGradesByEIDs(eids);
                var cityArea = await _schoolService.GetCityAndAreaByEIDs(eids);
                var resultData = finds.Select(p => new
                {
                    Name = p.Key,
                    Grade = grades.FirstOrDefault().Value.GetDescription(),
                    CityCode = cityArea.FirstOrDefault().Value.Item1,
                    AreaCode = cityArea.FirstOrDefault().Value.Item2
                });
                result = ResponseResult.Success(resultData);
            }
            return result;
        }

        /// <summary>
        /// 获取学校附近3公里学位房
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        [AuthToken]
        public async Task<ResponseResult> GetSurroundInfo(Guid eid)
        {
            var result = ResponseResult.Failed("No result");
            var schoolInfo = _schoolService.GetSchoolDetailById(eid);
            if (schoolInfo == null) return result;

            var finds = await _schoolService.GetBuildingPriceDataAsync(eid, schoolInfo.Latitude, schoolInfo.Longitude);
            if (finds?.Any() == true)
            {
                var resultData = new List<KeyValueDto<double, double, string, int>>();
                foreach (var item in finds)
                {
                    if (!resultData.Any(p => p.Data == item.Data && p.Key == item.Key && p.Message == item.Message && p.Value == item.Value))
                    {
                        resultData.Add(item);
                    }
                }
                result = ResponseResult.Success(resultData.Select(p => new
                {
                    Longitude = p.Key,
                    Latitude = p.Value,
                    Name = p.Message,
                    Price = p.Data
                }));
            }
            return result;
        }
    }
}