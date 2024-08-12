using PMS.UserManage.Domain.Dtos;
using PMS.UserManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace PMS.UserManage.Domain.IRepositories
{
    public interface ITalentRepository
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

        IEnumerable<TalentEntity> GetTalentsByUser(Guid[] userIds);

        /// <summary>
        /// 获取达人信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TalentEntity GetTalentDetail(string id);

        /// <summary>
        /// 修改达人信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        CommRespon UpdateTalent(TalentEntity talent);


        /// <summary>
        /// 更新讲师信息
        /// </summary>
        /// <param name="lector"></param>
        /// <param name="fields"></param>
        /// <param name="_dbtransaction"></param>
        /// <returns></returns>
        bool UpdateLector(Lector lector, string[] fields, IDbTransaction _dbtransaction);

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
        /// <param name="talent"></param>
        /// <param name="talentAudit"></param>
        /// <returns></returns>
        CommRespon InviteTalentByCode(TalentEntity talent, TalentAudit talentAudit, TalentEntity oldTalent, Lector lector, bool isNewLector);

        /// <summary>
        /// 邀请/申请达人
        /// </summary>
        /// <param name="talentInput"></param>
        /// <param name="code">邀请达人码/param>
        /// <returns></returns>
        [Obsolete]
        Task<CommRespon> InviteTalent(TalentInput talentInput, string code);

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
        /// <param name="talentStaff"></param>
        /// <param name="talentInvite"></param>
        /// <returns></returns>
        CommRespon InviteStaff(TalentStaff talentStaff, TalentInvite talentInvite);

        /// <summary>
        /// 确认邀请
        /// </summary>
        /// <param name="talent"></param>
        /// <param name="staffId"></param>
        /// <param name="isNewTalent"></param>
        /// <returns></returns>
        CommRespon StaffConfirmInvitation(string staffId, TalentEntity talent, Lector lector, bool isNewTalent, bool isNewLector);
        /// <summary>
        /// 删除员工
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DeleteTalentStaff(Guid id, string userId);

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
        CommRespon AttentionUser(Guid userId, Guid attionUserId, bool isTalent);

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
        /// <param name="userId"></param>
        /// <returns></returns>
        List<Object> GetFans(Guid userId, ref int total, int pageindex = 1, int pagesize = 10);

        /// <summary>
        /// 获取ta的关注
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<Object> GetAttention(Guid userId, ref int total, int pageindex = 1, int pagesize = 10);

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
        /// 检测用户是否为上学帮系统账号
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
        bool EditUserData(InterestDto interestDto);
        /// <summary>
        /// 获取个人资料
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        InterestOuPutDto GetUserData(Guid userId);
        /// <summary>
        /// 根据电话获取员工
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        TalentStaff GetTalentStaffByPhone(string phone, int status = 1);
        /// <summary>
        /// 根据用户ID获取达人
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isdelete"></param>
        /// <returns></returns>
        TalentEntity GetTalentByUserId(string userId, int isdelete = 0);
        /// <summary>
        /// 查询达人邀请码是否被使用过
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        bool CheckTalentCode(string code);
        /// <summary>
        /// 查询员工邀请码是否被使用过
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        bool CheckTalentStaffCode(string phone, string code);
        Task<string> RandVerificationCode(int n, Random random, string phone);

        /// <summary>
        /// 获取员工邀请
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        TalentInvite GetTalentInviteByParentId(string parentId);
        TalentStaff GetTalentStaffByParentIdPhone(string parentId, string phone, int isdelete = 0);
        TalentInvite GetTalentInviteByCode(string code);
        TalentStaff GetTalentStaffById(string id, int isdelete = 0);
        List<TalentStaff> GetTalentStaffsByParentId(string parentId, int isdelete = 0);
        Lector GetLectorByUserId(string userId);

        /// <summary>
        /// 查询讲师用户 已开课/已结束 课程数量
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        long GetLectureTotalByUserId(Guid userId, DateTime? startTime, DateTime? endTime);

        TalentEntity GetTalent(string id);
        CommRespon DisableTalent(string id, string lectorId, string staffId = null);
        TalentStaff GetTalentStaffByTalentId(string talentId, int isdelete = 0);
        CommRespon EnableTalent(string id, string lectorId, string staffId = null);
        /// <summary>
        /// 根据达人获取其所有的员工达人
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        List<TalentDetail> GetTalentsByParentId(string parentId);
        bool UpdateTalent(TalentEntity talent, IDbTransaction _dbtransaction);
        CommRespon ApplyTalent(TalentEntity talent, Lector lector, bool isNewTalent, bool isNewLector);
        TalentIdentity GetTalentIdentityByNameType(string identityName, int? type = 0);
        bool NoticeAllUser(Guid userId);
        int DeleteTalentStaff(Guid id);
        int DeleteTalent(Guid id);
        int DeleteLector(Guid userId);
        bool IsTalent(Guid userId);
        List<RecommendUserDto> GetTalentsByCity(int cityCode, bool isInCity, Guid[] excludeUserIds, long offset, int size);
        long GetTalentsByCityTotal(int cityCode, bool isInCity, Guid[] excludeUserIds);
        #endregion

        #region 达人榜相关
        /// <summary>
        /// 获取粉丝数前 N 的达人列表
        /// </summary>
        /// <param name="count">N</param>
        /// <returns></returns>
        IEnumerable<TalentFollowRankDto> GetTalentFollowRank(int count = 100);

        TalentRankingEntity GetRankingEntityByDate(DateTime date);

        bool AddTelentRanking(TalentRankingEntity entity);
        IEnumerable<TalentAbout> GetTalents(Guid[] ids);
        Task<IEnumerable<TalentUser>> GetTalentUsers(IEnumerable<Guid> userIds);
        #endregion
    }
}
