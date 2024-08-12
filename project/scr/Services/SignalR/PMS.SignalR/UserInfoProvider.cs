using Microsoft.AspNetCore.SignalR;
using PMS.AspNetCoreHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.SignalR
{
    public class UserInfoProvider: IUserIdProvider
    {
        public virtual string GetUserId(HubConnectionContext connection)
        {
            Guid? userId = connection.User?.Identity?.GetUserInfo()?.UserId;
            return userId.GetValueOrDefault().ToString();
        }
    }
}
