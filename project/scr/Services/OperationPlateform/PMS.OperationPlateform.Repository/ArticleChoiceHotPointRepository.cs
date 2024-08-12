using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Repository
{
    using Domain.Entitys;
    using Domain.IRespositories;
    using ProductManagement.Framework.MSSQLAccessor.DBContext;
    using System.Threading.Tasks;

    public class ArticleChoiceHotPointRepository : BaseRepository<ArticleChoiceHotPoint>, IArticleChoiceHotPointRepository
    {
        protected override OperationDBContext dbContext { get; }
        public ArticleChoiceHotPointRepository(OperationDBContext db)
        {
            dbContext = db;

        }

        public async Task<(IEnumerable<article> data, int total)> GetArticles(Guid id, int offset, int limit)
        {
            string sql = @"SELECT article.id,article.title,article.time,article.type,article.layout,article.overview,article.toTop,article.show,article.viewCount,article.viewCount_r,
article.No,article.AuthorType
FROM ArticleChoiceHotPointBind
JOIN article ON ArticleChoiceHotPointBind.AId = article.id
WHERE 
ArticleChoiceHotPointBind.ACHPId = @achpId
ORDER BY ArticleChoiceHotPointBind.Sort ASC,ArticleChoiceHotPointBind.CreateTime ASC
OFFSET @offset ROW FETCH NEXT @limit ROWS ONLY;
SELECT  COUNT(1) FROM ArticleChoiceHotPointBind
WHERE ArticleChoiceHotPointBind.ACHPId = @achpId;

";
            using (var grid = await dbContext.QueryMultipleAsync(sql, new { achpId = id, offset, limit }))
            {
                IEnumerable<article> data = await grid.ReadAsync<article>();
                int total = await grid.ReadFirstAsync<int>();
                return (data, total);
            }

        }
    }
}
