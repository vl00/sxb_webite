using PMS.Infrastructure.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Infrastructure.Application.IService
{
    public interface IServerStorageService
    {
        Task<ServerStorage> GetAsync(string hashKey, string key);
        Task<bool> SetAsync(ServerStorage serverStorage);
    }
}
