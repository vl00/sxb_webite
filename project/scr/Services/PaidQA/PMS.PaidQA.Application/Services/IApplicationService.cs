using System.Data;
using System.Threading.Tasks;

namespace PMS.PaidQA.Application.Services
{
    public interface IApplicationService<TEntity> where TEntity : class
    {
        TEntity Get<Tkey>(Tkey key);
        Task<TEntity> GetAsync<Tkey>(Tkey key);
        bool Add(TEntity entity);

        Task<bool> AddAsync(TEntity entity);

        bool Update(TEntity entity);

        Task<bool> UpdateAsync(TEntity entity);

        Task<bool> UpdateAsync(TEntity entity, string[] fileds);

        bool Delete(TEntity entity);
    }
}
