using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using ProductManagement.Infrastructure.AppService;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PMS.CommentsManage.Application.Services.Comment
{
    public class SchoolCommentExaminerService : AppService<SchoolCommentExamine>,ISchoolCommentExaminerService
    {
        private readonly ISchoolCommentExamineRepository _repo;
        public SchoolCommentExaminerService(ISchoolCommentExamineRepository repo)
        {
            _repo = repo;
        }

        public override int Delete(Guid Id)
        {
            return _repo.Delete(Id);
        }

        public bool ExecExaminer(Guid PartTimeJobAdminId, Guid TargetId, int Status, bool IsPartTimeJob = true)
        {
            return _repo.ExecExaminer(PartTimeJobAdminId, TargetId, Status,IsPartTimeJob);
        }

        public override IEnumerable<SchoolCommentExamine> GetList(Expression<Func<SchoolCommentExamine, bool>> where = null)
        {
            return _repo.GetList(where);
        }

        public override SchoolCommentExamine GetModelById(Guid Id)
        {
            return _repo.GetModelById(Id);
        }

        public List<SchoolCommentExamine> GetSchoolCommentByAdminId(Guid AdminId, int PageIndex, int PageSize,List<Guid> Ids, out int Total, bool IsPartTimeJob = true)
        {
            return _repo.GetSchoolCommentByAdminId(AdminId, PageIndex, PageSize, Ids, out Total,IsPartTimeJob);
        }

        public override int Insert(SchoolCommentExamine model)
        {
            return _repo.Insert(model);
        }

        public override bool isExists(Expression<Func<SchoolCommentExamine, bool>> where)
        {
            return _repo.isExists(where);
        }

        public override int Update(SchoolCommentExamine model)
        {
            return _repo.Update(model);
        }
    }
}
