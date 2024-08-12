using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WeChat.Model;

namespace WeChat.Interface
{

    /// <summary>
    /// 微信搜一搜服务
    /// </summary>
    public interface IWeChatSearchService
    {

        Task<string> UpsertPreUniversityBasic(string access_token, UpsertPreUniversityBasicRequest request);
    }
}
