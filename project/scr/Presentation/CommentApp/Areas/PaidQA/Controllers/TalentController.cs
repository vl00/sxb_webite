using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PMS.PaidQA.Application.Services;
using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Repository;
using PMS.UserManage.Application.IServices;
using Sxb.Web.Areas.PaidQA.Models.Talent;
using Sxb.Web.Controllers;
using Sxb.Web.Filters;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using ProductManagement.Framework.Foundation;
using AutoMapper;
using Sxb.Web.Areas.Common.Models;
using PMS.PaidQA.Domain.Enums;
using Sxb.Web.Areas.Common.WeChatQRCallBackHandle;
using Sxb.Web.Areas.Coupon.Models.Coupon;
using PMS.School.Application.IServices;
using Sxb.Web.Common;
using PMS.School.Domain.Dtos;
using IMessageService = PMS.PaidQA.Application.Services.IMessageService;
using PMS.OperationPlateform.Application.IServices;

namespace Sxb.Web.Areas.PaidQA.Controllers
{
    [Route("PaidQA/[controller]/[action]")]
    [ApiController]
    public class TalentController : ApiBaseController
    {
        IOrderService _orderService;
        ITalentService _talentService;
        ITalentSettingService _talentSettingService;
        ICommonSettingService _commonSettingService;
        IGradeService _gradeService;
        IRegionTypeService _regionTypeService;
        ICollectionService _collectionService;
        IRecommentTalentService _recommentTalentService;
        IMapper _mapper;
        ITalentRecordDataRelationService _talentRecordDataRelationService;
        ICouponTakeService _couponTakeService;
        ISchoolService _schoolService;
        IHotTypeService _hotTypeService;
        IMessageService _messageService;
        IArticleService _articleService;
        public TalentController(ITalentSettingService talentSettingService
            , ICommonSettingService commonSettingService
            , IGradeService gradeService
            , IRecommentTalentService recommentTalentService
            , IRegionTypeService regionTypeService
            , ICollectionService collectionService
            , IMapper mapper
            , ITalentService talentService
            , IOrderService orderService
            , ITalentRecordDataRelationService talentRecordDataRelationService
            , ICouponTakeService couponTakeService
            , ISchoolService schoolService
            , IHotTypeService hotTypeService
            , IMessageService messageService
            , IArticleService articleService)
        {
            _talentService = talentService;
            _collectionService = collectionService;
            _talentSettingService = talentSettingService;
            _commonSettingService = commonSettingService;
            _gradeService = gradeService;
            _regionTypeService = regionTypeService;
            _recommentTalentService = recommentTalentService;
            _mapper = mapper;
            _orderService = orderService;
            _talentRecordDataRelationService = talentRecordDataRelationService;
            _couponTakeService = couponTakeService;
            _schoolService = schoolService;
            _hotTypeService = hotTypeService;
            _messageService = messageService;
            _articleService = articleService;
        }


        [HttpGet]
        [Description("获取专家设置")]
        [Authorize]
        [ValidateAccoutBind]
        [ValidateFWHSubscribe(QRType = SubscribeQRCodeType.EnablePaidQA)]
        public async Task<ResponseResult> GetSetting()
        {
            var result = ResponseResult.Success();
            var find = await _talentSettingService.GetDetail(UserId.Value);
            if (find != null)
            {
                var priceCommonSetting = _commonSettingService.GetByKeyLikes("TalentPriceM");
                result.Data = new GetSettingResponse()
                {
                    GradeIDs = find.TalentGrades?.Select(p => p.ID),
                    IsEnable = find.IsEnable,
                    Price = find.Price.CutDecimalWithN(2).ToString(),
                    RegionTypeIDs = find.TalentRegions.Select(p => p.ID),
                    PriceMax = priceCommonSetting?.FirstOrDefault(p => p.Key == "TalentPriceMax")?.Value,
                    PriceMin = priceCommonSetting?.FirstOrDefault(p => p.Key == "TalentPriceMin")?.Value,
                    TalentType = find.TalentType
                };
            }
            return result;
        }

        [HttpPost]
        [Description("更新专家设置")]
        [Authorize]
        [ValidateAccoutBind]
        [ValidateFWHSubscribe(QRType = SubscribeQRCodeType.EnablePaidQA)]
        public async Task<ResponseResult> SaveSetting([FromBody] SaveSettingRequest request)
        {

            var result = ResponseResult.Success();
            var find = await _talentSettingService.GetByTalentUserID(UserId.Value);
            if (find != null)
            {
                var priceCommonSetting = _commonSettingService.GetByKeyLikes("TalentPriceM");
                var minPrice = priceCommonSetting?.FirstOrDefault(p => p.Key == "TalentPriceMin")?.Value;
                var maxPrice = priceCommonSetting?.FirstOrDefault(p => p.Key == "TalentPriceMax")?.Value;
                if (decimal.TryParse(minPrice, out decimal min) && decimal.TryParse(maxPrice, out decimal max))
                {
                    if (request.Price < min || request.Price > max)
                    {
                        return ResponseResult.Failed("Price range error");
                    }
                }
                else
                {
                    ResponseResult.Failed("CommonSetting error");
                }
                if (!request.IsEnable)
                {
                    //如果专家选择关闭上学问，那么需要检查是否有进行中的订单
                    var isHasProcessingOrder = await _orderService.IsHasProcessingOrder(find.TalentUserID);
                    if (isHasProcessingOrder)
                    {
                        return ResponseResult.Failed("检测到您当前有正在进行的订单，暂不可以停用上学问功能。请您结束订单后再停用。");
                    }
                }
                find.IsEnable = request.IsEnable;
                find.Price = request.Price;
                if (find.TalentLevelTypeID == Guid.Empty)
                {
                    var levels = await _talentSettingService.GetAllTalentLevels();
                    if (levels?.Any() == true)
                    {
                        find.TalentLevelTypeID = levels.OrderBy(p => p.Sort).First().ID;
                    }
                }
                if (_talentSettingService.Update(find))
                {
                    var removeResult = _regionTypeService.RemoveTalentRegions(UserId.Value).Result;
                    if (request.RegionTypeIDs?.Any() == true)
                    {
                        await _regionTypeService.AddTalentRegions(UserId.Value, request.RegionTypeIDs.Distinct());
                    }

                    _gradeService.RemoveTalentGrades(UserId.Value);
                    if (request.GradeIDs?.Any() == true)
                    {
                        _gradeService.AddTalentGrades(UserId.Value, request.GradeIDs.Distinct());
                    }
                }
            }
            else
            {
                result = ResponseResult.Failed("can't find the talent");
            }
            return result;
        }

        [HttpGet]
        [Description("专家详情")]
        public async Task<ResponseResult> GetDetail([FromQuery] GetDetailRequest request)
        {
            var result = ResponseResult.Success();

            //if (request.TalentID.HasValue)
            //{
            //    var talent = _talentService.GetTalentDetail(request.TalentID.Value.ToString());
            //    if (talent != null) request.ID = talent.user_id;
            //}
            //else
            //{
            //    var talent = _talentService.GetTalentDetail(request.ID.ToString());
            //    if (talent != null) request.ID = talent.user_id;
            //}


            var find = await _talentSettingService.GetDetail(request.ID);
            if (find != null)
            {
                var isFollowed = false;
                await Task.Run(() =>
                {
                    if (User.Identity.IsAuthenticated && UserId.Value != request.ID)
                    {
                        isFollowed = _collectionService.IsCollected(UserId.Value, request.ID);
                    }
                });

                GetDetailResponse detailResult = new GetDetailResponse()
                {
                    AuthName = find.AuthName,
                    GradeNames = find.TalentGrades?.OrderBy(p => p.Sort).Select(p => p.Name),
                    Introduction = find.TelentIntroduction,
                    LevelName = find.TalentLevelName,
                    NickName = find.NickName,
                    RegionTypeNames = find.TalentRegions?.Where(p => p.IsValid == true).Select(p => p.Name),
                    ReplyPercent = find.SixHourReplyPercent.ToString("f2"),
                    Score = find.AvgScore.ToString(),
                    UserID = find.TalentUserID,
                    HeadImgUrl = find.HeadImgUrl,
                    IsFollowed = isFollowed,
                    Price = find.Price.CutDecimalWithN(2).ToString(),
                    IsEnable = find.IsEnable,
                    TalentType = find.TalentType,
                    Covers = find.Covers,
                };
                var couponRules = OrderController.GetCouponRules(find);
                var coupons = await _couponTakeService.GetWaitUseCoupons(UserIdOrDefault, find.Price, couponRules);
                if (coupons.CanUse.Any())
                {
                    var bestCoupons = await _couponTakeService.GetBestCoupons(UserIdOrDefault, find.Price, OrderController.minPayAmount, couponRules);
                    coupons.CanUse = coupons.CanUse
                        .OrderByDescending(ctd => _couponTakeService.ComputeCouponAmmount(ctd.CouponInfo, find.Price, OrderController.minPayAmount))
                        .ThenBy(ctd => ctd.VaildEndTime)
                        .ToList();
                    detailResult.BestCoupons = _mapper.Map<List<CouponTakeResult>>(bestCoupons.BestCoupons);
                    detailResult.BestCouponAmmount = bestCoupons.CouponAmmount;
                    detailResult.CanUseCoupons = _mapper.Map<List<CouponTakeResult>>(coupons.CanUse);
                }
                result.Data = detailResult;

                if (find.SchoolExtId.HasValue)
                {
                    //关联学校处理
                    var latitude = Request.GetLatitude();
                    var longitude = Request.GetLongitude();
                    detailResult.School = _mapper.Map<SchoolResult>(await _schoolService.GetSchoolExtDtoAsync(find.SchoolExtId.Value, Convert.ToDouble(latitude), Convert.ToDouble(longitude)));
                    if (detailResult.School != null)
                    {
                        var schoolImages = await _schoolService.GetSchoolImages(detailResult.School.ExtId, null);
                        if (schoolImages?.Any() == true)
                        {
                            var images = schoolImages.OrderBy(p => p.Type).ToList();
                            detailResult.School.SchoolPhotos = new List<KeyValueDto<string, string, string>>();
                            detailResult.School.SchoolPhotos.AddRange(images.Where(p => p.Type == PMS.School.Domain.Enum.SchoolImageType.SchoolHonor).OrderBy(p => p.Sort).
                                Select(p => new KeyValueDto<string, string, string>()
                                {
                                    Key = p.Url,
                                    Value = p.ImageDesc,
                                    Message = p.SUrl
                                }));
                            if (detailResult.School.SchoolPhotos.Count < 3)
                            {
                                detailResult.School.SchoolPhotos.AddRange(images.Where(p => p.Type != PMS.School.Domain.Enum.SchoolImageType.SchoolHonor &&
                                p.Type != PMS.School.Domain.Enum.SchoolImageType.SchoolBrand).OrderBy(p => p.Type).ThenBy(p => p.Sort).
                                    Select(p => new KeyValueDto<string, string, string>()
                                    {
                                        Key = p.Url,
                                        Value = p.ImageDesc,
                                        Message = p.SUrl
                                    }));
                            }
                            if (detailResult.School.SchoolPhotos.Any())
                            {
                                detailResult.School.SchoolPhotos = detailResult.School.SchoolPhotos.Take(3).ToList();
                            }
                        }

                        detailResult.School.SchoolVideos = await _schoolService.GetSchoolVideos(detailResult.School.ExtId);
                        var schoolBrandImages = schoolImages.Where(s => s.Type == PMS.School.Domain.Enum.SchoolImageType.SchoolBrand)?.OrderBy(p => p.Sort).Take(8);
                        if (!schoolBrandImages.Any())
                        {
                            var hardwareImage = schoolImages.Where(s => s.Type == PMS.School.Domain.Enum.SchoolImageType.Hardware);
                            if (hardwareImage?.Any() == true)
                            {
                                schoolBrandImages = hardwareImage;
                            }
                        }
                        detailResult.School.SchoolBrands = schoolBrandImages?.ToList();
                    }
                }

            }
            return result;
        }


        [HttpGet]
        [Description("专家履历详情")]
        public async Task<ResponseResult> GetRecordDetail([FromQuery] GetDetailRequest request)
        {
            var result = ResponseResult.Success();
            var find = await _talentSettingService.GetRecordDetail(request.ID);
            if (find != null)
            {
                var isFollowed = false;
                await Task.Run(() =>
                {
                    if (User.Identity.IsAuthenticated && UserId.Value != request.ID)
                    {
                        isFollowed = _collectionService.IsCollected(UserId.Value, request.ID);
                    }
                });

                TalentRecordDetailResult detailResult = new TalentRecordDetailResult()
                {
                    AuthName = find.AuthName,
                    GradeNames = find.TalentGrades?.OrderBy(p => p.Sort).Select(p => p.Name),
                    Introduction = find.TelentIntroduction,
                    LevelName = find.TalentLevelName,
                    NickName = find.NickName,
                    RegionTypeNames = find.TalentRegions?.Select(p => p.Name),
                    ReplyPercent = find.SixHourReplyPercent.ToString("f2"),
                    Score = find.AvgScore.ToString(),
                    UserID = find.TalentUserID,
                    HeadImgUrl = find.HeadImgUrl,
                    IsFollowed = isFollowed,
                    Price = find.Price.CutDecimalWithN(2).ToString(),
                    IsEnable = find.IsEnable,
                    TalentType = find.TalentType,
                    CreateTime = find.CreateTime,
                    RegionDesc = find.RegionDesc,
                    TalentIntro = find.TalentIntro,
                    Covers = find.Covers,


                };
                var couponRules = OrderController.GetCouponRules(find);
                var coupons = await _couponTakeService.GetWaitUseCoupons(UserIdOrDefault, find.Price, couponRules);
                if (coupons.CanUse.Any())
                {
                    var bestCoupons = await _couponTakeService.GetBestCoupons(UserIdOrDefault, find.Price, OrderController.minPayAmount, couponRules);
                    coupons.CanUse = coupons.CanUse
                        .OrderByDescending(ctd => _couponTakeService.ComputeCouponAmmount(ctd.CouponInfo, find.Price, OrderController.minPayAmount))
                        .ThenBy(ctd => ctd.VaildEndTime)
                        .ToList();
                    detailResult.BestCoupons = _mapper.Map<List<CouponTakeResult>>(bestCoupons.BestCoupons);
                    detailResult.BestCouponAmmount = bestCoupons.CouponAmmount;
                    detailResult.CanUseCoupons = _mapper.Map<List<CouponTakeResult>>(coupons.CanUse);
                }
                result.Data = detailResult;
                if (find.SchoolExtId.HasValue)
                {
                    //关联学校处理
                    var latitude = Request.GetLatitude();
                    var longitude = Request.GetLongitude();
                    detailResult.School = _mapper.Map<SchoolResult>(await _schoolService.GetSchoolExtDtoAsync(find.SchoolExtId.Value, Convert.ToDouble(latitude), Convert.ToDouble(longitude)));
                    if (detailResult.School != null)
                    {
                        var schoolImages = await _schoolService.GetSchoolImages(detailResult.School.ExtId, null);
                        if (schoolImages?.Any() == true)
                        {
                            var images = schoolImages.OrderBy(p => p.Type).ToList();
                            detailResult.School.SchoolPhotos = new List<KeyValueDto<string, string, string>>();
                            detailResult.School.SchoolPhotos.AddRange(images.Where(p => p.Type == PMS.School.Domain.Enum.SchoolImageType.SchoolHonor).OrderBy(p => p.Sort).
                                Select(p => new KeyValueDto<string, string, string>()
                                {
                                    Key = p.Url,
                                    Value = p.ImageDesc,
                                    Message = p.SUrl
                                }));
                            if (detailResult.School.SchoolPhotos.Count < 3)
                            {
                                detailResult.School.SchoolPhotos.AddRange(images.Where(p => p.Type != PMS.School.Domain.Enum.SchoolImageType.SchoolHonor &&
                                p.Type != PMS.School.Domain.Enum.SchoolImageType.SchoolBrand).OrderBy(p => p.Type).ThenBy(p => p.Sort).
                                    Select(p => new KeyValueDto<string, string, string>()
                                    {
                                        Key = p.Url,
                                        Value = p.ImageDesc,
                                        Message = p.SUrl
                                    }));
                            }
                            if (detailResult.School.SchoolPhotos.Any())
                            {
                                detailResult.School.SchoolPhotos = detailResult.School.SchoolPhotos.Take(3).ToList();
                            }
                        }

                        detailResult.School.SchoolVideos = await _schoolService.GetSchoolVideos(detailResult.School.ExtId);
                        var schoolBrandImages = schoolImages.Where(s => s.Type == PMS.School.Domain.Enum.SchoolImageType.SchoolBrand)?.OrderBy(p => p.Sort).Take(8);
                        if (!schoolBrandImages.Any())
                        {
                            var hardwareImage = schoolImages.Where(s => s.Type == PMS.School.Domain.Enum.SchoolImageType.Hardware);
                            if (hardwareImage?.Any() == true)
                            {
                                schoolBrandImages = hardwareImage;
                            }
                        }
                        detailResult.School.SchoolBrands = schoolBrandImages?.ToList();
                    }

                }

            }
            return result;
        }


        [HttpGet]
        [Description("获取专家履历关联数据")]
        public async Task<ResponsePageResult> GetRecordDatas(Guid userId, TalentRecordDataRelationDataType dataType, int page = 1, int pageSize = 10)
        {
            switch (dataType)
            {
                case TalentRecordDataRelationDataType.article:
                    var articlePageResult = await _talentRecordDataRelationService.GetArticleDtos(userId, page, pageSize);
                    return ResponsePageResult.Success(articlePageResult.data, articlePageResult.total, "查询文章OK。");
                case TalentRecordDataRelationDataType.live:
                    var livePageResult = await _talentRecordDataRelationService.GetLives(userId, page, pageSize);
                    return ResponsePageResult.Success(livePageResult.data, livePageResult.total, "查询直播OK。");
                case TalentRecordDataRelationDataType.qacase:
                    var qacaseResult = await _talentRecordDataRelationService.GetQACases(userId, page, pageSize);
                    return ResponsePageResult.Success(qacaseResult.data.Select(qacase => new
                    {
                        qacase.Id,
                        qacase.Background,
                        Images = Newtonsoft.Json.JsonConvert.DeserializeObject(qacase.Images)
                    }), qacaseResult.total, "查询经典案例OK。");
                default:
                    return ResponsePageResult.Failed("没有这个dataType。");
            }

        }


        [HttpGet]
        [Description("获取专家等级")]
        public async Task<ResponseResult> ListTalentLevels()
        {
            var result = ResponseResult.Success();
            var finds = await _gradeService.GetAllGrades();
            if (finds != null)
            {
                result.Data = new ListTalentLevelsResponse()
                {
                    Items = finds.Select(p => new ListTalentLevelsItem()
                    {
                        ID = p.ID,
                        Name = p.Name,
                        Sort = p.Sort
                    })
                };
            }
            else
            {
                result = ResponseResult.Failed();
            }
            return result;
        }

        [HttpGet]
        [Description("专家列表，按用户关注专家领域查询，按粉丝排序")]
        public async Task<ResponseResult> GetTalentList([FromQuery] GetDetailRequest request)
        {
            var data = await _recommentTalentService.GetTalentList(request.ID.ToString());
            return ResponseResult.Success(data);
        }

        [HttpGet]
        [Description("上学问专家列表")]
        public async Task<ResponseResult> GetTalents()
        {
            var data = await _recommentTalentService.GetTalentList();
            return ResponseResult.Success(data);
        }

        [HttpGet]
        [Description("专家列表筛选参数")]
        public async Task<ResponseResult> PageParams()
        {
            var result = ResponseResult.Success();
            var responseResult = new PageParamsResponse();
            var grades = await _gradeService.GetAllGrades();
            var regionTypes = await _regionTypeService.GetAllRegionTypes();
            var levels = await _talentSettingService.GetAllTalentLevels();
            responseResult.OrderTypes = new List<object>()
            {
                new {
                    ID = 0,
                    Name = "默认排序",
                    Sort = 1
                },
                new {
                    ID = 1,
                    Name = "响应速度优先",
                    Sort = 2
                },
                new {
                    ID = 2,
                    Name = "价格从低到高",
                    Sort = 3
                },
            };
            if (grades?.Any() == true) responseResult.Grades = grades.OrderBy(p => p.Sort).Select(p => new { p.ID, p.Name, p.Sort });
            if (regionTypes?.Any() == true) responseResult.RegionTypes = regionTypes.OrderBy(p => p.Sort).Select(p => new { p.ID, p.Name, p.Sort, SubItems = p.SubItems?.OrderBy(s => s.Sort).Select(s => new { s.ID, s.Name, s.Sort }) });
            if (levels?.Any() == true) responseResult.TalentLevels = levels.OrderBy(p => p.Sort).Select(p => new { p.ID, p.Name, p.Sort });
            result.Data = responseResult;
            return result;
        }

        [HttpGet]
        [Description("分页获取专家列表")]
        public async Task<ResponseResult> Page([FromQuery] PageRequest request)
        {
            var result = ResponseResult.Success();
            decimal minPrice = 0;
            decimal maxPrice = 0;

            decimal.TryParse(request.MinPrice, out minPrice);
            decimal.TryParse(request.MaxPrice, out maxPrice);

            var finds = await _talentSettingService.PageTalentIDs(request.PageIndex, request.PageSize, request.GradeID, request.RegionTypeID, request.LevelID, request.OrderTypeID
                , minPrice, maxPrice, request.NickName);
            if (finds?.Any() == true)
            {
                var talents = await _talentSettingService.GetDetails(finds.Select(p => p.TalentUserID).Distinct());
                if (talents?.Any() == true)
                {
                    var responseItems = new List<PageResponse>();
                    foreach (var item in finds)
                    {
                        var find = talents.FirstOrDefault(p => p.TalentUserID == item.TalentUserID);
                        if (find != null)
                        {
                            responseItems.Add(new PageResponse()
                            {
                                AuthName = find.AuthName,
                                GradeNames = find.TalentGrades?.OrderBy(p => p.Sort).Select(p => p.Name),
                                HeadImgUrl = find.HeadImgUrl,
                                LevelName = find.TalentLevelName,
                                NickName = find.NickName,
                                Price = find.Price.CutDecimalWithN(2).ToString(),
                                RegionTypeNames = find.TalentRegions?.Where(p => p.IsValid == true).Select(p => p.Name),
                                ReplyPercent = find.SixHourReplyPercent.ToString("f2"),
                                Score = (int)find.AvgScore,
                                TalentUserID = find.TalentUserID,
                                TalentType = find.TalentType
                            });
                        }
                    }
                    result.Data = responseItems;
                }
            }
            else
            {
                result.Data = new object[0];
            }
            return result;
        }


        [HttpGet]
        [Description("推荐专家列表")]
        public async Task<ResponseResult<List<TalentInfoResult>>> GetSimilarTalents(Guid talentUserID, int count = 5)
        {
            var talentInfos = await _talentSettingService.GetSimilarTalents(talentUserID, count);
            List<TalentInfoResult> talentInfoResults = _mapper.Map<List<TalentInfoResult>>(talentInfos);
            return ResponseResult<List<TalentInfoResult>>.Success(talentInfoResults, "推荐专家列表");


        }

        [HttpGet]
        [Description("分页推荐专家")]
        public async Task<ResponseResult> PageSimilarTalents(Guid talentUserID, int pageIndex = 1, int pageSize = 10)
        {
            var talentInfos = await _talentSettingService.PageSimilarTalents(talentUserID, pageIndex, pageSize);
            List<TalentInfoResult> talentInfoResults = _mapper.Map<List<TalentInfoResult>>(talentInfos);
            return ResponseResult<List<TalentInfoResult>>.Success(talentInfoResults);


        }




        /// <summary>
        /// 获取达人详情信息
        /// </summary>
        /// <param name="aid">文章ID</param>
        /// <returns></returns>
        [HttpGet]
        [Description("升学顾问")]
        public async Task<ResponseResult> GetSchoolExpert(Guid? aid)
        {
            List<int> gradeTypes = new List<int>();
            if (aid!= null)
            {
                var schoolTypes = _articleService.GetCorrelationSchoolTypes(aid.Value);
                var random = new Random();
                var grades = schoolTypes.GroupBy(s => s.SchoolGrade).Select(s => s.Key);
                foreach (var grade in grades)
                {
                    switch (grade)
                    {
                        case 1:
                            gradeTypes.Add(1);
                            break;
                        case 2:
                            gradeTypes.Add(random.Next(2, 4));
                            break;
                        case 3:
                            gradeTypes.Add(4);
                            break;
                        case 4:
                            gradeTypes.Add(5);
                            break;
                    }
                }
            }
            if (gradeTypes.Any() == false)
            {
                gradeTypes.Add(1);
                gradeTypes.Add(2);
                gradeTypes.Add(3);
                gradeTypes.Add(4);
                gradeTypes.Add(5);
            }

            var ids = await _hotTypeService.GetRandomOrderIDByGrade(gradeTypes);
            if (ids?.Any() == true)
            {
                var orders = await _orderService.ListByIDs(ids);
                if (orders?.Any() == true)
                {
                    IEnumerable<TalentInfoResult_01> talentInfoResult_01s = _mapper.Map<IEnumerable<TalentInfoResult_01>>(await _talentSettingService.GetDetails(orders.Select(p => p.AnswerID).Distinct()));
                    var messages = await _messageService.GetOrdersQuetion(orders.Select(p => p.ID).Distinct());
                    foreach (var order in orders)
                    {
                        var talentSetting_01 = talentInfoResult_01s.FirstOrDefault(p => p.UserID == order.AnswerID);
                        var message = messages.FirstOrDefault(p => p.OrderID == order.ID);
                        if (message != null)
                        {
                            if (!string.IsNullOrWhiteSpace(message.Content))
                            {
                                try
                                {
                                    var messageContent = Newtonsoft.Json.JsonConvert.DeserializeObject<PMS.PaidQA.Domain.Message.TXTMessage>(message.Content);
                                    if (messageContent != null) talentSetting_01.Content = messageContent.Content;
                                }
                                catch
                                {
                                }
                            }
                        }
                    }

                    return ResponseResult.Success(talentInfoResult_01s);
                }
            }



            return ResponseResult.Success("No Data.");

        }
    }
}
