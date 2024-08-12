using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.DTOs;
using PMS.OperationPlateform.Domain.Entitys;
using ProductManagement.Infrastructure.Models;
using Sxb.Web.Areas.Article.Models.ArticleChoiceHotPoint;
using Sxb.Web.Common;
using Sxb.Web.Controllers;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Article.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ArticleChoiceHotPointController : ApiBaseController
    {
        IArticleChoiceHotPointService _articleChoiceHotPointService;
        public ArticleChoiceHotPointController(IArticleChoiceHotPointService articleChoiceHotPointService)
        {
            _articleChoiceHotPointService = articleChoiceHotPointService;
        }

        [HttpGet]
        [Description("热点列表")]
        public async Task<ResponseResult<IEnumerable<ArticleChoiceHotPoint>>> HotPoints()
        {
            //城市ID
            var city = Request.GetLocalCity();
            IEnumerable<ArticleChoiceHotPoint> result = await _articleChoiceHotPointService.GetHotPoints(city);
            return ResponseResult.TSuccess(result, "OK");

        }

        [HttpGet]
        [Description("热点下的文章列表")]
        public async Task<ResponseResult<PaginationModel<ArticleDto>>> GetArticles(Guid hotpointId, int pageIndex = 1, int pageSize = 20)
        {
            var pageResult = await _articleChoiceHotPointService.GetArticles(hotpointId, (pageIndex - 1) * pageSize, pageSize);
            return ResponseResult.TSuccess<PaginationModel<ArticleDto>>(pageResult, "OK"); ;

        }

    }
}
