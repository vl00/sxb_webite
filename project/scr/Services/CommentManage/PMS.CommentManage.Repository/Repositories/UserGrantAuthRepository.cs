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
    public class UserGrantAuthRepository : EntityFrameworkRepository<UserGrantAuth>,IUserGrantAuthRepository
    {
        public UserGrantAuthRepository(CommentsManageDbContext dbContext) :base(dbContext)
        {
        }

        public new bool Add(UserGrantAuth userGrant)
        {
            return base.Add(userGrant) > 0;
        }

        public bool IsGrantAuth(Guid AdminId)
        {
             return base.GetList(x => x.UserId == AdminId).FirstOrDefault() != null;
        }
    }
}
