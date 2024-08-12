using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.MessageSend
{
    public class TencentCloudSmsService 
    {
        private static readonly HttpClient _httpClient =
            new HttpClient { BaseAddress = new Uri("https://yun.tim.qq.com/v5/tlssmssvr/sendsms") };
        private readonly string _appId;
        private readonly string _appKey;
        private const string SIGNATURE = "...";
        private const int DOMESTIC_TEMPLATE_ID = 1234;
        private const int OVERSEA_TEMPLATE_ID = 5678;

        public async Task<bool> SendCode(string countryCode, long mobile, int code)
        {
            var random = GetRandom();
            var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            var data = new
            {
                tel = new { nationcode = countryCode.Replace("+", ""), mobile = mobile.ToString() },
                sign = SIGNATURE,
                tpl_id = countryCode == "+86" ? DOMESTIC_TEMPLATE_ID : OVERSEA_TEMPLATE_ID,
                @params = new[] { code.ToString() },
                sig = ComputeSignature(mobile, random, timestamp),
                time = timestamp,
                extend = "",
                ext = ""
            };

            var url = $"/v5/tlssmssvr/sendsms?sdkappid={_appId}&random={random}";
            var response = await _httpClient.PostAsJsonAsync<dynamic>(url, data);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            if (int.Parse(result) != 0)
            {
                return false;
            }

            return true;
        }

        private string ComputeSignature(long mobile, int random, long timestamp)
        {
            var input = $"appkey={_appKey}&random={random}&time={timestamp}&mobile={mobile}";
            var hasBytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(input));
            return string.Join("", hasBytes.Select(b => b.ToString("x2")));
        }

        private int GetRandom()
        {
            return new Random().Next(100000, 999999);
        }
    }

    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient httpClient, string requestUri, T value)
        {
            var stringContent = new StringContent(
                    JsonConvert.SerializeObject(value),
                    Encoding.UTF8,
                    "application/json");
            return await httpClient.PostAsync(requestUri, stringContent);
        }
    }

}
