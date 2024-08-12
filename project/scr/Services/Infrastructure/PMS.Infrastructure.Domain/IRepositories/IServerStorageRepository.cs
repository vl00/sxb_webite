using PMS.Infrastructure.Domain.Entities;
using ProductManagement.Framework.MSSQLAccessor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Infrastructure.Domain.IRepositories
{
    public interface IServerStorageRepository : IRepository<ServerStorage>
    {

        Task<bool> SetAsync(ServerStorage serverStorage);
    }
}
