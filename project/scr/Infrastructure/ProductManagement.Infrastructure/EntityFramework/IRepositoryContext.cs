using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Infrastructure.EntityFramework
{
    public interface IRepositoryContext : IUnitOfWork,IDisposable
    {

    }
}
