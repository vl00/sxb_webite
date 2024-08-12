using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.Entities.ProcViewEntities;
using PMS.CommentsManage.Domain.Entities.Total;
using PMS.CommentsManage.Domain.Entities.ViewEntities;
using ProductManagement.Infrastructure.AppService;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Domain.IRepositories
{
    /// <summary>
    /// 问题详情
    /// </summary>
    public interface IQuestionInfoRepository : IAppService<QuestionInfo>
    {
        int TotalQuestion(List<Guid> SchoolId);
        List<QuestionInfo> NewestSelectedQuestion(Guid BranchSchoolId);
        /// <summary>
        /// 修改问题回答数、点赞数
        /// </summary>
        /// <param name="CommentId"></param>
        /// <param name="Field">true：修改点赞次数，false：修改点评总次数</param>
        /// <returns></returns>
        int UpdateQuestionLikeOrReplayCount(Guid QuestionId, int operaValue, bool Field);

        /// <summary>
        /// 获取当前学校下问题数量统计
        /// </summary>
        /// <param name="SchoolId"></param>
        /// <returns></returns>
        List<SchoolQuestionTotal> CurrentQuestionTotalBySchoolId(Guid SchoolId);
        /// <summary>
        /// 获取该校下的精选问题
        /// </summary>
        /// <param name="schoolSectionId"></param>
        /// <returns></returns>
        List<QuestionInfo> GetSchoolSelectedQuestion(List<Guid> schoolSectionIds, SelectedQuestionOrder Order);
        List<QuestionInfo> GetHotQuestionInfoBySchoolId(Guid schoolSectionId);
        //分部点评统计
        List<SchoolSectionCommentOrQuestionTotal> GetTotalBySchoolSectionIds(List<Guid> SchoolSectionIds);
        /// <summary>
        /// 批量获取问题
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        List<QuestionInfo> GetQuestionInfoByIds(List<Guid> Ids);

        QuestionInfo GetQuestionByNo(long no);

        /// <summary>
        /// 获取问答中（点赞+回答） 排名最高前10问题
        /// </summary>
        /// <returns></returns>
        List<Guid> GetHotSchoolSectionId();
        /// <summary>
        /// 根据用户id查询提问数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isself"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<QuestionInfo> PageQuestionByUserId(Guid userId, bool isself, int pageIndex, int pageSize);
        /// <summary>
        /// 学校问题条件筛选
        /// </summary>
        /// <param name="schoolId"></param>
        /// <param name="query"></param>
        /// <param name="Order"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<QuestionInfo> PageSchoolCommentBySchoolSectionIds(Guid schoolId, QueryQuestion query, SelectedQuestionOrder Order, int pageIndex, int pageSize, out int total);
        /// <summary>
        /// 获取该时间段写入点评的学校总数
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        int SchoolCommentQuestionCountByTime(DateTime startTime, DateTime endTime);

        List<QuestionTotal> PageQuestionTotalTime(DateTime startTime, DateTime endTime, int pageNo, int pageSize);

        List<QuestionInfo> GetQuestionData(int pageNo, int pageSize, DateTime lastTime);

        List<SchoolTotal> SchoolTotalQuestion(List<Guid> SchoolIds);
        List<SchoolTotal> SchoolSectionTotalQuestion(List<Guid> SchoolExtIds);
        /// <summary>
        /// 获取问题回答数量
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        int GetAnswerCount(Guid questionId);
        int QuestionTotal(Guid userId);

        List<HotQuestionSchool> GetHotQuestionSchools(DateTime starTime, DateTime endTime, int count = 6);

        /// <summary>
        /// 获取官网 提问列表
        /// </summary>
        /// <param name="City"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        List<QuestionInfo> QuestionList_Pc(int City, int PageIndex, int PageSize);
        int QuestionListCount_Pc(int City);

        List<QuestionInfo> GetHotQuestion(DateTime startTime, DateTime endTime);

        /// <summary>
        /// 获取热门问答
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="count">条数</param>
        /// <returns></returns>
        List<QuestionInfo> GetHotQuestion(DateTime startTime, DateTime endTime, int count);

        DateTime QueryQuestionTime(Guid Id);

        /// <summary>
        /// 更新浏览量
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        bool UpdateViewCount(Guid questionId);

        SchQuestionData GetSchoolQuestionDataByID(Guid SchoolSectionId);
        List<SchQuestionDataEx> GetSchoolQuestionDataByIDs(List<Guid> schoolSectionIds);
        /// <summary>
        /// 根据分布ID列表查询各分部的问题总数
        /// </summary>
        /// <returns></returns>
        List<SchoolTotal> GetSchoolQuestionCountBuSchoolSectionIDs(List<Guid> guids, List<int> states = null);
        List<SchoolTotal> GetAnswerCount(List<Guid> questionIds);
        List<SchoolTotal> GetQuestionAnswerCount(int pageNo, int pageSize);

        List<QuestionInfo> PageQuestionByQuestionIds(List<Guid> questionIds, bool isself);
        List<QuestionItem> GetQuestionItems(List<Guid> Ids);
        List<UserQuestionQueryDto> GetUserQuestions(IEnumerable<Guid> Ids);
        IEnumerable<Guid> GetAvailableIds(IEnumerable<Guid> Ids);
    }
}
