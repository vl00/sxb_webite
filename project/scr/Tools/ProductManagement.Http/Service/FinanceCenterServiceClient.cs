using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ProductManagement.API.Http.Common;  
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model.FinanceCenter;
using ProductManagement.API.Http.Option.FinanceCenter;
using ProductManagement.API.Http.Result.FinanceCenter;
using ProductManagement.Tool.HttpRequest;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.API.Http.Service
{
    public class FinanceCenterServiceClient : HttpBaseClient<FinanceCenterConfig>, IFinanceCenterServiceClient
    {
      
        public FinanceCenterServiceClient(HttpClient client, IOptions<FinanceCenterConfig> config, ILoggerFactory log) : base(client, config.Value, log)
        {
            client.BaseAddress = new Uri(config.Value.ServerUrl);
            client.Timeout = TimeSpan.FromSeconds(60);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<PayCenterResponseResult<WechatJsPayParam>> AddPayOrder(AddPayOrderRequest request)
        {
            var result = new PayCenterResponseResult<WechatJsPayParam>();
            var option = new AddPayOrderOption();
           
            var r= await this.FinancialPostAsync<WechatJsPayParam>(option.UrlPath, JsonConvert.SerializeObject(request));
            return r;
        }

        public async Task<PayCenterResponseResult<RefundResult>> Refund(RefundRequest request)
        {
            var result = new PayCenterResponseResult<RefundResult>();
            var option = new RefundOption();
            var r = await this.FinancialPostAsync<RefundResult>(option.UrlPath, JsonConvert.SerializeObject(request));
            return r;
        }

        public async Task<PayCenterResponseResult<WalletResult>> Wallet(WalletRequest request)
        {
            var result = new PayCenterResponseResult<WalletResult>();
            var option = new WalletOptions();

            var r = await this.FinancialPostAsync<WalletResult>(option.UrlPath, JsonConvert.SerializeObject(request));
            return r;
        }
        public async Task<PayCenterResponseResult<CheckPayStatusResult>> CheckPayStatus(CheckPayStatusRequest request)
        {
            var result = new PayCenterResponseResult<CheckPayStatusResult>();
            string rawUrl = "/api/PayOrder/CheckPayStatus";
            var r = await this.FinancialPostAsync<CheckPayStatusResult>(rawUrl, JsonConvert.SerializeObject(request));
            return r;
        }


        /// <summary>
        /// 请求支付中心接口统一调用的httpclient，里面会代签名
        /// </summary>
        /// <param name="client"></param>
        /// <param name="requestUri"></param>
        /// <param name="requestJson"></param>
        /// <returns></returns>
        public  async Task<PayCenterResponseResult<TResult>> FinancialPostAsync<TResult>( string requestUri, string requestJson)
        {
            base.Log.LogInformation($"请求：{requestUri},请求体:{requestJson}");
            string body = string.Empty;
            try
            {
                using (var reqContent = new StringContent(requestJson, Encoding.UTF8, "application/json"))
                {
                    var timespan = DateTime.Now.ToString("yyyyMMddHHmmss");
                    var nonce = Guid.NewGuid().ToString("N");
                    reqContent.Headers.Add("sxb.timespan", timespan);
                    reqContent.Headers.Add("sxb.nonce", nonce);
                    reqContent.Headers.Add("sxb.key", "AskSystem");
                    StringBuilder query = new StringBuilder("woaisxb2021");//私钥，不能泄露
                    query.Append($"{timespan}\n{nonce}\n{requestJson}\n");
                    MD5 md5 = MD5.Create();
                    byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(query.ToString()));
                    StringBuilder result = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        result.Append(bytes[i].ToString("X2"));
                    }
                    var sign = result.ToString();
                    reqContent.Headers.Add("sxb.sign", sign);
                    using (var resp = await _client.PostAsync(requestUri, reqContent))
                    using (var respContent = resp.Content)
                    {

                        body = await respContent.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<PayCenterResponseResult<TResult>>(body);


                    }
                }



            }
            catch (Exception ex)
            {
                base.Log.LogError(ex, $"请求支付中心失败。{body}");
                return new PayCenterResponseResult<TResult>() { succeed = false, msg = $"请求支付中心失败,{ex.Message}" };
            }

        }



    }
}
