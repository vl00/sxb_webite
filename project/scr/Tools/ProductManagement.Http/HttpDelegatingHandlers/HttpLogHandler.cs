using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProductManagement.API.Http.HttpDelegatingHandlers
{
    public class HttpLogHandler : DelegatingHandler
    {
        ILogger _logger;
        public HttpLogHandler(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory?.CreateLogger("SxbHttpLogger");
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                string requestBody = await request.Content.ReadAsStringAsync();
                string responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogWarning($"请求返回结果疑似异常。\nUri={ request.RequestUri},\nMethod={request.Method},\nRequestBody={requestBody},\nResponseBody={responseBody}");
            }

            return response;
        }
    }
}
