using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ProductManagement.Framework.MSSQLAccessor
{
    public interface IRepository<TEntity> where TEntity:class 
    {
        IEnumerable<TEntity> GetBy(string where, object param = null, string order = null, string[] fileds = null);
        Task<IEnumerable<TEntity>> GetByAsync(string where, object param = null, string order = null, string[] fileds = null);

        TEntity Get<Tkey>(Tkey key);
        Task<TEntity> GetAsync<Tkey>(Tkey Id);
        bool Add(TEntity entity, IDbTransaction transaction = null);

        Task<bool> AddAsync(TEntity entity,IDbTransaction transaction = null);

        bool Update(TEntity entity);

        Task<bool> UpdateAsync(TEntity entity);

        bool Update(TEntity entity, IDbTransaction transaction = null, string[] fields=null);

        Task<bool> UpdateAsync(TEntity entity, IDbTransaction transaction = null, string[] fields = null);

        bool Delete(TEntity entity, IDbTransaction transaction = null);

        Task<bool> DeleteAsync(TEntity entity, IDbTransaction transaction = null);

        IDbTransaction BeginTransaction();
    }
}