using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMS.Infrastructure.Application.IService;
using Sxb.Inside.RequestModel;
using Sxb.Inside.Response;

namespace Sxb.Inside.Controllers
{
    public class TextController
    {
        private readonly ITextService _textService;
        public TextController(ITextService textService)
        {
            _textService = textService;
        }
        /// <summary>
        /// 敏感词检测接口
        /// </summary>
        /// <param name="Keywords"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> GreenTextCheck([FromBody]GreenTextData req)
        {
            var result = await _textService.GreenTextCheckDetail(req.Keywords);

            var pass = true;
            if (result != null && result.Length > 0)
            {
                if (result[0].code == 200)
                {
                    var blockResult = result[0].results.ToList().Where(q => q.suggestion == "block");
                    if (blockResult.Any())
                    {
                        pass = false;
                    }
                    else
                    {
                        pass = true;
                    }
                }
            }
            return ResponseResult.Success(new {  pass, result });
        }
    }
}
