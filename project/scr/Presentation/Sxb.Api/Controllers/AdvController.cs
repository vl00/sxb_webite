using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Sxb.Api.Controllers
{
    using Model;
    using Newtonsoft.Json;
    using PMS.OperationPlateform.Application.IServices;
    using Sxb.Api.Common;
    using Sxb.Api.Utils;
    using System.Web;

    /// <summary>
    /// 广告数据接口
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AdvController : ControllerBase
    {
        private readonly IAdvertisingBaseService _advertisingBaseService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="advertisingOptionService"></param>
        /// <param name="advertisingBaseService"></param>
        public AdvController(
            IAdvertisingBaseService advertisingBaseService
            )
        {

            _advertisingBaseService = advertisingBaseService;
        }

        /// <summary>
        /// 获取指定广告位置的配置信息
        /// </summary>
        /// <param name="location">广告位</param>
        /// <param name="callBack">回调函数</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAdvs([FromQuery] int[] location, [FromQuery] string callBack)
        {
            AdvResponseResult response = new AdvResponseResult()
            {
                Advs = new List<AdvResponseResult.AdvOption>()
            };
            var clientType = Request.GetClientType();
            var cityId = Request.GetAvaliableCity();
            try
            {
                foreach (var locationId in location)
                {
                    var advs = _advertisingBaseService.GetAdvertising(locationId, cityId);
                    if (clientType == UA.Mobile)
                    {
                        //手机端，链接要加前缀
                        advs = advs.Select(s =>
                        {
                            s.Url = $"ischool://web?url={HttpUtility.UrlEncode(s.Url)}";
                            return s;
                        });
                    }
                    response.Advs.Add(new AdvResponseResult.AdvOption()
                    {
                        Place = locationId,
                        Items = advs.ToList()
                    });
                }

                response.Status = Api.Response.ResponseCode.Success;
                response.ErrorMsg = "success";
            }
            catch (Exception ex)
            {
                response.Status = Api.Response.ResponseCode.Failed;
                response.ErrorMsg = ex.Message;
            }
            if (string.IsNullOrEmpty(callBack))
            {
                return new JsonResult(response);
            }
            else
            {
                //可能是jsonp
                var jsonResult = JsonConvert.SerializeObject(response);
                return Content($"{callBack}({jsonResult})", "text/html");
            }
        }
    }
}