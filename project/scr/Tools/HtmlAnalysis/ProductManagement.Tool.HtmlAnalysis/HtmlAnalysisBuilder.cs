using Microsoft.Extensions.Logging;
using ProductManagement.Tool.HtmlAnalysis.Helper;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProductManagement.Tool.HtmlAnalysis
{
    public class HtmlAnalysisBuilder : IHtmlAnalysisBuilder
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public HtmlAnalysisBuilder(ILogger logger, HttpClient httpClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClient;
        }

        public async Task<HttpContent> GetHttpContent(string url)
        {
            _httpClient.DefaultRequestHeaders.UserAgent.Clear();
            _httpClient.DefaultRequestHeaders.Add("ContentType", "text/html;charset=utf-8");
            _httpClient.DefaultRequestHeaders.Add("AcceptEncoding", "gzip, deflate, b");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Mobile Safari/537.36");

            var httpResponse = await _httpClient.GetAsync(url);

            return httpResponse.Content;
        }

        public async Task<string> GetHtml(string url)
        {
            try
            {
                var httpContent = await GetHttpContent(url);
                return await GetHtml(httpContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetHtml]请求错误");
            }
            return "";
        }

        public async Task<string> GetHtml(HttpContent httpContent)
        {
            try
            {
                string html = string.Empty;
                using (var stream = await httpContent.ReadAsStreamAsync())
                {
                    var charset = httpContent.Headers.ContentType.CharSet;
                    html = HttpEncodingHelper.GetEncodingHtml(stream);
                }
                return html;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetHtml]请求错误");
            }
            return "";
        }

        public async Task<HtmlAnalysisContext> GetHtmlAnalysisContext(string url, AnalysisType analysisType = AnalysisType.All)
        {
            var httpContent = await GetHttpContent(url);
            var html = await GetHtml(httpContent);
            return await GetHtmlAnalysisContext(html, url, analysisType);
        }

        public async Task<HtmlAnalysisContext> GetHtmlAnalysisContext(string html, string pageUrl, AnalysisType analysisType = AnalysisType.All)
        {
            var context = new HtmlAnalysisContext();
            try
            {
                context.LoadHtml(html, pageUrl);

                if (analysisType.HasFlag(AnalysisType.Title))
                {
                    await Task.Run(()=> { 
                        context.AnalysisTitle();
                    });
                }
                if (analysisType.HasFlag(AnalysisType.Images))
                {
                    await Task.Run(() => {
                        context.AnalysisImages();
                    });
                }
                if (analysisType.HasFlag(AnalysisType.TitleImage))
                {
                    await Task.Run(() => {
                        context.AnalysisTitleImage();
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetHtmlAnalysisContext]请求错误");
            }
            return context;
        }


    }
}
