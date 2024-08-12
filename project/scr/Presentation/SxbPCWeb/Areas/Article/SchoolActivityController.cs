using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMS.OperationPlateform.Application.Dtos;
using PMS.OperationPlateform.Application.IServices;
using Sxb.PCWeb.Controllers;
using Sxb.PCWeb.Response;

namespace Sxb.PCWeb.Areas.Article.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SchoolActivityController : ApiBaseController
    {
        private readonly ISchoolActivityService _schoolActivityService;

        public SchoolActivityController(ISchoolActivityService schoolActivityService)
        {
            _schoolActivityService = schoolActivityService;
        }

        /// <summary>
        /// 获取单一留资 信息
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        public ResponseResult<SchoolActivityDetailDto> GetActivityByExtId(Guid extId)
        {
            return _schoolActivityService.GetActivityByExtId(extId);
        }

        /// <summary>
        /// 获取统一留资信息
        /// </summary>
        /// <returns></returns>
        public ResponseResult<SchoolActivityDetailDto> GetCommonActivity()
        {
            return _schoolActivityService.GetCommonActivity();
        }

        /// <summary>
        /// 留资报名
        /// </summary>
        /// <param name="registerDto"></param>
        /// <returns></returns>
        public ResponseResult<ResponseSchoolActivityRegister> Register([FromBody] SchoolActivityRegisterDto registerDto)
        {

            registerDto.CreatorId = userId;
            return _schoolActivityService.Register(registerDto);
        }

        /// <summary>
        /// 发送报名验证码
        /// </summary>
        /// <param name="extensionId"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        public async Task<ResponseResult> SendCode(Guid extensionId, string phone)
        {
            return await _schoolActivityService.SendCode(extensionId, phone);
        }
    }
}
