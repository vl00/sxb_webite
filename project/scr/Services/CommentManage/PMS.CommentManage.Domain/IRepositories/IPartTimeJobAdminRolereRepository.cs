using PMS.CommentsManage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PMS.CommentsManage.Domain.IRepositories
{
    public interface IPartTimeJobAdminRolereRepository
    {
        bool Add(PartTimeJobAdminRole userGrant);

        List<PartTimeJobAdminRole> GetPartJobRoles(Guid AdminId);

        IEnumerable<PartTimeJobAdminRole> GetList(Expression<Func<PartTimeJobAdminRole, bool>> where = null);
        bool UpdateAdminShield(Guid Id);
    }
}
