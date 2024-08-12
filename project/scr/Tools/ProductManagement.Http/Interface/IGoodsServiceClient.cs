using PMS.OperationPlateform.Domain.Entitys;
using ProductManagement.API.Http.Result.Org;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.API.Http.Interface
{
    public interface IGoodsServiceClient
    {
        Task<GoodsObject> GetCourses(IEnumerable<string> ids);
        //Task GetCourses(IEnumerable<GoodsRank> enumerable);
    }
}
