using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.EntityExtend;
using PMS.PaidQA.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.PaidQA.Application.Services
{
    public class RegionTypeService : ApplicationService<RegionType>, IRegionTypeService
    {
        IRegionTypeRepository _regionTypeRepository;
        public RegionTypeService(IRegionTypeRepository regionTypeRepository) : base(regionTypeRepository)
        {
            _regionTypeRepository = regionTypeRepository;
        }

        public async Task<IEnumerable<RegionType>> GetByTalentUserID(Guid talentUserID)
        {
            if (talentUserID == Guid.Empty) return null;
            return await _regionTypeRepository.GetByTalentUserID(talentUserID);
        }

        public async Task<bool> RemoveTalentRegions(Guid talentUserID)
        {
            if (talentUserID == Guid.Empty) return false;
            return await _regionTypeRepository.RemoveTalentRegions(talentUserID) > 0;
        }

        public async Task<bool> AddTalentRegions(Guid talentUserID, IEnumerable<Guid> regionTypeIDs)
        {
            if (talentUserID == Guid.Empty || regionTypeIDs == null || !regionTypeIDs.Any()) return false;
            return await _regionTypeRepository.AddTalentRegions(talentUserID, regionTypeIDs) > 0;
        }

        public async Task<IEnumerable<RegionTypeExtend>> GetAllRegionTypes()
        {
            var str_where = "IsValid = 1";
            var result = new List<RegionTypeExtend>();

            await Task.Run(() =>
            {
                var finds = _regionTypeRepository.GetBy(str_where, new { }, fileds: new string[1] { "*" });
                if (finds?.Any() == true)
                {
                    foreach (var item in finds.Where(p => p.PID == null).OrderBy(p => p.Sort))
                    {
                        var entity = new RegionTypeExtend()
                        {
                            ID = item.ID,
                            IsValid = true,
                            Name = item.Name,
                            Sort = item.Sort
                        };
                        entity.SubItems = finds.Where(p => p.PID == item.ID)?.OrderBy(p => p.Sort).Select(p => new RegionType()
                        {
                            ID = p.ID,
                            IsValid = true,
                            Name = p.Name,
                            PID = item.ID,
                            Sort = p.Sort
                        });
                        result.Add(entity);
                    }
                }
            });

            return result;
        }

        public async Task<Dictionary<Guid, List<RegionType>>> GetByTalentUserIDs(IEnumerable<Guid> talentUserIDs)
        {
            if (talentUserIDs?.Any() == true)
            {
                return await _regionTypeRepository.GetByTalentUserIDs(talentUserIDs);
            }
            return new Dictionary<Guid, List<RegionType>>();
        }
    }
}
