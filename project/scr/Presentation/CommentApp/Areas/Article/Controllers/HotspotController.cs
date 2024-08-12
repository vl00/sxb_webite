using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMS.OperationPlateform.Application.Dtos;
using PMS.OperationPlateform.Application.IServices;
using Sxb.Web.Controllers;
using Sxb.Web.Response;
using System.Text.RegularExpressions;
using PMS.OperationPlateform.Domain.Entitys;
using AutoMapper;
using Sxb.Web.Areas.Article.Models.SchoolActivity;
using Microsoft.AspNetCore.Authorization;
using Sxb.Web.Common;
using ProductManagement.Framework.Foundation;
using Nest;
using PMS.School.Application.IServices;
using Sxb.Web.Areas.Common.Models;
using Sxb.Web.Areas.Common.Models.Search;

namespace Sxb.Web.Areas.Article.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class HotspotController : ApiBaseController
    {
        private readonly IHotspotService _hotspotService;
        private readonly ISchoolService _schoolService;

        public HotspotController(IHotspotService hotspotService, ISchoolService schoolService)
        {
            _hotspotService = hotspotService;
            _schoolService = schoolService;
        }

        /// <summary>
        /// 底纹推荐词
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Placeholders")]
        public async Task<ResponseResult> GetPlaceholders(int? cityId = null, ClientType clientType = ClientType.PC)
        {
            cityId = cityId ?? Request.GetAvaliableCity();
            var data = await _hotspotService.GetPlaceholders(cityId.GetValueOrDefault(), clientType);
            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 热门推荐词
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("HotWords")]
        public async Task<ResponseResult> GetHotWords(int? cityId = null, ClientType clientType = ClientType.PC)
        {
            cityId = cityId ?? Request.GetAvaliableCity();
            var data = await _hotspotService.GetHotWords(cityId.GetValueOrDefault(), clientType);
            return ResponseResult.Success(data);
        }


        /// <summary>
        /// 热门学校
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("HotSchools")]
        public async Task<ResponseResult> GetHotSchools(int? cityId = null, ClientType clientType = ClientType.PC)
        {
            cityId = cityId ?? Request.GetAvaliableCity();
            var data = await _hotspotService.GetHotSchools(cityId.GetValueOrDefault(), clientType);

            var extIds = data.Select(s => s.DataId.GetValueOrDefault());
            var schools = await _schoolService.SearchSchools(extIds, 0, 0);

            var vm = schools.Select(s => HotSchoolItemVM.Convert(s));
            return ResponseResult.Success(vm);
        }
    }
}
