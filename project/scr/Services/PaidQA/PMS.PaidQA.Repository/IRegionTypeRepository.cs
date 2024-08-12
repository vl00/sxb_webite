using PMS.PaidQA.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace PMS.PaidQA.Repository
{
    public interface IRegionTypeRepository : IRepository<RegionType>
    {
        Task<int> Count(string str_Where, object param);

        Task<IEnumerable<RegionType>> GetByTalentUserID(Guid talentID);
        Task<Dictionary<Guid, List<RegionType>>> GetByTalentUserIDs(IEnumerable<Guid> talentIDs);

        Task<int> RemoveTalentRegions(Guid talentUserID);
        Task<int> AddTalentRegions(Guid talentUserID, IEnumerable<Guid> regionTypeIDs);
    }
}
