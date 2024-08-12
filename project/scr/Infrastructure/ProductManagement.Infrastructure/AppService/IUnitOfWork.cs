using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ProductManagement.Infrastructure.AppService
{
    public interface IUnitOfWork : IDisposable
    {
        IDbTransaction BeginTransaction();

        void Commit();

        void Rollback();
    }
}
