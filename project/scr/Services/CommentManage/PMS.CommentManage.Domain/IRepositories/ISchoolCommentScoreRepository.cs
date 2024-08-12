using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.Entities.ProcViewEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.IRepositories
{
    public interface ISchoolCommentScoreRepository
    {
        decimal GetAvgScoreBybaraBranchSchool(Guid SchooId);
        int AddSchoolComment(SchoolCommentScore commentScore);
        List<SchoolCommentScoreTotal> PageSchoolCommentScore(List<Guid> schoolIds);
        SchoolCommentScoreTotal GetSchoolCommentScore(Guid SchooId, Guid SchoolSectionId);
        List<SchoolScore> SchoolScoreOrder(List<Guid> SchoolIds);
        /// <summary>
        /// 根据学校id，批量得到学校平均分
        /// </summary>
        /// <param name="SchoolId"></param>
        /// <returns></returns>
        List<SchoolScore> GetSchoolScoreBySchool(List<Guid> SchoolId);
        SchoolScore GetSchoolScoreById(Guid SchoolSectionId);
        SchoolScore GetSchoolScoreBySchoolId(Guid SchoolId);
        /// <summary>
        /// 根据点评id集合获取点评分数集合
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        List<SchoolCommentScore> GetSchoolScoreByCommentIds(List<Guid> Ids);

        bool UpdateSchoolQuestionTotal(SchoolScore schoolScore);
        List<SchoolScore> ListNewQuestion(DateTime oldTime);

        bool UpdateQuestionTotal(SchoolScore schoolScore);

        SchoolScore GetSchoolScore(Guid SchoolSectionId, Guid SchoolId);

        bool UpdataQuestionTotalNewTime(Guid SchoolSectionId, DateTime AddTime);

        #region 学校分数统计
        DateTime GetLastUpdateTime();
        DateTime GetQuestionLastUpdateTime();
        List<SchoolCommentScore> PageSchoolCommentScoreByTime(DateTime startTime, DateTime endTime, int pageNo, int pageSize);
        bool AddSchoolScore(SchoolScore schoolScore);
        bool UpdateSchoolScore(SchoolScore schoolScore);
        bool IsExistSchoolScore(Guid schoolSectionId);
        int SchoolCommentScoreCountByTime(DateTime startTime, DateTime endTime);
        
        List<SchoolScore> ListNewSchoolScores(DateTime oldTime);
        #endregion
    }
}
