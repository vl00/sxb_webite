using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMS.OperationPlateform.Repository
{
    using Dapper;
    using PMS.OperationPlateform.Domain.DTOs;
    using PMS.OperationPlateform.Domain.Entitys;
    using PMS.OperationPlateform.Domain.Enums;
    using PMS.OperationPlateform.Domain.IRespositories;
    using ProductManagement.Framework.MSSQLAccessor.DBContext;
    using System.Threading.Tasks;

    public class ArticleCommandRepository : IArticleCommandRepository
    {
        protected OperationCommandDBContext _db;

        public ArticleCommandRepository(OperationCommandDBContext dBContext)
        {
            this._db = dBContext;
        }

        
        public bool AddViewCount(Guid articleId, int viewCount)
        {
            string sql = @"UPDATE article set viewCount=viewCount+1  WHERE id=@id ";

            return _db.Execute(sql, new { id = articleId }) > 0;
        }

        public bool AddArticleSubscribeInfo(Article_SubscribePreference article_SubscribePreference)
        {
            return this._db.Insert(article_SubscribePreference) > 0;
        }

        public bool UpdateArticleSubscribeInfo(Article_SubscribePreference article_SubscribePreference)
        {
            return this._db.Update(entityToInsert: article_SubscribePreference, null, null);
        }
    }
}