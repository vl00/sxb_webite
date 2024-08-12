using PMS.UserManage.Application.ModelDto;
using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.UserManage.Application.IServices
{
    public interface IUserService
    {
        List<UserInfoOpenIdDto> ListUserOpenId(List<Guid> userIds);
        /// <summary>
        /// 获取随机用户
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>

        Task<IEnumerable<UserInfo>> GetRandomUsers(int count);
        Domain.Entities.UserInfo GetUserInfoDetail(Guid userId);
        bool SyncUserInfo(Guid UserId, string userName, string phone, string headImg);
        bool InsertUserInfo(Guid UserId, string userName, string phone, string headImg);
        UserInfoDto GetUserInfo(Guid userId);
        Task<IEnumerable<UserInfo>> GetUserInfos(IEnumerable<Guid> userIDs);
        UserInfoDto GetUserInfo(string phone);
        IEnumerable<UserInfoDto> GetUserInfosByPhone(string phone);
        List<UserInfoDto> ListUserInfo(List<Guid> userIds);
        List<UserInfoDto> ListAllUserInfo();

        //根据批量学校id历史记录表数据
        List<InviteUserInfoDto> GetUserInfoByHistory(List<Guid> SchoolIds, UserRole Role, int PageIndex, int PageSize, int type, int dataType, Guid senderID, Guid dataID, Guid eID, Guid userId, int city);
        //暂无相同学校类型历史记录
        List<InviteUserInfoDto> GetUserInfoByHistory(int Role, int pageIndex, int pageSize, int type, int dataType, Guid senderID, Guid dataID, Guid eID, Guid userId, int city);
        //获取普通用户
        List<UserInfoDto> GetOrdinaryUserInfo(int pageIndex, int pageSize, Guid userId);
        List<string> CheckTodayInviteSuccessTotal(Guid userId, Guid eID, Guid dataID, int type, int dataType, DateTime todayTime);
        //检测该用户对该数据源是否已经邀请过
        bool CheckUserisInvite(Guid userId, int type, int dataType, Guid senderID, Guid dataID, Guid eID);
        ThirdAuthUserDto CheckIsBinding(Guid UserId);
        /// <summary>
        /// 根据用户Gid获取学校关注列表
        /// </summary>
        /// <param name="UserId">用户Gid</param>
        /// <returns>学校Gids</returns>
        List<Guid> GetShlCollectionList(Guid UserId, int type);

        /// <summary>
        /// 查询是否被关注
        /// </summary>
        /// <param name="gid">文章、学校学部、问答、点评GUID</param>
        /// <param name="type">0：文章、1:学校学部、2:问答、3:点评</param>
        /// <returns></returns>
        bool IsCollection(Guid gid, int type);

        Talent GetTalentDetail(Guid UserId);
        IEnumerable<Talent> GetTalentDetails(IEnumerable<Guid> UserIds);

        /// <summary>
        /// 获取历史记录
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="type">0：文章、1:学校学部、2:问答、3:点评</param>
        /// <returns></returns>
        List<Guid> GetHistoryIds(string userId, int type);


        /// <summary>
        /// 判断是否关注了服务号
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool CheckIsSubscribeFwh(Guid userId, string cityCode);

        /// <summary>
        /// 获取用户的openId
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cityCode"></param>
        /// <returns></returns>
        bool TryGetOpenId(Guid userId, string cityCode, out string openId);

        Task<string> GetUserOpenID(Guid userID, string appName = "fwh");

        /// <summary>
        /// 通过openId获取上学帮用户信息
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        UserInfoDto GetUserByWeixinOpenId(string openId);

        /// <summary>
        /// 获取用户绑定的微信昵称
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        string GetUserWeixinNickName(Guid userId);

        /// <summary>
        /// 检查昵称是否存在
        /// </summary>
        /// <returns></returns>
        Task<bool> CheckNicknameExist(string nickname);
        IEnumerable<UserInfoDto> GetUserInfosByName(string name);
        Task<IEnumerable<UserInfoDto>> GetUserInfosByNames(IEnumerable<string> nicknames);
        IEnumerable<Guid> GetSamplePhoneUserIds(Guid userId);

        /// <summary>
        /// 获取最后登陆用户(们)
        /// </summary>
        /// <param name="endTime"></param>
        /// <returns></returns>
        Task<IEnumerable<UserInfo>> GetLastLoginUsers(DateTime startTime);
        string GetUnionWeixinNickName(Guid userId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="phone">传电话情况会根据电话匹配到unionID，尽管这个unionID不属于该账号。</param>
        /// <returns></returns>
        Task<string> GetWXUnionId(Guid userId);
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userID">userInfo.ID</param>
        /// <returns></returns>
        Task<bool> RemoveUser(Guid userID);
        Domain.Dtos.openid_weixinDto GetOpenWeixin(string openId);
    }
}
