using System;
using System.Collections.Generic;
using System.Text;

namespace iSchool.Operation.Api.Services
{
    using IServices;
    using Microsoft.Extensions.Logging;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Models;

    public class OperationApiService : IOperationApiService
    {


        protected HttpClient client;

        ILogger logger;

        public OperationApiService(
            HttpClient httpClient,
            ILogger<IOperationApiService> logger
            )
        {
            this.client = httpClient;
            this.logger = logger;

        }

        public async Task<List<ToolModule>> GetToolTypesAsync()
        {
            var result = await this.client.GetAsAsync<ResponseModel<List<ToolModule>>>("api/ToolsApi/GetToolTypes");
            if (!result.IsOK())
            {
                this.logger.LogInformation("请求工具模块时服务数据返回不正确。错误信息:{erro}", result.msg);
            }
            return result.data;

        }

        public async Task<List<Tool>> GetByToolTypeIdAsync(int toolTypeId)
        {

            var result = await this.client.GetAsAsync<ResponseModel<List<Tool>>>($"api/ToolsApi/GetByToolTypeId?toolTypeId={toolTypeId}");
            if (!result.IsOK())
            {
                this.logger.LogInformation("请求工具类型下的工具时返回不正确.错误信息:{erro}",result.msg);
            }

            return result.data;
        }

    }
}
