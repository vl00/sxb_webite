using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PMS.CommentsManage.Domain.Common;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using PMS.CommentsManage.Repository.Interface;
using ProductManagement.Framework.EntityFramework;
using ProductManagement.Infrastructure.AppService;

namespace PMS.CommentsManage.Repository.Repositories
{
    public class CommentReportRepository: EntityFrameworkRepository<SchoolCommentReport>, ICommentReportRepository
    {
        public CommentReportRepository(CommentsManageDbContext repository) : base(repository)
        {
        }

        public new int Delete(Guid Id)
        {
           return base.Delete(Id);
        }

        public new IEnumerable<SchoolCommentReport> GetList(Expression<Func<SchoolCommentReport, bool>> where = null)
        {
            return base.GetList(where);
        }

        public  SchoolCommentReport GetModelById(Guid Id)
        {
            return base.GetAggregateById(Id);
        }

        public  int Insert(SchoolCommentReport model)
        {
            return base.Add(model);
        }

        public  bool isExists(Expression<Func<SchoolCommentReport, bool>> where)
        {
            return base.GetList(where) == null;
        }


        public new int Update(SchoolCommentReport model)
        {
            return base.Update(model);
        }



        public List<SchoolCommentReport> PageCommentReport(int pageIndex, int pageSize, DateTime startTime, DateTime endTime, out int total, List<Guid> schoolIds = null)
        {
            var result = base.GetList();
            if (schoolIds != null)
            {
                if (schoolIds.Count > 0)
                {
                    result = result.Where(q => schoolIds.Contains(q.SchoolComments.SchoolId));
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

    }
}
