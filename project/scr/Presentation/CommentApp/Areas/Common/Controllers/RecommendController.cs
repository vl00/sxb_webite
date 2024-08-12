using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PMS.School.Application.IServices;
using PMS.UserManage.Application.IServices;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model.Recommend;
using Sxb.Web.Controllers;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using static PMS.UserManage.Domain.Common.EnumSet;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.Web.Areas.Common.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendController : ApiBaseController
    {
        private readonly ISchoolService _schoolService;

        public RecommendController(ISchoolService schoolService)
        {
            _schoolService = schoolService;
        }

        [HttpGet("Schools")]
        public async Task<ResponseResult> Schools(Guid extId, int pageIndex = 1, int pageSize = 10)
        {
            pageSize = pageSize > 100 ? 10 : pageSize;

            var data = await _schoolService.RecommendSchoolsAsync(extId, pageIndex, pageSize);
            return ResponseResult.Success(data);
        }
    }
}
