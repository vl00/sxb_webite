
using PMS.TopicCircle.Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.TopicCircle.Application.Services
{
    public abstract class ApplicationService<TEntity>:IApplicationService<TEntity> where TEntity : class
    {
        IRepository<TEntity> _repository;
        public ApplicationService(IRepository<TEntity> repository)
        {
            this._repository = repository;
        }

        public bool Add(TEntity entity)
        {
            return _repository.Add(entity);
        }

        public Task<bool> AddAsync(TEntity entity)
        {
            return _repository.AddAsync(entity);
        }

        public bool Delete(TEntity entity)
        {
            return _repository.Delete(entity);
        }

        public TEntity Get<Tkey>(Tkey key)
        {
            return _repository.Get(key);
        }

        public bool Update(TEntity entity)
        {
            return _repository.Update(entity);
        }

        public Task<bool> UpdateAsync(TEntity entity)
        {
            return _repository.UpdateAsync(entity);
        }
    }
}