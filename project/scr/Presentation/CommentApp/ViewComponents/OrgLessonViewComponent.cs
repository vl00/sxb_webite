using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ProductManagement.API.Http.Model.Org;
using ProductManagement.Framework.Foundation;
using Sxb.Web.Response;
using Sxb.Web.ViewModels.ViewComponent;
using System.Collections.Generic;

namespace Sxb.Web.ViewComponents
{
    public class OrgLessonViewComponent : ViewComponent
    {
        IConfiguration _configuration;
        string _apiUrl;

        public OrgLessonViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
            if (_configuration != null)
            {
                var baseUrl = _configuration.GetSection("ExternalInterface").GetValue<string>("OrganizationAddress");
                if (!string.IsNullOrWhiteSpace(baseUrl))
                {
                    _apiUrl = $"{baseUrl}api/ToSchools/hotsell/coursesandorgs?minage={{0}}&maxage={{1}}";
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="grade">1:幼儿园 2:小学 3:初中 4:高中</param>
        /// <returns></returns>
        public IViewComponentResult Invoke(byte grade)
        {
            if (string.IsNullOrWhiteSpace(_apiUrl)) return View();
            var ageMin = 3;
            var ageMax = 6;
            switch (grade)
            {
                case 2:
                    ageMin = 6;
                    ageMax = 12;
                    break;
                case 3:
                    ageMin = 12;
                    ageMax = 15;
                    break;
                case 4:
                    ageMin = 0;
                    ageMax = 3;
                    break;
            }
            try
            {
                var webClient = new WebUtils();
                var apiResponse = webClient.DoGet(string.Format(_apiUrl, ageMin, ageMax));
                if (!string.IsNullOrWhiteSpace(apiResponse))
                {

                    var object_Response = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseResult<OrgLessonViewModel>>(apiResponse);

                    if (object_Response.Succeed)
                    {
                        return View(Newtonsoft.Json.JsonConvert.DeserializeObject<OrgLessonViewModel>(Newtonsoft.Json.JsonConvert.SerializeObject(object_Response.Data)));
                    }
                }
            }
            catch
            {
            }

            return View(new OrgLessonViewModel()
            {
                HotSellCourses = new List<HotSellCourse>(),
                RecommendOrgs = new List<RecommendOrg>()
            });
        }

    }
}
