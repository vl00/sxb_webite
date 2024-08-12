using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.IRespositories;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.OperationPlateform.Application.Services
{
    public class GoodsRankService : IGoodsRankService
    {
        IGoodsRankRepository _goodsRankRepository;
        public GoodsRankService(IGoodsRankRepository goodsRankRepository)
        {
            _goodsRankRepository = goodsRankRepository;
        }

        public IEnumerable<GoodsRank> GetGoodsByPage(int pageindex, int pagesize)
        {

            return _goodsRankRepository.GetGoodsByPage(pageindex, pagesize);
        }


    }
}
