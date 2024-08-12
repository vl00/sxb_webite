using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.Entitys;
using Sxb.Web.Areas.Article.Models;
using Sxb.Web.Common;
using Sxb.Web.Controllers;
using Sxb.Web.Response;
using Sxb.Web.Utils;
using ProductManagement.Framework.Foundation;
using PMS.OperationPlateform.Domain.Enums;
using PMS.OperationPlateform.Application.Dtos;

namespace Sxb.Web.Areas.Article.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ArticleApiController : ApiBaseController
    {

        IArticleService _articleService;
        public ArticleApiController(IArticleService articleService, IArticleCoverService articleCoverService)
        {
            _articleService = articleService;
        }

        /// <summary>
        /// 目前只有百度小程序使用
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Description("获取文章列表")]
        public ResponsePageResult<IEnumerable<ArticleGetListResponse>> GetChoiceness([FromQuery]ArticleGetListRequest request)
        {
            string uuid = Request.GetDevice();
            var locationCityCode = Request.GetCity() == 0 ? Request.GetLocalCity() : Request.GetCity();
            var result = _articleService.GetChoiceness(new AppServicePageRequestDto<ArticleGetChoicenessRequestDto>()
            {
                input = new ArticleGetChoicenessRequestDto()
                {
                    CityCode = locationCityCode,
                    UUID = uuid,
                    UserId = base.UserId.GetValueOrDefault()
                },
                limit = request.Limit,
                offset = request.Offset
            });
            if (result.Status)
            {
                return ResponsePageResult<IEnumerable<ArticleGetListResponse>>.Success(result.Data.Select<ArticleGetChoicenessResultDto, ArticleGetListResponse>(s => s), result.Total);

            }
            else {
                return ResponsePageResult<IEnumerable<ArticleGetListResponse>>.Failed(result.Msg);
            }
        }

        /// <summary>
        /// 目前只有百度小程序使用
        /// </summary>
        /// <param name="id"></param>
        /// <param name="no"></param>
        /// <returns></returns>
        [HttpGet]
        [Description("获取文章详情信息")]
        public ResponseResult Detail([FromQuery] Guid id,[FromQuery] string no)
        {
            int no_int32 = 0;
            if (!string.IsNullOrEmpty(no))
            {
                no_int32 = (int)UrlShortIdUtil.Base322Long(no);
            }
            var result = this._articleService.GetDetail(new ArticleGetDetailRequestDto()
            {
                Id = id,
                No = no_int32
            });
            if (result.Status) {
                return ResponseResult.Success(result.Data,result.Msg);
            }
            else
            {
                return ResponseResult.Failed(result.Msg);
            }
        }


    }
}
