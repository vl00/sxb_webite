using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductManagement.API.Http.Common;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model;
using ProductManagement.API.Http.Option;
using ProductManagement.API.Http.Result;
using ProductManagement.Tool.HttpRequest;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProductManagement.API.Http.Service
{
    public class HtmlServiceClient : HttpBaseClient<HttpConfig>, IHtmlServiceClient
    {

        public HtmlServiceClient(HttpClient client, IOptions<HttpConfig> config, ILoggerFactory log)
                    : base(client, config.Value, log)
        {
            config.Value.ServerUrl = "";

            //添加更多编码
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        public async Task<string> GetTitle(string url)
        {
            try
            {
                string html = string.Empty;

                var option = new HttpOption(url);
                var resp = await GetHttpContentAsync(option);

                using (var stream = await resp.ReadAsStreamAsync())
                {
                    var charset = resp.Headers.ContentType.CharSet;
                    html = HttpEncodingHelper.GetEncodingHtml(stream);
                }

                Match m = Regex.Match(html, "<title(?:[^>]*>)([^<>/]*)</title>");
                if (m.Groups.Count == 2 && !string.IsNullOrWhiteSpace(m.Groups[1].Value))
                {
                    return m.Groups[1].Value;
                }

                //<meta property="og:title" content="几百万！买下了学位房，却迎来了学位慌！" />
                //<meta property="twitter:title" content="几百万！买下了学位房，却迎来了学位慌！" />
                var metaTitlePattern = "<meta(?:[^>]*)property=\"(?:og|twitter\\s*):title\"(?:.*)content=\"(.*)\"(?:[^>]*)(?:/?)>";
                m = Regex.Match(html, metaTitlePattern);
                if (m.Groups.Count >= 2)
                {
                    return m.Groups[1].Value;
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "[GetTitle]请求错误:{0}", url);
            }
            return "";
        }

        public async Task<Image> GetImage(string url)
        {
            var option = new HttpOption(url);
            var resp = await GetHttpContentAsync(option);
            //var contentType = resp.Content.Headers.ContentType.MediaType;
            var stream = await resp.ReadAsStreamAsync();
            return Image.FromStream(stream); ;
        }
    }
}
