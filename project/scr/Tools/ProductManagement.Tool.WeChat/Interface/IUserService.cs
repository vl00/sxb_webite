using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WeChat.Model;

namespace WeChat.Interface
{
    public interface IUserService
    {
        Task<GetUserInfoResponse> GetUserInfo(string accessToken, string openId);
    }
}
