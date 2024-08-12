using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using PMS.CommentsManage.Repository.Interface;
using ProductManagement.Framework.EntityFramework;

namespace PMS.CommentsManage.Repository.Repositories
{
    public class AnswerReportRepository : EntityFrameworkRepository<QuestionsAnswersReport>, IAnswerReportRepository
    {
        public AnswerReportRepository(CommentsManageDbContext repository) : base(repository)
        {
        }

        public List<QuestionsAnswersReport> GetAnswerReportList(int pageIndex, int pageSize, DateTime startTime, DateTime endTime, out int total, List<Guid> schoolIds = null)
        {
            var result = GetList();
            if (schoolIds != null)
            {
                if (schoolIds.Count > 0)
                {
                    result = result.Where(q => schoolIds.Contains(q.QuestionInfos.SchoolId));
                }
                else
                {
                    result = result.Where(q => false);
                }
            }
            if (startTime > DateTime.MinValue && endTime < DateTime.MaxValue)
            {
                result = result.Where(p => p.ReportTime >= startTime && p.ReportTime < endTime);
            }

            total = result.Count();
            var rez = result.OrderByDescending(q=>q.ReportTime).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            if (rez != null)
            {
                return rez.ToList();
            }
            return null;
        }

        public QuestionsAnswersReport GetModelById(Guid Id)
        {
            return base.GetAggregateById(Id);
        }

        public override int Add(QuestionsAnswersReport Entity)
        {
            return base.Add(Entity);
        }

    }
}
