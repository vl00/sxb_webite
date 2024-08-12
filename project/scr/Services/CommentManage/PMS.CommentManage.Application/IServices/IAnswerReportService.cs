using System;
using System.Collections.Generic;
using PMS.CommentsManage.Application.Model.Query;
using PMS.CommentsManage.Application.ModelDto;

namespace PMS.CommentsManage.Application.IServices
{
    public interface IAnswerReportService
    {
        bool AddReport(QuestionReportDto report);

        List<QuestionReportDto> PageAnswerReport(PageAnswerReportQuery query, out int total);
        QuestionReportDto GetAnswerReport(AnswerReportQuery query);

        bool ReplyAnswerReport(Guid answerReportId, Guid adminId, string replyContent);
    }
}
