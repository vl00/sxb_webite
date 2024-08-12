using ProductManagement.Framework.MSSQLAccessor;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PMS.PaidQA.Repository
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

        public IEnumerable<TEntity> GetBy(string where, object param = null, string order = null, string[] fileds = null,IDbTransaction transaction=null)
        {
            return this._dbContext.GetBy<TEntity>(where: where, param: param, order: order, fileds: fileds,transaction:transaction);
        }

        public async Task<IEnumerable<TEntity>> GetByAsync(string where, object param = null, string order = null, string[] fileds = null, IDbTransaction transaction = null)
        {
            return await this._dbContext.GetByAsync<TEntity>(where: where, param: param, order: order, fileds: fileds, transaction: transaction);
        }

        public virtual TEntity Get<Tkey>(Tkey key)
        {
            return this._dbContext.Get<TEntity, Tkey>(key);
        }
        public virtual async Task<TEntity> GetAsync<Tkey>(Tkey key,IDbTransaction transaction=null)
        {
            return await this._dbContext.GetAsync<TEntity, Tkey>(key, transaction);
        }


        public virtual bool Add(TEntity entity)
        {
            return this._dbContext.Insert<TEntity>(entity) > 0;
        }

        public virtual bool Update(TEntity entity)
        {
            return this._dbContext.Update<TEntity>(entity);
        }
        public virtual bool Update(TEntity entity, IDbTransaction transaction = null, string[] fields = null)
        {
            return this._dbContext.Update<TEntity>(entity, transaction, fields) != null;
        }

        public virtual bool Delete(TEntity entity)
        {
            return this._dbContext.Delete<TEntity>(entity);
        }

        public async Task<bool> AddAsync(TEntity entity)
        {
            return await this._dbContext.InsertAsync(entity) > 0;
        }

        public async Task<bool> AddAsync(TEntity entity,IDbTransaction transaction = null)
        {
           
            return await this._dbContext.InsertAsync(entity, transaction) > 0;
        }

        public async Task<bool> AddsAsync(IEnumerable<TEntity> entitys, IDbTransaction transaction = null)
        {
            return await this._dbContext.InsertsAsync(entitys, transaction) > 0;
        }

        public virtual async Task<bool> UpdateAsync(TEntity entity)
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
        public IDbTransaction BeginTransaction()
        {
           return this._dbContext.BeginTransaction();   
        }

        public void Commit()
        {
             this._dbContext.Commit();
        }
        public void Rollback()
        {
            this._dbContext.Rollback();
        }

        public Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null)
        {
            var res = this._dbContext.ExecuteAsync(sql, param, transaction);
            return res;
        }
    }
}
