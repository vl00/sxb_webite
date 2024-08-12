using PMS.Infrastructure.Application.IService;
using PMS.Infrastructure.Domain.Entities;
using PMS.Infrastructure.Domain.IRepositories;
using ProductManagement.Framework.Cache.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Infrastructure.Application.Service
{
    public class ServerStorageService : IServerStorageService
    {
        IServerStorageRepository _repository;
        IEasyRedisClient _easyRedisClient;
        public ServerStorageService(IServerStorageRepository repository, IEasyRedisClient easyRedisClient)
        {
            _repository = repository;
            _easyRedisClient = easyRedisClient;
        }

        public async Task<ServerStorage> GetAsync(string hashKey, string key)
        {
            string where = @" ExpireAt >= GETDATE()
AND HashKey=@HashKey
AND [Key] = @Key ";
           var res = await _repository.GetByAsync(where, new { HashKey = hashKey, Key = key });
            return res.FirstOrDefault();
        }

        public async Task<bool> SetAsync(ServerStorage serverStorage)
        {

            return await _repository.SetAsync(serverStorage);

        }
    }
}
