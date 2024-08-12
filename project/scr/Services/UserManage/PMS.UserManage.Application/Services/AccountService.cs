using Microsoft.Extensions.Options;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Application.ModelDto.Account;
using PMS.UserManage.Domain.Common;
using PMS.UserManage.Application.ModelDto;
using PMS.UserManage.Domain.IRepositories;
using ProductManagement.API.SMS;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using ProductManagement.UserCenter.TencentCommon.Model;
using ProductManagement.UserCenter.Wx.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using PMS.UserManage.Domain.Entities;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using static PMS.UserManage.Domain.Common.EnumSet;
using PMS.Infrastructure.Domain.Entities;
using PMS.CommentsManage.Application.IServices;
using WeChat.Interface;
using ProductManagement.API.Http.Interface;
using ProductManagement.API.Http.Model;
using WeChat.Model;
using PMS.UserManage.Application.ModelDto.Home;
using System.Threading.Tasks;

namespace PMS.UserManage.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IGiveLikeService _giveLike;
        private readonly IMessageRepository _messageRepo;
        private readonly ICollectionRepository _collectionRepo;
        private readonly IEasyRedisClient _easyRedisClient;
        private readonly INewAccountRepository _accountRepo;
        private readonly IUserRepository _userRepo;
        private readonly IInterestRepository _interestRepo;
        private readonly IsHost _IsHost;
        private readonly SMSHelper _SMS;
        private readonly IWeChatAppClient _weChatAppClient;
        private readonly IWeChatQRCodeService _weChatQRCodeService;
        public AccountService(IGiveLikeService giveLike,
            IMessageRepository messageRepo,
            ICollectionRepository collectionRepo,
            INewAccountRepository accountRepo,
            IInterestRepository interestRepo,
            IUserRepository userRepo,
            IEasyRedisClient easyRedisClient,
            IWeChatAppClient weChatAppClient,
            IWeChatQRCodeService weChatQRCodeService,
            IOptions<IsHost> IsHost)
        {
            _giveLike = giveLike;
            _messageRepo = messageRepo;
            _collectionRepo = collectionRepo;
            _accountRepo = accountRepo;
            _interestRepo = interestRepo;
            _userRepo = userRepo;
            _easyRedisClient = easyRedisClient;
            _IsHost = IsHost.Value;

            _SMS = new SMSHelper(easyRedisClient);
            _weChatAppClient = weChatAppClient;
            _weChatQRCodeService = weChatQRCodeService;
        }
        public BindInfo GetBindInfo(Guid userID)
        {
            var bindInfo = new BindInfo()
            {
                Mobile = _accountRepo.GetBindMobile(userID),
                UnionID_QQ = _accountRepo.GetBindQQ(userID),
                UnionID_Weixin = _accountRepo.GetBindWeixin(userID)
            };
            if (!string.IsNullOrEmpty(bindInfo.Mobile))
            {
                bindInfo.Mobile = UserCenterCommonHelper.HideNumber(bindInfo.Mobile);
            }
            return bindInfo;
        }
        public UserInfo GetUserInfo(Guid userID, bool hideNumber = true)
        {
            var info = _accountRepo.GetUserInfo(userID);
            if (!string.IsNullOrEmpty(info.Mobile) && hideNumber)
            {
                info.Mobile = UserCenterCommonHelper.HideNumber(info.Mobile);
            }
            return info;
        }
        public bool IsBindPhone(Guid userId)
        {
            return !string.IsNullOrWhiteSpace(_accountRepo.GetBindMobile(userId));
        }
        public bool IsBindWx(Guid userId)
        {
            return _accountRepo.GetBindWeixin(userId) != null;
        }
        public bool ChangePSW(Guid kid, short nationCode, string mobile, string password, string rndCode, out string failResult, bool mobileIsEncode = false, string codeType = "ChangePSW")
        {
            failResult = null;
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(rndCode))
            {
                failResult = "请填写完整信息";
                return false;
            }
            string privateKey = _easyRedisClient.GetAsync<string>($"privateKey:{kid}").Result?.ToString();
            if (string.IsNullOrEmpty(privateKey))
            {
                failResult = "呆太久了，再试一下吧";
                return false;
            }
            try
            {
                if (mobileIsEncode)
                {
                    mobile = RSAHelper.RSADecrypt(privateKey, mobile);
                }
                password = RSAHelper.RSADecrypt(privateKey, password);
                password = MD5Helper.GetMD5(password);
            }
            catch (Exception)
            {
                failResult = "数据解密失败，请再试一下吧";
                return false;
            }
            if (!_SMS.CheckRndCode(nationCode, mobile, rndCode, codeType, false))
            {
                failResult = "验证码错误或已失效";
                return false;
            }
            if (_accountRepo.ChangePSW(nationCode, mobile, Guid.Parse(password)))
            {
                _easyRedisClient.RemoveAsync("login:RNDCode-" + mobile + "-" + codeType);
                return true;
            }
            else
            {
                failResult = "密码修改失败";
                return false;
            }
        }
        public bool UpdateUserInfo(UserInfo userInfo)
        {
            return _accountRepo.UpdateUserInfo(userInfo);
        }


        public string GetBindMobile(Guid userID)
        {
            return _accountRepo.GetBindMobile(userID);
        }
        public bool BindMobile(Guid userID, Guid kid, short nationCode_o, string mobile_o, short nationCode_n, string mobile_n, string rndCode, string rndCode_o, out string failResult, out int status)
        {
            failResult = null;
            status = 1;
            if (string.IsNullOrEmpty(mobile_n) || string.IsNullOrEmpty(rndCode))
            {
                status = 2002;
                failResult = "请填写完整信息";
                return false;
            }
            string privateKey = _easyRedisClient.GetAsync<string>($"privateKey:{kid.ToString()}").Result;
            if (string.IsNullOrEmpty(privateKey))
            {
                status = 2003;
                failResult = "呆太久了，再试一下吧";
                return false;
            }
            try
            {
                mobile_n = RSAHelper.RSADecrypt(privateKey, mobile_n);
                mobile_o = string.IsNullOrEmpty(mobile_o) ? null : RSAHelper.RSADecrypt(privateKey, mobile_o);
            }
            catch (Exception)
            {
                status = 2004;
                failResult = "数据解密失败，请再试一下吧";
                return false;
            }
            if ((nationCode_o + mobile_o) == (nationCode_n + mobile_n))
            {
                status = 2005;
                failResult = "新旧号码一样无需修改";
                return false;
            }
            if (!_SMS.CheckRndCode(nationCode_n, mobile_n, rndCode, "ModifyMobile-N", false))
            {
                status = 2006;
                failResult = "目标验证码错误或已失效";
                return false;
            }

            var userInfo = _accountRepo.GetUserInfo(userID);
            if (userInfo == null || userInfo.Id == default)
            {
                status = 2007;
                failResult = "用户信息丢失";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(userInfo.Mobile))
            {
                if (userInfo.Mobile != mobile_o || userInfo.NationCode != nationCode_o)
                {
                    status = 2008;
                    failResult = "原手机号码不正确";
                    return false;
                }
                if (!_SMS.CheckRndCode(nationCode_o, mobile_o, rndCode_o, "ModifyMobile-O", false))
                {
                    status = 2009;
                    failResult = "原手机验证码错误或已失效";
                    return false;
                }
            }

            if (!_accountRepo.CheckMobileConflict(nationCode_n, mobile_n))
            {
                status = 2001;
                failResult = "无法完成本次操作，此手机号已被绑定到其它账号，请更改绑定的手机号";
                return false;
            }

            //if (_accountRepo.GetMobileBindCount(mobile_n) >= 10)
            //{
            //    status = 2001;
            //    failResult = "该手机号的绑定条目过多 , 请更改绑定的手机号";
            //    return false;
            //}

            if (_accountRepo.BindMobile(userID, nationCode_n, mobile_n))
            {
                status = 0;
                _easyRedisClient.RemoveAsync("login:RNDCode-" + nationCode_n + mobile_n + "-" + "ModifyMobile-N");
                _easyRedisClient.RemoveAsync("login:RNDCode-" + nationCode_o + mobile_o + "-" + "ModifyMobile-O");
                return true;
            }
            else
            {
                status = 201;
                failResult = "手机号绑定失败";
                return false;
            }
        }
        public bool BindMobile(Guid userID, short nationCode_n, string mobile_n)
        {
            return _accountRepo.BindMobile(userID, nationCode_n, mobile_n);
        }

        public Conflict ListConflict(Guid userID, Guid kid, short nationCode, ref string data, string rndCode, byte dataType, bool removeRndCache = true)
        {
            Conflict model = new Conflict();
            model.status = 2001;
            if (dataType == 0)
            {
                string privateKey = _easyRedisClient.GetAsync<string>($"privateKey:{kid.ToString()}").Result;
                if (string.IsNullOrEmpty(privateKey))
                {
                    model.status = 1;
                    model.errorDescription = "呆太久了，再试一下吧";
                    return model;
                }
                try
                {
                    data = RSAHelper.RSADecrypt(privateKey, data);
                }
                catch (Exception)
                {
                    model.status = 1;
                    model.errorDescription = "数据解密失败，请再试一下吧";
                    return model;
                }
                if (string.IsNullOrEmpty(data) || string.IsNullOrEmpty(rndCode))
                {
                    model.status = 1;
                    model.errorDescription = "无效请求";
                    return model;
                }
                if (!_SMS.CheckRndCode(nationCode, data, rndCode, "RegistOrLogin", removeRndCache))
                {
                    model.status = 1;
                    model.errorDescription = "验证失败，请再试一下吧";
                    return model;
                }
                model.dataType = 0;
                model.data = UserCenterCommonHelper.HideNumber(data);
                model.list = ListMobileConflict(nationCode, data);
                return model;
            }
            else if (dataType == 1)
            {
                var WXuserinfo = _easyRedisClient.GetAsync<WXUserInfoResult>("WXAuth:WXBindInfo-" + userID).Result;
                if (WXuserinfo == null)
                {
                    model.status = 1;
                    model.errorDescription = "呆太久了，再试一下吧";
                    return model;
                }
                model.dataType = 1;
                model.data = WXuserinfo.nickname;
                model.list = _accountRepo.ListWXConflict(WXuserinfo.unionid);
                return model;
            }
            else if (dataType == 2)
            {
                var OpenIDInfo = _easyRedisClient.GetAsync<QQOpenID>("QQAuth:QQBindInfo-" + userID).Result;
                var QQUserInfo = _easyRedisClient.GetAsync<QQUserInfoResult>("QQAuth:QQBindUserInfo-" + userID).Result;
                if (OpenIDInfo == null)
                {
                    model.status = 1;
                    model.errorDescription = "呆太久了，再试一下吧";
                    return model;
                }
                model.dataType = 2;
                model.data = QQUserInfo.nickname;
                model.list = _accountRepo.ListQQConflict(Guid.Parse(OpenIDInfo.unionid), Guid.Parse(OpenIDInfo.openid));
                return model;
            }
            else
            {
                model.status = 1;
                model.errorDescription = "无效请求";
                return model;
            }
        }
        public List<UserInfo> ListMobileConflict(short nationCode, string mobile)
        {
            return _accountRepo.ListMobileConflict(nationCode, mobile);
        }

        #region 用户兴趣
        public bool SetUserInterest(Interest interest, string uuid)
        {
            _easyRedisClient.RemoveAsync("interest-" + interest.UserID + "-" + uuid);
            return _interestRepo.SetUserInterest(interest);
        }
        private Interest GetDBOInterest(Guid? userID, string _uuid)
        {
            Guid? uuid = null;
            if (!string.IsNullOrEmpty(_uuid))
            {
                uuid = Guid.Parse(MD5Helper.GetMD5(_uuid));
            }
            var interest = _interestRepo.GetUserInterest(userID, uuid);
            if (interest == null)
            {
                interest = new Interest() { UserID = userID, UuID = uuid };
            }
            return interest;
        }
        public ApiInterestVo GetApiInterest(Guid? userID, string _uuid)
        {
            var interestModel = GetDBOInterest(userID, _uuid);
            ApiInterestVo apiInterest = new ApiInterestVo();
            JObject jobj = JObject.Parse(JsonConvert.SerializeObject(interestModel));
            foreach (var a in jobj)
            {
                if (a.Key.ToLower().Contains("grade") && Convert.ToBoolean(a.Value))
                {
                    int _enum = int.Parse(a.Key.Split('_').Last());
                    apiInterest.Interest.grade.Add(_enum);
                }
                if (a.Key.ToLower().Contains("nature") && Convert.ToBoolean(a.Value))
                {
                    int _enum = int.Parse(a.Key.Split('_').Last());
                    apiInterest.Interest.nature.Add(_enum);
                }
                if (a.Key.ToLower().Contains("lodging") && Convert.ToBoolean(a.Value))
                {
                    int _enum = int.Parse(a.Key.Split('_').Last());
                    apiInterest.Interest.lodging.Add(_enum);
                }
            }
            if (apiInterest.Interest.lodging.Count == 2)
            {
                apiInterest.Interest.lodging = new List<int>();
            }
            _easyRedisClient.AddAsync("interest-" + (userID.Equals(Guid.Empty) ? "" : userID.ToString()) + "-" + _uuid, apiInterest, new TimeSpan(0, 0, 0, 3600));
            return apiInterest;
        }
        public PMS.UserManage.Application.ModelDto.Info.Interest GetUserInterest(Guid? userID, string _uuid)
        {
            var interestModel = GetDBOInterest(userID, _uuid);
            PMS.UserManage.Application.ModelDto.Info.Interest interest = new PMS.UserManage.Application.ModelDto.Info.Interest();
            var columnList = _interestRepo.GetInterestColumns();
            JObject jobj = JObject.Parse(JsonConvert.SerializeObject(interestModel).ToLower());
            foreach (var column in columnList)
            {
                column.selected = Convert.ToBoolean(jobj[column.Key]);
                if (column.Key.ToLower().Contains("grade"))
                {
                    interest.grade.Add(column);
                }
                else if (column.Key.ToLower().Contains("nature"))
                {
                    interest.nature.Add(column);
                }
                else if (column.Key.ToLower().Contains("lodging"))
                {
                    interest.lodging.Add(column);
                }
            }
            return interest;
        }
        #endregion

        public CountData GetCountData(Guid userID, string cookieStr)
        {
            try
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("Cookie", cookieStr);

                var Usercount = _giveLike.StatisticalSum(userID);
                //var m2Count = _client.GetAsAsync<VoBase>($"{_IsHost.SiteHost_M}/Common/GetCurrentPublishTotal", headers).Result;
                //var m2CountData = JsonConvert.DeserializeObject<M2CountData>(m2Count.data.ToString());
                return new CountData()
                {
                    follow = _collectionRepo.GetCollectionCount(userID),
                    message = _messageRepo.GetMessageCount(userID),
                    publish = Usercount.PublishTotal,
                    reply = Usercount.AnswerAndReplyTotal,
                    like = Usercount.LikeTotal
                };
            }
            catch
            {
                return new CountData();
            }
        }


        public List<UserInfo> GetUserInfos(List<Guid> ids)
        {
            List<Guid> idsCopy = ids.Distinct().ToList();
            List<UserInfo> result = _accountRepo.ListUserInfo(idsCopy);
            List<UserInfo> OutputResult = new List<UserInfo>();
            foreach (Guid id in ids)
            {
                var r = result.Find(a => a.Id == id);
                if (r == null)
                {
                    OutputResult.Add(new UserInfo() { Id = id });
                }
                else
                {
                    OutputResult.Add(r);
                }
            }
            return OutputResult;
        }
        public void SetDeviceToken(string _uuid, string deviceToken, DeviceType type, Guid userID, int city)
        {

            Guid uuid = Guid.Parse(MD5Helper.GetMD5(_uuid));
            var model = new DeviceToken()
            {
                uuID = uuid,
                deviceToken = deviceToken,
                userID = userID.Equals(Guid.Empty) ? (Guid?)null : userID,
                type = type,
                city = city
            };
            _userRepo.SetDeviceToken(model);
        }


        public async Task<int> CheckAccountStatus(Guid userID)
        {
            object status = await _easyRedisClient.GetOrAddAsync($"AccountStatus-{userID.ToString().ToLower()}", () =>
            {
                return _accountRepo.CheckAccountStatus(userID);
            }, TimeSpan.FromHours(1));
            //object status = _easyRedisClient.GetAsync<object>($"AccountStatus-{userID.ToString().ToLower()}").Result;
            //if (status == null)
            //{
            //    status = _accountRepo.CheckAccountStatus(userID);
            //}
            return (int)status;
        }

        public string GenerateBindAccountQRCode(string callbackUrl, int? expireDay = 5)
        {
            string sceneKey = $"bindAccount-{Guid.NewGuid()}";
            int expire_second = (int)TimeSpan.FromDays(10).TotalSeconds; //10天有效期的二维码
            var accessToken = _weChatAppClient.GetAccessToken(new WeChatGetAccessTokenRequest() { App = "fwh" }).GetAwaiter().GetResult();
            _ = _easyRedisClient.AddStringAsync(sceneKey, callbackUrl, TimeSpan.FromSeconds(expire_second));
            var Scene = new CreateTempQRStrSceneCodeRequest(expire_second);
            Scene.SetScene(sceneKey);
            var qrcodeResponse = _weChatQRCodeService.GenerateTempQRCode(accessToken.token, Scene).GetAwaiter().GetResult();
            return qrcodeResponse?.ImgUrl;
        }
    }
}
