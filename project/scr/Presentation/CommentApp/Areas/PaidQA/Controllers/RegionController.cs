using Microsoft.AspNetCore.Mvc;
using PMS.PaidQA.Application.Services;
using Sxb.Web.Areas.PaidQA.Models.Region;
using Sxb.Web.Controllers;
using Sxb.Web.Response;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.PaidQA.Controllers
{

    [Route("PaidQA/[controller]/[action]")]
    public class RegionController : ApiBaseController
    {
        IRegionTypeService _regionTypeService;

        public RegionController(IRegionTypeService regionTypeService)
        {
            _regionTypeService = regionTypeService;
        }

        public async Task<ResponseResult> List()
        {
            var result = ResponseResult.Success();
            var finds = await _regionTypeService.GetAllRegionTypes();
            if (finds == null || !finds.Any()) return ResponseResult.Failed();

            result.Data = finds.Select(p => new ListResponse()
            {
                ID = p.ID,
                Sort = p.Sort ?? 1,
                Name = p.Name,
                SubItems = p.SubItems?.Select(o => new ResponseItem()
                {
                    ID = o.ID,
                    Name = o.Name,
                    Sort = o.Sort ?? 1
                })
            });

            return result;
        }
    }
}
