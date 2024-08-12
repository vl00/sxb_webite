using Microsoft.AspNetCore.Mvc;
using PMS.Infrastructure.Application.IService;
using PMS.Infrastructure.Domain.Entities;
using PMS.UserManage.Application.IServices;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using Sxb.Web.Areas.Common.Models.SystemEventLog;
using Sxb.Web.Common;
using Sxb.Web.Controllers;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProductManagement.Tool.Email;
using System.Net.Mail;
using System.Text;
using System.Data;
using Sxb.Web.Utils;
using Sxb.Web.Filters;
using ProductManagement.Framework.AspNetCoreHelper.Filters;
using Microsoft.AspNetCore.Hosting;
using PMS.Infrastructure.Domain.Dtos;
using PMS.CommentsManage.Application.IServices;
using PMS.School.Application.IServices;
using System.Text.RegularExpressions;
using NPOIHelper;
using Sxb.Web.Areas.Common.WeChatQRCallBackHandle;
using WeChat.Model;

namespace Sxb.Web.Areas.Common.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SystemEventLogController : ApiBaseController
    {
        IKeyValueService _keyValueService;
        ISystemEventLogService _systemEventLogService;
        IUserService _userService;
        ITalentService _talentService;
        IEmailClient _emailClient;
        ISchoolService _schoolService;
        IWeixinSubscribeLogService _weixinSubscribeLogService;

        public SystemEventLogController(IEmailClient emailClient, ISystemEventLogService systemEventLogService, IUserService userService, ITalentService talentService, IKeyValueService keyValueService, ISchoolService schoolService, IWeixinSubscribeLogService weixinSubscribeLogService)
        {
            _systemEventLogService = systemEventLogService;
            _userService = userService;
            _talentService = talentService;
            _keyValueService = keyValueService;
            _emailClient = emailClient;
            _schoolService = schoolService;
            _weixinSubscribeLogService = weixinSubscribeLogService;
        }

        [HttpPost]
        public async Task<ResponseResult> AddLog([FromBody] AddLogRequest request)
        {
            SystemEventLog systemEventLog = new SystemEventLog()
            {
                AppName = request.AppName,
                AppVersion = await _keyValueService.GetValueFromCache("SxbAppVersion") ?? "undefine",
                Body = request.Body,
                Equipment = request.Equipment,
                CreateTime = request.Time,
                Creator = request.Creator,
                UserId = UserIdOrDefault,
                DeviceId = Request.GetDeviceToGuid(),
                Event = request.Event,
                EventId = request.EventId,
                IsTalent = _talentService.IsTalent(UserIdOrDefault.ToString()),
                System = request.System,
                Client = GetSystemEventLogClientType(),
                Location = HttpContext.Request.GetLocalCity().ToString()
            };
            bool res = await _systemEventLogService.AddLog(systemEventLog);

            return ResponseResult.Success(res, null);
        }


        public class A
        {
            public string b { get; set; }
            public int c { get; set; }

        }

        [HttpPost]
        [ExportDataFilter(ExportDataToMailRequestParamName = "request")]
        public async Task<ResponseResult> ExportPaidQAData([FromBody] ExportPaidQADataRequest request)
        {
            var rows = await _systemEventLogService.ExportPaidQAData(request.ETime.Date.AddDays(-7), request.ETime.Date);
            List<object> datas = new List<object>();
            foreach (var row in rows)
            {

                var root = JObject.Parse(row.Body);
                List<string> props = new List<string>();
                foreach (var property in root.Properties())
                {
                    if (property.Name.Equals("pagePath", StringComparison.CurrentCultureIgnoreCase)
                        ||
                        property.Name.Equals("version", StringComparison.CurrentCultureIgnoreCase))
                    {
                        continue;
                    }
                    if (property.Value.Type == JTokenType.Array)
                    {
                        ;
                        props.Add($"{ property.Name}={ string.Join(' ', property.Value.Select(tstr => tstr.ToString()?.Replace("\n", "")))}");
                    }
                    else
                    {
                        props.Add($"{ property.Name}={ property.Value.ToString()?.Replace("\n", "")}");
                    }

                }
                IDictionary<string, object> dic = new Dictionary<string, object>() {
                    { "点序号",row.Id.ToString()},
                    { "AppName",row.AppName},
                    { "AppVersion",row.AppVersion},
                    { "UserId",row.UserId.ToString()},
                    { "NickName",row.NickName},
                    { "Client",row.Client},
                    { "Equipment",row.Equipment},
                    { "Login", row.Login ? "1" : "0"},
                    { "Talent",row.Talent ? "1" : "0"},
                    { "System",row.System},
                    { "Location",row.Location},
                    { "Date",row.Date},
                    { "Time",row.Time},
                    { "Event",row.Event},
                    { "EventId",row.EventId},
                    { "私有属性",string.Join(';', props)},
                    { "版本",row.Version},

                };
                datas.Add(dic);
            }
            var sheets = new Dictionary<string, IEnumerable<dynamic>>() {
                {  "上学问数据埋点报表",datas}
            };
            Attachment attachment = new Attachment(contentStream: ExcelHelper.ToExcel(sheets), name: $"上学问数据埋点报表【{request.ETime.ToString("yyMMdd")}】.xlsx");
            await _emailClient.NotifyByMailAsync($"上学问数据埋点报表【{request.ETime.ToString("yyMMdd")}】", "上学问数据埋点报表。", request.MainMails.ToArray(), new List<object>() { attachment }, request.CCMails.ToArray());
            return ResponseResult.Success($"{request.BTime.Date}至{request.ETime.Date}");

        }


        /// <summary>
        /// 将真实的客户端分类转为
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public int GetSystemEventLogClientType()
        {
            var ct = Request.Headers.GetClientType();
            if (ct.HasFlag(ClientType.App))
            {
                return 4;
            }
            if (ct.HasFlag(ClientType.WxMinApp))
            {
                return 3;
            }
            if (ct.HasFlag(ClientType.Mobile))
            {
                return 1;
            }
            if (ct.HasFlag(ClientType.PC))
            {
                return 2;
            }
            return 0;

        }

        [HttpGet]
        public async Task<IActionResult> ExportWeChatDaysPvUv(DateTime? startTime, DateTime? endTime)
        {
            var data = await GetWeChatDaysPvUv(startTime, endTime);

            var excelName = $"微信院校库数据统计";
            if (startTime != null && endTime != null)
            {
                var start = startTime.Value.ToString("yyyyMMdd");
                var end = endTime.Value.ToString("MMdd");
                excelName += $"（{start}-{end}）";
            }

            var helper = NPOIHelperBuild.GetHelper(excelName);
            helper.Add("页面数据统计需求", data, new Column[] {
                new Column("Date", "日期", ColumnType.Date),
                new Column("SchoolName", "学校名称"),
                new Column("SchFType0Desc", "学校类型"),
                new Column("CityName", "学校所在城市"),
                new Column("AreaName", "学校所在行政区"),
                new Column("PagePath", "入口页面链接（包括锚点）"),
                new Column("AnchorName", "锚点"),
                new Column("Uv", "UV", ColumnType.Number),
                new Column("Pv", "PV", ColumnType.Number),
                new Column("WeixinPv", "点击品牌/好物标题关注或进入服务号次数", ColumnType.Number)
            });
            return File(helper.ToArray(), helper.ContentType, helper.FullName);
        }

        [HttpGet]
        public async Task<IActionResult> ExportBigKPvUv(DateTime? startTime, DateTime? endTime)
        {
            var data = await GetBigKPvUv(startTime, endTime);

            var excelName = $"政策大卡数据统计";
            if (startTime != null && endTime != null)
            {
                var start = startTime.Value.ToString("yyyyMMdd");
                var end = endTime.Value.ToString("MMdd");
                excelName += $"（{start}-{end}）";
            }

            var helper = NPOIHelperBuild.GetHelper(excelName);
            helper.Add("页面数据统计需求", data, new Column[] {
                new Column("Date", "日期", ColumnType.Date),
                new Column("Title", "页面标题"),
                new Column("PagePath", "入口页面链接（包括锚点）"),
                new Column("Uv", "UV", ColumnType.Number),
                new Column("Pv", "PV", ColumnType.Number),
                new Column("WeixinSubscribeUv", "成功关注服务号独立用户数（含旧用户）", ColumnType.Number),
                new Column("WeixinNewSubscribeUv", "成功关注服务号新独立用户数", ColumnType.Number),
                new Column("WeixinScanPv", "成功关注服务号人次数（含重复关注情况）", ColumnType.Number)
            });
            return File(helper.ToArray(), helper.ContentType, helper.FullName);
        }

        [HttpGet]
        public async Task<List<SystemEventLogWeChatPvUvDto>> GetWeChatDaysPvUv(DateTime? startTime, DateTime? endTime)
        {
            startTime = startTime ?? new DateTime(1970, 1, 1);
            endTime = endTime ?? DateTime.MaxValue;

            //查询puv
            var data = await GetDaysPvUvAsync(startTime, endTime);

            //查询学校信息
            var extIds = data.Where(s => s.ExtId != null).Select(s => s.ExtId.Value).Distinct();
            var schools = await _schoolService.GetSchoolNameTypeAreaAsync(extIds);

            //查询微信关注或进入服务号人数
            var types = new string[] { SubscribeQRCodeType.OrgDetailPage.ToString(), SubscribeQRCodeType.CourseDetailPage.ToString() };
            var weChatEvents = new List<WeChatEventEnum>() { WeChatEventEnum.subscribe, WeChatEventEnum.SCAN };
            var weixinPuvs = await _weixinSubscribeLogService.GetSubscribePvAsync(types, weChatEvents, isFirstSubscribe: null, startTime.GetValueOrDefault(), endTime.GetValueOrDefault());

            foreach (var item in data)
            {
                var school = schools.Where(s => s.ExtId == item.ExtId).FirstOrDefault();
                if (school != null)
                {
                    item.SchoolName = school.SchoolName;
                    item.SchFType0Desc = school.SchFType0.GetDesc();
                    item.CityName = school.CityName;
                    item.AreaName = school.AreaName;
                }

                item.WeixinPv = weixinPuvs.FirstOrDefault(s => s.FromWhere == item.PagePath && s.Date == item.Date)?.Pv ?? 0;
            }
            return data;
        }

        private async Task<List<SystemEventLogWeChatPvUvDto>> GetDaysPvUvAsync(DateTime? startTime, DateTime? endTime)
        {
            //https://m3.sxkid.com/school_detail_wechat/data/eid=092D223A-5D02-48FF-AE04-FD1C9683EB5D_type=1
            var host = Request.Host.ToString();
            //为了测试
            if (!new Regex("m.?\\.sxkid\\.com").IsMatch(host))
            {
                host = "m.sxkid.com";
            }
            //第一版前端url
            //var startWithUrl = $"https://{host}/school_detail_wechat/";
            //第二版前端url
            var startWithUrl = $"https://{host}/school/wechat/";

            var sources = await _systemEventLogService.GetDaysPvUv(startWithUrl, startTime, endTime);
            var data = CommonHelper.MapperProperty<SystemEventLogPvUvDto, SystemEventLogWeChatPvUvDto>(sources).ToList();



            //从补全信息
            //根据短链接换取长连接
            var schoolShortNos = data.Where(s => s.ExtId == null && s.SchoolShortNo > 0)
                .Select(s => s.SchoolShortNo)
                .ToList();

            var exts = _schoolService.GetExtIdByNosAsync(schoolShortNos).GetAwaiter().GetResult();
            foreach (var item in data)
            {
                if (item.ExtId == null)
                {
                    var (No, ExtId) = exts.FirstOrDefault(s => s.No == item.SchoolShortNo);
                    if (No > 0)
                    {
                        item.ExtId = ExtId;
                    }
                }
            }
            return data;
        }



        [HttpGet]
        public async Task<List<SystemEventLogBigKPvUvDto>> GetBigKPvUv(DateTime? startTime, DateTime? endTime)
        {
            startTime = startTime ?? new DateTime(1970, 1, 1);
            endTime = endTime ?? DateTime.MaxValue;

            var host = Request.Host.ToString();
            //为了测试
            if (!new Regex("m.?\\.sxkid\\.com").IsMatch(host))
            {
                host = "m3.sxkid.com";
            }
            //前端url
            //https://m3.sxkid.com/recruit/policy/areaCode=440104_grade=1
            var startWithUrl = $"https://{host}/recruit/";

            //查询puv
            var sources = await _systemEventLogService.GetDaysPvUv(startWithUrl, startTime, endTime);
            var data = CommonHelper.MapperProperty<SystemEventLogPvUvDto, SystemEventLogBigKPvUvDto>(sources).ToList();

            var types = new string[] { SubscribeQRCodeType.CourseDetailPage.ToString() };
            var weChatEvents = new List<WeChatEventEnum>() { WeChatEventEnum.subscribe, WeChatEventEnum.SCAN };
            //查询微信关注服务号人数（含取关再关）
            var weixinSubscribePuvs = await _weixinSubscribeLogService.GetSubscribePvAsync(types, weChatEvents, isFirstSubscribe: false, startTime.GetValueOrDefault(), endTime.GetValueOrDefault());
            //查询微信新关注服务号人数
            var weixinNewSubscribePuvs = await _weixinSubscribeLogService.GetSubscribePvAsync(types, weChatEvents, isFirstSubscribe:true, startTime.GetValueOrDefault(), endTime.GetValueOrDefault());
            //查询微信关注或进入服务号次数
            var weChatEventsScan = new List<WeChatEventEnum>() { WeChatEventEnum.SCAN };
            var weixinScanPuvs = await _weixinSubscribeLogService.GetSubscribePvAsync(types, weChatEventsScan, isFirstSubscribe:false, startTime.GetValueOrDefault(), endTime.GetValueOrDefault());

            foreach (var item in data)
            {
                item.WeixinSubscribeUv = weixinSubscribePuvs.FirstOrDefault(s => s.FromWhere == item.PagePath && s.Date == item.Date)?.Pv ?? 0;
                item.WeixinNewSubscribeUv = weixinNewSubscribePuvs.FirstOrDefault(s => s.FromWhere == item.PagePath && s.Date == item.Date)?.Pv ?? 0;
                item.WeixinScanPv = weixinScanPuvs.FirstOrDefault(s => s.FromWhere == item.PagePath && s.Date == item.Date)?.Pv ?? 0;
            }
            return data;
        }
    }
}
