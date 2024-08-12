using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMS.OperationPlateform.Application.IServices;
using Sxb.Web.Response;

namespace Sxb.Web.Areas.Article.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class HuaneAsksApiController : ControllerBase
    {
        private readonly IHuaneAsksService _huaneAsksService;
        public HuaneAsksApiController(IHuaneAsksService huaneAsksService)
        {
            _huaneAsksService = huaneAsksService;
        }

        /// <summary>
        /// 获取详情信息
        /// </summary>
        [HttpGet]
        [Description("获取列表")]
        public async Task<ResponseResult> List(int pageIndex = 1,int pageSize = 40)
        {
            var result = await _huaneAsksService.GetQuestionLsit(pageIndex, pageSize);

            return ResponseResult.Success(new  {
                list = result.Item1,
                total = result.Item2,
                pageTotal = (result.Item2-1) / pageSize + 1,
                pageIndex = pageIndex
            });
        }

        /// <summary>
        /// 获取详情信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Description("获取详情信息")]
        public async Task<ResponseResult> Detail([FromQuery] int id)
        {
            var result = await _huaneAsksService.GetQuestionDatail(id);
            if (result == null)
            {
                return ResponseResult.Failed("问题不存在");
            }
            return ResponseResult.Success(result);
        }
    }
}
