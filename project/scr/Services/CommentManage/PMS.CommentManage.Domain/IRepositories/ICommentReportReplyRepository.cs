using System;
namespace PMS.CommentsManage.Domain.IRepositories
{
    public interface ICommentReportReplyRepository
    {
        bool AddCommentReportReply(Guid reportId, Guid adminId, string replyContent);
    }
}
