using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using PMS.Infrastructure.Application.IService;
using PMS.Search.Domain.Entities;
using PMS.Search.Domain.IRepositories;
using PMS.Search.Elasticsearch;
using ProductManagement.API.Http.Interface;
using ProductManagement.Framework.Foundation;
using Sxb.Web.Controllers;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ProductManagement.API.Http.Service.StaticInsideClient;
using static ProductManagement.Framework.Foundation.BatchHelper;

namespace Sxb.Web.Areas.Common.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyncEsDataController : ApiBaseController
    {
        private readonly IDataImport _dataImport;
        private readonly IStaticInsideClient _staticInsideClient;
        private readonly ICityInfoService _cityInfoService;

        public SyncEsDataController(IDataImport dataImport, IStaticInsideClient staticInsideClient, ICityInfoService cityInfoService)
        {
            _dataImport = dataImport;
            _staticInsideClient = staticInsideClient;
            _cityInfoService = cityInfoService;
        }

        [HttpGet("Sync")]
        public async Task<ResponseResult> SyncAsync([FromQuery] List<StaticInsideType> staticInsideTypes)
        {
            //null is all
            if (!(staticInsideTypes?.Any() == true))
            {
                var type = typeof(StaticInsideType);
                staticInsideTypes = Enum.GetValues(type).Cast<StaticInsideType>().ToList();
            }

            var cities = await _cityInfoService.GetAllCityCodes();
            foreach (var staticInsideType in staticInsideTypes)
            {
                var esIndexName = staticInsideType.ToString().ToLower();
                //清空所有uv
                await _dataImport.ResetUVAsync(esIndexName);
                foreach (var city in cities)
                {
                    var dataUVs = await _staticInsideClient.GetDatUv(staticInsideType, city.Id, 7);
                    if (!dataUVs.Any())
                    {
                        continue;
                    }

                    var entities = dataUVs.Select(s => new SearchUV()
                    {
                        Id = s.DataId,
                        Uv = s.Uv
                    });
                    await new Batch<SearchUV>(entities).RunAsync(async partItems =>
                    {
                        await _dataImport.UpdateUVAsync(esIndexName, partItems);

                        //避免es操作频繁
                        await Task.Delay(200);
                    });
                }
            }
            return ResponseResult.Success();
        }

        [HttpGet("Test")]
        private async Task<ResponseResult> Test(List<StaticInsideType> staticInsideTypes, int cityId = 440100)
        {
            if (staticInsideTypes == null || !staticInsideTypes.Any())
            {
                return ResponseResult.Failed();
            }

            foreach (var staticInsideType in staticInsideTypes)
            {
                var esIndexName = staticInsideType.ToString().ToLower();
                var dataUVs = await _staticInsideClient.GetDatUv(staticInsideType, cityId, 7);
                await _dataImport.UpdateUVAsync(esIndexName, dataUVs.Select(s => new SearchUV()
                {
                    Id = s.DataId,
                    Uv = s.Uv
                }));
            }
            return ResponseResult.Success();
        }
    }
}
