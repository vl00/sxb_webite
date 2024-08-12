using PMS.CommentsManage.Domain.Entities;
using PMS.CommentsManage.Domain.IRepositories;
using PMS.CommentsManage.Repository.Interface;
using ProductManagement.Framework.EntityFramework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using ProductManagement.Infrastructure.AppService;

namespace PMS.CommentsManage.Repository.Repositories
{
    public class PartTimeJobAdminRolereRepository : EntityFrameworkRepository<PartTimeJobAdminRole>,IPartTimeJobAdminRolereRepository
    {
        //private readonly EntityFrameworkRepository<PartTimeJobAdminRole> efRepository;

        public PartTimeJobAdminRolereRepository(CommentsManageDbContext _dbContext):base(_dbContext)
        {
            //efRepository = eFRepo;
        }

        public new IEnumerable<PartTimeJobAdminRole> GetList(Expression<Func<PartTimeJobAdminRole, bool>> where = null)
        {
            var data = base.GetList(where);
            if (data.Count() != 0)
            {
                return data.ToList();
            }
            return null;
        }

        public new bool Add(PartTimeJobAdminRole userGrant)
        {
            return base.Add(userGrant) > 0;
        }

        public List<PartTimeJobAdminRole> GetPartJobRoles(Guid AdminId)
        {
            return base.GetList(x => x.AdminId == AdminId)?.ToList();
        }

        public bool UpdateAdminShield(Guid Id) 
        {
            PartTimeJobAdminRole partTimeJobAdmin = GetList(x=>x.AdminId == Id && x.Role == 1).FirstOrDefault();
            partTimeJobAdmin.Shield = !partTimeJobAdmin.Shield;
            base.Update(partTimeJobAdmin);
            return partTimeJobAdmin.Shield;
        }
        //public bool IsGrantAuth(Guid AdminId)
        //{
        //    return GetList(x => x.UserId == AdminId).FirstOrDefault() != null;
        //}
    }
}
