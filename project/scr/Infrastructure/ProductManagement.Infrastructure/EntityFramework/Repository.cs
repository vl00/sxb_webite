using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.Infrastructure.EntityFramework
{
    public abstract class Repository<Entity> : IRepository<Entity> where Entity : class
    {
        protected Repository(IRepositoryContext repository) { }
        public abstract int Add(Entity Entity);
        public abstract int Delete(Guid Id);
        public abstract Entity GetAggregateById(Guid Id);
        public abstract int GetCount(Expression<Func<Entity, bool>> where = null);
        public abstract IEnumerable<Entity> GetList(Expression<Func<Entity, bool>> where = null);
        public abstract IEnumerable<Entity> GetPageList(Expression<Func<Entity, bool>> where = null, string order = "", int pageIndex = 1, int pageSize = 10);
        public abstract int Update(Entity Entity);
        public abstract IEnumerable<Entity> QueryByProc(string procName, out List<object> outputData, params SqlParameter[] paras);
        public abstract IEnumerable<Entity> Query(string sql, params SqlParameter[] paras);
        public abstract int ExecuteNonQuery(string sql, params SqlParameter[] paras);
    }
}
