using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ProductManagement.Infrastructure.EntityFramework
{
    /// <summary>
    /// 实体上下文
    /// </summary>
    public class EntityFrameworkRepositoryContext : RepositoryContext, IEntityFrameworkRepositoryContext, IUnitOfWork, IDisposable
    {
        private DbContext _dbContext;
        public DbContext dbContext { get { return this._dbContext; } }

        private IDbContextTransaction _contextTransaction;

        public EntityFrameworkRepositoryContext(DbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public override void BeginTransaction()
        {
            _contextTransaction = _dbContext.Database.BeginTransaction();
        }

        public override int CommitAsync()
        {
            if (_contextTransaction == null)
                return _dbContext.SaveChanges();
            else
                _contextTransaction.Commit();
            return 1;
        }

        public override void Rollback()
        {
            if(_contextTransaction!=null)
                this.dbContext.Database.RollbackTransaction();
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this._dbContext.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~UnitOfWork() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public override void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }

        #endregion
    }
}
