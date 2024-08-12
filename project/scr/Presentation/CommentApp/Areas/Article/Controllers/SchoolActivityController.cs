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
    [Route("api/[controller]/[action]")]
    public class SchoolActivityController : ApiBaseController
    {
        private readonly ISchoolActivityService _schoolActivityService;

        private IMapper _mapper;

        public SchoolActivityController(ISchoolActivityService schoolActivityService, IMapper mapper)
        {
            _schoolActivityService = schoolActivityService;
            _mapper = mapper;
        }

        /// <summary>
        /// 获取留资 信息
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        public ResponseResult<SchoolActivityDetailDto> GetActivity(Guid id)
        {
            return _schoolActivityService.GetActivity(id);
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
        /// <param name="isFromSignInPage"></param>
        /// <returns></returns>
        [HttpPost]
        public ResponseResult<ResponseSchoolActivityRegister> Register([FromBody] SchoolActivityRegisterDto registerDto)
        {
            registerDto.CreatorId = UserId;
            return _schoolActivityService.Register(registerDto);
        }

        /// <summary>
        /// 留资报名
        /// </summary>
        /// <param name="registerDto"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ResponseResult<ResponseSchoolActivityRegister> CommonRegister([FromBody] SchoolActicityUnCheckRegisterDto registerDto)
        {
            return _schoolActivityService.UnCheckRegister(registerDto);
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

        [HttpGet]
        [Description("签到")]
        public async Task<ResponseResult> SignIn(
            [FromQuery] Guid activityId,
            [FromQuery] string phone)
        {
            Regex regex_mobile = new Regex(@"^(1[0-9][0-9])\d{8}$");
            if (!regex_mobile.IsMatch(phone))
            {
                return ResponseResult.Failed("无效的手机号码");
            }
            //检查是否报名状态。
            bool HasRegister;
            bool SignInSuccess = false;
            if (HasRegister = _schoolActivityService.ExistPhone(activityId, phone))
            {
                //已报名：进行签到 
                if (await _schoolActivityService.HasSignIn(activityId, phone))
                {
                    return ResponseResult.Failed("您已签到成功");
                }
                if (!(SignInSuccess = await _schoolActivityService.SignIn(activityId, phone, PMS.OperationPlateform.Domain.Enums.SignType.BaseSign)))
                {
                    //if 签到失败，返回操作失败
                    return ResponseResult.Failed("签到失败。");
                }

            }
            //默认返回报名状态、签到状态
            return ResponseResult.Success(new
            {
                HasRegister,
                SignInSuccess
            }, "操作成功。");
        }

        [HttpGet]
        [Description("获取活动流程")]
        public async Task<ResponseResult> GetActProcessInfo(Guid Id)
        {
            var ActivityResult = _schoolActivityService.GetActivity(Id);
            if (!ActivityResult.Status) {
                return ResponseResult.Failed(ActivityResult.Msg);
            }
            var ActivityInfo = _mapper.Map<SchoolActivityResult>(ActivityResult.Data);
            var Processs = _mapper.Map<IEnumerable<SchoolActivityProcessResult>>(await _schoolActivityService.GetProcesses(Id));

            return ResponseResult.Success(new {
                ActivityInfo,
                Processs
            }, "success");


        }


        [HttpGet]
        public async Task<ResponseResult> GetSignIns(Guid activityId)
        {
            var activity = await _schoolActivityService.GetBy(activityId);
            if (activity == null)
            {
                return ResponseResult.Failed("抱歉，找不到活动。");
            }

            var fields = await _schoolActivityService.GetRegisteFields(activityId);
            var namefield = fields.FirstOrDefault(fod => fod.Name.Contains("姓名"));
            if (namefield == null)
            {
                return ResponseResult.Failed("抱歉，找不到姓名字段。");
            }

            var registes = await _schoolActivityService.GetRegiste(activityId);
            var outputregistes = registes.Select(s => new
            {
                s.Id,
                Name = typeof(SchoolActivityRegiste).GetProperty($"Field{namefield.Sort}").GetValue(s),
                s.Phone,
                s.Channel,
                s.SignInType

            });
            var result = outputregistes.GroupBy(r => r.SignInType).ToDictionary((d) => $"SignInType_{d.Key}");
            return ResponseResult.Success(new { 
                Activity = activity,
                SignInfos = result
            }, "OK");



        }

    }
}
