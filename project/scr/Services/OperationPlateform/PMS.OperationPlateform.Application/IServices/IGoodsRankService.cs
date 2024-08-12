using PMS.OperationPlateform.Domain.Entitys;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.OperationPlateform.Application.IServices
{
    public interface IGoodsRankService:IApplicationService<GoodsRank>
    {
        IEnumerable<GoodsRank> GetGoodsByPage(int pageindex, int pagesize);

    }
}
