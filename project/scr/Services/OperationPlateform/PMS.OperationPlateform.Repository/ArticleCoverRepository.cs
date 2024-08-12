using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Repository
{
    using PMS.OperationPlateform.Domain.Entitys;
    using PMS.OperationPlateform.Domain.IRespositories;
    using ProductManagement.Framework.MSSQLAccessor.DBContext;

    public class ArticleCoverRepository : BaseQueryRepository<article_cover>, IArticleCoverRepository
    {
        public ArticleCoverRepository(OperationQueryDBContext db) : base(db)
        {
        }

        public IEnumerable<article_cover> GetCoversByIds(Guid[] articleIds)
        {
            //查询文章的背景图片
            string sql = @"SELECT * FROM article_cover WHERE articleID in @articleIds order by sortID asc ";
            return _db.Query<article_cover>(sql, new { articleIds });
        }
    }
}
