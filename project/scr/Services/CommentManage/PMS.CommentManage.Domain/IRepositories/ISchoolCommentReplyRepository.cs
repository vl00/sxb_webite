using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.Entities.ViewEntities;
using ProductManagement.Infrastructure.AppService;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PMS.CommentsManage.Domain.IRepositories
{
    /// <summary>
    /// 点评回复
    /// </summary>
    public interface ISchoolCommentReplyRepository : IAppService<SchoolCommentReply>
    {
        SchoolCommentReply GetCommentReply(Guid replyId);
        int Add(SchoolCommentReply schoolComment);
        List<SchoolCommentReply> CommentReply(Guid CommentId, int PageIndex, int PageSize, out int total);
        List<SchoolCommentReply> SelectedCommentReply(Guid commentId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ReplayId"></param>
        /// <param name="Field">true：修改点赞次数，false：修改点评总次数</param>
        /// <returns></returns>
        int UpdateCommentReplyLikeorReplayCount(Guid ReplayId, int operaValue, bool Field);
        int SchoolReplyTotal(Guid SchoolId);
        new List<SchoolCommentReply> GetList(Expression<Func<SchoolCommentReply, bool>> where = null);
        int CommentReplyTotalById(Guid replyId);
        List<SchoolCommentReplyExt> CommentReplyById(Guid replyId,int ordertype, int pageIndex, int pageSize);
        SchoolCommentReplyExt QueryCommentReply(Guid replyId);

        /// <summary>
        /// 获取上级对话
        /// </summary>
        List<SchoolCommentReplyExt> PageDialog(Guid id, List<Guid> userId, int pageIndex, int pageSize);

        //批量获取回复中最新回复
        List<SchoolCommentReplyExt> CommentReplyExtNewest(List<Guid> ReplyIds);

        //获取该用户下最新点评回复
        List<SchoolCommentReply> GetCurretUserNewestReply(int PageIndex, int PageSize, Guid UserId);

        //获取回复的顶级父级
        SchoolCommentReply GetTopLevel(Guid Id);

        int CommentReplyTotal(Guid userId);
        int ReplyTotal(Guid userId);

        List<CommentReplyAndReply> CurrentSchoolCommenReplyAndReply(Guid userId, int PageIndex, int PageSize);

        List<CommentReplyAndReply> CurrentUserLikeCommentAndReply(Guid userId, int PageIndex, int PageSize);

        /// <summary>
        /// 更新浏览量
        /// </summary>
        /// <param name="replyId"></param>
        /// <returns></returns>
        bool UpdateViewCount(Guid replyId);

        /// <summary>
        /// 根据回复Id 获取最热门前三条回复
        /// </summary>
        /// <param name="replyId"></param>
        /// <returns></returns>
        List<SchoolCommentReplyExt> GetSchoolCommentHottestReplies(Guid replyId);
        List<ParentReply> GetParentReplyByIds(List<Guid> Ids);
    }
}
