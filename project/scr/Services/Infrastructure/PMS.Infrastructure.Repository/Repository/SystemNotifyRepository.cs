using PMS.Infrastructure.Domain.Entities;
using PMS.Infrastructure.Domain.IRepositories;
using ProductManagement.Framework.MSSQLAccessor;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Infrastructure.Repository.Repository
{
    public class SystemNotifyRepository : Repository<SystemNotify, JcDbContext>, ISystemNotifyRepository
    {
        private JcDbContext _dbContext;

        public SystemNotifyRepository(JcDbContext jcDbContext) : base(jcDbContext)
        {

            _dbContext = jcDbContext;
        }

    }
}
