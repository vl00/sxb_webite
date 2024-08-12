using PMS.PaidQA.Domain.Entities;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using PMS.PaidQA.Domain.Dtos;

namespace PMS.PaidQA.Repository
{
    public interface ITalentSettingRepository : IRepository<TalentSetting>
    {
        Task<string> GetLevelName(Guid talentUserID);
        Task<IEnumerable<LevelType>> GetAllTalentLevels();

        Task<IEnumerable<PageTalentIDDto>> PageTalentIDs(int pageIndex = 1, int pageSize = 10, Guid? gradeID = null, Guid? regionTypeID = null, Guid? levelID = null, int orderTypeID = 0
            , decimal minPrice = 0, decimal maxPrice = 0, string nickName = null, bool isInternal = false);

        /// <summary>
        /// 擅长领域和学段重叠度高的达人ID
        /// </summary>
        /// <param name="talentID"></param>
        /// <returns>TalentUserID, Count</returns>
        Task<IEnumerable<KeyValuePair<Guid, int>>> GetSimilarTalentIDs(Guid talentID);
        /// <summary>
        /// 分页擅长领域和学段重叠度高的达人ID
        /// </summary>
        /// <returns>TalentUserID, Count</returns>
        Task<IEnumerable<KeyValuePair<Guid, int>>> PageSimilarTalentIDs(IEnumerable<Guid> regionTypeIDs, IEnumerable<Guid> gradeIDs, int pageIndex = 1, int pageSize = 10, Guid? excludeUserID = null);

        /// <summary>
        /// 获取随机达人
        /// </summary>
        /// <param name="count">数量</param>
        /// <param name="notInTalentIDs">排除的达人UserID</param>
        /// <returns></returns>
        Task<IEnumerable<TalentSetting>> GetRandomTalents(int count, IEnumerable<Guid> notInTalentIDs = null);


        Task<TalentRecordDto> GetTalentRecord(Guid userId);


        /// <summary>
        /// 年级的达人
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<(int Grade, Guid UserId)>> GradeUserIds();

        Task<IEnumerable<TalentSetting>> GetBySchool(Guid schoolExtId);

        Task<Guid?> GetSchoolId(Guid talentUserId);
    }
}
