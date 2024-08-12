using System;
using System.Collections.Generic;
using PMS.UserManage.Domain.Entities;

namespace PMS.UserManage.Application.IServices
{
    public interface IOAuthQQService
    {
        void QQLogin(string unionid, string openid, string appName, ref UserInfo userInfo);
        bool QQUnionIDLogin(Guid unionID, Guid openID, string appName, ref UserInfo userInfo);
        bool QQOpenIDLogin(Guid openID, string appName, ref UserInfo userInfo);
        bool BindQQ(Guid userID, Guid unionID, Guid openID, bool confirm, string appName = "web");
        bool UnboundQQ(Guid userID);
        List<UserInfo> ListQQConflict(string unionID, string openID);
    }
}
