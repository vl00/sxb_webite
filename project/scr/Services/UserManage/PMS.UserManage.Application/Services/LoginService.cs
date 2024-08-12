using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Domain.IRepositories;
using ProductManagement.API.SMS;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;

namespace PMS.UserManage.Application.Services
{
    /// <summary>
    /// 手机号登录
    /// </summary>
    public class LoginService:ILoginService
    {
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly INewAccountRepository _accountRepository;
        private readonly SMSHelper _SMS;

        public LoginService(INewAccountRepository accountRepository,
            IEasyRedisClient easyRedisClient)
        {
            _accountRepository = accountRepository;
            _easyRedisClient = easyRedisClient;
            _SMS = new SMSHelper(_easyRedisClient);
        }

        public async Task<string> GetUserCode(Guid userId) {

            //不考虑userId已经存在

            //登录凭证
            var code =  ShakeNoHelper.Shake();
            var key = string.Format(RedisKeys.UserLoginCodeKey, code);
            
            
            var success = await _easyRedisClient.AddAsync(key, userId, TimeSpan.FromMinutes(5));
            if (success)
            {
                return code;
            }
            return string.Empty;
        }

        public async Task<UserInfo> LoginByCode(string code)
        {
            var key = string.Format(RedisKeys.UserLoginCodeKey, code);
            var userId = await _easyRedisClient.GetAsync<Guid>(key);
            if (userId != Guid.Empty)
            {
                _accountRepository.UpdateLoginTime(userId);
                var userInfo = _accountRepository.GetUserInfo(userId);

                //使用一次后移除code
                await _easyRedisClient.RemoveAsync(key);
                return userInfo;
            }
            return null;
        }

        public UserInfo Login(Guid kid, short nationCode, string mobile, string password, string rndCode, out string failResult, string nickname, int? city, out bool mobileRepeatUser)
        {
            mobileRepeatUser = false;
            string publicKey = _easyRedisClient.GetAsync<string>($"privateKey:{kid}")?.Result;
            if (string.IsNullOrEmpty(publicKey))
            {
                failResult = "呆太久了，再试一下吧";
                return null;
            }
            try
            {
                mobile = RSAHelper.RSADecrypt(publicKey, mobile);
                password = string.IsNullOrEmpty(password) ? null : RSAHelper.RSADecrypt(publicKey, password);
            }
            catch (Exception)
            {
                failResult = "数据解密失败，请再试一下吧";
                return null;
            }

            if (!string.IsNullOrWhiteSpace(mobile) && !string.IsNullOrEmpty(rndCode))
            {
                string rndCodePassword = MD5Helper.GetMD5(rndCode);
                //部分邀请学校用户会多人使用单账号，新增使用验证码参数验证密码

                if (!_SMS.CheckRndCode(nationCode, mobile, rndCode, "RegistOrLogin") &&
                    !_accountRepository.CheckPasswordLogin(nationCode, mobile, rndCodePassword))
                {
                    failResult = "验证码错误";
                    return null;
                }
            }
            else if (!string.IsNullOrWhiteSpace(mobile) && !string.IsNullOrEmpty(password))
            {
                password = MD5Helper.GetMD5(password);
                if (!_accountRepository.CheckPasswordLogin(nationCode, mobile, password))
                {
                    failResult = "手机号或密码错误";
                    return null;
                }
            }
            else
            {
                failResult = "请输入完整登录信息";
                return null;
            }
            UserInfo user = GetLoginUser(nationCode, mobile, out failResult, out mobileRepeatUser);
            return user;
        }


        private UserInfo GetLoginUser(short nationCode, string mobile, out string failResult, out bool mobileRepeatUser)
        {
            UserInfo user = new UserInfo();
            failResult = null;
            var users = _accountRepository.GetUserInfoByMobile(nationCode, mobile);
            if (users.Any())
            {
                var unblockUsers = users.Where(q => q.Blockage == false).ToList();
                mobileRepeatUser = unblockUsers.Count() > 1;

                if (users.Any() && !unblockUsers.Any())
                {
                    failResult = "账号已被封禁，请与客服联系";
                }
                else
                {
                    user = unblockUsers.FirstOrDefault();
                    _accountRepository.UpdateLoginTime(user.Id);
                }
            }
            else
            {
                mobileRepeatUser = false;

                user.NationCode = nationCode;
                user.Mobile = mobile;

                RegisUserInfo(ref user);
            }
            return user;
        }


        public bool RegisUserInfo(ref UserInfo userInfo)
        {
            userInfo.Id = userInfo.Id == Guid.Empty ? Guid.NewGuid() : userInfo.Id;
            userInfo.NickName = string.IsNullOrEmpty(userInfo.NickName) ? "手机用户-" + userInfo.Id.ToString("N").Substring(0, 8) : userInfo.NickName;
            userInfo.HeadImgUrl = string.IsNullOrWhiteSpace(userInfo.HeadImgUrl) ? "https://cos.sxkid.com/images/head.png" : userInfo.HeadImgUrl;
            return _accountRepository.CreateUserInfo(userInfo);
        }

        public List<UserInfo> GetSameMobileUsers(short nationCode, string mobile)
        {
            var users = _accountRepository.GetUserInfoByMobile(nationCode, mobile);
            var unblockUsers = users.Where(q => q.Blockage == false).ToList();
            return unblockUsers;
        }

        public UserInfo SelectUserLogin(Guid userid)
        {
            _accountRepository.UpdateLoginTime(userid);
            return _accountRepository.GetUserInfo(userid);
        }

        public bool CheckUserMobile(Guid userid, short nationCode, string mobile)
        {
            return _accountRepository.CheckBindMobile(userid, nationCode, mobile);
        }

        public bool Login(string appId, string secret)
        {
            if (appId == null || secret == null)
            {
                return false;
            }
            return appId.ToUpper() == "757EA0330D7E4A5C826E1776BD45F927" && secret == "6iKjNcmiBufUJ7Ki";
        }

        public string GetSecret(string appId)
        {
            if (appId == null)
            {
                return string.Empty;
            }
            if (appId.ToUpper() == "757EA0330D7E4A5C826E1776BD45F927")
            {
                return "6iKjNcmiBufUJ7Ki";
            }
            return string.Empty;
        }
    }
}
