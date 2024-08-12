using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProductManagement.API.Http.Model.StaticInside;
using ProductManagement.API.Http.Result;
using ProductManagement.API.Http.Service;

namespace ProductManagement.API.Http.Interface
{
    public interface IStaticInsideClient
    {
        Task<IEnumerable<StaticDataUV>> GetDatUv(StaticInsideClient.StaticInsideType type, int cityId, int days = 7);
    }
}
