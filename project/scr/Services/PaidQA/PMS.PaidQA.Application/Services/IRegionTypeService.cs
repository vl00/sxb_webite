using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.EntityExtend;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.PaidQA.Application.Services
{
    public interface IRegionTypeService : IApplicationService<RegionType>
    {
        Task<IEnumerable<RegionType>> GetByTalentUserID(Guid talentUserID);
        Task<Dictionary<Guid, List<RegionType>>> GetByTalentUserIDs(IEnumerable<Guid> talentUserIDs);
        Task<bool> RemoveTalentRegions(Guid talentUserID);
        Task<bool> AddTalentRegions(Guid talentUserID, IEnumerable<Guid> regionTypeIDs);

        Task<IEnumerable<RegionTypeExtend>> GetAllRegionTypes();
    }
}
