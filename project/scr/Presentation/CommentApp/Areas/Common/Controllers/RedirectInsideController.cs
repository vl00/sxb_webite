using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class RedirectInsideController : ApiBaseController
    {
        private readonly IRecommendClient _recommendClient;

        public RedirectInsideController(IRecommendClient recommendClient)
        {
            _recommendClient = recommendClient;
        }

        [HttpPost("Add/School/ShortId")]
        [Description("新增学校跳转记录")]
        public async Task<object> AddSchoolRedirect([FromBody] ShortIdRedirectAddRequestDto requestDto)
        {
            return await _recommendClient.AddRedirectInsideSchool(requestDto.ReferShortId, requestDto.CurrentShortId);
        }

        [HttpPost("Add/Article/ShortId")]
        [Description("新增文章跳转记录")]
        public async Task<object> AddArticleRedirect([FromBody] ShortIdRedirectAddRequestDto requestDto)
        {
            return await _recommendClient.AddRedirectInsideArticle(requestDto.ReferShortId, requestDto.CurrentShortId);
        }
    }
}
