using iSchool.Internal.API.UserModule.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace iSchool.Internal.API.UserModule
{
    public class UserApiServices
    {
        readonly HttpClient client;
        readonly ILogger logger;

        public UserApiServices(HttpClient client, ILogger<UserApiServices> logger)
        {
            this.client = client;
            this.logger = logger;
        }


        /// <summary>
        /// 获取用户问卷填写信息
        /// </summary>
        public async Task<ApiInterestInfo> GetUserInterestAsync(Guid? userId,string uuId)
        {
            string url = $"info/getuserinterest?userID={userId}&uuid={uuId}";
            var result = await this.client.GetAsAsync<ApiInterest>(url);
            if (!result.IsOK())
            {
                this.logger.LogInformation("请求info/getuserinterest失败，错误信息：{erro}", result.errorDescription);
            }

            return result.interest;
        }

        public async Task<Guid[]> GetSchoolCollectionID(string iSchoolAuth)
        {
            string url = "/collection/GetSchoolCollectionID";

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get,url);
            requestMessage.Headers.Add("cookie",new[] { $"iSchoolAuth={iSchoolAuth}" });

          var result =   await this.client.SendAsync(requestMessage);
            if (result.IsSuccessStatusCode)
            {

              var ids =await   result.Content.ReadAsAsync<SchoolIDs>();
                if (ids.IsOK())
                {
                    return ids.data;
                }
                else {
                    this.logger.LogError(await result.Content.ReadAsStringAsync());
                    return null;
                }
            }
            else {
                this.logger.LogError("");
                return null;
            }
           
        }

    }
}
