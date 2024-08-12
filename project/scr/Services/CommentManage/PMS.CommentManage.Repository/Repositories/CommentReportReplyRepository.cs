using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using PMS.CommentsManage.Repository.Interface;
using ProductManagement.Framework.EntityFramework;
using ProductManagement.Infrastructure.AppService;

namespace PMS.CommentsManage.Repository.Repositories
{
    public class CommentReportReplyRepository : EntityFrameworkRepository<SchoolCommentReportReply>, ICommentReportReplyRepository
    {
        public CommentReportReplyRepository(CommentsManageDbContext repository) : base(repository)
        {
        }

        public bool AddCommentReportReply(Guid reportId, Guid adminId,string replyContent)
        {
            return Add(new SchoolCommentReportReply {
                AdminId = adminId,
                ReportId = reportId,
                ReplyContent = replyContent
            })>0;
        }

    }
}
