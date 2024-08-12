using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ProductManagement.Tool.HttpRequest.Encrypt;
using ProductManagement.Tool.HttpRequest.Option;
using Newtonsoft.Json;
using System.IO;

namespace ProductManagement.Tool.HttpRequest
{
    public abstract class HttpBaseClient<TConfig> where TConfig : RequestConfig
    {
        protected readonly HttpClient _client;

        protected HttpBaseClient(HttpClient client,TConfig config, ILoggerFactory log) :
        this(client,config, new DefaultEncryptFactory(), log)
        {
        }
         
        protected HttpBaseClient(HttpClient client,TConfig config, IEncryptFactory factory, ILoggerFactory log)
        {
            Config = config ?? throw new ArgumentNullException();
            Factory = factory ?? throw new ArgumentNullException();
            Log = log.CreateLogger(GetType().FullName);
            _client = client;

            var timeout = config.TimeOut <= 0 ? 10 : config.TimeOut;
            _client.Timeout =  TimeSpan.FromSeconds(timeout);
        }

        protected readonly ILogger Log;

        public TConfig Config { get; }

        public IEncryptFactory Factory { get; }

        protected async Task<TResult> GetAsync<TResult, TOption>(TOption option)
            where TOption : BaseOption
            where TResult : new()
        {
            var postContent = HttpContent(option);
            return await RequestAsync<TResult, TOption>(option, (client, url) => client.GetAsync(url));
        }

        protected async Task<TResult> GetAsync<TResult>(string surl, object param)
            where TResult : new()
        {
            var option = new UrlBaseOption(surl, param);
            return await RequestAsync<TResult, UrlBaseOption>(option, (client, url) => client.GetAsync(url));
        }

        protected async Task<string> GetAsync<TOption>(TOption option)
            where TOption : BaseOption
        {
            var postContent = HttpContent(option);
            return await RequestAsync<TOption>(option, (client, url) => client.GetAsync(url));
        }

        protected async Task<HttpContent> GetHttpContentAsync<TOption>(TOption option)
            where TOption : BaseOption
        {
            var postContent = HttpContent(option);
            return await RequestHttpContentAsync<TOption>(option, (client, url) => client.GetAsync(url));
        }


        protected async Task<TResult> PostAsync<TResult, TOption>(TOption option)
            where TOption : BaseOption
            where TResult : new()
        {
            var postContent = HttpContent(option);

            return await RequestAsync<TResult, TOption>(option, (client, url) => client.PostAsync(url, postContent));
        }

        protected async Task<TResult> PostAsync1<TResult, TOption>(TOption option)
            where TOption : BaseOption
            where TResult : new()
        {
            var postContent = HttpContent(option);

            return await RequestAsync<TResult, TOption>(option, (client, url) => client.PostAsync(url, postContent));
        }

        protected async Task<TResult> RequestAsync<TResult, TOption>(
            TOption option,
            Func<HttpClient, string, Task<HttpResponseMessage>> func)
            where TOption : BaseOption
            where TResult : new()
        {
            var url = Config.ServerUrl + option.UrlPath;
            var requestId = Guid.NewGuid();
            try
            {

                Log.LogDebug("[{0}]请求开始：{1} {2} {3}", requestId, url, Environment.NewLine, option);

                var message = await func(_client, url);
                
                if (message.StatusCode != HttpStatusCode.OK)
                {
                    Log.LogWarning(0, "[{0}]请求错误:{1}_{2}", requestId, url, message.StatusCode);

                    throw new HttpCodeException("请求结果不正确" + message.StatusCode);
                }

                var responseContent = await message.Content.ReadAsStringAsync();

                Log.LogDebug("[{0}]请求结束：{1} {2} {3}", requestId, url, Environment.NewLine, responseContent);
                

                return JsonConvert.DeserializeObject<TResult>(responseContent);
            }
            catch (Exception ex)
            {
                Log.LogError(new EventId(0), ex, "[{0}]请求错误:{1}", requestId, url);
                throw;
            }
        }

        protected async Task<string> RequestAsync<TOption>(
            TOption option,
            Func<HttpClient, string, Task<HttpResponseMessage>> func)
            where TOption : BaseOption
        {
            var url = Config.ServerUrl + option.UrlPath;
            var requestId = Guid.NewGuid();
            try
            {

                Log.LogDebug("[{0}]请求开始：{1} {2} {3}", requestId, url, Environment.NewLine, option);

                var message = await func(_client, url);

                if (message.StatusCode != HttpStatusCode.OK)
                {
                    Log.LogWarning(0, "[{0}]请求错误:{1}_{2}", requestId, url, message.StatusCode);

                    throw new HttpCodeException("请求结果不正确" + message.StatusCode);
                }

                var responseContent = await message.Content.ReadAsStringAsync();

                Log.LogDebug("[{0}]请求结束：{1} {2} {3}", requestId, url, Environment.NewLine, responseContent);

                return responseContent;
            }
            catch (Exception ex)
            {
                Log.LogError(new EventId(0), ex, "[{0}]请求错误:{1}", requestId, url);
                throw;
            }
        }

        protected async Task<HttpContent> RequestHttpContentAsync<TOption>(
            TOption option,
            Func<HttpClient, string, Task<HttpResponseMessage>> func)
            where TOption : BaseOption
        {
            var url = Config.ServerUrl + option.UrlPath;
            var requestId = Guid.NewGuid();
            try
            {

                Log.LogDebug("[{0}]请求开始：{1} {2} {3}", requestId, url, Environment.NewLine, option);

                var message = await func(_client, url);

                //if (message.StatusCode != HttpStatusCode.OK)
                //{
                //    Log.LogWarning(0, "[{0}]请求错误:{1}_{2}", requestId, url, message.StatusCode);

                //    throw new HttpCodeException("请求结果不正确" + message.StatusCode);
                //}

                var responseContent = message.Content;

                Log.LogDebug("[{0}]请求结束：{1} {2} {3}", requestId, url, Environment.NewLine, responseContent);

                return responseContent;
            }
            catch (Exception ex)
            {
                Log.LogError(new EventId(0), ex, "[{0}]请求错误:{1}", requestId, url);
                throw;
            }
        }

        private HttpContent HttpContent<TOption>(TOption option) where TOption : BaseOption
        {
            var keyValuePairs = new List<KeyValuePair<string, object>>();

            foreach (var keyValue in option.KeyValuePairs)
            {
                if (keyValue.Value.IsEncrypt)
                {
                    var encrypt = Factory.Instantiation(option.Encrypt);
                    var encryptData = encrypt.Encrypt(keyValue.Value.ToString());
                    keyValuePairs.Add(new KeyValuePair<string, object>(keyValue.Key, encryptData));
                }
                else
                {
                    keyValuePairs.Add(new KeyValuePair<string, object>(keyValue.Key, keyValue.Value.Value));
                }
            }

            var postContent = PostContent(option, keyValuePairs);

            //CookieContainer cookieContainer = new CookieContainer();
            foreach (var header in option.Headers)
            {
                if (header.Key?.ToLower() == "contenttype")
                {
                    postContent.Headers.ContentType = MediaTypeHeaderValue.Parse(header.Value);
                }
                //else if (header.Key.ToLower() == "cookie")
                //{
                //    string[] para = header.Value.ToString().Split('=');
                //    cookieContainer.Add(new Cookie(para[0], para[1]));
                //}
                else if (header.Key?.ToLower() == "user-agent")
                {
                    _client.DefaultRequestHeaders.UserAgent.Clear();
                    _client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
                else
                {
                    postContent.Headers.Add(header.Key, header.Value);
                    _client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            //if(cookieContainer.Count > 0)
            //{
            //    HttpClientHandler httpClientHandler = new HttpClientHandler()
            //    {
            //        CookieContainer = cookieContainer,
            //        AllowAutoRedirect = true,
            //        UseCookies = true
            //    };
            //    postContent
            //}

            return postContent;
        }

        private static HttpContent PostContent<TOption>(TOption option,
            IEnumerable<KeyValuePair<string, object>> keyValuePairs)
            where TOption : BaseOption
        {
            if (option.IsJsonContent())
            {
                string content;
                if (keyValuePairs.Any())
                {
                    var keyValueDic = keyValuePairs.ToDictionary(kv => kv.Key, kv => kv.Value);
                    content = JsonConvert.SerializeObject(keyValueDic);
                }
                else
                {
                    content = JsonConvert.SerializeObject(option.PostBody);
                }
                return new StringContent(content);
            }

            var data = keyValuePairs
                .Select(kv => $"{kv.Key}={kv.Value.ToString()}");

            return new StringContent(string.Join("&", data), Encoding.UTF8, "application/x-www-form-urlencoded");

        }
    }
}
