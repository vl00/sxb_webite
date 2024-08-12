using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using System.Data;
using ProductManagement.Framework.MSSQLAccessor;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System.Threading.Tasks;
namespace ProductManagement.Framework.MSSQLAccessor
{
    public abstract class Repository<TEntity, TDbType> : IRepository<TEntity>
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
            return this._dbContext.GetBy<TEntity>(where: where, param: param, order: order, fileds: fileds, null);
        }

        public Task<IEnumerable<TEntity>> GetByAsync(string where, object param = null, string order = null, string[] fileds = null)
        {
            return this._dbContext.GetByAsync<TEntity>(where: where, param: param, order: order, fileds: fileds, null);
        }

        public virtual TEntity Get<Tkey>(Tkey key)
        {
            return this._dbContext.Get<TEntity, Tkey>(key);
        }

        public virtual bool Add(TEntity entity, IDbTransaction transaction = null)
        {
            return this._dbContext.Insert<TEntity>(entity, transaction) > 0;
        }

        public virtual bool Update(TEntity entity)
        {
            return this._dbContext.Update<TEntity>(entity);
        }
        public virtual bool Update(TEntity entity, IDbTransaction transaction = null, string[] fields = null)
        {
            return this._dbContext.Update<TEntity>(entity, transaction, fields) != null;
        }

        public virtual bool Delete(TEntity entity, IDbTransaction transaction = null)
        {
            return this._dbContext.Delete<TEntity>(entity, transaction);
        }

        public async Task<bool> AddAsync(TEntity entity, IDbTransaction transaction = null)
        {
            return await this._dbContext.InsertAsync(entity, transaction) > 0;
        }

        public async Task<bool> UpdateAsync(TEntity entity)
        {
            return await this._dbContext.UpdateAsync(entity);
        }

        public virtual async Task<bool> UpdateAsync(TEntity entity, IDbTransaction transaction = null, string[] fields = null)
        {
            return await this._dbContext.UpdateAsync<TEntity>(entity, transaction, fields) != null;
        }

        public async Task<bool> DeleteAsync(TEntity entity, IDbTransaction transaction = null)
        {
            return await this._dbContext.DeleteAsync(entity,transaction);
        }

        public async Task<TEntity> GetAsync<Tkey>(Tkey Id)
        {
            return await _dbContext.GetAsync<TEntity, Tkey>(Id, null, null);
        }

        public IDbTransaction BeginTransaction()
        {
            return _dbContext.BeginTransaction();
        }
     

    }
}