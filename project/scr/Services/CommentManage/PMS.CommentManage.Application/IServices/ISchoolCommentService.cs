using PMS.CommentsManage.Application.Model.Query;
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
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PMS.CommentsManage.Application.IServices
{
    /// <summary>
    /// 点评管理
    /// </summary>
    public interface ISchoolCommentService
    {
        /// <summary>
        /// 检测是否授权
        /// </summary>
        /// <param name="AdminId"></param>
        /// <returns></returns>
        bool UserAgreement(Guid AdminId);

        SchoolComment QuerySchoolCommentNo(long No);

        /// <summary>
        /// 获取该管理员的点评列表
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="Total"></param>
        /// <returns></returns>
        List<SchoolComment> GetSchoolCommentByAdminId(Guid AdminId, int PageIndex, int PageSize, out int Total);

        List<SchoolComment> QueryAllExaminers(List<Guid> Ids, int page, int limit, out int total);

        /// <summary>
        /// 分页获取学校的点评内容
        /// </summary>
        /// <returns>The school comment by school identifier.</returns>
        /// <param name="schoolId">School identifier.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Page size.</param>
        List<SchoolCommentDto> PageSchoolCommentBySchoolId(Guid schoolId, Guid UserId, int pageIndex, int pageSize, out int total, QueryCondition query = QueryCondition.All, CommentListOrder commentListOrder = CommentListOrder.None);

        List<SchoolCommentDto> PageSchoolCommentBySchoolSectionId(Guid schoolSectionId, int pageIndex, int pageSize, out int total);

        ExaminerStatistics GetExaminerStatistics();

        List<SchoolCommentDto> PageSchoolCommentByExamineState(int page, int limit, int examineState, out int total);

        List<SchoolCommentDto> PageSchoolComment(PageSchoolCommentQuery query, out int total);

        SchoolCommentDto QueryComment(Guid commentId);

        SchoolCommentDto QueryCommentByNo(long no);

        /// <summary>
        /// 根据点评Id 批量获取点评数据
        /// </summary>
        /// <param name="Ids"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        List<SchoolCommentDto> QueryCommentByIds(List<Guid> Ids, Guid UserId);

        /// <summary>
        /// 获取该用户最新点评
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        List<SchoolCommentDto> QueryNewestCommentByIds(int PageIndex, int PageSize, Guid UserId);

        List<SchoolCommentDto> GetCommentLikeTotal(List<Guid> CommentId);

        bool SetTop(Guid schoolCommentId);

        /// <summary>
        /// 根据学校id获取该学校的总点评数
        /// </summary>
        /// <param name="SchoolId"></param>
        /// <returns></returns>
        int SchoolTotalComment(Guid SchoolId);

        SchoolComment SchoolTopComment(Guid SchoolId);

        SchoolComment PraiseAndReplyTotalTop(Guid SchoolId);

        List<SchoolCommentTotal> CurrentCommentTotalBySchoolId(Guid SchoolId);

        /// <summary>
        /// 根据学校id获取最精华点评
        /// </summary>
        /// <param name="SchoolId"></param>
        /// <returns></returns>
        SchoolComment SelectedComment(Guid SchoolId);

        /// <summary>
        /// 根据学校id列表获取精华点评列表
        /// </summary>
        /// <param name="SchoolSectionIds"></param>
        /// <returns></returns>
        List<SchoolComment> GetSchoolCommentsByIds(List<Guid> SchoolSectionIds);

        void TranAdd(SchoolComment comment);

        SchoolComment QuerySchoolComment(Guid Id);

        bool AddSchoolComment(SchoolComment schoolComment);

        List<SchoolCommentDto> SelectedThreeComment(Guid schoolSectionId, Guid UserId, QueryCondition query = QueryCondition.All, CommentListOrder commentListOrder = CommentListOrder.None);

        /// <summary>
        ///
        /// </summary>
        /// <param name="CommentId"></param>
        /// <param name="Field">true：修改点赞次数，false：修改点评总次数</param>
        /// <returns></returns>
        int UpdateCommentLikeorReplayCount(Guid CommentId, int operaValue, bool Field);

        /// <summary>
        /// 学校点评列表篇
        /// </summary>
        /// <param name="UserId">当前用户id</param>
        /// <param name="grade">学校年级</param>
        /// <param name="type">学校类型</param>
        /// <param name="isLodging">是否住宿【0：住宿，1：住宿，2：无条件】</param>
        /// <param name="order">排序方式</param>
        /// <param name="schoolArea">学校区域码</param>
        /// <returns></returns>
        List<SchoolCommentDto> AllSchoolSelectedComment(Guid UserId, List<Guid> schoolBranchIds, int order);

        List<SchoolTotalDto> GetCommentCountBySchool(List<Guid> schoolSectionId);

        List<SchoolTotalDto> GetReplyCount(List<Guid> commentIds);

        /// <summary>
        /// 获取该校的最新点评信息
        /// </summary>
        /// <param name="SchoolId"></param>
        /// <param name="UserId"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        List<SchoolCommentDto> GetNewSchoolComment(Guid SchoolId, Guid UserId, int PageIndex, int PageSize);

        /// <summary>
        /// 推送学校最热门的点评数据
        /// </summary>
        /// <param name="SchoolIds"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<SchoolCommentDto> PushSchoolInfo(List<Guid> SchoolIds, Guid userId);

        //分部点评统计
        List<SchoolSectionCommentOrQuestionTotal> GetTotalBySchoolSectionIds(List<Guid> SchoolSectionIds);

        //学校分数统计
        SchoolScore GetSchoolScoreBySchoolId(Guid SchoolSectionId);

        /// <summary>
        /// 更具学校id获取最精选点评
        /// </summary>
        /// <param name="SelectionId"></param>
        /// <returns></returns>
        SchoolCommentDto GetSchoolSelectedComment(Guid SelectionId, Guid userId);

        /// <summary>
        /// 获取点评中（点赞+回复）数最高的前10学校分部
        /// </summary>
        /// <returns></returns>
        List<Guid> GetHotSchoolSectionId();

        List<SchoolCommentDto> GetCommentData(int pageNo, int pageSize, DateTime lastTime);

        List<CommentAndReply> GetCommentAndReplies(Guid UserId, int PageIndex, int PageSize);

        /// <summary>
        /// PC端 点评列表
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        List<SchoolCommentDto> CommentList_Pc(int City, int PageIndex, int PageSize, Guid UserId, out int Total);

        int CommentTotal(Guid userId);

        #region 喜哥

        /// <summary>
        /// 获取学校点评卡片
        /// </summary>
        /// <param name="Ids">【点评Ids | 学校分部Ids】</param>
        /// <param name="query">true：点评id列表 | false：学校分部Id列表</param>
        /// <param name="UserId">用户id</param>
        /// <returns></returns>
        List<SchoolCommentDto> GetSchoolCardComment(List<Guid> Ids, SchoolSectionOrIds query, Guid UserId);

        /// <summary>
        /// 根据【学校id | 点评内容】获取该学校下的点评列表
        /// </summary>
        /// <param name="SchoolId">学校分部id</param>
        /// <param name="Conent">点评内容，默认值为空</param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        List<SchoolCommentDto> GetSchoolCommentBySchoolIdOrConente(Guid SchoolId, string Conent = "");

        #endregion 喜哥

        List<HotCommentSchoolDto> GetHotCommentSchools(DateTime beginTime, DateTime endTime);

        List<HotCommentDto> GetHotComments(DateTime date);

        /// <summary>
        /// 检测是否允许注销
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool CheckLogout(Guid userId);

        /// <summary>
        /// 学校热门点评【分地区】
        /// </summary>
        /// <param name="CommentQuery"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        Task<List<SchoolCommentDto>> HotComment(HotCommentQuery CommentQuery, Guid UserId);

        /// <summary>
        /// 两天时间内最热门的点【全国】
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        List<SchoolCommentDto> HottestComment(DateTime StartTime, DateTime EndTime);

        /// <summary>
        /// 批量获取点赞数
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        List<LikeCountDto> GetLikeCount(List<Guid> ids);

        /// <summary>
        /// 热评学校
        /// </summary>
        /// <param name="query"></param>
        /// <param name="queryAll">true：全国，false：指定类型的学校</param>
        /// <returns></returns>
        Task<List<HotCommentSchoolDto>> HottestSchool(HotCommentQuery query, bool queryAll);

        /// <summary>
        /// 兼职用户点评检测
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        bool Checkisdistinct(string content);

        /// <summary>
        /// 更新浏览量
        /// </summary>
        /// <returns></returns>
        bool UpdateCommentViewCount(Guid commentId);

        /// <summary>
        /// 通过学部ID统计点评数据
        /// </summary>
        /// <param name="SchoolSectionId"></param>
        /// <returns></returns>
        SchCommentData GetCommentDataByID(Guid SchoolSectionId);

        List<SchoolCommentDto> GetSchoolCommentByCommentId(List<Guid> commentIds, Guid userId = default);


        List<SchoolTotalDto> GetCommentReplyCount(int pageNo, int pageSize);

        List<SchoolCommentDto> PageCommentByUserId(Guid userId,Guid queryUserId, int pageIndex, int pageSize, bool isSelf = true);
        List<SchoolCommentDto> PageCommentByCommentIds(Guid userId,List<Guid> commentIds, bool isSelf = true);
        IEnumerable<SchoolComment> GetList(Expression<Func<SchoolComment, bool>> where);
        List<SchoolComment> GetSchoolCommentsBySchoolUser(List<Guid> extIds, List<Guid> userIds);
        List<SchoolComment> GetCommentsByIds(List<Guid> commentIds);
        PaginationModel<SchoolCommentDto> SearchComments(SearchCommentQuery query, Guid loginUserId);
        List<SchoolCommentDto> SearchComments(IEnumerable<Guid> ids, Guid loginUserId);

        IEnumerable<SchoolCommentScoreStatisticsDto> GetSchoolCommentScoreStatistics(Guid extID);
        Task<List<UserCommentDto>> SearchUserComments(IEnumerable<Guid> ids, Guid loginUserId);
    }
}