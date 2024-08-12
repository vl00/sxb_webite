using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.Entitys.Signup;
using ProductManagement.API.SMS;
using ProductManagement.Framework.Cache.Redis;
using Sxb.Web.Areas.SignUp.Models;
using Sxb.Web.Response;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.Web.Areas.SignUp.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class QuestionnaireController : Controller
    {
        private readonly IQuestionnaireService _questionnaireService;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly SMSHelper _SMS;

        public QuestionnaireController(IQuestionnaireService questionnaireService, IEasyRedisClient easyRedisClient)
        {
            _questionnaireService = questionnaireService;
            _easyRedisClient = easyRedisClient;
            _SMS = new SMSHelper(_easyRedisClient);
        }


        [HttpPost]
        public ResponseResult AddSaleSign([FromBody] SaleSignData data)
        {
            
            SaleSignup sign = new SaleSignup
            {
                Name = data.Name,
                SignDate = data.SignDate,
                AgreeShare = data.AgreeShare,
                Grade = data.Grade,
                Phone = data.Phone
            };

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetId();
                sign.UserId = userId;
            }


            if (data.Choices.Count() < 10 || string.IsNullOrWhiteSpace(sign.Name)
                || string.IsNullOrWhiteSpace(sign.Phone) || sign.AgreeShare == null)
            {
                return ResponseResult.Failed("还有必填项未完成哦!");
            }

            if (string.IsNullOrWhiteSpace(data.Verifycode))
            {
                return ResponseResult.Failed("请输入验证码!");
            }

            if (!_SMS.CheckRndCode(86, data.Phone, data.Verifycode, "salesign", false))
            {
                return ResponseResult.Failed("验证码错误!");
            }


            sign.Question1 = string.Join(",", data.Choices[0].ToArray());
            sign.Question2 = string.Join(",", data.Choices[1].ToArray());
            sign.Question3 = string.Join(",", data.Choices[2].ToArray());
            sign.Question4 = string.Join(",", data.Choices[3].ToArray());
            sign.Question5 = string.Join(",", data.Choices[4].ToArray());
            sign.Question6 = string.Join(",", data.Choices[5].ToArray());
            sign.Question7 = string.Join(",", data.Choices[6].ToArray());
            sign.Question8 = string.Join(",", data.Choices[7].ToArray());
            sign.Question9 = string.Join(",", data.Choices[8].ToArray());
            sign.Question10 = string.Join(",", data.Choices[9].ToArray());
            sign.Question11 = data.Other;

            var result = _questionnaireService.AddSaleSign(sign);
            if (result.Item1)
            {
                return ResponseResult.Success("报名成功");
            }
            else
            {
                return ResponseResult.Failed(result.Item2);
            }

        }
    }
}
