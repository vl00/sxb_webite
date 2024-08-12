using PMS.School.Domain.Entities.Mongo;
using System;
using System.Threading.Tasks;

namespace PMS.School.Domain.IMongo
{
    public interface ISchoolMongoRepository
    {
        Task<int> GetSchoolCountViaSchFtypeCity(string schFType, int cityCode);
        Task<int> GetSchoolCountViaSchFtypeArea(string schFType, int areaCode);
        Task<int> GetSchoolCount(string schFType, int areaCode, int minScore, int maxScore);
        Task<int> GetSchoolCount(string schFType, int areaCode, int maxScore);
        Task<GDParams> GetGDParamsByEID(Guid eid);
        Task<GDParams> GetGDParamsAvg(string schFtype, int areaCode);
        Task<AiParams> GetAiParamsByEID(Guid eid);
        Task<AiParams> GetAiParamsAvg(string schFtype, int areaCode);
    }
}
