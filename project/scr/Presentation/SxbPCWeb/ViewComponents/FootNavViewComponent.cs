using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMS.Infrastructure.Application.IService;
using PMS.Infrastructure.Application.ModelDto;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Sxb.PCWeb.ViewComponents
{
    public class FootNavViewComponent : ViewComponent
    {
        private readonly ICityInfoService _cityService;
        public FootNavViewComponent(ICityInfoService cityService)
        {
            _cityService = cityService;
        }

        public IViewComponentResult Invoke()
        {
            ViewBag.Provinces = new Lazy<List<ProvinceCodeDto>>(fn_lazy_ProvinceCode);
            List<ProvinceCodeDto> fn_lazy_ProvinceCode()
            {
                return _cityService.GetProvince().Result;
            }
            return View();
        }
    }
}
