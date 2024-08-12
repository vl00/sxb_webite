using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Dtos;
using PMS.UserManage.Domain.Entities;

namespace PMS.UserManage.Domain.IRepositories
{
    public interface IUserRepository
    {
        List<OpenIdWeixin> ListUserInfoOpenId(List<Guid> userIds);
        UserInfo GetUserInfoDetail(Guid userId);
        UserInfo GetUserInfo(Guid userId);
        IEnumerable<UserInfo> GetUserInfos(string phone);
        bool AddUserInfo(UserInfo user);
        bool UpdateUserInfo(UserInfo user);
        bool UpdateUserInfo(Guid userid, string nickName, string mobile, string headImgUrl, int? sex);
        List<UserInfo> ListUserInfo(List<Guid> userIds);
        List<UserInfo> ListAllUserInfo();

        //根据批量学校id历史记录表数据
        List<UserInvite> GetUserInfoByHistory(List<Guid> SchoolIds, UserRole Role, int PageIndex, int PageSize, int type, int dataType, Guid senderID, Guid dataID, Guid eID, Guid userId, int city);
        //暂无相同学校类型历史记录
        List<UserInvite> GetUserInfoByHistory(int Role, int pageIndex, int pageSize, int type, int dataType, Guid senderID, Guid dataID, Guid eID, Guid userId, int city);
        //获取普通用户
        List<UserInfo> GetOrdinaryUserInfo(int pageIndex, int pageSize, Guid userId);
        //检测该用户今天是否对同一数据源邀请10个
        List<string> CheckTodayInviteSuccessTotal(Guid userId, Guid eID, Guid dataID, int type, int dataType, DateTime todayTime);
        bool CheckUserisInvite(Guid userId, int type, int dataType, Guid senderID, Guid dataID, Guid eID);
        //检测是否绑定微信openid
        ThirdAuthUser CheckIsBinding(Guid UserId);
        /// <summary>
        /// 根据用户Gid获取学校关注列表
        /// </summary>
        /// <param name="UserId">用户Gid</param>
        /// <param name="dataType">0：文章1：学校学部2：问答3：点评</param>
        /// <returns>学校Gids</returns>
        List<Guid> GetShlCollectionList(Guid UserId, int dataType);

        bool IsCollection(Guid dataID, int dataType);

        Talent GetTalentDetail(Guid UserId);
        IEnumerable<Talent> GetTalentDetails(IEnumerable<Guid> UserIDs);

        List<Guid> GetHistoryIds(string userId, int dataType);
        IEnumerable<UserInfo> GetUsers(Guid[] ids);


        /// <summary>
        /// 获取用户的openId
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        openid_weixinDto GetOpenId(Guid userId, string type = "fwh");

        UserInfo GetUserByWeixinOpenId(string openId);

        string GetUserWeixinNickName(Guid userId);

        UserInfo GetUserByBaiduOpenId(string openId);
        OpenidBaidu GetBaiduOpenId(Guid userId);


        /// <summary>
        /// 获取随机用户
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>

        Task<IEnumerable<UserInfo>> GetRandomUsers(int count);
        Task<bool> CheckNicknameExist(string nickname);
        IEnumerable<UserInfo> GetUserInfosByName(string name);
        Task<IEnumerable<UserInfo>> GetUserInfosByNames(IEnumerable<string> nicknames);
        UserInfo GetUserInfo(string phone);
        IEnumerable<Guid> GetSamplePhoneUserIds(Guid userId);

        Task<IEnumerable<UserInfo>> GetLastLoginUsersByStartTime(DateTime startTime);

        /// <summary>
        /// 获取微信昵称
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        string GetUnionWeixinNickName(Guid userId);
        bool SetDeviceToken(DeviceToken model);
        bool UpdateUserInfo(Guid userid, string nickName, string headImgUrl);



        /// <summary>
        /// 获取UnionID
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        Task<string> GetWXUnionId(Guid userId);
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        Task<bool> DeleteUser(Guid userID);
        openid_weixinDto GetOpenWeixin(string openId);
    }
}
