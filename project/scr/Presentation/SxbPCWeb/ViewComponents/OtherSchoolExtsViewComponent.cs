using Microsoft.AspNetCore.Mvc;
using PMS.School.Application.IServices;
using PMS.School.Domain.Common;
using PMS.School.Domain.Dtos;
using Sxb.PCWeb.Common;
using Sxb.PCWeb.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.PCWeb.ViewComponents
{
    public class OtherSchoolExtsViewComponent : ViewComponent
    {

        private readonly IHotPopularService _hotPopularService;
        private readonly ISchoolService _schoolService;
        private readonly ISchService _schService;

        //依赖注入
        public OtherSchoolExtsViewComponent(IHotPopularService hotPopularService, ISchoolService schoolService, ISchService schService)
        {
            _hotPopularService = hotPopularService;
            _schoolService = schoolService;
            _schService = schService;
        }

        public IViewComponentResult Invoke(Guid? eid = null)
        {
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

            //其他分部
            ViewData["OtherSchoolExts"] = new Lazy<List<KeyValueDto<Guid>>>(fn_lazy_OtherSchoolExts);
            List<KeyValueDto<Guid>> fn_lazy_OtherSchoolExts()
            {
                return _schoolService.GetSchoolExtName(schext.Sid);
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
