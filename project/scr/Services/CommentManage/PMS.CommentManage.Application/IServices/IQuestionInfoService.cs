using PMS.CommentsManage.Application.ModelDto;
using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.Entities.ProcViewEntities;
using PMS.CommentsManage.Domain.Entities.Total;
using PMS.CommentsManage.Domain.Entities.ViewEntities;
using PMS.Search.Application.ModelDto.Query;
using ProductManagement.Infrastructure.AppService;
using ProductManagement.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.CommentsManage.Application.IServices
{
    /// <summary>
    /// 问题管理服务接口
    /// </summary>
    public interface IQuestionInfoService
    {
        QuestionDto CovertQuestionToDto(QuestionInfo questionInfo, Guid UserId, List<GiveLike> Likes, List<SchoolImage> Images, List<AnswerInfoDto> answers = null);
        List<QuestionDto> PageQuestionByQuestionIds(Guid QueryUserId, List<Guid> questionIds, bool IsSelf);
        List<QuestionDto> PageQuestionByUserId(Guid UserId, Guid QueryUserId, bool IsSelf, int PageIndex, int PageSize);
        QuestionDto GetQuestionById(Guid Id, Guid UserId);
        List<QuestionDto> GetQuestionByIds(List<Guid> Ids, Guid UserId);
        int TotalQuestion(Guid SchoolId);
        bool AddQuestion(QuestionInfo question);
        /// <summary>
        /// 获取该学校分部下的精选问题列表
        /// </summary>
        /// <param name="SchoolId"></param>
        /// <param name="UserId"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        List<QuestionDto> GetHotQuestionInfoBySchoolId(Guid SchoolId, Guid UserId, int PageIndex, int PageSize, SelectedQuestionOrder commentListOrder);
        /// <summary>
        /// 获取最新问题列表
        /// </summary>
        /// <param name="SchoolId"></param>
        /// <param name="UserId"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        List<QuestionDto> GetNewQuestionInfoBySchoolId(Guid SchoolId, Guid UserId, int PageIndex, int PageSize, QueryQuestion query, SelectedQuestionOrder Order, out int total);
        /// <summary>
        /// 获取该学校下的所有条件查询
        /// </summary>
        /// <returns></returns>
        Task<List<QuestionDto>> AllSchoolSelectedQuestion(Guid UserId, List<Guid> schoolBranchIds, SelectedQuestionOrder Order);
        /// <summary>
        /// 问题详情
        /// </summary>
        /// <param name="No"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        Task<QuestionDto> QuestionDetailByNo(long No, Guid UserId);
        /// <summary>
        /// 问题详情
        /// </summary>
        /// <param name="questionId"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        QuestionDto QuestionDetail(Guid questionId, Guid UserId);
        //修改问题回答/点赞总次数
        int UpdateQuestionLikeorReplayCount(Guid QuestionId, int operaValue, bool Field);
        List<SchoolQuestionTotal> CurrentQuestionTotalBySchoolId(Guid SchoolId);
        /// <summary>
        /// 问题列表
        /// </summary>
        /// <param name="SchoolIds"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<QuestionDto> PushSchoolInfo(List<Guid> SchoolIds, Guid userId);

        /// <summary>
        /// 获取该用户最新提问
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        List<QuestionDto> GetNewestQuestion(int PageIndex, int PageSize, Guid UserID);

        //分部点评统计
        List<SchoolSectionCommentOrQuestionTotal> GetTotalBySchoolSectionIds(List<Guid> SchoolSectionIds);
        /// <summary>
        /// 统计该时间段有提问的学校
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        int SchoolCommentQuestionCountByTime(DateTime startTime, DateTime endTime);

        List<SchoolTotalDto> SchoolTotalQuestion(List<Guid> SchoolIds);
        List<SchoolTotalDto> SchoolSectionTotalQuestion(List<Guid> SchoolExtIds);

        List<QuestionTotalByTimeDto> PageQuestionTotalTime(DateTime startTime, DateTime endTime, int pageNo, int pageSize);

        List<Guid> GetHotSchoolSectionId();

        List<QuestionDto> GetQuestionData(int pageNo, int pageSize, DateTime lastTime);
        #region 喜哥
        /// <summary>
        /// 根据【学校id | 点评内容】获取该学校下的问题列表
        /// </summary>
        /// <param name="SchoolId">学校分部id</param>
        /// <param name="Conent">点评内容，默认值为空</param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        List<QuestionDto> GetQuestionAnswerBySchoolIdOrConente(Guid SchoolId, string Conent, int PageIndex, int PageSize, out int Total);

        /// <summary>
        /// 根据问题id获取问题详情
        /// </summary>
        /// <param name="QuestionId"></param>
        /// <returns></returns>
        (QuestionDto, int) GetQuestionInfoById(Guid QuestionId);
        #endregion

        int QuestionTotal(Guid userId);

        List<HotQuestionSchoolDto> GetHotQuestionSchools(DateTime starTime, DateTime endTime, int count = 6);

        List<QuestionDto> QuestionList_Pc(Guid UserId, int City, int PageIndex, int PageSize, out int Total);

        Task<List<HotQuestionSchoolDto>> HottestSchool();

        List<QuestionDto> GetHotQuestion(DateTime startTime, DateTime endTime);
        /// <summary>
        /// 获取热门问答
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="count">获取条数</param>
        /// <returns></returns>
        List<QuestionDto> GetHotQuestion(DateTime startTime, DateTime endTime, int count = 10);
        Task<List<QuestionDto>> GetHotQuestion();
        List<SchoolTotalDto> GetAnswerCount(List<Guid> questionIds);
        List<SchoolTotalDto> GetQuestionAnswerCount(int pageNo, int pageSize);
        /// <summary>
        /// 通过学部ID统计问答数据
        /// </summary>
        /// <param name="SchoolSectionId"></param>
        /// <returns></returns>
        SchQuestionData GetQuestionDataByID(Guid SchoolSectionId);

        /// <summary>
        /// 更新浏览量
        /// </summary>
        /// <returns></returns>
        bool UpdateQuestionViewCount(Guid questionId);

        List<QuestionItem> GetQuestionItems(List<Guid> Ids);
        List<QuestionInfo> GetQuestionsBySchoolUser(List<Guid> extIds, List<Guid> userIds);
        List<QuestionInfo> GetQuestionInfoByIds(List<Guid> Ids);
        PaginationModel<QuestionDto> SearchQuestions(SearchQuestionQuery query, Guid loginUserId);
        List<QuestionDto> SearchQuestions(IEnumerable<Guid> ids, Guid loginUserId);
        Task<List<UserQuestionDto>> SearchUserQuestions(IEnumerable<Guid> ids, Guid loginUserId);
    }
}
