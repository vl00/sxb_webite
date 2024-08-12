using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMS.Infrastructure.Application.IService;
using PMS.Infrastructure.Domain.Entities;
using Sxb.Web.Areas.Common.Models.SystemNotify;
using Sxb.Web.Controllers;
using Sxb.Web.Filters;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Common.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemNotifyController : ApiBaseController
    {
        ISystemNotifyService _systemNotifyService;

        public SystemNotifyController(ISystemNotifyService systemNotifyService)
        {
            _systemNotifyService = systemNotifyService;
        }


        [HttpGet]
        public async Task<ResponseResult> Get([FromQuery] int[] infoTypes)
        {
            if (UserIdOrDefault == default(Guid))
            {
                return ResponseResult.Success("No Data");
            }

            var notifies = await _systemNotifyService.GetAsync(infoTypes.ToList(), UserIdOrDefault);
            foreach (var notify in notifies)
            {
                if (notify.IsAutoAsk == true)
                {
                    await _systemNotifyService.ConfirmAsync(notify.Id);
                }
            }
            var msgs = notifies.Select(n =>
             {
                 return new
                 {
                     id = n.Id,
                     infoType = n.InfoType,
                     time = n.CreateTime,
                     @from = n.FromUser,
                     to = n.ToUser,
                     body = Newtonsoft.Json.JsonConvert.DeserializeObject(n.Body)

                 };
             });
            return ResponseResult.Success(new
            {
                msgs = msgs
            });
        }


        [HttpPost("Publish")]
        [ValidateSxbInnerToken]
        public async Task<ResponseResult> Publish([FromBody] PublishNotifyRequest request)
        {
            SystemNotify systemNotify = new SystemNotify()
            {
                Id = Guid.NewGuid(),
                InfoType = request.InfoType,
                MediaType = request.MediaType,
                CreateTime = DateTime.Now,
                ReadTime = null,
                Body = Newtonsoft.Json.JsonConvert.SerializeObject(request.Body),
                ToUser = request.ToUser,
                FromUser = request.FromUser,
                IsAutoAsk = request.IsAutoAsk,
            };

            bool flag = await _systemNotifyService.AddAsync(systemNotify);
            if (flag)
            {
                return ResponseResult.Success();
            }
            else {
                return ResponseResult.Failed("fail");
            }

        }

    }
}
