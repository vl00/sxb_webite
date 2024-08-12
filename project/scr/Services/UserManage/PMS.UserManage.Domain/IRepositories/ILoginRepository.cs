using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.UserManage.Domain.IRepositories
{
    public interface ILoginRepository
    {
        bool QQUnionIDLogin(Guid unionID, Guid openID, string appName, ref UserInfo userInfo);
        bool QQOpenIDLogin(Guid openID, string appName, ref UserInfo userInfo);
    }
}
