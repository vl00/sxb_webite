using System;
using PMS.UserManage.Application.ModelDto;
using PMS.UserManage.Domain.Entities;

namespace PMS.UserManage.Application.IServices
{
    public interface IOAuthBaiduService
    {
        bool Login(string code, ref UserInfo userInfo, ref string openId, ref bool isReg);
        BDUserInfoDto GetUserInfo(string encryptedData, string iv, string sessionKey);
        BDMobileDto GetMobile(string encryptedData, string iv, string sessionKey);
        BDAuthInfoDto GetAuthInfo(Guid userId);
    }
}
