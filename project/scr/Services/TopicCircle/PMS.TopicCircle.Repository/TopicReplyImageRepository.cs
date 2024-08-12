using PMS.TopicCircle.Domain.Entities;
using PMS.TopicCircle.Domain.Repositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Text;
using static PMS.TopicCircle.Domain.Enum.GlobalEnum;

namespace PMS.TopicCircle.Repository
{
    public class TopicReplyImageRepository : Repository<TopicReplyImage, TopicCircleDBContext>, ITopicReplyImageRepository
    {
        TopicCircleDBContext _dbContext;
        public TopicReplyImageRepository(TopicCircleDBContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }

        public int AddOrUpdate(IEnumerable<TopicReplyImage> images)
        {
            var sql = $@" 
Merge into TopicReplyImage T1
using(
    select @Id as Id
) T2 on T1.Id = T2.Id
when matched then update
    set TopicReplyId = @TopicReplyId, Url = @Url, Sort = @Sort
when not matched then insert
    (Id, TopicReplyId, Url, Sort) Values(@Id, @TopicReplyId, @Url, @Sort)
;
";
            //insert into TopicReplyImage(Id, TopicReplyId, Url, Sort) Values(@Id, @TopicReplyId, @Url, @Sort)
            return _dbContext.ExecuteUow(sql, images);
        }

        public int Delete(IEnumerable<TopicReplyImage> images)
        {
            var sql = $@" update TopicReplyImage set IsDeleted = 1 where Id = @Id ";
            return _dbContext.ExecuteUow(sql, images);
        }

        public IEnumerable<TopicReplyImage> GetList(IEnumerable<Guid> topicReplyIds)
        {
            var sql = $@" select * from TopicReplyImage where IsDeleted = 0 and TopicReplyId in @topicReplyIds ";
            return _dbContext.Query<TopicReplyImage>(sql, new { topicReplyIds });
        }
    }
}
