using PMS.CommentsManage.Application.Model.Query;
using PMS.CommentsManage.Application.ModelDto;
using PMS.CommentsManage.Domain.Entities;
using ProductManagement.Infrastructure.AppService;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.IServices
{
    public interface ISchoolCommentScoreService
    {
        decimal GetAvgScoreBybaraBranchSchool(Guid SchooId);
        int AddSchoolComment(SchoolCommentScore commentScore);


        bool UpdataSchoolCommentTotal(Guid SchoolId, Guid SchoolSectionId, SchoolCommentScore commentScore, DateTime AddTime);
        List<SchoolCommentScoreDto> PageSchoolCommentScore(PageCommentScoreQuery query);
        SchoolCommentScoreDto GetSchoolCommentScore(Guid SchooId, Guid SchoolSectionId);

        SchoolScoreDto GetSchoolScoreById(Guid SchoolSectionId);
        SchoolScoreDto GetSchoolScoreBySchoolId(Guid SchooId);

        List<SchoolScoreDto> SchoolScoreOrder(List<Guid> SchoolIds);

        bool UpdateQuestionTotal(SchoolScore schoolScore);
        #region 学校分数统计
        DateTime GetLastUpdateTime();

        DateTime GetQuestionLastUpdateTime();

        List<SchoolCommentScoreDto> PageSchoolCommentScoreByTime(DateTime startTime, DateTime endTime, int pageNo, int pageSize);

        bool UpdateSchoolScore(SchoolScoreDto schoolScore);
        bool UpdateSchoolScore(QuestionTotalByTimeDto questionTotal);
        int SchoolCommentScoreCountByTime(DateTime startTime,DateTime endTime);
        
        List<SchoolScoreDto> ListNewSchoolScores(DateTime oldTime);

        List<SchoolScoreDto> ListNewQuestion(DateTime oldTime);

        #endregion
    }
}
