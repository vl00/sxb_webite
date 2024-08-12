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

namespace Sxb.Web.Areas.Article.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class UniversityController : ApiBaseController
    {
        private readonly IUniversityService _universityService;

        public UniversityController(IUniversityService universityService)
        {
            _universityService = universityService;
        }

        /// <summary>
        /// 根据Id获取高校详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ResponseResult> Get(int id)
        {
            var data = await _universityService.Get(id);
            return ResponseResult.Success(data);
        }

        /// <summary>
        /// 获取高校分页列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="cityId"></param>
        /// <param name="type"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>ram>
        /// <returns></returns>
        [HttpGet("pagination")]
        public async Task<ResponseResult> GetPagination(string keyword, int cityId, int? type, int pageIndex, int pageSize)
        {
            var data = await _universityService.GetPagination(keyword, cityId, type, pageIndex, pageSize);
            return ResponseResult.Success(data);
        }


        /// <summary>
        /// 获取高校推荐列表
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("recommends")]
        public async Task<ResponseResult> GetRecommends(int cityId, int pageIndex, int pageSize)
        {
            var data = await _universityService.GetRecommends(cityId, pageIndex, pageSize);
            return ResponseResult.Success(data);
        }
    }
}
