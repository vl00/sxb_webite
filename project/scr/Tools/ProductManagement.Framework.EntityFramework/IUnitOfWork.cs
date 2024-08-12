using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.Framework.EntityFramework
{
    public interface IUnitOfWork
    {
        void BeginTransaction();
        int CommitAsync();
        void Rollback();
        void TranCommit();
    }
}
