using PMS.Infrastructure.Domain.IRepositories;
using System.Threading.Tasks;

namespace PMS.Infrastructure.Repository.Repository
{
    public class KeyValyeRepository : IKeyValyeRepository
    {
        private JcDbContext _dbContext;
        public KeyValyeRepository(JcDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> GetValue(string key)
        {
            var str_SQL = $"Select TOP 1 [Value] From [KeyValue] Where [Key] = @key";
            return await _dbContext.QuerySingleAsync<string>(str_SQL, new { key });
        }
    }
}
