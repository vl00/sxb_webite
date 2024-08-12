using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.IRespositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMS.OperationPlateform.Repository
{
   public class Article_TalentRepository:BaseRepository<Article_SchoolBind>, IArticle_SchoolBindRepository
    {
        public Article_TalentRepository(OperationDBContext db) : base(db)
        {
        }


        public IEnumerable<Guid> GetArticleIds(Guid[] sids)
        {
            string sql = "select  ArticleId from Article_SchoolBind  where SchoolId in @sids group by ArticleId";
            var result = _db.Query<Guid>(sql, new { sids });
            return result;

        }
    }
}
