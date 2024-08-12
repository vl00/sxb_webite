using PMS.School.Domain.IRespository;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.School.Repository.Repositories
{
    public class GeneralTagRespository : IGeneralTagRespository
    {
        ISchoolDataDBContext _db;

        public GeneralTagRespository(ISchoolDataDBContext db)
        {
            _db = db;
        }

        public async Task<Dictionary<Guid, string>> GetByIDs(IEnumerable<Guid> ids)
        {
            var str_SQL = $@"Select ID as [Key] , Name as [Value] from GeneralTag WHERE id in @ids";
            var result = await _db.QueryAsync<KeyValuePair<Guid, string>>(str_SQL, new { ids });
            if (result?.Any() == true)
            {
                return result as Dictionary<Guid, string>;
            }
            return new Dictionary<Guid, string>();
        }

        public async Task<IEnumerable<KeyValuePair<Guid,string>>> GetByDataId(Guid dataId)
        {
            var str_SQL = $@"Select gt.ID as [Key] , gt.Name as [Value] from GeneralTag  gt
JOIN GeneralTagBind gtb ON gt.ID = gtb.TagId
WHERE gtb.dataId = @dataId";
            var result = await _db.QueryAsync<KeyValuePair<Guid, string>>(str_SQL, new { dataId });
            return result;

        }

        public async Task<Dictionary<Guid, string>> GetByNames(IEnumerable<string> names)
        {
            var str_SQL = $@"Select ID as [Key] , Name as [Value] from GeneralTag WHERE name in @names";
            var result = await _db.QueryAsync<KeyValuePair<Guid, string>>(str_SQL, new { names });
            if (result?.Any() == true)
            {
                return result.ToDictionary(k => k.Key, v => v.Value);
            }
            return new Dictionary<Guid, string>();
        }
    }
}
