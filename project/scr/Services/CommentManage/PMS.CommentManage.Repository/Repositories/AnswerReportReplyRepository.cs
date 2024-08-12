using System;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using PMS.CommentsManage.Repository.Interface;
using ProductManagement.Framework.EntityFramework;

namespace PMS.CommentsManage.Repository.Repositories
{
    public class AnswerReportReplyRepository : EntityFrameworkRepository<QuestionsAnswersReportReply>, IAnswerReportReplyRepository
    {
        public AnswerReportReplyRepository(CommentsManageDbContext repository) : base(repository)
        {
        }

        public bool AddAnswerReportReply(Guid reportId, Guid adminId, string replyContent)
        {
            return Add(new QuestionsAnswersReportReply
            {
                AdminId = adminId,
                ReportId = reportId,
                ReplyContent = replyContent
            }) > 0;
        }
    }
}
