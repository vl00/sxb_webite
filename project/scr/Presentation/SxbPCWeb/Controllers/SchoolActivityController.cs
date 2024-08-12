using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMS.OperationPlateform.Application.IServices;
using PMS.School.Application.IServices;

namespace Sxb.PCWeb.Controllers
{
    public class SchoolActivityController : Controller
    {
        private readonly ISchoolActivityService _schoolActivityService;
        private readonly ISchoolService _schoolService;

        public SchoolActivityController(ISchoolActivityService schoolActivityService, ISchoolService schoolService)
        {
            _schoolActivityService = schoolActivityService;
            _schoolService = schoolService;
        }


        /// <summary>
        /// 仅显示单一留资, 如果没有进入404页面
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        [Description("学校留资详情")]
        [Route("/SchoolActivity/Index")]
        [Route("/retained-capital/abroad.html")]//移动端跳转
        public async Task<IActionResult> Index(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException("activity id not found");
            }

            /// 获取单一留资 信息
            var activity = _schoolActivityService.GetActivity(id).Data;
            if (activity.ExtId != null && activity.ExtId != Guid.Empty)
            {
                var school = await _schoolService.GetSchoolExtensionDetails(activity.ExtId.Value);
                ViewBag.School = school;
            }
            return View(activity);
        }


        [Description("学校详情")]
        public async Task<IActionResult> School(Guid extId)
        {
            var school = await _schoolService.GetSchoolExtensionDetails(extId);
            return View(school);
        }

    }
}
