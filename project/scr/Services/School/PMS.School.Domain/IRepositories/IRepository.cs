using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PMS.School.Domain.IRepositories
{
    public interface IRepository<TEntity> where TEntity : class
    {

        Task<IEnumerable<TEntity>> GetByAsync(string where, object param = null, string order = null, string[] fileds = null, IDbTransaction transaction = null);
        IEnumerable<TEntity> GetBy(string where, object param = null, string order = null, string[] fileds = null, IDbTransaction transaction = null);

        TEntity Get<Tkey>(Tkey key);

        Task<TEntity>  GetAsync<Tkey>(Tkey key, IDbTransaction transaction= null);

        bool Add(TEntity entity);

        Task<bool> AddAsync(TEntity entity);

        Task<bool> AddAsync(TEntity entity, IDbTransaction transaction = null);

        Task<bool> AddsAsync(IEnumerable<TEntity> entitys, IDbTransaction transaction = null);
        bool Update(TEntity entity);

        Task<bool> UpdateAsync(TEntity entity);

        bool Update(TEntity entity, IDbTransaction transaction = null, string[] fields = null);

        Task<bool> UpdateAsync(TEntity entity, IDbTransaction transaction = null, string[] fields = null);

        bool Delete(TEntity entity);

        Task<bool> DeleteAsync(TEntity entity);


        IDbTransaction BeginTransaction();
        void Commit();

        void Rollback();

        Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null);

    }
}
