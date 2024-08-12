using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMS.Infrastructure.Application.IService;
using PMS.Infrastructure.Application.ModelDto;
using ProductManagement.Framework.Cache.Redis;
using Sxb.UserCenter.Response;
using Sxb.UserCenter.Utils;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.UserCenter.Controllers
{
    public class ApiCommonController : Controller
    {
        private readonly ICityInfoService _cityCodeService;
        private readonly IEasyRedisClient _easyRedisClient;

        public ApiCommonController(ICityInfoService cityCodeService, IEasyRedisClient easyRedisClient)
        {
            _cityCodeService = cityCodeService;
            _easyRedisClient = easyRedisClient;
        }

        [Description("切换城市")]
        public async  Task<ResponseResult> ChangeCity(int localCity) 
        {
            if (localCity == 0)
            {
                //获取当前用户所在城市
                localCity = Request.GetLocalCity();
            }
            else
            {
                //切换城市
                Response.SetLocalCity(localCity.ToString());
            }
            
            var localCityInfo = await _cityCodeService.GetInfoByCityCode(localCity);
            return ResponseResult.Success(localCityInfo);
        }

        [Description("获取城市信息")]
        public async Task<ResponseResult> City() 
        {

            int localCity = Request.GetLocalCity();

            Guid userId = Guid.Empty;
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.GetUserInfo();
                userId = user.UserId;
            }


            var adCodes = await _cityCodeService.IntiAdCodes();

            var localCityInfo = adCodes.SelectMany(p => p.CityCodes).FirstOrDefault(p => p.Id == localCity);

            List<AreaDto> history = new List<AreaDto>();
            if (userId != default(Guid))
            {
                string Key = $"SearchCity:{userId}";
                var historyCity = await _easyRedisClient.SortedSetRangeByRankAsync<AreaDto>(Key, 0, 10, StackExchange.Redis.Order.Descending);
                history.AddRange(historyCity.ToList());
            }

            List<AreaDto> hottest = await _cityCodeService.GetHotCity();

            AreaDto local = new AreaDto();
            if (localCityInfo != null) 
            {
                local.AdCode = localCityInfo.Id;
                local.AreaName = localCityInfo.Name;
            }

            return ResponseResult.Success(new { local, history , hottest,allCity = adCodes });
        }

    }
}
