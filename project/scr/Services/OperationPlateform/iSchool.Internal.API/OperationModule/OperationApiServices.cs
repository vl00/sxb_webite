using iSchool.Internal.API.OperationModule.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace iSchool.Internal.API.OperationModule
{
    public class OperationApiServices
    {

        protected HttpClient client;
        readonly ILogger logger;

        public OperationApiServices(
            HttpClient httpClient,
            ILogger<OperationApiServices> logger
            )
        {
            this.client = httpClient;
            this.logger = logger;

        }

        public async Task<List<ToolModule>> GetToolTypesAsync(int cityId)
        {
            var result = await this.client.GetAsAsync<ResponseModel<List<ToolModule>>>($"api/ToolsApi/GetToolTypes?cityId={cityId}");
            if (!result.IsOK())
            {
                this.logger.LogInformation("请求工具模块时服务数据返回不正确。错误信息:{erro}", result.msg);
            }
            return result.data;

        }

        public async Task<List<Tool>> GetByToolTypeIdAsync(int toolTypeId,int cityId)
        {

            var result = await this.client.GetAsAsync<ResponseModel<List<Tool>>>($"api/ToolsApi/GetByToolTypeId?toolTypeId={toolTypeId}&cityId={cityId}");
            if (!result.IsOK())
            {
                this.logger.LogInformation("请求工具类型下的工具时返回不正确.错误信息:{erro}", result.msg);
            }

            return result.data;
        }
    }
}
