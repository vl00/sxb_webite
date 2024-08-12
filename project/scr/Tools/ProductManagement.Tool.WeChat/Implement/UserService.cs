using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WeChat.Interface;
using WeChat.Model;

namespace WeChat.Implement
{
    public class UserService : IUserService
    {
        HttpClient _client;
        ILogger _logger;
        public UserService(HttpClient client, ILogger<MediaService> logger)
        {
            _client = client;
            _logger = logger;
        }
        public async Task<GetUserInfoResponse> GetUserInfo(string accessToken, string openId)
        {
            string url = $"https://api.weixin.qq.com/cgi-bin/user/info?access_token={accessToken}&openid={openId}&lang=zh_CN";
            return await _client.GetAsync<GetUserInfoResponse>(url, _logger);
        }
    }
}
