using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.Entities.ViewEntities;
using ProductManagement.Infrastructure.AppService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.CommentsManage.Domain.IRepositories
{
    /// <summary>
    /// 答题详情
    /// </summary>
    public interface IQuestionsAnswersInfoRepository : IAppService<QuestionsAnswersInfo>
    {
        List<QuestionsAnswersInfo> PageAnswerByUserId(Guid UserId, bool IsSelf, int PageIndex, int PageSize, List<Guid> AnswerIds = null);
        /// <summary>
        /// 获取该管理员提交的问答
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="Total"></param>
        /// <returns></returns>
        List<QuestionsAnswersInfo> GetQuestionsAnswerByAdminId(Guid Id, int PageIndex, int PageSize, out int Total);

        bool SetNotTopByQuestionId(Guid questionId);
        bool SetTop(Guid answerId, bool isTop);
        List<QuestionsAnswersInfo> PageQuestionsAnswer(int pageIndex, int pageSize, DateTime startTime, DateTime endTime, out int total, List<Guid> schoolIds = null);
        List<QuestionsAnswersInfo> PageQuestionsAnswerByExamineState(int page, int limit, int examineState, out int total);
        List<QuestionsAnswersInfo> ListAnswersById(IEnumerable<Guid> AnswerIds);
        QuestionsAnswersInfo QueryAnswerByReplyId(Guid ReplyId);

        List<QuestionsAnswersInfo> ListAnswerByReplyIds(List<Guid> replyIds);
        List<QuestionsAnswersInfo> ListReplyByAnswerId(Guid AnswerId);
        /// <summary>
        /// 修改回答详情点赞数、回复数
        /// </summary>
        /// <param name="ReplayId"></param>
        /// <param name="operaValue"></param>
        /// <param name="Field"></param>
        /// <returns></returns>
        int UpdateAnswerLikeorReplayCount(Guid ReplayId, int operaValue, bool Field);

        int AnswerReplyTotalById(Guid answerId);
        List<QuestionsAnswersInfoExt> PageReplyByAnswerId(Guid AnswerId, int ordertype, int pageNo,int pageSize);
        List<QuestionsAnswersInfoExt> PageDialog(Guid id, List<Guid> userId, int pageIndex, int pageSize);

        /// <summary>
        /// 获取该用户提问中最新回答
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        List<QuestionsAnswersInfo> GetCurrentUserNewestAnswer(int PageIndex, int PageSize, Guid UserId);
        /// <summary>
        /// 根据规则取回复数据，1、手动置顶，2、校方回复，3、达人回复，4、点赞+回复
        /// </summary>
        /// <param name="QuestionInfoId"></param>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        List<QuestionsAnswersInfo> QuestionAnswersOrderByRole(List<Guid> QuestionInfoIds, int Take);
        /// <summary>
        /// 最新回复
        /// </summary>
        /// <param name="replyIds"></param>
        /// <returns></returns>
        List<QuestionsAnswersInfo> ReplyNewest(List<Guid> replyIds);

        //获取最顶级回复
        QuestionsAnswersInfo GetFirstParent(Guid Id);

        int QuestionAnswer(Guid userId);
        int AnswerReplyTotal(Guid userId);

        List<QuestionAnswerAndReply> CurrentPublishQuestionAnswerAndReply(Guid UserId, int PageIndex, int PageSize);
        List<QuestionAnswerAndReply> CurrentLikeQuestionAndAnswer(Guid UserId, int PageIndex, int PageSize);

        bool CheckAnswerDistinct(string content);


        /// <summary>
        /// 更新浏览量
        /// </summary>
        /// <param name="answerId"></param>
        /// <returns></returns>
        bool UpdateViewCount(Guid answerId);
        /// <summary>
        /// 获取回答回复数量
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        int GetReplyCount(Guid parentId);

        /// <summary>
        /// 获取回答的 热门回复
        /// </summary>
        /// <param name="answerReplyId"></param>
        /// <returns></returns>
        List<QuestionsAnswersInfoExt> GetAnswerHottestReplys(Guid answerReplyId);
        List<AnswerReply> GetQuestionAnswerReplyByIds(List<Guid> Ids);

        /// <summary>
        /// 根据问题ID获取顶级回答数量
        /// </summary>
        /// <param name="ids">问题ID</param>
        /// <returns></returns>
        Task<Dictionary<Guid, int>> GetAnswerCountByQuestionIDs(IEnumerable<Guid> ids);
    }
}
