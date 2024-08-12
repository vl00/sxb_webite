using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PMS.UserManage.Domain.Entities;

namespace PMS.UserManage.Domain.IRepositories
{
    public interface INewAccountRepository
    {
        bool CreateUserInfo(UserInfo userInfo);
        bool CheckRepeatNickName(string nickName);
        int CheckAccountStatus(Guid userID);
        bool UpdateUserInfo(UserInfo user);
        bool UpdateLoginTime(Guid userId);

        UserInfo GetUserInfo(Guid userId);
        List<UserInfo> ListUserInfo(List<Guid> userIds);

        #region 手机号账号
        List<UserInfo> GetUserInfoByMobile(short nationCode, string mobile);
        /// <summary>
        /// 判断旧手机号是否正确
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        bool OldMobileMatch(Guid userID, short nationCode, string mobile);
        /// <summary>
        /// 绑定手机号
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="nationCode"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        bool BindMobile(Guid userID, short nationCode, string mobile);
        /// <summary>
        /// 检查手机号是否冲突
        /// </summary>
        /// <param name="nationCode"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>

        bool CheckMobileConflict(short nationCode, string mobile);
        /// <summary>
        /// 检查用户绑定的手机号
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="nationCode"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        bool CheckBindMobile(Guid userID, short nationCode, string mobile);
        // <summary>
        /// 获取绑定的手机号
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        string GetBindMobile(Guid userID);
        /// <summary>
        /// 获取绑定手机号的账号数量
        /// </summary>
        /// <param name="mobileNo"></param>
        /// <returns></returns>
        int GetMobileBindCount(string mobileNo);

        bool CheckPasswordLogin(short nationCode, string mobile, string password);
        /// <summary>
        /// 修改手机号码
        /// </summary>
        /// <param name="nationCode"></param>
        /// <param name="mobile"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool ChangePSW(short nationCode, string mobile, Guid password);
        /// <summary>
        /// 列出手机号码冲突账号
        /// </summary>
        /// <param name="nationCode"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        List<UserInfo> ListMobileConflict(short nationCode, string mobile);
        #endregion

        #region 第三方账号--微信
        /// <summary>
        /// 检查微信UnionID号是否冲突
        /// </summary>
        /// <param name="unionID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool CheckWXCanBind(string unionID, Guid userID);
        /// <summary>
        /// 列出微信冲突账号
        /// </summary>
        /// <param name="unionid"></param>
        /// <returns></returns>
        List<UserInfo> ListWXConflict(string unionid);
        /// <summary>
        /// 绑定微信OpenID
        /// </summary>
        /// <param name="openID"></param>
        /// <param name="appName"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool BindWXOpenID(string openID, Guid userID, string sessionKey, string appName, string appId, bool subscribeStatus = false);
        /// <summary>
        /// 绑定微信UnionID
        /// </summary>
        /// <param name="unionID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool BindWXUnionID(string unionID, Guid userID, string nickname);
        /// <summary>
        /// 获取绑定的微信UnionID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        string GetBindWeixin(Guid userID);
        /// <summary>
        /// 微信解绑
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool UnboundWx(Guid userID);
        /// <summary>
        /// 获取微信OpenID绑定的用户信息
        /// </summary>
        /// <param name="openID"></param>
        /// <returns></returns>
        ThirdAuthUser GetWXOpenIDUserInfo(string openID);


        /// <summary>
        /// 获取微信unionID绑定的用户信息
        /// </summary>
        /// <param name="unionID"></param>
        /// <returns></returns>
        ThirdAuthUser GetWXUnionIDUserInfo(string unionID, string appId);


        OpenIdWeixin GetWeixinOpenId(Guid userId, string appName);
        bool UpdateWeiXinSessionKey(string openid, string sessionKey);
        Task<bool> UpdateOpenIDUserID(Guid oldUserID, Guid newUserID);
        #endregion

        #region 第三方账号--百度
        /// <summary>
        /// 绑定百度OpenID
        /// </summary>
        /// <param name="openID"></param>
        /// <param name="appName"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool BindBDOpenID(string openID, Guid userID, string accessKey, string appName = "baidubox", bool valid = true);

        UserInfo GetUserByBaiduOpenId(string openId);
        OpenidBaidu GetBaiduOpenId(Guid userId);
        #endregion

        #region 第三方账号--QQ
        /// <summary>
        /// 检查QQUnionID号是否冲突
        /// </summary>
        /// <param name="unionID"></param>
        /// <returns></returns>
        bool CheckQQConflict(Guid unionID);
        /// <summary>
        /// 列出QQ冲突账号
        /// </summary>
        /// <param name="unionid"></param>
        /// <returns></returns>
        List<UserInfo> ListQQConflict(Guid unionid, Guid openid);

        /// <summary>
        /// 绑定QQOpenID
        /// </summary>
        /// <param name="openID"></param>
        /// <param name="userID"></param>
        /// <param name="appName"></param>
        /// <returns></returns>
        bool BindQQOpenID(Guid openID, Guid userID, string appName);
        /// <summary>
        /// 绑定QQUnionID
        /// </summary>
        /// <param name="unionID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool BindQQUnionID(Guid unionID, Guid userID);
        /// <summary>
        /// 获取绑定的QQUnionID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        Guid? GetBindQQ(Guid userID);
        /// <summary>
        /// 解绑QQ
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        bool UnboundQQ(Guid userID);
        #endregion
    }
}
