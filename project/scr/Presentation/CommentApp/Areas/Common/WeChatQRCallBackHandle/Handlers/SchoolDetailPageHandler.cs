using PMS.Infrastructure.Application.IService;
using PMS.School.Application.IServices;
using ProductManagement.API.Http.Interface;
using Sxb.Web.Areas.Common.Models;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers
{
    public class SchoolDetailPageHandler : ISubscribeCallBackHandler
    {
        IWeChatService _weChatService;
        ISchoolService _schoolService;
        public SchoolDetailPageHandler(IWeChatService weChatService
           , ISchoolService schoolService)
        {
            _weChatService = weChatService;
            _schoolService = schoolService;
        }

        public  async Task<ResponseResult> Process(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest)
        {
           var schoolExtDto =  _schoolService.GetSchoolDetailById(qrequest.DataId.GetValueOrDefault());
            if(schoolExtDto == null)
            {
                return ResponseResult.Failed("处理失败，找不到学校分部信息。");
            }

            await _weChatService.SendNews(frequest.OpenId
               , "点击查看更多内容"
               , $"感谢关注上学帮！点击查看{schoolExtDto.Name}信息"
               , "https://cdn.sxkid.com/images/logo_share_v4.png"
               , qrequest.DataUrl);
            return ResponseResult.Success("OK");
        }
    }
}
