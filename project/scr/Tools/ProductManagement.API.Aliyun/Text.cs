using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductManagement.API.Aliyun.Common;
using ProductManagement.API.Aliyun.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.API.Aliyun
{



    public class Text : IText
    {
        HttpClient client;
        ILogger logger;
        string accessKeyID = "bLWOeFkMmFTOpJDW";

        string accessKeySecret = "srvgyWsLNjxvDXiNOwkevPiFCfEk1e";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="client">请自行提供client对象并且管理其生命周期</param>
        /// <param name="accessKeyID">不传取默认值</param>
        /// <param name="accessKeySecret">不传取默认值</param>
        /// <param name="logger">不传不使用log功能</param>
        public Text(HttpClient client, IOptions<AliTextConfig> config, ILogger<Text> logger)
        {
            this.client = client;
            this.logger = logger;


            var textConfig = config.Value;

            if (!string.IsNullOrEmpty(textConfig.ApiKey))
                this.accessKeyID = textConfig.ApiKey;
            if (!string.IsNullOrEmpty(textConfig.ApiSecret))
                this.accessKeySecret = textConfig.ApiSecret;
            if (this.client.BaseAddress == null)
            {
                if (!string.IsNullOrEmpty(textConfig.BaseUrl))
                    this.client.BaseAddress = new Uri(textConfig.BaseUrl);
                else
                    this.client.BaseAddress = new Uri("https://green.cn-shenzhen.aliyuncs.com");
            }
        }


        public async Task<bool> GarbageCheck(string text)
        {
            var contentCheck = await GarbageCheck(new ProductManagement.API.Aliyun.Model.GarbageCheckRequest()
            {
                scenes = new[] { "antispam" },
                tasks = new List<ProductManagement.API.Aliyun.Model.GarbageCheckRequest.Task>() {
                      new ProductManagement.API.Aliyun.Model.GarbageCheckRequest.Task(){
                       content=text,
                         dataId = Guid.NewGuid().ToString()
                      }
                      }
            });
            return contentCheck.data.FirstOrDefault()?.results.FirstOrDefault()?.suggestion == "block";
        }

        public async Task<CommonResponse<GarbageCheckResponse[]>> GarbageCheck(GarbageCheckRequest request)
        {
            string raw_url = "/green/text/scan";
            string httpMethod = "POST";
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            string contentMd5 = GetBodyMd5HashToBase64(body);
            string date = DateTime.UtcNow.ToString("r");
            string noncstr = GetNonce(32);
            string sign = Signature(accessKeySecret, httpMethod, contentMd5, date, "HMAC-SHA1", noncstr, "1.0", "2018-05-09", raw_url);
            var requestMessage = new HttpRequestMessage(new HttpMethod(httpMethod), raw_url);
            requestMessage.Headers.Add("Accept", "application/json");
            requestMessage.Headers.Add("Date", date);
            requestMessage.Headers.Add("x-acs-version", "2018-05-09");
            requestMessage.Headers.Add("x-acs-signature-nonce", noncstr);
            requestMessage.Headers.Add("x-acs-signature-version", "1.0");
            requestMessage.Headers.Add("x-acs-signature-method", "HMAC-SHA1");
            requestMessage.Headers.Add("Authorization", $"acs {accessKeyID}:{sign}");
            HttpContent request_Content = new StringContent(body);
            request_Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            request_Content.Headers.Add("Content-MD5", contentMd5);
            requestMessage.Content = request_Content;
            var response = await this.client.SendAsync(requestMessage);
            var result = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                if (this.logger != null)
                {
                    this.logger.LogWarning(result);
                }

                return null;
            }
            else
            {
                var responseModel = Newtonsoft.Json.JsonConvert.DeserializeObject<CommonResponse<GarbageCheckResponse[]>>(result);
                return responseModel;
            }

        }


        public async Task<bool> Check(GarbageCheckRequest request)
        {
            var contentCheck = await GarbageCheck(request);
            return contentCheck.data.FirstOrDefault().results.FirstOrDefault().suggestion != "block";

        }


        /// <summary>
        /// content-md5 处理
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static string GetBodyMd5HashToBase64(string body)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(body));
            return Convert.ToBase64String(data);
        }

        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="key"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        static string Signature(string key, string type, string md5, string date, string sMethod, string sNonce, string sVersion, string version, string raw_url)
        {
            // Initialize the keyed hash object.
            using (HMACSHA1 hmac = new HMACSHA1(Encoding.UTF8.GetBytes(key)))
            {
                StringBuilder text = new StringBuilder();
                text.AppendFormat("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}\n{9}",
                    type,
                    "application/json",
                    md5,
                    "application/json",
                    date,
                    $"x-acs-signature-method:{sMethod}",
                    $"x-acs-signature-nonce:{sNonce}",
                    $"x-acs-signature-version:{sVersion}",
                    $"x-acs-version:{version}",
                    raw_url
                    );
                byte[] textByte = Encoding.UTF8.GetBytes(text.ToString());
                byte[] hashValue = hmac.ComputeHash(textByte);
                return Convert.ToBase64String(hashValue);
            }

        } // end SignFile


        /// <summary>
        /// 获取随机字符串
        /// </summary>
        /// <param name="bit"></param>
        /// <returns></returns>
        static string GetNonce(int bit)
        {
            string str = "abcdefghiklmnopqvuABCDEFGHIJKLMNOPVU1234567890";
            Random r = new Random();
            StringBuilder end = new StringBuilder();
            for (int i = 0; i < bit; i++)
            {
                var index = r.Next(0, str.Length);
                end.Append(str[index]);
            }
            return end.ToString();

        }



    }
}
