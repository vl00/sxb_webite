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
    public class MixController : ApiBaseController
    {
        private readonly IMixService _mixService;

        public MixController(IMixService mixService)
        {
            _mixService = mixService;
        }

        /// <summary>
        /// 每日pv/uv统计更新
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("BuildDaysStatistics")]
        public async Task<ResponseResult> BuildDaysStatisticsAsync()
        {
            await _mixService.BuildDaysStatisticsAsync();
            return ResponseResult.Success();
        }
    }
}
