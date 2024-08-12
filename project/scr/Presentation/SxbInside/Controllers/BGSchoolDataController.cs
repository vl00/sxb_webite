using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMS.School.Application.IServices;
using PMS.Search.Domain.Entities;
using Newtonsoft.Json;
using PMS.Search.Application.IServices;
using Sxb.Inside.Response;
using Sxb.Inside.Common;
using Microsoft.Extensions.Logging;
using System.Threading;
using PMS.Infrastructure.Application.IService;
using WeChat.Model;
using static WeChat.Model.UpsertPreUniversityBasicRequest;
using PMS.School.Domain.Common;
using System.Text;
using PMS.School.Domain.Dtos;
using PMS.School.Domain.Entities.WechatDemo;
using iSchool;
using Newtonsoft.Json.Linq;
using ProductManagement.Framework.Foundation;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using Sxb.Inside.RequestModel;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.Inside.Controllers
{
    public class BGSchoolDataController : Controller
    {
        private ISchoolService _schoolService;
        private IImportService _importService;
        private IWeChatService _weChatService;
        private IOnlineSchoolRecruitService _onlineSchoolRecruitService;
        private ICityInfoService _cityInfoService;
        private IGeneralTagService _generalTagService;

        private readonly ILogger<BGSchoolDataController> _logger;
        ISchoolOverViewService _schoolOverViewService;
        IOnlineSchoolQuotaService _onlineSchoolQuotaService;
        IOnlineSchoolFractionService _onlineSchoolFractionService;
        IAreaRecruitPlanService _areaRecruitPlanService;
        ISchoolDataDBContext _schoolDataDBContext;


        public BGSchoolDataController(ISchoolService schoolService, IImportService importService, ILoggerFactory loggerFactory, IWeChatService weChatService, IOnlineSchoolRecruitService onlineSchoolRecruitService, ICityInfoService cityInfoService, IGeneralTagService generalTagService, ISchoolOverViewService schoolOverViewService, IOnlineSchoolQuotaService onlineSchoolQuotaService, IOnlineSchoolFractionService onlineSchoolFractionService, IAreaRecruitPlanService areaRecruitPlanService, ISchoolDataDBContext schoolDataDBContext)
        {
            _schoolService = schoolService;
            _importService = importService;

            _logger = loggerFactory.CreateLogger<BGSchoolDataController>();
            _weChatService = weChatService;
            _onlineSchoolRecruitService = onlineSchoolRecruitService;
            _cityInfoService = cityInfoService;
            _generalTagService = generalTagService;
            _schoolOverViewService = schoolOverViewService;
            _onlineSchoolQuotaService = onlineSchoolQuotaService;
            _onlineSchoolFractionService = onlineSchoolFractionService;
            _areaRecruitPlanService = areaRecruitPlanService;
            _schoolDataDBContext = schoolDataDBContext;
        }

        /// <summary>
        /// 创建ES 学校索引
        /// </summary>
        public void CreateSchoolSearchIndex()
        {
            _importService.CreateSchoolSearch();
        }

        /// <summary>
        /// ES 学校数据导入
        /// </summary>
        public void FirstImportSearchSchool()
        {
            //如果程序中断 则获取es中最后一条录入成功的学校数据的createtime，然后去数据库中取大于这条时间的数据 则开始取
            var lastTime = _importService.GetLastUpdate("bdschoolsearchindex");
            //var time = lastTime.Find(x => x.Name == "bdschoolsearchindex").LastTime;
            var time = new DateTime(2019, 6, 1, 0, 0, 0);
            int pageIndex = 1;
            while (true)
            {
                try
                {
                    var data = _schoolService.SchoolSearchImport(pageIndex, time);

                    if (!data.Any())
                    {
                        _logger.LogInformation("数据导入完成");
                        break;
                    }
                    else
                    {
                        time = data.OrderByDescending(x => x.CreateTime).FirstOrDefault().CreateTime;
                    }

                    var schoolImports = SchoolToBTSearchSchool.SchoolDtoToBTSearchSchool(data);
                    bool IsSuccess = _importService.ImportSearchSchoolData(schoolImports);

                    if (IsSuccess)
                    {
                        pageIndex++;
                        _logger.LogInformation("成功导入");
                        Thread.Sleep(3000);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"导入失败，错误异常日志：{ex.Message}，最后更新时间：{DateTime.Now}");
                }
            }
        }

        /// <summary>
        /// ES [添加学校信息 | 修改学校信息]
        /// </summary>
        /// <param name="schoolImport"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult SchNewestData([FromBody] List<Guid> Sid)
        {
            if (Sid?.Count > 0)
            {
                var data = _schoolService.BDSchDataSearch(Sid);
                var update = SchoolToBTSearchSchool.SchoolDtoToBTSearchSchool(data);
                bool IsSuccess = _importService.ImportSearchSchoolData(update);
                return ResponseResult.Success(new { success = IsSuccess });
            }
            return ResponseResult.Success();
        }

        /// <summary>
        /// ES 批量删除学校
        /// </summary>
        /// <param name="SchIds"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult DeleteSchByIds([FromBody] List<string> Sid)
        {
            if (Sid?.Count > 0)
            {
                Sid = Sid.Select(x => x.ToString().ToLower()).ToList();
                bool IsSuccess = _importService.DelSchoolData(Sid);
                return ResponseResult.Success(new { success = IsSuccess });
            }
            return ResponseResult.Success();
        }

        /// <summary>
        /// 更新学校最新数据【学校数据修改发生变动或者新增学校数据】
        /// </summary>
        /// <param name="SchIds">学校id</param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult Update([FromBody] List<Guid> Sid)
        {
            if (Sid?.Count > 0)
            {
                var data = _schoolService.BDSchDataSearch(Sid);
                var update = SchoolToBTSearchSchool.SchoolDtoToBTSearchSchool(data);
                //根据学校id 直接查出该学校最新数据 覆盖旧的数据
                bool IsSuccess = _importService.UpdateBDSchooData(update);

                return ResponseResult.Success(new { success = IsSuccess });
            }
            return ResponseResult.Success();
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }




        [HttpPost]
        public async Task<ResponseResult> UpsertPreUniversityBasic([FromBody] List<UpsertPreUniversityBasicItemData> itemDatas, [FromQuery] bool isTest = true)
        {

            int skip = 0, take = 1;
            int jump_type = 1;
            List<dynamic> retruns = new List<dynamic>();
            List<dynamic> fails = new List<dynamic>();
            List<Guid> offlineSchools = new List<Guid>();
            List<Guid> unregisterInWeChatSchools = new List<Guid>();
            var wechatSouYiSouSchools = await _schoolService.GetWeChatSouYiSouSchools(itemDatas.Select(s=>s.EId).ToList());
            while (skip < itemDatas.Count)
            {
                var fetchItems = itemDatas.Skip(skip).Take(take).ToList();
                //var fetchIds = itemDatas.Select(s => s.EId).Skip(skip).Take(take).ToList();
                skip += take;
                if (fetchItems?.Any() == false)
                {
                    break;
                }
                await Task.Run(async () =>
                {
                    List<Pre_University> pre_Universitys = new List<Pre_University>();


                    foreach (var extSchool in fetchItems)
                    {
                        var schoolInfo = await _schoolService.GetSchoolExtensionDetailsAny(extSchool.EId);


                        if (schoolInfo == null)
                        {
                            offlineSchools.Add(extSchool.EId);
                            continue;
                        }
                        var wechatSouYiSouSchool = wechatSouYiSouSchools.FirstOrDefault(s => s.EId == schoolInfo.ExtId);
                        if (wechatSouYiSouSchool == null)
                        {
                            //找不到微信分配ID后的学校
                            unregisterInWeChatSchools.Add(extSchool.EId);
                            continue;
                        }
                        Pre_University pre_University = new Pre_University();
                        pre_University.wx_school_id = wechatSouYiSouSchool.WXSchoolId;
                        if (!string.IsNullOrEmpty(extSchool.Reason))
                            pre_University.reason = extSchool.Reason;
                        else
                            pre_University.reason = await GetWechatSchoolModifyReason(extSchool.EId);
                        if (string.IsNullOrEmpty(pre_University.reason))
                        {
                            fails.Add(new
                            {
                                eid = extSchool.EId,
                                schoolName = wechatSouYiSouSchool.Name,
                                wxschoolid = wechatSouYiSouSchool.WXSchoolId,
                                errmsg = "该学校未检测到填写修改原因。"
                            });
                            continue;
                        }

                        pre_University.school_name = wechatSouYiSouSchool.Name;
                        pre_University.aliases = await GetSchoolAlias(schoolInfo.ExtId);
                        if (!string.IsNullOrEmpty(schoolInfo.WebSite) && schoolInfo.WebSite != "暂未收录")
                        {
                            pre_University.official_website = schoolInfo.WebSite;
                        }


                        pre_University.tags.Add(ReplaceSchFTypeName(schoolInfo.SchFType));
                        if (schoolInfo.EduSysType != PMS.School.Domain.Enum.EduSysType.JuniMidSch
                            && schoolInfo.EduSysType != PMS.School.Domain.Enum.EduSysType.SeniMidSch
                            && !string.IsNullOrEmpty(schoolInfo.EduSysTypeName))
                        {
                            pre_University.tags.Add(schoolInfo.EduSysTypeName);
                        }
                        if (schoolInfo.AuthenticationList?.Any() == true)
                        {
                            List<string> authentications = new List<string>();
                            if (schoolInfo.AuthenticationList.Any(k => k.Key == "省一级"))
                            {
                                authentications.Add("省一级");
                            }
                            else if (schoolInfo.AuthenticationList.Any(k => k.Key == "市一级"))
                            {
                                authentications.Add("市一级");
                            }
                            if (schoolInfo.AuthenticationList.Any(k => k.Key == "国家级示范性"))
                            {
                                authentications.Add("国家级示范性");
                            }
                            else if (schoolInfo.AuthenticationList.Any(k => k.Key == "省级示范性"))
                            {
                                authentications.Add("省级示范性");
                            }
                            else if (schoolInfo.AuthenticationList.Any(k => k.Key == "市级示范性"))
                            {
                                authentications.Add("市级示范性");
                            }
                            pre_University.tags.AddRange(authentications);
                        }

                        pre_University.school_grade = schoolInfo.Grade;
                        var images = await _schoolService.GetSchoolImages(extSchool.EId, null);
                        if (images?.Any() == true)
                        {
                            var brandImages = images.Where(i => i.Type == PMS.School.Domain.Enum.SchoolImageType.SchoolBrand);
                            if (brandImages?.Any() == true)
                            {
                                pre_University.bg_img_info = new Bg_Img_Info()
                                {
                                    jump_type = jump_type,
                                    jump_url = $"https://m.sxkid.com/school/wechat/detail/{UrlShortIdUtil.Long2Base32(schoolInfo.SchoolNo)}_type=7",
                                    img_url = brandImages.First().SUrl,
                                    total_cnt = images.Count

                                };
                            }
                        }



                        //位置信息
                        pre_University.pos_info.address = schoolInfo.Address;
                        pre_University.pos_info.longitude = schoolInfo.Longitude.GetValueOrDefault();
                        pre_University.pos_info.latitude = schoolInfo.Latitude.GetValueOrDefault();

                        pre_University.pos_info.area_codes = new[] { schoolInfo.Province, schoolInfo.City, schoolInfo.Area == 440312 ? 440307 : schoolInfo.Area }; //这里区域做一下特殊出来，区域是大鹏区要替换为龙岗区。

                        var schoolExtensions = _schoolService.GetCurrentSchoolAllExt(schoolInfo.Sid);
                        if (schoolExtensions?.Any() == true)
                        {
                            foreach (var schoolExtension in schoolExtensions)
                            {
                                if (schoolExtension.Id == extSchool.EId)
                                    continue;
                                if (schoolExtension.Id == schoolExtension.SchoolId)
                                    continue;
                                //关联分校
                                pre_University.hints.Add(schoolExtension.SchoolName);
                            }
                        }
                        //招生办电话
                        if (!string.IsNullOrEmpty(schoolInfo.Tel) && schoolInfo.Tel != "暂未收录")
                        {
                            string[] tels = schoolInfo.Tel.Split('；', StringSplitOptions.RemoveEmptyEntries);
                            if (tels?.Any() == true)
                            {
                                pre_University.contact_info = new Contact_Info()
                                {
                                    phones = tels.ToList()
                                };
                            }
                        }
                        var overviewOtherInfo = await _schoolOverViewService.GetByEID(extSchool.EId);
                        var recruits = (await _onlineSchoolRecruitService.GetByEID(extSchool.EId, year: -1, type: -1));
                        var recruit = recruits.OrderBy(r => r.Type).FirstOrDefault(); //按照类型的优先级来取。
                        bool hasAreaRecruitPlans = false;
                        if (schoolInfo.Grade == (byte)SchoolGrade.SeniorMiddleSchool)
                        {
                            var areaRecruitPlans = await _areaRecruitPlanService.GetByAreaCodeAndSchFType(schoolInfo.City.ToString(),
                                schoolInfo.SchFType.Code);
                            hasAreaRecruitPlans = areaRecruitPlans?.Any() == true;
                        }
                        else
                        {
                            var areaRecruitPlans = await _areaRecruitPlanService.GetByAreaCodeAndSchFType(schoolInfo.Area.ToString(),
                                schoolInfo.SchFType.Code);
                            hasAreaRecruitPlans = areaRecruitPlans?.Any() == true;
                        }
                        if (hasAreaRecruitPlans)
                        {
                            string recruitName = $"{schoolInfo.CityName}{schoolInfo.AreaName}{schoolInfo.GradeName}招生政策";
                            //高中不需要显示区
                            if (schoolInfo.Grade == 4)
                            {
                                recruitName = $"{schoolInfo.CityName}{schoolInfo.GradeName}招生政策";
                            }

                            pre_University.recruit_info.recruit_items.Add(new
                            {
                                recruit_type = RecruitType.本区政策,
                                can_jump = 1,
                                jump_type = 1,
                                jump_url = $"https://m.sxkid.com/school/wechat/detail/{UrlShortIdUtil.Long2Base32(schoolInfo.SchoolNo)}_type=2_ref=2",
                                descs = new List<string>() { recruitName },
                            });
                        }
                        if (recruit != null)
                        {
                            //招生信息


                            List<string> plantext = new List<string>();
                            string prefix = string.Empty;
                            if (recruit.Type == 1)
                            {
                                prefix = "就读";
                            }
                            if (recruit.Type == 2)
                            {
                                prefix = "户籍生招收";
                            }
                            if (recruit.Type == 3)
                            {
                                prefix = "积分入学招收";
                            }
                            if (recruit.Type == 4)
                            {
                                prefix = "分类入学招收";
                            }
                            if (recruit.Type == 5)
                            {
                                prefix = "社会招生招收";
                            }
                            if (recruit.Type == 6) {
                                prefix = "自主招生招收";
                            }
                            if (recruit?.PlanQuantity != null && recruit?.PlanQuantity > 0)
                            {
                                plantext.Add($"{recruit?.PlanQuantity}人");
                            }
                            else
                            {

                                if (recruit?.Quantity != null && recruit?.Quantity > 0)
                                {
                                    plantext.Add($"{recruit?.Quantity }人");
                                }
                                if (recruit?.ClassQuantity != null && recruit?.ClassQuantity > 0)
                                {
                                    plantext.Add($"{recruit?.ClassQuantity }个班");
                                }
                            }
                            if (plantext?.Any() == true)
                            {
                                pre_University.recruit_info.recruit_items.Add(new
                                {
                                    recruit_type = RecruitType.招生计划,
                                    can_jump = 0,
                                    descs = new List<string>() { $"{prefix}{string.Join("，", plantext)  }" },
                                });
                            }
                            if (!string.IsNullOrEmpty(recruit?.Target) && (schoolInfo.Grade == (byte)SchoolGrade.Kindergarten || schoolInfo.Grade == (byte)SchoolGrade.PrimarySchool))
                            {
                                //var targets = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(recruit?.Target);
                                pre_University.recruit_info.recruit_items.Add(new
                                {
                                    recruit_type = RecruitType.招生对象,
                                    can_jump = 0,
                                    descs = new List<string>() { recruit?.Target }

                                });
                            }
                            if (overviewOtherInfo?.RecruitWay_Obj != null)
                            {
                                pre_University.recruit_info.recruit_items.Add(new
                                {
                                    recruit_type = RecruitType.招生方式,
                                    can_jump = 0,
                                    descs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(overviewOtherInfo?.RecruitWay)
                                });
                            }

                            if (!string.IsNullOrEmpty(overviewOtherInfo?.OAAccount))
                            {
                                pre_University.relate_accts.Add(overviewOtherInfo?.OAAccount);
                            }
                            if (!string.IsNullOrEmpty(overviewOtherInfo?.MPAccount))
                            {
                                pre_University.relate_accts.Add(overviewOtherInfo?.MPAccount);
                            }
                            if (!string.IsNullOrEmpty(overviewOtherInfo?.VAAccount))
                            {
                                pre_University.relate_accts.Add(overviewOtherInfo?.VAAccount);
                            }

                        }

                        var serviceItems = GetSouYiSouServiceItems(schoolInfo, recruit);

                        pre_University.service_info.service_items.AddRange(serviceItems);
                        ///学校状态，1->下线 0->上线
                        pre_University.status = (schoolInfo.Status == SchoolStatus.Success && schoolInfo.IsValid) ? 0 : 1;
                        pre_Universitys.Add(pre_University);
                    }
                    UpsertPreUniversityBasicRequest upsertPreUniversityBasicRequest = new UpsertPreUniversityBasicRequest()
                    {
                        is_test = isTest ? 1 : 0,
                        pre_university_list = pre_Universitys.ToArray(),


                    };
                    var requestreturn = await _weChatService.UpsertPreUniversityBasic(upsertPreUniversityBasicRequest, PMS.Infrastructure.Application.Enums.WeChatAppChannel.fwh);
                    var jobj = JObject.Parse(requestreturn);
                    if (jobj["errcode"].Value<int>() != 0)
                    {
                        var errorlist = jobj["list"].ToArray();
                        for (int i = 0; i < errorlist.Length; i++)
                        {
                            var listItem = errorlist[i];
                            if (listItem["errcode"].Value<int>() != 0)
                            {
                                var fetchItem = fetchItems[i];
                                var schoolName = pre_Universitys[i].school_name;
                                var wxschoolid = pre_Universitys[i].wx_school_id;
                                var errmsg = listItem["errmsg"].Value<string>();
                                fails.Add(new
                                {
                                    eid = fetchItem.EId,
                                    schoolName,
                                    wxschoolid,
                                    errmsg
                                });
                                continue;
                            }
                        }

                    }
                    else
                    {
                        await UpdateWeChatSouYiSouSchoolBody(upsertPreUniversityBasicRequest);
                    }
                    retruns.Add(new
                    {
                        retruns = JsonConvert.DeserializeObject(requestreturn),
                        upsertPreUniversityBasicRequest = upsertPreUniversityBasicRequest
                    }
                     );
                });
            }

            return ResponseResult.Success(new
            {
                retruns,
                fails,
                unregisterInWeChatSchools,
                offlineSchools,
                total = itemDatas.Count
            });
        }


        private List<Service_Item> GetSouYiSouServiceItems(SchoolExtensionDto schoolInfo, OnlineSchoolRecruitInfo recruit)
        {
            List<Service_Item> items = new List<Service_Item>();
            var enumFiledInfos = typeof(SvrType).GetFields();
            foreach (var filedInfo in enumFiledInfos)
            {
                var permissionObjs = filedInfo.GetCustomAttributes(typeof(SvrTypePermissionAttribute), false);
                var svrTypeServiceUrl = filedInfo.GetCustomAttributes(typeof(SvrTypeServiceUrlAttribute), false).FirstOrDefault() as SvrTypeServiceUrlAttribute;
                if (permissionObjs?.Any() == false)
                {
                    continue;
                }
                foreach (SvrTypePermissionAttribute permission in permissionObjs)
                {
                    if (
                        (permission.City == schoolInfo.City || permission.City == 0)
                        &&
                        (permission.SchoolGrade == schoolInfo.Grade || permission.SchoolGrade == 0)
                        )
                    {
                        var srv_type = (SvrType)filedInfo.GetValue(null);
                        items.Add(new Service_Item()
                        {
                            jump_type = 1,
                            svr_type = srv_type,
                            service_url = svrTypeServiceUrl?.GetServiceUrl(UrlShortIdUtil.Long2Base32(schoolInfo.SchoolNo)),
                        });
                        break;
                    }

                }
            }
            List<Service_Item> afterNullFilters = new List<Service_Item>();
            foreach (var item in items)
            {
                if (item.svr_type == SvrType.学校概况)
                {
                    item.service_url = $"https://m.sxkid.com/school/wechat/{UrlShortIdUtil.Long2Base32(schoolInfo.SchoolNo)}";
                    afterNullFilters.Add(item);
                    continue;
                }
                if (item.svr_type == SvrType.招生简章)
                {
                    if (!string.IsNullOrEmpty(recruit?.Brief))
                    {
                        afterNullFilters.Add(item);

                    }
                    else
                    {
                        var areaRecruitPlans = _areaRecruitPlanService.GetByAreaCodeAndSchFType(schoolInfo.City.ToString(), schoolInfo.SchFType.Code).GetAwaiter().GetResult();
                        if (
                            recruit != null
                            ||
                            areaRecruitPlans?.Any() == true
                            )
                        {
                            afterNullFilters.Add(item);
                        }
                    }

                    continue;
                }
                if (item.svr_type == SvrType.招生范围)
                {
                    if (!string.IsNullOrEmpty(recruit?.Score))
                    {
                        switch (recruit.Type)
                        {
                            case 1:
                            case 3:
                                item.service_url = $"https://m.sxkid.com/school/wechat/detail/{UrlShortIdUtil.Long2Base32(schoolInfo.SchoolNo)}_type=2_ref=19";
                                break;
                            case 2:
                                item.service_url = $"https://m.sxkid.com/school/wechat/detail/{UrlShortIdUtil.Long2Base32(schoolInfo.SchoolNo)}_type=2_ref=42";
                                break;
                            case 4:
                                item.service_url = $"https://m.sxkid.com/school/wechat/detail/{UrlShortIdUtil.Long2Base32(schoolInfo.SchoolNo)}_type=2_ref=43";
                                break;
                            case 5:
                                item.service_url = $"https://m.sxkid.com/school/wechat/detail/{UrlShortIdUtil.Long2Base32(schoolInfo.SchoolNo)}_type=2_ref=44";
                                break;
                            case 6:
                                item.service_url = $"https://m.sxkid.com/school/wechat/detail/{UrlShortIdUtil.Long2Base32(schoolInfo.SchoolNo)}_type=2_ref=45";
                                break;

                        }
                        afterNullFilters.Add(item);
                    }
                    continue;
                }
                if (item.svr_type == SvrType.片区范围)
                {
                    if (!string.IsNullOrEmpty(recruit?.ScribingScopeStr) || recruit?.ScribingScope_Obj?.Any() == true)
                    {
                        afterNullFilters.Add(item);
                    }
                    continue;
                }
                if (item.svr_type == SvrType.对口地段)
                {
                    if (!string.IsNullOrEmpty(recruit.AllocationScore))
                    {
                        item.service_url = $"https://m.sxkid.com/school/wechat/detail/{UrlShortIdUtil.Long2Base32(schoolInfo.SchoolNo)}_type=2_ref=28";
                        afterNullFilters.Add(item);
                    }
                    else if (!string.IsNullOrEmpty(recruit.CounterpartScore))
                    {
                        item.service_url = $"https://m.sxkid.com/school/wechat/detail/{UrlShortIdUtil.Long2Base32(schoolInfo.SchoolNo)}_type=2_ref=29";
                        afterNullFilters.Add(item);
                    }

                    continue;
                }
                if (item.svr_type == SvrType.招生报名时间)
                {
                    if (recruit != null)
                    {
                        var schedules = _onlineSchoolRecruitService.GetRecruitSchedules(schoolInfo.City, new[] { recruit.Type }, schoolInfo.SchFType.ToString(), schoolInfo.Area).GetAwaiter().GetResult();
                        if ((schedules?.Any()).GetValueOrDefault() == true)
                        {
                            switch (recruit.Type)
                            {
                                case 1:
                                    item.service_url = $"https://m.sxkid.com/school/wechat/detail/{UrlShortIdUtil.Long2Base32(schoolInfo.SchoolNo)}_type=2_ref=24";
                                    break;
                                case 2:
                                    item.service_url = $"https://m.sxkid.com/school/wechat/detail/{UrlShortIdUtil.Long2Base32(schoolInfo.SchoolNo)}_type=2_ref=51";
                                    break;
                                case 3:
                                    item.service_url = $"https://m.sxkid.com/school/wechat/detail/{UrlShortIdUtil.Long2Base32(schoolInfo.SchoolNo)}_type=2_ref=52";
                                    break;
                                case 4:
                                    item.service_url = $"https://m.sxkid.com/school/wechat/detail/{UrlShortIdUtil.Long2Base32(schoolInfo.SchoolNo)}_type=2_ref=53";
                                    break;
                                case 5:
                                    item.service_url = $"https://m.sxkid.com/school/wechat/detail/{UrlShortIdUtil.Long2Base32(schoolInfo.SchoolNo)}_type=2_ref=54";
                                    break;
                                case 6:
                                    item.service_url = $"https://m.sxkid.com/school/wechat/detail/{UrlShortIdUtil.Long2Base32(schoolInfo.SchoolNo)}_type=2_ref=55";
                                    break;

                            }

                            afterNullFilters.Add(item);
                        }

                    }
                    continue;
                }
                if (item.svr_type == SvrType.招生报名材料清单)
                {
                    if (!string.IsNullOrEmpty(recruit?.Material))
                    {
                        switch (recruit.Type)
                        {
                            case 1:
                                item.service_url = $"https://m.sxkid.com/school/wechat/detail/{UrlShortIdUtil.Long2Base32(schoolInfo.SchoolNo)}_type=2_ref=23";
                                break;
                            case 2:
                                item.service_url = $"https://m.sxkid.com/school/wechat/detail/{UrlShortIdUtil.Long2Base32(schoolInfo.SchoolNo)}_type=2_ref=46";
                                break;
                            case 3:
                                item.service_url = $"https://m.sxkid.com/school/wechat/detail/{UrlShortIdUtil.Long2Base32(schoolInfo.SchoolNo)}_type=2_ref=47";
                                break;
                            case 4:
                                item.service_url = $"https://m.sxkid.com/school/wechat/detail/{UrlShortIdUtil.Long2Base32(schoolInfo.SchoolNo)}_type=2_ref=48";
                                break;
                            case 5:
                                item.service_url = $"https://m.sxkid.com/school/wechat/detail/{UrlShortIdUtil.Long2Base32(schoolInfo.SchoolNo)}_type=2_ref=49";
                                break;
                            case 6:
                                item.service_url = $"https://m.sxkid.com/school/wechat/detail/{UrlShortIdUtil.Long2Base32(schoolInfo.SchoolNo)}_type=2_ref=50";
                                break;

                        }

                        afterNullFilters.Add(item);
                    }
                    continue;
                }
                if (item.svr_type == SvrType.划片范围)
                {
                    if (!string.IsNullOrEmpty(recruit?.ScribingScopeStr) || recruit?.ScribingScope_Obj?.Any() == true)
                    {
                        afterNullFilters.Add(item);
                    }
                    continue;
                }
                if (item.svr_type == SvrType.对口初中)
                {
                    if ((!string.IsNullOrWhiteSpace(schoolInfo.CounterPart) && !schoolInfo.CounterPart.Equals("暂未收录")) && schoolInfo.CounterPart != "[]")
                    {
                        if (JArray.Parse(schoolInfo.CounterPart).Count == 1)
                        {
                            afterNullFilters.Add(item);
                        }

                    }
                    continue;
                }
                if (item.svr_type == SvrType.小升初派位安排)
                {
                    if ((!string.IsNullOrWhiteSpace(schoolInfo.CounterPart) && !schoolInfo.CounterPart.Equals("暂未收录")) && schoolInfo.CounterPart != "[]")
                    {
                        if (JArray.Parse(schoolInfo.CounterPart).Count > 1)
                        {
                            afterNullFilters.Add(item);
                        }
                    }
                    continue;
                }
                if (item.svr_type == SvrType.积分入学)
                {
                    if (recruit?.Type == 3)
                    {
                        afterNullFilters.Add(item);
                    }

                    continue;
                }
                if (item.svr_type == SvrType.对口小学)
                {
                    if (recruit?.Type == 2 && recruit?.CounterpartPrimary_Obj != null)
                    {
                        afterNullFilters.Add(item);
                    }
                    continue;
                }
                if (item.svr_type == SvrType.指标到校)
                {
                    var quotas = _onlineSchoolQuotaService.GetByEID(schoolInfo.ExtId).GetAwaiter().GetResult();
                    if (quotas?.Any(q => q.Type == 3) == true)
                    {
                        afterNullFilters.Add(item);
                    }
                    continue;
                }
                if (item.svr_type == SvrType.指标分配)
                {
                    var quotas = _onlineSchoolQuotaService.GetByEID(schoolInfo.ExtId).GetAwaiter().GetResult();
                    if (quotas?.Any(q => q.Type != 3) == true)
                    {
                        afterNullFilters.Add(item);
                    }
                    continue;
                }
                if (item.svr_type == SvrType.自主招生)
                {
                    if (recruit?.Type == 6)
                    {
                        afterNullFilters.Add(item);
                    }
                    continue;
                }
                if (item.svr_type == SvrType.中考录取分数线)
                {
                    var fractions = _onlineSchoolFractionService.GetByEID(schoolInfo.ExtId).GetAwaiter().GetResult();
                    var fractions2 = _onlineSchoolFractionService.Get2ByEID(schoolInfo.ExtId).GetAwaiter().GetResult();
                    if (fractions?.Any() == true || fractions2?.Any() == true)
                    {
                        afterNullFilters.Add(item);
                    }
                    continue;
                }
                afterNullFilters.Add(item);
            }
            return afterNullFilters;
        }


        private async Task<List<string>> GetSchoolAlias(Guid eid)
        {
            string sql = @"SELECT nickname FROM OnlineSchoolExtension WHERE ISJSON(nickname) = 1 AND id = @eid";
            var nicknamejson = await _schoolDataDBContext.ExecuteScalarAsync<string>(sql, new { eid });
            if (!string.IsNullOrEmpty(nicknamejson))
            {
                return JsonConvert.DeserializeObject<List<string>>(nicknamejson);
            }
            else {
                return null;
            }
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


        /// <summary>
        /// 查询推给微信学校修改的原因
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        private async Task<string> GetWechatSchoolModifyReason(Guid eid)
        {
            StringBuilder reason = new StringBuilder();
            //新增：学校电话、学校地址、2022政策文件
            //修改：学校标签、招生方式、2021招生人数、2021招生班数
            //更新原因：信息更新核查修改
            //新增：学校电话、学校地址、2022政策文件、2022招生人数、2021招生班数
            //修改：
            //更新原因：传输新学校内容

            string sql = @"SELECT Attrs,IsModify 
  FROM [iSchoolData].[dbo].[WechatModifyLogInfo]
  WHERE EID = @eid and cast(CreateDate as date) = cast(GETDATE() as date)
  order by CreateDate desc";
            var attrModifys = await _schoolDataDBContext.QueryAsync(sql,new { eid});
            List<string> news = new List<string>();
            List<string> updates = new List<string>();
            foreach (var attrModifysGroup in attrModifys)
            {
                var attrs = JArray.Parse((string)attrModifysGroup.Attrs).Select(t => t.Value<string>());
                if ((bool)attrModifysGroup.IsModify) {
                    //修改
                    updates.AddRange(attrs);
                }
                else
                {
                    //新增
                    news.AddRange(attrs);
                }

            }
            if (news.Any())
            {
                reason.AppendLine(string.Format("新增：{0}", string.Join("、", news)));
            }
            if (updates.Any())
            {
                reason.AppendLine(string.Format("修改：{0}", string.Join("、", updates)));
            }
            if (reason.Length > 0)
            {
                reason.AppendLine("信息更新核查修改");
            }
            return reason.ToString();


        }

        private async Task UpdateWeChatSouYiSouSchoolBody(UpsertPreUniversityBasicRequest  request) {
            foreach (var item in request.pre_university_list)
            {
                string sql = @"
UPDATE [dbo].[WeChatSouYiSouSchool]
   SET 
      [LatestPushBody] =@LatestPushBody
      ,[LatestPushTime] = @LatestPushTime
 WHERE  WXSchoolId = @WXSchoolId";
                await _schoolDataDBContext.ExecuteAsync(sql, new
                {
                    LatestPushBody = Newtonsoft.Json.JsonConvert.SerializeObject(item),
                    LatestPushTime = DateTime.Now,
                    WXSchoolId = item.wx_school_id
                }) ;
            }

        }
    }
}
