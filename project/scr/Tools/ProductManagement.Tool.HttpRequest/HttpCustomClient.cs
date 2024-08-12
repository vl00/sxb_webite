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

namespace ProductManagement.Tool.HttpRequest
{
    public abstract class HttpCustomClient
    {
        private readonly HttpClient _client;

        protected HttpCustomClient(HttpClient client,ILoggerFactory log) :
        this(client,new DefaultEncryptFactory(), log)
        {
        }
         
        protected HttpCustomClient(HttpClient client,IEncryptFactory factory, ILoggerFactory log)
        {
            Factory = factory ?? throw new ArgumentNullException();
            Log = log.CreateLogger(GetType().FullName);
            _client = client;
            _client.Timeout = TimeSpan.FromSeconds(3);
        }

        protected readonly ILogger Log;

        public IEncryptFactory Factory { get; }

        protected async Task<TResult> GetAsync<TResult, TOption>(TOption option)
            where TOption : BaseOption
            where TResult : new()
        {
            var postContent = HttpContent(option);
            return await RequestAsync<TResult, TOption>(option, (client, url) => client.GetAsync(url));
        }

        protected async Task<TResult> PostAsync<TResult, TOption>(TOption option)
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
            var url = option.UrlPath;
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

            foreach (var header in option.Headers)
            {
                if (header.Key?.ToLower() == "contenttype")
                {
                    postContent.Headers.ContentType = MediaTypeHeaderValue.Parse(header.Value);
                }
                else
                {
                    //postContent.Headers.Add(header.Key, header.Value);
                    _client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            return postContent;
        }

        private static HttpContent PostContent<TOption>(TOption option,
            IEnumerable<KeyValuePair<string, object>> keyValuePairs)
            where TOption : BaseOption
        {
            if (option.IsJsonContent())
            {
                var keyValueDic = keyValuePairs.ToDictionary(kv => kv.Key, kv => kv.Value);

                var content = JsonConvert.SerializeObject(keyValueDic);

                return new StringContent(content);
            }

            var data = keyValuePairs
                .Select(kv => $"{kv.Key}={kv.Value.ToString()}");

            return new StringContent(string.Join("&", data), Encoding.UTF8, "application/x-www-form-urlencoded");

        }
    }
}
