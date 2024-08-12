using Microsoft.AspNetCore.Mvc;
using PMS.School.Application.IServices;
using PMS.School.Domain.Common;
using PMS.School.Domain.Dtos;
using PMS.UserManage.Application.ModelDto.Collection;
using ProductManagement.Framework.Foundation;
using Sxb.PCWeb.Common;
using Sxb.PCWeb.Response;
using Sxb.PCWeb.ViewModels.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PCWeb.ViewComponents
{
    public class NearSchoolViewComponent : ViewComponent
    {

        private readonly IHotPopularService _hotPopularService;
        readonly ISchService _schService;

        //依赖注入
        public NearSchoolViewComponent(IHotPopularService hotPopularService, ISchService schService)
        {
            _hotPopularService = hotPopularService;
            _schService = schService;
        }

        public IViewComponentResult Invoke(Guid? eid = null)
        {
            var latitude = Convert.ToDouble(Request.GetLatitude());
            var longitude = Convert.ToDouble(Request.GetLongitude());

            var userCitycode = Request.GetLocalCity();
            //周边学校
            ViewData["NearestSchools"] = new Lazy<SmpNearestSchoolDto[]>(fn_lazy_NearestSchools);
            SmpNearestSchoolDto[] fn_lazy_NearestSchools()
            {
                SmpNearestSchoolDto[] schools = null;

                if (eid != null)
                {
                    schools = _hotPopularService.GetNearestSchools(eid.Value, 50).Result;
                }
                else
                {
                    schools = _hotPopularService.GetNearestSchools(userCitycode, (longitude, latitude), null, 50).Result;
                }

                if (schools?.Length > 0)
                {
                    var list_Schools = schools.ToList();
                    CommonHelper.ListRandom(list_Schools);
                    schools = list_Schools.Take(6).OrderByDescending(p=>p.TotalScore).ToArray();
                    var eids = schools.Select(p => p.Eid).Distinct().ToArray();
                    if (eids?.Length > 0)
                    {
                        var nos = _schService.GetSchoolextNo(eids);
                        if (nos?.Length > 0)
                        {
                            foreach (var item in schools)
                            {
                                if (nos.Any(p => p.Item1 == item.Eid)) item.SchoolNo = nos.FirstOrDefault(p => p.Item1 == item.Eid).Item2;
                            }
                        }
                    }
                }
                return schools;
            }
            return View();
        }

    }
}
