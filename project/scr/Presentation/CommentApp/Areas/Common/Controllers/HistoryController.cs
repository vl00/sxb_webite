using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PMS.UserManage.Application.IServices;
using Sxb.Web.Controllers;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PMS.UserManage.Domain.Common.EnumSet;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.Web.Areas.Common.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryController : ApiBaseController
    {
        private readonly IHistoryService _historyService;

        public HistoryController(IHistoryService historyService)
        {
            _historyService = historyService;
        }


        /// <summary>
        /// 更新浏览数最高的数据Id
        /// 浏览数前100的学校id
        /// 浏览数前100的点评id
        /// 浏览数前100的问题id
        /// </summary>
        /// <returns></returns>
        [HttpGet("RefreshTopViewCountCache")]
        [AllowAnonymous]
        public async Task<ResponseResult> RefreshTopViewCountCache()
        {
            await _historyService.RefreshAndGetTop(MessageDataType.School);
            await _historyService.RefreshAndGetTop(MessageDataType.Comment);
            await _historyService.RefreshAndGetTop(MessageDataType.Question);

            //_historyService.RefreshAndGetTop(MessageDataType.Article);
            return ResponseResult.Success();
        }

        [HttpGet("BulkSchoolViewCount")]
        [AllowAnonymous]
        public async Task<ResponseResult> BulkSchoolViewCount()
        {
            await _historyService.BulkSchoolViewCount();
            return ResponseResult.Success();
        }
    }
}
