using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Framework.EntityFramework
{
    public interface IEntityFrameworkRepositoryContext : IRepositoryContext,IUnitOfWork,IDisposable
    {
        DbContext dbContext { get; }
    }
}
