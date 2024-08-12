using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Framework.EntityFramework
{
    public abstract class RepositoryContext : IRepositoryContext, IUnitOfWork, IDisposable
    {
        public abstract void BeginTransaction();
        public abstract int CommitAsync();

        public abstract void Dispose();

        public abstract void Rollback();
        public abstract void TranCommit();
    }
}
