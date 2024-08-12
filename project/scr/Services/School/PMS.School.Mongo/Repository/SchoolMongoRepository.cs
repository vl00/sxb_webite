using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using PMS.School.Domain.Entities.Mongo;
using PMS.School.Domain.IMongo;
using ProductManagement.Framework.MongoDb;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.School.Mongo.Repository
{
    public class SchoolMongoRepository : ISchoolMongoRepository
    {
        IMongoService _mongo;
        public SchoolMongoRepository(IMongoService mongo)
        {
            _mongo = mongo;
        }

        public async Task<int> GetSchoolCountViaSchFtypeArea(string schFType, int areaCode)
        {
            var collection = _mongo.ImongdDb.GetCollection<AiParams>(nameof(AiParams));
            return await collection.AsQueryable().CountAsync(p => p.areacode == areaCode && p.SchFtype == schFType);
        }

        public async Task<int> GetSchoolCountViaSchFtypeCity(string schFType, int cityCode)
        {
            var collection = _mongo.ImongdDb.GetCollection<AiParams>(nameof(AiParams));
            return await collection.AsQueryable().CountAsync(p => p.areacode >= cityCode && p.areacode < (cityCode + 100) && p.SchFtype == schFType);
        }

        public async Task<int> GetSchoolCount(string schFType, int areaCode, int minScore, int maxScore)
        {
            var collection = _mongo.ImongdDb.GetCollection<AiParams>(nameof(AiParams));
            return await collection.AsQueryable().CountAsync(p => p.totalscore >= minScore && p.totalscore < maxScore && p.SchFtype == schFType && p.areacode == areaCode);
        }

        public async Task<int> GetSchoolCount(string schFType, int areaCode, int maxScore)
        {
            var collection = _mongo.ImongdDb.GetCollection<AiParams>(nameof(AiParams));
            return await collection.AsQueryable().CountAsync(p => p.totalscore > maxScore && p.SchFtype == schFType && p.areacode == areaCode);
        }

        public async Task<GDParams> GetGDParamsByEID(Guid eid)
        {
            var collection = _mongo.ImongdDb.GetCollection<GDParams>(nameof(GDParams));
            return await collection.AsQueryable().FirstOrDefaultAsync(p => p.eid.ToLower() == eid.ToString().ToLower());
        }

        public async Task<GDParams> GetGDParamsAvg(string schFtype, int areaCode)
        {
            var collection = _mongo.ImongdDb.GetCollection<GDParams>(nameof(GDParams));
            var query = collection.AsQueryable().Where(p => p.SchFtype == schFtype && p.area == areaCode);
            if (!query.Any()) return new GDParams();
            return new GDParams()
            {
                bookmarket = Math.Round((query?.Average(p => p.bookmarket) ?? 0) + 0.5),
                buildingprice = Math.Round((query?.Average(p => p.buildingprice) ?? 0) + 0.5),
                bus = Math.Round((query?.Average(p => p.bus) ?? 0) + 0.5),
                hospital = Math.Round((query?.Average(p => p.hospital) ?? 0) + 0.5),
                metro = Math.Round((query?.Average(p => p.metro) ?? 0) + 0.5),
                museum = Math.Round((query?.Average(p => p.museum) ?? 0) + 0.5),
                police = Math.Round((query?.Average(p => p.police) ?? 0) + 0.5),
                shoppinginfo = Math.Round((query?.Average(p => p.shoppinginfo) ?? 0) + 0.5),
                toptraininfo = Math.Round((query?.Average(p => p.toptraininfo) ?? 0) + 0.5),
                traininfo = Math.Round((query?.Average(p => p.traininfo) ?? 0) + 0.5),
                library = Math.Round((query?.Average(p => p.library) ?? 0) + 0.5)
            };
        }

        public async Task<AiParams> GetAiParamsByEID(Guid eid)
        {
            var collection = _mongo.ImongdDb.GetCollection<AiParams>(nameof(AiParams));
            return await collection.AsQueryable().FirstOrDefaultAsync(p => p.eid.ToLower() == eid.ToString().ToLower());
        }

        public async Task<AiParams> GetAiParamsAvg(string schFtype, int areaCode)
        {
            var collection = _mongo.ImongdDb.GetCollection<AiParams>(nameof(AiParams));
            var query = collection.AsQueryable().Where(p => p.SchFtype == schFtype && p.areacode == areaCode);
            if (!query.Any()) return new AiParams();
            return new AiParams()
            {
                avgcommentscore = Math.Round((await query?.AverageAsync(p => p.avgcommentscore)) + 0.5),
                hot = Math.Round((await query?.AverageAsync(p => p.hot)) + 0.5),
                searchhot = Math.Round((await query?.AverageAsync(p => p.searchhot)) + 0.5),
                student_count = (int)Math.Round((await query?.AverageAsync(p => p.student_count)) + 0.5),
                totalscore = (int)Math.Round((await query?.AverageAsync(p => p.totalscore)) + 0.5)
            };
        }
    }
}
