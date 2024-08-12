using Microsoft.AspNetCore.Http;
using PMS.OperationPlateform.Application.IServices;
using Microsoft.AspNetCore.Mvc;
using PMS.OperationPlateform.Domain.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductManagement.API.Http.Interface;
using Sxb.Web.Controllers;
using ProductManagement.Framework.AspNetCoreHelper.ResponseModel;

namespace Sxb.Web.Areas.Org.Controller
{
    [Route("api/org/[controller]/[action]")]
    [ApiController]
    public class GoodsRankController : ApiBaseController
    {
        private readonly IGoodsServiceClient _goodsServiceClient;
        IGoodsRankService _goodsRankService;
        public GoodsRankController(IGoodsServiceClient goodsServiceClient, IGoodsRankService goodsRankService)
        {
            _goodsServiceClient = goodsServiceClient;
            _goodsRankService = goodsRankService;
        }

        [HttpGet]
        public async Task<ResponseResult> GetAllGoods(int pageindex = 1,int pagesize = 10)
        {
            var result = ResponseResult.Success();

            
            if (pageindex <= 0)
            {
                result = ResponseResult.Failed("PageIndex应为大于0的正整数");
                return result;
            }
            else if (pagesize <= 0)
            {
                result = ResponseResult.Failed("PageSize应为大于0的正整数");
                return result;
            }

            // 根据传参调用库内数据——IschoolArticle->GoodsRank
            var querydata = _goodsRankService.GetGoodsByPage(pageindex, pagesize);

            List<string> idlists = new List<string> { };

            // 提取短id
            foreach (var data in querydata)
            {
                idlists.Add(data.sid);
            }
            
            // 短id接口调用获取数据详情
            var goodsdata = await _goodsServiceClient.GetCourses(idlists);

            if(goodsdata == null)
            {
                result = ResponseResult.Failed("网络问题或其中存在商品id下架或商品id不存在");
                return result;
            }
            else if (goodsdata.Courses.Count() != 0)
            {
                result.Data = goodsdata;
                return result;
            }
            else
            {
                result = ResponseResult.Success(new List<object>());
                //result.Data = "列表为空";
                return result;
            }
            
        }

    }
}
