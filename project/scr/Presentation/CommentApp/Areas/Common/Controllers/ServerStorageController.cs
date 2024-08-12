using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PMS.Infrastructure.Application.IService;
using PMS.Infrastructure.Domain.Entities;
using PMS.Infrastructure.Domain.Enums;
using Sxb.Web.Areas.Common.Models.ServerStorage;
using Sxb.Web.Common;
using Sxb.Web.Controllers;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Common.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServerStorageController : ApiBaseController
    {
        IServerStorageService _serverStorageService;
        public ServerStorageController(IServerStorageService serverStorageService)
        {
            _serverStorageService = serverStorageService;
        }

        [HttpGet]
        public async Task<ResponseResult> Get(string key)
        {

           ServerStorage serverStorage =  await _serverStorageService.GetAsync(Request.GetGlobalIdentity().ToString(), key);
            if (serverStorage != null)
            {
                object value = null;
                switch (serverStorage.DataType)
                {
                    case ServerStorageDataType.Json:
                        value = Newtonsoft.Json.JsonConvert.DeserializeObject(serverStorage.Value);
                        break;
                    case ServerStorageDataType.String:
                        value = serverStorage.Value;
                        break;
                    case ServerStorageDataType.Int64:
                        value = Convert.ToInt64(serverStorage.Value);
                        break;
                    case ServerStorageDataType.Double:
                        value = Convert.ToDouble(serverStorage.Value);
                        break;
                }
                return ResponseResult.Success(value,"OK");
            }
            else {
                return ResponseResult.Success(null,"empty");
            }
        }


        [HttpPost]
        public async Task<ResponseResult> Set([FromBody] SetRequest request)
        {

            if (request.Value != null)
            {
                ServerStorage serverStorage = new ServerStorage()
                {
                    Id = Guid.NewGuid(),
                    HashKey = Request.GetGlobalIdentity().ToString(),
                    CreateTime = DateTime.Now,
                    Key = request.Key,
                    ExpireAt = DateTime.Now.AddHours(request.ExpireHours)
                };
                Type type = request.Value.GetType();
                switch (type.FullName)
                {
                    case "System.String":
                        serverStorage.DataType = ServerStorageDataType.String;
                        serverStorage.Value = request.Value.ToString();
                        break;
                    case "System.Int64":
                        serverStorage.DataType = ServerStorageDataType.Int64;
                        serverStorage.Value = request.Value.ToString();
                        break;
                    case "System.Double":
                        serverStorage.DataType = ServerStorageDataType.Double;
                        serverStorage.Value = request.Value.ToString();
                        break;
                    case "Newtonsoft.Json.Linq.JObject":
                    case "Newtonsoft.Json.Linq.JArray":
                        serverStorage.DataType = ServerStorageDataType.Json;
                        serverStorage.Value = request.Value.ToString();
                        break;
                    default:
                        return ResponseResult.Success("暂不支持该数据类型");
                }
                bool flag = await _serverStorageService.SetAsync(serverStorage);
                if (flag)
                {

                    return ResponseResult.Success("ok.");
                }
                else
                {
                    return ResponseResult.Failed("erro:the value unset in the storage.");
                }
            }
            else {
                return ResponseResult.Failed("erro:the value can not be null.");
            }

        }



    }
}
