using PMS.UserManage.Application.ModelDto;
using PMS.UserManage.Application.ModelDto.Account;
using PMS.UserManage.Application.ModelDto.Home;
using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static PMS.UserManage.Domain.Common.EnumSet;

namespace PMS.UserManage.Application.IServices
{
    public interface IAccountService
    {
        UserInfo GetUserInfo(Guid userID, bool hideNumber = true);
        /// <summary>
        /// 获取用户是否绑定了手机
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool IsBindPhone(Guid userId);
        /// <summary>
        /// 获取用户是否绑定了微信
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool IsBindWx(Guid userId);
        BindInfo GetBindInfo(Guid userID);
        /// <summary>
        /// 绑定手机号
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="nationCode"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        bool BindMobile(Guid userID, Guid kid, short nationCode_o, string mobile_o, short nationCode_n, string mobile_n, string rndCode, string rndCode_o, out string failResult, out int status);
        bool BindMobile(Guid userID, short nationCode_n, string mobile_n);
        bool ChangePSW(Guid kid, short nationCode, string mobile, string password, string rndCode, out string failResult, bool mobileIsEncode = false, string codeType = "ChangePSW");
        bool UpdateUserInfo(UserInfo userInfo);
        Conflict ListConflict(Guid userID, Guid kid, short nationCode, ref string data, string rndCode, byte dataType, bool removeRndCache = true);
        List<UserInfo> ListMobileConflict(short nationCode, string mobile);
        /// <summary>
        /// 当前UserID是否被封禁
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        Task<int> CheckAccountStatus(Guid userID);

        bool SetUserInterest(Interest interest, string uuid);
        ApiInterestVo GetApiInterest(Guid? userID, string _uuid);
        PMS.UserManage.Application.ModelDto.Info.Interest GetUserInterest(Guid? userID, string _uuid);

        CountData GetCountData(Guid userID, string cookieStr);
        List<UserInfo> GetUserInfos(List<Guid> ids);
        void SetDeviceToken(string _uuid, string deviceToken, DeviceType type, Guid userID, int city);
        string GenerateBindAccountQRCode(string callbackUrl, int? expireDay = 5);
    }
}
