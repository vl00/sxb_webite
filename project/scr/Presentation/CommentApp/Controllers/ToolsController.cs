using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Sxb.Web.Controllers
{
    using iSchool.Internal.API.OperationModule;
    using Microsoft.AspNetCore.Http;
    using NSwag.Annotations;
    using PMS.OperationPlateform.Application.IServices;
    using PMS.OperationPlateform.Domain.Entitys;
    using Sxb.Web.Common;
    using Sxb.Web.Utils;
    using System.ComponentModel;
    using System.Text.RegularExpressions;
    using System.Web;

    [OpenApiIgnore]
    public class ToolsController : Controller
    {
        private OperationApiServices operationApiService;

        private IToolTypesServices toolTypesServices;
        private IToolsService toolsService;
        private ILocalV2Services localV2Services;

        public ToolsController(OperationApiServices operationApiService, IToolTypesServices toolTypesServices, IToolsService toolsService, ILocalV2Services localV2Services)
        {
            this.operationApiService = operationApiService;
            this.toolTypesServices = toolTypesServices;
            this.toolsService = toolsService;
            this.localV2Services = localV2Services;
        }

        [Description("工具页")]
        public IActionResult Index()
        {
            int locationCityCode = HttpContext.Request.GetLocalCity();
            var local = this.localV2Services.GetById(locationCityCode);
            var result = this.toolTypesServices.GetOnlineToolTypes(local.id, local.parent).ToList();
            return View(result);
        }

        [Description("工具页列表")]
        public IActionResult ToolList(int toolTypeId, string toolTypeName)
        {
            int locationCityCode = HttpContext.Request.GetLocalCity();
            var local = this.localV2Services.GetById(locationCityCode);
            var result = this.toolsService.GetByToolTypeId(toolTypeId, local.id, local.parent).ToList();
            if (result.Any())
            {
                if (result.Count == 1)
                {
                    return Redirect(result.First().LinkUrl);
                }
                else
                {
                    result.AdaptAppUrlHandle(this.Request);
                    ViewBag.ToolTypeName = toolTypeName;

                    return View(result);
                }
            }
            else
            {
                return NotFound();
            }
        }
    }

    public static class List_tools_ext
    {
        /// <summary>
        /// 为工具做适配app的URL处理
        /// </summary>
        /// <param name="tools"></param>
        /// <returns></returns>
        public static void AdaptAppUrlHandle(this List<Tools> tools, HttpRequest request)
        {
            var clientType = request.GetClientType();
            if (clientType == UA.Mobile)
            {
                //外部的链接才需要加  ?fullscreen=false 参数
                Regex regex = new Regex("(http|https)://");
                tools.ForEach(t =>
                {
                    if (regex.IsMatch(t.LinkUrl))
                    {
                        //外部链接
                        //Regex matchQueryString = new Regex("(.*)(\\?.*)");
                        //if (matchQueryString.IsMatch(url))
                        //{
                        //    var match = matchQueryString.Match(url);
                        //    var parmas = match.Groups[2].Value;
                        //    var parr= parmas.Remove(0,1).Split('&', StringSplitOptions.RemoveEmptyEntries).ToList();
                        //    parr.Add("fullscreen=false");
                        //    var newstr= string.Join('&', parr);
                        //    return match.Groups[1].Value + "?" + newstr;

                        //}
                        //else {
                        //    return url + "?fullscreen=false";
                        //}

                        t.LinkUrl = $"ischool://web?url={HttpUtility.UrlEncode(t.LinkUrl)}&fullscreen=false";
                    }
                });
            }
        }
    }
}