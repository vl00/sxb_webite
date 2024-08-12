using PMS.TopicCircle.Domain.Entities;
using PMS.TopicCircle.Domain.Repositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.TopicCircle.Repository
{
    public class TagRepository : Repository<Tag, TopicCircleDBContext>, ITagRepository
    {
        TopicCircleDBContext _dbContext;
        public TagRepository(TopicCircleDBContext dBContext) : base(dBContext)
        {
            _dbContext = dBContext;
        }

        public IEnumerable<Tag> ExcuteUSP_STASTICHOTTAG(Guid circleId, int takeCount)
        {
            return this._dbContext.Query<Tag>("USP_STASTICHOTTAG", new { circleId, takeCount }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Tag>> GetByTopicID(Guid topicID)
        {
            var str_SQL = $@"SELECT
	                            *
                            FROM
	                            TopicTag AS tt
	                            LEFT JOIN Tag AS t ON t.Id = tt.TagId 
                            WHERE
	                            t.IsDeleted = 0 
	                            AND tt.TopicId = @topicID";
            return await _dbContext.QueryAsync<Tag>(str_SQL, new { topicID });
        }

        public IEnumerable<Tag> GetTags()
        {
            IEnumerable<Tag> tags = this._dbContext.GetBy<Tag>("IsDeleted=0", null, null, null,null);
            return tags;
        }
    }
}
