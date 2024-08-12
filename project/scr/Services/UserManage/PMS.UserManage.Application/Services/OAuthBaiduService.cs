using System;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Application.ModelDto;
using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Domain.IRepositories;
using ProductManagement.UserCenter.BaiduCommon;

namespace PMS.UserManage.Application.Services
{
    public class OAuthBaiduService : IOAuthBaiduService
    {
        private readonly IBaiduOAuthClient _OAuthClient;
        private readonly INewAccountRepository _accountRepo;
        public OAuthBaiduService(IBaiduOAuthClient OAuthClient, INewAccountRepository accountRepo)
        {
            _OAuthClient = OAuthClient;
            _accountRepo = accountRepo;
        }

        public bool Login(string code, ref UserInfo userInfo, ref string openId, ref bool isReg)
        {
            var bdAuth = _OAuthClient.GetBaiduAccessKey(code).Result;

            if (bdAuth == null || bdAuth.Errno != 0)//获取access失败
            {
                return false;
            }
            openId = bdAuth.Openid;
            userInfo = _accountRepo.GetUserByBaiduOpenId(bdAuth.Openid);
            if (userInfo == null)
            {
                userInfo = new UserInfo
                {
                    Id = Guid.NewGuid()
                };
                if (string.IsNullOrEmpty(userInfo.NickName))
                {
                    userInfo.NickName = "百度用户-" + userInfo.Id.ToString("N").Substring(0, 8);
                }
                isReg = _accountRepo.CreateUserInfo(userInfo);
                if (isReg)
                    _accountRepo.BindBDOpenID(bdAuth.Openid, userInfo.Id, bdAuth.Session_key);
            }
            else if (userInfo.Blockage)
            {
                return false;
            }
            bool isLogin = _accountRepo.UpdateLoginTime(userInfo.Id);
            return isLogin;
        }


        public BDUserInfoDto GetUserInfo(string encryptedData, string iv, string sessionKey)
        {
            string decryptData = AES_decrypt(encryptedData, iv, sessionKey);
            // 解密结果应该是 '{"openid":"open_id","nickname":"baidu_user","headimgurl":"url of image","sex":1}'
            var bdUserInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<BDUserInfoDto>(decryptData);

            var userInfo = _accountRepo.GetUserByBaiduOpenId(bdUserInfo.OpenId);
            if (userInfo != null)
            {
                userInfo.NickName = bdUserInfo.NickName;
                //百度接口性别：值为 0 时是未知，为 1 时是男性，为 2 时是女性。需要转换
                userInfo.Sex = bdUserInfo.Sex == 0 ? (int?)null : (bdUserInfo.Sex == 2 ? 0 : 1);
                userInfo.HeadImgUrl = bdUserInfo.HeadImgUrl;

                _accountRepo.UpdateUserInfo(userInfo);
            }
            return bdUserInfo;
        }
        public BDMobileDto GetMobile(string encryptedData, string iv, string sessionKey)
        {
            string decryptData = AES_decrypt(encryptedData, iv, sessionKey);
            // 解密结果应该是 '{"mobile":"13826220070"}'
            var bdInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<BDMobileDto>(decryptData);

            return bdInfo;
        }

        public BDAuthInfoDto GetAuthInfo(Guid userId)
        {
            var authInfo = _accountRepo.GetBaiduOpenId(userId);
            if (authInfo == null)
            {
                return null;
            }
            return new BDAuthInfoDto
            {
                UserId = authInfo.UserId,
                OpenId = authInfo.OpenId,
                AccessKey = authInfo.AccessKey
            };
        }


        private string AES_decrypt(string Input, string Iv, string Key)
        {

            System.Security.Cryptography.RijndaelManaged aes = new System.Security.Cryptography.RijndaelManaged
            {
                KeySize = 256,
                BlockSize = 128,
                Mode = System.Security.Cryptography.CipherMode.CBC,
                Padding = System.Security.Cryptography.PaddingMode.None,
                Key = Convert.FromBase64String(Key),
                IV = Convert.FromBase64String(Iv)
            };
            var decrypt = aes.CreateDecryptor(aes.Key, aes.IV);
            byte[] xBuff = null;
            using (var ms = new System.IO.MemoryStream())
            {
                byte[] bytes = null;
                using (var cs = new System.Security.Cryptography.CryptoStream(ms, decrypt, System.Security.Cryptography.CryptoStreamMode.Write))
                {
                    byte[] xXml = Convert.FromBase64String(Input);
                    byte[] msg = new byte[xXml.Length + 32 - xXml.Length % 32];
                    Array.Copy(xXml, msg, xXml.Length);
                    cs.Write(xXml, 0, xXml.Length);
                }
                bytes = Decode2(ms.ToArray());

                // 分离16位随机字符串,去除随机填充内容
                byte[] networkOrder = new byte[4];
                Array.Copy(bytes, 16, networkOrder, 0, 4);
                int xmlLength = RecoverNetworkBytesOrder(networkOrder);

                xBuff = new byte[bytes.Length];
                Array.Copy(bytes, 20, xBuff, 0, xmlLength);
            }
            return System.Text.Encoding.UTF8.GetString(xBuff);
        }
        private int RecoverNetworkBytesOrder(byte[] orderBytes)
        {
            int sourceNumber = 0;
            int length = 4;
            int number = 8;
            for (int i = 0; i < length; i++)
            {
                sourceNumber <<= number;
                sourceNumber |= orderBytes[i] & 0xff;
            }
            return sourceNumber;
        }
        //删除解密后明文的补位字符
        private byte[] Decode2(byte[] decrypted)
        {
            int pad = (int)decrypted[decrypted.Length - 1];
            if (pad < 1 || pad > 32)
            {
                pad = 0;
            }
            byte[] res = new byte[decrypted.Length - pad];
            Array.Copy(decrypted, 0, res, 0, decrypted.Length - pad);
            return res;
        }
    }
}
