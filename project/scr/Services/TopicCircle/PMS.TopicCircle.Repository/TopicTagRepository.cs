using PMS.TopicCircle.Domain.Dtos;
using PMS.TopicCircle.Domain.Entities;
using PMS.TopicCircle.Domain.Repositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Text;
using static PMS.TopicCircle.Domain.Enum.GlobalEnum;

namespace PMS.TopicCircle.Repository
{
    public class TopicTagRepository : Repository<TopicTag, TopicCircleDBContext>, ITopicTagRepository
    {
        TopicCircleDBContext _dbContext;
        public TopicTagRepository(TopicCircleDBContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }

        public int AddOrUpdate(IEnumerable<TopicTag> topicTags)
        {
            var sql = $@" 
Merge into TopicTag T1
using(
    select @TopicId as TopicId, @TagId as TagId
) T2 on T1.TopicId = T2.TopicId and T1.TagId = T2.TagId
when matched then update
    set TopicId = @TopicId, TagId = @TagId
when not matched then insert
    (TopicId,TagId) Values(@TopicId, @TagId)
;
";
            //insert into TopicReplyImage(Id, TopicReplyId, Url, Sort) Values(@Id, @TopicReplyId, @Url, @Sort)
            return _dbContext.ExecuteUow(sql, topicTags);
        }

        public int Delete(IEnumerable<TopicTag> topicTags)
        {
            var sql = $@" delete from TopicTag where TopicId = @TopicId and TagId = @TagId ";
            return _dbContext.ExecuteUow(sql, topicTags);
        }

        public IEnumerable<TopicTag> GetList(IEnumerable<Guid> topicIds)
        {
            var sql = $@" select * from TopicTag where TopicId in @topicIds ";
            return _dbContext.Query<TopicTag>(sql, new { topicIds });
        }

        public IEnumerable<SimpleTagDto> GetTags()
        {
            var sql = $@" 
select
    T.*,
    Tp.Name as ParentName
from
    Tag T
    left join Tag TP on TP.Id = T.ParentId
where
    T.IsDeleted = 0
    AND T.ParentId != 0
";
            return _dbContext.Query<SimpleTagDto>(sql, new { });
        }
    }
}
