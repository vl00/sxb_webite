using PMS.UserManage.Application.ModelDto;
using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.UserManage.Application.IServices
{
    public interface IOAuthWeixinService
    {
        void WXLogin(string unionid, string openid, string sessionKey, string appName, string appId, ref UserInfo userInfo, ref bool isReg, bool subscribeStatus = false);
        bool BindWX(string unionid, string openid, string sessionKey, string appName, string appId, Guid userId, string nickname, bool subscribeStatus = false);
        bool UnboundWx(Guid userID);
        WXAuthInfoDto GetWXAuthInfo(Guid userId, string appName);
        /// <summary>
        /// 检查微信UnionID号是否冲突,返回true是没有冲突
        /// </summary>
        /// <returns></returns>
        bool CheckWXCanBind(string unionID, Guid userID);
        string GetBindWxUnionId(Guid useId);
        List<UserInfo> ListWXConflict(string unionID, string openID);
        bool UpdateWeiXinSessionKey(string openID, string session_Key);
        /// <summary>
        /// 更换openID对应的UserID
        /// </summary>
        /// <param name="oldUserID">旧userID</param>
        /// <param name="newUserID">新userID</param>
        /// <returns></returns>
        Task<bool> ChangeOpenIDUser(Guid oldUserID, Guid newUserID);
    }
}
