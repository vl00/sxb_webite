using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Application.ModelDto;
using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Domain.IRepositories;

namespace PMS.UserManage.Application.Services
{
    public class OAuthWeixinService : IOAuthWeixinService
    {
        private readonly INewAccountRepository _accountRepo;
        public OAuthWeixinService(INewAccountRepository accountRepo)
        {
            _accountRepo = accountRepo;
        }

        public void WXLogin(string unionid, string openid, string sessionKey, string appName, string appId, ref UserInfo userInfo, ref bool isReg, bool subscribeStatus = false)
        {
            var user = new ThirdAuthUser();

            if (!string.IsNullOrEmpty(unionid))
            {
                user = _accountRepo.GetWXUnionIDUserInfo(unionid, appId);
            }
            else
            {
                user = _accountRepo.GetWXOpenIDUserInfo(openid);
            }

            if (user != null)
            {
                //账户如果封禁则直接返回
                if (user.Blockage) return;
                //如果登录的用户缺少UnionId或者 某appId的OpenId，尝试补充完整
                if (string.IsNullOrWhiteSpace(user.UnionId) && !string.IsNullOrWhiteSpace(unionid))
                {
                    _accountRepo.BindWXUnionID(unionid, user.Id, userInfo.NickName);
                }
                if (string.IsNullOrWhiteSpace(user.OpneId) && !string.IsNullOrWhiteSpace(openid))
                {
                    _accountRepo.BindWXOpenID(openid, user.Id, sessionKey, appName, appId, subscribeStatus);
                }
                else
                {
                    _accountRepo.UpdateWeiXinSessionKey(openid, sessionKey);
                }
                _accountRepo.UpdateLoginTime(user.Id);

                var hasNewImage = string.IsNullOrWhiteSpace(userInfo.HeadImgUrl);
                var hasOldImage = string.IsNullOrWhiteSpace(user.HeadImgUrl)
                    || user.HeadImgUrl == "https://cos.sxkid.com/images/head.png"
                    || user.HeadImgUrl == "https://file.sxkid.com/images/head.png";

                // 传入了头像, 并且库中头像是原始头像或者空, 则使用传入的头像
                var headImageUrl = hasNewImage && hasOldImage ? userInfo.HeadImgUrl : user.HeadImgUrl;
                userInfo = new UserInfo
                {
                    Id = user.Id,
                    NickName = user.NickName,
                    HeadImgUrl = headImageUrl,
                    Sex = user.Sex,
                    Mobile = user.Mobile,
                    Introduction = user.Introduction,
                    VerifyTypes = user.VerifyTypes
                };
            }
            else
            {
                //新建用户，并且绑定微信的UnionId和OpenId
                isReg = true;
                userInfo.Id = userInfo.Id == Guid.Empty ? Guid.NewGuid() : userInfo.Id;
                userInfo.NickName = string.IsNullOrEmpty(userInfo.NickName) ? "微信用户-" + userInfo.Id.ToString("N").Substring(0, 8) : userInfo.NickName;
                userInfo.HeadImgUrl = string.IsNullOrWhiteSpace(userInfo.HeadImgUrl) ? "https://cos.sxkid.com/images/head.png" : userInfo.HeadImgUrl;

                if (string.IsNullOrEmpty(userInfo.NickName))
                {
                    userInfo.NickName = "微信用户-" + userInfo.Id.ToString("N").Substring(0, 8);
                }
                else
                {
                    if (_accountRepo.CheckRepeatNickName(userInfo.NickName))
                    {
                        //处理当前昵称和数据库已有用户的昵称冲突
                        userInfo.NickName += userInfo.Id.ToString().Substring(0, 8);
                    }
                }

                _accountRepo.CreateUserInfo(userInfo);
                BindWX(unionid, openid, sessionKey, appName, appId, userInfo.Id, userInfo.NickName, subscribeStatus);
            }
        }



        public bool BindWX(string unionid, string openid, string sessionKey, string appName, string appId, Guid userId, string nickname, bool subscribeStatus = false)
        {
            if (!string.IsNullOrWhiteSpace(unionid))
            {
                _accountRepo.BindWXUnionID(unionid, userId, nickname);
            }
            if (!string.IsNullOrWhiteSpace(openid))
            {
                _accountRepo.BindWXOpenID(openid, userId, sessionKey, appName, appId, subscribeStatus);
            }
            return true;
        }



        public bool UnboundWx(Guid userID)
            => _accountRepo.UnboundWx(userID);


        public WXAuthInfoDto GetWXAuthInfo(Guid userId, string appName)
        {
            var authInfo = _accountRepo.GetWeixinOpenId(userId, appName);
            if (authInfo == null)
            {
                return null;
            }
            return new WXAuthInfoDto
            {
                UserId = authInfo.UserId,
                OpenId = authInfo.OpenId,
                SessionKey = authInfo.SessionKey
            };
        }

        public string GetBindWxUnionId(Guid useId)
        {
            return _accountRepo.GetBindWeixin(useId);
        }

        public List<UserInfo> ListWXConflict(string unionID, string openID)
        {
            return _accountRepo.ListWXConflict(unionID);
        }

        public bool CheckWXCanBind(string unionID, Guid userID)
        {
            return _accountRepo.CheckWXCanBind(unionID, userID);
        }

        public bool UpdateWeiXinSessionKey(string openID, string session_Key)
        {
            return _accountRepo.UpdateWeiXinSessionKey(openID, session_Key);
        }

        public async Task<bool> ChangeOpenIDUser(Guid oldUserID, Guid newUserID)
        {
            return await _accountRepo.UpdateOpenIDUserID(oldUserID, newUserID);
        }
    }
}
