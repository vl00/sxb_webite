using PMS.Search.Domain.QueryModel;
using PMS.UserManage.Application.ModelDto.Talent;
using PMS.UserManage.Domain.Common;
using PMS.UserManage.Domain.Dtos;
using PMS.UserManage.Domain.Entities;
using ProductManagement.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.UserManage.Application.IServices
{
    public interface ITalentService
    {
        #region 达人
        /// <summary>
        /// 获取机构或个人达人列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="identity"></param>
        /// <param name="certificationType"></param>
        /// <param name="begainDate"></param>
        /// <param name="endDate"></param>
        /// <param name="isInvite">邀请列表为1</param>
        /// <param name="type"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        List<TalentEntity> GetTalentList(string userId, string userName, string phone, int? certificationType, DateTime? begainDate, DateTime? endDate, ref int total, int isInvite = 0, int type = 0, int status = -1, int pageindex = 1, int pagesize = 10);

        /// <summary>
        /// 获取子员工达人
        /// </summary>
        /// <param name="talentId"></param>
        /// <returns></returns>
        List<TalentDetail> GetTalentChildren(string talentId);

        /// <summary>
        /// 获取达人信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TalentEntity GetTalentDetail(string id);
        TalentEntity GetTalentByUserId(string userId);


        /// <summary>
        /// 修改达人信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        CommRespon UpdateTalent(TalentEntity talent);

        /// <summary>
        /// 修改达人状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        bool ChangTalentStatus(string id, int status);

        /// <summary>
        /// 审核达人
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="reason"></param>
        /// <param name="userId"></param>
        void AuditTalent(string id, int status, string reason, string userId);

        /// <summary>
        /// 达人码认证
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user_id"></param>
        CommRespon InviteTalentByCode(string code, string user_id);

        /// <summary>
        /// 邀请/申请达人
        /// </summary>
        /// <param name="talent"></param>
        /// <returns></returns>
        Task<CommRespon> InviteTalent(TalentInput talentInput);

        /// <summary>
        /// 申请达人认证
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<CommRespon> ApplyTalent(TalentInput talentInput);
        /// <summary>
        /// 取消达人认证
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        CommRespon DisableTalent(string id);
        /// <summary>
        /// 恢复达人认证
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        CommRespon EnableTalent(string id);

        /// <summary>
        /// 删除达人证明图片
        /// </summary>
        /// <param name="imgs"></param>
        /// <returns></returns>
        CommRespon DeleteTalentImg(List<TalentImg> imgs);

        /// <summary>
        /// 新增达人证明图片
        /// </summary>
        /// <param name="imgs"></param>
        /// <returns></returns>
        CommRespon AddTalentImg(List<TalentImg> imgs);
        #endregion

        #region 机构员工
        /// <summary>
        /// 员工列表
        /// </summary>
        /// <param name="talentId"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        List<TalentStaff> GetStaffList(string userId, ref int total, int pageindex = 1, int pagesize = 10);

        /// <summary>
        /// 邀请员工
        /// </summary>
        /// <param name="talent"></param>
        /// <param name="effectivedate"></param>
        /// <returns></returns>
        CommRespon InviteStaff(string userId, TalentStaffInputDto talentStaffdto);

        /// <summary>
        /// 发送 申请达人 短信验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<CommRespon> SendCode(string phone);

        /// <summary>
        /// 验证 申请达人 短信验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="smsCode"></param>
        /// <returns></returns>
        Task<CommRespon> VerifyCode(string phone, string smsCode);

        /// <summary>
        /// 发送  机构邀请员工  短信url链接
        /// 模板  {1}正在为您申请{2}认证。
        /// XXX      正在为您申请    认证账号，点击链接https登录即可成功    认证
        /// login/login-verify.html?code=
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="name"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        CommRespon SendInviteMessage(string phone, string name, string code);
        /// <summary>
        /// 验证员工登录验证短信验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="smsCode"></param>
        /// <returns></returns>
        Task<CommRespon> VerifyStaffConfirmCode(string phone, string smsCode);
        /// <summary>
        /// 确认邀请
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="inviteCode">达人码</param>
        /// <param name="smsCode">手机验证码</param>
        /// <param name="reconfirm">手机验证码</param>
        /// <returns></returns>
        CommRespon StaffConfirmInvitation(string phone, string inviteCode, bool reconfirm);
        /// <summary>
        /// 删除员工
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DeleteTalentStaff(Guid id);

        /// <summary>
        /// 编辑员工
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool EditTalentStaff(TalentStaffInputDto talentStaffdto);
        #endregion

        #region 认证审核
        /// <summary>
        /// 检查是否符合认证条件
        /// </summary>
        /// <param name="userId"></param>
        CheckConditionOutPut CheckCondition(Guid userId);
        #endregion

        #region 关注
        /// <summary>
        /// 关注用户
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="attionUserId"></param>
        /// <returns></returns>
        CommRespon AttentionUser(Guid userId, Guid attionUserId);

        /// <summary>
        /// 取消关注用户
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="attionUserId"></param>
        CommRespon CancleAttention(Guid userId, Guid attionUserId);

        /// <summary>
        /// 获取新增的粉丝
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<Object> GetNewFans(Guid userId, ref int total, int pageindex = 1, int pagesize = 10);

        /// <summary>
        /// 获取新增粉丝数量
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        int GetNewFansCount(Guid userId);

        /// <summary>
        /// 获取ta的粉丝
        /// </summary>
        /// <param name="loginUserId">登录用户</param>
        /// <param name="searchUserId">被查询人</param>
        /// <returns></returns>
        PaginationModel<UserCollectionDto> GetFans(Guid loginUserId, Guid searchUserId, int pageindex = 1, int pagesize = 10);

        /// <summary>
        /// 获取ta的关注
        /// </summary>
        /// <param name="loginUserId">登录用户</param>
        /// <param name="searchUserId">被查询人</param>
        /// <returns></returns>
        PaginationModel<UserCollectionDto> GetAttention(Guid loginUserId, Guid searchUserId, int pageindex = 1, int pagesize = 10);

        /// <summary>
        /// 标记为已读
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool NoticeUser(List<Guid> id);
        #endregion

        #region 达人身份认证
        /// <summary>
        /// 获取机构或个人达人身份列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="identity"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        List<TalentIdentity> GetTalentPageIdentityList(Guid? id, string identity, ref int total, int type = 0, int pageindex = 1, int pagesize = 10);

        /// <summary>
        /// 获取机构或个人达人身份列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="identity"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        List<TalentIdentity> GetTalentAllIdentityList(Guid? id, string identity, int type = 0);

        /// <summary>
        /// 修改或新增达人身份
        /// </summary>
        /// <param name="talentIdentity"></param>
        /// <returns></returns>
        CommRespon EditOrAddIdentity(TalentIdentityInput talentIdentityInput);

        /// <summary>
        /// 检测是否为系统账号
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool CheckUserIsSysSxb(Guid userId);
        #endregion

        #region 个人资料
        /// <summary>
        /// 修改个人资料
        /// </summary>
        /// <param name="interestDto"></param>
        /// <returns></returns>
        CommRespon EditUserData(InterestDto interestDto);
        /// <summary>
        /// 获取个人资料
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        InterestOuPutDto GetUserData(Guid userId);

        /// <summary>
        /// 根据邀请码获取员工电话
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        CommRespon GetTalentStaffPhoneByInviteCode(string code);

        /// <summary>
        /// 发送员工登录验证短信验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        Task<CommRespon> SendStaffConfirmCode(string phone);
        TalentStaff GetTalentStaffByTalentId(string talentId);

        /// <summary>
        /// 查询讲师用户 已开课/已结束 课程数量
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        long GetLectureTotalByUserId(Guid userId, DateTime? startTime, DateTime? endTime);
        bool IsTalent(string userId);
        TalentRecommend GetRecommendTalents(TalentRecommend recommend);
        #endregion

        #region 达人榜相关
        /// <summary>
        /// 获取目前粉丝数前N名的达人
        /// </summary>
        /// <param name="count">获取条数</param>
        /// <returns></returns>
        /// <modify author="qzy" time="2020-10-15 17:14:15"></modify>
        List<TalentFollowRankDto> GetTalentFollowRank(int count = 100);

        /// <summary>
        /// 根据日期获取达人榜
        /// </summary>
        /// <param name="date">日期 -> (yyyy-MM-dd 00:00:00)</param>
        /// <returns></returns>
        TalentRankingEntity GetTalentRankingByDay(DateTime date);
        /// <summary>
        /// 获取当日达人榜
        /// </summary>
        /// <returns></returns>
        Task<TalentRankingEntity> GetTodayTalentRanking();
        /// <summary>
        /// 获取达人头像
        /// </summary>
        /// <param name="userIDs"></param>
        /// <returns></returns>
        Task<Dictionary<Guid, string>> GetTalentHeadImgUrl(IEnumerable<Guid> userIDs);

        /// <summary>
        /// 插入达人榜
        /// </summary>
        /// <returns></returns>
        bool AddTalentRanking(IEnumerable<TalentFollowRankDto> datas, DateTime date = default);

        /// <summary>
        /// es搜索达人,  如果存在loginUserId, 则在结果中置顶其关注的达人
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="loginUserId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        PaginationModel<SearchTalentDto> SearchTalents(Guid? loginUserId, SearchBaseQueryModel queryModel);
        List<SearchTalentDto> SearchTalents(IEnumerable<Guid> ids, Guid? loginUserId);
        TalentFollowDto GetTalentByUserId(Guid userId, Guid? loginUserId);
        TalentFollowDto GetTalent(Guid talentId, Guid? loginUserId);
        Task<CommRespon> MpEditUserData(MpUpdateUserDto dto);
        #endregion
    }
}
