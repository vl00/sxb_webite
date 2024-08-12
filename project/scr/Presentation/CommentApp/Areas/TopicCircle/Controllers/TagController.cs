using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMS.TopicCircle.Application.Dtos;
using PMS.TopicCircle.Application.Services;
using Sxb.Web.Response;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.Web.Areas.TopicCircle.Controllers
{

    /// <summary>
    /// 话题标签
    /// </summary>
    [ApiController]
    [Route("tpc/[controller]/[action]")]
    public class TagController : ControllerBase
    {
        ITagService _tagService;
        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [HttpGet]
        public ResponseResult<IEnumerable<TagDto>> GetTags()
        {
            AppServiceResultDto<IEnumerable<TagDto>> result = this._tagService.GetTags();
            return result;

        }

        [HttpGet]
        public ResponseResult<IEnumerable<TagHotDto>> GetHotTags(Guid circleId,int takeCount=3)
        {
            AppServiceResultDto<IEnumerable<TagHotDto>> result = this._tagService.GetHotTags(circleId, takeCount);
            return result;
        }
    }
}
