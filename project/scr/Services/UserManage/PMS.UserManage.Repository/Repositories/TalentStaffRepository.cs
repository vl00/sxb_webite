using PMS.OperationPlateform.Repository;
using PMS.UserManage.Domain.Entities;
using PMS.UserManage.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMS.UserManage.Repository.Repositories
{
    public class TalentStaffRepository : ITalentStaffRepository
    {
        private readonly UserDbContext _dbcontext;

        public TalentStaffRepository(UserDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }


    }
}
