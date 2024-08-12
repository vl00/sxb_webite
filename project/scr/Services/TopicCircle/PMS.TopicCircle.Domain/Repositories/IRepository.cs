using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PMS.TopicCircle.Domain.Repositories
{
    public interface IRepository<TEntity> where TEntity:class 
    {
        IEnumerable<TEntity> GetBy(string where, object param = null, string order = null, string[] fileds = null);

        TEntity Get<Tkey>(Tkey key);

        bool Add(TEntity entity);

        Task<bool> AddAsync(TEntity entity);

        bool Update(TEntity entity);

        Task<bool> UpdateAsync(TEntity entity);

        bool Update(TEntity entity, IDbTransaction transaction = null, string[] fields=null);

        Task<bool> UpdateAsync(TEntity entity, IDbTransaction transaction = null, string[] fields = null);

        bool Delete(TEntity entity);

        Task<bool> DeleteAsync(TEntity entity);
    }
}