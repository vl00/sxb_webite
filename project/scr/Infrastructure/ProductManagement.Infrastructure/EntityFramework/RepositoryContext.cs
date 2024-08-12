using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Infrastructure.EntityFramework
{
    public abstract class RepositoryContext : IRepositoryContext, IUnitOfWork, IDisposable
    {
        public abstract void BeginTransaction();
        public abstract int CommitAsync();

        public abstract void Dispose();

        public abstract void Rollback();
    }
}
