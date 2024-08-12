using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using ProductManagement.Framework.EntityFramework;
using ProductManagement.Infrastructure.AppService;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq;

namespace PMS.CommentsManage.Repository.Repositories
{
    public class SchoolScoreRepository : EntityFrameworkRepository<SchoolScore>, ISchoolScoreRepository
    {
        public SchoolScoreRepository(CommentsManageDbContext dbContext):base(dbContext) 
        {
        
        }

        public SchoolScore GetModelById(Guid Id)
        {
            return base.GetAggregateById(Id);
        }

        public int Insert(SchoolScore model)
        {
            return base.Add(model);
        }

        public bool isExists(Expression<Func<SchoolScore, bool>> where)
        {
            return base.GetList(where).FirstOrDefault() != null;
        }

        IEnumerable<SchoolScore> IqAppService<SchoolScore>.GetList(Expression<Func<SchoolScore, bool>> where)
        {
            return base.GetList(where);
        }
    }
}
