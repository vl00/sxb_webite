using System;
using System.Threading.Tasks;
using ProductManagement.Tool.Amap.Model;
using ProductManagement.Tool.Amap.Result;

namespace ProductManagement.Tool.Amap
{
    public interface IAmapClient
    {
        //行政区域查询
        //https://lbs.amap.com/api/webservice/guide/api/district
        Task<DistrictResult> GetDistrict(DistrictModel info);
        Task<CurrentLocation> GetCurrentLocation(string ip);
    }
}
