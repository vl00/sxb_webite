using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSwag.Annotations;
using PMS.Infrastructure.Application.IService;
using PMS.Infrastructure.Application.ModelDto;
using ProductManagement.Framework.Cache.Redis;
using Sxb.Web.Common;

namespace Sxb.Web.Controllers
{
    [OpenApiIgnore]
    public class SchoolSearchController : Controller
    {
        private readonly IEasyRedisClient _easyRedisClient;
        readonly ICityInfoService _cityService;

        public SchoolSearchController(IEasyRedisClient easyRedisClient, ICityInfoService cityService)
        {
            _easyRedisClient = easyRedisClient;
            _cityService = cityService;
        }
        [Description("查询学校")]
        public async Task<IActionResult> Index(bool isComment,Guid BackSchoolId)
        {
            Guid userId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                userId = user.UserId;
            }

            string ToAction = "";
            if (isComment)
                ToAction = "/SchoolComment/Index?SchoolId=";
            else
                ToAction = "/Question/Index?SchoolId=";

            List<AreaDto> history = new List<AreaDto>();
            if (userId != default(Guid))
            {
                string Key = $"SearchCity:{userId}";
                var historyCity = await _easyRedisClient.SortedSetRangeByRankAsync<AreaDto>(Key, 0, 10, StackExchange.Redis.Order.Descending);
                history.AddRange(historyCity.ToList());
            }

            ViewBag.HotCity = JsonConvert.SerializeObject(await _cityService.GetHotCity());
            ViewBag.History = JsonConvert.SerializeObject(history);
            var adCodes = await _cityService.IntiAdCodes();
            //城市编码
            ViewBag.AllCity = JsonConvert.SerializeObject(adCodes);

            //获取当前用户所在城市
            var localCityCode = Request.GetLocalCity();

            //将数据保存在前台
            ViewBag.LocalCity = adCodes.SelectMany(p => p.CityCodes).FirstOrDefault(p => p.Id == localCityCode);

            ViewBag.redirect = ToAction + BackSchoolId;
            ViewBag.toUrl = ToAction;
            ViewBag.isComment = isComment;
            return View();
        }
    }

}