using PMS.Live.Domain.IRepositories;
using ProductManagement.Framework.MSSQLAccessor;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PMS.Live.Repository
{
    public abstract class Repository<TEntity, TDbType> : Domain.IRepositories.IRepository<TEntity>
        where TEntity : class
        where TDbType : DBContext<TDbType>
    {
        DBContext<TDbType> _dbContext;

        public Repository(DBContext<TDbType> dBContext)
        {
            this._dbContext = dBContext;
        }

        public IEnumerable<TEntity> GetBy(string where, object param = null, string order = null, string[] fileds = null)
        {
            return this._dbContext.GetBy<TEntity>(where: where,
                                                  param: param,
                                                  order: order,
                                                  fileds: fileds,
                                                  transaction:null);
        }

        public virtual TEntity Get<Tkey>(Tkey key)
        {
            return this._dbContext.Get<TEntity, Tkey>(key);
        }

        public virtual bool Add(TEntity entity)
        {
            return this._dbContext.Insert(entity) > 0;
        }

        public virtual bool Update(TEntity entity)
        {
            return this._dbContext.Update(entity);
        }
        public virtual bool Update(TEntity entity, IDbTransaction transaction = null, string[] fields = null)
        {
            return this._dbContext.Update(entity, transaction, fields) != null;
        }

        public virtual bool Delete(TEntity entity)
        {
            return this._dbContext.Delete(entity);
        }

        public async Task<bool> AddAsync(TEntity entity)
        {
            return await this._dbContext.InsertAsync(entity) > 0;
        }

        public async Task<bool> UpdateAsync(TEntity entity)
        {
            return await this._dbContext.UpdateAsync(entity);
        }

        public virtual async Task<bool> UpdateAsync(TEntity entity, IDbTransaction transaction = null, string[] fields = null)
        {
            return await this._dbContext.UpdateAsync<TEntity>(entity, transaction, fields) != null;
        }

        public Task<bool> DeleteAsync(TEntity entity)
        {
            throw new System.NotImplementedException();
        }
    }
}