using PMS.CommentsManage.Application.IServices.IProcViewService;
using PMS.CommentsManage.Domain.Entities.ProcViewEntities;
using PMS.CommentsManage.Domain.IRepositories.IProcViewRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.CommentsManage.Application.Services.ProcView
{
    public class ProcParAdminService : IProcParAdminService
    {
        private readonly IParAdminServiceRepository _repo;
        public ProcParAdminService(IParAdminServiceRepository repo)
        {
            _repo = repo;
        }

        public List<ProcGetAdminByRoleTypeEntity> ExecGetAdminByRoleType(Guid parentId, int type, int PageIndex, int PageSize, DateTime beginTime, DateTime endTime, out int TotalNumber)
        {
            return _repo.ExecGetAdminByRoleType(parentId, type, PageIndex, PageSize,beginTime,endTime, out TotalNumber);
        }
    }
}
