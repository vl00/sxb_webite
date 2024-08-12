using iSchool;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NSwag.Annotations;
using PMS.School.Application.IServices;
using PMS.School.Application.ModelDto;
using PMS.School.Domain.Common;
using PMS.School.Domain.Dtos;
using ProductManagement.Framework.Foundation;
using Sxb.Web.Models;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Sxb.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToFenghuangtongController : ApiBaseController
    {
        private readonly IToFenghuangtongService _toFenghuangtongService;

        public ToFenghuangtongController(IToFenghuangtongService toFenghuangtongService)
        {
            _toFenghuangtongService = toFenghuangtongService;
        }

        /// <summary>
        /// 通过学校ID查询学校名称+学部名称+经纬度坐标
        /// </summary>
        /// <param name="id">学部id</param>
        /// <returns></returns>
        [HttpGet("SchoolExtLatLongInfo")]
        [ProducesResponseType(typeof(SchExtDto1), 200)]
        public async Task<ResponseResult<SchExtDto1>> GetSchoolExtLatLongInfo(string id)
        {
            var eid = default(Guid);
            var eno = default(long);
            if (!Guid.TryParse(id, out eid))
            {
                eno = UrlShortIdUtil.Base322Long(id);
            }
            var result = await _toFenghuangtongService.GetSchoolExtLatLongInfo(eid, eno);
            return ResponseResult<SchExtDto1>.Success(result, "OK");
        }

    }
}