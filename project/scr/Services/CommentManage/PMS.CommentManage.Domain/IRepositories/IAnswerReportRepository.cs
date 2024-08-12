using System;
using System.Collections.Generic;
using PMS.CommentsManage.Domain.Entities;
using ProductManagement.Infrastructure.AppService;

namespace PMS.CommentsManage.Domain.IRepositories
{
    public interface IAnswerReportRepository
    {
        List<QuestionsAnswersReport> GetAnswerReportList(int pageIndex, int pageSize, DateTime startTime, DateTime endTime, out int total, List<Guid> schoolIds = null);
        QuestionsAnswersReport GetModelById(Guid Id);
        int Update(QuestionsAnswersReport data);
        int Add(QuestionsAnswersReport data);
    }
}
