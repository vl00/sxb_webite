using Microsoft.AspNetCore.Mvc;
using PMS.School.Application.IServices;
using PMS.School.Domain.Common;
using PMS.School.Domain.Dtos;
using ProductManagement.Framework.Foundation;
using Sxb.PCWeb.Common;
using Sxb.PCWeb.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PCWeb.ViewComponents
{
    public class HotSchoolViewComponent : ViewComponent
    {

        private readonly IHotPopularService _hotPopularService;
        private readonly ISchService _schService;

        //依赖注入
        public HotSchoolViewComponent(IHotPopularService hotPopularService, ISchService schService)
        {
            _hotPopularService = hotPopularService;
            _schService = schService;
        }

        public IViewComponentResult Invoke(Guid? eid = null)
        {
            var userCitycode = Request.GetLocalCity();
            iSchool.SchFType0[] schtypes = null;

            var schext = eid != null ? GetSchoolSimpleInfo(eid.Value).Result : null;
            if (eid != null && schext == null) throw new ResponseResultException((int)ResponseCode.SchoolOffline, "学校不存在");
            if (schext != null)
            {
                if (!schext.IsValid) throw new ResponseResultException((int)ResponseCode.SchoolOffline, "学校已被删除了");
                if (schext.Status != (byte)SchoolStatus.Success) throw new ResponseResultException((int)ResponseCode.SchoolOffline, "学校可能已被下架");

                //schtypes = new[] { new SchFType0(schext.Grade, schext.Type, schext.Discount, schext.Diglossia, schext.Chinese) };
            }
            else
            {
                //从用户的关注中拿出学部总类型
                //...
            }
            //热门学校
            ViewData["HotVisitSchools"] = new Lazy<SimpleHotSchoolDto[]>(fn_lazy_HotVisitSchools);
            SimpleHotSchoolDto[] fn_lazy_HotVisitSchools()
            {
                var result = _hotPopularService.GetHotVisitSchools(userCitycode, schtypes, 50).Result;
                if (result?.Length > 0)
                {
                    var list_Result = result.ToList();
                    CommonHelper.ListRandom(list_Result);
                    result = list_Result.Take(6).OrderByDescending(p => p.TotalScore).ToArray();

                    var eids = result.Select(p => p.Eid).ToArray();
                    var nos = _schService.GetSchoolextNo(eids);
                    if (nos?.Length > 0)
                    {
                        ViewBag.SchoolNos = nos.ToDictionary(k => k.Item1, v => UrlShortIdUtil.Long2Base32(v.Item2));
                    }
                }
                return result;
            }

            return View();
        }

        public async Task<SchExtDto0> GetSchoolSimpleInfo(Guid eid, Guid sid = default)
        {
            if (HttpContext.Items.TryGetValue($"__SchextSimpleInfo_{eid}__", out var o) && o is SchExtDto0 schext)
                return ensure_schext() ? schext : null;

            schext = await _schService.GetSchextSimpleInfo(eid);
            ensure_schext();
            if (schext != null)
                HttpContext.Items[$"__SchextSimpleInfo_{eid}__"] = schext;
            return schext;

            bool ensure_schext()
            {
                if (schext == null) return false;
                if (!schext.IsValid || schext.Status != (byte)SchoolStatus.Success) return false;
                if (sid != Guid.Empty && schext.Sid != sid) return false;
                return true;
            }
        }

    }
}
