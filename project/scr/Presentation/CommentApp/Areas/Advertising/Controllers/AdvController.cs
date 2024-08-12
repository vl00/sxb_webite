using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace Sxb.Api.Controllers
{

    using PMS.OperationPlateform.Application.IServices;
    using Sxb.Web.Areas.Advertising.Models;
    using Sxb.Web.Utils;
    using System.Web;
    using System.Text.Json;
    using Sxb.Web.Common;
    using System.Drawing;
    using System.Net.Http;
    using System.Net.Mime;
    using ProductManagement.Framework.Foundation;
    using System.Diagnostics;
    using System.IO;
    using ProductManagement.Tool.QRCoder;
    using System.Drawing.Imaging;
    using ProductManagement.API.Http.Interface;
    using Sxb.Web.Areas.TopicCircle.Models;
    using PMS.School.Application.IServices;
    using PMS.OperationPlateform.Domain.Entitys;
    using Sxb.Web.Controllers;
    using Sxb.Web.Response;
    using WeChat;
    using PMS.TopicCircle.Application.Services;
    using ProductManagement.API.Http.Model;
    using PMS.OperationPlateform.Domain.Enums;
    using ProductManagement.Framework.Cache.Redis;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json.Linq;
    using PMS.PaidQA.Application.Services;
    using PMS.School.Domain.Enum;
    using ProductManagement.API.Http.Model.Org;
    using WeChat.Interface;
    using PMS.OperationPlateform.Application.Dtos;

    /// <summary>
    /// 广告数据接口
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdvController : ApiBaseController
    {
        private readonly ILogger<AdvController> _logger;

        private readonly ISchoolService _schoolService;
        private readonly IArticleService _articleService;
        private readonly IDataPacketService _dataPacketService;
        private readonly IKeyValueService _keyValueService;
        private readonly IAdvertisingBaseService _advertisingBaseService;
        private readonly PMS.Infrastructure.Application.IService.IWeixinTemplateService _weixinTemplateService;

        private readonly IOperationClient _operationClient;
        private readonly IWeChatAppClient _weChatAppClient;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly IHtmlServiceClient _htmlServiceClient;
        private readonly IFileServiceClient _fileServiceClient;
        IRegionTypeService _regionTypeService;
        IOrgServiceClient _orgServiceClient;
        IGradeService _gradeService;
        ICustomMsgService _customMsgService;

        public AdvController(ISchoolService schoolService
            , IArticleService articleService
            , IDataPacketService dataPacketService
            , IKeyValueService keyValueService
            , IAdvertisingBaseService advertisingBaseService
            , PMS.Infrastructure.Application.IService.IWeixinTemplateService weixinTemplateService
            , IOperationClient operationClient
            , IWeChatAppClient weChatAppClient
            , IEasyRedisClient easyRedisClient
            , IHtmlServiceClient htmlServiceClient
            , ILogger<AdvController> logger
            , IFileServiceClient fileServiceClient
            , IRegionTypeService regionTypeService
            , IOrgServiceClient orgServiceClient
            , IGradeService gradeService
            , ICustomMsgService customMsgService)
        {
            _schoolService = schoolService;
            _articleService = articleService;
            _dataPacketService = dataPacketService;
            _keyValueService = keyValueService;
            _advertisingBaseService = advertisingBaseService;
            _weixinTemplateService = weixinTemplateService;
            _operationClient = operationClient;
            _weChatAppClient = weChatAppClient;
            _easyRedisClient = easyRedisClient;
            _htmlServiceClient = htmlServiceClient;
            _logger = logger;
            _fileServiceClient = fileServiceClient;
            _regionTypeService = regionTypeService;
            _orgServiceClient = orgServiceClient;
            _gradeService = gradeService;
            _customMsgService = customMsgService;
        }


        /// <summary>
        /// 获取referer中的id
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public Guid? GetRefererId(string referer)
        {
            Debug.WriteLine("referer:" + referer);
            Guid? dataId = null;

            var urlEndPoints = UrlEndPointsCommon.Build(referer);
            if (urlEndPoints != null)
            {
                var no = urlEndPoints.No;
                var id = urlEndPoints.Id;
                //识别到Id, 根据ID查询
                if (id != null && id.Value != Guid.Empty)
                {
                    dataId = id;
                }
                //未识别到Id, 根据No查询
                else if (urlEndPoints.No > 0)
                {
                    #region 根据No查询
                    switch (urlEndPoints.ControllerName)
                    {
                        case "school":
                            var school = _schoolService.GetSchoolByNo(no);
                            dataId = school?.Id;
                            break;
                        case "article":
                            var article = _articleService.GetArticleByNo(no);
                            dataId = article?.id;
                            break;
                    }
                    #endregion
                }
            }
            return dataId;
        }

        public async Task<IActionResult> WxDataPacketRedirect(Guid dataId, Guid? userId, string referer)
        {
            var ticks = DateTime.Now.Ticks;
            _logger.LogInformation("获取资料包中间页,dataId={0},referer={1},ticks={2}", dataId, referer, ticks);


            var dataPacket = _dataPacketService.Add(new DataPacket()
            {
                UserId = userId != null && userId.Value != Guid.Empty ? userId.Value : UserIdOrDefault,
                ScanPage = referer ?? string.Empty,
                DataId = dataId
            });
            var id = dataPacket.Id;

            //回调中间页图片
            //string callbackUrl = $"{ConfigHelper.GetHost()}/api/wechat/AdWxData?id={id}&ticks={ticks}";
            string callbackUrl = $"{ConfigHelper.GetHost()}/datapacket/AdWxData?id={id}&ticks={ticks}";
            return Redirect(callbackUrl);
        }

        /// <summary>
        /// 微信资料包广告图片
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetWxDataPacketAdImage()
        {
            //背景图片
            var baseImageUrl = "https://cos.sxkid.com/images/mkt/90caf8c9960c4e5490d81f6bd5cdca70.jpg";
            var referer = Request.Headers["referer"];
            var dataId = GetRefererId(referer);
            if (dataId == null)
            {
                //return Redirect(baseImageUrl);
                dataId = Guid.Empty;
            }
            var encodeReferer = HttpUtility.UrlEncode(referer);

            var refererMd5 = MD5Helper.GetSimpleMD5(referer);
            var key = string.Format(RedisKeys.AdDataPacketKey, refererMd5);
            var imageUrl  = await _easyRedisClient.GetAsync<string>(key);
            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                return Redirect(imageUrl);
            }

            //回调中间页图片
            string callbackUrl = $"{ConfigHelper.GetHost()}/api/adv/WxDataPacketRedirect?dataId={dataId}&userId={UserId}&referer={encodeReferer}";
            //使用短链, 避免扫不到码
            var shortUrlResp = await _operationClient.GetShortUrl(callbackUrl);
            if (shortUrlResp.success)
            {
                callbackUrl = shortUrlResp.Data;
            }
            var qrcodeImage = new LinkCoder().Create(callbackUrl);


            using (var ms = await ComposeImages(baseImageUrl, qrcodeImage, 266, 107, 82, 82))
            {
                //上传到cdn, 并缓存cdn url
                var uploadResp = await _fileServiceClient.UploadDataPacketImage($"{Guid.NewGuid()}.png", ms);//上学帮让升学更简单.png

                if (uploadResp == null)
                {
                    imageUrl = baseImageUrl;
                }
                else
                {
                    imageUrl = uploadResp.cdnUrl;
                }

            }

            await _easyRedisClient.AddAsync(key, imageUrl, TimeSpan.FromDays(30));
            return Redirect(imageUrl);
        }


        [NonAction]
        public async Task<MemoryStream> ComposeImages(string baseImageUrl, Image qrcodeImage, int x, int y, int width, int height)
        {
            //背景图片
            using (var baseImage = await _htmlServiceClient.GetImage(baseImageUrl))
            {
                //二维码图片
                return await GraphicsHelper.ComposeImages(baseImage, qrcodeImage, x, y, width, height);
            }
        }

        /// <summary>
        /// 批量发送资料包
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseResult> BatchSendDataPacket(DataPacketStep step, bool uncheckTime = false)
        {
            var dataPackets = await _dataPacketService.GetStep(step, uncheckTime);
            foreach (var item in dataPackets)
            {
                await SendDataPacket(item);
            }
            return ResponseResult.Success();
        }


        [HttpGet]
        public async Task<ResponseResult> SendDataPacket(DataPacket dataPacket)
        {
            var nextStep = dataPacket.Step + 1;
            var key = nextStep.Description();
            var text = GetTemplateText(key);
            if (string.IsNullOrWhiteSpace(text))
            {
                return ResponseResult.Failed("资料包关注回复模板为空");
            }
            await SendWxMsgText(dataPacket.OpenId, text);

            //48h额外发送图片
            //if (nextStep == DataPacketStep.Above48h)
            //{
            //    var mediaKey = nextStep.GetDefaultImageKeyValue();
            //    var mediaId = GetTemplateText(mediaKey);
            //    await SendWxMsgImage(dataPacket.OpenId, mediaId);
            //}

            _dataPacketService.UpdateStep(dataPacket.Id, dataPacket.Step + 1);

            return ResponseResult.Success();
        }

        private async Task<bool> SendWxMsgText(string openId, string text)
        {
            if (string.IsNullOrWhiteSpace(openId) || string.IsNullOrWhiteSpace(text))
            {
                _logger.LogError("发送微信信息失败,openId={0},text={1}", openId, text);
                return false;
            }

            TextCustomMsg msg = new TextCustomMsg()
            {
                ToUser = openId,
                content = text
            };
            var accessToken = await _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" });
            return (await _customMsgService.Send(accessToken.token, msg))?.Success ?? false;
        }

        private async Task<bool> SendWxMsgImage(string openId, string mediaId)
        {
            if (string.IsNullOrWhiteSpace(openId) || string.IsNullOrWhiteSpace(mediaId))
            {
                _logger.LogError("发送微信信息失败,openId={0},mediaId={1}", openId, mediaId);
                return false;
            }

            ImageCustomMsg msg = new ImageCustomMsg()
            {
                ToUser = openId,
                MediaId = mediaId
            };
            var accessToken = await _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" });
            return (await _customMsgService.Send(accessToken.token, msg))?.Success ?? false;
        }

        /// <summary>
        /// 资料包模板
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string GetTemplateText(string key)
        {
            var templateIdStr = _keyValueService.Get(key);
            if (!Guid.TryParse(templateIdStr, out Guid templateId))
            {
                return string.Empty;
            }
            return _weixinTemplateService.GetTemplateTextFromCache(templateId).GetAwaiter().GetResult();
        }




        /// <summary>
        /// 获取指定广告位置的配置信息
        /// </summary>
        /// <param name="location"></param>
        /// <param name="callBack"></param>
        /// <param name="dataId"></param>
        /// <param name="type">指定dataId类型，1->学校 2->文章 0或不传忽略</param>
        /// <param name="autos">指定需要自动生成广告的广告位</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAdvs(
            [FromQuery] int[] location
            , [FromQuery] string callBack
            , [FromQuery] string dataId
            , [FromQuery] int type=0
            , [FromQuery] int[] autos=null)
        {
            AdvGetAdvsResponse response = new AdvGetAdvsResponse()
            {
                Advs = new List<AdvGetAdvsResponse.AdvOption>()
            };
            try
            {
                var clientType = Request.GetClientType();
                var cityId = Request.GetAvaliableCity();
                foreach (var locationId in location)
                {
                    var advs = new List<AdvertisingBaseGetAdvertisingResultDto>();

                    if (locationId == 99 && await _easyRedisClient.ExistsAsync("org:linshi_mallthemes:fty_" + dataId))
                    {
                        string iconKey = "org:linshi_mallthemes:icon";
                        //移动端学校详情页悬浮广告临时解决方案
                        var mallthemesAds = await _easyRedisClient.CacheRedisClient.Database.HashGetAllAsync("org:linshi_mallthemes:fty_" + dataId);

                        foreach (var malltheme in mallthemesAds)
                        {
                            advs.Add(new AdvertisingBaseGetAdvertisingResultDto
                            {
                                Id = malltheme.Name == "h5mp" ? 1 : malltheme.Name == "pc" ? 2 : 0,
                                PicUrl = await _easyRedisClient.GetStringAsync(iconKey),
                                Url = malltheme.Value
                            });
                        }
                    }
                    else
                    {
                        advs = _advertisingBaseService.GetAdvertising(locationId, cityId, dataId).ToList();
                    }

                    if (clientType == UA.Mobile)
                    {
                        //手机端，链接要加前缀
                        advs = advs.Select(s =>
                        {
                            s.Url = $"ischool://web?url={HttpUtility.UrlEncode(s.Url)}";
                            return s;
                        }).ToList();
                    }
                    var option = new AdvGetAdvsResponse.AdvOption
                    {
                        Place = locationId,
                        Items = advs.ToList()
                    };

                    if (autos?.Any(a => a == locationId) == true)
                    {
                        if (type > 0 && Guid.TryParse(dataId, out Guid gDataId)) // && advs?.Any() == false
                        {
                           var gg21course = await GetRecommandCourses(gDataId, type);
                           option.Courses = gg21course;
                        }
                    }
                    response.Advs.Add(option);
                }
                response.status = 0;
                response.msg = "success";
            }
            catch (Exception ex)
            {
                response.status = 1;
                response.msg = ex.Message;
            }
            var jsonResult = JsonSerializer.Serialize(response);
            if (string.IsNullOrEmpty(callBack))
            {
                return Content(jsonResult, "application/json");
            }
            else
            {
                //可能是jsonp

                return Content($"{callBack}({jsonResult})", "text/html");
            }
        }

        private class MallthemesAdsModel
        {
            public string H5mp { get; set; }
            public string PC { get; set; }
            public string Fwh { get; set; }
        }


        /// <summary>
        /// 获取自动匹配的课程广告
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        private async Task<IEnumerable<GG21Course>> GetRecommandCourses(Guid dataId,int type)
        {
            //匹配学科的属性
            string articleType = string.Empty;
            List<string> talentRegions = new List<string>();
            List<string> specialCourses =  new List<string>();
            List<string> courseSettings = new List<string>();
            //匹配年龄的属性
            List<string> talentGrades = new List<string>();
            List<string> articleGradeTypes = new List<string>();
            List<(byte minage, byte maxage)> schoolAges = new List<(byte minage, byte maxage)>();

            //匹配价格的属性
            List<string> schoolTypes = new List<string>();

            if (type == 1) {
                //使用学校来匹配
                var schoolInfo = await _schoolService.GetSchoolExtensionDetailsNoZanWeiShouLu(dataId);
                if (schoolInfo == null)
                {
                    return null;
                }
                var schtype = schoolInfo.TypeName;
                //获取特色课程属性

                if (!string.IsNullOrEmpty(schoolInfo.Characteristic))
                {
                    var characteristics = JArray.Parse(schoolInfo.Characteristic);
                    if (characteristics?.Any() == true)
                    {
                        specialCourses.AddRange(characteristics.Select(token => token["Key"].Value<string>()));
                    }
                }
                //获取课程设置【以标签来表达】，目前只分为国内和国外两种。
                var tags = await _schoolService.GetTagTypes(schoolInfo.ExtId);
                if (!tags.Contains("国内课程"))
                {
                    courseSettings.Add("国际课程");
                }
                schoolAges.Add((schoolInfo.Age.GetValueOrDefault(), schoolInfo.MaxAge.GetValueOrDefault()));
                //获取学校类型
                PMS.School.Domain.Common.SchoolType schoolType = (PMS.School.Domain.Common.SchoolType)schoolInfo.Type;
                if (schoolType == PMS.School.Domain.Common.SchoolType.Private
                    || schoolType == PMS.School.Domain.Common.SchoolType.International
                    || schoolType == PMS.School.Domain.Common.SchoolType.ForeignNationality)
                {
                    schoolTypes.Add(schoolType.GetDescription());
                }
                else {
                    schoolTypes.Add("其它");
                }
            }
            if (type == 2)
            {
                //使用文章来匹配
                var talentUserId = await  _articleService.GetTalentUserId(dataId);
                //擅长领域
                if (!talentUserId.HasValue)
                {
                    //无达人情况
                    //文章类型选择全部
                    articleType = "all";
                   //获取文章学段
                   var articleSchoolTypes = _articleService.GetCorrelationSchoolTypes(dataId);
                   var articleSchoolGrades = articleSchoolTypes
                        ?.Select(s => (SchoolGrade)s.SchoolGrade)
                        ?.Distinct()
                        ?.Select(d=>d.GetDescription());
                    if (articleSchoolGrades?.Any() == true)
                    {
                        articleGradeTypes.AddRange(articleSchoolGrades);
                    }

                }
                else
                {
                    var regionTypes = await _regionTypeService.GetByTalentUserID(talentUserId.Value);
                    if (regionTypes?.Any() == true)
                    {
                        talentRegions.AddRange(regionTypes.Select(r => r.Name));
                    }

                    var talentGradesFromDB =  (await  _gradeService.GetByTalentUserID(talentUserId.Value))
                        .Select(g=>g.Name);

                    if (talentGradesFromDB?.Any() == true)
                    {
                        talentGrades.AddRange(talentGradesFromDB);
                    }
                }


            }

        
            var subjectEnums = SubjectEnumHelper.GetSubjectEnums(
                 talentRegions: talentRegions
                , articleType: articleType
                , specialCourses: specialCourses
                , courseSettings: courseSettings);

          
            var ageRanges = AgeRangeHelper.GetAgeRanges(
                 talentGrades: talentGrades,
                 articleGradeTypes: articleGradeTypes,
                 ages: schoolAges
                 );
            var price = PriceRangeHelper.GetPrice(schoolTypes);


            var gg21Res = await _orgServiceClient.GetGG21Courses(new ProductManagement.API.Http.Request.Org.GetGG21CoursesRequest()
            {
                ages = ageRanges.Select(a => a.GetAgeRange()),
                subjs = string.Join(',', subjectEnums.Select(s => (int)s)),
                price = (int)price
            });

            return gg21Res;

        }



    }
}