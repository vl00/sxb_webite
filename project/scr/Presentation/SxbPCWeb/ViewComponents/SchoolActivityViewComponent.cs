using Microsoft.AspNetCore.Mvc;
using PMS.OperationPlateform.Application.IServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PCWeb.ViewComponents
{
    public class SchoolActivityViewComponent : ViewComponent
    {

        private readonly ISchoolActivityService _schoolActivityService;

        public SchoolActivityViewComponent(ISchoolActivityService schoolActivityService)
        {
            _schoolActivityService = schoolActivityService;
        }

        public IViewComponentResult Invoke(Guid extId)
        {
            ViewBag.ExtId = extId;
            var activity = extId == Guid.Empty ? null : _schoolActivityService.GetActivityByExtIdOrDefault(extId).Data;
            //ViewBag.Activity = activity;
            return View(activity);
        }
    }
}
