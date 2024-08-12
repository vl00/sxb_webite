using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using PMS.CommentsManage.Repository.Interface;
using ProductManagement.Framework.EntityFramework;
using ProductManagement.Infrastructure.AppService;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PMS.CommentsManage.Repository.Repositories
{
    /// <summary>
    /// 问题审核
    /// </summary>
    public class QuestionExamineRepository : EntityFrameworkRepository<QuestionExamine>, IQuestionExamineRepository
    {
        public QuestionExamineRepository(CommentsManageDbContext dbContext):base(dbContext)
        {
        }

        public new int Delete(Guid Id)
        {
            return base.Delete(Id);
        }

        public new IEnumerable<QuestionExamine> GetList(Expression<Func<QuestionExamine, bool>> where = null)
        {
            return base.GetList(where);
        }

        public new QuestionExamine GetModelById(Guid Id)
        {
            return base.GetAggregateById(Id);
        }

        public int Insert(QuestionExamine model)
        {
            return base.Add(model);
        }

        public bool isExists(Expression<Func<QuestionExamine, bool>> where)
        {
            return base.GetList(where) == null;
        }

        public new int Update(QuestionExamine model)
        {
            return base.Update(model);
        }
    }
}
