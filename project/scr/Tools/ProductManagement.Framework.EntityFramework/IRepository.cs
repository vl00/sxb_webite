using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.Framework.EntityFramework
{
    public interface IRepository<Entity> where Entity : class
    {
        void TranAdd(Entity entity);
        int Add(Entity Entity);
        int Update(Entity Entity);
        int Delete(Guid Id);
        Entity GetAggregateById(Guid Id);
        IQueryable<Entity> GetList(Expression<Func<Entity, bool>> where = null);
        IEnumerable<Entity> GetPageList(Expression<Func<Entity, bool>> where = null, string order = "", int pageIndex = 1, int pageSize = 10);
        int GetCount(Expression<Func<Entity, bool>> where = null);
        IEnumerable<Entity> QueryByProc(string procName, out List<object> outputData, params SqlParameter[] paras);
        IEnumerable<TEntity> QueryByProc<TEntity>(string procName, out List<object> outputData, params SqlParameter[] paras) where TEntity : class,new();
        IEnumerable<Entity> Query(string sql, params SqlParameter[] paras);
        IEnumerable<TEntity> Query<TEntity>(string sql, params SqlParameter[] paras) where TEntity : class,new();
        int ExecuteNonQuery(string sql, params SqlParameter[] paras);
        object ExecuteScalar(string sql, params SqlParameter[] paras);
    }
}
