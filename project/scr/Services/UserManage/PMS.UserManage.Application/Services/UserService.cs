using PMS.UserManage.Application.IServices;
using PMS.UserManage.Application.ModelDto;
using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Domain.IRepositories;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.UserManage.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public Domain.Entities.UserInfo GetUserInfoDetail(Guid userId)
        {
            return _userRepository.GetUserInfo(userId);
        }


        private static int[] ArrayStringToInt(string[] stringArray)
        {

            List<int> result = new List<int>();

            foreach (var item in stringArray.Where(q => !string.IsNullOrWhiteSpace(q)))
            {
                result.Add(int.Parse(item));
            }
            return result.ToArray();
        }

        private static UserInfoDto ToUserInfoDto(Domain.Entities.UserInfo info)
        {
            return new UserInfoDto
            {
                Id = info.Id,
                Mobile = info.Mobile,
                NickName = info.NickName,
                HeadImgUrl = string.IsNullOrWhiteSpace(info.HeadImgUrl) ? "https://cos.sxkid.com/images/school_v3/head.png" : info.HeadImgUrl,
                Blockage = info.Blockage,
                Sex = info.Sex,
                Channel = info.Channel,
                City = info.City,
                NationCode = info.NationCode,
                VerifyTypes = ArrayStringToInt((info.VerifyTypes ?? "").Split(',')),
                LoginTime = info.LoginTime,
                RegTime = info.RegTime
            };
        }

        private static UserInfoOpenIdDto ToUserInfoOpenIdDto(Domain.Entities.OpenIdWeixin info)
        {
            return new UserInfoOpenIdDto
            {
                OpenId = info.OpenId,
                UserId = info.UserId,
                AppId = info.AppId,
                AppName = info.AppName
            };
        }
        public UserInfoDto GetUserInfo(Guid userId)
        {
            var result = _userRepository.GetUserInfo(userId);

            if (result == null)
            {
                return new UserInfoDto();
            }
            return ToUserInfoDto(result);
        }

        public UserInfoDto GetUserInfo(string Phone)
        {
            var result = _userRepository.GetUserInfo(Phone);
            if (result == null)
            {
                return null;
            }

            return ToUserInfoDto(result);
        }

        public IEnumerable<UserInfoDto> GetUserInfosByPhone(string phone)
        {
            var result = _userRepository.GetUserInfos(phone);
            return result.Select(s => ToUserInfoDto(s));
        }

        public IEnumerable<Guid> GetSamplePhoneUserIds(Guid userId)
        {
            return _userRepository.GetSamplePhoneUserIds(userId);
        }

        public IEnumerable<UserInfoDto> GetUserInfosByName(string name)
        {
            var result = _userRepository.GetUserInfosByName(name);
            return result.Select(s => ToUserInfoDto(s));
        }

        public bool InsertUserInfo(Guid UserId, string userName, string phone, string headImg)
        {
            return _userRepository.AddUserInfo(new Domain.Entities.UserInfo
            {
                Id = UserId,
                NickName = userName,
                Mobile = phone,
                HeadImgUrl = headImg
            });
        }

        public bool SyncUserInfo(Guid UserId, string userName, string phone, string headImg)
        {
            var user = _userRepository.GetUserInfo(UserId);

            bool result;
            if (user == null)
            {
                result = _userRepository.AddUserInfo(new Domain.Entities.UserInfo
                {
                    Id = UserId,
                    NickName = userName,
                    Mobile = phone,
                    HeadImgUrl = headImg
                });
            }
            else
            {
                result = _userRepository.UpdateUserInfo(new Domain.Entities.UserInfo
                {
                    Id = UserId,
                    NickName = userName,
                    Mobile = phone,
                    HeadImgUrl = headImg
                });
            }
            return result;
        }


        public List<UserInfoDto> ListAllUserInfo()
        {
            var list = _userRepository.ListAllUserInfo();
            return list.Select(result => ToUserInfoDto(result)).ToList();
        }


        public List<UserInfoDto> ListUserInfo(List<Guid> userIds)
        {
            var list = _userRepository.ListUserInfo(userIds);
            return list.Select(result => ToUserInfoDto(result)).ToList();
        }
        public List<UserInfoOpenIdDto> ListUserOpenId(List<Guid> userIds)
        {
            var list = _userRepository.ListUserInfoOpenId(userIds);
            return list.Select(result => ToUserInfoOpenIdDto(result)).ToList();
        }


        public List<InviteUserInfoDto> GetUserInfoByHistory(List<Guid> SchoolIds, UserRole Role, int PageIndex, int PageSize, int type, int dataType, Guid senderID, Guid dataID, Guid eID, Guid userId, int city)
        {
            var userInfos = _userRepository.GetUserInfoByHistory(SchoolIds, Role, PageIndex, PageSize, type, dataType, senderID, dataID, eID, userId, city);
            return userInfos.Where(x => x.Id != default(Guid)).Select(result => new InviteUserInfoDto()
            {
                Id = result.Id,
                Mobile = result.Mobile,
                NickName = result.NickName,
                HeadImgUrl = string.IsNullOrWhiteSpace(result.HeadImgUrl) ? "https://cos.sxkid.com/images/school_v3/head.png" : result.HeadImgUrl,
                Blockage = result.Blockage,
                Sex = result.Sex,
                Channel = result.Channel,
                City = result.City,
                NationCode = result.NationCode,
                VerifyTypes = ArrayStringToInt((result.VerifyTypes ?? "").Split(',')),
                LoginTime = result.LoginTime,
                RegTime = result.RegTime,
                isInvite = result.IsInvite,
                intro1 = result.Intro1,
                intro2 = result.Intro2
            })?.ToList();
        }

        public List<InviteUserInfoDto> GetUserInfoByHistory(int Role, int pageIndex, int pageSize, int type, int dataType, Guid senderID, Guid dataID, Guid eID, Guid userId, int city)
        {
            var userInfos = _userRepository.GetUserInfoByHistory(Role, pageIndex, pageSize, type, dataType, senderID, dataID, eID, userId, city);
            return userInfos.Select(result => new InviteUserInfoDto()
            {
                Id = result.Id,
                Mobile = result.Mobile,
                NickName = result.NickName,
                HeadImgUrl = string.IsNullOrWhiteSpace(result.HeadImgUrl) ? "https://cos.sxkid.com/images/school_v3/head.png" : result.HeadImgUrl,
                Blockage = result.Blockage,
                Sex = result.Sex,
                Channel = result.Channel,
                City = result.City,
                NationCode = result.NationCode,
                VerifyTypes = ArrayStringToInt((result.VerifyTypes ?? "").Split(',')),
                LoginTime = result.LoginTime,
                RegTime = result.RegTime,
                isInvite = result.IsInvite,
                intro1 = result.Intro1,
                intro2 = result.Intro2
            })?.ToList();
        }

        public List<UserInfoDto> GetOrdinaryUserInfo(int pageIndex, int pageSize, Guid userId)
        {
            var userInfos = _userRepository.GetOrdinaryUserInfo(pageIndex, pageSize, userId);
            return userInfos.Select(result => ToUserInfoDto(result))?.ToList();
        }

        public List<string> CheckTodayInviteSuccessTotal(Guid userId, Guid eID, Guid dataID, int type, int dataType, DateTime todayTime)
        {
            var data = _userRepository.CheckTodayInviteSuccessTotal(userId, eID, dataID, type, dataType, todayTime);
            data = data.Select(s => s.ToHeadImgUrl()).ToList();
            return data;
        }

        public bool CheckUserisInvite(Guid userId, int type, int dataType, Guid senderID, Guid dataID, Guid eID)
        {
            return _userRepository.CheckUserisInvite(userId, type, dataType, senderID, dataID, eID);
        }

        public ThirdAuthUserDto CheckIsBinding(Guid UserId)
        {
            var model = _userRepository.CheckIsBinding(UserId);
            if (model == null)
            {
                return null;
            }
            else
            {
                return new ThirdAuthUserDto()
                {
                    Id = model.Id,
                    NickName = model.NickName,
                    Mobile = model.Mobile,
                    OpenId = model.OpneId
                };
            }
        }

        /// <summary>
        /// 根据用户Gid获取学校关注列表
        /// </summary>
        /// <param name="UserId">用户Gid</param>
        /// <returns>学校Gids</returns>
        public List<Guid> GetShlCollectionList(Guid UserId, int type)
        {
            List<Guid> gids = new List<Guid>();
            gids = _userRepository.GetShlCollectionList(UserId, type);
            return gids;
        }

        public bool IsCollection(Guid gid, int type)
        {
            return _userRepository.IsCollection(gid, type);
        }
        public Talent GetTalentDetail(Guid UserId)
        {
            return _userRepository.GetTalentDetail(UserId) ?? new Talent();
        }
        public List<Guid> GetHistoryIds(string userId, int type) => _userRepository.GetHistoryIds(userId, type);

        public bool CheckIsSubscribeFwh(Guid userId, string cityCode)
        {
            Domain.Dtos.openid_weixinDto openid = _userRepository.GetOpenId(userId, cityCode);

            return openid != null;
        }
        public Domain.Dtos.openid_weixinDto GetOpenWeixin(string openId)
        {
            return _userRepository.GetOpenWeixin(openId);
        }


        public bool TryGetOpenId(Guid userId, string cityCode, out string openId)
        {
            Domain.Dtos.openid_weixinDto openid = _userRepository.GetOpenId(userId, cityCode);
            openId = openid?.OpenId;
            return openid != null;
        }

        public UserInfoDto GetUserByWeixinOpenId(string openId)
        {
            return _userRepository.GetUserByWeixinOpenId(openId);
        }

        public string GetUserWeixinNickName(Guid userId)
        {
            return _userRepository.GetUserWeixinNickName(userId);
        }

        public IEnumerable<Talent> GetTalentDetails(IEnumerable<Guid> UserIDs)
        {
            return _userRepository.GetTalentDetails(UserIDs);
        }

        public async Task<IEnumerable<Domain.Entities.UserInfo>> GetUserInfos(IEnumerable<Guid> userIDs)
        {
            return await Task.Run(() =>
            {
                return _userRepository.GetUsers(userIDs.ToArray()); ;
            });
        }

        public async Task<bool> CheckNicknameExist(string nickname)
        {
            if (string.IsNullOrWhiteSpace(nickname)) return false;
            return await _userRepository.CheckNicknameExist(nickname);
        }

        public async Task<string> GetUserOpenID(Guid userID, string appName = "fwh")
        {
            var result = string.Empty;
            await Task.Run(() =>
            {
                var find = _userRepository.GetOpenId(userID, appName);
                if (find != null) result = find.OpenId;
            });

            return result;
        }

        public Task<IEnumerable<Domain.Entities.UserInfo>> GetRandomUsers(int count)
        {
            return _userRepository.GetRandomUsers(count);
        }

        public async Task<IEnumerable<Domain.Entities.UserInfo>> GetLastLoginUsers(DateTime startTime)
        {
            return await _userRepository.GetLastLoginUsersByStartTime(startTime);
        }

        public string GetUnionWeixinNickName(Guid userId)
        {
            return _userRepository.GetUnionWeixinNickName(userId);
        }


        public async Task<IEnumerable<UserInfoDto>> GetUserInfosByNames(IEnumerable<string> nicknames)
        {
            if (nicknames == null || !nicknames.Any()) return null;
            var finds = await _userRepository.GetUserInfosByNames(nicknames);
            return finds.Select(s => ToUserInfoDto(s));
        }

        public async Task<string> GetWXUnionId(Guid userId)
        {
            return await _userRepository.GetWXUnionId(userId);
        }

        public async Task<bool> RemoveUser(Guid userID)
        {
            if (userID == default) return false;
            return await _userRepository.DeleteUser(userID);
        }
    }
}
