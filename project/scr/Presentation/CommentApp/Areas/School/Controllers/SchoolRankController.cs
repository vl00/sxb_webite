using Microsoft.AspNetCore.Mvc;
using PMS.CommentsManage.Application.Common;
using PMS.CommentsManage.Application.IServices;
using PMS.OperationPlateform.Application.IServices;
using PMS.School.Application.IServices;
using PMS.School.Domain.Dtos;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Request.Org;
using ProductManagement.Framework.Cache.Redis;
using Sxb.PC.Areas.School.Models;
using Sxb.Web.Areas.School.Models;
using Sxb.Web.Common;
using Sxb.Web.Controllers;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PC.Areas.School.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SchoolRankController : ApiBaseController
    {
        IOrgServiceClient _orgServiceClient;
        ISchoolRankService _schoolRankService;
        IEasyRedisClient _easyRedisClient;
        ISchoolService _schoolService;
        public SchoolRankController(IOrgServiceClient orgServiceClient,ISchoolRankService schoolRankService, IEasyRedisClient easyRedisClient, ISchoolService schoolService)
        {
            _schoolService = schoolService;
            _easyRedisClient = easyRedisClient;
            _schoolRankService = schoolRankService;
            _orgServiceClient = orgServiceClient;
        }

        /// <summary>
        /// 获取榜单
        /// <para>当地榜单不够时取全国榜单</para>
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseResult> GetLocalRanks()
        {
            var result = ResponseResult.Failed();
            var cityCode = Request.GetLocalCity();
            var finds = await GetRankAsync(cityCode);
            if (finds?.Any() == true)
            {
                result = ResponseResult.Success(finds);
            }
            return result;
        }
       

        /// <summary>
        /// 获取学校排行榜
        /// </summary>
        public async Task<List<SchoolRankApiModel>> GetRankAsync(int city = 0)
        {
            string cacheKey = $"GetTheNationwideAndNewsRanks-{city}";
            var result = await _easyRedisClient.GetOrAddAsync(cacheKey, async () =>
            {

                var RankResult = _schoolRankService.GetTheNationwideAndNewsRanks(city);
                var schoolIds = RankResult.schoolranks.SelectMany(sr =>
                {
                    return sr.SchoolRankBinds.Select(sb => sb.SchoolId);
                });
                var schools = await _schoolService.ListExtSchoolByBranchIds(schoolIds.ToList());
                return RankResult.schoolranks.Select(sr =>
                {
                    SchoolRankApiModel schoolRankApiModel = new SchoolRankApiModel()
                    {
                        RankId = sr.Id,
                        RankName = sr.Title,
                        Cover = sr.Cover,
                        IsShow = sr.IsShow.GetValueOrDefault(),
                        No = sr.No,
                        Sort = (int)sr.Rank,
                        ToTop = sr.ToTop.GetValueOrDefault()

                    };
                    schoolRankApiModel.Items = sr.SchoolRankBinds.Select(sb =>
                    {
                        var school = schools.FirstOrDefault(s => s.ExtId == sb.SchoolId);
                        return new SchoolRankApiModel.SchoolRankBindItems()
                        {
                            SchoolExtId = sb.SchoolId,
                            SchoolName = school?.Name,
                            Sort = (int)sb.Sort,
                            SchoolNo = school.SchoolNo
                        };
                    });

                    return schoolRankApiModel;
                }).ToList();

            }, DateTimeOffset.Now.AddDays(7));
            return result;
        }
    }
}

