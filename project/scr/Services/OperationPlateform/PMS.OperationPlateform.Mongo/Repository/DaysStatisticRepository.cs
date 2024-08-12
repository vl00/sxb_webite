using MongoDB.Bson;
using MongoDB.Driver;
using ProductManagement.Framework.MongoDb;
using System;
using System.Collections.Generic;
using System.Text;
using PMS.OperationPlateform.Domain.IMongo;
using System.Linq;
using PMS.OperationPlateform.Domain.MongoModel;
using Newtonsoft.Json;
using PMS.OperationPlateform.Domain.MongoModel.Base;
using System.Threading.Tasks;

namespace PMS.OperationPlateform.Mongo.Repository
{
    public class DaysStatisticRepository : IDaysStatisticRepository
    {
        private readonly IMongoService _mongo;
        public readonly IMongoDatabase _database;
        public readonly IMongoCollection<DaysStatistics> _collection;

        public DaysStatisticRepository(IMongoService<IStatisticsMongoProvider> mongo)
        {
            _mongo = mongo;
            _database = _mongo.GetDatabase("ischoollog");
            _collection = _database.GetCollection<DaysStatistics>("daysstatistics");
        }

        public async Task AddAsync(IEnumerable<DaysStatistics> daysStatistics)
        {
            await _collection.InsertManyAsync(daysStatistics);
        }
    }
}
