using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMS.Infrastructure.Application.IService;
using PMS.Infrastructure.Application.ModelDto;
using PMS.School.Application.IServices;
using Sxb.PCWeb.RequestModel;
using Sxb.PCWeb.ViewModels;

namespace Sxb.PCWeb.Controllers
{
    public class NavController : Controller
    {

        private readonly ISchoolService _schoolService;
        private readonly ICityInfoService _cityService;

        public NavController(ISchoolService schoolService, ICityInfoService cityService)
        {
            _schoolService = schoolService;
            _cityService = cityService;
        }
        [Route("pr{option1}")]
        [Route("pr{option1}/pg{page}")]
        [Route("city1/{option2}")]
        [Route("city1/{option2}/pg{page}")]
        public async Task<IActionResult> List(string option1, string option2, int page = 1)
        {
            if (!(Request.Headers["User-Agent"].ToString().ToLower().Contains("spider")
                || Request.Headers["User-Agent"].ToString().ToLower().Contains("Googlebot")))
            {
                var redirectUrl = $"/school/list";
                Response.Redirect(redirectUrl);
                Response.StatusCode = 302;
                return Json(new { });
            }
            var data = GetQueryData(option1, option2);
            var city = data.City;
            var province = data.Province;
            var grade = data.Grade;
            var type = data.Type;

            int pageSize = 500;
            ViewBag.PageIndex = page;
            ViewBag.PageSize = pageSize;
            ViewBag.City = city;
            ViewBag.Province = province;
            ViewBag.Grade = grade;
            ViewBag.Type = type;

            ViewBag.Option1 = option1;
            ViewBag.Option2 = option2;

            var provinceInfo = new ProvinceCodeDto();
            if (province == null)
            {
                provinceInfo = await _cityService.GetProvinceInfoByCity(city ?? 0);
            }
            else if (city == null)
            {
                provinceInfo = await _cityService.GetProvinceInfo(province ?? 0);
            }
            var provinceName = provinceInfo.Province;
            ViewBag.ProvinceName = provinceName;
            var citys = provinceInfo.CityCodes;
            ViewBag.Citys = citys.Select(q=>new NavViewModel {
                Key = q.Id,
                Value = q.Name,
                Url = GetUrl(null, q.Id, grade, type, page),
            }).ToList();

            ViewBag.CityName = citys.FirstOrDefault(q => q.Id == city)?.Name;

            Dictionary<int, string> grades = new Dictionary<int, string>
            {
                { 1, "幼儿园" },
                { 2, "小学" },
                { 3, "初中" },
                { 4, "高中" }
            };
            ViewBag.Grades = grades.Select(q => new NavViewModel
            {
                Key = q.Key,
                Value = q.Value,
                Url = GetUrl(province, city, q.Key, type, page),
            }).ToList();
            ViewBag.GradeName = grades.FirstOrDefault(q => q.Key == grade).Value;

            Dictionary<int, string> types = new Dictionary<int, string>
            {
                { 1, "公办" },
                { 2, "民办" },
                { 3, "国际" },
                { 4, "外籍" }
            };
            ViewBag.Types = types.Select(q => new NavViewModel
            {
                Key = q.Key,
                Value = q.Value,
                Url = GetUrl(province, city, grade, q.Key, page),
            }).ToList();
            ViewBag.TypeName = types.FirstOrDefault(q => q.Key == type).Value;

            var schoolexts = _schoolService.GetSchoolExtensions(page, pageSize, province ?? 0, city ?? 0, grade ?? 0, type ?? 0);
            var total = _schoolService.GetSchoolExtensionsCount(province ?? 0, city ?? 0, grade ?? 0, type ?? 0);
            ViewBag.Schoolexts = schoolexts;
            ViewBag.Total = total;

            return View();
        }

        private NavData GetQueryData(string option1,string option2)
        {
            var data = new NavData();
            if (!string.IsNullOrWhiteSpace(option1))
            {
                string patternProvince = @"\d+(?=(gr\d+)?(ty\d+)?)";
                var pMatch = new Regex(patternProvince).Match(option1).Value;
                if (!string.IsNullOrWhiteSpace(pMatch))
                    data.Province = Convert.ToInt32(pMatch);

                string patternGrade = @"(?<=gr)\d+";
                var gMatch = new Regex(patternGrade).Match(option1).Value;
                if (!string.IsNullOrWhiteSpace(gMatch))
                    data.Grade = Convert.ToInt32(gMatch);

                string patternType = @"(?<=ty)\d+";
                var tMatch = new Regex(patternType).Match(option1).Value;
                if (!string.IsNullOrWhiteSpace(tMatch))
                    data.Type = Convert.ToInt32(tMatch);
            }
            else if(!string.IsNullOrWhiteSpace(option2))
            {
                string patternCity = @"(?<=c)\d+";
                var cMatch = new Regex(patternCity).Match(option2).Value;
                if (!string.IsNullOrWhiteSpace(cMatch))
                    data.City = Convert.ToInt32(cMatch);

                string patternGrade = @"(?<=gr)\d+";
                var gMatch = new Regex(patternGrade).Match(option2).Value;
                if (!string.IsNullOrWhiteSpace(gMatch))
                    data.Grade = Convert.ToInt32(gMatch);

                string patternType = @"(?<=ty)\d+";
                var tMatch = new Regex(patternType).Match(option2).Value;
                if (!string.IsNullOrWhiteSpace(tMatch))
                    data.Type = Convert.ToInt32(tMatch);
            }
            return data;
        }
        private string GetUrl(int? provinceCode, int? cityCode, int? grade, int? type ,int pageNo)
        {
            string url;
            if (provinceCode != null)
            {
                url = "/pr" + provinceCode;
            }
            else
            {
                url = "/city1/c" + cityCode;
            }
            if (grade != null)
            {
                url += "gr" + grade;
            }
            if (type != null)
            {
                url += "ty" + type;
            }
            if (pageNo > 1)
            {
                url += "/pg" + pageNo;
            }
            return url;
        }
    }
}
