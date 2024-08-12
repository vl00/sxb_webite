using PMS.TopicCircle.Domain.Entities;
using PMS.TopicCircle.Domain.Repositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Text;
using static PMS.TopicCircle.Domain.Enum.GlobalEnum;

namespace PMS.TopicCircle.Repository
{
    public class TopicReplyAttachmentRepository : Repository<TopicReplyAttachment, TopicCircleDBContext>, ITopicReplyAttachmentRepository
    {

        TopicCircleDBContext _dbContext;
        public TopicReplyAttachmentRepository(TopicCircleDBContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }

        public int AddOrUpdate(TopicReplyAttachment attachment)
        {
            var sql = $@"
Merge into TopicReplyAttachment T1
using(
    select @Id as Id
) T2 on T2.Id = T1.Id
when matched then update
    set Content = @Content, AttachId = @AttachId, AttachUrl = @AttachUrl, Type = @Type, UpdateTime = getdate() 
when not matched then insert
    (Id, TopicId, TopicReplyId, Content, AttachId, AttachUrl, Type) 
    Values
    (@Id, @TopicId, @TopicReplyId, @Content, @AttachId, @AttachUrl, @Type)
;
";
            return _dbContext.ExecuteUow(sql, attachment);
        }

        public int Add(TopicReplyAttachment attachment)
        {
            var sql = $@"
insert into TopicReplyAttachment(
    Id, TopicId, TopicReplyId, Content, AttachId, AttachUrl, Type
) 
Values(
    @Id, @TopicId, @TopicReplyId, @Content, @AttachId, @AttachUrl, @Type
)";
            return _dbContext.ExecuteUow(sql, attachment);
        }

        public int Delete(TopicReplyAttachment attachment)
        {
            var sql = $@" update TopicReplyAttachment set IsDeleted = 1, UpdateTime = getdate() where Id = @Id ";
            return _dbContext.ExecuteUow(sql, attachment);
        }

        public int Update(TopicReplyAttachment attachment)
        {
            var sql = $@" 
update TopicReplyAttachment 
set 
    Content = @Content
    , AttachId = @AttachId
    , AttachUrl = @AttachUrl
    , Type = @Type
    , UpdateTime = getdate() 
where 
    Id = @Id 
    and TopicId = @TopicId
    and TopicReplyId = @TopicReplyId 
";
            return _dbContext.ExecuteUow(sql, attachment);
        }

        public IEnumerable<TopicReplyAttachment> GetList(IEnumerable<Guid> topicReplyIds)
        {
            var sql = $@" select * from TopicReplyAttachment where IsDeleted = 0 and TopicReplyId in @topicReplyIds ";
            return _dbContext.Query<TopicReplyAttachment>(sql, new { topicReplyIds });
        }
    }
}
