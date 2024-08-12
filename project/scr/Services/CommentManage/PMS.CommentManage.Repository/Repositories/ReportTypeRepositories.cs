using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using PMS.CommentsManage.Repository.Interface;
using ProductManagement.Framework.EntityFramework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PMS.CommentsManage.Repository.Repositories
{
    public class ReportTypeRepositories : EntityFrameworkRepository<ReportType>, IReportTypeRepositories
    {
        public ReportTypeRepositories(CommentsManageDbContext repository) : base(repository)
        {
        }

        public List<ReportType> GetReportTypes()
        {
            return base.GetList().OrderByDescending(x=>x.Sort).ToList();
        }

    }
}
