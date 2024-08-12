using Microsoft.AspNetCore.Mvc;
using PMS.School.Application.IServices;
using Sxb.Web.Areas.Home.Models;
using Sxb.Web.Controllers;
using Sxb.Web.Response;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using Microsoft.Extensions.Configuration;
using Sxb.Web.Areas.Org.Model;

namespace Sxb.Web.Areas.Coupon.Controller
{
    [Route("Api/[controller]/[action]")]
    [ApiController]
    public class OrgController : ApiBaseController
    {
        string _apiOrgBaseUrl;

        IEasyRedisClient _easyRedisClient;
        IConfiguration _configuration;

        public OrgController(IEasyRedisClient easyRedisClient, IConfiguration configuration)
        {
            _easyRedisClient = easyRedisClient;
            _configuration = configuration;
            if (_configuration != null)
            {
                var baseUrl = _configuration.GetSection("ExternalInterface").GetValue<string>("OrganizationAddress");
                if (!string.IsNullOrWhiteSpace(baseUrl))
                {
                    _apiOrgBaseUrl = $"{baseUrl.TrimEnd('/')}";
                }
            }
        }

        /// <summary>
        /// 获取热销课程
        /// </summary>
        public async Task<ResponseResult> GetHotSellLesson(int count = 3)
        {
            var result = ResponseResult.Failed();

            var coursesAndOrgs = await _easyRedisClient.GetOrAddAsync("Org:HotSellCoursesAndOrgs", () =>
            {
                try
                {
                    var webClient = new WebUtils();
                    var apiResponse = webClient.DoGet($"{_apiOrgBaseUrl}/api/ToSchools/hotsell/coursesandorgs");
                    if (!string.IsNullOrWhiteSpace(apiResponse))
                    {
                        var object_Response = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseResultEx<CoursesAndOrgs>>(apiResponse);
                        if (object_Response?.Succeed == true && object_Response.Data != null)
                        {
                            return object_Response.Data;
                        }
                    }
                }
                catch
                {
                }
                return null;
            }, TimeSpan.FromHours(3));
            if (coursesAndOrgs?.HotSellCourses.Any() == true)
            {
                result = ResponseResult.Success(coursesAndOrgs.HotSellCourses.Take(count).Select(p => new
                {
                    ImgUrl = p.Banner,
                    p.Title,
                    p.Authentication,
                    p.Price,
                    Url = $"/org/course/detail/{p.ID_S}"
                }));
            }

            return result;
        }

        /// <summary>
        /// 获取热门机构
        /// </summary>
        public async Task<ResponseResult> GetRecommendOrg(int pageIndex = 1, int pageSize = 12, int type = 1)
        {
            var result = ResponseResult.Failed();

            var recommendOrgs = await _easyRedisClient.GetOrAddAsync($"Org:RecommendOrg_Type{type}_Page{pageIndex}_PageSize{pageSize}", () =>
            {
                try
                {
                    var webClient = new WebUtils();
                    var apiResponse = webClient.DoGet($"{_apiOrgBaseUrl}/api/ToSchools/SubjRecommendOrgs?pageIndex={pageIndex}&pageSize={pageSize}&type={type}");
                    if (!string.IsNullOrWhiteSpace(apiResponse))
                    {
                        var object_Response = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseResultEx<RecommendOrg>>(apiResponse);
                        if (object_Response?.Succeed == true && object_Response.Data != null)
                        {
                            return object_Response.Data;
                        }
                    }
                }
                catch
                {
                }
                return null;
            }, TimeSpan.FromHours(1));
            if (recommendOrgs?.PageInfo?.CurrentPageItems?.Any() == true)
            {
                result = ResponseResult.Success(recommendOrgs);
            }

            return result;
        }
    }
}
