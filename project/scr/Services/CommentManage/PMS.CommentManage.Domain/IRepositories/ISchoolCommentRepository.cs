using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.Entities.ProcViewEntities;
using ProductManagement.Infrastructure.AppService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using PMS.CommentsManage.Domain.Entities.Total;
using PMS.CommentsManage.Domain.Entities.ViewEntities;
using System.Threading.Tasks;
using PMS.CommentsManage.Application.ModelDto;

namespace PMS.CommentsManage.Domain.IRepositories
{
    /// <summary>
    /// 点评详情
    /// </summary>
    public interface ISchoolCommentRepository : IAppService<SchoolComment>
    {
        /// <summary>
        /// 获取该管理员提交的点评
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="Total"></param>
        /// <returns></returns>
        List<SchoolComment> GetSchoolCommentByUserId(Guid Id, int PageIndex, int PageSize, out int Total);

        List<SchoolComment> PageCommentByCommentIds(List<Guid> commentIds, bool isSelf = true);
        List<SchoolComment> PageSchoolComment(int pageIndex, int pageSize, DateTime startTime, DateTime endTime, out int total, List<Guid> schoolIds = null);

        List<SchoolComment> PageSchoolCommentByExamineState(int page, int limit, int examineState, out int total);

        /// <summary>
        /// 最新点评分页查询
        /// </summary>
        /// <param name="query"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<SchoolComment> PageSchoolCommentBySchoolSectionIds(Guid schoolId, QueryCondition query, CommentListOrder commentListOrder, int pageIndex, int pageSize, out int total);

        List<SchoolComment> GetSchoolSelectedComment(List<Guid> schoolSectionId, int order);

        List<SchoolComment> PageSchoolCommentBySchoolSectionId(Guid schoolSectionId, int pageIndex, int pageSize, out int total);

        bool SetNotTopBySchoolId(Guid schoolId, Guid schoolSectionId);

        bool SetTop(Guid SchoolCommentId, bool isTop);

        int SchoolTotalComment(Guid SchoolId);

        SchoolComment SchoolTopComment(Guid SchoolId);

        /// <summary>
        /// 点评回复数+点赞数最高排该学校点评第一
        /// </summary>
        /// <param name="SchoolId"></param>
        /// <returns></returns>
        SchoolComment PraiseAndReplyTotalTop(Guid SchoolId);

        /// <summary>
        /// 分部各类点评统计
        /// </summary>
        /// <param name="SchoolId"></param>
        /// <returns></returns>
        List<SchoolCommentTotal> CurrentCommentTotalBySchoolId(Guid SchoolId);

        /// <summary>
        /// 【学校最精华】根据学校id获取点评信息
        /// </summary>
        /// <param name="schoolSectionId"></param>
        /// <returns></returns>
        SchoolComment SelectedComment(Guid schoolSectionId);

        /// <summary>
        /// 【学校最精华】根据学校id列表获取点评信息
        /// </summary>
        /// <param name="SchoolSectionIds"></param>
        /// <returns></returns>
        List<SchoolComment> GetCommentsBySchoolExtIds(List<Guid> SchoolSectionIds);

        void TranAdd(SchoolComment comment);

        List<SchoolComment> SelectedThreeComment(Guid schoolId, QueryCondition query);

        /// <summary>
        /// 修改点评回答数、点赞数
        /// </summary>
        /// <param name="CommentId"></param>
        /// <param name="Field">true：修改点赞次数，false：修改点评总次数</param>
        /// <returns></returns>
        int UpdateCommentLikeorReplayCount(Guid CommentId, int operaValue, bool Field);

        /// <summary>
        /// 获取该学校下的总点评数量
        /// </summary>
        /// <param name="SchoolId"></param>
        /// <returns></returns>
        //int CurrentSchoolCommentTotal(Guid SchoolId);
        /// <summary>
        /// 获取该校下的最精选点评
        /// </summary>
        /// <param name="SchoolId"></param>
        /// <returns></returns>
        SchoolComment GetSchoolSelectedCommentBySchoolId(Guid SchoolId);

        List<SchoolSectionCommentOrQuestionTotal> GetTotalBySchoolSectionIds(List<Guid> SchoolSectionIds);

        //学校分数统计
        SchoolScore GetSchoolScoreBySchoolId(Guid SchoolSectionId);

        /// <summary>
        /// 缓存点赞数更新
        /// </summary>
        /// <param name="CommentId"></param>
        /// <returns></returns>
        List<SchoolComment> GetCommentLikeTotal(List<Guid> CommentId);

        /// <summary>
        /// 获取点评中（点赞+回复）数最高的前10点评
        /// </summary>
        /// <returns></returns>
        List<Guid> GetHotSchoolSectionId();

        /// <summary>
        /// 根据点评id列表获取点评信息
        /// </summary>
        List<SchoolComment> GetCommentsByIds(List<Guid> CommentIds);

        /// <summary>
        /// 根据点评id获取点评信息
        /// </summary>
        SchoolComment GetCommentById(Guid commentId);

        /// <summary>
        /// 根据点评No获取点评信息
        /// </summary>
        SchoolComment GetCommentByNo(long no);

        List<SchoolComment> GetCommentData(int pageNo, int pageSize, DateTime lastTime);

        /// <summary>
        /// PC 端点评列表
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        List<SchoolComment> CommentList_Pc(int City, int PageIndex, int PageSize, out int Total);

        /// <summary>
        /// 检测是否允许注销
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool CheckLogout(Guid userId);

        /// <summary>
        /// 检测用户是否同意授权
        /// </summary>
        /// <param name="AdminId"></param>
        /// <returns></returns>
        bool UserAgreement(Guid AdminId);

        #region 喜哥

        List<SchoolComment> GetSchoolCommentBySchoolIdOrConente(Guid SchoolId, string Conent = "");

        #endregion 喜哥

        #region 用户信息api接口

        List<CommentAndReply> GetCommentAndReplies(Guid UserId, int PageIndex, int PageSize);

        int CommentTotal(Guid userId);

        #endregion 用户信息api接口

        /// <summary>
        /// 热评学校
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        List<HotCommentSchool> GetHotCommentSchools(DateTime starTime, DateTime endTime, int count = 6);

        /// <summary>
        /// 热评
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        List<HotComment> GetHotComments(DateTime date);

        /// <summary>
        /// 学校热门点评【分地区、分类型】
        /// </summary>
        /// <param name="CommentQuery"></param>
        /// <returns></returns>
        List<SchoolComment> HotComment(HotCommentQuery CommentQuery);

        /// <summary>
        /// 最热门点评【全国】
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        List<SchoolComment> HottestComment(DateTime StartTime, DateTime EndTime);

        /// <summary>
        /// 热评学校
        /// </summary>
        /// <param name="query"></param>
        /// <param name="queryAll">true：全国，false：指定类型</param>
        /// <returns></returns>
        List<HotCommentSchool> HottestSchool(HotCommentQuery query, bool queryAll);

        bool Checkisdistinct(string content);

        DateTime QueryCommentTime(Guid CommentId);

        /// <summary>
        /// 更新浏览量
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        bool UpdateViewCount(Guid commentId);

        /// <summary>
        /// 通过学部ID统计点评数据
        /// </summary>
        /// <param name="SchoolSectionId"></param>
        /// <returns></returns>
        SchCommentData GetSchoolCommentDataByID(Guid SchoolSectionId);

        List<SchoolTotal> GetCommentCountBySchool(List<Guid> schoolSectionId);
        List<SchoolTotal> GetCommentCountBySchoolSectionIDs(List<Guid> schoolSectionIDs);

        List<SchoolTotal> GetReplyCount(List<Guid> commentIds);

        List<SchoolTotal> GetCommentReplyCount(int pageNo, int pageSize);

        List<SchoolComment> GetSchoolCommentByCommentId(List<Guid> commentIds);

        /// <summary>
        /// 更改点评实体状态值
        /// </summary>
        /// <param name="commentId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        int ChangeCommentState(Guid commentId, int state);

        List<SchoolComment> PageCommentByUserId(Guid userId, int pageIndex, int pageSize, bool isSelf = true);

        List<LikeTotal> GetLikeCount(List<Guid> ids);

        IEnumerable<(DateTime, string, decimal)> GetSimpleCommentScores(Guid extID);
        List<UserCommentQueryDto> GetUserComments(IEnumerable<Guid> Ids, Guid? loginUserId);
        IEnumerable<Guid> GetAvailableIds(IEnumerable<Guid> Ids);
    }
}