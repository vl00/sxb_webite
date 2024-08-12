using PMS.PaidQA.Domain.Entities;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.PaidQA.Repository
{
    public class RegionTypeRepository : Repository<RegionType, PaidQADBContext>, IRegionTypeRepository
    {
        PaidQADBContext _paidQADBContext;
        public RegionTypeRepository(PaidQADBContext paidQADBContext) : base(paidQADBContext)
        {
            _paidQADBContext = paidQADBContext;
        }

        public async Task<int> Count(string str_Where, object param)
        {
            var str_SQL = $"Select Count(1) From [Order] Where {str_Where}";
            return await _paidQADBContext.QuerySingleAsync<int>(str_SQL, param);
        }

        public async Task<IEnumerable<RegionType>> GetByTalentUserID(System.Guid talentID)
        {
            var str_SQL = $@"SELECT
	                            rt.* 
                            FROM
	                            RegionType AS rt
	                            LEFT JOIN TalentRegion AS tr ON tr.RegionTypeID = rt.ID 
                            WHERE
	                            rt.IsValid = 1 
	                            AND tr.UserID = @talentID";
            return await _paidQADBContext.QueryAsync<RegionType>(str_SQL, new { talentID });
        }

        public async Task<int> RemoveTalentRegions(System.Guid talentUserID)
        {
            var str_SQL = $"delete From [TalentRegion] Where UserID = @talentUserID";
            return await _paidQADBContext.ExecuteAsync(str_SQL, new { talentUserID });
        }

        public async Task<int> AddTalentRegions(System.Guid talentUserID, IEnumerable<System.Guid> regionTypeIDs)
        {
            var result = 0;
            foreach (var item in regionTypeIDs)
            {
                var newGuid = System.Guid.NewGuid();
                var str_SQL = $"Insert Into [TalentRegion] (ID, UserID, RegionTypeID) Values (@id, @talentUserID, @regionTypeID)";
                result += await _paidQADBContext.ExecuteAsync(str_SQL, new { id = newGuid, talentUserID, regionTypeID = item });
            }
            return result;
        }

        public async Task<Dictionary<Guid, List<RegionType>>> GetByTalentUserIDs(IEnumerable<System.Guid> talentIDs)
        {
            var talentRegions = await _paidQADBContext.QueryAsync<(Guid, Guid)>("Select UserID,RegionTypeID from TalentRegion WHERE UserID in @talentIDs ORDER BY UserID", new { talentIDs });
            if (talentRegions?.Any() == true)
            {
                var ids = talentRegions.Select(p => p.Item2).Distinct();
                var regions = await _paidQADBContext.QueryAsync<RegionType>("Select * from RegionType WHERE ID IN @ids", new { ids });
                if (regions?.Any() == true)
                {
                    var result = new Dictionary<Guid, List<RegionType>>();
                    foreach (var item in talentRegions)
                    {
                        if (!result.ContainsKey(item.Item1))
                        {
                            result.Add(item.Item1, new List<RegionType>());
                        }
                        var region = regions.FirstOrDefault(p => p.ID == item.Item2);
                        if (region != null)
                        {
                            result[item.Item1].Add(region);
                        }
                    }
                    return result;
                }
            }
            return null;
        }
    }
}
