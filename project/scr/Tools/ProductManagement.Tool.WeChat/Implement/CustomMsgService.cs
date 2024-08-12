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
    public class CustomMsgService : ICustomMsgService
    {
        HttpClient _client;
        ILogger _logger;
        public CustomMsgService(HttpClient client, ILogger<CustomMsgService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<SendCustomMsgResponse> Send(string token, CustomMsg msg)
        {
            SendCustomMsgResponse sendCustomMsgResponse = null;
            try
            {
                string url = $@"https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token={token}";
                string data = msg.ToJson();
                HttpContent content = new StringContent(data);
                sendCustomMsgResponse = await _client.PostAsync<SendCustomMsgResponse>(url, content, null);
                if (!sendCustomMsgResponse.Success)
                {
                    throw new Exception(sendCustomMsgResponse.errmsg);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "发送客服消息发生异常。");
            }
            return sendCustomMsgResponse;
        }


    }
}
