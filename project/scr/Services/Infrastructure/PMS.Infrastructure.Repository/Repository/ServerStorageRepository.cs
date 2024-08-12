using PMS.Infrastructure.Domain.Entities;
using PMS.Infrastructure.Domain.IRepositories;
using ProductManagement.Framework.MSSQLAccessor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Infrastructure.Repository.Repository
{
    public class ServerStorageRepository : Repository<ServerStorage, JcDbContext>, IServerStorageRepository
    {
        private JcDbContext _dbContext;

        public ServerStorageRepository(JcDbContext jcDbContext) : base(jcDbContext)
        {

            _dbContext = jcDbContext;
        }

        public async Task<bool> SetAsync(ServerStorage serverStorage)
        {
            string delsql = @"delete ServerStorage WHERE (HashKey = @HashKey AND [Key] = @key) Or ExpireAt < GETDATE()";
            using (var tran = _dbContext.BeginTransaction())
            {
                try
                {
                    bool flag = false;
                    flag = (await _dbContext.ExecuteAsync(delsql, serverStorage,tran)) > 0;
                    flag = (await _dbContext.InsertAsync(serverStorage, tran)) > 0;
                    tran.Commit();
                    return flag;
                }
                catch (Exception)
                {
                    tran.Rollback();
                    return false;
                }
            }

        }
    }
}
