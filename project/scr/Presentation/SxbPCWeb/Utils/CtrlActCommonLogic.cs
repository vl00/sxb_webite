using iSchool;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using PMS.OperationPlateform.Application.IServices;
using PMS.School.Application.IServices;
using PMS.School.Domain.Dtos;
using PMS.Search.Application.IServices;
using ProductManagement.Infrastructure.Exceptions;
using Sxb.PCWeb.Common;
using Sxb.PCWeb.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolStatus = PMS.School.Domain.Common.SchoolStatus;

namespace Sxb.PCWeb.Utils
{
    public static class CtrlActCommonLogic
    {
        public static async Task<SchExtDto0> GetSchoolSimpleInfo(Controller controller, Guid eid, Guid sid = default)
        {
            var _this = controller;
            var HttpContext = _this.HttpContext;
            var services = HttpContext.RequestServices;

            if (HttpContext.Items.TryGetValue($"__SchextSimpleInfo_{eid}__", out var o) && o is SchExtDto0 schext)
                return ensure_schext() ? schext : null;

            var _schService = services.GetService<ISchService>();
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

        /// <summary>
        /// 侧边栏（延迟加载）
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="eid">学校详情需要学部id, 其他没学校的页面不需要学部id</param>
        public static void GetPageSideViewModel(Controller controller, Guid? eid = null)
        {
            var _this = controller;
            var ViewData = controller.ViewData;
            var HttpContext = controller.HttpContext;
            var services = HttpContext.RequestServices;
            var Request = HttpContext.Request;

            var _schService = services.GetService<ISchService>();
            var _hotPopularService = services.GetService<IHotPopularService>();
            var _schoolService = services.GetService<ISchoolService>();
            var _articleService = services.GetService<IArticleService>();
            var _dataSearch = services.GetService<ISearchService>();
            ////var pageSideViewModel = new PageSideViewModel();

            var latitude = Convert.ToDouble(Request.GetLatitude());
            var longitude = Convert.ToDouble(Request.GetLongitude());

            var userCitycode = Request.GetLocalCity();
            if (Request.Query["city"].ToString() is string city0 && !string.IsNullOrEmpty(city0))
            {
                userCitycode = Convert.ToInt32(city0);
                //Response.SetLocalCity(city0);
            }

            var user = HttpContext.User.Identity.IsAuthenticated ? HttpContext.User.Identity?.GetUserInfo() : null;

            var schext = eid != null ? GetSchoolSimpleInfo(_this, eid.Value).Result : null;
            if (eid != null && schext == null) throw new ResponseResultException((int)ResponseCode.SchoolOffline, "学校不存在");

            SchFType0[] schtypes = null;
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
                return _hotPopularService.GetHotVisitSchools(userCitycode, schtypes).Result;
            }

            //周边学校
            ViewData["NearestSchools"] = new Lazy<SmpNearestSchoolDto[]>(fn_lazy_NearestSchools);
            SmpNearestSchoolDto[] fn_lazy_NearestSchools()
            {
                if (eid != null) return _hotPopularService.GetNearestSchools(eid.Value).Result;
                else return _hotPopularService.GetNearestSchools(userCitycode, (longitude, latitude)).Result;
            }

            //热门攻略
            ViewData["HotStrategies"] = new Lazy<ViewModels.Article.ArticleListItemViewModel[]>(fn_lazy_HotStrategies);
            ViewModels.Article.ArticleListItemViewModel[] fn_lazy_HotStrategies()
            {
                throw new NotImplementedException();
            }

            ////...

            //其他分部
            ViewData["OtherSchoolExts"] = new Lazy<List<KeyValueDto<Guid>>>(fn_lazy_OtherSchoolExts);
            List<KeyValueDto<Guid>> fn_lazy_OtherSchoolExts()
            {
                return _schoolService.GetSchoolExtName(schext.Sid);
            }

            //选校关键词
            ViewData["SearchSchoolKeywords"] = new Lazy<IEnumerable<string>>(fn_SearchSchoolKeywords);
            IEnumerable<string> fn_SearchSchoolKeywords()
            {
                var channel = 0;
                return _dataSearch.GetHotHistoryList(channel, 8,null ,userCitycode);
            }

            //附近学校
            ViewData["NearSchools"] = new Lazy<SchExtDto0[]>(fn_lazy_NearSchools);
            SchExtDto0[] fn_lazy_NearSchools()
            {
                return _schService.GetNearSchoolsBySchType((longitude, latitude), schtypes).Result;
            }

            //推荐学校
            ViewData["RecommendSchools"] = new Lazy<List<KeyValueDto<Guid, Guid, string, string, int>>>(fn_lazy_RecommendSchools);
            List<KeyValueDto<Guid, Guid, string, string, int>> fn_lazy_RecommendSchools()
            {
                return _schoolService.RecommendSchoolAsync(eid.Value, schext.Type, schext.Grade, schext.City, 8).Result;
            }
        }

        // 文章列表项的时间格式
        private static string ArticleListItemTimeFormart(DateTime dateTime)
        {
            var passBy_H = (int)(DateTime.Now - dateTime).TotalHours;

            if (passBy_H > 24 || passBy_H < 0)
            {
                //超过24小时,显示日期
                return dateTime.ToString("yyyy年MM月dd日");
            }
            if (passBy_H == 0)
            {
                return dateTime.ToString("刚刚");
            }
            else
            {
                return $"{passBy_H}小时前";
            }
        }
    }
}