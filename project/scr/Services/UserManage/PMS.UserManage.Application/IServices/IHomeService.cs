using PMS.UserManage.Application.ModelDto.Home;
using System;
using System.Collections.Generic;
using System.Text;
using Index = PMS.UserManage.Application.ModelDto.Home.Index;

namespace PMS.UserManage.Application.IServices
{
    public interface IHomeService
    {
        Index GetUserCenterHomeInfo(Guid userID, string cookieStr, string uuid);
    }
}
