using PMS.CommentsManage.Application.Model.Query;
using PMS.CommentsManage.Application.ModelDto;
using PMS.CommentsManage.Domain.Entities;
using ProductManagement.Infrastructure.AppService;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using PMS.UserManage.Domain.Entities;
using PMS.CommentsManage.Domain.Entities.ViewEntities;
using System.Threading.Tasks;

namespace PMS.CommentsManage.Application.IServices
{
    /// <summary>
    /// 问答详情管理
    /// </summary>
    public interface IQuestionsAnswersInfoService : IAppService<QuestionsAnswersInfo>
    {
        #region 后台方法

        /// <summary>
        /// 获取该管理员的答题详情
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="Total"></param>
        /// <returns></returns>
        List<QuestionsAnswersInfo> GetQuestionsAnswerByAdminId(Guid AdminId, int PageIndex, int PageSize, out int Total);
        List<AnswerDto> PageQuestionsAnswerByExamineState(int page, int limit, int examineState, out int total);
        List<AnswerDto> PageQuestionsAnswer(PageQuestionAnswerQuery query, out int Total);
        AnswerDto QueryQuestionAnswer(Guid answerId);
        bool SetTop(Guid answerId);
        ExaminerStatistics GetExaminerStatistics();
        #endregion


        List<AnswerInfoDto> PageAnswerByUserId(Guid UserId, Guid QueryUserId, bool IsSelf, int PageIndex, int PageSize, List<Guid> AnswerIds = null);
        /// <summary>
        /// 获取问题下的答题列表
        /// </summary>
        /// <param name="QuestionId"></param>
        /// <param name="UserId"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        List<AnswerInfoDto> GetAnswerInfoByQuestionId(Guid QuestionId, Guid UserId, int PageIndex, int PageSize);
        /// <summary>
        /// 回答最新详情
        /// </summary>
        /// <param name="QuestionId"></param>
        /// <param name="UserId"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        List<AnswerInfoDto> GetNewAnswerInfoByQuestionId(Guid QuestionId, Guid UserId, int PageIndex, int PageSize, out int total);


        AnswerInfoDto QueryAnswerInfo(Guid answerId);
        List<AnswerInfoDto> PageAnswerReply(Guid answerId, Guid userId, int ordertype, int pageIndex, int pageSize);
        int GetAnswerReplyTotal(Guid answerId);
        int UpdateAnswerLikeorReplayCount(Guid ReplayId, int operaValue, bool Field);
        List<AnswerInfoDto> PageDialog(Guid replyId, Guid userId, int pageIndex, int pageSize);

        /// <summary>
        /// 根据规则取回复数据，1、手动置顶，2、校方回复，3、达人回复，4、点赞+回复
        /// </summary>
        /// <param name="QuestionInfoId"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        List<AnswerInfoDto> QuestionAnswersOrderByRole(Guid QuestionInfoId, Guid UserId, int PageIndex, int PageSize);

        /// <summary>
        /// 根据规则取回复数据，1、手动置顶，2、校方回复，3、达人回复，4、点赞+回复
        /// </summary>
        /// <param name="questions">问题列表</param>
        /// <param name="like">点赞</param>
        /// <param name="users">用户列表</param>
        /// <param name="Take">每个问题取多少条回答</param>
        /// <returns></returns>
        List<AnswerInfoDto> QuestionAnswersOrderByQuestionIds(List<QuestionInfo> questions, Guid UserId, int Take);

        /// <summary>
        /// 最新回复
        /// </summary>
        /// <param name="replyIds"></param>
        /// <returns></returns>
        List<AnswerInfoDto> ReplyNewest(List<Guid> replyIds);

        /// <summary>
        /// 获取最顶级回复
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        AnswerInfoDto GetFirstParent(Guid Id);

        /// <summary>
        /// 批量获取问题回答\回复
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        List<AnswerInfoDto> GetAnswerInfoDtoByIds(List<Guid> Ids);
        /// <summary>
        /// 批量获取问题回答\回复
        /// </summary>
        /// <param name="Ids"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        List<AnswerInfoDto> GetAnswerInfoDtoByIds(List<Guid> Ids, Guid UserId);

        /// <summary>
        /// 根据当前用户id获取排序最新的回答
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        List<AnswerInfoDto> GetNewestAnswerInfoDtoByUserId(int PageIndex, int PageSize, Guid UserId);

        /// <summary>
        /// 获取当前用户最新提问中最新回答
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        List<AnswerInfoDto> GetCurrentUserNewestAnswer(int PageIndex, int PageSize, Guid UserId);

        #region 喜哥
        List<AnswerInfoDto> GetListDto(Expression<Func<QuestionsAnswersInfo, bool>> where = null);
        /// <summary>
        /// 根据【学校id】获取该问题下的回答列表
        /// </summary>
        /// <param name="SchoolId">学校分部id</param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        List<AnswerInfoDto> GetAnswerByQuestionId(Guid SchoolId, int PageIndex = 1, int PageSize = 20);
        #endregion

        int QuestionAnswer(Guid userId);
        int AnswerReplyTotal(Guid userId);


        List<QuestionAnswerAndReply> CurrentPublishQuestionAnswerAndReply(Guid UserId, int PageIndex, int PageSize);

        List<QuestionAnswerAndReply> CurrentLikeQuestionAndAnswer(Guid UserId, int PageIndex, int PageSize);

        bool CheckAnswerDistinct(string content);

        /// <summary>
        /// 更新浏览量
        /// </summary>
        /// <returns></returns>
        bool UpdateAnswerViewCount(Guid answerId);

        /// <summary>
        /// 回答 的热门回复
        /// </summary>
        /// <param name="answerReplyId"></param>
        /// <returns></returns>
        List<AnswerInfoDto> GetAnswerHottestReplys(Guid answerReplyId);
        List<AnswerReply> GetQuestionAnswerReplyByIds(List<Guid> Ids);
        int Add(QuestionsAnswersInfo questionsAnswersInfo);

        Task<Dictionary<Guid, int>> GetTopAnswerCountsByQuestionIDs(IEnumerable<Guid> questionIDs);
    }
}
