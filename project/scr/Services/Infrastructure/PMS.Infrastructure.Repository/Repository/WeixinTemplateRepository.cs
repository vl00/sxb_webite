using PMS.Infrastructure.Domain.IRepositories;
using System;
using System.Threading.Tasks;

namespace PMS.Infrastructure.Repository.Repository
{
    public class WeixinTemplateRepository : IWeixinTemplateRepository
    {
        private JcDbContext _dbContext;
        public WeixinTemplateRepository(JcDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> GetTemplateText(Guid id)
        {
            var str_SQL = $"Select TOP 1 [Text] From [weixin_reply_template] Where [Id] = @id";
            return await _dbContext.QuerySingleAsync<string>(str_SQL, new { id });
        }
    }
}
