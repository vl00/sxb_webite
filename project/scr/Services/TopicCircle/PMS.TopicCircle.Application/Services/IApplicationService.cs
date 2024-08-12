using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.TopicCircle.Application.Services
{
    public interface IApplicationService<TEntity> where TEntity:class
    {
        TEntity Get<Tkey>(Tkey key);

        bool Add(TEntity entity);

        Task<bool> AddAsync(TEntity entity);

        bool Update(TEntity entity);

        Task<bool> UpdateAsync(TEntity entity);

        bool Delete(TEntity entity);
    }
}