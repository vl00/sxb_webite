using PMS.CommentsManage.Application.ModelDto.Reply;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.Entities.ViewEntities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PMS.CommentsManage.Application.IServices
{
    public interface ISchoolCommentReplyService
    {
        SchoolCommentReply GetCommentReply(Guid replyId);
        int Add(SchoolCommentReply schoolComment, out Guid? replyId);

        /// <summary>
        /// 根据点评ID分页获取回复
        /// </summary>
        List<SchoolCommentReply> PageCommentReply(Guid CommentId, int PageIndex, int PageSize,out int total);

        /// <summary>
        /// 根据回复ID分页获取下级回复
        /// </summary>
        List<SchoolCommentReplyExt> PageCommentReplyById(Guid replyId,int ordertype, int PageIndex, int PageSize, out int total);
        List<SchoolCommentReplyExt> PageDialog(Guid replyId, int PageIndex, int PageSize);

        List<ReplyDto> GetCommentReplyByIds(List<Guid> Ids, Guid UserId);


        List<ReplyDto> CommentReply(Guid CommentId, Guid UserId,int PageIndex, int PageSize, out int total);
        List<SchoolCommentReply> SelectedCommentReply(Guid commentId);
        List<ReplyDto> SelectedCommentReply(Guid commentId, Guid UserId);

        List<SchoolCommentReplyExt> CommentReplyExtNewest(List<Guid> ReplyIds);

        List<ReplyDto> GetNewestCommentReplyByUserId(int PageIndex, int PageSize, Guid UserId);

        /// <summary>
        /// 获取当前用户下点评最新回复
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        List<ReplyDto> GetCurretUserNewestReply(int PageIndex, int PageSize, Guid UserId);

        //获取回复的顶级父级
        ReplyDto GetTopLevel(Guid Id);

        int CommentReplyTotal(Guid userId);
        int ReplyTotal(Guid userId);

        List<CommentReplyAndReply> CurrentSchoolCommenReplyAndReply(Guid userId, int PageIndex, int PageSize);

        List<CommentReplyAndReply> CurrentUserLikeCommentAndReply(Guid userId, int PageIndex, int PageSize);

        /// <summary>
        /// 更新浏览量
        /// </summary>
        /// <returns></returns>
        bool UpdateReplyViewCount(Guid replyId);

        /// <summary>
        /// 获取回复详情中的 热门回复
        /// </summary>
        /// <param name="replyId"></param>
        /// <returns></returns>
        List<SchoolCommentReplyExt> GetSchoolCommentHottestReplies(Guid replyId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        List<ParentReply> GetParentReplyByIds(List<Guid> Ids);
        IEnumerable<SchoolCommentReply> GetList(Expression<Func<SchoolCommentReply, bool>> where);
    }
}
