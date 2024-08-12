using Microsoft.AspNetCore.Mvc;
using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.Entitys.Signup;
using PMS.School.Infrastructure.Common;
using Sxb.Web.Areas.SignUp.Models;
using Sxb.Web.Response;
using System;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.Web.Areas.SignUp.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class HurunController : Controller
    {

        IHurunService _HurunService;

        public HurunController(IHurunService hurunService)
        {
            _HurunService = hurunService;
        }

        /// <summary>
        /// 新增接口
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> Sign([FromBody] HurunSignUpData data)
        {
            var entity = new HurunReportSignup()
            {
                Name = data.Name,
                Contact = data.Contact,
                EastSchools = data.EastSchools is null ? null : JsonHelper.ObjectToJSON(data.EastSchools),
                SouthSchools = data.SouthSchools is null ? null : JsonHelper.ObjectToJSON(data.SouthSchools),
                WesternSchools = data.WesternSchools is null ? null : JsonHelper.ObjectToJSON(data.WesternSchools),
                NorthernSchools = data.NorthernSchools is null ? null : JsonHelper.ObjectToJSON(data.NorthernSchools),
                Job = data.Job,
                Organization = data.Organization,
                Question3 = data.Question3,
                Question4 = data.Question4,
                WorkingYears = data.WorkingYears,
                CreateTime = DateTime.Now,
                CityName = string.IsNullOrWhiteSpace(data.CityName) ? "未知" : data.CityName
            };

            var result = await _HurunService.Add(entity);
            if (result.Item1)
            {
                return ResponseResult.Success(result.Item2);
            }
            else
            {
                return ResponseResult.Failed(result.Item2);
            }

        }
    }
}
