using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.Infrastructure.EntityFramework
{
    public interface IEntityFrameworkRepositoryContext : IRepositoryContext,IUnitOfWork,IDisposable
    {
        DbContext dbContext { get; }
    }
}
