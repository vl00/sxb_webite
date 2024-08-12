using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.EntityExtend;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using PMS.PaidQA.Domain.Dtos;

namespace PMS.PaidQA.Application.Services
{
    public interface ITalentSettingService : IApplicationService<TalentSetting>
    {
        /// <summary>
        /// 获取专家详情
        /// </summary>
        /// <param name="talentUserID">专家UserID</param>
        /// <returns></returns>
        Task<TalentDetailExtend> GetDetail(Guid talentUserID);

        /// <summary>
        /// 当前操作用户
        /// </summary>
        /// <param name="talentUserID"></param>
        /// <param name="operatorID"></param>
        /// <returns></returns>
        Task<TalentDetailExtend> GetDetail(Guid talentUserID, Guid operatorID);



        /// <summary>
        /// 获取达人履历详细基础信息。
        /// </summary>
        /// <param name="talentUserID"></param>
        /// <returns></returns>
         Task<TalentRecordDetailDto> GetRecordDetail(Guid talentUserID);

        Task<IEnumerable<TalentDetailExtend>> GetDetails(IEnumerable<Guid> talentUserIDs);

        /// <summary>
        /// 通过学校关联查询上学问达人
        /// </summary>
        /// <param name="schoolExtId"></param>
        /// <returns></returns>
        Task<IEnumerable<TalentDetailExtend>> GetDetailsBySchool(Guid schoolExtId);

        /// <summary>
        /// 获取专家认证等级名称
        /// </summary>
        /// <param name="talentUserID">专家UserID</param>
        /// <returns></returns>
        Task<string> GetTalentLevelName(Guid talentUserID);

        Task<TalentSetting> GetByTalentUserID(Guid talentUserID);
        /// <summary>
        /// 获取所有专家等级
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<LevelType>> GetAllTalentLevels();

        /// <summary>
        /// 分页获取专家ID
        /// </summary>
        /// <param name="gradeID">学段ID</param>
        /// <param name="regionTypeID">领域ID</param>
        /// <param name="levelID">等级ID</param>
        /// <param name="orderTypeID">排序</param>
        /// <returns></returns>
        Task<IEnumerable<PageTalentIDDto>> PageTalentIDs(int pageIndex = 1, int pageSize = 10, Guid? gradeID = null, Guid? regionTypeID = null, Guid? levelID = null, int? orderTypeID = null
            , decimal minPrice = 0, decimal maxPrice = 0, string nickName = null, bool isInternal = false);

        /// <summary>
        /// 获取学段和擅长领域重叠度最高的专家
        /// </summary>
        /// <param name="talentID">原始专家ID</param>
        /// <param name="count">获取条数</param>
        /// <returns></returns>
         Task<IEnumerable<TalentDetailExtend>> GetSimilarTalents(Guid talentID, int count = 5);

        /// <summary>
        /// 获取学段和擅长领域重叠度最高的专家
        /// </summary>
        /// <param name="talentID">原始专家ID</param>
        /// <param name="count">获取条数</param>
        /// <returns></returns>
        public Task<IEnumerable<TalentDetailExtend>> PageSimilarTalents(Guid talentID, int pageIndex = 1, int pageSize = 10);
        Task<IEnumerable<(int Grade, Guid UserId)>> GradeUserIds();
    }
}