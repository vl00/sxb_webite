using System;
using System.Collections.Generic;
using PMS.CommentsManage.Application.Model.Query;
using PMS.CommentsManage.Application.ModelDto;

namespace PMS.CommentsManage.Application.IServices
{
    public interface ICommentReportService
    {
        bool AddReport(CommentReportDto report);

        List<CommentReportDto> PageCommentReport(PageCommentReportQuery query,out int total);

        CommentReportDto GetCommentReport(CommentReportQuery query);

        bool ReplyCommentReport(Guid commentReportId, Guid adminId, string replyContent);

    }
}
