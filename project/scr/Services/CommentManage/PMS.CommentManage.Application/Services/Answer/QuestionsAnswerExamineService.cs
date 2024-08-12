using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using ProductManagement.Infrastructure.AppService;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PMS.CommentsManage.Application.Services.Answer
{
    public class QuestionsAnswerExamineService : AppService<QuestionsAnswerExamine>,IQuestionsAnswerExamineService
    {
        private readonly IQuestionsAnswerExamineRepository _repo;
        public QuestionsAnswerExamineService(IQuestionsAnswerExamineRepository repo)
        {
            _repo = repo;
        }

        public override int Delete(Guid Id)
        {
            return _repo.Delete(Id);
        }

        public bool ExecExaminer(Guid PartTimeJobAdminId, Guid TargetId, int Status, bool IsPartTimeJob = true)
        {
            return _repo.ExecExaminer(PartTimeJobAdminId, TargetId, Status,  IsPartTimeJob);
        }

        public override IEnumerable<QuestionsAnswerExamine> GetList(Expression<Func<QuestionsAnswerExamine, bool>> where = null)
        {
            return _repo.GetList(where);
        }

        public override QuestionsAnswerExamine GetModelById(Guid Id)
        {
            return _repo.GetModelById(Id);
        }

        public List<QuestionsAnswerExamine> GetAnswerInfoByAdminId(Guid AdminId, int PageIndex, int PageSize,List<Guid> Ids, out int Total)
        {
            return _repo.GetAnswerInfoByAdminId(AdminId, PageIndex, PageSize,Ids, out Total);
        }

        public override int Insert(QuestionsAnswerExamine model)
        {
            return _repo.Insert(model);
        }

        public override bool isExists(Expression<Func<QuestionsAnswerExamine, bool>> where)
        {
            return _repo.isExists(where);
        }

        public override int Update(QuestionsAnswerExamine model)
        {
            return _repo.Update(model);
        }
    }
}
