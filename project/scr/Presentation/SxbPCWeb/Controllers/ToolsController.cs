using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;



namespace Sxb.PCWeb.Controllers
{
    using iSchool.Internal.API.OperationModule;
    using PMS.Infrastructure.Application.IService;
    using PMS.OperationPlateform.Application.IServices;
    using Sxb.PCWeb.Common;
    using System.ComponentModel;
    using System.Text.RegularExpressions;

    public class ToolsController : BaseController
    {
        OperationApiServices operationApiService;
        IToolTypesServices toolTypesServices;
        IToolsService toolsService;
        protected ICityInfoService _cityCodeService;
        ILocalV2Services localV2Services;

        public ToolsController(OperationApiServices operationApiService,
            IToolTypesServices toolTypesServices, 
            IToolsService toolsService,
            ICityInfoService cityCodeService,
            ILocalV2Services localV2Services)
        {
            this.operationApiService = operationApiService;
            this.toolTypesServices = toolTypesServices;
            this.toolsService = toolsService;
            this._cityCodeService = cityCodeService;
            this.localV2Services = localV2Services;

        }
        [Description("工具页")]
        public async Task<IActionResult> Index(int city)
        {
            int locationCityCode = city == 0 ? Request.GetLocalCity() : city;
            var local = this.localV2Services.GetById(locationCityCode);
            var result =    this.toolTypesServices.GetOnlineToolTypes(local.id,local.parent).ToList();

            ViewBag.CityName = await _cityCodeService.GetCityName(locationCityCode);
            //热门城市
            var hotCity = await _cityCodeService.GetHotCity();
            ViewBag.HotCity = hotCity;

            return View(result);
        }

        [Description("工具页列表接口")]
        public IActionResult GetTools(int toolTypeId)
        {
            int locationCityCode = HttpContext.Request.GetLocalCity();
            var local = this.localV2Services.GetById(locationCityCode);
            var result = this.toolsService.GetByToolTypeId(toolTypeId, local.id,local.parent).ToList();
            if (result.Any())
            {
                return AjaxSuccess(data: result);
            }
            else {

                return AjaxFail("该模块无任何工具。");
            }
               
        }


    


    }
}