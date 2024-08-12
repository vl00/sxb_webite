using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PMS.UserManage.Domain.Entities;

namespace PMS.UserManage.Application.IServices
{
    public interface ILoginService
    {
        bool RegisUserInfo(ref UserInfo userInfo);
        UserInfo Login(Guid kid, short nationCode, string mobile, string password, string rndCode, out string failResult, string nickname, int? city, out bool mobileRepeatUser);

        List<UserInfo> GetSameMobileUsers(short nationCode, string mobile);

        UserInfo SelectUserLogin(Guid userid);
        /// <summary>
        /// 检查用户是否绑定了此手机号
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="nationCode"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        bool CheckUserMobile(Guid userid, short nationCode, string mobile);
        bool Login(string appId, string secret);
        string GetSecret(string appId);
        Task<string> GetUserCode(Guid userId);
        Task<UserInfo> LoginByCode(string code);
    }
}
