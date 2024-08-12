using PMS.CommentsManage.Application.IServices;
using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PMS.CommentsManage.Application.Services.Admin
{
    public class PartTimeJobAdminRolereService : IPartTimeJobAdminRolereService
    {
        private IPartTimeJobAdminRolereRepository _partTimeJob;

        public PartTimeJobAdminRolereService(IPartTimeJobAdminRolereRepository partTimeJob) 
        {
            _partTimeJob = partTimeJob;
        }

        public bool Add(PartTimeJobAdminRole userGrant) 
        {
            return _partTimeJob.Add(userGrant);
        }

        public List<PartTimeJobAdminRole> GetPartJobRoles(Guid AdminId) 
        {
            return _partTimeJob.GetPartJobRoles(AdminId);
        }

        public IEnumerable<PartTimeJobAdminRole> GetList(Expression<Func<PartTimeJobAdminRole, bool>> where = null) 
        {
            return _partTimeJob.GetList(where);
        }

        public bool UpdateAdminShield(Guid Id)
        {
            return _partTimeJob.UpdateAdminShield(Id);
        }
    }
}
