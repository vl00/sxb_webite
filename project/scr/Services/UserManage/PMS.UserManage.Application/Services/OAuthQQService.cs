using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Domain.IRepositories;
using System;
using System.Collections.Generic;

namespace PMS.UserManage.Application.Services
{
    public class OAuthQQService: IOAuthQQService
    {
        private readonly ILoginRepository _login;
        private readonly INewAccountRepository _accountRepo;
        public OAuthQQService(ILoginRepository login, INewAccountRepository accountRepo)
        {
            _login = login;
            _accountRepo = accountRepo;
        }

        public void QQLogin(string unionid, string openid, string appName, ref UserInfo userInfo)
        {
            _login.QQUnionIDLogin(Guid.Parse(unionid), Guid.Parse(openid), appName, ref userInfo);
        }

        public bool QQOpenIDLogin(Guid openID, string appName, ref UserInfo userInfo)
        {
            return _login.QQOpenIDLogin(openID, appName, ref userInfo);
        }

        public bool QQUnionIDLogin(Guid unionID, Guid openID, string appName, ref UserInfo userInfo)
        {
            return _login.QQUnionIDLogin(unionID, openID, appName, ref userInfo);
        }

        public bool BindQQ(Guid userID, Guid unionID, Guid openID, bool confirm, string appName = "web")
        {
            if (!confirm && !_accountRepo.CheckQQConflict(unionID))
            {
                return false;
            }
            return _accountRepo.BindQQUnionID(unionID, userID) && _accountRepo.BindQQOpenID(openID, userID, appName);
        }

        public List<UserInfo> ListQQConflict(string unionID, string openID)
        {
            return _accountRepo.ListQQConflict(Guid.Parse(unionID), Guid.Parse(openID));
        }
        /// <summary>
        /// 解绑QQ
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public bool UnboundQQ(Guid userID)
            => _accountRepo.UnboundQQ(userID);
    }
}
