using System;
namespace PMS.CommentsManage.Domain.IRepositories
{
    public interface IAnswerReportReplyRepository
    {
        bool AddAnswerReportReply(Guid reportId, Guid adminId, string replyContent);
    }
}
