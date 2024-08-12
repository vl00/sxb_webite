using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Repository
{
    using PMS.OperationPlateform.Domain.Entitys;
    using PMS.OperationPlateform.Domain.IRespositories;
    using ProductManagement.Framework.MSSQLAccessor.DBContext;

    public class ArticleCommentRepository :BaseRepository<comment> ,IArticleCommentRepository
    {
        public ArticleCommentRepository(OperationDBContext db) : base(db)
        {
        }

        public IEnumerable<dynamic> Statistics_CommentsCount(Guid[] forumIds)
        {
            string sql = @"SELECT forumID Id,COUNT(1) Count from comment
                WHERE FORUMID in @forumIds
                GROUP BY forumID ";

           return _db.Query(sql, new { forumIds });

        }
    }
}
